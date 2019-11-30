using MongoDB.Driver;
using Proyecto_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Chat> _chats;

        public ChatService(IChatsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _chats = database.GetCollection<Chat>(settings.ChatsCollectionName);
        }

        public List<Chat> Get(string username)
        {
            return _chats.Find(chat => chat.userSender == username).ToList();
        }

        //public List<Friend> Get(Friend friends)
        //{
        //    return _friends.Find(friend => friend.userName == friends.userName && friend.userFriend == friends.userFriend).ToList();
        //}

        public Chat Create(Chat chat)
        {
            _chats.InsertOne(chat);
            return chat;
        }

        //public void Update(string username, Friend friendIn)
        //{
        //    _friends.ReplaceOne(friend => friend.userName == username && friend.userFriend==friend.userFriend, friendIn);
        //}

        public void Remove(Chat chatIn)
        {
            _chats.DeleteOne(chat => chat.userSender == chatIn.userSender && chat.userRecipient == chatIn.userRecipient);
        }

        //public void Remove(string username)
        //{
        //    _friends.DeleteMany(friend => friend.userName == username);
        //}
    }
}
