using MongoDB.Driver;
using Proyecto_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Services
{
    public class FriendService
    {
        private readonly IMongoCollection<Friend> _friends;

        public FriendService(IFriendsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _friends = database.GetCollection<Friend>(settings.FriendsCollectionName);
        }

        public List<Friend> Get(string username)
        {
            return _friends.Find(friend => friend.userName == username).ToList();
        }

        //public List<Friend> Get(Friend friends)
        //{
        //    return _friends.Find(friend => friend.userName == friends.userName && friend.userFriend == friends.userFriend).ToList();
        //}

        public Friend Create(Friend friend)
        {
            _friends.InsertOne(friend);
            return friend;
        }

        //public void Update(string username, Friend friendIn)
        //{
        //    _friends.ReplaceOne(friend => friend.userName == username && friend.userFriend==friend.userFriend, friendIn);
        //}

        public void Remove(Friend friendIn)
        {
            _friends.DeleteOne(friend => friend.userName == friendIn.userName && friend.userFriend == friendIn.userFriend);
        }

        //public void Remove(string username)
        //{
        //    _friends.DeleteMany(friend => friend.userName == username);
        //}
    }
}
