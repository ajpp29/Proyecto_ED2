using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLogin.Models
{
    public class Chat
    {
        public string Id { get; set; }

        public string userSender { get; set; }

        public string userRecipient { get; set; }
    }
}