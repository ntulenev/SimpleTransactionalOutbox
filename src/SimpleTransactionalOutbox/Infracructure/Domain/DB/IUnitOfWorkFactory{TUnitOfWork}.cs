namespace Infracructure.Domain.DB
{
    public interface IUnitOfWorkFactory<TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}
