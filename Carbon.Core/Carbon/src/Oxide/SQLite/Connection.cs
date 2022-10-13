///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Data.Common;
using System.Data.SQLite;
using Oxide.Plugins;

namespace Oxide.Ext.SQLite
{
	public sealed class Connection
	{
		internal string ConnectionString { get; set; }
		internal bool ConnectionPersistent { get; set; }
		internal SQLiteConnection Con { get; set; }
		internal Plugin Plugin { get; set; }
		public long LastInsertRowId { get; set; }

		public Connection(string connection, bool persistent)
		{
			ConnectionString = connection;
			ConnectionPersistent = persistent;
		}
	}
}
