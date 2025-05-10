using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// 非同期コールバックのラップ機能。
/// </summary>
public sealed class WbemUnsecuredApartment : IDisposable
{
	private IWbemUnsecuredApartment? _obj;

	internal WbemUnsecuredApartment(object o)
	{
		_obj = (IWbemUnsecuredApartment)o;
	}

	public WbemUnsecuredApartment()
	{
		Guid CLSID_UnsecuredApartment = new("49bd2028-1523-11d1-ad79-00c04fd8fdff");
		_obj = (IWbemUnsecuredApartment)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_UnsecuredApartment)!)!;
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

	public WbemAsyncObjectSink CreateObjectStub(Action<WbemClassObject[]> indicate, Action<WbemStatusType, int, string?, WbemClassObject?> setStatus)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.CreateObjectStub(new WbemObjectSinkWorker(indicate, setStatus), out var x));
		return new(x!);
	}

	public WbemAsyncObjectSink CreateSinkStub(WbemUnsecuredApartmentCheckAccess check, Action<WbemClassObject[]> indicate, Action<WbemStatusType, int, string?, WbemClassObject?> setStatus)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.CreateSinkStub(new WbemObjectSinkWorker(indicate, setStatus), (uint)check, null, out var x));
		return new(x!);
	}
}

public enum WbemUnsecuredApartmentCheckAccess : uint
{
	DefaultCheckAccess = 0,
	CheckAccess = 1,
	NoChekAccess = 2,
}

internal sealed class WbemObjectSinkWorker(
	Action<WbemClassObject[]> indicate,
	Action<WbemStatusType, int, string?, WbemClassObject?> setStatus
	) : IWbemObjectSink
{
	private readonly Action<WbemClassObject[]> _indicate = indicate;
	private readonly Action<WbemStatusType, int, string?, WbemClassObject?> _setStatus = setStatus;

	public int Indicate(int lObjectCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IWbemClassObject[]? apObjArray)
	{
		try
		{
			_indicate(apObjArray != null ? [.. apObjArray.Select(p => new WbemClassObject(p))] : []);
			return 0;
		}
		catch (Exception ex)
		{
			return ex.HResult;
		}
	}

	public int SetStatus(int lFlags, int hResult, [MarshalAs(UnmanagedType.BStr)] string? strParam, IWbemClassObject? pObjParam)
	{
		try
		{
			_setStatus((WbemStatusType)lFlags, hResult, strParam, pObjParam != null ? new(pObjParam) : null);
			return 0;
		}
		catch (Exception ex)
		{
			return ex.HResult;
		}
	}
}
