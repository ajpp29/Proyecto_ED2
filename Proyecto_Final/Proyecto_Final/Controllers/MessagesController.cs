using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metodos;
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
        Cifrado cifrado = new Cifrado(0);

        public MessagesController(MessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: api/Messages/GetConversation
        [HttpGet,Route("GetConversation/{userName}/{userFriend}")]
        public ActionResult<List<Message>> GetConversation(string userName,string userFriend)
        {
            Friend friend = new Friend();
            friend.userName = userName;
            friend.userFriend = userFriend;


            var messagesList = _messageService.Get(friend.userName,friend);

            foreach (var item in messagesList)
            {
                var numeroCifrado = cifrado.GenerarNumeroCifrado(item.userSender,item.userRecipient);
                item.messageSent = cifrado.LeerArchivo(item.messageSent, numeroCifrado, false);
                numeroCifrado = cifrado.GenerarNumeroCifrado(item.userRecipient);
                item.messageSent = cifrado.LeerArchivo(item.messageSent, numeroCifrado, false);
                numeroCifrado = cifrado.GenerarNumeroCifrado(item.userSender);
                item.messageSent = cifrado.LeerArchivo(item.messageSent, numeroCifrado, false);
            }

            return messagesList;
        }

        // GET: api/Messages/5
        //[HttpGet("{id}", Name = "GetMessage")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Messages/SendMessage
        [HttpPost,Route("SendMessage")]
        public IActionResult Post(Message message)
        {
            var numeroCifrado = cifrado.GenerarNumeroCifrado(message.userSender);
            message.messageSent = cifrado.LeerArchivo(message.messageSent, numeroCifrado, true);
            numeroCifrado = cifrado.GenerarNumeroCifrado(message.userRecipient);
            message.messageSent = cifrado.LeerArchivo(message.messageSent, numeroCifrado, true);
            numeroCifrado = cifrado.GenerarNumeroCifrado(message.userSender, message.userRecipient);
            message.messageSent = cifrado.LeerArchivo(message.messageSent, numeroCifrado, true);

            message.dateTime = DateTime.Now;
            message.Original = true;
            _messageService.Create(message);
            Message copia = new Message();
            copia.userSender = message.userSender;
            copia.userRecipient = message.userRecipient;
            copia.messageSent = message.messageSent;
            copia.dateTime = message.dateTime;
            copia.Original = false;
            _messageService.Create(copia);

            return Ok();
        }

        //// PUT: api/Messages/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/Messages/DeleteMessages
        //us loggeado, receptor
        [HttpDelete,Route("DeleteMessages/{userName}/{userFriend}")]
        public IActionResult Delete(string userName,string userFriend)
        {
            Friend friend = new Friend();
            friend.userName = userName;
            friend.userFriend = userFriend;
            _messageService.Remove(friend);

            return Ok();
        }
    }
}
