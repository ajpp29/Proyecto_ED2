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
    public class FriendsController : ControllerBase
    {
        private readonly FriendService _friendService;

        public FriendsController(FriendService friendService)
        {
            _friendService = friendService;
        }

        // GET: api/Friends
        //[HttpGet("{userName}", Name = "Get")]
        //public ActionResult<List<Friend>> Get(string userName)
        //{
        //    return  _friendService.Get(userName);
        //}

        // GET: api/Friends/5
        [HttpGet("{userName}", Name = "GetFriend")]
        public ActionResult<List<Friend>> Get(string userName)
        {
            return _friendService.Get(userName);
        }

        // POST: api/Friends
        [HttpPost]
        public ActionResult<Friend> Create(Friend friend)
        {
            _friendService.Create(friend);

            return CreatedAtRoute("GetFriend", new { userName = friend.userName.ToString() }, friend);
        }

        // PUT: api/Friends/5
        [HttpPut("{userName}")]
        public IActionResult Delete(string userName, Friend friendIn)
        {
            var friends = _friendService.Get(userName);
            

            if (!ContainsFriend(friends,friendIn))
            {
                return NotFound();
            }

            _friendService.Remove(friendIn);

            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        bool ContainsFriend(List<Friend> friends, Friend friendIn)
        {
            var find = false;
            for (int i = 0; i < friends.Count(); i++)
            {
                if(friends.ToArray()[i].userName==friendIn.userName && friends.ToArray()[i].userFriend == friendIn.userFriend)
                {
                    find = true;
                }
            }

            return find;
        }
    }
}
