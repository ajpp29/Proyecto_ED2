using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Models;
using Proyecto_Final.Services;
using Metodos;

namespace Proyecto_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserService _userService;
        Cifrado cifrado = new Cifrado(0);

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            var usersList = _userService.Get();

            foreach (var item in usersList)
            {
                var numeroCifrado = cifrado.GenerarNumeroCifrado(item.userName);
                item.Password = cifrado.LeerArchivo(item.Password, numeroCifrado, false);
            }

            return usersList;
        }

        // GET: api/Users/5
        //[HttpGet("{id:length(24)}", Name = "GetUser")]
        [HttpGet("{userName}", Name = "GetUser")]
        public ActionResult<User> Get(string userName)
        {
            var user = _userService.Get(userName);

            if (user == null)
            {
                return NotFound();
            }

            var numeroCifrado = cifrado.GenerarNumeroCifrado(user.userName);
            user.Password = cifrado.LeerArchivo(user.Password, numeroCifrado, false);

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            ///////RECIEN AÑADIDO
            var numeroCifrado = cifrado.GenerarNumeroCifrado(user.userName);
            //////////////////////////////

            user.Password = cifrado.LeerArchivo(user.Password, numeroCifrado, true);
            //var descifrar = cifrado.LeerArchivo(cifrar, false);

            _userService.Create(user);

            //return CreatedAtRoute("GetUser", new { userName = user.userName.ToString() }, user);
            return Ok();
        }

        // PUT: api/Users/e
        //[HttpPut("{id:length(24)}")]
        [HttpPut("edit")]
        public IActionResult Update(User userIn)
        {
            var numeroCifrado = cifrado.GenerarNumeroCifrado(userIn.userName);
            userIn.Password= cifrado.LeerArchivo(userIn.Password, numeroCifrado, true);

            string userName = userIn.userName;
            var user = _userService.Get(userName);

            if (user == null)
            {
                return NotFound();
            }

            userIn.Id = user.Id;
            _userService.Update(userName, userIn);

            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id:length(24)}")]
        [HttpDelete, Route("Delete/{userName}")]
        public IActionResult Delete(string userName)
        {
            var user = _userService.Get(userName);

            if(user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.userName);

            return NoContent();
        }
    }
}
