namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("9556dc99-828c-11cf-a37e-00aa003240c7")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemServices
{
	[PreserveSig]
	int OpenNamespace(
		[MarshalAs(UnmanagedType.BStr)] string strNamespace,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemServices? ppWorkingNamespace,
		out IWbemCallResult? ppResult);

	[PreserveSig]
	int CancelAsyncCall(
		IWbemObjectSink? pSink);

	[PreserveSig]
	int QueryObjectSink(
		int lFlags,
		out IWbemObjectSink? ppResponseHandler);

	[PreserveSig]
	int GetObject(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemClassObject? ppObject,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int GetObjectAsync(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int PutClass(
		IWbemClassObject? pObject,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int PutClassAsync(
		IWbemClassObject? pObject,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int DeleteClass(
		[MarshalAs(UnmanagedType.BStr)] string strClass,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int DeleteClassAsync(
		[MarshalAs(UnmanagedType.BStr)] string strClass,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int CreateClassEnum(
		[MarshalAs(UnmanagedType.BStr)] string? strSuperclass,
		int lFlags,
		IWbemContext? pCtx,
		out IEnumWbemClassObject ppEnum);

	[PreserveSig]
	int CreateClassEnumAsync(
		[MarshalAs(UnmanagedType.BStr)] string? strSuperclass,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int PutInstance(
		IWbemClassObject? pInst,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int PutInstanceAsync(
		IWbemClassObject? pInst,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int DeleteInstance(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		int lFlags,
		IWbemContext? pCtx,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int DeleteInstanceAsync(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int CreateInstanceEnum(
		[MarshalAs(UnmanagedType.BStr)] string strFilter,
		int lFlags,
		IWbemContext? pCtx,
		out IEnumWbemClassObject ppEnum);

	[PreserveSig]
	int CreateInstanceEnumAsync(
		[MarshalAs(UnmanagedType.BStr)] string strFilter,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int ExecQuery(
		[MarshalAs(UnmanagedType.BStr)] string strQueryLanguage,
		[MarshalAs(UnmanagedType.BStr)] string strQuery,
		int lFlags,
		IWbemContext? pCtx,
		out IEnumWbemClassObject? ppEnum);

	[PreserveSig]
	int ExecQueryAsync(
		[MarshalAs(UnmanagedType.BStr)] string strQueryLanguage,
		[MarshalAs(UnmanagedType.BStr)] string strQuery,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int ExecNotificationQuery(
		[MarshalAs(UnmanagedType.BStr)] string strQueryLanguage,
		[MarshalAs(UnmanagedType.BStr)] string strQuery,
		int lFlags,
		IWbemContext? pCtx,
		out IEnumWbemClassObject? ppEnum);

	[PreserveSig]
	int ExecNotificationQueryAsync(
		[MarshalAs(UnmanagedType.BStr)] string strQueryLanguage,
		[MarshalAs(UnmanagedType.BStr)] string strQuery,
		int lFlags,
		IWbemContext? pCtx,
		IWbemObjectSink? pResponseHandler);

	[PreserveSig]
	int ExecMethod(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		[MarshalAs(UnmanagedType.BStr)] string strMethodName,
		int lFlags,
		IWbemContext? pCtx,
		IWbemClassObject? pInParams,
		out IWbemClassObject? ppOutParams,
		out IWbemCallResult? ppCallResult);

	[PreserveSig]
	int ExecMethodAsync(
		[MarshalAs(UnmanagedType.BStr)] string strObjectPath,
		[MarshalAs(UnmanagedType.BStr)] string strMethodName,
		int lFlags,
		IWbemContext? pCtx,
		IWbemClassObject? pInParams,
		IWbemObjectSink? pResponseHandler);
}
