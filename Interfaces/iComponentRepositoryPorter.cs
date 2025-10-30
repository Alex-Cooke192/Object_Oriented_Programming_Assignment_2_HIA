namespace JetInteriorApp.Interfaces
{
    public interface IComponentPorter
    {
        Task<List<FantasyComponent>> GetFullComponentsForUser(Guid userId);
    }
}