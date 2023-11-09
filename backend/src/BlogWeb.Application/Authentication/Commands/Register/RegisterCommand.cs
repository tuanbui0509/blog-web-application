using System.ComponentModel.DataAnnotations;
using BlogWeb.Domain.Interfaces.Repositories;
using BlogWeb.Domain.Models.Authentication;

namespace BlogWeb.Application.Authentication.Commands.Register
{
    public class RegisterCommand : IRequestWrapper<AuthenticateResponse>
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}