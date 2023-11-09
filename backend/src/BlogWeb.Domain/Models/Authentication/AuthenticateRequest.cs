using System.ComponentModel.DataAnnotations;

namespace BlogWeb.Domain.Models.Authentication;
public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
