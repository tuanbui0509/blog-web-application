using Microsoft.AspNetCore.Authorization;

namespace BlogWeb.Application.Entities.Authentication
{
    [Flags]
    public enum Role
    {
        Admin = 0,
        SuperAdmin = 1,
        User = 2,
        SuperUser = 3
    }

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(Role roleEnum)
        {
            Roles = roleEnum.ToString().Replace(" ", string.Empty);
        }
    }
}