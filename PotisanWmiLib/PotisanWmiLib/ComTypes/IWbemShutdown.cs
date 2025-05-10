namespace Potisan.Windows.Wmi.ComTypes;

// TODO
[ComImport]
[Guid("b7b31df9-d515-11d3-a11c-00105a1f515a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemShutdown
{
	[PreserveSig]
	int Shutdown(
		int uReason,
		uint uMaxMilliseconds,
		IWbemContext? pCtx);
}
