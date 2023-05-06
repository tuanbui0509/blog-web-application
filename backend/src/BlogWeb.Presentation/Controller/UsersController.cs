using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogWeb.Infrastructure.ApplicationUser.Queries;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.SignUp;
using BlogWeb.Domain.Models.Authentication;
using BlogWeb.Domain.Constants;
using BlogWeb.Application.Interfaces;

namespace BlogWeb.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        // public async Task<IActionResult> Login(AuthenticateRequest model)
        // {
        //     var response = await _userService.Authenticate(model);
        //     return Ok(response);
        // }
        public async Task<ActionResult<ServiceResult<AuthenticateResponse>>> Login(GetTokenQuery query, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var response = await _userService.RegisterUser(registerUser);
            return Ok(response);
        }

        [HttpPost("LoginWithOTP")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var response = await _userService.LoginWithOTP(code, username);
            return Ok(response);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("[action]")]
        public IEnumerable<string> UsersById()
        {
            return new List<string> { "Ahmed", "Ali", "hahaha", "huhuhu" };
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