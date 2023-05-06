using BlogWeb.Domain.Models;
using MediatR;

namespace BlogWeb.Domain.Interfaces
{
    public interface IRequestWrapper<T> : IRequest<ServiceResult<T>>
    {
         
    }
    public interface IRequestHandlerWrapper<TIn, TOut> : IRequestHandler<TIn, ServiceResult<TOut>> where TIn : IRequestWrapper<TOut>
    {

    }
}