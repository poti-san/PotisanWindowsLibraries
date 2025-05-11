namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("44aca674-e8fc-11d0-a07c-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemContext
{
	[PreserveSig]
	int Clone(
		out IWbemContext ppNewCopy);

	// TODO CHECK
	[PreserveSig]
	int GetNames(
		int lFlags,
		[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] pNames);

	[PreserveSig]
	int BeginEnumeration(
		int lFlags);

	[PreserveSig]
	int Next(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string pstrName,
		[MarshalAs(UnmanagedType.Struct)] out object? pValue);

	[PreserveSig]
	int EndEnumeration();

	[PreserveSig]
	int SetValue(
		string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] in object? pValue);

	// TODO CHECK
	[PreserveSig]
	int GetValue(
		string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] out object? pValue);

	[PreserveSig]
	int DeleteValue(
		string wszName,
		int lFlags);

	[PreserveSig]
	int DeleteAll();
}
