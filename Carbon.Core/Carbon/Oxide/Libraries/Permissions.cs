using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oxide.Core.Plugins;
using Carbon.Core.Harmony;
using Carbon.Core;

namespace Oxide.Core.Libraries
{
    public class Permission
    {
        public bool IsGlobal
        {
            get
            {
                return false;
            }
        }

        public bool IsLoaded { get; private set; }

        public Permission ()
        {
            this.permset = new Dictionary<Plugin, HashSet<string>> ();
            this.LoadFromDatafile ();
        }

        private void LoadFromDatafile ()
        {
            Utility.DatafileToProto<Dictionary<string, UserData>> ( "oxide.users", true );
            Utility.DatafileToProto<Dictionary<string, GroupData>> ( "oxide.groups", true );
            this.userdata = ( ProtoStorage.Load<Dictionary<string, UserData>> ( new string []
            {
                "oxide.users"
            } ) ?? new Dictionary<string, UserData> () );
            this.groupdata = ( ProtoStorage.Load<Dictionary<string, GroupData>> ( new string []
            {
                "oxide.groups"
            } ) ?? new Dictionary<string, GroupData> () );
            foreach ( KeyValuePair<string, GroupData> keyValuePair in this.groupdata )
            {
                if ( !string.IsNullOrEmpty ( keyValuePair.Value.ParentGroup ) && this.HasCircularParent ( keyValuePair.Key, keyValuePair.Value.ParentGroup ) )
                {
                    CarbonCore.WarnFormat ( "Detected circular parent group for '{0}'! Removing parent '{1}'", keyValuePair.Key, keyValuePair.Value.ParentGroup );
                    keyValuePair.Value.ParentGroup = null;
                }
            }
            this.IsLoaded = true;
        }

        public void Export ( string prefix = "auth" )
        {
            if ( !this.IsLoaded )
            {
                return;
            }
            Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, GroupData>> ( prefix + ".groups", this.groupdata, false );
            Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, UserData>> ( prefix + ".users", this.userdata, false );
        }

        public void SaveData ()
        {
            this.SaveUsers ();
            this.SaveGroups ();
        }

        public void SaveUsers ()
        {
            ProtoStorage.Save ( this.userdata, new string []
            {
                "oxide.users"
            } );
        }

        public void SaveGroups ()
        {
            ProtoStorage.Save ( this.groupdata, new string []
            {
                "oxide.groups"
            } );
        }

        public void RegisterValidate ( Func<string, bool> val )
        {
            this.validate = val;
        }

        public void CleanUp ()
        {
            if ( !this.IsLoaded || this.validate == null )
            {
                return;
            }
            string [] array = ( from k in this.userdata.Keys
                                where !this.validate ( k )
                                select k ).ToArray<string> ();
            if ( array.Length == 0 )
            {
                return;
            }
            foreach ( string key in array )
            {
                this.userdata.Remove ( key );
            }
        }

        public void MigrateGroup ( string oldGroup, string newGroup )
        {
            if ( !this.IsLoaded )
            {
                return;
            }
            if ( this.GroupExists ( oldGroup ) )
            {
                string fileDataPath = ProtoStorage.GetFileDataPath ( "oxide.groups.data" );
                File.Copy ( fileDataPath, fileDataPath + ".old", true );
                foreach ( string perm in this.GetGroupPermissions ( oldGroup, false ) )
                {
                    this.GrantGroupPermission ( newGroup, perm, null );
                }
                if ( this.GetUsersInGroup ( oldGroup ).Length == 0 )
                {
                    this.RemoveGroup ( oldGroup );
                }
            }
        }

        public void RegisterPermission ( string name, Plugin owner )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                return;
            }
            name = name.ToLower ();
            if ( this.PermissionExists ( name, null ) )
            {
                CarbonCore.WarnFormat ( "Duplicate permission registered '{0}' (by plugin '{1}')", name, owner.Name );
                return;
            }
            HashSet<string> hashSet;
            if ( !this.permset.TryGetValue ( owner, out hashSet ) )
            {
                hashSet = new HashSet<string> ();
                this.permset.Add ( owner, hashSet );
            }
            hashSet.Add ( name );
            Interface.CallHook ( "OnPermissionRegistered", name, owner );
            string text = owner.Name.ToLower () + ".";
            if ( !name.StartsWith ( text ) && !owner.IsCorePlugin )
            {
                CarbonCore.WarnFormat ( "Missing plugin name prefix '{0}' for permission '{1}' (by plugin '{2}')", text, name, owner.Name );
            }
        }

        public bool PermissionExists ( string name, Plugin owner = null )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                return false;
            }
            name = name.ToLower ();
            if ( owner == null )
            {
                if ( this.permset.Count > 0 )
                {
                    if ( name.Equals ( "*" ) )
                    {
                        return true;
                    }
                    if ( name.EndsWith ( "*" ) )
                    {
                        name = name.TrimEnd ( new char []
                        {
                            '*'
                        } );
                        return this.permset.Values.SelectMany ( ( HashSet<string> v ) => v ).Any ( ( string p ) => p.StartsWith ( name ) );
                    }
                }
                return this.permset.Values.Any ( ( HashSet<string> v ) => v.Contains ( name ) );
            }
            HashSet<string> hashSet;
            if ( !this.permset.TryGetValue ( owner, out hashSet ) )
            {
                return false;
            }
            if ( hashSet.Count > 0 )
            {
                if ( name.Equals ( "*" ) )
                {
                    return true;
                }
                if ( name.EndsWith ( "*" ) )
                {
                    name = name.TrimEnd ( new char []
                    {
                        '*'
                    } );
                    return hashSet.Any ( ( string p ) => p.StartsWith ( name ) );
                }
            }
            return hashSet.Contains ( name );
        }

        public bool UserIdValid ( string id )
        {
            return this.validate == null || this.validate ( id );
        }

        public bool UserExists ( string id )
        {
            return this.userdata.ContainsKey ( id );
        }

        public UserData GetUserData ( string id )
        {
            UserData result;
            if ( !this.userdata.TryGetValue ( id, out result ) )
            {
                this.userdata.Add ( id, result = new UserData () );
            }
            return result;
        }

        public void UpdateNickname ( string id, string nickname )
        {
            if ( this.UserExists ( id ) )
            {
                UserData userData = this.GetUserData ( id );
                string lastSeenNickname = userData.LastSeenNickname;
                string obj = nickname.Sanitize ();
                userData.LastSeenNickname = nickname.Sanitize ();
                Interface.CallHook ( "OnUserNameUpdated", id, lastSeenNickname, obj );
            }
        }

        public bool UserHasAnyGroup ( string id )
        {
            return this.UserExists ( id ) && this.GetUserData ( id ).Groups.Count > 0;
        }

        public bool GroupsHavePermission ( HashSet<string> groups, string perm )
        {
            return groups.Any ( ( string group ) => this.GroupHasPermission ( group, perm ) );
        }

        public bool GroupHasPermission ( string name, string perm )
        {
            GroupData groupData;
            return this.GroupExists ( name ) && !string.IsNullOrEmpty ( perm ) && this.groupdata.TryGetValue ( name.ToLower (), out groupData ) && ( groupData.Perms.Contains ( perm.ToLower () ) || this.GroupHasPermission ( groupData.ParentGroup, perm ) );
        }

        public bool UserHasPermission ( string id, string perm )
        {
            if ( string.IsNullOrEmpty ( perm ) )
            {
                return false;
            }
            if ( id.Equals ( "server_console" ) )
            {
                return true;
            }
            perm = perm.ToLower ();
            UserData userData = this.GetUserData ( id );
            return userData.Perms.Contains ( perm ) || this.GroupsHavePermission ( userData.Groups, perm );
        }

        public string [] GetUserGroups ( string id )
        {
            return this.GetUserData ( id ).Groups.ToArray<string> ();
        }

        public string [] GetUserPermissions ( string id )
        {
            UserData userData = this.GetUserData ( id );
            List<string> list = userData.Perms.ToList<string> ();
            foreach ( string name in userData.Groups )
            {
                list.AddRange ( this.GetGroupPermissions ( name, false ) );
            }
            return new HashSet<string> ( list ).ToArray<string> ();
        }

        public string [] GetGroupPermissions ( string name, bool parents = false )
        {
            if ( !this.GroupExists ( name ) )
            {
                return new string [ 0 ];
            }
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( name.ToLower (), out groupData ) )
            {
                return new string [ 0 ];
            }
            List<string> list = groupData.Perms.ToList<string> ();
            if ( parents )
            {
                list.AddRange ( this.GetGroupPermissions ( groupData.ParentGroup, false ) );
            }
            return new HashSet<string> ( list ).ToArray<string> ();
        }

        public string [] GetPermissions ()
        {
            return new HashSet<string> ( this.permset.Values.SelectMany ( ( HashSet<string> v ) => v ) ).ToArray<string> ();
        }

        public string [] GetPermissionUsers ( string perm )
        {
            if ( string.IsNullOrEmpty ( perm ) )
            {
                return new string [ 0 ];
            }
            perm = perm.ToLower ();
            HashSet<string> hashSet = new HashSet<string> ();
            foreach ( KeyValuePair<string, UserData> keyValuePair in this.userdata )
            {
                if ( keyValuePair.Value.Perms.Contains ( perm ) )
                {
                    hashSet.Add ( keyValuePair.Key + "(" + keyValuePair.Value.LastSeenNickname + ")" );
                }
            }
            return hashSet.ToArray<string> ();
        }

        public string [] GetPermissionGroups ( string perm )
        {
            if ( string.IsNullOrEmpty ( perm ) )
            {
                return new string [ 0 ];
            }
            perm = perm.ToLower ();
            HashSet<string> hashSet = new HashSet<string> ();
            foreach ( KeyValuePair<string, GroupData> keyValuePair in this.groupdata )
            {
                if ( keyValuePair.Value.Perms.Contains ( perm ) )
                {
                    hashSet.Add ( keyValuePair.Key );
                }
            }
            return hashSet.ToArray<string> ();
        }

        public void AddUserGroup ( string id, string name )
        {
            if ( !this.GroupExists ( name ) )
            {
                return;
            }
            if ( !this.GetUserData ( id ).Groups.Add ( name.ToLower () ) )
            {
                return;
            }
            HookExecutor.CallStaticHook ( "OnUserGroupAdded", new object []
            {
                id,
                name
            } );
        }

        public void RemoveUserGroup ( string id, string name )
        {
            if ( !this.GroupExists ( name ) )
            {
                return;
            }
            UserData userData = this.GetUserData ( id );
            if ( name.Equals ( "*" ) )
            {
                if ( userData.Groups.Count <= 0 )
                {
                    return;
                }
                userData.Groups.Clear ();
                return;
            }
            else
            {
                if ( !userData.Groups.Remove ( name.ToLower () ) )
                {
                    return;
                }
                HookExecutor.CallStaticHook ( "OnUserGroupRemoved", new object []
                {
                    id,
                    name
                } );
                return;
            }
        }

        public bool UserHasGroup ( string id, string name )
        {
            return this.GroupExists ( name ) && this.GetUserData ( id ).Groups.Contains ( name.ToLower () );
        }

        public bool GroupExists ( string group )
        {
            return !string.IsNullOrEmpty ( group ) && ( group.Equals ( "*" ) || this.groupdata.ContainsKey ( group.ToLower () ) );
        }

        public string [] GetGroups ()
        {
            return this.groupdata.Keys.ToArray<string> ();
        }

        public string [] GetUsersInGroup ( string group )
        {
            if ( !this.GroupExists ( group ) )
            {
                return new string [ 0 ];
            }
            group = group.ToLower ();
            return ( from u in this.userdata
                     where u.Value.Groups.Contains ( @group )
                     select u.Key + " (" + u.Value.LastSeenNickname + ")" ).ToArray<string> ();
        }

        public string GetGroupTitle ( string group )
        {
            if ( !this.GroupExists ( group ) )
            {
                return string.Empty;
            }
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( group.ToLower (), out groupData ) )
            {
                return string.Empty;
            }
            return groupData.Title;
        }

        public int GetGroupRank ( string group )
        {
            if ( !this.GroupExists ( group ) )
            {
                return 0;
            }
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( group.ToLower (), out groupData ) )
            {
                return 0;
            }
            return groupData.Rank;
        }

        public void GrantUserPermission ( string id, string perm, Plugin owner )
        {
            if ( !this.PermissionExists ( perm, owner ) )
            {
                return;
            }
            UserData data = this.GetUserData ( id );
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                HashSet<string> source;
                if ( owner == null )
                {
                    source = new HashSet<string> ( this.permset.Values.SelectMany ( ( HashSet<string> v ) => v ) );
                }
                else if ( !this.permset.TryGetValue ( owner, out source ) )
                {
                    return;
                }
                if ( perm.Equals ( "*" ) )
                {
                    source.Aggregate ( false, ( bool c, string s ) => c | data.Perms.Add ( s ) );
                    return;
                }
                perm = perm.TrimEnd ( new char []
                {
                    '*'
                } );
                ( from s in source
                  where s.StartsWith ( perm )
                  select s ).Aggregate ( false, ( bool c, string s ) => c | data.Perms.Add ( s ) );
                return;
            }
            else
            {
                if ( !data.Perms.Add ( perm ) )
                {
                    return;
                }
                HookExecutor.CallStaticHook ( "OnUserPermissionGranted", new object []
                {
                    id,
                    perm
                } );
                return;
            }
        }

        public void RevokeUserPermission ( string id, string perm )
        {
            if ( string.IsNullOrEmpty ( perm ) )
            {
                return;
            }
            UserData userData = this.GetUserData ( id );
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                if ( !perm.Equals ( "*" ) )
                {
                    perm = perm.TrimEnd ( new char []
                    {
                        '*'
                    } );
                    userData.Perms.RemoveWhere ( ( string s ) => s.StartsWith ( perm ) );
                    return;
                }
                if ( userData.Perms.Count <= 0 )
                {
                    return;
                }
                userData.Perms.Clear ();
                return;
            }
            else
            {
                if ( !userData.Perms.Remove ( perm ) )
                {
                    return;
                }
                HookExecutor.CallStaticHook ( "OnUserPermissionRevoked", new object []
                {
                    id,
                    perm
                } );
                return;
            }
        }

        public void GrantGroupPermission ( string name, string perm, Plugin owner )
        {
            if ( !this.PermissionExists ( perm, owner ) || !this.GroupExists ( name ) )
            {
                return;
            }
            GroupData data;
            if ( !this.groupdata.TryGetValue ( name.ToLower (), out data ) )
            {
                return;
            }
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                HashSet<string> source;
                if ( owner == null )
                {
                    source = new HashSet<string> ( this.permset.Values.SelectMany ( ( HashSet<string> v ) => v ) );
                }
                else if ( !this.permset.TryGetValue ( owner, out source ) )
                {
                    return;
                }
                if ( perm.Equals ( "*" ) )
                {
                    source.Aggregate ( false, ( bool c, string s ) => c | data.Perms.Add ( s ) );
                    return;
                }
                perm = perm.TrimEnd ( new char []
                {
                    '*'
                } ).ToLower ();
                ( from s in source
                  where s.StartsWith ( perm )
                  select s ).Aggregate ( false, ( bool c, string s ) => c | data.Perms.Add ( s ) );
                return;
            }
            else
            {
                if ( !data.Perms.Add ( perm ) )
                {
                    return;
                }
                HookExecutor.CallStaticHook ( "OnGroupPermissionGranted", new object []
                {
                    name,
                    perm
                } );
                return;
            }
        }

        public void RevokeGroupPermission ( string name, string perm )
        {
            if ( !this.GroupExists ( name ) || string.IsNullOrEmpty ( perm ) )
            {
                return;
            }
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( name.ToLower (), out groupData ) )
            {
                return;
            }
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                if ( !perm.Equals ( "*" ) )
                {
                    perm = perm.TrimEnd ( new char []
                    {
                        '*'
                    } ).ToLower ();
                    groupData.Perms.RemoveWhere ( ( string s ) => s.StartsWith ( perm ) );
                    return;
                }
                if ( groupData.Perms.Count <= 0 )
                {
                    return;
                }
                groupData.Perms.Clear ();
                return;
            }
            else
            {
                if ( !groupData.Perms.Remove ( perm ) )
                {
                    return;
                }
                HookExecutor.CallStaticHook ( "OnGroupPermissionRevoked", new object []
                {
                    name,
                    perm
                } );
                return;
            }
        }

        public bool CreateGroup ( string group, string title, int rank )
        {
            if ( this.GroupExists ( group ) || string.IsNullOrEmpty ( group ) )
            {
                return false;
            }
            GroupData value = new GroupData
            {
                Title = title,
                Rank = rank
            };
            group = group.ToLower ();
            this.groupdata.Add ( group, value );
            Interface.CallHook ( "OnGroupCreated", group, title, rank );
            return true;
        }

        public bool RemoveGroup ( string group )
        {
            if ( !this.GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            bool flag = this.groupdata.Remove ( group );
            if ( flag )
            {
                foreach ( GroupData groupData in this.groupdata.Values.Where ( x => x.ParentGroup == group ) )
                {
                    groupData.ParentGroup = string.Empty;
                }
            }
            if ( this.userdata.Values.Aggregate ( false, ( bool current, UserData userData ) => current | userData.Groups.Remove ( group ) ) )
            {
                this.SaveUsers ();
            }
            if ( flag )
            {
                Interface.CallHook ( "OnGroupDeleted", group );
            }
            return true;
        }

        public bool SetGroupTitle ( string group, string title )
        {
            if ( !this.GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( group, out groupData ) )
            {
                return false;
            }
            if ( groupData.Title == title )
            {
                return true;
            }
            groupData.Title = title;
            Interface.CallHook ( "OnGroupTitleSet", group, title );
            return true;
        }

        public bool SetGroupRank ( string group, int rank )
        {
            if ( !this.GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( group, out groupData ) )
            {
                return false;
            }
            if ( groupData.Rank == rank )
            {
                return true;
            }
            groupData.Rank = rank;
            Interface.CallHook ( "OnGroupRankSet", group, rank );
            return true;
        }

        public string GetGroupParent ( string group )
        {
            if ( !this.GroupExists ( group ) )
            {
                return string.Empty;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( this.groupdata.TryGetValue ( group, out groupData ) )
            {
                return groupData.ParentGroup;
            }
            return string.Empty;
        }

        public bool SetGroupParent ( string group, string parent )
        {
            if ( !this.GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( group, out groupData ) )
            {
                return false;
            }
            if ( string.IsNullOrEmpty ( parent ) )
            {
                groupData.ParentGroup = null;
                return true;
            }
            if ( !this.GroupExists ( parent ) || group.Equals ( parent.ToLower () ) )
            {
                return false;
            }
            parent = parent.ToLower ();
            if ( !string.IsNullOrEmpty ( groupData.ParentGroup ) && groupData.ParentGroup.Equals ( parent ) )
            {
                return true;
            }
            if ( this.HasCircularParent ( group, parent ) )
            {
                return false;
            }
            groupData.ParentGroup = parent;
            Interface.CallHook ( "OnGroupParentSet", group, parent );
            return true;
        }

        private bool HasCircularParent ( string group, string parent )
        {
            GroupData groupData;
            if ( !this.groupdata.TryGetValue ( parent, out groupData ) )
            {
                return false;
            }
            HashSet<string> hashSet = new HashSet<string>
            {
                group,
                parent
            };
            while ( !string.IsNullOrEmpty ( groupData.ParentGroup ) )
            {
                if ( !hashSet.Add ( groupData.ParentGroup ) )
                {
                    return true;
                }
                if ( !this.groupdata.TryGetValue ( groupData.ParentGroup, out groupData ) )
                {
                    return false;
                }
            }
            return false;
        }

        private readonly Dictionary<Plugin, HashSet<string>> permset;

        private Dictionary<string, UserData> userdata;

        private Dictionary<string, GroupData> groupdata;

        private Func<string, bool> validate;
    }
}
