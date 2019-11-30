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
    public class ChatsController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatsController(ChatService chatService)
        {
            _chatService = chatService;
        }
        
        
        // GET: api/Chats
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/Chats/GetChat
        [HttpGet,Route("GetChat/{userSender}")]
        public ActionResult<List<Chat>> Get(string userSender)
        {
            return _chatService.Get(userSender);
        }

        // POST: api/Chats/Create
        [HttpPost,Route("Create")]
        public ActionResult<Friend> Create(Chat chat)
        {
            _chatService.Create(chat);

            Chat copia = new Chat();
            copia.userSender = chat.userSender;
            copia.userRecipient = chat.userRecipient;

            _chatService.Create(copia);

            return Ok();
        }

        // PUT: api/Chats/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        [HttpDelete,Route("Delete/{userSender}/{userRecipient}")]
        public IActionResult Delete(string userSender, string userRecipient)
        {
            return Ok();
        }
    }
}
