using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameKernel;

/// <summary>
/// A struct that obscures a value of type <typeparamref name="T"/> by XORing it with a random key.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Obscured<T> : IEquatable<Obscured<T>>, IComparable<Obscured<T>> where T : unmanaged
{
    private readonly T _hiddenValue;
    private readonly T _key;

    /// <summary>
    /// Gets the original value by XORing the hidden value with the key.
    /// </summary>
    public T Value => Xor(_hiddenValue, _key);

    /// <summary>
    /// Initializes a new instance of the <see cref="Obscured{T}"/> struct.
    /// </summary>
    public Obscured()
    {
        _key = GenerateKey();
        _hiddenValue = Xor(default, _key);
    }

    private Obscured(T value)
    {
        _key = GenerateKey();
        _hiddenValue = Xor(value, _key);
    }

    internal Obscured(T hiddenValue, T key)
    {
        _hiddenValue = hiddenValue;
        _key = key;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe T GenerateKey()
    {
        var buffer = (Span<byte>)stackalloc byte[sizeof(T)];
        Random.Shared.NextBytes(buffer);
        return MemoryMarshal.Read<T>(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Xor(T value, T key)
    {
        var v = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
        var k = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref key, 1));
        for (var i = 0; i < v.Length; i++)
        {
            v[i] ^= k[i];
        }
        return MemoryMarshal.Read<T>(v);
    }

    /// <summary>
    /// Implicit conversion from <see cref="Obscured{T}"/> to <typeparamref name="T"/> by XORing the hidden value with the key.
    /// </summary>
    /// <param name="obscured"></param>
    /// <returns></returns>
    public static implicit operator T(Obscured<T> obscured) => Xor(obscured._hiddenValue, obscured._key);

    /// <summary>
    /// Implicit conversion from <typeparamref name="T"/> to <see cref="Obscured{T}"/> by creating a new instance of <see cref="Obscured{T}"/> with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Obscured<T>(T value) => new(value);

    /// <inheritdoc />
    public bool Equals(Obscured<T> other) =>
        EqualityComparer<T>.Default.Equals(Xor(_hiddenValue, _key), Xor(other._hiddenValue, other._key));

    /// <inheritdoc />
    public int CompareTo(Obscured<T> other) =>
        Comparer<T>.Default.Compare(Xor(_hiddenValue, _key), Xor(other._hiddenValue, other._key));

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Xor(_hiddenValue, _key));

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Obscured<T> other && Equals(other);

    /// <summary>
    /// Returns a value indicating whether two <see cref="Obscured{T}"/> values are equal.
    /// </summary>
    public static bool operator ==(Obscured<T> left, Obscured<T> right) =>
        EqualityComparer<T>.Default.Equals(Xor(left._hiddenValue, left._key), Xor(right._hiddenValue, right._key));

    /// <summary>
    /// Returns a value indicating whether two <see cref="Obscured{T}"/> values are not equal.
    /// </summary>
    public static bool operator !=(Obscured<T> left, Obscured<T> right) =>
        !EqualityComparer<T>.Default.Equals(Xor(left._hiddenValue, left._key), Xor(right._hiddenValue, right._key));
}