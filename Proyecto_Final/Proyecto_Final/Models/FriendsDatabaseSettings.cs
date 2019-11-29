using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Models
{
    public class FriendsDatabaseSettings: IFriendsDatabaseSettings
    {
        public string FriendsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IFriendsDatabaseSettings
    {
        string FriendsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
