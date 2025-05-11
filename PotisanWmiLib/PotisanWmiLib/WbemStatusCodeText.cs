using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIエラーの情報取得機能。
/// </summary>
public sealed class WbemStatusCodeText : IDisposable
{
	private IWbemStatusCodeText? _obj;

	public WbemStatusCodeText()
	{
		Guid CLSID_WbemStatusCodeText = new("eb87e1bd-3233-11d2-aec9-00c04fb68820");
		_obj = (IWbemStatusCodeText)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemStatusCodeText)!)!;
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

	public string GetErrorCodeText(int code)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetErrorCodeText(code, 0, 0, out var x));
		return x;
	}

	public string GetFacilityCodeText(int code)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetFacilityCodeText(code, 0, 0, out var x));
		return x;
	}
}
