///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Oxide.Core.Plugins;
using Oxide.Plugins;

namespace Oxide.Ext.MySql
{
	public sealed class Connection
	{
		internal string ConnectionString { get; set; }
		internal bool ConnectionPersistent { get; set; }
		internal MySqlConnection Con { get; set; }
		internal Plugin Plugin { get; set; }
		public long LastInsertRowId { get; set; }

		public Connection(string connection, bool persistent)
		{
			ConnectionString = connection;
			ConnectionPersistent = persistent;
		}
	}
}
