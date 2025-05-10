namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("C49E32C7-BC8B-11d2-85D4-00105A1F8304")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemBackupRestore
{
	[PreserveSig]
	int Backup(
		[MarshalAs(UnmanagedType.LPWStr)] string strBackupToFile,
		int lFlags);

	[PreserveSig]
	int Restore(
		[MarshalAs(UnmanagedType.LPWStr)] string strRestoreFromFile,
		int lFlags);
}
