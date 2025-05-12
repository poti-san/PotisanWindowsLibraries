using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Potisan.Windows.BluetoothLE.KnownShortUuids;

namespace Potisan.Windows.BluetoothLE;

/// <summary>
/// BluetoothLEデバイスとGATTサービスデバイスの共通インターフェイス。
/// </summary>
public interface IBluetoothLEDevice : IDisposable
{
	/// <summary>
	/// デバイスハンドル。実体はデバイスパスのファイルハンドルです。
	/// </summary>
	SafeHandle Handle { get; }

	/// <summary>
	/// デバイスインターフェイスGUID。
	/// </summary>
	Guid InterfaceGuid { get; }
}

/// <summary>
/// BluetoothLEデバイスとGATTサービスデバイスの共通実装。
/// </summary>
public abstract class BluetoothLEDeviceBase : IBluetoothLEDevice
{
	/// <inheritdoc/>
	public abstract Guid InterfaceGuid { get; }

	internal FileStream _file;

	internal BluetoothLEDeviceBase(FileStream file)
	{
		_file = file;
	}

	/// <inheritdoc/>
	public SafeHandle Handle => _file.SafeFileHandle;

	/// <inheritdoc/>
	public void Dispose()
	{
		_file.Dispose();
		GC.SuppressFinalize(this);
	}
}

/// <summary>
/// BluetoothLEデバイス。操作可能なハンドルを持ちます。
/// </summary>
public sealed class BluetoothLEDevice : BluetoothLEDeviceBase
{
	/// <inheritdoc/>
	public override Guid InterfaceGuid { get; }

	internal BluetoothLEDevice(BluetoothLEDeviceInfo info)
		: base(File.Open(info.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
	{
		InterfaceGuid = info.InterfaceGuid;
	}

	/// <summary>
	/// デバイスで使用できる全てのプライマリサービスを取得します。
	/// </summary>
	public ImmutableArray<BluetoothLEGattServiceForDevice> Services
	{
		get
		{
			const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);

			var hr = NativeMethods.BluetoothGATTGetServices(Handle, 0, null, out var required, 0);
			if (hr != HR_ERROR_MORE_DATA)
				Marshal.ThrowExceptionForHR(hr);

			var infos = new BTH_LE_GATT_SERVICE[required];
			hr = NativeMethods.BluetoothGATTGetServices(Handle, required, infos, out _, 0);
			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);
			return [.. infos.Select(info => new BluetoothLEGattServiceForDevice(Handle, info, InterfaceGuid))];
		}
	}

	/// <summary>
	/// UUIDを指定して単一のプライマリサービスを取得します。
	/// </summary>
	/// <param name="uuid">プライマリサービスのUUID。</param>
	/// <remarks>
	/// 対応するプライマリサービスが見つからないか複数存在する場合、このメソッドは失敗します。
	/// </remarks>
	public BluetoothLEGattServiceForDevice? GetSingleServiceByUuid(KnownServiceShortUuid uuid)
	{
		return Services.SingleOrDefault(service => service.ServiceUuid.ShortUuid is { } shortUuid && shortUuid == (ushort)uuid);
	}
}

/// <summary>
/// BluetoothLE GATTサービス。デバイスとGATTサービスの共通インターフェイスでもあります。
/// デバイスとして開かれたBluetoothLE GATTサービスデバイスとBluetoothLEデバイスから取得されたBluetoothLE GATTサービスデバイス情報をまとめます。
/// </summary>
public interface IBluetoothLEGattService
{
	/// <summary>
	/// サービスデバイスハンドル。実体はデバイスパスのファイルハンドルです。
	/// </summary>
	SafeHandle Handle { get; }

	/// <summary>
	/// プロファイルの全ての特性を取得します。
	/// </summary>
	ImmutableArray<BluetoothLEGattCharacteristic> Characteristics { get; }
}

/// <summary>
/// BluetoothLE GATTサービスの機能。
/// </summary>
public static class IBluetoothLEGattServiceExtensions
{
	/// <summary>
	/// 信頼性の高い書き込みを開始します。
	/// 完了後は適切に<see cref="EndReliableWrite(IBluetoothLEGattService, ulong)"/>または<see cref="AbortReliableWrite(IBluetoothLEGattService, ulong)"/>を呼び出してください。
	/// </summary>
	/// <param name="service">対象となるGATTサービス。</param>
	public static ulong BeginReliableWrite(this IBluetoothLEGattService service)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTBeginReliableWrite(service.Handle, out var ctx, 0));
		return ctx;
	}

	/// <summary>
	/// 信頼性の高い書き込みを終了します。
	/// </summary>
	/// <param name="service">対象となるGATTサービス。</param>
	/// <param name="context">開始時の戻り値。</param>
	public static void EndReliableWrite(this IBluetoothLEGattService service, ulong context)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTEndReliableWrite(service.Handle, context, 0));
	}

	/// <summary>
	/// 信頼性の高い書き込みを中断します。
	/// </summary>
	/// <param name="service">対象となるGATTサービス。</param>
	/// <param name="context">開始時の戻り値。</param>
	public static void AbortReliableWrite(this IBluetoothLEGattService service, ulong context)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTAbortReliableWrite(service.Handle, context, 0));
	}

	/// <summary>
	/// UUIDに対応する単一の特性を取得します。特性が存在しないか複数存在する場合、メソッドは失敗します。
	/// </summary>
	public static BluetoothLEGattCharacteristic? GetSingleCharacteristicByUuid(
		this IBluetoothLEGattService service,
		KnownCharacteristicsShortUuid uuid)
	{
		return service.Characteristics.SingleOrDefault(
			characteristics => characteristics.CharacteristicUuid.ShortUuid is { } shortUuid
			&& shortUuid == (ushort)uuid);
	}
}

/// <summary>
/// BluetoothLE GATTサービスデバイス。操作可能なハンドルを持ちます。
/// </summary>
public sealed class BluetoothLEGattServiceDevice : BluetoothLEDeviceBase, IBluetoothLEGattService
{
	/// <inheritdoc/>
	public override Guid InterfaceGuid { get; }

	internal BluetoothLEGattServiceDevice(BluetoothLEGattServiceDeviceInfo info)
		: base(File.Open(info.Path, FileMode.Open, FileAccess.Read, 0))
	{
		InterfaceGuid = info.InterfaceGuid;
	}

	/// <inheritdoc/>
	public ImmutableArray<BluetoothLEGattCharacteristic> Characteristics
		=> BluetoothLEGattServiceForDevice.GetCharacteristics(Handle, Unsafe.NullRef<BTH_LE_GATT_SERVICE>(), InterfaceGuid);

	/// <summary>
	/// サービスで使用可能な全てのサービスを取得します。
	/// </summary>
	public ImmutableArray<BluetoothLEGattServiceForDevice> IncludedServices
		=> BluetoothLEGattServiceForDevice.GetIncludedServices(Handle, Unsafe.NullRef<BTH_LE_GATT_SERVICE>(), InterfaceGuid);
}

/// <summary>
/// BluetoothLEデバイスから取得したBluetoothLE GATTサービスデバイス情報。操作可能なハンドルを作成できます。
/// GATTプロファイルのサービスでもあります。
/// </summary>
public class BluetoothLEGattServiceForDevice : IBluetoothLEGattService
{
	/// <inheritdoc/>
	public SafeHandle Handle { get; }

	private readonly BTH_LE_GATT_SERVICE _service;

	/// <summary>
	/// デバイスインターフェイスGUID。
	/// </summary>
	public Guid InterfaceGuid { get; }

	internal BluetoothLEGattServiceForDevice(SafeHandle handle, in BTH_LE_GATT_SERVICE service, in Guid interfaceGuid)
	{
		Handle = handle;
		_service = service;
		InterfaceGuid = interfaceGuid;
	}

	/// <summary>
	/// BluetoothLE GATTサービスUUID。
	/// </summary>
	public BluetoothLEServiceUuid ServiceUuid => new(_service.ServiceUuid);

	/// <summary>
	/// 属性ハンドル。
	/// </summary>
	public ushort AttributeHandle => _service.AttributeHandle;

	internal static ImmutableArray<BluetoothLEGattCharacteristic> GetCharacteristics(SafeHandle handle, in BTH_LE_GATT_SERVICE service, in Guid interfaceGuid)
	{
		const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);

		var hr = NativeMethods.BluetoothGATTGetCharacteristics(handle, service, 0, null, out var required, 0);
		if (hr != HR_ERROR_MORE_DATA)
			Marshal.ThrowExceptionForHR(hr);
		var infos = new BTH_LE_GATT_CHARACTERISTIC[required];
		hr = NativeMethods.BluetoothGATTGetCharacteristics(handle, service, required, infos, out _, 0);
		if (hr != 0)
			Marshal.ThrowExceptionForHR(hr);

		var results = new BluetoothLEGattCharacteristic[infos.Length];
		for (uint i = 0; i < results.Length; i++)
			results[i] = new BluetoothLEGattCharacteristic(handle, interfaceGuid, infos[i]);
		return ImmutableCollectionsMarshal.AsImmutableArray(results);
	}

	/// <inheritdoc/>
	public ImmutableArray<BluetoothLEGattCharacteristic> Characteristics => GetCharacteristics(Handle, _service, _service.ServiceUuid.ToGuid());

	internal static ImmutableArray<BluetoothLEGattServiceForDevice> GetIncludedServices(
		SafeHandle handle,
		in BTH_LE_GATT_SERVICE service,
		in Guid interfaceGuid)
	{
		const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);
		const int HR_ERROR_NOT_FOUND = unchecked((int)0x80070490);

		var hr = NativeMethods.BluetoothGATTGetIncludedServices(handle, service, 0, null, out var required, 0);
		if (hr == HR_ERROR_NOT_FOUND) return [];
		if (hr != HR_ERROR_MORE_DATA) Marshal.ThrowExceptionForHR(hr);

		var infos = new BTH_LE_GATT_SERVICE[required];
		hr = NativeMethods.BluetoothGATTGetIncludedServices(handle, service, required, infos, out _, 0);
		if (hr != 0)
			Marshal.ThrowExceptionForHR(hr);
		var services = new BluetoothLEGattServiceForDevice[required];
		for (int i = 0; i < required; i++)
			services[i] = new(handle, service, interfaceGuid);
		return [.. services];
	}

	/// <summary>
	/// サービスで使用可能な全てのサービスを取得します。
	/// </summary>
	public ImmutableArray<BluetoothLEGattServiceForDevice> IncludedServices => GetIncludedServices(Handle, _service, InterfaceGuid);

	/// <summary>
	/// BluetoothLE GATTサービスデバイス情報を取得します。
	/// </summary>
	public BluetoothLEGattServiceDeviceInfo? GetServiceDeviceInfo()
	{
		return BluetoothLE.CreateGattServiceDeviceInfoForInterface(ServiceUuid.ToGuid(), presentItems: true);
	}
}

/// <summary>
/// BluetoothLE GATTプロファイルの特性。
/// </summary>
public sealed class BluetoothLEGattCharacteristic
{
	private readonly SafeHandle _handle;

	/// <summary>
	/// BluetoothLE GATTプロファイルの所属するBluetoothLE GATTサービスのインターフェイスGUID。
	/// </summary>
	public Guid ServiceInterfaceId { get; }

	internal readonly BTH_LE_GATT_CHARACTERISTIC _characteristic;

	internal BluetoothLEGattCharacteristic(SafeHandle handle, in Guid serviceInterfaceId, in BTH_LE_GATT_CHARACTERISTIC characteristic)
	{
		_handle = handle;
		ServiceInterfaceId = serviceInterfaceId;
		_characteristic = characteristic;
	}

	/// <summary>
	/// サービスハンドル。
	/// </summary>
	public ushort ServiceHandle => _characteristic.ServiceHandle;

	/// <summary>
	/// 特性UUID。
	/// </summary>
	public BluetoothLECharacteristicUuid CharacteristicUuid => new(_characteristic.CharacteristicUuid);

	/// <summary>
	/// 属性ハンドル。
	/// </summary>
	public ushort AttributeHandle => _characteristic.AttributeHandle;

	/// <summary>
	/// 特性値ハンドル。
	/// </summary>
	public ushort CharacteristicValueHandle => _characteristic.CharacteristicValueHandle;

	/// <summary>
	/// ブロードキャスト可能。
	/// </summary>
	public bool IsBroadcastable => _characteristic.IsBroadcastable != 0;

	/// <summary>
	/// 読み取り可能。
	/// </summary>
	public bool IsReadable => _characteristic.IsReadable != 0;

	/// <summary>
	/// 書き込み可能。
	/// </summary>
	public bool IsWritable => _characteristic.IsWritable != 0;

	/// <summary>
	/// 応答なしで書き込み可能。
	/// </summary>
	public bool IsWritableWithoutResponse => _characteristic.IsWritableWithoutResponse != 0;

	/// <summary>
	/// 署名済み書き込み可能。
	/// </summary>
	public bool IsSignedWritable => _characteristic.IsWritable != 0;

	/// <summary>
	/// 通知可能。値はコールバック関数を介して返されます。
	/// </summary>
	public bool IsNotifiable => _characteristic.IsNotifiable != 0;

	/// <summary>
	/// 指示可能。値はコールバック関数を介して返されます。
	/// </summary>
	public bool IsIndicatable => _characteristic.IsIndicatable != 0;

	/// <summary>
	/// 拡張プロパティがあるか。
	/// </summary>
	public bool HasExtendedProperties => _characteristic.HasExtendedProperties != 0;

	/// <summary>
	/// 全ての特性記述子を取得します。
	/// </summary>
	public ImmutableArray<BluetoothLEGattDescriptor> Descriptors
	{
		get
		{
			const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);
			const int HR_ERROR_NOT_FOUND = unchecked((int)0x80070490);

			var hr = NativeMethods.BluetoothGATTGetDescriptors(_handle, _characteristic, 0, null, out var required, 0);
			if (hr != HR_ERROR_MORE_DATA)
			{
				if (hr == HR_ERROR_NOT_FOUND) return [];
				Marshal.ThrowExceptionForHR(hr);
			}

			var infos = new BTH_LE_GATT_DESCRIPTOR[required];
			hr = NativeMethods.BluetoothGATTGetDescriptors(_handle, _characteristic, required, infos, out _, 0);
			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);
			return [.. infos.Select(info => new BluetoothLEGattDescriptor(_handle, this, info))];
		}
	}

	/// <summary>
	/// 特性値を取得します。
	/// </summary>
	/// <param name="flags">取得方法。</param>
	public byte[] GetValue(BluetoothLEGattFlag flags = BluetoothLEGattFlag.None)
	{
		if (!IsReadable)
			throw new InvalidOperationException("特性は読み取り不可能です。");

		using var serviceHandle = BluetoothLE.OpenInterfaceHandle(ServiceInterfaceId);

		var hr = NativeMethods.BluetoothGATTGetCharacteristicValue(serviceHandle, _characteristic,
			0, ref Unsafe.NullRef<byte>(), out var required, (uint)flags);
		const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);
		if (hr != HR_ERROR_MORE_DATA)
			Marshal.ThrowExceptionForHR(hr);

		var buffer = ArrayPool<byte>.Shared.Rent(required);
		try
		{
			hr = NativeMethods.BluetoothGATTGetCharacteristicValue(serviceHandle, _characteristic,
				required, ref MemoryMarshal.GetArrayDataReference(buffer), out _, (uint)flags);
			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);
			return [.. buffer.AsSpan(
				(int)Marshal.OffsetOf<BTH_LE_GATT_CHARACTERISTIC_VALUE_1>(nameof(BTH_LE_GATT_CHARACTERISTIC_VALUE_1.Data0)),
				(int)BinaryPrimitives.ReadUInt32LittleEndian(buffer))];
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	/// <summary>
	/// 特性値を取得します。失敗時は<c>null</c>を返します。
	/// </summary>
	/// <param name="flags">取得方法。</param>
	public byte[]? TryGetValue(BluetoothLEGattFlag flags = BluetoothLEGattFlag.None)
	{
		try
		{
			return GetValue(flags);
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// 既定の方法で特性値を取得します。
	/// </summary>
	public byte[] Value => GetValue();

	/// <summary>
	/// 既定の方法で特性値を取得します。失敗時は<c>null</c>を返します。
	/// </summary>
	public byte[]? ValueOrNull => TryGetValue();

	/// <summary>
	/// 特性値を設定します。
	/// </summary>
	/// <param name="value">新しい値。</param>
	/// <param name="flags">設定方法。</param>
	/// <param name="reliableWriteContext">信頼性の高い書き込みのコンテキスト。
	/// <see cref="IBluetoothLEGattServiceExtensions.BeginReliableWrite(IBluetoothLEGattService)"/></param>の戻り値です。
	/// <exception cref="InvalidOperationException"></exception>
	public void SetValue(ReadOnlySpan<byte> value, BluetoothLEGattFlag flags = BluetoothLEGattFlag.None, ulong? reliableWriteContext = null)
	{
		if (!IsWritable)
			throw new InvalidOperationException("特性は読み取り不可能です。");

		var dataOffset = (int)Marshal.OffsetOf<BTH_LE_GATT_CHARACTERISTIC_VALUE_1>(
			nameof(BTH_LE_GATT_CHARACTERISTIC_VALUE_1.Data0));
		var buffer = ArrayPool<byte>.Shared.Rent(dataOffset + value.Length);
		BinaryPrimitives.WriteInt32LittleEndian(buffer, value.Length);
		value.CopyTo(buffer.AsSpan(dataOffset));
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTSetCharacteristicValue(
			_handle, _characteristic, in MemoryMarshal.GetArrayDataReference(buffer),
			reliableWriteContext ?? Unsafe.NullRef<ulong>(), (uint)flags));
	}

	/// <summary>
	/// 通知イベントのコールバック関数を登録します。
	/// </summary>
	/// <param name="eventType">イベントの種類。</param>
	/// <param name="callback">コールバック関数。</param>
	/// <param name="context">コールバック関数に渡される値。</param>
	/// <exception cref="NotSupportedException">特性は通知非対応です。</exception>
	public BluetoothLEGattEvent RegisterEvent(
		BluetoothLEGattEventType eventType,
		BluetoothLEGattEventCallback callback,
		nint context = 0)
	{
		if (!IsNotifiable)
			throw new NotSupportedException("特性は通知非対応です。");

		using var serviceHandle = BluetoothLE.OpenInterfaceHandle(ServiceInterfaceId);
		var eventReg = new BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION_1
		{
			NumCharacteristics = 1,
			Characteristics0 = _characteristic,
		};
		var hr = NativeMethods.BluetoothGATTRegisterEvent(
			serviceHandle, eventType, eventReg, callback, context, out var eventHandle, 0);
		Marshal.ThrowExceptionForHR(hr);
		return new(eventHandle, callback);
	}
}

/// <summary>
/// 読み書きの方法。
/// </summary>
[Flags]
public enum BluetoothLEGattFlag : uint
{
	/// <summary>
	/// 特定のGATT要件はありません。
	/// </summary>
	None = 0x00000000,
	/// <summary>
	/// 暗号化された送信を要求します。
	/// </summary>
	ConnectionEncrypted = 0x00000001,
	/// <summary>
	/// 認証された送信を要求します。
	/// </summary>
	ConnectionAuthenticated = 0x00000002,
	/// <summary>
	/// 常にデバイスから直接読み取ります。
	/// </summary>
	ForceReadFromDevice = 0x00000004,
	/// <summary>
	/// 常にキャッシュから読み込みます。
	/// </summary>
	ForceReadFromCache = 0x00000008,
	/// <summary>
	/// 署名付きで書き込みます。
	/// </summary>
	SignedWrite = 0x00000010,
	/// <summary>
	/// 応答なしで書き込みます。
	/// </summary>
	WriteWithoutResponse = 0x00000020,
	/// <summary>
	/// 全て取得します。
	/// </summary>
	ReturnAll = 0x00000040,
}

/// <summary>
/// GATTプロファイルの特性記述子。
/// </summary>
public sealed class BluetoothLEGattDescriptor
{
	private readonly SafeHandle _handle;

	/// <summary>
	/// 特性記述子を含む特性。
	/// </summary>
	public BluetoothLEGattCharacteristic ParentCharacteristic { get; }

	private readonly BTH_LE_GATT_DESCRIPTOR _descriptor;

	internal BluetoothLEGattDescriptor(SafeHandle handle, BluetoothLEGattCharacteristic parentCharacteristic, in BTH_LE_GATT_DESCRIPTOR descriptor)
	{
		_handle = handle;
		ParentCharacteristic = parentCharacteristic;
		_descriptor = descriptor;
	}

	/// <summary>
	/// サービスハンドル。
	/// </summary>
	public ushort ServiceHandle => _descriptor.ServiceHandle;

	/// <summary>
	/// 特性ハンドル。
	/// </summary>
	public ushort CharacteristicHandle => _descriptor.CharacteristicHandle;

	/// <summary>
	/// 記述子の種類。
	/// </summary>
	public BluetoothLEGattDescriptorType DescriptorType => _descriptor.DescriptorType;

	/// <summary>
	/// BluetoothLE GATT記述子UUID。
	/// </summary>
	public BluetoothLEDescriptorUuid DescriptorUuid => new(_descriptor.DescriptorUuid);

	/// <summary>
	/// 属性ハンドル。
	/// </summary>
	public ushort AttributeHandle => _descriptor.AttributeHandle;

	/// <summary>
	/// 特性記述子の値を取得します。
	/// NOTE：動作確認環境がないため、動作未確認です。
	/// </summary>
	public BluetoothLEGattDescriptorValue GetValue(BluetoothLEGattFlag flags = BluetoothLEGattFlag.None)
	{
		using var interfaceHandle = BluetoothLE.OpenInterfaceHandle(ParentCharacteristic.ServiceInterfaceId, FileAccess.Read);

		var hr = NativeMethods.BluetoothGATTGetDescriptorValue(
			_handle, _descriptor, 0, ref Unsafe.NullRef<byte>(), out var required, (uint)flags);
		const int HR_ERROR_MORE_DATA = unchecked((int)0x800700ea);
		if (hr != HR_ERROR_MORE_DATA)
			Marshal.ThrowExceptionForHR(hr);

		var buffer = GC.AllocateUninitializedArray<byte>(required);
		//ref var hdr = ref MemoryMarshal.AsRef<BTH_LE_GATT_DESCRIPTOR_VALUE_1>(buffer.AsSpan());
		//hdr.DataSize = checked(required - (uint)Marshal.SizeOf<BTH_LE_GATT_DESCRIPTOR_VALUE_1>());
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTGetDescriptorValue(
			_handle, _descriptor, required, ref MemoryMarshal.GetArrayDataReference(buffer), out Unsafe.NullRef<ushort>(), (uint)flags));
		return new(buffer);
	}

	/// <summary>
	/// 特性記述子の値を取得します。失敗時は<c>null</c>を返します。
	/// NOTE：動作確認環境がないため、動作未確認です。
	/// </summary>
	public BluetoothLEGattDescriptorValue? TryGetValue(BluetoothLEGattFlag flags = BluetoothLEGattFlag.None)
	{
		try
		{
			return GetValue(flags);
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// 既定の方法で特性記述子の値を取得します。
	/// NOTE：動作確認環境がないため、動作未確認です。
	/// </summary>
	public BluetoothLEGattDescriptorValue Value => GetValue();

	/// <summary>
	/// 既定の方法で特性記述子の値を取得します。失敗時は<c>null</c>を返します。
	/// NOTE：動作確認環境がないため、動作未確認です。
	/// </summary>
	public BluetoothLEGattDescriptorValue? ValueOrNull => TryGetValue();

	// NOTE
	// 値の書き込み機能は未実装です。
	// 必要に応じて値の種類別の書き込みメソッドを作成します。
}

/// <summary>
/// GATTプロファイルの特性記述子値。
/// </summary>
/// <remarks>
/// <c>BTH_LE_GATT_DESCRIPTOR_VALUE</c>構造体のラッパーです。
/// </remarks>
public sealed class BluetoothLEGattDescriptorValue
{
	private readonly byte[] _raw;
	private BTH_LE_GATT_DESCRIPTOR_VALUE_1 _value1;

	internal BluetoothLEGattDescriptorValue(byte[] raw)
	{
		_raw = raw;
		_value1 = MemoryMarshal.Cast<byte, BTH_LE_GATT_DESCRIPTOR_VALUE_1>(raw)[0];
	}

	/// <summary>
	/// 特性拡張プロパティ型。
	/// </summary>
	public readonly struct CharacteristicExtendedPropertiesType
	{
		/// <summary>
		/// 信頼性の高い書き込みが有効か。
		/// </summary>
		public readonly bool IsReliableWriteEnabled;
		/// <summary>
		/// 書き込み可能か。
		/// </summary>
		public readonly bool IsAuxiliariesWritable;

		internal CharacteristicExtendedPropertiesType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.CharacteristicExtendedPropertiesType x)
		{
			IsReliableWriteEnabled = x.IsReliableWriteEnabled != 0;
			IsAuxiliariesWritable = x.IsAuxiliariesWritable != 0;
		}
	}

	/// <summary>
	/// クライアント特性構成型。
	/// </summary>
	public readonly struct ClientCharacteristicConfigurationType
	{
		/// <summary>
		/// 通知を受け取るために特性が登録されているか。
		/// </summary>
		public readonly bool IsSubscribeToNotification;

		/// <summary>
		/// 表示を受け取るために特性が登録されているか。
		/// </summary>
		public readonly bool IsSubscribeToIndication;

		internal ClientCharacteristicConfigurationType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.ClientCharacteristicConfigurationType x)
		{
			IsSubscribeToNotification = x.IsSubscribeToNotification != 0;
			IsSubscribeToIndication = x.IsSubscribeToIndication != 0;
		}
	}

	/// <summary>
	/// サーバー特性構成型。
	/// </summary>
	public readonly struct ServerCharacteristicConfigurationType
	{
		/// <summary>
		/// ブロードキャスト可能か。
		/// </summary>
		public readonly bool IsBroadcast;

		internal ServerCharacteristicConfigurationType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.ServerCharacteristicConfigurationType x)
		{
			IsBroadcast = x.IsBroadcast != 0;
		}
	}

	/// <summary>
	/// 特性形式型。
	/// </summary>
	public readonly struct CharacteristicFormatType
	{
		/// <summary>
		/// 形式。
		/// </summary>
		public readonly byte Format;
		/// <summary>
		/// 指数値。
		/// </summary>
		public readonly byte Exponent;
		/// <summary>
		/// 定義済みの単位。
		/// </summary>
		public readonly BluetoothLEUuid Unit;
		/// <summary>
		/// 定義済みの名前空間。
		/// </summary>
		public readonly byte NameSpace;
		/// <summary>
		/// 形式のUUID。
		/// </summary>
		public readonly BluetoothLEUuid Description;

		internal CharacteristicFormatType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.CharacteristicFormatType x)
		{
			Format = x.Format;
			Exponent = x.Exponent;
			Unit = new(x.Unit);
			NameSpace = x.NameSpace;
			Description = new(x.Description);
		}
	}

	/// <summary>
	/// 特性記述子型。
	/// </summary>
	public BluetoothLEGattDescriptorType DescriptorType => _value1.DescriptorType;
	/// <summary>
	/// 特性記述子UUID。
	/// </summary>
	public BluetoothLEUuid DescriptorUuid => _value1.DescriptorUuid;

	/// <summary>
	/// 特性拡張プロパティ。
	/// </summary>
	public CharacteristicExtendedPropertiesType CharacteristicExtendedProperties
		=> DescriptorType == BluetoothLEGattDescriptorType.CharacteristicExtendedProperties
			? new(_value1.u.CharacteristicExtendedProperties) : throw new InvalidOperationException();

	/// <summary>
	/// クライアント特性構成。
	/// </summary>
	public ClientCharacteristicConfigurationType ClientCharacteristicConfiguration
		=> DescriptorType == BluetoothLEGattDescriptorType.ClientCharacteristicConfiguration
			? new(_value1.u.ClientCharacteristicConfiguration) : throw new InvalidOperationException();

	/// <summary>
	/// サーバー特性構成。
	/// </summary>
	public ServerCharacteristicConfigurationType ServerCharacteristicConfiguration
		=> DescriptorType == BluetoothLEGattDescriptorType.ServerCharacteristicConfiguration
			? new(_value1.u.ServerCharacteristicConfiguration) : throw new InvalidOperationException();

	/// <summary>
	/// 特性書式。
	/// </summary>
	public CharacteristicFormatType CharacteristicFormat
		=> DescriptorType == BluetoothLEGattDescriptorType.CharacteristicFormat
			? new(_value1.u.CharacteristicFormat) : throw new InvalidOperationException();

	/// <summary>
	/// 特性記述子値。
	/// </summary>
	public ReadOnlySpan<byte> Data => _raw.AsSpan(
		(int)Marshal.OffsetOf<BTH_LE_GATT_DESCRIPTOR_VALUE_1>(nameof(BTH_LE_GATT_DESCRIPTOR_VALUE_1.Data1)),
		(int)_value1.DataSize);
}

/// <summary>
/// GATTプロファイルの特性。
/// </summary>
/// <remarks>
/// 警告：このクラス及び関連処理は動作確認できていません。正常に動作しない可能性があります。
/// </remarks>
public sealed class BluetoothLEGattEvent
{
	/// <summary>
	/// イベントハンドル。
	/// </summary>
	public nint EventHandle { get; }

	/// <summary>
	/// コールバック関数。
	/// </summary>
	public BluetoothLEGattEventCallback Callback { get; }

	internal BluetoothLEGattEvent(nint eventHandle, BluetoothLEGattEventCallback callback)
	{
		EventHandle = eventHandle;
		Callback = callback;
	}

	/// <summary>
	/// イベント通知を解除します。
	/// </summary>
	public void Unregister()
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTUnregisterEvent(EventHandle, 0));
	}
}

file static class NativeMethods
{
	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetServices(
		SafeHandle hDevice,
		ushort ServicesBufferCount,
		[MarshalAs(UnmanagedType.LPArray), Out] BTH_LE_GATT_SERVICE[]? ServicesBuffer,
		out ushort ServicesBufferActual,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetCharacteristics(
		SafeHandle hDevice,
		in BTH_LE_GATT_SERVICE Service,
		ushort CharacteristicsBufferCount,
		[MarshalAs(UnmanagedType.LPArray), Out] BTH_LE_GATT_CHARACTERISTIC[]? CharacteristicsBuffer,
		out ushort CharacteristicsBufferActual,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetDescriptors(
		SafeHandle hDevice,
		in BTH_LE_GATT_CHARACTERISTIC Characteristic,
		ushort DescriptorsBufferCount,
		[MarshalAs(UnmanagedType.LPArray), Out] BTH_LE_GATT_DESCRIPTOR[]? DescriptorsBuffer,
		out ushort DescriptorsBufferActual,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetIncludedServices(
		SafeHandle hDevice,
		in BTH_LE_GATT_SERVICE ParentService,
		ushort IncludedServicesBufferCount,
		[MarshalAs(UnmanagedType.LPArray), Out] BTH_LE_GATT_SERVICE[]? IncludedServicesBuffer,
		out ushort IncludedServicesBufferActual,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTSetCharacteristicValue(
		SafeHandle hDevice,
		in BTH_LE_GATT_CHARACTERISTIC Characteristic,
		in byte CharacteristicValue,
		in ulong ReliableWriteContext,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetCharacteristicValue(
		SafeHandle hDevice,
		in BTH_LE_GATT_CHARACTERISTIC Characteristic,
		uint CharacteristicValueDataSize,
		ref byte CharacteristicValue,
		out ushort CharacteristicValueSizeRequired,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTGetDescriptorValue(
		SafeHandle hDevice,
		in BTH_LE_GATT_DESCRIPTOR Descriptor,
		uint DescriptorValueDataSize,
		ref byte DescriptorValue,
		out ushort DescriptorValueSizeRequired,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTSetDescriptorValue(
		SafeHandle hDevice,
		in BTH_LE_GATT_DESCRIPTOR Descriptor,
		in byte DescriptorValue,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTBeginReliableWrite(
	SafeHandle hDevice,
		out ulong ReliableWriteContext,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTEndReliableWrite(
		SafeHandle hDevice,
		in ulong ReliableWriteContext,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTAbortReliableWrite(
		SafeHandle hDevice,
		in ulong ReliableWriteContext,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTRegisterEvent(
		SafeHandle hService,
		BluetoothLEGattEventType EventType,
		in BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION_1 EventParameterIn,
		BluetoothLEGattEventCallback Callback,
		nint CallbackContext,
		out nint pEventHandle,
		uint Flags);

	[DllImport("BluetoothAPIs.dll")]
	public static extern int BluetoothGATTUnregisterEvent(
		nint EventHandle,
		uint Flags);
}

/// <summary>
/// BTH_LE_GATT_DESCRIPTOR_TYPE
/// </summary>
public enum BluetoothLEGattDescriptorType : uint
{
	/// <summary>
	/// 特性拡張プロパティ。
	/// </summary>
	CharacteristicExtendedProperties,
	/// <summary>
	/// 特性ユーザー定義情報。
	/// </summary>
	CharacteristicUserDescription,
	/// <summary>
	/// クライアント特性構成。
	/// </summary>
	ClientCharacteristicConfiguration,
	/// <summary>
	/// サーバー特性構成。
	/// </summary>
	ServerCharacteristicConfiguration,
	/// <summary>
	/// 特性書式。
	/// </summary>
	CharacteristicFormat,
	/// <summary>
	/// 特性集計書式。
	/// </summary>
	CharacteristicAggregateFormat,
	/// <summary>
	/// カスタム特性記述子。
	/// </summary>
	CustomDescriptor,
}

/// <summary>
/// BTH_LE_GATT_EVENT_TYPE
/// </summary>
/// <remarks>
/// <c>BTH_LE_GATT_EVENT_TYPE</c>
/// </remarks>
public enum BluetoothLEGattEventType
{
	/// <summary>
	/// 特性値の変更。
	/// </summary>
	CharacteristicValueChangedEvent,
}

/// <summary>
/// BluetoothLE GATTイベント通知コールバック。
/// </summary>
/// <param name="eventType">イベントの種類。</param>
/// <param name="eventOutParameter">イベント情報。</param>
/// <param name="context">コールバック登録時のパラメーター。</param>
/// <remarks>
/// NOTE：
/// eventOutParameterは本来構造体に変換して渡すべきです。
/// ただし、そもそもイベントを補足できないのでひとまず対応しません。
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public delegate void BluetoothLEGattEventCallback(
	BluetoothLEGattEventType eventType,
	nint eventOutParameter,
	nint context);

internal readonly struct BTH_LE_GATT_CHARACTERISTIC
{
	public readonly ushort ServiceHandle;
	public readonly BTH_LE_UUID CharacteristicUuid;
	public readonly ushort AttributeHandle;
	public readonly ushort CharacteristicValueHandle;
	public readonly byte IsBroadcastable;
	public readonly byte IsReadable;
	public readonly byte IsWritable;
	public readonly byte IsWritableWithoutResponse;
	public readonly byte IsSignedWritable;
	public readonly byte IsNotifiable;
	public readonly byte IsIndicatable;
	public readonly byte HasExtendedProperties;
}

internal struct BTH_LE_GATT_CHARACTERISTIC_VALUE_1
{
	public uint DataSize;
	public byte Data0;
}

/// <summary>
/// BTH_LE_GATT_SERVICE
/// </summary>
internal readonly struct BTH_LE_GATT_SERVICE
{
	public readonly BTH_LE_UUID ServiceUuid;
	public readonly ushort AttributeHandle;
}

internal readonly struct BTH_LE_GATT_DESCRIPTOR
{
	public readonly ushort ServiceHandle;
	public readonly ushort CharacteristicHandle;
	public readonly BluetoothLEGattDescriptorType DescriptorType;
	public readonly BTH_LE_UUID DescriptorUuid;
	public readonly ushort AttributeHandle;
}

internal struct BTH_LE_GATT_DESCRIPTOR_VALUE_1
{
	public struct CharacteristicExtendedPropertiesType
	{
		public byte IsReliableWriteEnabled;
		public byte IsAuxiliariesWritable;
	}

	public struct ClientCharacteristicConfigurationType
	{
		public byte IsSubscribeToNotification;
		public byte IsSubscribeToIndication;
	}

	public struct ServerCharacteristicConfigurationType
	{
		public byte IsBroadcast;
	}

	public struct CharacteristicFormatType
	{
		public byte Format;
		public byte Exponent;
		public BTH_LE_UUID Unit;
		public byte NameSpace;
		public BTH_LE_UUID Description;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct UnionType
	{
		[FieldOffset(0)]
		public CharacteristicExtendedPropertiesType CharacteristicExtendedProperties;
		[FieldOffset(0)]
		public ClientCharacteristicConfigurationType ClientCharacteristicConfiguration;
		[FieldOffset(0)]
		public ServerCharacteristicConfigurationType ServerCharacteristicConfiguration;
		[FieldOffset(0)]
		public CharacteristicFormatType CharacteristicFormat;
	}

	public BluetoothLEGattDescriptorType DescriptorType;
	public BluetoothLEUuid DescriptorUuid;
	public UnionType u;
	public uint DataSize;
	internal byte Data1;
}

// NOTE
// このライブラリでは要素数1のイベント登録しか扱わないため、それに合わせた定義を使用します。
// 複数使用する場合はNumCharacteristicsとCharacteristics0間のパディングに注意してください。
internal struct BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION_1
{
	public ushort NumCharacteristics;
	public BTH_LE_GATT_CHARACTERISTIC Characteristics0;
}
