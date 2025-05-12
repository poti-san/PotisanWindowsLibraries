using System.Runtime.CompilerServices;

namespace Potisan.Windows.BluetoothLE;

/// <summary>
/// BluetoothLEライブラリのメインクラス。
/// 各オブジェクトはこのクラスを利用して作成します。
/// </summary>
public static class BluetoothLE
{
	/// <summary>
	/// BluetoothLEデバイスのデバイスインターフェイスGUID。
	/// </summary>
	public static Guid BluetoothLEDeviceInterfaceGuid => new(0x781aee18, 0x7733, 0x4ce4, 0xad, 0xd0, 0x91, 0xf4, 0x1c, 0x67, 0xb5, 0x92);

	/// <summary>
	/// BluetoothLE GATTサービスデバイスのデバイスインターフェイスGUID。
	/// </summary>
	public static Guid BluetoothLEGattServiceDeviceInterfaceGuid => new(0x6e3bb679, 0x4372, 0x40c8, 0x9e, 0xaa, 0x45, 0x09, 0xdf, 0x26, 0x0c, 0xd8);

	/// <summary>
	/// BluetoothLEデバイスのデバイス情報コレクションを作成します。
	/// 通常は全ての引数を省略して呼び出します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEDeviceInterfaceGuid"/>。通常は指定しません。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEDeviceInfoCollection CreateDeviceInfoCollection(
		Guid? interfaceGuid = null,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		interfaceGuid ??= BluetoothLEDeviceInterfaceGuid;
		return new(interfaceGuid.Value, interfaceGuid.Value, defaultItem, presentItems, profileItems, true);
	}

	/// <summary>
	/// BluetoothLE GATTサービスデバイスのデバイス情報コレクションを作成します。
	/// 通常は全ての引数を省略して呼び出します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEGattServiceDeviceInterfaceGuid"/>。通常は指定しません。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEGattServiceDeviceInfoCollection CreateGattServiceDeviceInfoCollection(
		Guid? interfaceGuid = null,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		interfaceGuid ??= BluetoothLEGattServiceDeviceInterfaceGuid;
		return new(interfaceGuid.Value, interfaceGuid.Value, defaultItem, presentItems, profileItems, true);
	}

	/// <summary>
	/// デバイスクラスGUIDを指定してBluetoothLEデバイスのデバイス情報コレクションを作成します。
	/// 通常は<see cref="CreateDeviceInfoCollection(Guid?, bool, bool, bool)"/>を使用します。
	/// </summary>
	/// <param name="classGuid">デバイスクラスGUID。</param>
	/// <param name="interfaceGuid">インターフェイスGUID。省略時は<see cref="BluetoothLEDeviceInterfaceGuid"/>です。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEDeviceInfoCollection CreateDeviceInfoCollectionForClass(
		in Guid classGuid,
		Guid? interfaceGuid = null,
		bool presentItems = false,
		bool profileItems = false)
	{
		return new(classGuid, interfaceGuid ?? BluetoothLEDeviceInterfaceGuid, false, presentItems, profileItems, false);
	}

	/// <summary>
	/// デバイスクラスGUIDを指定してBluetoothLE GATTサービスデバイスのデバイス情報コレクションを作成します。
	/// 通常は<see cref="CreateGattServiceDeviceInfoCollection(Guid?, bool, bool, bool)"/>を使用します。
	/// </summary>
	/// <param name="classGuid">デバイスクラスGUID。</param>
	/// <param name="interfaceGuid">インターフェイスGUID。省略時は<see cref="BluetoothLEGattServiceDeviceInterfaceGuid"/>です。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEGattServiceDeviceInfoCollection CreateGattServiceDeviceInfoCollectionForClass(
		in Guid classGuid,
		Guid? interfaceGuid = null,
		bool presentItems = false,
		bool profileItems = false)
	{
		return new(classGuid, interfaceGuid ?? BluetoothLEGattServiceDeviceInterfaceGuid, false, presentItems, profileItems, false);
	}

	/// <summary>
	/// デバイスクラスGUIDを指定してBluetoothLEデバイスのデバイス情報を作成します。
	/// </summary>
	/// <param name="classGuid">デバイスクラスGUID。</param>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEDeviceInterfaceGuid"/>です。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報。</returns>
	public static BluetoothLEDeviceInfo? CreateDeviceInfoForClass(
		in Guid classGuid,
		Guid? interfaceGuid = null,
		bool presentItems = false,
		bool profileItems = false)
	{
		return CreateDeviceInfoCollectionForClass(classGuid, interfaceGuid ?? BluetoothLEDeviceInterfaceGuid, presentItems, profileItems).SingleOrDefault();
	}

	/// <summary>
	/// デバイスクラスGUIDを指定してBluetoothLE GATTサービスデバイスのデバイス情報を作成します。
	/// </summary>
	/// <param name="classGuid">デバイスクラスGUID。</param>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEGattServiceDeviceInterfaceGuid"/>です。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報。</returns>
	public static BluetoothLEGattServiceDeviceInfo? CreateGattServiceDeviceInfoForClass(
		in Guid classGuid,
		Guid? interfaceGuid = null,
		bool presentItems = false,
		bool profileItems = false)
	{
		return CreateGattServiceDeviceInfoCollectionForClass(classGuid, interfaceGuid ?? BluetoothLEGattServiceDeviceInterfaceGuid, presentItems, profileItems).SingleOrDefault();
	}

	/// <summary>
	/// デバイスインターフェイスGUIDを指定してBluetoothLEデバイスのデバイス情報コレクションを作成します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEDeviceInterfaceGuid"/>です。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEDeviceInfoCollection CreateDeviceInfoCollectionForInterface(
		in Guid interfaceGuid,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		return new(interfaceGuid, interfaceGuid, defaultItem, presentItems, profileItems, true);
	}

	/// <summary>
	/// デバイスインターフェイスGUIDを指定してBluetoothLE GATTサービスデバイスのデバイス情報コレクションを作成します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEGattServiceDeviceInterfaceGuid"/>です。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報コレクション。</returns>
	public static BluetoothLEGattServiceDeviceInfoCollection CreateGattServiceDeviceInfoCollectionForInterface(
		in Guid interfaceGuid,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		return new(interfaceGuid, interfaceGuid, defaultItem, presentItems, profileItems, true);
	}

	/// <summary>
	/// デバイスインターフェイスGUIDを指定してBluetoothLEデバイスのデバイス情報を作成します。
	/// デバイスが1つ以外の場合は失敗します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEDeviceInterfaceGuid"/>です。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報。</returns>
	public static BluetoothLEDeviceInfo? CreateDeviceInfoForInterface(
		in Guid interfaceGuid,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		return CreateDeviceInfoCollectionForInterface(interfaceGuid, defaultItem, presentItems, profileItems).SingleOrDefault();
	}

	/// <summary>
	/// デバイスインターフェイスGUIDを指定してBluetoothLE GATTサービスデバイスのデバイス情報を作成します。
	/// デバイスが1つ以外の場合は失敗します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。省略時は<see cref="BluetoothLEGattServiceDeviceInterfaceGuid"/>です。</param>
	/// <param name="defaultItem">既定のデバイスのみ含める。</param>
	/// <param name="presentItems">存在するデバイスのみ含める。</param>
	/// <param name="profileItems">プロファイルの登録されたデバイスのみ含める。</param>
	/// <returns>新しいデバイス情報。</returns>
	public static BluetoothLEGattServiceDeviceInfo? CreateGattServiceDeviceInfoForInterface(
		in Guid interfaceGuid,
		bool defaultItem = false,
		bool presentItems = false,
		bool profileItems = false)
	{
		return CreateGattServiceDeviceInfoCollectionForInterface(interfaceGuid, defaultItem, presentItems, profileItems).SingleOrDefault();
	}

	/// <summary>
	/// デバイスインターフェイスGUIDに対応するデバイスパスを返します。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。</param>
	/// <returns>デバイスパス。ファイルシステム上には存在しませんが、ファイルハンドルを開ける場合があります。</returns>
	public static string GetInterfacePath(in Guid interfaceGuid)
	{
		const uint DIGCF_PRESENT = 0x00000002;
		const uint DIGCF_DEVICEINTERFACE = 0x00000010;

		using var handle = NativeMethods.SetupDiGetClassDevsW(interfaceGuid, null, 0, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
		if (handle.IsInvalid)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

		var interfaceData = new SP_DEVICE_INTERFACE_DATA((uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>());
		if (!NativeMethods.SetupDiEnumDeviceInterfaces(handle, in Unsafe.NullRef<SP_DEVINFO_DATA>(), interfaceGuid, 0, ref interfaceData))
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

		return InternalHelper.GetInterfacePath(handle, interfaceData);
	}

	/// <summary>
	/// デバイスインターフェイスGUIDに対応するデバイスハンドルを開きます。
	/// </summary>
	/// <param name="interfaceGuid">デバイスインターフェイスGUID。</param>
	/// <param name="access">アクセス権限。</param>
	/// <param name="share">共有方法。</param>
	/// <returns>デバイスハンドル。実体はファイルハンドルです。</returns>
	public static SafeHandle OpenInterfaceHandle(in Guid interfaceGuid, FileAccess access = FileAccess.Read, FileShare share = FileShare.ReadWrite)
	{
		return File.Open(GetInterfacePath(interfaceGuid), FileMode.Open, access, share).SafeFileHandle;
	}
}

file static class NativeMethods
{
	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public static extern SafeDeviceInfoHandle SetupDiGetClassDevsW(in Guid ClassGuid, string? Enumerator, nint hwndParent, uint Flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInterfaces(SafeDeviceInfoHandle DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData,
		in Guid InterfaceClassGuid, uint MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiGetDeviceInterfaceDetailW(
		SafeDeviceInfoHandle DeviceInfoSet,
		in SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
		ref byte DeviceInterfaceDetailData,
		uint DeviceInterfaceDetailDataSize,
		out uint RequiredSize,
		nint DeviceInfoData);
}
