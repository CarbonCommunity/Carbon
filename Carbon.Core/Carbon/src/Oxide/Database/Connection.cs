///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Data.Common;
using Oxide.Core.Plugins;
using Oxide.Plugins;

namespace Oxide.Core.Database
{
	public class Connection
	{
		public string ConnectionString;
		public bool ConnectionPersistent;
		public DbConnection Con;
		public Plugin Plugin;
		public long LastInsertRowId;

		public Connection(string connection, bool persistent)
		{
			ConnectionString = connection;
			ConnectionPersistent = persistent;
		}
	}
}
