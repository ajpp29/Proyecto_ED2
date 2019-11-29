using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessagesController(MessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: api/Messages
        [HttpGet,Route("GetFriend/{userName}/{userFriend}")]
        public ActionResult<List<Message>> GetFriend(string userName,string userFriend)
        {
            Friend friend = new Friend();
            friend.userName = userName;
            friend.userFriend = userFriend;
            return _messageService.Get(friend.userName,friend);
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "GetMessage")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Messages
        [HttpPost]
        public ActionResult<Message> Post(Message message)
        {
            message.dateTime = DateTime.Today;
            message.Original = true;
            _messageService.Create(message);
            Message copia = new Message();
            copia.userSender = message.userSender;
            copia.userRecipient = message.userRecipient;
            copia.messageSent = message.messageSent;
            copia.dateTime = message.dateTime;
            copia.Original = false;
            _messageService.Create(copia);

            return CreatedAtRoute("GetMessage", new { userName = message.userSender.ToString() }, message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
