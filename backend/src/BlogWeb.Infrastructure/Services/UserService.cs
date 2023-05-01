using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogWeb.Application.Models;
using BlogWeb.Application.Models.Emails;
using BlogWeb.Common.Helpers;
using BlogWeb.Infrastructure.Authorization;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure.Services.Emails;
using Microsoft.AspNetCore.Identity;

namespace BlogWeb.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        public UserService(
            ApplicationDbContext context
            , IJwtUtils jwtUtils
            , UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager
            , SignInManager<IdentityUser> signInManager
            , IEmailService emailService)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        public async Task<Response> Authenticate(AuthenticateRequest loginModel)
        {
            try
            {
                // var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);
                var user = await _userManager.FindByNameAsync(loginModel.Username);
                // validate
                if (user.TwoFactorEnabled)
                {
                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" };
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    // authentication successful so generate jwt token
                    var jwtToken = _jwtUtils.GenerateJwtToken(authClaims);

                    return new AuthenticateResponse(user, new JwtSecurityTokenHandler().WriteToken(jwtToken), jwtToken.ValidTo);
                    // return Ok(new
                    // {
                    //     token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    //     expiration = jwtToken.ValidTo
                    // });
                    //returning the token...

                }
                return new Response { Status = "Failure", Message = $"Username or password is incorrect" };
            }
            catch (System.Exception)
            {
                throw new AppException("Username or password is incorrect");
            }
        }

        // public IEnumerable<UserApplication> GetAll()
        // {
        //     return _context.Users;
        // }

        // public UserApplication GetById(int id)
        // {
        //     var user = _context.Users.Find(id);
        //     if (user == null) throw new KeyNotFoundException("User not found");
        //     return user;
        // }
    }
}