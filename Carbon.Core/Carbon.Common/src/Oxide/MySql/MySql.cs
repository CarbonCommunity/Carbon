using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Logger = Carbon.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.MySql.Libraries;

public class MySql : Library, IDatabaseProvider
{
	public MySql()
	{
		_worker = new Thread(new ThreadStart(Worker));
		_worker.Start();
	}

	private void Worker()
	{
		while (_running || _queue.Count > 0)
		{
			var mySqlQuery = (MySqlQuery)null;
			var syncroot = _syncroot;

			lock (syncroot)
			{
				if (_queue.Count > 0)
				{
					mySqlQuery = _queue.Dequeue();
				}
				else
				{
					foreach (var connection in _runningConnections)
					{
						if (connection != null && !connection.ConnectionPersistent)
						{
							CloseDb(connection);
						}
					}

					_runningConnections.Clear();
				}
			}

			if (mySqlQuery != null)
			{
				mySqlQuery.Handle();
				if (mySqlQuery.Connection != null)
				{
					_runningConnections.Add(mySqlQuery.Connection);
				}
			}
			else if (_running)
			{
				_workevent.WaitOne();
			}
		}
	}

	public Connection OpenDb(string host, int port, string database, string user, string password, Plugin plugin, bool persistent = false)
		=> OpenDb($"Server={host};Port={port};Database={database};User={user};Password={password};Pooling=false;default command timeout=120;Allow Zero Datetime=true;", plugin, persistent);

	public Connection OpenDb(string conStr, Plugin plugin, bool persistent = false)
	{
		if (!_connections.TryGetValue((plugin?.Name) ?? "null", out var dictionary))
		{
			dictionary = _connections[(plugin?.Name) ?? "null"] = new Dictionary<string, Connection>();
		}
		if (dictionary.TryGetValue(conStr, out var connection))
		{
			DbConnection con = connection.Con;
			Logger.Warn($"Already open connection ({con?.ConnectionString}), using existing instead...");
		}
		else
		{
			connection = new Connection(conStr, persistent)
			{
				Plugin = plugin,
				Con = new MySqlConnection(conStr)
			};
			dictionary[conStr] = connection;
		}
		return connection;
	}

	public void CloseDb(Connection db)
	{
		if (db == null) return;

		var connections = _connections;
		var plugin = db.Plugin;

		if (connections.TryGetValue((plugin?.Name) ?? "null", out var dictionary))
		{
			dictionary.Remove(db.ConnectionString);
			if (dictionary.Count == 0)
			{
				var connections2 = _connections;
				var plugin2 = db.Plugin;
				connections2.Remove((plugin2?.Name) ?? "null");
			}
		}

		db.Con?.Close();
		db.Plugin = null;
	}

	public Sql NewSql()
	{
		return Sql.Builder;
	}

	public void Query(Sql sql, Connection db, Action<List<Dictionary<string, object>>> callback)
	{
		var item = new MySql.MySqlQuery
		{
			Sql = sql,
			Connection = db,
			Callback = callback
		};

		var syncroot = _syncroot;
		lock (syncroot)
		{
			_queue.Enqueue(item);
		}
		_workevent.Set();
	}
	public void ExecuteNonQuery(Sql sql, Connection db, Action<int> callback = null)
	{
		var item = new MySql.MySqlQuery
		{
			Sql = sql,
			Connection = db,
			CallbackNonQuery = callback,
			NonQuery = true
		};
		object syncroot = _syncroot;
		lock (syncroot)
		{
			_queue.Enqueue(item);
		}
		_workevent.Set();
	}
	public void Insert(Sql sql, Connection db, Action<int> callback = null)
	{
		ExecuteNonQuery(sql, db, callback);
	}
	public void Update(Sql sql, Connection db, Action<int> callback = null)
	{
		ExecuteNonQuery(sql, db, callback);
	}
	public void Delete(Sql sql, Connection db, Action<int> callback = null)
	{
		ExecuteNonQuery(sql, db, callback);
	}

	public override void Dispose()
	{
		_running = false;
		_workevent.Set();
		_worker.Join();
	}

	private readonly Queue<MySql.MySqlQuery> _queue = new();
	private readonly object _syncroot = new();
	private readonly AutoResetEvent _workevent = new(false);
	private readonly HashSet<Connection> _runningConnections = new();

	private bool _running = true;

	private readonly Dictionary<string, Dictionary<string, Connection>> _connections = new();
	private readonly Thread _worker;

	public class MySqlQuery
	{
		internal object _cmdSource;
		internal object _connectionSource;
		internal IAsyncResult _result;

		internal MySqlCommand _cmd()
		{
			return _cmdSource as MySqlCommand;
		}
		internal MySqlConnection _connection()
		{
			return _connectionSource as MySqlConnection;
		}

		public Action<List<Dictionary<string, object>>> Callback { get; internal set; }

		public Action<int> CallbackNonQuery { get; internal set; }
		public Sql Sql { get; internal set; }
		public Connection Connection { get; internal set; }
		public bool NonQuery { get; internal set; }

		private void Cleanup()
		{
			if (_cmd() != null)
			{
				_cmd().Dispose();
				_cmdSource = null;
			}
			_connectionSource = null;
		}
		public bool Handle()
		{
			var list = (List<Dictionary<string, object>>)null;
			var nonQueryResult = 0;
			var lastInsertRowId = 0L;

			try
			{
				if (Connection == null)
				{
					throw new Exception("Connection is null");
				}

				_connectionSource = (MySqlConnection)Connection.Con;
				if (_connection().State == ConnectionState.Closed)
				{
					_connection().Open();
				}
				_cmdSource = _connection().CreateCommand();
				_cmd().CommandTimeout = 120;
				_cmd().CommandText = Sql.SQL;
				Sql.AddParams(_cmd(), Sql.Arguments, "@");
				_result = (NonQuery ? _cmd().BeginExecuteNonQuery() : _cmd().BeginExecuteReader());
				_result.AsyncWaitHandle.WaitOne();
				if (NonQuery)
				{
					nonQueryResult = _cmd().EndExecuteNonQuery(_result);
				}
				else
				{
					using var mySqlDataReader = _cmd().EndExecuteReader(_result);
					list = new List<Dictionary<string, object>>();
					while (mySqlDataReader.Read() && (!Connection.ConnectionPersistent || (Connection.Con.State != ConnectionState.Closed && Connection.Con.State != ConnectionState.Broken)))
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						for (int i = 0; i < mySqlDataReader.FieldCount; i++)
						{
							dictionary.Add(mySqlDataReader.GetName(i), mySqlDataReader.GetValue(i));
						}
						list.Add(dictionary);
					}
				}
				lastInsertRowId = _cmd().LastInsertedId;
				Cleanup();
			}
			catch (Exception ex)
			{
				var text = "MySql handle raised an exception";
				var connection = Connection;

				if ((connection?.Plugin) != null)
				{
					text += string.Format(" in '{0} v{1}' plugin", Connection.Plugin.Name, Connection.Plugin.Version);
				}

				Logger.Error(text, ex);
				Cleanup();
			}
			Interface.Oxide.NextTick(delegate
			{
				Connection?.Plugin?.TrackStart();
				try
				{
					if (Connection != null)
					{
						Connection.LastInsertRowId = lastInsertRowId;
					}
					if (!NonQuery)
					{
						Callback(list);
					}
					else
					{
						CallbackNonQuery?.Invoke(nonQueryResult);
					}
				}
				catch (Exception ex2)
				{
					var text2 = "MySql command callback raised an exception";
					if ((Connection?.Plugin) != null)
					{
						text2 += $" in '{Connection.Plugin.Name} v{Connection.Plugin.Version}' plugin";
					}

					Logger.Error(text2, ex2);
				}

				if (Connection == null || Connection.Plugin == null) return;

				Connection.Plugin.TrackEnd();
			});
			return true;
		}
	}
}
