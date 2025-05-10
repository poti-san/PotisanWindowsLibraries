using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIクラスまたはインスタンス。
/// </summary>
public sealed class WbemClassObject : IDisposable
{
	private IWbemClassObject? _obj;

	internal WbemClassObject(object o)
	{
		_obj = (IWbemClassObject)o;
	}

	public WbemClassObject()
	{
		Guid CLSID_WbemClassObject = new("9A653086-174F-11d2-B5F9-00104B703EFD");
		_obj = (IWbemClassObject)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WbemClassObject)!)!;
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

	public WbemQualifierSet QualifierSet
	{
		get
		{
			ThrowIfDisposed();
			Marshal.ThrowExceptionForHR(_obj.GetQualifierSet(out var x));
			return new(x);
		}
	}

	public (object Value, CimType Type, WbemFlavor flavor) this[string name]
	{
		get
		{
			ThrowIfDisposed();
			Marshal.ThrowExceptionForHR(_obj.Get(
				name, 0, out var value, out var type, out var flavor));
			return (value, type, (WbemFlavor)flavor);
		}
		set
		{
			ThrowIfDisposed();
			Marshal.ThrowExceptionForHR(_obj.Put(name, 0, value.Value, value.Type));
		}
	}

	public void DeleteProperty(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Delete(name));
	}

	public enum EnumPropFilter1 : uint
	{
		All = WbemFlags.WBEM_FLAG_ALWAYS,
		HasQualifierName = WbemFlags.WBEM_FLAG_ONLY_IF_TRUE,
		HasNoQualifierName = WbemFlags.WBEM_FLAG_ONLY_IF_FALSE,
		HasQualifierNameAndValue = WbemFlags.WBEM_FLAG_ONLY_IF_IDENTICAL,
	}

	public enum EnumPropFilter2 : uint
	{
		All = 0,
		KeysOnly = WbemFlags.WBEM_FLAG_KEYS_ONLY,
		RefsOnly = WbemFlags.WBEM_FLAG_REFS_ONLY,
	}

	public enum EnumPropFilter3 : uint
	{
		All = 0,
		SystemOnly = WbemFlags.WBEM_FLAG_SYSTEM_ONLY,
		NonSystemOnly = WbemFlags.WBEM_FLAG_NONSYSTEM_ONLY,
		ClassOverridesOnly = WbemFlags.WBEM_FLAG_CLASS_OVERRIDES_ONLY,
		ClassLocalAndOverrides = WbemFlags.WBEM_FLAG_CLASS_LOCAL_AND_OVERRIDES,
	}

	public IEnumerable<string> EnumeratePropertyNames(
		EnumPropFilter1 filter1 = EnumPropFilter1.All,
		EnumPropFilter2 filter2 = EnumPropFilter2.All,
		EnumPropFilter3 filter3 = EnumPropFilter3.All)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration((int)filter1 | (int)filter2 | (int)filter3));
		for (; ; )
		{
			var hr = _obj.Next(0, out var name, out _, out Unsafe.NullRef<CimType>(), out Unsafe.NullRef<int>());
			if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA) break;
			Marshal.ThrowExceptionForHR(hr);
			yield return name!;
		}
		Marshal.ThrowExceptionForHR(_obj.EndEnumeration());
	}

	public ImmutableArray<string> AllPropertyNames => [.. EnumeratePropertyNames()];
	public ImmutableArray<string> SystemPropertyNames => [.. EnumeratePropertyNames(filter3: EnumPropFilter3.SystemOnly)];
	public ImmutableArray<string> NonSystemPropertyNames => [.. EnumeratePropertyNames(filter3: EnumPropFilter3.NonSystemOnly)];

	public IEnumerable<(string Name, object? Value, CimType Type, WbemFlavor Flavor)> EnumeratePropertyInfos(
		EnumPropFilter1 filter1 = EnumPropFilter1.All,
		EnumPropFilter2 filter2 = EnumPropFilter2.All,
		EnumPropFilter3 filter3 = EnumPropFilter3.All)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginEnumeration((int)filter1 | (int)filter2 | (int)filter3));
		for (; ; )
		{
			var hr = _obj.Next(0, out var name, out var value, out var type, out var flavor);
			if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA) break;
			Marshal.ThrowExceptionForHR(hr);
			yield return (name!, value, type, (WbemFlavor)flavor);
		}
		Marshal.ThrowExceptionForHR(_obj.EndEnumeration());
	}

	public ImmutableArray<(string Name, object? Value, CimType Type, WbemFlavor Flavor)> AllPropertyInfos
		=> [.. EnumeratePropertyInfos()];
	public ImmutableArray<(string Name, object? Value, CimType Type, WbemFlavor Flavor)> SystemPropertyInfos
		=> [.. EnumeratePropertyInfos(filter3: EnumPropFilter3.SystemOnly)];
	public ImmutableArray<(string Name, object? Value, CimType Type, WbemFlavor Flavor)> NonSystemPropertyInfos
		=> [.. EnumeratePropertyInfos(filter3: EnumPropFilter3.NonSystemOnly)];

	public WbemQualifierSet GetPropertyQualifierSet(string propName)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetPropertyQualifierSet(propName, out var x));
		return new(x);
	}

	public WbemClassObject Clone()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.Clone(out var x));
		return new(x!);
	}

	private string GetObjectText(int flags)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetObjectText(flags, out var x));
		return x!;
	}

	public string ObjectText => GetObjectText(0);
	public string ObjectTextNoFlavors => GetObjectText((int)WbemFlags.WBEM_FLAG_NO_FLAVORS);

	public WbemClassObject SpawnDerivedClass()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.SpawnDerivedClass(0, out var x));
		return new(x!);
	}

	public WbemClassObject SpawnInstance()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.SpawnInstance(0, out var x));
		return new(x!);
	}

	public bool CompareTo(WbemClassObject other, WbemComparison flags = WbemComparison.IncludeAll)
	{
		ThrowIfDisposed();
		var hr = _obj.CompareTo((int)flags, (IWbemClassObject)other.WrappedObject!);
		Marshal.ThrowExceptionForHR(hr);
		return hr == (int)WbemStatus.WBEM_S_SAME;
	}

	public string GetPropertyOrigin(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetPropertyOrigin(name, out var s));
		return s!;
	}

	public bool InheritsFrom(string ancestor)
	{
		ThrowIfDisposed();
		var hr = _obj.InheritsFrom(ancestor);
		Marshal.ThrowExceptionForHR(hr);
		return hr == (int)WbemStatus.WBEM_S_NO_ERROR;
	}

	public (WbemClassObject InSignature, WbemClassObject OutSignature) GetMethod(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetMethod(name, 0, out var insig, out var outsig));
		return (new(insig!), new(outsig!));
	}

	public void PutMethod(string name, WbemClassObject? InSignature = null, WbemClassObject? OutSignature = null)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.PutMethod(
			name,
			0,
			InSignature?.WrappedObject as IWbemClassObject,
			OutSignature?.WrappedObject as IWbemClassObject));
	}

	public void DeleteMethod(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.DeleteMethod(name));
	}

	public enum EnumMethodFilter : uint
	{
		All = 0,
		LocalOnly = WbemFlags.WBEM_FLAG_LOCAL_ONLY,
		PropagatedOnly = WbemFlags.WBEM_FLAG_PROPAGATED_ONLY,
	}

	public IEnumerable<string> EnumerateMethodNames(EnumMethodFilter filter = EnumMethodFilter.All)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginMethodEnumeration((int)filter));
		for (; ; )
		{
			var hr = _obj.NextMethod(0, out var name, out _, out _);
			if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA) break;
			Marshal.ThrowExceptionForHR(hr);
			yield return name!;
		}
		Marshal.ThrowExceptionForHR(_obj.EndMethodEnumeration());
	}

	public ImmutableArray<string> AllMethodNames => [.. EnumerateMethodNames()];
	public ImmutableArray<string> LocalMethodNames => [.. EnumerateMethodNames(EnumMethodFilter.LocalOnly)];
	public ImmutableArray<string> PropagatedMethodNames => [.. EnumerateMethodNames(EnumMethodFilter.PropagatedOnly)];

	public IEnumerable<(string Name, WbemClassObject? inSignature, WbemClassObject? outSignature)> EnumerateMethodInfos(
		EnumMethodFilter filter = EnumMethodFilter.All)
	{
		ThrowIfDisposed();

		Marshal.ThrowExceptionForHR(_obj.BeginMethodEnumeration((int)filter));
		for (; ; )
		{
			var hr = _obj.NextMethod(0, out var name, out var inSig, out var outSig);
			if (hr == (int)WbemStatus.WBEM_S_NO_MORE_DATA) break;
			Marshal.ThrowExceptionForHR(hr);
			yield return (name!, inSig != null ? new(inSig) : null, outSig != null ? new(outSig) : null);
		}
		Marshal.ThrowExceptionForHR(_obj.EndMethodEnumeration());
	}

	public ImmutableArray<(string Name, WbemClassObject? inSignature, WbemClassObject? outSignature)> AllMethodInfos
		=> [.. EnumerateMethodInfos()];
	public ImmutableArray<(string Name, WbemClassObject? inSignature, WbemClassObject? outSignature)> LocalMethodInfos
		=> [.. EnumerateMethodInfos(EnumMethodFilter.LocalOnly)];
	public ImmutableArray<(string Name, WbemClassObject? inSignature, WbemClassObject? outSignature)> PropagatedMethodInfos
		=> [.. EnumerateMethodInfos(EnumMethodFilter.PropagatedOnly)];

	public WbemQualifierSet GetMethodQualifierSet(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetMethodQualifierSet(name, out var x));
		return new(x);
	}

	public string GetOrigin(string name)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.GetMethodOrigin(name, out var x));
		return x!;
	}

	public string? ClassName => this["__CLASS"].Value is string x ? x : null;
	public string? Derivation => this["__DERIVATION"].Value is string x ? x : null;
	public string? Dynasty => this["__DYNASTY"].Value is string x ? x : null;
	public WbemGenus? Genus => this["__GENUS"].Value is int x ? (WbemGenus)x : null;
	public string? Namespace => this["__NAMESPACE"].Value is string x ? x : null;
	public string? Path => this["__PATH"].Value is string x ? x : null;
	public int? NonSystemPropertyCount => this["__PROPERTY_COUNT"].Value is int x ? x : null;
	public string? RelativePath => this["__RELPATH"].Value is string x ? x : null;
	public string? ServerName => this["__SERVER"].Value is string x ? x : null;
	public string? SuperclassName => this["__SUPERCLASS"].Value is string x ? x : null;

	public string? Name => this["NAME"].Value is string x ? x : null;
}
