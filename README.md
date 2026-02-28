# GameKernel

[![dotnet-test](https://github.com/AndanteTribe/GameKernel/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/AndanteTribe/GameKernel/actions/workflows/dotnet-test.yml)
[![nuget](https://img.shields.io/nuget/v/AndanteTribe.GameKernel.svg)](https://www.nuget.org/packages/AndanteTribe.GameKernel/)
[![Releases](https://img.shields.io/github/release/AndanteTribe/GameKernel.svg)](https://github.com/AndanteTribe/GameKernel/releases)
[![GitHub license](https://img.shields.io/github/license/AndanteTribe/GameKernel.svg)](./LICENSE)

English | [日本語](README_JA.md)

## Overview

**GameKernel** is a .NET library that provides shared domain primitives and value objects for engine-agnostic game development.

It provides the following:

1. `MasterId<TGroup>` — A high-performance, comparable, and formattable value type for master data IDs. It combines an enum-based group discriminator with a numeric ID, supporting equality, ordering, and span-based string formatting.
2. `Obscured<T>` — A struct that protects an unmanaged value from memory inspection by XOR-encrypting it with a randomly generated key. Supports equality, comparison, and transparent implicit conversions.
3. `GameKernel.MessagePack` — An optional package providing MessagePack serialization support for `MasterId<TGroup>` and `Obscured<T>`.

## Installation

### NuGet Packages

This library requires .NET Standard 2.1 or higher. The packages can be obtained from NuGet.

### .NET CLI

#### Core package

```ps1
dotnet add package AndanteTribe.GameKernel
```

#### MessagePack support (optional)

```ps1
dotnet add package AndanteTribe.GameKernel.MessagePack
```

### Package Manager

#### Core package

```ps1
Install-Package AndanteTribe.GameKernel
```

#### MessagePack support (optional)

```ps1
Install-Package AndanteTribe.GameKernel.MessagePack
```

## Quick Start

### MasterId

```csharp
using GameKernel;

public enum ItemGroup { Weapon, Armor, Consumable }

// Create a MasterId from a (group, id) tuple
MasterId<ItemGroup> id = (ItemGroup.Weapon, 1u);

// Format as "Weapon.0001"
string formatted = id.ToString(); // "Weapon.0001"

// Comparison and equality are fully supported
var other = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
bool equal = id == other; // true
```

### Obscured

```csharp
using GameKernel;

// Store a value in memory-obscured form
Obscured<int> score = 9999;

// Read back the original value transparently
int raw = score; // 9999

// Equality and comparison work on the unobscured value
Obscured<int> copy = 9999;
bool equal = score == copy; // true
```

## MasterId\<TGroup\>

`MasterId<TGroup>` is a readonly record struct that pairs an enum `Group` with a `uint` `Id`.

- Implements `IEquatable<T>`, `IComparable<T>`, and `ISpanFormattable`.
- Default format is `"{Group}.{Id:0000}"` (e.g. `"Weapon.0001"`), with an optional format specifier to control the numeric format.
- Provides an implicit conversion from `ValueTuple<TGroup, uint>`.

```csharp
MasterId<ItemGroup> id = (ItemGroup.Armor, 42u);

// Custom format for the numeric part
string s = id.ToString("00000"); // "Armor.00042"
```

## Obscured\<T\>

`Obscured<T>` stores any `unmanaged` value XOR-encrypted with a per-instance random key, making the real value invisible in raw memory dumps.

- The true value is recovered on demand via the `Value` property or implicit conversion to `T`.
- Supports `IEquatable<T>` and `IComparable<T>` operating on the decrypted value.
- Implicit conversions between `T` and `Obscured<T>` allow drop-in usage.

```csharp
Obscured<float> hp = 100.0f;
float current = hp; // 100.0f
```

## MessagePack Support

The `AndanteTribe.GameKernel.MessagePack` package provides:

- `GameKernelResolver` — An `IFormatterResolver` that resolves formatters for `MasterId<TGroup>` and `Obscured<T>`.
- `MasterIdFormatter<TGroup>` — An `IMessagePackFormatter<MasterId<TGroup>>` for serializing and deserializing `MasterId<TGroup>` instances.
- `ObscuredFormatter<T>` — An `IMessagePackFormatter<Obscured<T>>` for serializing and deserializing `Obscured<T>` instances (serializes the plain value).

```csharp
using MessagePack;
using GameKernel.MessagePack;

var options = MessagePackSerializerOptions.Standard
    .WithResolver(MessagePack.Resolvers.CompositeResolver.Create(
        GameKernelResolver.Shared,
        MessagePack.Resolvers.StandardResolver.Instance));

MasterId<ItemGroup> id = (ItemGroup.Weapon, 1u);
byte[] bytes = MessagePackSerializer.Serialize(id, options);
MasterId<ItemGroup> deserialized = MessagePackSerializer.Deserialize<MasterId<ItemGroup>>(bytes, options);
```

## License

This library is released under the MIT license.
