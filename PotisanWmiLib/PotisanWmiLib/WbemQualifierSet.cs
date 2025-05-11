using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI修飾子セット。
/// </summary>
public class WbemQualifierSet : IDisposable
{
	private IWbemQualifierSet? _obj;

	internal WbemQualifierSet(object o)
	{
		_obj = (IWbemQualifierSet)o;
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

	public (object Value, WbemFlavor Flavor) Get(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Get(name, 0, out var value, out var flavor));
		return (value, (WbemFlavor)flavor);
	}

	public void Put(string name, object value, WbemFlavor flavor)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Put(name, value, (int)flavor));
	}

	public void Delete(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Delete(name));
	}

	private IEnumerable<string> EnumerateQualifierNames(int flags)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration(flags));
		try
		{
			for (; ; )
			{
				var hr = _obj.Next(0, out var s, out Unsafe.NullRef<object>()!, out Unsafe.NullRef<int>());
				if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA)
					break;
				Marshal.ThrowExceptionForHR(hr);
				yield return s;
			}
		}
		finally
		{
			_obj.EndEnumeration();
		}
	}
	public IEnumerable<string> EnumerateAllQualifierNames() => EnumerateQualifierNames(0);
	public IEnumerable<string> EnumerateLocalQualifierNames() => EnumerateQualifierNames((int)WbemFlags.WBEM_FLAG_LOCAL_ONLY);
	public IEnumerable<string> EnumeratePropagatedQualifierNames() => EnumerateQualifierNames((int)WbemFlags.WBEM_FLAG_PROPAGATED_ONLY);

	public ImmutableArray<string> AllQualifierNames => [.. EnumerateAllQualifierNames()];
	public ImmutableArray<string> LocalQualifierNames => [.. EnumerateLocalQualifierNames()];
	public ImmutableArray<string> PropagatedQualifierNames => [.. EnumeratePropagatedQualifierNames()];

	private IEnumerable<(string Name, object Value, WbemFlavor Flavor)> EnumerateQualifiers(int flags)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration(flags));
		try
		{
			for (; ; )
			{
				var hr = _obj.Next(0, out var s, out var o, out var flavor);
				if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA)
					break;
				Marshal.ThrowExceptionForHR(hr);
				yield return (s, o, (WbemFlavor)flavor);
			}
		}
		finally
		{
			_obj.EndEnumeration();
		}
	}
	public IEnumerable<(string Name, object Value, WbemFlavor Flavor)> EnumerateAllQualifiers() => EnumerateQualifiers(0);
	public IEnumerable<(string Name, object Value, WbemFlavor Flavor)> EnumerateLocalQualifiers() => EnumerateQualifiers((int)WbemFlags.WBEM_FLAG_LOCAL_ONLY);
	public IEnumerable<(string Name, object Value, WbemFlavor Flavor)> EnumeratePropagatedQualifiers() => EnumerateQualifiers((int)WbemFlags.WBEM_FLAG_PROPAGATED_ONLY);

	public ImmutableArray<(string Name, object Value, WbemFlavor Flavor)> AllQualifiers => [.. EnumerateAllQualifiers()];
	public ImmutableArray<(string Name, object Value, WbemFlavor Flavor)> LocalQualifiers => [.. EnumerateLocalQualifiers()];
	public ImmutableArray<(string Name, object Value, WbemFlavor Flavor)> PropagatedQualifiers => [.. EnumeratePropagatedQualifiers()];
}
