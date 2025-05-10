namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("7c857801-7381-11cf-884d-00aa004b2e24")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemObjectSink
{
	[PreserveSig]
	int Indicate(
		int lObjectCount,
		[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IWbemClassObject[]? apObjArray);

	[PreserveSig]
	int SetStatus(
		int lFlags,
		int hResult,
		[MarshalAs(UnmanagedType.BStr)] string? strParam,
		IWbemClassObject? pObjParam);
}
