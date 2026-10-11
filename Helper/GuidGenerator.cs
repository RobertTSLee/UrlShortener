using StackExchange.Redis;

namespace UrlShortener.Helper;

public interface IGuidGenerator
{
    Guid GenerateUniqueId();   
}

public interface IGuidGeneratorAsync
{
    Task<Guid> GenerateUniqueId();
}

public class GuidGenerator : IGuidGenerator
{
    public Guid GenerateUniqueId()
    {
        // .net 10 introduces Guid.CreateVersion7() for generating time-ordered GUIDs
        // mathematically impossible to generate duplicates in a single instance of the application, but if you have multiple instances,
        // time-ordered, can be used in distributed systems, better performance than random GUIDs
        // save DB round trips for uniqueness checks        
        return Guid.CreateVersion7();
    }
}