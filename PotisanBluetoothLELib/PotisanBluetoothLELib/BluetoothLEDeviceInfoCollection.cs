using System.Collections;
using System.Collections.Immutable;

using Potisan.Windows.BluetoothLE.KnownShortUuids;

namespace Potisan.Windows.BluetoothLE;

/// <summary>
/// BluetoothLEデバイスとGATTサービスデバイス情報コレクションの共通インターフェイス。
/// </summary>
public interface IBluetoothLEDeviceCollection : IDisposable
{
	/// <summary>
	/// BluetoothLE GATTデバイスまたはサービスデバイスコレクションハンドル。
	/// </summary>
	/// <remarks>
	/// デバイス情報リストハンドルです。
	/// </remarks>
	SafeHandle Handle { get; }

	/// <summary>
	/// 全てのデバイス情報を取得します。
	/// </summary>
	ImmutableArray<IBluetoothLEDeviceInfo> Items { get; }

	/// <summary>
	/// デバイス情報の列挙可能オブジェクトを取得します。
	/// </summary>
	IEnumerable<IBluetoothLEDeviceInfo> GetDeviceInfoEnumerable();
}

/// <summary>
/// BluetoothLEデバイス情報とGATTサービスデバイス情報コレクションの共通実装。
/// </summary>
public abstract class BluetoothLEDeviceInfoCollectionBase<T> : IDisposable, IReadOnlyList<T>
	where T : IBluetoothLEDeviceInfo
{
	/// <summary>
	/// コレクション作成時のデバイスインターフェイスGUID。
	/// </summary>
	public Guid InterfaceGuid { get; }

	/// <summary>
	/// デバイス情報リストハンドル。
	/// </summary>
	public SafeHandle Handle { get; }

	/// <summary>
	/// 全てのデバイス情報を取得します。
	/// </summary>
	public ImmutableArray<T> Items { get; }

	internal BluetoothLEDeviceInfoCollectionBase(in Guid classGuid, in Guid interfaceGuid, bool defaultItem, bool presentItems, bool profileItems, bool interfaceItems)
	{
		const uint DIGCF_DEFAULT = 0x00000001;
		const uint DIGCF_PRESENT = 0x00000002;
		const uint DIGCF_PROFILE = 0x00000008;
		const uint DIGCF_DEVICEINTERFACE = 0x00000010;

		var handle = NativeMethods.SetupDiGetClassDevsW(classGuid, null, 0, (interfaceItems ? DIGCF_DEVICEINTERFACE : 0)
			| (defaultItem ? DIGCF_DEFAULT : 0) | (presentItems ? DIGCF_PRESENT : 0) | (profileItems ? DIGCF_PROFILE : 0));
		if (handle.IsInvalid)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

		const int ERROR_NO_MORE_ITEMS = 259;

		var infos = new List<T>();
		var devInfo = new SP_DEVINFO_DATA((uint)Marshal.SizeOf<SP_DEVINFO_DATA>());
		var interfaceInfo = new SP_DEVICE_INTERFACE_DATA((uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>());
		for (uint deviceIndex = 0; ; deviceIndex++)
		{
			if (!NativeMethods.SetupDiEnumDeviceInfo(handle, deviceIndex, ref devInfo))
			{
				if (Marshal.GetLastPInvokeError() == ERROR_NO_MORE_ITEMS)
					break;
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			for (uint interfaceIndex = 0; ; interfaceIndex++)
			{
				if (!NativeMethods.SetupDiEnumDeviceInterfaces(handle, devInfo, interfaceGuid, interfaceIndex, ref interfaceInfo))
				{
					if (Marshal.GetLastPInvokeError() == ERROR_NO_MORE_ITEMS)
						break;
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				infos.Add(CreateItem(this, devInfo, interfaceGuid,
					(DeviceInterfaceFlag)interfaceInfo.Flags, InternalHelper.GetInterfacePath(handle, interfaceInfo)));
			}
		}

		InterfaceGuid = interfaceGuid;
		Handle = handle;
		Items = [.. infos];
	}

	internal abstract T CreateItem(
		BluetoothLEDeviceInfoCollectionBase<T> owner,
		SP_DEVINFO_DATA devinfo,
		in Guid interfaceGuid,
		DeviceInterfaceFlag interfaceFlags,
		string path);

	/// <inheritdoc/>
	public void Dispose()
	{
		Handle.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc/>
	public int Count => ((IReadOnlyCollection<T>)Items).Count;

	/// <inheritdoc/>
	public T this[int index] => ((IReadOnlyList<T>)Items)[index];

	/// <inheritdoc/>
	public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}

/// <summary>
/// BluetoothLEデバイス情報コレクション。
/// </summary>
public class BluetoothLEDeviceInfoCollection :
	BluetoothLEDeviceInfoCollectionBase<BluetoothLEDeviceInfo>,
	IBluetoothLEDeviceCollection
{
	internal BluetoothLEDeviceInfoCollection(in Guid classGuid, in Guid interfaceGuid, bool defaultItem, bool presentItems, bool profileItems, bool interfaceItems)
		: base(classGuid, interfaceGuid, defaultItem, presentItems, profileItems, interfaceItems)
	{
	}

	internal override BluetoothLEDeviceInfo CreateItem(
		BluetoothLEDeviceInfoCollectionBase<BluetoothLEDeviceInfo> owner,
		SP_DEVINFO_DATA devinfo,
		in Guid interfaceGuid,
		DeviceInterfaceFlag interfaceFlags,
		string path)
	{
		return new(owner, devinfo, interfaceGuid, interfaceFlags, path);
	}

	ImmutableArray<IBluetoothLEDeviceInfo> IBluetoothLEDeviceCollection.Items => [.. Items.OfType<IBluetoothLEDeviceInfo>()];

	/// <inheritdoc/>
	public IEnumerable<IBluetoothLEDeviceInfo> GetDeviceInfoEnumerable() => Items.OfType<IBluetoothLEDeviceInfo>();

	/// <summary>
	/// BluetoothLE GATTサービスUUIDに対応するデバイス情報を取得します。
	/// </summary>
	/// <param name="uuid">BluetoothLE GATTサービスUUID。</param>
	public ImmutableArray<BluetoothLEDeviceInfo> GetDeviceInfosByServiceUuid(KnownServiceShortUuid uuid)
	{
		var list = new List<BluetoothLEDeviceInfo>();
		foreach (var deviceInfo in Items)
		{
			using var device = deviceInfo.Open();
			if (device.Services.Any(service => service.ServiceUuid.ShortUuid is { } shortUuid && shortUuid == (ushort)uuid))
				list.Add(deviceInfo);
		}
		return [.. list];
	}
}

/// <summary>
/// BluetoothLE GATTサービスデバイス情報コレクション。
/// </summary>
public class BluetoothLEGattServiceDeviceInfoCollection :
	BluetoothLEDeviceInfoCollectionBase<BluetoothLEGattServiceDeviceInfo>,
	IBluetoothLEDeviceCollection
{
	internal BluetoothLEGattServiceDeviceInfoCollection(in Guid classGuid, in Guid interfaceGuid, bool defaultItem, bool presentItems, bool profileItems, bool interfaceItems)
		: base(classGuid, interfaceGuid, defaultItem, presentItems, profileItems, interfaceItems)
	{
	}

	internal override BluetoothLEGattServiceDeviceInfo CreateItem(
		BluetoothLEDeviceInfoCollectionBase<BluetoothLEGattServiceDeviceInfo> owner,
		SP_DEVINFO_DATA devinfo,
		in Guid interfaceGuid,
		DeviceInterfaceFlag interfaceFlags,
		string path)
	{
		return new(owner, devinfo, interfaceGuid, interfaceFlags, path);
	}

	ImmutableArray<IBluetoothLEDeviceInfo> IBluetoothLEDeviceCollection.Items => [.. Items.OfType<IBluetoothLEDeviceInfo>()];

	/// <inheritdoc/>
	public IEnumerable<IBluetoothLEDeviceInfo> GetDeviceInfoEnumerable() => Items.OfType<IBluetoothLEDeviceInfo>();
}

file static class NativeMethods
{
	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public static extern SafeDeviceInfoHandle SetupDiGetClassDevsW(in Guid ClassGuid, string? Enumerator, nint hwndParent, uint Flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInfo(SafeDeviceInfoHandle DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInterfaces(SafeDeviceInfoHandle DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData,
		in Guid InterfaceClassGuid, uint MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);
}

/// <summary>
/// デバイスインターフェイスの状態。
/// </summary>
[Flags]
public enum DeviceInterfaceFlag : uint
{
	/// <summary>
	/// インターフェイスはアクティブです。
	/// </summary>
	Active = 0x00000001,
	/// <summary>
	/// 既定のインターフェイスです。
	/// </summary>
	Default = 0x00000002,
	/// <summary>
	/// 削除済みのインターフェイスです。
	/// </summary>
	Removed = 0x00000004,
}

internal sealed class SafeDeviceInfoHandle : SafeHandle
{
	public SafeDeviceInfoHandle() : base(0, true) { }

	public override bool IsInvalid => handle is 0 or -1;

	protected override bool ReleaseHandle()
	{
		[DllImport("setupapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetupDiDestroyDeviceInfoList(nint DeviceInfoSet);

		return SetupDiDestroyDeviceInfoList(handle);
	}
}

internal readonly struct SP_DEVINFO_DATA(uint size)
{
	public readonly uint cbSize = size;
	public readonly Guid ClassGuid;
	public readonly uint DevInst;
	public readonly nint Reserved;
}

internal readonly struct SP_DEVICE_INTERFACE_DATA(uint size)
{
	public readonly uint cbSize = size;
	public readonly Guid InterfaceClassGuid;
	public readonly uint Flags;
	public readonly nint Reserved;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct SP_DEVICE_INTERFACE_DETAIL_DATA_W_1
{
	public uint cbSize;
	public char DevicePath0;
}

internal readonly struct DEVPROPKEY(in Guid fmtid, uint pid)
{
	public readonly Guid FmtID = fmtid;
	public readonly uint PID = pid;
}
