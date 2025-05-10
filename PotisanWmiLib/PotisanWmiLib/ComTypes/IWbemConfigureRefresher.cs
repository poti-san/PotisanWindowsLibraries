namespace Potisan.Windows.Wmi.ComTypes;

// TODO
[ComImport]
[Guid("49353c92-516b-11d1-aea6-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemConfigureRefresher
{
	[PreserveSig]
	int AddObjectByPath(
		IWbemServices pNamespace,
		string wszPath,
		int lFlags,
		IWbemContext? pContext,
		out IWbemClassObject ppRefreshable,
		out int plId);

	[PreserveSig]
	int AddObjectByTemplate(
		IWbemServices pNamespace,
		IWbemClassObject pTemplate,
		int lFlags,
		IWbemContext? pContext,
		out IWbemClassObject ppRefreshable,
		out int plId);

	[PreserveSig]
	int AddRefresher(
		IWbemRefresher pRefresher,
		int lFlags,
		out int plId);

	[PreserveSig]
	int Remove(
		int lId,
		int lFlags);

	[PreserveSig]
	int AddEnum(
		IWbemServices pNamespace,
		string wszClassName,
		int lFlags,
		IWbemContext? pContext,
		out IWbemHiPerfEnum ppEnum,
		out int plId);
}
