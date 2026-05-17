using MessagePack;
using MessagePack.Formatters;

namespace GameKernel.MessagePack;

public sealed class ObscuredFormatter<T> : IMessagePackFormatter<Obscured<T>> where T : unmanaged
{
    /// <inheritdoc />
    void IMessagePackFormatter<Obscured<T>>.Serialize(ref MessagePackWriter writer, Obscured<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(1);
        options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
    }

    /// <inheritdoc />
    Obscured<T> IMessagePackFormatter<Obscured<T>>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Typecode is null, struct not supported");
        }

        options.Security.DepthStep(ref reader);
        var count = reader.ReadArrayHeader();
        if (count != 1)
        {
            throw new MessagePackSerializationException("Invalid array length. Expected 1, but got " + count);
        }

        var value = options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);

        var result = (Obscured<T>)value;
        reader.Depth--;
        return result;
    }
}