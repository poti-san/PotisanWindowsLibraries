using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Potisan.Windows.BluetoothLE;

/// <summary>
/// BluetoothLEデバイス情報とGATTサービス情報の共通インターフェイス。
/// </summary>
public interface IBluetoothLEDeviceInfo
{
	Guid InterfaceGuid { get; }
	Guid ClassGuid { get; }
	DeviceInterfaceFlag InterfaceFlags { get; }
	string Path { get; }

	string DeviceClassName { get; }
	string Name { get; }
	string FriendlyName { get; }

	IBluetoothLEDevice Open();
	IBluetoothLEDevice? TryOpen();
}

/// <summary>
/// BluetoothLEデバイス情報とGATTサービス情報の共通実装。
/// </summary>
public abstract class BluetoothLEDeviceInfoBase<T>
	where T : IBluetoothLEDeviceInfo
{
	public BluetoothLEDeviceInfoCollectionBase<T> Owner { get; }
	private readonly SP_DEVINFO_DATA _devinfo;
	public Guid InterfaceGuid { get; }
	public Guid ClassGuid => _devinfo.ClassGuid;
	public DeviceInterfaceFlag InterfaceFlags { get; }
	public string Path { get; }

	internal BluetoothLEDeviceInfoBase(BluetoothLEDeviceInfoCollectionBase<T> owner,
		in SP_DEVINFO_DATA devinfo, in Guid interfaceGuid, DeviceInterfaceFlag interfaceFlags, string path)
	{
		Owner = owner;
		_devinfo = devinfo;
		InterfaceGuid = interfaceGuid;
		InterfaceFlags = interfaceFlags;
		Path = path;
	}

	private static (byte[] Data, uint DataType) GetDeviceProperty(SafeHandle handle, in SP_DEVINFO_DATA devinfo, in DEVPROPKEY propKey)
	{
		NativeMethods.SetupDiGetDevicePropertyW(handle, devinfo, propKey, out _, ref Unsafe.NullRef<byte>(), 0, out var required, 0);
		var buffer = GC.AllocateUninitializedArray<byte>(checked((int)required));
		if (!NativeMethods.SetupDiGetDevicePropertyW(handle, devinfo, propKey, out var propType, ref MemoryMarshal.GetArrayDataReference(buffer), required, out _, 0))
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		return (buffer, propType);
	}

	private static string GetDevicePropertyString(SafeHandle handle, in SP_DEVINFO_DATA devinfo, in DEVPROPKEY propKey)
	{
		const uint DEVPROP_TYPE_STRING = 0x00000012;

		var (Data, DataType) = GetDeviceProperty(handle, devinfo, propKey);
		if (DataType != DEVPROP_TYPE_STRING)
			throw new InvalidDataException();

		return Encoding.Unicode.GetString(Data.AsSpan()[..^2]);
	}

	public string DeviceClassName
	{
		get
		{
			NativeMethods.SetupDiClassNameFromGuidW(ClassGuid, ref Unsafe.NullRef<char>(), 0, out var required);
			var buffer = (stackalloc char[checked((int)required)]);
			if (!NativeMethods.SetupDiClassNameFromGuidW(ClassGuid, ref MemoryMarshal.GetReference(buffer), required, out _))
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return new(buffer[..^1]);
		}
	}

	public string Name
	{
		get
		{
			DEVPROPKEY DEVPKEY_NAME = new(new(0xb725f130, 0x47ef, 0x101a, 0xa5, 0xf1, 0x02, 0x60, 0x8c, 0x9e, 0xeb, 0xac), 10);
			return GetDevicePropertyString(Owner.Handle, _devinfo, DEVPKEY_NAME);
		}
	}

	public string FriendlyName
	{
		get
		{
			DEVPROPKEY DEVPKEY_Device_FriendlyName = new(new(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);
			return GetDevicePropertyString(Owner.Handle, _devinfo, DEVPKEY_Device_FriendlyName);
		}
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Name} ({Path})";
}

/// <summary>
/// BluetoothLEデバイス情報。
/// </summary>
[DebuggerDisplay("{Name} ({Path})")]
public class BluetoothLEDeviceInfo : BluetoothLEDeviceInfoBase<BluetoothLEDeviceInfo>, IBluetoothLEDeviceInfo
{
	internal BluetoothLEDeviceInfo(BluetoothLEDeviceInfoCollectionBase<BluetoothLEDeviceInfo> owner,
		in SP_DEVINFO_DATA devinfo, in Guid interfaceGuid, DeviceInterfaceFlag interfaceFlags, string path)
		: base(owner, devinfo, interfaceGuid, interfaceFlags, path)
	{
	}

	public BluetoothLEDevice Open() => new(this);
	public BluetoothLEDevice? TryOpen()
	{
		try
		{
			return Open();
		}
		catch
		{
			return null;
		}
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Name} ({Path})";

	IBluetoothLEDevice IBluetoothLEDeviceInfo.Open() => Open();
	IBluetoothLEDevice? IBluetoothLEDeviceInfo.TryOpen() => TryOpen();
}

/// <summary>
/// BluetoothLE GATTサービスデバイス情報。
/// </summary>
[DebuggerDisplay("{Name} ({Path})")]
public class BluetoothLEGattServiceDeviceInfo : BluetoothLEDeviceInfoBase<BluetoothLEGattServiceDeviceInfo>, IBluetoothLEDeviceInfo
{
	internal BluetoothLEGattServiceDeviceInfo(BluetoothLEDeviceInfoCollectionBase<BluetoothLEGattServiceDeviceInfo> owner,
		in SP_DEVINFO_DATA devinfo, in Guid interfaceGuid, DeviceInterfaceFlag interfaceFlags, string path)
		: base(owner, devinfo, interfaceGuid, interfaceFlags, path)
	{
	}

	public BluetoothLEGattServiceDevice Open() => new(this);
	public BluetoothLEGattServiceDevice? TryOpen()
	{
		try
		{
			return Open();
		}
		catch
		{
			return null;
		}
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Name} ({Path})";

	IBluetoothLEDevice IBluetoothLEDeviceInfo.Open() => Open();
	IBluetoothLEDevice? IBluetoothLEDeviceInfo.TryOpen() => TryOpen();
}

file static class NativeMethods
{
	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiGetDevicePropertyW(
		SafeHandle DeviceInfoSet,
		in SP_DEVINFO_DATA DeviceInfoData,
		in DEVPROPKEY PropertyKey,
		out uint PropertyType,
		ref byte PropertyBuffer,
		uint PropertyBufferSize,
		out uint RequiredSize,
		uint Flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiClassNameFromGuidW(in Guid ClassGuid, ref char ClassName, uint ClassNameSize, out uint RequiredSize);
}