# GameKernel

[![dotnet-test](https://github.com/AndanteTribe/GameKernel/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/AndanteTribe/GameKernel/actions/workflows/dotnet-test.yml)
[![nuget](https://img.shields.io/nuget/v/AndanteTribe.GameKernel.svg)](https://www.nuget.org/packages/AndanteTribe.GameKernel/)
[![Releases](https://img.shields.io/github/release/AndanteTribe/GameKernel.svg)](https://github.com/AndanteTribe/GameKernel/releases)
[![GitHub license](https://img.shields.io/github/license/AndanteTribe/GameKernel.svg)](./LICENSE)

[English](README.md) | 日本語

## 概要

**GameKernel** は、ゲームエンジンに依存しないゲーム開発のための共通ドメインプリミティブと値オブジェクトを提供する .NET ライブラリです。

主な機能：

1. `MasterId<TGroup>` — マスターデータ ID のための高パフォーマンスな比較・フォーマット対応の値型です。列挙型ベースのグループ識別子と数値 ID を組み合わせ、等値比較・順序比較・スパンベースの文字列フォーマットをサポートします。
2. `Obscured<T>` — ランダムに生成されたキーで XOR 暗号化することで、アンマネージド値をメモリ参照から保護する構造体です。等値比較・順序比較・透過的な暗黙的変換をサポートします。
3. `GameKernel.MessagePack` — `MasterId<TGroup>` と `Obscured<T>` の MessagePack シリアライズをサポートするオプションのパッケージです。

## インストール

### NuGet パッケージ

このライブラリには .NET Standard 2.1 以上が必要です。パッケージは NuGet から取得できます。

### .NET CLI

#### コアパッケージ

```ps1
dotnet add package AndanteTribe.GameKernel
```

#### MessagePack サポート（オプション）

```ps1
dotnet add package AndanteTribe.GameKernel.MessagePack
```

### パッケージマネージャー

#### コアパッケージ

```ps1
Install-Package AndanteTribe.GameKernel
```

#### MessagePack サポート（オプション）

```ps1
Install-Package AndanteTribe.GameKernel.MessagePack
```

## クイックスタート

### MasterId

```csharp
using GameKernel;

public enum ItemGroup { Weapon, Armor, Consumable }

// コンストラクタを使って MasterId を生成する（推奨）
var id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);

// "Weapon.0001" としてフォーマットされる
string formatted = id.ToString(); // "Weapon.0001"

// 比較・等値比較が完全にサポートされている
var other = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
bool equal = id == other; // true
```

`(TGroup, uint)` タプルからの暗黙的変換は、ルックアップメソッドなどの呼び出し側で使う便利な手段です：

```csharp
// MasterId を引数に取るメソッドへ (group, id) タプルを渡してアイテムを検索する
Item found = masterTable.Find((ItemGroup.Weapon, 1u));
```

### Obscured

推奨パターンは `Obscured<T>` を **private** フィールドに留め、パブリックプロパティで平文の値を公開する方法です：

```csharp
using GameKernel;

public class PlayerStats
{
    private Obscured<int> _hp;
    private Obscured<float> _attack;

    public int Hp
    {
        get => (int)_hp;
        set => _hp = value;
    }

    public float Attack
    {
        get => (float)_attack;
        set => _attack = value;
    }
}
```

## MasterId\<TGroup\>

`MasterId<TGroup>` は、列挙型の `Group` と `uint` の `Id` を組み合わせた読み取り専用レコード構造体です。

- `IEquatable<T>`・`IComparable<T>`・`ISpanFormattable` を実装しています。
- デフォルトのフォーマットは `"{Group}.{Id:0000}"` です（例：`"Weapon.0001"`）。数値部分のフォーマット指定子を指定することも可能です。
- `ValueTuple<TGroup, uint>` からの暗黙的変換は、呼び出し側のリテラルタプルを `MasterId<TGroup>` を引数に取るメソッドへ渡す際の省略記法として提供されています。

```csharp
// コンストラクタで生成する（推奨）
var id = new MasterId<ItemGroup>(ItemGroup.Armor, 42u);

// 数値部分のカスタムフォーマット
string s = id.ToString("00000"); // "Armor.00042"
```

## Obscured\<T\>

`Obscured<T>` は、任意のアンマネージド値をインスタンスごとのランダムキーで XOR 暗号化して保持し、生のメモリダンプから実際の値が見えないようにします。

推奨パターンは `Obscured<T>` を **private** フィールドに留め、パブリックプロパティで平文の値を公開する方法です：

```csharp
public class PlayerStats
{
    private Obscured<int> _hp;

    public int Hp
    {
        get => (int)_hp;
        set => _hp = value;
    }
}
```

- 真の値は `Value` プロパティまたは `T` への暗黙的変換によりオンデマンドで取得できます。
- `IEquatable<T>` と `IComparable<T>` は復号化した値に対して動作します。

## MessagePack サポート

`AndanteTribe.GameKernel.MessagePack` パッケージは以下を提供します：

- `GameKernelResolver` — `MasterId<TGroup>` と `Obscured<T>` のフォーマッターを解決する `IFormatterResolver`。
- `MasterIdFormatter<TGroup>` — `MasterId<TGroup>` のシリアライズ・デシリアライズを行う `IMessagePackFormatter<MasterId<TGroup>>`。
- `ObscuredFormatter<T>` — `Obscured<T>` のシリアライズ・デシリアライズを行う `IMessagePackFormatter<Obscured<T>>`（平文の値としてシリアライズされます）。

```csharp
using MessagePack;
using GameKernel.MessagePack;

var options = MessagePackSerializerOptions.Standard
    .WithResolver(MessagePack.Resolvers.CompositeResolver.Create(
        GameKernelResolver.Shared,
        MessagePack.Resolvers.StandardResolver.Instance));

MasterId<ItemGroup> id = new MasterId<ItemGroup>(ItemGroup.Weapon, 1u);
byte[] bytes = MessagePackSerializer.Serialize(id, options);
MasterId<ItemGroup> deserialized = MessagePackSerializer.Deserialize<MasterId<ItemGroup>>(bytes, options);
```

## ライセンス

このライブラリは MIT ライセンスのもとで公開されています。
