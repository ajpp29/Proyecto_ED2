using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Models
{
    public class Friend
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("User")]
        public string userName { get; set; }

        [BsonElement("Friend")]
        public string userFriend { get; set; }
    }
}
