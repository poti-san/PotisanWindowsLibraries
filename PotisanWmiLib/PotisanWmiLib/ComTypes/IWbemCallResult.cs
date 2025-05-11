namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("44aca675-e8fc-11d0-a07c-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemCallResult
{
	[PreserveSig]
	int GetResultObject(
		uint lTimeout,
		out IWbemClassObject? ppResultObject);

	[PreserveSig]
	int GetResultString(
		uint lTimeout,
		[MarshalAs(UnmanagedType.BStr)] out string pstrResultString);

	[PreserveSig]
	int GetResultServices(
		uint lTimeout,
		out IWbemServices ppServices);

	[PreserveSig]
	int GetCallStatus(
		uint lTimeout,
		out int plStatus);
}
