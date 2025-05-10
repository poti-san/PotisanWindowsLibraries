using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIロケーター。
/// </summary>
public sealed class WbemLocator : IDisposable
{
	private IWbemLocator? _obj;

	public WbemLocator()
	{
		Guid CLSID_WbemLocator = new("4590f811-1d3a-11d0-891f-00aa004b2e24");

		var t = Type.GetTypeFromCLSID(CLSID_WbemLocator)!;
		_obj = (IWbemLocator)Activator.CreateInstance(t)!;
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

	public WbemNamespace ConnectServer(
		string @namespace,
		bool usesMaxWait = false,
		string? user = null,
		string? password = null,
		string? locale = null,
		string? authority = null,
		ComAuthenticationLevel authLevel = ComAuthenticationLevel.Call,
		ComImpersonateLevel impLevel = ComImpersonateLevel.Impersonate,
		OleAuthenticationCap authCaps = OleAuthenticationCap.None)
	{
		const uint WBEM_FLAG_CONNECT_USE_MAX_WAIT = 0x80;

		ArgumentException.ThrowIfNullOrWhiteSpace(@namespace);

		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.ConnectServer(@namespace, user, password, locale,
			(int)(usesMaxWait ? WBEM_FLAG_CONNECT_USE_MAX_WAIT : 0), authority, null, out var x));
		return new(x!, authLevel, impLevel, authCaps);
	}
}
