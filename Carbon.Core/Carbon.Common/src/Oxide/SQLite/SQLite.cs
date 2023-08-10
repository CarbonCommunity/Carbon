using System.Data;
using Mono.Data.Sqlite;
using Logger = Carbon.Logger;

namespace Oxide.Core.SQLite.Libraries
{
	public class SQLite : Library, IDatabaseProvider
	{
		private readonly string _dataDirectory;

		private readonly Queue<SQLiteQuery> _queue = new Queue<SQLiteQuery>();
		private readonly object _syncroot = new object();
		private readonly AutoResetEvent _workevent = new AutoResetEvent(false);
		private readonly HashSet<Connection> _runningConnections = new HashSet<Connection>();
		private bool _running = true;
		private readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();
		private readonly Thread _worker;

		/// <summary>
		/// Represents a single MySqlQuery instance
		/// </summary>
		public class SQLiteQuery
		{
			/// <summary>
			/// Gets the callback delegate
			/// </summary>
			public Action<List<Dictionary<string, object>>> Callback { get; internal set; }

			/// <summary>
			/// Gets the callback delegate
			/// </summary>
			public Action<int> CallbackNonQuery { get; internal set; }

			/// <summary>
			/// Gets the sql
			/// </summary>
			public Sql Sql { get; internal set; }

			/// <summary>
			/// Gets the connection
			/// </summary>
			public Connection Connection { get; internal set; }

			/// <summary>
			/// Gets the non query
			/// </summary>
			public bool NonQuery { get; internal set; }

			private SqliteCommand _cmd;
			private SqliteConnection _connection;

			private void Cleanup()
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
					_cmd = null;
				}
				_connection = null;
			}

			public void Handle()
			{
				List<Dictionary<string, object>> list = null;
				int nonQueryResult = 0;
				long lastInsertRowId = 0L;
				try
				{
					if (Connection == null)
					{
						throw new Exception("Connection is null");
					}

					_connection = (SqliteConnection)Connection.Con;
					if (_connection.State == ConnectionState.Closed)
					{
						_connection.Open();
					}

					_cmd = _connection.CreateCommand();
					_cmd.CommandText = Sql.SQL;
					Sql.AddParams(_cmd, Sql.Arguments, "@");
					if (NonQuery)
					{
						nonQueryResult = _cmd.ExecuteNonQuery();
					}
					else
					{
						using (SqliteDataReader reader = _cmd.ExecuteReader())
						{
							list = new List<Dictionary<string, object>>();
							while (reader.Read())
							{
								Dictionary<string, object> dict = new Dictionary<string, object>();
								for (int i = 0; i < reader.FieldCount; i++)
								{
									dict.Add(reader.GetName(i), reader.GetValue(i));
								}
								list.Add(dict);
							}
						}
					}

					lastInsertRowId = GetLastInsertRowId(_connection);
					Cleanup();
				}
				catch (Exception ex)
				{
					string message = "Sqlite handle raised an exception";
					if (Connection?.Plugin != null)
					{
						message += $" in '{Connection.Plugin.Name} v{Connection.Plugin.Version}' plugin";
					}

					Logger.Error(message, ex);
					Cleanup();
				}
				Interface.Oxide.NextTick(() =>
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
					catch (Exception ex)
					{
						string message = "Sqlite command callback raised an exception";
						if (Connection?.Plugin != null)
						{
							message += $" in '{Connection.Plugin.Name} v{Connection.Plugin.Version}' plugin";
						}

						Logger.Error(message, ex);
					}
					Connection?.Plugin?.TrackEnd();
				});
			}

			private long GetLastInsertRowId(SqliteConnection connection)
			{
				using (SqliteCommand command = new SqliteCommand("SELECT last_insert_rowid()", connection))
				{
					return (long)command.ExecuteScalar();
				}
			}
		}

		/// <summary>
		/// The worker thread method
		/// </summary>
		private void Worker()
		{
			while (_running || _queue.Count > 0)
			{
				SQLiteQuery query = null;
				lock (_syncroot)
				{
					if (_queue.Count > 0)
					{
						query = _queue.Dequeue();
					}
					else
					{
						foreach (Connection connection in _runningConnections)
						{
							if (connection != null && !connection.ConnectionPersistent)
							{
								CloseDb(connection);
							}
						}

						_runningConnections.Clear();
					}
				}
				if (query != null)
				{
					query.Handle();
					if (query.Connection != null)
					{
						_runningConnections.Add(query.Connection);
					}
				}
				else if (_running)
				{
					_workevent.WaitOne();
				}
			}
		}

		public SQLite()
		{
			_dataDirectory = Interface.Oxide.DataDirectory;
			_worker = new Thread(Worker);
			_worker.Start();
		}

		public Connection OpenDb(string file, Plugin plugin, bool persistent = false)
		{
			if (string.IsNullOrEmpty(file))
			{
				return null;
			}

			string filename = Path.Combine(_dataDirectory, file);
			if (!filename.StartsWith(_dataDirectory, StringComparison.Ordinal))
			{
				throw new Exception("Only access to Carbon directory!");
			}

			string conStr = $"Data Source={filename};Version=3;";
			Connection connection;
			if (_connections.TryGetValue(conStr, out connection))
			{
				if (plugin != connection.Plugin)
				{
					Logger.Warn($"Already open connection ({conStr}), by plugin '{connection.Plugin}'...");
					return null;
				}

				Logger.Warn($"Already open connection ({conStr}), using existing instead...");
			}
			else
			{
				connection = new Connection(conStr, persistent)
				{
					Plugin = plugin,
					Con = new SqliteConnection(conStr)
				};
				_connections[conStr] = connection;
			}

			return connection;
		}

		private void OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			List<string> toRemove = new List<string>();
			foreach (KeyValuePair<string, Connection> connection in _connections)
			{
				if (connection.Value.Plugin != sender)
				{
					continue;
				}

				if (connection.Value.Con?.State != ConnectionState.Closed)
				{
					Logger.Warn($"Unclosed sqlite connection ({connection.Value.ConnectionString}), by plugin '{(connection.Value.Plugin?.Name ?? "null")}', closing...");
				}

				connection.Value.Con?.Close();
				connection.Value.Plugin = null;
				toRemove.Add(connection.Key);
			}
			foreach (string conStr in toRemove)
			{
				_connections.Remove(conStr);
			}
		}

		public void CloseDb(Connection db)
		{
			if (db == null)
			{
				return;
			}

			_connections.Remove(db.ConnectionString);

			db.Con?.Close();
			db.Plugin = null;
		}

		public Sql NewSql()
		{
			return Sql.Builder;
		}

		public void Query(Sql sql, Connection db, Action<List<Dictionary<string, object>>> callback)
		{
			SQLiteQuery query = new SQLiteQuery
			{
				Sql = sql,
				Connection = db,
				Callback = callback
			};
			lock (_syncroot)
			{
				_queue.Enqueue(query);
			}

			_workevent.Set();
		}

		public void ExecuteNonQuery(Sql sql, Connection db, Action<int> callback = null)
		{
			SQLiteQuery query = new SQLiteQuery
			{
				Sql = sql,
				Connection = db,
				CallbackNonQuery = callback,
				NonQuery = true
			};
			lock (_syncroot)
			{
				_queue.Enqueue(query);
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

		public void Shutdown()
		{
			_running = false;
			_workevent.Set();
			_worker.Join();
		}
	}
}
