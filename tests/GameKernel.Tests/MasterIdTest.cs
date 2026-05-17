namespace GameKernel.Tests;

public class MasterIdTest
{
    public enum ItemGroup : uint { Weapon = 1, Armor = 2, Accessory = 3 }
    internal enum CharacterGroup : byte { Hero = 0 }

    // Enum name "VeryLongGroup" is 13 chars; formatted as "VeryLongGroup.0001" = 18 chars total,
    // exceeding the initial internal 16-char buffer and triggering buffer-doubling.
    internal enum VeryLongGroup : uint { VeryLongGroup = 1 }

    [Fact]
    public void Constructor_SetsGroupAndId()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 42u);
        Assert.Equal(ItemGroup.Weapon, id.Group);
        Assert.Equal(42u, id.Id);
    }

    [Fact]
    public void ImplicitConversion_FromValueTuple()
    {
        MasterId<ItemGroup> id = (ItemGroup.Armor, 100u);
        Assert.Equal(ItemGroup.Armor, id.Group);
        Assert.Equal(100u, id.Id);
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 1u, true)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Armor, 1u, false)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 2u, false)]
    public void Equals_WithVaryingGroupsAndIds(ItemGroup group1, uint id1, ItemGroup group2, uint id2, bool expected)
    {
        var masterId1 = new MasterId<ItemGroup>(group1, id1);
        var masterId2 = new MasterId<ItemGroup>(group2, id2);
        Assert.Equal(expected, masterId1.Equals(masterId2));
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 1u, true)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 2u, false)]
    public void EqualityOperator_WithSameOrDifferentValues(ItemGroup group1, uint id1, ItemGroup group2, uint id2, bool shouldBeEqual)
    {
        var masterId1 = new MasterId<ItemGroup>(group1, id1);
        var masterId2 = new MasterId<ItemGroup>(group2, id2);
        Assert.Equal(shouldBeEqual, masterId1 == masterId2);
        Assert.Equal(!shouldBeEqual, masterId1 != masterId2);
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 1u, 0)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Armor, 1u, -1)]
    [InlineData(ItemGroup.Armor, 1u, ItemGroup.Weapon, 1u, 1)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Weapon, 2u, -1)]
    [InlineData(ItemGroup.Weapon, 2u, ItemGroup.Weapon, 1u, 1)]
    public void CompareTo_WithVaryingValues(ItemGroup group1, uint id1, ItemGroup group2, uint id2, int expectedComparison)
    {
        var masterId1 = new MasterId<ItemGroup>(group1, id1);
        var masterId2 = new MasterId<ItemGroup>(group2, id2);
        var result = masterId1.CompareTo(masterId2);
        if (expectedComparison == 0)
            Assert.Equal(0, result);
        else if (expectedComparison < 0)
            Assert.True(result < 0);
        else
            Assert.True(result > 0);
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 1u, "Weapon.0001")]
    [InlineData(ItemGroup.Armor, 42u, "Armor.0042")]
    [InlineData(ItemGroup.Accessory, 99u, "Accessory.0099")]
    public void ToString_WithFormattedOutput(ItemGroup group, uint id, string expected)
    {
        var masterId = new MasterId<ItemGroup>(group, id);
        Assert.Equal(expected, masterId.ToString());
    }

    [Fact]
    public void ToString_WithCustomFormat_UsesProvidedFormat()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Armor, 42u);
        var result = id.ToString("D", null);
        Assert.Equal("Armor.42", result);
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 5u, 32, true)]
    public void TryFormat_SufficientBuffer_Succeeds(ItemGroup group, uint id, int bufferSize, bool shouldSucceed)
    {
        var masterId = new MasterId<ItemGroup>(group, id);
        var buffer = new char[bufferSize];
        var result = masterId.TryFormat(buffer, out var written);
        Assert.Equal(shouldSucceed, result);
        if (shouldSucceed)
            Assert.NotEqual(0, written);
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 1u, 3)]
    [InlineData(ItemGroup.Weapon, 1u, 6)]
    [InlineData(ItemGroup.Weapon, 1u, 8)]
    public void TryFormat_BufferTooSmall_ReturnsFalse(ItemGroup group, uint id, int bufferSize)
    {
        var masterId = new MasterId<ItemGroup>(group, id);
        var buffer = new char[bufferSize];
        var result = masterId.TryFormat(buffer, out var written);
        Assert.False(result);
        Assert.Equal(0, written);
    }

    [Fact]
    public void TryFormat_WithCustomFormat_UsesProvidedFormat()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Armor, 7u);
        var buffer = new char[32];
        var result = id.TryFormat(buffer, out var written, "D");
        Assert.True(result);
        Assert.Equal("Armor.7", new string(buffer, 0, written));
    }

    [Theory]
    [InlineData(ItemGroup.Weapon, 99u, ItemGroup.Weapon, 99u, true)]
    [InlineData(ItemGroup.Weapon, 1u, ItemGroup.Armor, 2u, false)]
    public void GetHashCode_WithSameOrDifferentValues(ItemGroup group1, uint id1, ItemGroup group2, uint id2, bool shouldBeEqual)
    {
        var masterId1 = new MasterId<ItemGroup>(group1, id1);
        var masterId2 = new MasterId<ItemGroup>(group2, id2);
        if (shouldBeEqual)
            Assert.Equal(masterId1.GetHashCode(), masterId2.GetHashCode());
        else
            Assert.NotEqual(masterId1.GetHashCode(), masterId2.GetHashCode());
    }

    [Fact]
    public void WorksWithByteEnum()
    {
        var id = new MasterId<CharacterGroup>(CharacterGroup.Hero, 100u);
        Assert.Equal(CharacterGroup.Hero, id.Group);
        Assert.Equal(100u, id.Id);
        Assert.Equal("Hero.0100", id.ToString());
    }

    [Fact]
    public void RecordEquality_ViaEqualityComparer()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Accessory, 50u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Accessory, 50u);
        Assert.True(id1.Equals((object)id2));
    }

    [Fact]
    public void ToString_LongEnumName_BufferDoublesInternally()
    {
        // "VeryLongGroup" (13 chars) + "." + "0001" (4 chars) = 18 chars,
        // which exceeds the initial internal buffer of 16, causing the buffer to double.
        var id = new MasterId<VeryLongGroup>(VeryLongGroup.VeryLongGroup, 1u);
        Assert.Equal("VeryLongGroup.0001", id.ToString());
    }
}