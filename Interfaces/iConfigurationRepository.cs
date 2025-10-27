public interface iConfigurationRepository
// Interface for direct access to database - methods contained allow loading 
// Extending access through interface allows multiple database structures to be used (e.g. JSON, XML, C#) - supports
// single responsibility, & facilities polymorphism/abstraction to boost scalability
{
    Task Loadall;
    Task SaveAll;

    Task SaveConfig;
}