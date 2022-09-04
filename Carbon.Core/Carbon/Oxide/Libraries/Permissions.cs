using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
            permset = new Dictionary<Plugin, HashSet<string>> ();
            LoadFromDatafile ();
        }

        private void LoadFromDatafile ()
        {
            Utility.DatafileToProto<Dictionary<string, UserData>> ( "oxide.users", true );
            Utility.DatafileToProto<Dictionary<string, GroupData>> ( "oxide.groups", true );
            userdata = ( ProtoStorage.Load<Dictionary<string, UserData>> ( new string []
            {
                "oxide.users"
            } ) ?? new Dictionary<string, UserData> () );
            groupdata = ( ProtoStorage.Load<Dictionary<string, GroupData>> ( new string []
            {
                "oxide.groups"
            } ) ?? new Dictionary<string, GroupData> () );
            foreach ( KeyValuePair<string, GroupData> keyValuePair in groupdata )
            {
                if ( !string.IsNullOrEmpty ( keyValuePair.Value.ParentGroup ) && HasCircularParent ( keyValuePair.Key, keyValuePair.Value.ParentGroup ) )
                {
                    CarbonCore.WarnFormat ( "Detected circular parent group for '{0}'! Removing parent '{1}'", keyValuePair.Key, keyValuePair.Value.ParentGroup );
                    keyValuePair.Value.ParentGroup = null;
                }
            }
            IsLoaded = true;
        }

        public void Export ( string prefix = "auth" )
        {
            if ( !IsLoaded )
            {
                return;
            }
            Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, GroupData>> ( prefix + ".groups", groupdata, false );
            Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, UserData>> ( prefix + ".users", userdata, false );
        }

        public void SaveData ()
        {
            SaveUsers ();
            SaveGroups ();
        }

        public void SaveUsers ()
        {
            ProtoStorage.Save ( userdata, new string []
            {
                "oxide.users"
            } );
        }

        public void SaveGroups ()
        {
            ProtoStorage.Save ( groupdata, new string []
            {
                "oxide.groups"
            } );
        }

        public void RegisterValidate ( Func<string, bool> val )
        {
            validate = val;
        }

        public void CleanUp ()
        {
            if ( !IsLoaded || validate == null )
            {
                return;
            }
            string [] array = ( from k in userdata.Keys
                                where !validate ( k )
                                select k ).ToArray<string> ();
            if ( array.Length == 0 )
            {
                return;
            }
            foreach ( string key in array )
            {
                userdata.Remove ( key );
            }
        }

        public void MigrateGroup ( string oldGroup, string newGroup )
        {
            if ( !IsLoaded )
            {
                return;
            }
            if ( GroupExists ( oldGroup ) )
            {
                string fileDataPath = ProtoStorage.GetFileDataPath ( "oxide.groups.data" );
                File.Copy ( fileDataPath, fileDataPath + ".old", true );
                foreach ( string perm in GetGroupPermissions ( oldGroup, false ) )
                {
                    GrantGroupPermission ( newGroup, perm, null );
                }
                if ( GetUsersInGroup ( oldGroup ).Length == 0 )
                {
                    RemoveGroup ( oldGroup );
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
            if ( PermissionExists ( name, null ) )
            {
                CarbonCore.WarnFormat ( "Duplicate permission registered '{0}' (by plugin '{1}')", name, owner.Name );
                return;
            }
            HashSet<string> hashSet;
            if ( !permset.TryGetValue ( owner, out hashSet ) )
            {
                hashSet = new HashSet<string> ();
                permset.Add ( owner, hashSet );
            }
            hashSet.Add ( name );
            Interface.CallHook ( "OnPermissionRegistered", name, owner );
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
                if ( permset.Count > 0 )
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
                        return permset.Values.SelectMany ( ( HashSet<string> v ) => v ).Any ( ( string p ) => p.StartsWith ( name ) );
                    }
                }
                return permset.Values.Any ( ( HashSet<string> v ) => v.Contains ( name ) );
            }
            HashSet<string> hashSet;
            if ( !permset.TryGetValue ( owner, out hashSet ) )
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
            return validate == null || validate ( id );
        }

        public bool UserExists ( string id )
        {
            return userdata.ContainsKey ( id );
        }

        public UserData GetUserData ( string id )
        {
            UserData result;
            if ( !userdata.TryGetValue ( id, out result ) )
            {
                userdata.Add ( id, result = new UserData () );
            }
            return result;
        }

        public void UpdateNickname ( string id, string nickname )
        {
            if ( UserExists ( id ) )
            {
                UserData userData = GetUserData ( id );
                string lastSeenNickname = userData.LastSeenNickname;
                string obj = nickname.Sanitize ();
                userData.LastSeenNickname = nickname.Sanitize ();
                Interface.CallHook ( "OnUserNameUpdated", id, lastSeenNickname, obj );
            }
        }

        public bool UserHasAnyGroup ( string id )
        {
            return UserExists ( id ) && GetUserData ( id ).Groups.Count > 0;
        }

        public bool GroupsHavePermission ( HashSet<string> groups, string perm )
        {
            return groups.Any ( ( string group ) => GroupHasPermission ( group, perm ) );
        }

        public bool GroupHasPermission ( string name, string perm )
        {
            GroupData groupData;
            return GroupExists ( name ) && !string.IsNullOrEmpty ( perm ) && groupdata.TryGetValue ( name.ToLower (), out groupData ) && ( groupData.Perms.Contains ( perm.ToLower () ) || GroupHasPermission ( groupData.ParentGroup, perm ) );
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
            UserData userData = GetUserData ( id );
            return userData.Perms.Contains ( perm ) || GroupsHavePermission ( userData.Groups, perm );
        }

        public string [] GetUserGroups ( string id )
        {
            return GetUserData ( id ).Groups.ToArray<string> ();
        }

        public string [] GetUserPermissions ( string id )
        {
            UserData userData = GetUserData ( id );
            List<string> list = userData.Perms.ToList<string> ();
            foreach ( string name in userData.Groups )
            {
                list.AddRange ( GetGroupPermissions ( name, false ) );
            }
            return new HashSet<string> ( list ).ToArray<string> ();
        }

        public string [] GetGroupPermissions ( string name, bool parents = false )
        {
            if ( !GroupExists ( name ) )
            {
                return new string [ 0 ];
            }
            GroupData groupData;
            if ( !groupdata.TryGetValue ( name.ToLower (), out groupData ) )
            {
                return new string [ 0 ];
            }
            List<string> list = groupData.Perms.ToList<string> ();
            if ( parents )
            {
                list.AddRange ( GetGroupPermissions ( groupData.ParentGroup, false ) );
            }
            return new HashSet<string> ( list ).ToArray<string> ();
        }

        public string [] GetPermissions ()
        {
            return new HashSet<string> ( permset.Values.SelectMany ( ( HashSet<string> v ) => v ) ).ToArray<string> ();
        }

        public string [] GetPermissionUsers ( string perm )
        {
            if ( string.IsNullOrEmpty ( perm ) )
            {
                return new string [ 0 ];
            }
            perm = perm.ToLower ();
            HashSet<string> hashSet = new HashSet<string> ();
            foreach ( KeyValuePair<string, UserData> keyValuePair in userdata )
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
            foreach ( KeyValuePair<string, GroupData> keyValuePair in groupdata )
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
            if ( !GroupExists ( name ) )
            {
                return;
            }
            if ( !GetUserData ( id ).Groups.Add ( name.ToLower () ) )
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
            if ( !GroupExists ( name ) )
            {
                return;
            }
            UserData userData = GetUserData ( id );
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
            return GroupExists ( name ) && GetUserData ( id ).Groups.Contains ( name.ToLower () );
        }

        public bool GroupExists ( string group )
        {
            return !string.IsNullOrEmpty ( group ) && ( group.Equals ( "*" ) || groupdata.ContainsKey ( group.ToLower () ) );
        }

        public string [] GetGroups ()
        {
            return groupdata.Keys.ToArray<string> ();
        }

        public string [] GetUsersInGroup ( string group )
        {
            if ( !GroupExists ( group ) )
            {
                return new string [ 0 ];
            }
            group = group.ToLower ();
            return ( from u in userdata
                     where u.Value.Groups.Contains ( @group )
                     select u.Key + " (" + u.Value.LastSeenNickname + ")" ).ToArray<string> ();
        }

        public string GetGroupTitle ( string group )
        {
            if ( !GroupExists ( group ) )
            {
                return string.Empty;
            }
            GroupData groupData;
            if ( !groupdata.TryGetValue ( group.ToLower (), out groupData ) )
            {
                return string.Empty;
            }
            return groupData.Title;
        }

        public int GetGroupRank ( string group )
        {
            if ( !GroupExists ( group ) )
            {
                return 0;
            }
            GroupData groupData;
            if ( !groupdata.TryGetValue ( group.ToLower (), out groupData ) )
            {
                return 0;
            }
            return groupData.Rank;
        }

        public void GrantUserPermission ( string id, string perm, Plugin owner )
        {
            if ( !PermissionExists ( perm, owner ) )
            {
                return;
            }
            UserData data = GetUserData ( id );
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                HashSet<string> source;
                if ( owner == null )
                {
                    source = new HashSet<string> ( permset.Values.SelectMany ( ( HashSet<string> v ) => v ) );
                }
                else if ( !permset.TryGetValue ( owner, out source ) )
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
            UserData userData = GetUserData ( id );
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
            if ( !PermissionExists ( perm, owner ) || !GroupExists ( name ) )
            {
                return;
            }
            GroupData data;
            if ( !groupdata.TryGetValue ( name.ToLower (), out data ) )
            {
                return;
            }
            perm = perm.ToLower ();
            if ( perm.EndsWith ( "*" ) )
            {
                HashSet<string> source;
                if ( owner == null )
                {
                    source = new HashSet<string> ( permset.Values.SelectMany ( ( HashSet<string> v ) => v ) );
                }
                else if ( !permset.TryGetValue ( owner, out source ) )
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
            if ( !GroupExists ( name ) || string.IsNullOrEmpty ( perm ) )
            {
                return;
            }
            GroupData groupData;
            if ( !groupdata.TryGetValue ( name.ToLower (), out groupData ) )
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
            if ( GroupExists ( group ) || string.IsNullOrEmpty ( group ) )
            {
                return false;
            }
            GroupData value = new GroupData
            {
                Title = title,
                Rank = rank
            };
            group = group.ToLower ();
            groupdata.Add ( group, value );
            Interface.CallHook ( "OnGroupCreated", group, title, rank );
            return true;
        }

        public bool RemoveGroup ( string group )
        {
            if ( !GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            bool flag = groupdata.Remove ( group );
            if ( flag )
            {
                foreach ( GroupData groupData in groupdata.Values.Where ( x => x.ParentGroup == group ) )
                {
                    groupData.ParentGroup = string.Empty;
                }
            }
            if ( userdata.Values.Aggregate ( false, ( bool current, UserData userData ) => current | userData.Groups.Remove ( group ) ) )
            {
                SaveUsers ();
            }
            if ( flag )
            {
                Interface.CallHook ( "OnGroupDeleted", group );
            }
            return true;
        }

        public bool SetGroupTitle ( string group, string title )
        {
            if ( !GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !groupdata.TryGetValue ( group, out groupData ) )
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
            if ( !GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !groupdata.TryGetValue ( group, out groupData ) )
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
            if ( !GroupExists ( group ) )
            {
                return string.Empty;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( groupdata.TryGetValue ( group, out groupData ) )
            {
                return groupData.ParentGroup;
            }
            return string.Empty;
        }

        public bool SetGroupParent ( string group, string parent )
        {
            if ( !GroupExists ( group ) )
            {
                return false;
            }
            group = group.ToLower ();
            GroupData groupData;
            if ( !groupdata.TryGetValue ( group, out groupData ) )
            {
                return false;
            }
            if ( string.IsNullOrEmpty ( parent ) )
            {
                groupData.ParentGroup = null;
                return true;
            }
            if ( !GroupExists ( parent ) || group.Equals ( parent.ToLower () ) )
            {
                return false;
            }
            parent = parent.ToLower ();
            if ( !string.IsNullOrEmpty ( groupData.ParentGroup ) && groupData.ParentGroup.Equals ( parent ) )
            {
                return true;
            }
            if ( HasCircularParent ( group, parent ) )
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
            if ( !groupdata.TryGetValue ( parent, out groupData ) )
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
                if ( !groupdata.TryGetValue ( groupData.ParentGroup, out groupData ) )
                {
                    return false;
                }
            }
            return false;
        }

        private readonly Dictionary<Plugin, HashSet<string>> permset;

        private Dictionary<string, UserData> userdata = new Dictionary<string, UserData> ();

        private Dictionary<string, GroupData> groupdata = new Dictionary<string, GroupData> ();

        private Func<string, bool> validate;
    }
}
