using System.Buffers.Text;
using System.Text;

namespace UrlShortener.Helper;

public interface IUrlConverter
{
    string Encode<T>(T input);
}

// Base62Converter encodes a long integer into a Base62 string.
// get unique long integers from DB increamental primary key, and convert to Base62 for short URL representation.
// or, get from distributed unique ID generator like Snowflake or ULID, and convert to Base62 for short URL representation.
public class Base62UrlConverter : IUrlConverter
{
    private readonly string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public string Encode<T>(T input)
    {
        if (input is not long number)
            throw new ArgumentException($"Expected long, got {typeof(T).Name}", nameof(input));

        if (number == 0) return Alphabet[0].ToString();

        var result = new StringBuilder();
        while (number > 0)
        {
            result.Insert(0, Alphabet[(int)(number % 62)]);
            number /= 62;
        }
        return result.ToString();
    }
}

// Base64GuidConverter encodes a Guid into a URL-safe Base64 string of 10 characters.
public class Base64GuidConverter : IUrlConverter
{
    public string Encode<T>(T input)
    {
        if (input is not Guid guid)
            throw new ArgumentException($"Expected Guid, got {typeof(T).Name}", nameof(input));
        Span<byte> guidBytes = stackalloc byte[16];
        guid.TryWriteBytes(guidBytes);

        // Base64 uses 24 chars for 16 bytes, but the last 2 are always '==' padding
        Span<byte> base64Bytes = stackalloc byte[24];
        Base64.EncodeToUtf8(guidBytes, base64Bytes, out _, out _);

        Span<char> base64Chars = stackalloc char[24];
        Encoding.UTF8.GetChars(base64Bytes, base64Chars);

        // 10 take first 10 characters of the base64 string
        // unique combinations of 10 characters from the base64 alphabet is 64^10
        // Make URL safe: Replace '+' with '-', '/' with '_', and strip out '==' padding
        return new string(base64Chars[..10])
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
