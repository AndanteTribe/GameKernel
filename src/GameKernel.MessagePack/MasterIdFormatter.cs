using MessagePack;
using MessagePack.Formatters;

namespace GameKernel.MessagePack;

/// <summary>
/// A MessagePack formatter for the <see cref="MasterId{TGroup}"/> struct.
/// </summary>
/// <typeparam name="TGroup"></typeparam>
public sealed class MasterIdFormatter<TGroup> : IMessagePackFormatter<MasterId<TGroup>> where TGroup : unmanaged, Enum
{
    /// <inheritdoc />
    void IMessagePackFormatter<MasterId<TGroup>>.Serialize(ref MessagePackWriter writer, MasterId<TGroup> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(2);
        options.Resolver.GetFormatterWithVerify<TGroup>().Serialize(ref writer, value.Group, options);
        writer.Write(value.Id);
    }

    /// <inheritdoc />
    MasterId<TGroup> IMessagePackFormatter<MasterId<TGroup>>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Typecode is null, struct not supported");
        }

        options.Security.DepthStep(ref reader);
        var count = reader.ReadArrayHeader();
        if (count != 2)
        {
            throw new MessagePackSerializationException("Invalid array length. Expected 2, but got " + count);
        }

        var group = options.Resolver.GetFormatterWithVerify<TGroup>().Deserialize(ref reader, options);
        var id = reader.ReadUInt32();

        var result = new MasterId<TGroup>(group, id);
        reader.Depth--;
        return result;
    }
}