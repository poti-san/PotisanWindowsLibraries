namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("bfbf883a-cad7-11d3-a11b-00105a1f515a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemObjectTextSrc
{
	[PreserveSig]
	int GetText(
		int lFlags,
		IWbemClassObject pObj,
		uint uObjTextFormat,
		IWbemContext? pCtx,
		[MarshalAs(UnmanagedType.BStr)] out string strText);

	[PreserveSig]
	int CreateFromText(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] string strText,
		uint uObjTextFormat,
		IWbemContext? pCtx,
		out IWbemClassObject pNewObj);
}
