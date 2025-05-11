// リフレクションを回避するために特殊化したクラスを実装します。

namespace Potisan.Windows.BluetoothLE;

using KnownShortUuids;

/// <summary>
/// BluetoothLEのUUID。
/// </summary>
public class BluetoothLEUuid
{
	public Guid? LongUuid { get; }
	public ushort? ShortUuid { get; }

	internal BluetoothLEUuid(in BTH_LE_UUID uuid)
	{
		LongUuid = uuid.IsShortUuid == 0 ? uuid.Value.LongUuid : null;
		ShortUuid = uuid.IsShortUuid != 0 ? uuid.Value.ShortUuid : null;
	}

	public Guid ToGuid()
		=> ShortUuid is { } shortUuid
			// BTH_LE_ATT_BLUETOOTH_BASE_GUID
			? new(shortUuid, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB)
			: LongUuid!.Value;

	public override string ToString() => ToGuid().ToString("B");
}

public class BluetoothLEServiceUuid : BluetoothLEUuid
{
	internal BluetoothLEServiceUuid(in BTH_LE_UUID uuid) : base(uuid) { }

	public override string ToString()
	{
		if (ShortUuid is { } shortUuid && Enum.GetName((KnownServiceShortUuid)shortUuid) is { } name)
			return $"{name} (0x{shortUuid:X04})";
		return base.ToString();
	}
}

public class BluetoothLECharacteristicUuid : BluetoothLEUuid
{
	internal BluetoothLECharacteristicUuid(in BTH_LE_UUID uuid) : base(uuid) { }

	public override string ToString()
	{
		if (ShortUuid is { } shortUuid && Enum.GetName((KnownCharacteristicsShortUuid)shortUuid) is { } name)
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
