// リフレクションを回避するために特殊化したクラスを実装します。

namespace Potisan.Windows.BluetoothLE;

using KnownShortUuids;

/// <summary>
/// BluetoothLEのUUID。
/// </summary>
public class BluetoothLEUuid
{
	/// <summary>
	/// 長いUUID。カスタムUUIDです。
	/// </summary>
	public Guid? LongUuid { get; }

	/// <summary>
	/// 短いUUID（16ビットUUID）。定義済みのUUIDです。
	/// </summary>
	public ushort? ShortUuid { get; }

	internal BluetoothLEUuid(in BTH_LE_UUID uuid)
	{
		LongUuid = uuid.IsShortUuid == 0 ? uuid.Value.LongUuid : null;
		ShortUuid = uuid.IsShortUuid != 0 ? uuid.Value.ShortUuid : null;
	}

	/// <summary>
	/// GUID表現を取得します。
	/// </summary>
	public Guid ToGuid()
		=> ShortUuid is { } shortUuid
			// BTH_LE_ATT_BLUETOOTH_BASE_GUID
			? new(shortUuid, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB)
			: LongUuid!.Value;

	/// <summary>
	/// GUID表現の文字列表現を取得します。
	/// </summary>
	public override string ToString() => ToGuid().ToString("B");
}

/// <summary>
/// BluetoothLEのサービスUUID。定義済みのUUIDを適切に書式化します。
/// </summary>
public class BluetoothLEServiceUuid : BluetoothLEUuid
{
	internal BluetoothLEServiceUuid(in BTH_LE_UUID uuid) : base(uuid) { }

	/// <inheritdoc/>
	public override string ToString()
	{
		if (ShortUuid is { } shortUuid && Enum.GetName((KnownServiceShortUuid)shortUuid) is { } name)
			return $"{name} (0x{shortUuid:X04})";
		return base.ToString();
	}
}

/// <summary>
/// BluetoothLEの特性UUID。定義済みのUUIDを適切に書式化します。
/// </summary>
public class BluetoothLECharacteristicUuid : BluetoothLEUuid
{
	internal BluetoothLECharacteristicUuid(in BTH_LE_UUID uuid) : base(uuid) { }

	/// <inheritdoc/>
	public override string ToString()
	{
		if (ShortUuid is { } shortUuid && Enum.GetName((KnownCharacteristicsShortUuid)shortUuid) is { } name)
			return $"{name} (0x{shortUuid:X04})";
		return base.ToString();
	}
}

/// <summary>
/// BluetoothLEの特性記述子UUID。定義済みのUUIDを適切に書式化します。
/// </summary>
public class BluetoothLEDescriptorUuid : BluetoothLEUuid
{
	internal BluetoothLEDescriptorUuid(in BTH_LE_UUID uuid) : base(uuid) { }

	/// <inheritdoc/>
	public override string ToString()
	{
		if (ShortUuid is { } shortUuid && Enum.GetName((KnownDescriptorShortUuid)shortUuid) is { } name)
			return $"{name} (0x{shortUuid:X04})";
		return base.ToString();
	}
}

internal readonly struct BTH_LE_UUID
{
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct DummyUnion
	{
		[FieldOffset(0)]
		public readonly ushort ShortUuid;
		[FieldOffset(0)]
		public readonly Guid LongUuid;
	}

	public readonly byte IsShortUuid;
	public readonly DummyUnion Value;

	public readonly Guid ToGuid()
		=> IsShortUuid != 0
			// BTH_LE_ATT_BLUETOOTH_BASE_GUID
			? new(Value.ShortUuid, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB)
			: Value.LongUuid;
}
