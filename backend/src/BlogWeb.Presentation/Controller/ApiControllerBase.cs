using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BlogWeb.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();        
    }
}