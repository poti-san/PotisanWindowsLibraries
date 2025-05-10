using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIの通知受信シンク。
/// </summary>
public class WbemObjectSink : IDisposable
{
	private IWbemObjectSink? _obj;

	internal WbemObjectSink(object o)
	{
		_obj = (IWbemObjectSink)o;
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

	public void Indicate(WbemClassObject[] objects)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Indicate(
			objects.Length,
			[.. objects.Select(o => (IWbemClassObject)o.WrappedObject!)]));
	}

	public void SetStatus(WbemStatusType status, int hresult, string path, WbemClassObject obj)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.SetStatus((int)status, hresult, path, (IWbemClassObject)obj.WrappedObject!));
	}
}

/// <summary>
/// WMIの通知受信シンク。<see cref="WbemUnsecuredApartment"/>で作成されます。
/// WMIの非同期関数にはこのクラスのインタスタンスを渡します。
/// </summary>
public sealed class WbemAsyncObjectSink : WbemObjectSink
{
	internal WbemAsyncObjectSink(object sink)
		: base(sink)
	{
	}
}

/// <summary>
/// WBEM_STATUS_TYPE
/// </summary>
[Flags]
public enum WbemStatusType : uint
{
	Complete = 0,
	Requirements = 1,
	Progress = 2,
	LoggingInfo = 0x100,
	LoggingInfoProvider = 0x200,
	LoggingInfoHost = 0x400,
	LoggingInfoRepository = 0x800,
	LoggingInfoEss = 0x1000
}
