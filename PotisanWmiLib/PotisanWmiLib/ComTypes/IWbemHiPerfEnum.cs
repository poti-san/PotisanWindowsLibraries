namespace Potisan.Windows.Wmi.ComTypes;

// TODO
[ComImport]
[Guid("2705C288-79AE-11d2-B348-00105A1F8177")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemHiPerfEnum
{
	[PreserveSig]
	int AddObjects(
		int lFlags,
		uint uNumObjects,
		[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] apIds,
		[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IWbemObjectAccess[] apObj);

	[PreserveSig]
	int RemoveObjects(
		int lFlags,
		uint uNumObjects,
		[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] apIds);

	[PreserveSig]
	int GetObjects(
		int lFlags,
		uint uNumObjects,
		out IWbemObjectAccess? apObj,
		out uint puReturned);

	[PreserveSig]
	int RemoveAll(
		int lFlags);
}