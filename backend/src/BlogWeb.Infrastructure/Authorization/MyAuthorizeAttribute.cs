using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BlogWeb.Infrastructure.Authorization
{
    public class MyAuthorizeAttribute:AuthorizeAttribute
    {
        // 401 (Unauthorized) - indicates that the request has not been applied because it lacks valid 
        // authentication credentials for the target resource.
        // 403 (Forbidden) - when the user is authenticated but isn’t authorized to perform the requested 
        // operation on the given resource.
        // protected override void HandleUnauthorizedRequest(IHttpContextAccessor actionContext)
        // {
        //     if (!HttpContext.Current.User.Identity.IsAuthenticated)
        //     {
        //         base.HandleUnauthorizedRequest(actionContext);
        //     }
        //     else
        //     {
        //         actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        //     }
        // }
    }
}