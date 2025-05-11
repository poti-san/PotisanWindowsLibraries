using System.Collections.Immutable;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIサービス呼び出し時の追加コンテキスト情報。
/// </summary>
public sealed class WbemContext : IDisposable
{
	private IWbemContext? _obj;

	internal WbemContext(object o)
	{
		_obj = (IWbemContext)o;
	}

	public WbemContext()
	{
		Guid CLSID_WbemContext = new("674B6698-EE92-11d0-AD71-00C04FD8FDFF");
		_obj = (IWbemContext)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemContext)!)!;
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

	public WbemContext Clone()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Clone(out var x));
		return new(x);
	}

	public IEnumerable<string> EnumerateValueNames()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration(0));
		try
		{
			for (; ; )
			{
				var hr = _obj.Next(0, out var name, out _);
				if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA)
					break;
				Marshal.ThrowExceptionForHR(hr);
				yield return name;
			}
		}
		finally
		{
			Marshal.ThrowExceptionForHR(_obj.EndEnumeration());
		}
	}
	public ImmutableArray<string> ValueNames => [.. EnumerateValueNames()];

	public IEnumerable<KeyValuePair<string, object?>> EnumerateValues()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration(0));
		try
		{
			for (; ; )
			{
				var hr = _obj.Next(0, out var name, out var value);
				if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA)
					break;
				Marshal.ThrowExceptionForHR(hr);
				yield return new(name, value);
			}
		}
		finally
		{
			Marshal.ThrowExceptionForHR(_obj.EndEnumeration());
		}
	}
	public ImmutableArray<KeyValuePair<string, object?>> Values => [.. EnumerateValues()];

	// TODO: WbemClassObjectの特別扱い
	public void SetValue(string name, object? value)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.SetValue(name, 0, value));
	}

	// TODO: WbemClassObjectの特別扱い
	public object? GetValue(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetValue(name, 0, out var x));
		return x;
	}

	public void DeleteValue(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.DeleteValue(name, 0));
	}

	public void DeleteAll()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.DeleteAll());
	}
}
