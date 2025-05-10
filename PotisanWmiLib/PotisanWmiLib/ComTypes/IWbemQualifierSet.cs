namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("dc12a680-737f-11cf-884d-00aa004b2e24")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemQualifierSet
{
	[PreserveSig]
	int Get(
		string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] out object pVal,
		out int plFlavor);

	[PreserveSig]
	int Put(
		string wszName,
		[MarshalAs(UnmanagedType.Struct)] in object pVal,
		int lFlavor);

	[PreserveSig]
	int Delete(
		string wszName);

	[PreserveSig]
	int GetNames(
		int lFlags,
		[MarshalAs(UnmanagedType.SafeArray)] out object[] pNames);

	[PreserveSig]
	int BeginEnumeration(
		int lFlags);

	[PreserveSig]
	int Next(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string pstrName,
		[MarshalAs(UnmanagedType.Struct)] out object pVal,
		out int plFlavor);

	[PreserveSig]
	int EndEnumeration();
}
