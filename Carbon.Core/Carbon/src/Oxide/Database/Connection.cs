///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Data.Common;
using Oxide.Core.Plugins;
using Oxide.Plugins;

namespace Oxide.Core.Database
{
	public sealed class Connection
	{
		public string ConnectionString { get; set; }
		public bool ConnectionPersistent { get; set; }
		public DbConnection Con { get; set; }
		public Plugin Plugin { get; set; }
		public long LastInsertRowId { get; set; }

		public Connection(string connection, bool persistent)
		{
			ConnectionString = connection;
			ConnectionPersistent = persistent;
		}
	}
}
