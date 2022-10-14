///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Data.Common;
using System.Data.SQLite;
using Oxide.Plugins;

namespace Oxide.Ext.SQLite
{
	public class Connection
	{
		internal string ConnectionString;
		internal bool ConnectionPersistent;
		internal object Con;
		internal Plugin Plugin;
		public long LastInsertRowId;

		public Connection(string connection, bool persistent)
		{
			ConnectionString = connection;
			ConnectionPersistent = persistent;
		}
	}
}
