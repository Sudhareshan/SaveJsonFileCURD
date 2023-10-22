using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaveJsonFileCURD.Models;
using SaveJsonFileCURD.Repository;

namespace SaveJsonFileCURD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDataStore _userService;

        public UserController(IUserDataStore userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAllUsers")]
        public IActionResult GetUsersInfo()
        {
            try
            {
                var userInfo = _userService.GetUsers();
                if (userInfo.Count > 0)
                {
                    return Ok(userInfo);
                }
                else
                {
                    return NotFound();
                }
            }
            catch 
            {
                return BadRequest("Invalid user data");
            }
            
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] User user)
        {
            try
            {
                _userService.CreateUser(user);
                // You can return a 201 Created status code along with the newly created user.
                // Or you can return a 200 OK if preferred.
                // return CreatedAtAction("GetUserById", new { userId = user.UserId }, user);

                return Ok(user);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid user data");
            }
        }

       
        [HttpGet("GetUserById/{id:int}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
              var userinfo= _userService.GetUserById(id);
                return Ok(userinfo);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid user data");
            }
        }

        [HttpDelete("DeleteUser/{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok("User removed successfully.");
            }
            catch(Exception e)
            {
                return NotFound("User not found with the specified userId.");
            }
        }

        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser([FromBody] User user)
        {
            try
            {
                _userService.UpdateUser(user);
                return Ok("User Updated successfully");
            }
            catch
            {
                return NotFound("User not found with the specified userId.");
            }
        }


    }
}
