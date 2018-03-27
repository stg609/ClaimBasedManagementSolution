namespace Common.Domain
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Begin();
    }
}
