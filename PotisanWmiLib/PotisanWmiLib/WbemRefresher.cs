using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI更新可能オブジェクトの更新機能。
/// </summary>
public sealed class WbemRefresher : IDisposable
{
	private IWbemRefresher? _obj;

	internal WbemRefresher(object o)
	{
		_obj = (IWbemRefresher)o;
	}

	public WbemRefresher()
	{
		Guid CLSID_WbemRefresher = new("c71566f2-561e-11d1-ad87-00c04fd8fdff");
		_obj = (IWbemRefresher)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemRefresher)!)!;
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

	public void Refresh(bool autoRecorrent)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Refresh((int)(autoRecorrent
			? WbemFlags.WBEM_FLAG_REFRESH_AUTO_RECONNECT : WbemFlags.WBEM_FLAG_REFRESH_NO_AUTO_RECONNECT)));
	}
}