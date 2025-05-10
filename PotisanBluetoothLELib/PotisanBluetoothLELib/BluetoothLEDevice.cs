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
	SafeHandle Handle { get; }
	Guid InterfaceGuid { get; }
}

/// <summary>
/// BluetoothLEデバイスとGATTサービスデバイスの共通実装。
/// </summary>
public abstract class BluetoothLEDeviceBase : IBluetoothLEDevice
{
	public abstract Guid InterfaceGuid { get; }

	internal FileStream _file;

	internal BluetoothLEDeviceBase(FileStream file)
	{
		_file = file;
	}

	public SafeHandle Handle => _file.SafeFileHandle;

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
	public override Guid InterfaceGuid { get; }

	internal BluetoothLEDevice(BluetoothLEDeviceInfo info)
		: base(File.Open(info.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
	{
		InterfaceGuid = info.InterfaceGuid;
	}

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
	SafeHandle Handle { get; }
	ImmutableArray<BluetoothLEGattCharacteristic> Characteristics { get; }
}

/// <summary>
/// BluetoothLE GATTサービスの機能。
/// </summary>
public static class IBluetoothLEGattServiceExtensions
{
	public static ulong BeginReliableWrite(this IBluetoothLEGattService service)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTBeginReliableWrite(service.Handle, out var ctx, 0));
		return ctx;
	}

	public static void EndReliableWrite(this IBluetoothLEGattService service, ulong context)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTEndReliableWrite(service.Handle, context, 0));
	}

	public static void AbortReliableWrite(this IBluetoothLEGattService service, ulong context)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.BluetoothGATTAbortReliableWrite(service.Handle, context, 0));
	}

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
	public override Guid InterfaceGuid { get; }

	internal BluetoothLEGattServiceDevice(BluetoothLEGattServiceDeviceInfo info)
		: base(File.Open(info.Path, FileMode.Open, FileAccess.Read, 0))
	{
		InterfaceGuid = info.InterfaceGuid;
	}

	public ImmutableArray<BluetoothLEGattCharacteristic> Characteristics
		=> BluetoothLEGattServiceForDevice.GetCharacteristics(Handle, Unsafe.NullRef<BTH_LE_GATT_SERVICE>(), InterfaceGuid);

	public ImmutableArray<BluetoothLEGattServiceForDevice> IncludedServices
		=> BluetoothLEGattServiceForDevice.GetIncludedServices(Handle, Unsafe.NullRef<BTH_LE_GATT_SERVICE>(), InterfaceGuid);
}

/// <summary>
/// BluetoothLEデバイスから取得したBluetoothLE GATTサービスデバイス情報。操作可能なハンドルを作成できます。
/// GATTプロファイルのサービスでもあります。
/// </summary>
public class BluetoothLEGattServiceForDevice : IBluetoothLEGattService
{
	public SafeHandle Handle { get; }
	private readonly BTH_LE_GATT_SERVICE _service;
	public Guid InterfaceGuid { get; }

	internal BluetoothLEGattServiceForDevice(SafeHandle handle, in BTH_LE_GATT_SERVICE service, in Guid interfaceGuid)
	{
		Handle = handle;
		_service = service;
		InterfaceGuid = interfaceGuid;
	}

	public BluetoothLEServiceUuid ServiceUuid => new(_service.ServiceUuid);
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

	public ImmutableArray<BluetoothLEGattServiceForDevice> IncludedServices => GetIncludedServices(Handle, _service, InterfaceGuid);

	public BluetoothLEGattServiceDeviceInfo? GetServiceDeviceInfo()
	{
		return BluetoothLE.CreateGattServiceDeviceInfoForInterface(ServiceUuid.ToGuid(), presentItems: true);
	}
}

/// <summary>
/// GATTプロファイルの特性。
/// </summary>
public sealed class BluetoothLEGattCharacteristic
{
	private readonly SafeHandle _handle;
	public Guid ServiceInterfaceId { get; }
	internal readonly BTH_LE_GATT_CHARACTERISTIC _characteristic;

	internal BluetoothLEGattCharacteristic(SafeHandle handle, in Guid serviceInterfaceId, in BTH_LE_GATT_CHARACTERISTIC characteristic)
	{
		_handle = handle;
		ServiceInterfaceId = serviceInterfaceId;
		_characteristic = characteristic;
	}

	public ushort ServiceHandle => _characteristic.ServiceHandle;
	public BluetoothLECharacteristicUuid CharacteristicUuid => new(_characteristic.CharacteristicUuid);
	public ushort AttributeHandle => _characteristic.AttributeHandle;
	public ushort CharacteristicValueHandle => _characteristic.CharacteristicValueHandle;
	public bool IsBroadcastable => _characteristic.IsBroadcastable != 0;
	public bool IsReadable => _characteristic.IsReadable != 0;
	public bool IsWritable => _characteristic.IsWritable != 0;
	public bool IsWritableWithoutResponse => _characteristic.IsWritableWithoutResponse != 0;
	public bool IsSignedWritable => _characteristic.IsWritable != 0;
	public bool IsNotifiable => _characteristic.IsNotifiable != 0;
	public bool IsIndicatable => _characteristic.IsIndicatable != 0;
	public bool HasExtendedProperties => _characteristic.HasExtendedProperties != 0;

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

	public byte[] Value => GetValue();
	public byte[]? ValueOrNull => TryGetValue();

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

[Flags]
public enum BluetoothLEGattFlag : uint
{
	None = 0x00000000,
	ConnectionEncrypted = 0x00000001,
	ConnectionAuthenticated = 0x00000002,
	ForceReadFromDevice = 0x00000004,
	ForceReadFromCache = 0x00000008,
	SignedWrite = 0x00000010,
	WriteWithoutResponse = 0x00000020,
	ReturnAll = 0x00000040,
}

/// <summary>
/// GATTプロファイルの記述子。
/// </summary>
public sealed class BluetoothLEGattDescriptor
{
	private readonly SafeHandle _handle;
	public BluetoothLEGattCharacteristic ParentCharacteristic { get; }
	private readonly BTH_LE_GATT_DESCRIPTOR _descriptor;

	internal BluetoothLEGattDescriptor(SafeHandle handle, BluetoothLEGattCharacteristic parentCharacteristic, in BTH_LE_GATT_DESCRIPTOR descriptor)
	{
		_handle = handle;
		ParentCharacteristic = parentCharacteristic;
		_descriptor = descriptor;
	}

	public ushort ServiceHandle => _descriptor.ServiceHandle;
	public ushort CharacteristicHandle => _descriptor.CharacteristicHandle;
	public BluetoothLEGattDescriptorType DescriptorType => _descriptor.DescriptorType;
	public BluetoothLEUuid DescriptorUuid => new(_descriptor.DescriptorUuid);
	public ushort AttributeHandle => _descriptor.AttributeHandle;

	// NOTE
	// 動作確認環境がないため、動作未確認です。
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

	// NOTE
	// 動作確認環境がないため、動作未確認です。
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

	// NOTE
	// 取得に成功したことがないので動作未確認です。
	public BluetoothLEGattDescriptorValue Value => GetValue();
	public BluetoothLEGattDescriptorValue? ValueOrNull => TryGetValue();

	// NOTE
	// 値の書き込み機能は未実装です。
	// 必要に応じて値の種類別の書き込みメソッドを作成します。
}

/// <summary>
/// GATTプロファイルの記述子値。
/// </summary>
public sealed class BluetoothLEGattDescriptorValue
{
	private readonly byte[] _raw;
	private BTH_LE_GATT_DESCRIPTOR_VALUE_1 _value1;

	internal BluetoothLEGattDescriptorValue(byte[] raw)
	{
		_raw = raw;
		_value1 = MemoryMarshal.Cast<byte, BTH_LE_GATT_DESCRIPTOR_VALUE_1>(raw)[0];
	}

	public readonly struct CharacteristicExtendedPropertiesType
	{
		public readonly bool IsReliableWriteEnabled;
		public readonly bool IsAuxiliariesWritable;

		internal CharacteristicExtendedPropertiesType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.CharacteristicExtendedPropertiesType x)
		{
			IsReliableWriteEnabled = x.IsReliableWriteEnabled != 0;
			IsAuxiliariesWritable = x.IsAuxiliariesWritable != 0;
		}
	}

	public readonly struct ClientCharacteristicConfigurationType
	{
		public readonly bool IsSubscribeToNotification;
		public readonly bool IsSubscribeToIndication;

		internal ClientCharacteristicConfigurationType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.ClientCharacteristicConfigurationType x)
		{
			IsSubscribeToNotification = x.IsSubscribeToNotification != 0;
			IsSubscribeToIndication = x.IsSubscribeToIndication != 0;
		}
	}

	public readonly struct ServerCharacteristicConfigurationType
	{
		public readonly bool IsBroadcast;

		internal ServerCharacteristicConfigurationType(BTH_LE_GATT_DESCRIPTOR_VALUE_1.ServerCharacteristicConfigurationType x)
		{
			IsBroadcast = x.IsBroadcast != 0;
		}
	}

	public readonly struct CharacteristicFormatType
	{
		public readonly byte Format;
		public readonly byte Exponent;
		public readonly BluetoothLEUuid Unit;
		public readonly byte NameSpace;
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

	public BluetoothLEGattDescriptorType DescriptorType => _value1.DescriptorType;
	public BluetoothLEUuid DescriptorUuid => _value1.DescriptorUuid;

	public CharacteristicExtendedPropertiesType CharacteristicExtendedProperties
		=> DescriptorType == BluetoothLEGattDescriptorType.CharacteristicExtendedProperties
			? new(_value1.u.CharacteristicExtendedProperties) : throw new InvalidOperationException();

	public ClientCharacteristicConfigurationType ClientCharacteristicConfiguration
		=> DescriptorType == BluetoothLEGattDescriptorType.ClientCharacteristicConfiguration
			? new(_value1.u.ClientCharacteristicConfiguration) : throw new InvalidOperationException();

	public ServerCharacteristicConfigurationType ServerCharacteristicConfiguration
		=> DescriptorType == BluetoothLEGattDescriptorType.ServerCharacteristicConfiguration
			? new(_value1.u.ServerCharacteristicConfiguration) : throw new InvalidOperationException();

	public CharacteristicFormatType CharacteristicFormat
		=> DescriptorType == BluetoothLEGattDescriptorType.CharacteristicFormat
			? new(_value1.u.CharacteristicFormat) : throw new InvalidOperationException();

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
	public nint EventHandle { get; }
	public BluetoothLEGattEventCallback Callback { get; }

	internal BluetoothLEGattEvent(nint eventHandle, BluetoothLEGattEventCallback callback)
	{
		EventHandle = eventHandle;
		Callback = callback;
	}

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
	CharacteristicExtendedProperties,
	CharacteristicUserDescription,
	ClientCharacteristicConfiguration,
	ServerCharacteristicConfiguration,
	CharacteristicFormat,
	CharacteristicAggregateFormat,
	CustomDescriptor
}

/// <summary>
/// BTH_LE_GATT_EVENT_TYPE
/// </summary>
/// <remarks>
/// <c>BTH_LE_GATT_EVENT_TYPE</c>
/// </remarks>
public enum BluetoothLEGattEventType
{
	CharacteristicValueChangedEvent,
}

// NOTE
// eventOutParameterは本来構造体に変換して渡すべきです。
// ただし、そもそもイベントを補足できないのでひとまず対応しません。
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
