using System.Threading.Channels;
using UrlShortener.Model;

namespace UrlShortener.Service;

public interface IUrlChannel
{
    bool TryWrite(Url url);
    IAsyncEnumerable<Url> ReadAllAsync(CancellationToken cancellationToken);
}

public class UrlChannel : IUrlChannel
{
    private readonly Channel<Url> _urlChannel;
    // todo: move this to ENV or configuration
    // The default capacity of the channel is set to 1000,
    // which means it can hold up to 1000 Url objects before blocking writers.
    // This value can be adjusted based on expected load and memory constraints.
    // should not put too high, the channel is in-memory, if too high, it will cause memory pressure and OOM
    private const int DefaultCapacity = 1000;

    public UrlChannel()
    {
        // Multiple writers can write to the channel,
        // but only one reader can read from it at a time.
        // This is suitable for scenarios where multiple producers generate URLs
        // and a single consumer processes them.
        var option = new BoundedChannelOptions(DefaultCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _urlChannel = Channel.CreateBounded<Url>(option);
    }

    public bool TryWrite(Url url)
    {
        return _urlChannel.Writer.TryWrite(url);
    }

    public IAsyncEnumerable<Url> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _urlChannel.Reader.ReadAllAsync(cancellationToken);
    }
}
