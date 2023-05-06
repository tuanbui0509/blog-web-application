using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogWeb.Infrastructure.Authorization;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure.Services.Emails;

using MapsterMapper;
using BlogWeb.Domain.Emails;
using BlogWeb.Domain.Models.Authentication;
using BlogWeb.Domain.Dto;
using BlogWeb.Domain.Helpers;
using BlogWeb.Domain.Exceptions;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.SignUp;
using BlogWeb.Domain.Constants;
using BlogWeb.Domain.Entities.Authentication;
using BlogWeb.Application.Interfaces;

namespace BlogWeb.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly UserManager<UserApplication> _userManager;
        private readonly SignInManager<UserApplication> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public UserService(
            ApplicationDbContext context
            , IJwtUtils jwtUtils
            , UserManager<UserApplication> userManager
            , RoleManager<IdentityRole> roleManager
            , SignInManager<UserApplication> signInManager
            , IEmailService emailService
            , IMapper mapper)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _mapper = mapper;
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

                    return new AuthenticateResponse
                    {
                        User = new ApplicationUserDto { UserName = user.UserName, Email = user.Email, Id = user.Id },
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken)
                    };
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

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizeException();
            }

            return user.UserName;
        }

        public async Task<ApplicationUserDto> CheckUserPassword(string email, string password)
        {
            UserApplication user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return _mapper.Map<ApplicationUserDto>(user);
            }

            return null;
        }

        public async Task<bool> UserIsInRole(string userId, string role)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(UserApplication user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            var user = new UserApplication
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<Response> RegisterUser(RegisterUser registerUser)
        {
            var role = Roles.User;
            //Check User Exist 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return new Response { Status = "Error", Message = "User already exists!" };
            }

            //Add the User in the database
            UserApplication user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username,
                TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return new Response { Status = "Error", Message = "User Failed to Create" };
                }
                //Add role to the user....

                await _userManager.AddToRoleAsync(user, role);

                //Add Token to Verify the email....
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = await ConfirmEmail(token, user.Email);
                var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink.Message);
                _emailService.SendEmail(message);
                return new Response { Status = "Success", Message = $"User created & Email Sent to {user.Email} SuccessFully" };

            }
            else
            {
                return new Response { Status = "Error", Message = "This Role Doesnot Exist." };
            }
        }

        public async Task<Response> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
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

                    var jwtToken = _jwtUtils.GenerateJwtToken(authClaims);

                    return new AuthenticateResponse
                    {
                        User = new ApplicationUserDto { UserName = user.UserName, Email = user.Email, Id = user.Id },
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken)
                    };
                }
            }
            return new Response { Status = "Success", Message = $"Invalid Code" };
        }

        public async Task<Response> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new Response { Status = "Success", Message = nameof(ConfirmEmail) };
                }
            }
            return new Response { Status = "Error", Message = "This User Doesnot exist!" };
        }
    }
}