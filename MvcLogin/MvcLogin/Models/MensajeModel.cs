using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLogin.Models
{
    public class MensajeModel
    {
        public DateTime dateTime { get; set; }
        public string messageSent { get; set; }
        public string userSender { get; set; }
        public string userRecipient { get; set; }

    }
}