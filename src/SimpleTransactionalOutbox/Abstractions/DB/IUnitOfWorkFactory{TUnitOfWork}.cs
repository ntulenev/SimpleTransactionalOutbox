namespace Abstractions.DB
{
    public interface IUnitOfWorkFactory<TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}
