namespace JetInteriorApp.Interfaces
{
    public interface IComponentRepositoryPorter
    {
        Task<List<InteriorComponent>> GetFullComponentsForUser(Guid userId);
    }
}