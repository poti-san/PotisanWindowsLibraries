namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("eb87e1bc-3233-11d2-aec9-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemStatusCodeText
{
	[PreserveSig]
	int GetErrorCodeText(
		int hRes,
		uint LocaleId,
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string MessageText);

	[PreserveSig]
	int GetFacilityCodeText(
		int hRes,
		uint LocaleId,
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string MessageText);
}