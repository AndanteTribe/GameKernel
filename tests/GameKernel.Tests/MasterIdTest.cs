namespace GameKernel.Tests;

public class MasterIdTest
{
    private enum ItemGroup : uint { Weapon = 1, Armor = 2, Accessory = 3 }
    private enum CharacterGroup : byte { Hero = 0, Villain = 1 }

    // Enum name "VeryLongGroup" is 13 chars; formatted as "VeryLongGroup.0001" = 18 chars total,
    // exceeding the initial internal 16-char buffer and triggering buffer-doubling.
    private enum VeryLongGroup : uint { VeryLongGroup = 1 }

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

    [Fact]
    public void Equals_SameGroupAndId_ReturnsTrue()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        Assert.True(id1.Equals(id2));
    }

    [Fact]
    public void Equals_DifferentGroup_ReturnsFalse()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Armor, 1u);
        Assert.False(id1.Equals(id2));
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 2u);
        Assert.False(id1.Equals(id2));
    }

    [Fact]
    public void EqualityOperator_SameValues_ReturnsTrue()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ReturnsTrue()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 2u);
        Assert.True(id1 != id2);
        Assert.False(id1 == id2);
    }

    [Fact]
    public void CompareTo_EqualValues_ReturnsZero()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        Assert.Equal(0, id1.CompareTo(id2));
    }

    [Fact]
    public void CompareTo_SmallerGroup_ReturnsNegative()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);   // Weapon = 1
        var id2 = new MasterId<ItemGroup>(ItemGroup.Armor, 1u);    // Armor = 2
        Assert.True(id1.CompareTo(id2) < 0);
        Assert.True(id2.CompareTo(id1) > 0);
    }

    [Fact]
    public void CompareTo_SameGroup_SortsByIdWhenGroupsEqual()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 2u);
        Assert.True(id1.CompareTo(id2) < 0);
        Assert.True(id2.CompareTo(id1) > 0);
    }

    [Fact]
    public void ToString_DefaultFormat_FormatsGroupAndId()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        Assert.Equal("Weapon.0001", id.ToString());
    }

    [Fact]
    public void ToString_DefaultFormat_PadsIdToFourDigits()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Armor, 42u);
        Assert.Equal("Armor.0042", id.ToString());
    }

    [Fact]
    public void ToString_WithCustomFormat_UsesProvidedFormat()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Armor, 42u);
        var result = id.ToString("D", null);
        Assert.Equal("Armor.42", result);
    }

    [Fact]
    public void TryFormat_SufficientBuffer_Succeeds()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 5u);
        var buffer = new char[32];
        var result = id.TryFormat(buffer, out var written);
        Assert.True(result);
        Assert.Equal("Weapon.0005", new string(buffer, 0, written));
    }

    [Fact]
    public void TryFormat_BufferTooSmallForEnum_ReturnsFalse()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var buffer = new char[3]; // "Weapon" is 6 chars, so 3 is too small
        var result = id.TryFormat(buffer, out var written);
        Assert.False(result);
        Assert.Equal(0, written);
    }

    [Fact]
    public void TryFormat_BufferTooSmallForSeparator_ReturnsFalse()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var buffer = new char[6]; // Exactly "Weapon" length, no room for '.'
        var result = id.TryFormat(buffer, out var written);
        Assert.False(result);
        Assert.Equal(0, written);
    }

    [Fact]
    public void TryFormat_BufferTooSmallForId_ReturnsFalse()
    {
        var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var buffer = new char[8]; // "Weapon." = 7 chars, need 4 more for "0001"
        var result = id.TryFormat(buffer, out var written);
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

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 99u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Weapon, 99u);
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ReturnsDifferentHash()
    {
        var id1 = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
        var id2 = new MasterId<ItemGroup>(ItemGroup.Armor, 2u);
        Assert.NotEqual(id1.GetHashCode(), id2.GetHashCode());
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
