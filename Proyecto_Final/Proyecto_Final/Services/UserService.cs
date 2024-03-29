﻿using MongoDB.Driver;
using Proyecto_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IUsersDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<User> Get()
        {
            return _users.Find(user => true).ToList();
        }

        public User Get(string username)
        {
           return  _users.Find<User>(user => user.userName == username).FirstOrDefault();
        }

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string username, User userIn)
        {
            _users.ReplaceOne(user => user.userName == username, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.userName == userIn.userName);
        }

        public void Remove(string username)
        {
            _users.DeleteOne(user => user.userName == username);
        }
    }
}
