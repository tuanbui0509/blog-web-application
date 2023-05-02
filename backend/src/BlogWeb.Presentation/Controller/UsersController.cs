using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BlogWeb.Application.Models;
using BlogWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using BlogWeb.Common.Constants;

namespace BlogWeb.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);
            return Ok(response);
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("[action]")]
        public IEnumerable<string> Users()
        {
            return new List<string> { "Ahmed", "Ali", "Ahsan" };
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("[action]")]
        public IEnumerable<string> UsersById()
        {
            return new List<string> { "Ahmed", "Ali", "hahaha","huhuhu" };
        }

        // [Authorize(Roles = "Admin")]
        // [HttpGet]
        // public IActionResult GetAll()
        // {
        //     var users = _userService.GetAll();
        //     return Ok(users);
        // }

        // [HttpGet("{id:int}")]
        // public IActionResult GetById(int id)
        // {
        //     // only admins can access other user records
        //     var currentUser = (User)HttpContext.Items["User"];
        //     if (id != currentUser.Id && currentUser.Role != Role.Admin)
        //         return Unauthorized(new { message = "Unauthorized" });

        //     var user = _userService.GetById(id);
        //     return Ok(user);
        // }
    }
}