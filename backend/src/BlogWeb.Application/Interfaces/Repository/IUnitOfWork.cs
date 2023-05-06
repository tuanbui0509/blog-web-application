namespace BlogWeb.Application.Interfaces.Repository
{
    public interface IUnitOfWork
    {
         Task CommitAsync();   
    }
}