using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogWeb.Domain.SignUp;
using BlogWeb.Domain.Constants;
using BlogWeb.Application.Authentication.Commands.Authentication;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.Models.Authentication;
using BlogWeb.Application.Authentication.Commands.Register;

namespace BlogWeb.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) => _userService = userService;

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResult<AuthenticateResponse>>> Login(AuthenticationCommand query,CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResult<AuthenticateResponse>>> Register([FromBody] RegisterCommand registerUser)
        {
            var response = await Mediator.Send(registerUser);
            var user = response.Data.User;
            var token = response.Data.Token;
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            
            return Ok("Thank you for confirming your email.");
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

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            return Ok();
        }


    }
}