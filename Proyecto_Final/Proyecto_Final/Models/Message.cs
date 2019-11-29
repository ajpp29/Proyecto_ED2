using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Final.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Sender")]
        public string userSender { get; set; }

        [BsonElement("Recipient")]
        public string userRecipient { get; set; }

        [BsonElement("Message")]
        public string messageSent { get; set; }

        [BsonElement("DateTime")]
        public DateTime dateTime { get; set; }

        [BsonElement("Original")]
        public bool Original { get; set; }
    }
}
