using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIリポジトリのバックアップと復元。
/// </summary>
public sealed class WbemBackupRestore : IDisposable
{
	private IWbemBackupRestore? _obj;

	internal WbemBackupRestore(object o)
	{
		_obj = (IWbemBackupRestore)o;
	}

	public WbemBackupRestore()
	{
		Guid CLSID_WbemBackupRestore = new("C49E32C6-BC8B-11d2-85D4-00105A1F8304");
		_obj = (IWbemBackupRestore)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemBackupRestore)!)!;
	}

	[NotNullIfNotNull(nameof(_obj))]
	public object? WrappedObject => _obj;

	[MemberNotNullWhen(false, nameof(_obj))]
	public bool IsDisposed => _obj == null;

	public void Dispose()
	{
		if (_obj == null) return;
		Marshal.FinalReleaseComObject(_obj);
		_obj = null;
		GC.SuppressFinalize(this);
	}

	[MemberNotNull(nameof(_obj))]
	public void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_obj == null, typeof(WbemLocator));
	}

	public void Backup(string backupToFile)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Backup(backupToFile, 0));
	}

	public void Backup(string backupToFile, bool restoresForceShutdown)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Backup(backupToFile,
			(int)(restoresForceShutdown ? WbemFlags.WBEM_FLAG_BACKUP_RESTORE_FORCE_SHUTDOWN : WbemFlags.WBEM_FLAG_BACKUP_RESTORE_DEFAULT)));
	}
}