using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GameKernel;

/// <summary>
/// Master Data ID.
/// </summary>
/// <param name="Id"></param>
/// <param name="Group"></param>
/// <typeparam name="TGroup"></typeparam>
public readonly record struct MasterId<TGroup>(TGroup Group, uint Id)
    : IEquatable<MasterId<TGroup>>, IComparable<MasterId<TGroup>>, ISpanFormattable where TGroup : unmanaged, Enum
{
    /// <summary>
    /// Implicit conversion from <see cref="ValueTuple{TGroup,T}"/> to <see cref="MasterId{TGroup}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MasterId<TGroup>(in ValueTuple<TGroup, uint> value) => new(value.Item1, value.Item2);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MasterId<TGroup> other) => EqualityComparer<TGroup>.Default.Equals(Group, other.Group) && Id == other.Id;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(MasterId<TGroup> other) => Comparer<TGroup>.Default.Compare(Group, other.Group) switch
    {
        0 => Id.CompareTo(other.Id),
        var c => c
    };

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
#if NET8_0_OR_GREATER
        if (!Enum.TryFormat(Group, destination, out charsWritten))
        {
            charsWritten = 0;
            return false;
        }
#else
        var group = Group.ToString().AsSpan();

        if (!group.TryCopyTo(destination))
        {
            charsWritten = 0;
            return false;
        }
        charsWritten = group.Length;
#endif

        if (charsWritten + 1 > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        destination[charsWritten++] = '.';

        // Id部分はフォーマット対応している.
        if (!Id.TryFormat(destination[charsWritten..], out var idCharsWritten, format.IsEmpty ? "0000" : format, provider))
        {
            charsWritten = 0;
            return false;
        }
        charsWritten += idCharsWritten;
        return true;
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) => GetStringInternal(format, formatProvider);

    /// <inheritdoc />
    public override string ToString() => GetStringInternal(ReadOnlySpan<char>.Empty, null);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(Group, Id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStringInternal(in ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    {
        var bufferLength = 16;
        var result = "";
        while (!TryGetStr(this, format, formatProvider, bufferLength, out result))
        {
            bufferLength *= 2;
        }
        return result;

        static bool TryGetStr(in MasterId<TGroup> instance, in ReadOnlySpan<char> format, IFormatProvider? formatProvider, int bufferLength, out string result)
        {
            var buffer = (Span<char>)stackalloc char[bufferLength];
            if (instance.TryFormat(buffer, out var written, format, formatProvider))
            {
                result = buffer[..written].ToString();
                return true;
            }
            result = "";
            return false;
        }
    }
}