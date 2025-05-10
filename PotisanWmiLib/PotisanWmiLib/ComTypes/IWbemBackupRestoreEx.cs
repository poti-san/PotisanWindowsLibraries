namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("A359DEC5-E813-4834-8A2A-BA7F1D777D76")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemBackupRestoreEx // IWbemBackupRestore
{
	#region IWbemBackupRestore

	[PreserveSig]
	int Backup(
		[MarshalAs(UnmanagedType.LPWStr)] string strBackupToFile,
		int lFlags);

	[PreserveSig]
	int Restore(
		[MarshalAs(UnmanagedType.LPWStr)] string strRestoreFromFile,
		int lFlags);

	#endregion

	[PreserveSig]
	int Pause();

	[PreserveSig]
	int Resume();
}
