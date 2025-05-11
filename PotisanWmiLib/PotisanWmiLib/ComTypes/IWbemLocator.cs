namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("dc12a687-737f-11cf-884d-00aa004b2e24")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemLocator
{
	[PreserveSig]
	int ConnectServer(
		[MarshalAs(UnmanagedType.BStr)] string strNetworkResource,
		[MarshalAs(UnmanagedType.BStr)] string? strUser,
		[MarshalAs(UnmanagedType.BStr)] string? strPassword,
		[MarshalAs(UnmanagedType.BStr)] string? strLocale,
		int lSecurityFlags,
		[MarshalAs(UnmanagedType.BStr)] string? strAuthority,
		IWbemContext? pCtx,
		out IWbemServices? ppNamespace);
}
