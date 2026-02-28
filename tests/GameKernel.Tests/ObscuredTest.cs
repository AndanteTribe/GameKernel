namespace GameKernel.Tests;

public class ObscuredTest
{
    [Fact]
    public void DefaultConstructor_ValueIsDefault()
    {
        var o = new Obscured<int>();
        Assert.Equal(default(int), o.Value);
    }

    [Fact]
    public void ImplicitFromT_StoresValue()
    {
        Obscured<int> o = 42;
        Assert.Equal(42, o.Value);
    }

    [Fact]
    public void ImplicitToT_ReturnsOriginalValue()
    {
        Obscured<int> o = 42;
        int value = o;
        Assert.Equal(42, value);
    }

    [Fact]
    public void Value_IsCorrectAfterAssignment()
    {
        Obscured<int> o = 12345;
        Assert.Equal(12345, o.Value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Obscured<int> a = 10;
        Obscured<int> b = 10;
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Obscured<int> a = 10;
        Obscured<int> b = 20;
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void EqualityOperator_SameValue_ReturnsTrue()
    {
        Obscured<int> a = 10;
        Obscured<int> b = 10;
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ReturnsTrue()
    {
        Obscured<int> a = 10;
        Obscured<int> b = 20;
        Assert.True(a != b);
        Assert.False(a == b);
    }

    [Fact]
    public void CompareTo_LesserValue_ReturnsNegative()
    {
        Obscured<int> small = 1;
        Obscured<int> large = 100;
        Assert.True(small.CompareTo(large) < 0);
    }

    [Fact]
    public void CompareTo_GreaterValue_ReturnsPositive()
    {
        Obscured<int> small = 1;
        Obscured<int> large = 100;
        Assert.True(large.CompareTo(small) > 0);
    }

    [Fact]
    public void CompareTo_EqualValues_ReturnsZero()
    {
        Obscured<int> a = 50;
        Obscured<int> b = 50;
        Assert.Equal(0, a.CompareTo(b));
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValueString()
    {
        Obscured<int> o = 42;
        Assert.Equal("42", o.ToString());
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        Obscured<int> a = 99;
        Obscured<int> b = 99;
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ReturnsDifferentHash()
    {
        Obscured<int> a = 1;
        Obscured<int> b = 2;
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ObjectOverload_SameValue_ReturnsTrue()
    {
        Obscured<int> a = 42;
        Obscured<int> b = 42;
        Assert.True(a.Equals((object)b));
    }

    [Fact]
    public void Equals_ObjectOverload_DifferentValue_ReturnsFalse()
    {
        Obscured<int> a = 42;
        Obscured<int> b = 99;
        Assert.False(a.Equals((object)b));
    }

    [Fact]
    public void Equals_ObjectOverload_NullReturnsFalse()
    {
        Obscured<int> a = 42;
        Assert.False(a.Equals(null));
    }

    [Fact]
    public void WorksWithFloat()
    {
        Obscured<float> o = 3.14f;
        Assert.Equal(3.14f, (float)o);
    }

    [Fact]
    public void WorksWithLong()
    {
        Obscured<long> o = long.MaxValue;
        Assert.Equal(long.MaxValue, o.Value);
    }

    [Fact]
    public void WorksWithBool_True()
    {
        Obscured<bool> o = true;
        Assert.True(o.Value);
    }

    [Fact]
    public void WorksWithBool_False()
    {
        Obscured<bool> o = false;
        Assert.False(o.Value);
    }

    [Fact]
    public void DefaultConstructor_Float_ValueIsDefault()
    {
        var o = new Obscured<float>();
        Assert.Equal(default(float), o.Value);
    }

    [Fact]
    public void ImplicitConversion_SameValue_ProducesEqualInstances()
    {
        Obscured<int> a = 77;
        Obscured<int> b = 77;
        Assert.Equal(a.Value, b.Value);
        Assert.True(a == b);
    }
}
