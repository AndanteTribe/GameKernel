using MessagePack;
using MessagePack.Formatters;

namespace GameKernel.MessagePack;

/// <summary>
/// A custom <see cref="IFormatterResolver"/> that provides formatters for the <see cref="MasterId{TGroup}"/> and <see cref="Obscured{T}"/> structs.
/// </summary>
public class GameKernelResolver : IFormatterResolver
{
    /// <summary>
    /// A shared instance of the <see cref="GameKernelResolver"/> that can be used throughout the application.
    /// </summary>
    public static readonly IFormatterResolver Shared = new GameKernelResolver();

    private static readonly RuntimeTypeHandle s_masterIdTypeHandle = typeof(MasterId<>).TypeHandle;
    private static readonly RuntimeTypeHandle s_obscuredTypeHandle = typeof(Obscured<>).TypeHandle;

    private GameKernelResolver()
    {
    }

    /// <inheritdoc />
    public IMessagePackFormatter<T>? GetFormatter<T>() => FormatterCache<T>.Formatter;

    private static class FormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T>? Formatter = GetFormatter(typeof(T)) as IMessagePackFormatter<T>;
    }

    private static object? GetFormatter(Type t)
    {
        if (t.IsGenericType)
        {
            var typeHandle = t.GetGenericTypeDefinition().TypeHandle;
            if (typeHandle.Equals(s_masterIdTypeHandle))
            {
                return Activator.CreateInstance(typeof(MasterIdFormatter<>).MakeGenericType(t.GenericTypeArguments))!;
            }
            if (typeHandle.Equals(s_obscuredTypeHandle))
            {
                return Activator.CreateInstance(typeof(ObscuredFormatter<>).MakeGenericType(t.GenericTypeArguments))!;
            }
        }
        return null;
    }
}