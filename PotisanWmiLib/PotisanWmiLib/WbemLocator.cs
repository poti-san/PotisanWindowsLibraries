using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIロケーター。
/// </summary>
public sealed class WbemLocator : IWmiDisposable
{
	private IWbemLocator? _obj;

	/// <summary>
	/// 新しいWMIロケーターを作成します。
	/// </summary>
	public WbemLocator()
	{
		Guid CLSID_WbemLocator = new("4590f811-1d3a-11d0-891f-00aa004b2e24");

		var t = Type.GetTypeFromCLSID(CLSID_WbemLocator)!;
		_obj = (IWbemLocator)Activator.CreateInstance(t)!;
	}

	/// <inheritdoc/>
	[NotNullIfNotNull(nameof(_obj))]
	public object? WrappedObject => _obj;

	/// <inheritdoc/>
	[MemberNotNullWhen(false, nameof(_obj))]
	public bool IsDisposed => _obj == null;

	/// <inheritdoc/>
	public void Dispose()
	{
		if (_obj == null) return;

		Marshal.FinalReleaseComObject(_obj);
		_obj = null;
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc/>
	[MemberNotNull(nameof(_obj))]
	public void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_obj == null, typeof(WbemLocator));
	}

	/// <summary>
	/// サーバーの名前空間に接続します。
	/// </summary>
	/// <param name="nameSpace">名前空間。</param>
	/// <param name="usesMaxWait">最大待機時間の適用。</param>
	/// <param name="user">接続時のユーザー名。省略時は現在のセキュリティコンテキストを使用します。</param>
	/// <param name="password">接続時のパスワード。省略時は現在のセキュリティコンテキストを使用します。</param>
	/// <param name="locale">ロケール。省略時は現在のロケールです。</param>
	/// <param name="authority">権限。</param>
	/// <param name="authLevel">権限レベル。</param>
	/// <param name="impLevel">偽装レベル。</param>
	/// <param name="authCaps">権限キャパシティ。</param>
	/// <returns></returns>
	public WbemNamespace ConnectServer(
		string nameSpace,
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

		ArgumentException.ThrowIfNullOrWhiteSpace(nameSpace);

		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.ConnectServer(nameSpace, user, password, locale,
			(int)(usesMaxWait ? WBEM_FLAG_CONNECT_USE_MAX_WAIT : 0), authority, null, out var x));
		return new(x!, authLevel, impLevel, authCaps);
	}
}
