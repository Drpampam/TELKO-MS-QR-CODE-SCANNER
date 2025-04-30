namespace Application.Interfaces
{
    public interface IUnitOfWork 
    {
        Task<int> CommitChangesAsync();
        int CommitChanges();

        Task CommitAsync();
    }
}
