using System.Runtime.CompilerServices;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI名前空間。
/// </summary>
public sealed class WbemNamespace : IDisposable
{
	private IWbemServices? _obj;
	public ComAuthenticationLevel AuthenticationLevel { get; }
	public ComImpersonateLevel ImpersonateLevel { get; }
	public OleAuthenticationCap AuthenticationCapabilities { get; }

	internal WbemNamespace(object o, ComAuthenticationLevel authLevel, ComImpersonateLevel impLevel, OleAuthenticationCap authCaps)
	{
		_obj = (IWbemServices)o;
		WmiUtility.SetProxyBlanket(_obj, authLevel, impLevel, authCaps);

		AuthenticationLevel = authLevel;
		ImpersonateLevel = impLevel;
		AuthenticationCapabilities = authCaps;
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

	public WbemNamespace OpenNamespace(string @namespace)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.OpenNamespace(@namespace, 0, null, out var x, out Unsafe.NullRef<IWbemCallResult?>()));
		return new(x!, AuthenticationLevel, ImpersonateLevel, AuthenticationCapabilities);
	}

	public void CancelAsyncCall(WbemAsyncObjectSink sink)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.CancelAsyncCall((IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemAsyncObjectSink QueryObjectSink()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.QueryObjectSink(0, out var x));
		return new(x!);
	}

	public WbemClassObject GetObject(string path, bool usesAmendedQualifiers = false, bool readsDirect = false)
	{
		ThrowIfDisposed();

		var flags = (usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (readsDirect ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0);
		Marshal.ThrowExceptionForHR(_obj.GetObject(path, (int)flags, null, out var x, out var t));
		Marshal.ReleaseComObject(t!);
		return new(x!);
	}

	public void GetObjectAsync(
		WbemAsyncObjectSink sink,
		string path,
		bool usesAmendedQualifiers = false,
		bool directRead = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.GetObjectAsync(path, (int)flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public void PutClass(
		WbemClassObject instance,
		CreateOrUpdate createOrUpdate = CreateOrUpdate.Both,
		bool usesAmendedQualifiers = false,
		bool ownerUpdate = false,
		UpdateMode updateMode = UpdateMode.Compatible)
	{
		ThrowIfDisposed();

		var flags = (int)createOrUpdate
			| (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)(ownerUpdate ? WbemFlags.WBEM_FLAG_OWNER_UPDATE : 0)
			| (int)updateMode;
		Marshal.ThrowExceptionForHR(_obj.PutClass(
			(IWbemClassObject)instance.WrappedObject!,
			flags, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	public void PutClassAsync(
		WbemAsyncObjectSink sink,
		WbemClassObject instance,
		CreateOrUpdate createOrUpdate = CreateOrUpdate.Both,
		bool usesAmendedQualifiers = false,
		bool ownerUpdate = false,
		UpdateMode updateMode = UpdateMode.Compatible,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)createOrUpdate
			| (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)(ownerUpdate ? WbemFlags.WBEM_FLAG_OWNER_UPDATE : 0)
			| (int)updateMode
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.PutClassAsync(
			(IWbemClassObject)instance.WrappedObject!,
			flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public void DeleteClass(
		string objectPath,
		bool ownerUpdate = false)
	{
		ThrowIfDisposed();

		var flags = (int)(ownerUpdate ? WbemFlags.WBEM_FLAG_OWNER_UPDATE : 0);
		Marshal.ThrowExceptionForHR(_obj.DeleteClass(objectPath, flags, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	public void DeleteClassAsync(
		WbemAsyncObjectSink sink,
		string objectPath,
		bool ownerUpdate = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)(ownerUpdate ? WbemFlags.WBEM_FLAG_OWNER_UPDATE : 0) | (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.DeleteClassAsync(objectPath, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemClassObjectEnumerable GetClassEnumerable(
		string? superclass = null,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool returnsImmediately = true,
		WbemMillisecond? timeout = null,
		ComAuthenticationLevel? authnLevel = null,
		ComImpersonateLevel? impLevel = null,
		OleAuthenticationCap? authnCaps = null)
	{
		ThrowIfDisposed();

		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateClassEnum(superclass, flags, null, out var x));
		return new(x, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}

	public void GetClassEnumerableAsync(
		WbemAsyncObjectSink sink,
		string? superclass = null,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateClassEnumAsync(superclass, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public void PutInstance(
		WbemClassObject instance,
		CreateOrUpdate createOrUpdate = CreateOrUpdate.Both,
		bool usesAmendedQualifiers = false)
	{
		ThrowIfDisposed();

		var flags = (int)createOrUpdate
			| (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0);
		Marshal.ThrowExceptionForHR(_obj.PutInstance(
			(IWbemClassObject)instance.WrappedObject!,
			flags, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	public void PutInstanceAsync(
		WbemAsyncObjectSink sink,
		WbemClassObject instance,
		CreateOrUpdate createOrUpdate = CreateOrUpdate.Both,
		bool usesAmendedQualifiers = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)createOrUpdate
			| (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.PutInstanceAsync(
			(IWbemClassObject)instance.WrappedObject!,
			flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public void DeleteInstance(string objectPath)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.DeleteInstance(objectPath, 0, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	public void DeleteInstanceAsync(
		string objectPath,
		WbemAsyncObjectSink sink,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.DeleteInstanceAsync(objectPath, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemClassObjectEnumerable GetInstanceEnumerable(
		string className,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool readsDirect = false,
		bool returnsImmediately = true,
		WbemMillisecond? timeout = null,
		ComAuthenticationLevel? authnLevel = null,
		ComImpersonateLevel? impLevel = null,
		OleAuthenticationCap? authnCaps = null)
	{
		ThrowIfDisposed();

		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(readsDirect ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateInstanceEnum(className, flags, null, out var x));
		return new(x, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}

	public void GetInstanceEnumerableAsync(
		WbemAsyncObjectSink sink,
		string className,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool readsDirect = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(readsDirect ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateInstanceEnumAsync(className, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemClassObjectEnumerable ExecQuery(
		string query,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool readsDirect = false,
		bool ensuresLocatable = true,
		bool prototype = false,
		bool returnsImmediately = true,
		WbemMillisecond? timeout = null,
		ComAuthenticationLevel? authnLevel = null,
		ComImpersonateLevel? impLevel = null,
		OleAuthenticationCap? authnCaps = null)
	{
		ThrowIfDisposed();
		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(readsDirect ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(ensuresLocatable ? WbemFlags.WBEM_FLAG_ENSURE_LOCATABLE : 0)
			| (int)(prototype ? WbemFlags.WBEM_FLAG_PROTOTYPE : 0)
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecQuery("WQL", query, flags, null, out var x));
		return new(x!, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}

	public void ExecQueryAsync(
		WbemAsyncObjectSink sink,
		string query,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool readsDirect = false,
		bool ensuresLocatable = true,
		bool prototype = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(readsDirect ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(ensuresLocatable ? WbemFlags.WBEM_FLAG_ENSURE_LOCATABLE : 0)
			| (int)(prototype ? WbemFlags.WBEM_FLAG_PROTOTYPE : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecQueryAsync("WQL", query, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemClassObjectEnumerable ExecNotificationQuery(
		string query,
		bool bidirectional = false,
		WbemMillisecond? timeout = null,
		bool returnsImmediately = true,
		ComAuthenticationLevel? authnLevel = null,
		ComImpersonateLevel? impLevel = null,
		OleAuthenticationCap? authnCaps = null)
	{
		ThrowIfDisposed();

		var flags = (int)(bidirectional ? WbemFlags.WBEM_FLAG_BIDIRECTIONAL : 0)
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecNotificationQuery("WQL", query, flags, null, out var x));
		return new(x!, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}

	public void ExecNotificationQueryAsync(
		WbemAsyncObjectSink sink,
		string query,
		bool bidirectional = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)(bidirectional ? WbemFlags.WBEM_FLAG_BIDIRECTIONAL : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecNotificationQueryAsync("WQL", query, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	public WbemClassObject ExecMethod(string objectPath, string methodName, WbemClassObject? inParams)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.ExecMethod(objectPath, methodName, 0, null,
			inParams?.WrappedObject as IWbemClassObject, out var x, out Unsafe.NullRef<IWbemCallResult?>()));
		return new(x!);
	}

	public void ExecMethodAsync(WbemAsyncObjectSink sink, string objectPath, string methodName, WbemClassObject? inParams,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecMethodAsync(objectPath, methodName, flags, null,
			inParams?.WrappedObject as IWbemClassObject, (IWbemObjectSink)sink.WrappedObject!));
	}

	public enum DeepOrShallow : uint
	{
		Shallow = WbemFlags.WBEM_FLAG_SHALLOW,
		Deep = WbemFlags.WBEM_FLAG_DEEP,
	}

	public enum Direction : uint
	{
		Forward = WbemFlags.WBEM_FLAG_FORWARD_ONLY,
		Bidirectional = WbemFlags.WBEM_FLAG_BIDIRECTIONAL,
	}

	public enum CreateOrUpdate : uint
	{
		Both = WbemFlags.WBEM_FLAG_CREATE_OR_UPDATE,
		CreateOnly = WbemFlags.WBEM_FLAG_CREATE_ONLY,
		UpdateOnly = WbemFlags.WBEM_FLAG_UPDATE_ONLY,
	}

	public enum UpdateMode : uint
	{
		Compatible = WbemFlags.WBEM_FLAG_UPDATE_COMPATIBLE,
		SafeMode = WbemFlags.WBEM_FLAG_UPDATE_SAFE_MODE,
		Force = WbemFlags.WBEM_FLAG_UPDATE_FORCE_MODE,
	}
}
