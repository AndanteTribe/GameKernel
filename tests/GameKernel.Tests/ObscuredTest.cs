namespace GameKernel.Tests;

public class ObscuredTest
{
    [Fact]
    public void DefaultConstructor_ValueIsDefault()
    {
        var o = new Obscured<int>();
        Assert.Equal(default(int), o.Value);
    }

    [Theory]
    [InlineData(42, 42)]
    [InlineData(12345, 12345)]
    [InlineData(77, 77)]
    public void ImplicitFromT_StoresValue(int value, int expected)
    {
        Obscured<int> o = value;
        Assert.Equal(expected, o.Value);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(12345)]
    public void ImplicitToT_ReturnsOriginalValue(int value)
    {
        Obscured<int> o = value;
        int result = o;
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(10, 20, false)]
    public void Equals_WithSameOrDifferentValue(int a, int b, bool expected)
    {
        Obscured<int> obscuredA = a;
        Obscured<int> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals(obscuredB));
    }

    [Theory]
    [InlineData(10, 10, true, false)]
    [InlineData(10, 20, false, true)]
    public void EqualityOperator_WithSameOrDifferentValues(int a, int b, bool shouldBeEqual, bool shouldBeNotEqual)
    {
        Obscured<int> obscuredA = a;
        Obscured<int> obscuredB = b;
        Assert.Equal(shouldBeEqual, obscuredA == obscuredB);
        Assert.Equal(shouldBeNotEqual, obscuredA != obscuredB);
    }

    [Theory]
    [InlineData(1, 100, true)]
    [InlineData(100, 1, false)]
    [InlineData(50, 50, false)]
    public void CompareTo_WithInt(int small, int large, bool shouldBeLessThan)
    {
        Obscured<int> obscuredSmall = small;
        Obscured<int> obscuredLarge = large;
        Assert.Equal(shouldBeLessThan, obscuredSmall.CompareTo(obscuredLarge) < 0);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(99)]
    public void ToString_ReturnsStringRepresentation(int value)
    {
        Obscured<int> o = value;
        Assert.Equal(value.ToString(), o.ToString());
    }

    [Theory]
    [InlineData(99, 99, true)]
    [InlineData(1, 2, false)]
    public void GetHashCode_WithSameOrDifferentValues(int a, int b, bool shouldBeEqual)
    {
        Obscured<int> obscuredA = a;
        Obscured<int> obscuredB = b;
        if (shouldBeEqual)
            Assert.Equal(obscuredA.GetHashCode(), obscuredB.GetHashCode());
        else
            Assert.NotEqual(obscuredA.GetHashCode(), obscuredB.GetHashCode());
    }

    [Theory]
    [InlineData(42, 42, true)]
    [InlineData(42, 99, false)]
    public void Equals_ObjectOverload(int a, int b, bool expected)
    {
        Obscured<int> obscuredA = a;
        Obscured<int> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals((object)obscuredB));
    }

    [Fact]
    public void Equals_ObjectOverload_WithNull_ReturnsFalse()
    {
        Obscured<int> o = 42;
        Assert.False(o.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        Obscured<int> o = 42;
        object other = "not an Obscured";
        Assert.False(o.Equals(other));
    }

    [Theory]
    [InlineData(3.14f)]
    [InlineData(2.71f)]
    public void ImplicitConversion_WithFloat(float value)
    {
        Obscured<float> o = value;
        float result = o;
        Assert.Equal(value, result, 2);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ImplicitConversion_WithBool(bool value)
    {
        Obscured<bool> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(3.14159, 5)]
    [InlineData(2.71828, 5)]
    public void ImplicitConversion_WithDouble(double value, int precision)
    {
        Obscured<double> o = value;
        Assert.Equal(value, o.Value, precision);
    }

    [Theory]
    [InlineData((byte)255)]
    [InlineData((byte)0)]
    public void ImplicitConversion_WithByte(byte value)
    {
        Obscured<byte> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(1.5f, 2.5f)]
    public void CompareTo_WithFloat(float small, float large)
    {
        Obscured<float> obscuredSmall = small;
        Obscured<float> obscuredLarge = large;
        Assert.True(obscuredSmall.CompareTo(obscuredLarge) < 0);
        Assert.True(obscuredLarge.CompareTo(obscuredSmall) > 0);
    }

    [Theory]
    [InlineData(1.5, 2.5)]
    public void CompareTo_WithDouble(double small, double large)
    {
        Obscured<double> obscuredSmall = small;
        Obscured<double> obscuredLarge = large;
        Assert.True(obscuredSmall.CompareTo(obscuredLarge) < 0);
    }

    [Theory]
    [InlineData(1.5f, 1.5f, true)]
    [InlineData(1.5f, 2.5f, false)]
    public void Equals_WithFloat(float a, float b, bool expected)
    {
        Obscured<float> obscuredA = a;
        Obscured<float> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals(obscuredB));
    }

    [Theory]
    [InlineData(1.5, 1.5, true)]
    [InlineData(1.5, 2.5, false)]
    public void Equals_WithDouble(double a, double b, bool expected)
    {
        Obscured<double> obscuredA = a;
        Obscured<double> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals(obscuredB));
    }

    [Theory]
    [InlineData(1.5f, 1.5f, true)]
    [InlineData(1.5f, 2.5f, false)]
    public void EqualityOperator_WithFloat(float a, float b, bool shouldBeEqual)
    {
        Obscured<float> obscuredA = a;
        Obscured<float> obscuredB = b;
        Assert.Equal(shouldBeEqual, obscuredA == obscuredB);
    }

    [Theory]
    [InlineData(1.5, 2.5, false)]
    public void EqualityOperator_WithDouble_DifferentValue(double a, double b, bool shouldBeEqual)
    {
        Obscured<double> obscuredA = a;
        Obscured<double> obscuredB = b;
        Assert.Equal(shouldBeEqual, obscuredA == obscuredB);
    }

    [Theory]
    [InlineData(1.5, 2.5)]
    public void InequalityOperator_WithDouble(double a, double b)
    {
        Obscured<double> obscuredA = a;
        Obscured<double> obscuredB = b;
        Assert.True(obscuredA != obscuredB);
    }

    [Theory]
    [InlineData(1.5f)]
    [InlineData(3.5f)]
    public void ToString_WithFloat(float value)
    {
        Obscured<float> o = value;
        Assert.NotEmpty(o.ToString());
    }

    [Theory]
    [InlineData(2.71828)]
    [InlineData(3.14159)]
    public void ToString_WithDouble(double value)
    {
        Obscured<double> o = value;
        string formatted = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Assert.Contains(formatted, o.ToString(), StringComparison.InvariantCulture);
    }

    [Theory]
    [InlineData(1.5f, 1.5f, true)]
    [InlineData(1.5f, 2.5f, false)]
    public void Equals_ObjectOverload_WithFloat(float a, float b, bool expected)
    {
        Obscured<float> obscuredA = a;
        Obscured<float> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals((object)obscuredB));
    }

    [Theory]
    [InlineData(1.5, 2.5, false)]
    public void Equals_ObjectOverload_WithDouble_DifferentValue(double a, double b, bool expected)
    {
        Obscured<double> obscuredA = a;
        Obscured<double> obscuredB = b;
        Assert.Equal(expected, obscuredA.Equals((object)obscuredB));
    }

    [Fact]
    public void Equals_ObjectOverload_WithDouble_Null_ReturnsFalse()
    {
        Obscured<double> o = 1.5;
        Assert.False(o.Equals(null));
    }

    [Theory]
    [InlineData(42u)]
    public void ImplicitConversion_WithUint(uint value)
    {
        Obscured<uint> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(ulong.MaxValue)]
    public void ImplicitConversion_WithUlong(ulong value)
    {
        Obscured<ulong> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(-1000)]
    [InlineData(1000)]
    public void ImplicitConversion_WithShort(short value)
    {
        Obscured<short> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(65535)]
    [InlineData(100)]
    public void ImplicitConversion_WithUshort(ushort value)
    {
        Obscured<ushort> o = value;
        Assert.Equal(value, o.Value);
    }

    [Theory]
    [InlineData(long.MaxValue)]
    public void ImplicitConversion_WithLong(long value)
    {
        Obscured<long> o = value;
        Assert.Equal(value, o.Value);
    }
}
