using MongoDB.Driver;
using Proyecto_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Services
{
    public class MessageService
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageService(IMessagesDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _messages = database.GetCollection<Message>(settings.MessagesCollectionName);
        }

        //public List<Message> Get()
        //{
        //    return _messages.Find(message => true).ToList();
        //}

        //public Message Get(string username)
        //{
        //    return _messages.Find<Message>(message => message.userSender == username).FirstOrDefault();
        //}

        public Message Create(Message message)
        {
            _messages.InsertOne(message);
            return message;
        }

        public List<Message> Get(string username, Friend friendIn)
        {
            return _messages.Find(message => message.userSender == friendIn.userName && message.userRecipient == friendIn.userFriend && message.Original == true).ToList();
        }

        public void Remove(Friend friendIn)
        {
            _messages.DeleteMany(message => message.userSender == friendIn.userName && message.userRecipient == friendIn.userFriend && message.Original == true);
            _messages.DeleteMany(message => message.userSender == friendIn.userFriend && message.userRecipient == friendIn.userName && message.Original == false);
        }

        //public void Remove(string username)
        //{
        //    _users.DeleteOne(user => user.userName == username);
        //}
    }
}
