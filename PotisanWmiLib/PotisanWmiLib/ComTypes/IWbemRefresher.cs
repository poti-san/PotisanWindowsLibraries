namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("49353c99-516b-11d1-aea6-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemRefresher
{
	[PreserveSig]
	int Refresh(
		int lFlags);
}
