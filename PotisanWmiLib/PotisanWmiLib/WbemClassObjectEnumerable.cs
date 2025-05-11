using System.Collections;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIクラス列挙。
/// </summary>
public sealed class WbemClassObjectEnumerable : IDisposable, IEnumerable<WbemClassObject>
{
	private IEnumWbemClassObject? _obj;
	public ComAuthenticationLevel AuthenticationLevel { get; }
	public ComImpersonateLevel ImpersonateLevel { get; }
	public OleAuthenticationCap AuthenticationCapabilities { get; }
	public WbemMillisecond Timeout { get; }

	internal WbemClassObjectEnumerable(
		object o,
		WbemMillisecond timeout,
		ComAuthenticationLevel authLevel,
		ComImpersonateLevel impLevel,
		OleAuthenticationCap authCaps)
	{
		_obj = (IEnumWbemClassObject)o;
		WmiUtility.SetProxyBlanket(o, authLevel, impLevel, authCaps);

		AuthenticationLevel = authLevel;
		ImpersonateLevel = impLevel;
		AuthenticationCapabilities = authCaps;
		Timeout = timeout;
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

	public IEnumerator<WbemClassObject> GetEnumerator()
	{
		ThrowIfDisposed();
		var _timeout = Timeout.Value;
		for (; ; )
		{
			var hr = _obj.Next(_timeout, 1, out var x, out _);
			if (hr == 1) break;
			Marshal.ThrowExceptionForHR(hr);
			yield return new(x!);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Reset()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Reset());
	}

	public WbemClassObjectEnumerable Clone(WbemMillisecond? timeout = null)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Clone(out var x));
		return new(x!, timeout ?? Timeout, AuthenticationLevel, ImpersonateLevel, AuthenticationCapabilities);
	}
}