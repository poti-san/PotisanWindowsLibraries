using System.ComponentModel;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIクラスのテキスト表現。
/// </summary>
public sealed class WbemObjectTextSource : IDisposable
{
	private IWbemObjectTextSrc? _obj;

	internal WbemObjectTextSource(object o)
	{
		_obj = (IWbemObjectTextSrc)o;
	}

	public WbemObjectTextSource()
	{
		Guid CLSID_WbemObjectTextSrc = new("8D1C559D-84F0-4bb3-A7D5-56A7435A9BA6");
		_obj = (IWbemObjectTextSrc)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemObjectTextSrc)!)!;
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

	public string GetText(WbemClassObject obj, WmiObjectText format)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetText(0, (IWbemClassObject)obj.WrappedObject!, (uint)format, null, out var x));
		return x;
	}

	// TODO: CHECK
	public WbemClassObject CreateFromText(string text, WmiObjectText format)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.CreateFromText(0, text, (uint)format, null, out var x));
		return new(x);
	}
}

public enum WmiObjectText : uint
{
	CimDtd2dot0 = 1,
	WmiDtd2dot0 = 2,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt1 = 3,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt2 = 4,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt3 = 5,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt4 = 6,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt5 = 7,
	[EditorBrowsable(EditorBrowsableState.Never)]
	EmiExt6 = 8,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt7 = 9,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt8 = 10,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt9 = 11,
	[EditorBrowsable(EditorBrowsableState.Never)]
	WmiExt10 = 12,
	[EditorBrowsable(EditorBrowsableState.Never)]
	Last = 13
}
