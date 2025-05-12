using System.Runtime.CompilerServices;

using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI名前空間。
/// </summary>
/// <remarks>
/// <see cref="WbemLocator"/>から作成します。
/// </remarks>
public sealed class WbemNamespace : IWmiDisposable
{
	private IWbemServices? _obj;
	/// <summary>
	/// 作成時の権限レベル。
	/// </summary>
	public ComAuthenticationLevel AuthenticationLevel { get; }
	/// <summary>
	/// 作成時の偽装レベル。
	/// </summary>
	public ComImpersonateLevel ImpersonateLevel { get; }
	/// <summary>
	/// 作成時の権限キャパシティ。
	/// </summary>
	public OleAuthenticationCap AuthenticationCapabilities { get; }

	internal WbemNamespace(object o, ComAuthenticationLevel authLevel, ComImpersonateLevel impLevel, OleAuthenticationCap authCaps)
	{
		_obj = (IWbemServices)o;
		WmiUtility.SetProxyBlanket(_obj, authLevel, impLevel, authCaps);

		AuthenticationLevel = authLevel;
		ImpersonateLevel = impLevel;
		AuthenticationCapabilities = authCaps;
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
	/// 下位名前空間を開きます。
	/// <see cref="WbemLocator"/>の新規作成と同じですが、相対パスを使用できます。
	/// </summary>
	public WbemNamespace OpenNamespace(string nameSpace)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.OpenNamespace(nameSpace, 0, null, out var x, out Unsafe.NullRef<IWbemCallResult?>()));
		return new(x!, AuthenticationLevel, ImpersonateLevel, AuthenticationCapabilities);
	}

	/// <summary>
	/// 非同期処理を中断します。
	/// </summary>
	public void CancelAsyncCall(WbemAsyncObjectSink sink)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.CancelAsyncCall((IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// 非同期処理シンクを取得します。
	/// </summary>
	/// <returns></returns>
	public WbemAsyncObjectSink QueryObjectSink()
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.QueryObjectSink(0, out var x));
		return new(x!);
	}

	/// <summary>
	/// パスからクラスまたはインスタンスを取得します。
	/// </summary>
	/// <param name="path">オブジェクトパス。<c>null</c>の場合は新しいクラスが作成されます。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <returns></returns>
	public WbemClassObject GetObject(string? path, bool usesAmendedQualifiers = false, bool directRead = false)
	{
		ThrowIfDisposed();

		var flags = (usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0);
		Marshal.ThrowExceptionForHR(_obj.GetObject(path, (int)flags, null, out var x, out var t));
		Marshal.ReleaseComObject(t!);
		return new(x!);
	}

	/// <summary>
	/// パスからクラスまたはインスタンスを非同期で取得します。
	/// </summary>
	/// <param name="path">オブジェクトパス。<c>null</c>の場合は新しいクラスが作成されます。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	/// <returns></returns>
	public void GetObjectAsync(
		WbemAsyncObjectSink sink,
		string? path,
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

	/// <summary>
	/// 新しいクラスを作成または既存クラスを更新します。
	/// </summary>
	/// <param name="classObj">クラスオブジェクト。参照カウントは変更されません。</param>
	/// <param name="createOrUpdate">作成か更新か。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="ownerUpdate">クラスの変更を通知するか。プッシュプロバイダーで使用します。</param>
	/// <param name="updateMode">更新モード。</param>
	public void PutClass(
		WbemClassObject classObj,
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
			(IWbemClassObject)classObj.WrappedObject!,
			flags, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	/// <summary>
	/// 新しいクラスを非同期で作成または既存クラスを非同期で更新します。
	/// </summary>
	/// <param name="classObj">クラスオブジェクト。参照カウントは変更されません。</param>
	/// <param name="createOrUpdate">作成か更新か。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="ownerUpdate">クラスの変更を通知するか。プッシュプロバイダーで使用します。</param>
	/// <param name="updateMode">更新モード。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	public void PutClassAsync(
		WbemAsyncObjectSink sink,
		WbemClassObject classObj,
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
			(IWbemClassObject)classObj.WrappedObject!,
			flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// クラスを削除します。
	/// </summary>
	/// <param name="path">オブジェクトパス。</param>
	/// <param name="ownerUpdate">クラスの変更を通知するか。プッシュプロバイダーで使用します。</param>
	public void DeleteClass(
		string path,
		bool ownerUpdate = false)
	{
		ThrowIfDisposed();

		var flags = (int)(ownerUpdate ? WbemFlags.WBEM_FLAG_OWNER_UPDATE : 0);
		Marshal.ThrowExceptionForHR(_obj.DeleteClass(path, flags, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	/// <summary>
	/// クラスを非同期で削除します。
	/// </summary>
	/// <param name="objectPath">オブジェクトパス。</param>
	/// <param name="ownerUpdate">クラスの変更を通知するか。プッシュプロバイダーで使用します。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
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

	/// <summary>
	/// クラスの列挙可能オブジェクトを取得します。
	/// </summary>
	/// <param name="superclass">上位クラス。省略可能です。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="returnsImmediately">半同期か。</param>
	/// <param name="timeout">待機時間。省略時は無限に待ちます。</param>
	/// <param name="authnLevel">権限レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="impLevel">偽装レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="authnCaps">権限キャパシティ。省略時は作成時の設定を使用します。</param>
	/// <returns></returns>
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

	/// <summary>
	/// クラスの列挙可能オブジェクトを非同期で取得します。
	/// </summary>
	/// <param name="superclass">上位クラス。省略可能です。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	/// <returns></returns>
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

	/// <summary>
	/// 新しいインスタンスを作成または既存のインスタンスを更新します。
	/// </summary>
	/// <param name="instance">インスタンス。</param>
	/// <param name="createOrUpdate">作成か更新か。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
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

	/// <summary>
	/// 新しいインスタンスを非同期で作成または既存のインスタンスを非同期で更新します。
	/// </summary>
	/// <param name="instance">インスタンス。</param>
	/// <param name="createOrUpdate">作成か更新か。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
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

	/// <summary>
	/// インスタンスを削除します。
	/// </summary>
	/// <param name="path">オブジェクトパス。</param>
	public void DeleteInstance(string path)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.DeleteInstance(path, 0, null, out Unsafe.NullRef<IWbemCallResult?>()));
	}

	/// <summary>
	/// インスタンスを非同期で削除します。
	/// </summary>
	/// <param name="path">オブジェクトパス。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	public void DeleteInstanceAsync(
		string path,
		WbemAsyncObjectSink sink,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.DeleteInstanceAsync(path, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// インスタンスの列挙可能オブジェクトを取得します。
	/// </summary>
	/// <param name="className">クラス名。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <param name="returnsImmediately">半同期か。</param>
	/// <param name="timeout">待機時間。省略時は無限に待ちます。</param>
	/// <param name="authnLevel">権限レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="impLevel">偽装レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="authnCaps">権限キャパシティ。省略時は作成時の設定を使用します。</param>
	/// <returns></returns>
	public WbemClassObjectEnumerable GetInstanceEnumerable(
		string className,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool directRead = false,
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
			| (int)(directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateInstanceEnum(className, flags, null, out var x));
		return new(x, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}


	/// <summary>
	/// インスタンスの列挙可能オブジェクトを非同期で取得します。
	/// </summary>
	/// <param name="className">クラス名。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	public void GetInstanceEnumerableAsync(
		WbemAsyncObjectSink sink,
		string className,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool directRead = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();

		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.CreateInstanceEnumAsync(className, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// WQLクエリを実行します。
	/// </summary>
	/// <param name="query">WQLクエリ。</param>
	/// <param name="ensuresLocatable"><c>__PATH</c>、<c>__RELPATH、</c>、<c>__SERVER</c>等のシステムプロパティを有効化するか。</param>
	/// <param name="prototype">プロトタイプを作成するか。真の場合、クエリは実行せず結果オブジェクト様のオブジェクトを返します。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <param name="returnsImmediately">半同期か。</param>
	/// <param name="timeout">待機時間。省略時は無限に待ちます。</param>
	/// <param name="authnLevel">権限レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="impLevel">偽装レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="authnCaps">権限キャパシティ。省略時は作成時の設定を使用します。</param>
	public WbemClassObjectEnumerable ExecQuery(
		string query,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool directRead = false,
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
			| (int)(directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(ensuresLocatable ? WbemFlags.WBEM_FLAG_ENSURE_LOCATABLE : 0)
			| (int)(prototype ? WbemFlags.WBEM_FLAG_PROTOTYPE : 0)
			| (int)(returnsImmediately ? WbemFlags.WBEM_FLAG_RETURN_IMMEDIATELY : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecQuery("WQL", query, flags, null, out var x));
		return new(x!, timeout ?? WbemMillisecond.Infinite,
			authnLevel ?? AuthenticationLevel,
			impLevel ?? ImpersonateLevel,
			authnCaps ?? AuthenticationCapabilities);
	}

	/// <summary>
	/// WQLクエリを非同期で実行します。
	/// </summary>
	/// <param name="query">WQLクエリ。</param>
	/// <param name="ensuresLocatable"><c>__PATH</c>、<c>__RELPATH、</c>、<c>__SERVER</c>等のシステムプロパティを有効化するか。</param>
	/// <param name="prototype">プロトタイプを作成するか。真の場合、クエリは実行せず結果オブジェクト様のオブジェクトを返します。</param>
	/// <param name="usesAmendedQualifiers">ローカライズされた名前空間の修飾子を使用するか。</param>
	/// <param name="deepOrShallow">含めるクラスの種類。</param>
	/// <param name="direction">列挙可能方向。</param>
	/// <param name="directRead">指定クラスのプロバイダーに直接アクセスするか。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	public void ExecQueryAsync(
		WbemAsyncObjectSink sink,
		string query,
		bool usesAmendedQualifiers = false,
		DeepOrShallow deepOrShallow = DeepOrShallow.Deep,
		Direction direction = Direction.Forward,
		bool directRead = false,
		bool ensuresLocatable = true,
		bool prototype = false,
		bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(usesAmendedQualifiers ? WbemFlags.WBEM_FLAG_USE_AMENDED_QUALIFIERS : 0)
			| (int)deepOrShallow
			| (int)direction
			| (int)(directRead ? WbemFlags.WBEM_FLAG_DIRECT_READ : 0)
			| (int)(ensuresLocatable ? WbemFlags.WBEM_FLAG_ENSURE_LOCATABLE : 0)
			| (int)(prototype ? WbemFlags.WBEM_FLAG_PROTOTYPE : 0)
			| (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecQueryAsync("WQL", query, flags, null, (IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// WQLクエリを受信用に実行します。
	/// </summary>
	/// <param name="query">WQLクエリ。</param>
	/// <param name="bidirectional">列挙可能オブジェクトの方向。</param>
	/// <param name="returnsImmediately">半同期か。</param>
	/// <param name="timeout">待機時間。省略時は無限に待ちます。</param>
	/// <param name="authnLevel">権限レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="impLevel">偽装レベル。省略時は作成時の設定を使用します。</param>
	/// <param name="authnCaps">権限キャパシティ。省略時は作成時の設定を使用します。</param>
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

	/// <summary>
	/// WQLクエリを受信用に非同期で実行します。
	/// </summary>
	/// <param name="query">WQLクエリ。</param>
	/// <param name="bidirectional">列挙可能オブジェクトの方向。</param>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
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

	/// <summary>
	/// メソッドを実行します。
	/// </summary>
	/// <param name="path">オブジェクトパス。</param>
	/// <param name="methodName">メソッド名。</param>
	/// <param name="inParams">入力パラメーター。</param>
	/// <returns>出力パラメーター。</returns>
	public WbemClassObject ExecMethod(string path, string methodName, WbemClassObject? inParams)
	{
		ThrowIfDisposed();
		Marshal.ThrowExceptionForHR(_obj.ExecMethod(path, methodName, 0, null,
			inParams?.WrappedObject as IWbemClassObject, out var x, out Unsafe.NullRef<IWbemCallResult?>()));
		return new(x!);
	}

	/// <summary>
	/// メソッドを非同期で実行します。
	/// </summary>
	/// <param name="path">オブジェクトパス。</param>
	/// <param name="methodName">メソッド名。</param>
	/// <param name="inParams">入力パラメーター。</param>
	/// <returns>出力パラメーター。</returns>
	/// <param name="sink">非同期処理シンク。</param>
	/// <param name="sendsStatus">ステータス変更を通知するか。</param>
	public void ExecMethodAsync(WbemAsyncObjectSink sink, string path, string methodName, WbemClassObject? inParams, bool sendsStatus = true)
	{
		ThrowIfDisposed();
		var flags = (int)(sendsStatus ? WbemFlags.WBEM_FLAG_SEND_STATUS : 0);
		Marshal.ThrowExceptionForHR(_obj.ExecMethodAsync(path, methodName, flags, null,
			inParams?.WrappedObject as IWbemClassObject, (IWbemObjectSink)sink.WrappedObject!));
	}

	/// <summary>
	/// クラスまたはインスタンスの探索深度。
	/// </summary>
	public enum DeepOrShallow : uint
	{
		/// <summary>
		/// 純粋なインスタンスのみ含めます。
		/// </summary>
		Shallow = WbemFlags.WBEM_FLAG_SHALLOW,
		/// <summary>
		/// 階層内の全てのサブクラスを含めます。
		/// </summary>
		Deep = WbemFlags.WBEM_FLAG_DEEP,
	}

	/// <summary>
	/// 列挙可能オブジェクトの方向。
	/// </summary>
	public enum Direction : uint
	{
		/// <summary>
		/// 前向きのみ。メモリ消費が抑制されます。
		/// </summary>
		Forward = WbemFlags.WBEM_FLAG_FORWARD_ONLY,
		/// <summary>
		/// 両方向。メモリ消費が増加します。
		/// </summary>
		Bidirectional = WbemFlags.WBEM_FLAG_BIDIRECTIONAL,
	}

	/// <summary>
	/// 作成または更新。
	/// </summary>
	public enum CreateOrUpdate : uint
	{
		/// <summary>
		/// 作成または更新します。
		/// </summary>
		Both = WbemFlags.WBEM_FLAG_CREATE_OR_UPDATE,
		/// <summary>
		/// 作成します。
		/// </summary>
		CreateOnly = WbemFlags.WBEM_FLAG_CREATE_ONLY,
		/// <summary>
		/// 更新します。
		/// </summary>
		UpdateOnly = WbemFlags.WBEM_FLAG_UPDATE_ONLY,
	}

	/// <summary>
	/// 更新方法。
	/// </summary>
	public enum UpdateMode : uint
	{
		/// <summary>
		/// 派生クラスやインスタンスがない場合のみ更新します。
		/// 非インポート修飾子に限定される場合はどの場合も更新されます。
		/// </summary>
		Compatible = WbemFlags.WBEM_FLAG_UPDATE_COMPATIBLE,
		/// <summary>
		/// 変更が子クラスと競合しない場合のみ更新します。
		/// </summary>
		SafeMode = WbemFlags.WBEM_FLAG_UPDATE_SAFE_MODE,
		/// <summary>
		/// 変更が子クラスと競合する場合も更新します。
		/// </summary>
		Force = WbemFlags.WBEM_FLAG_UPDATE_FORCE_MODE,
	}
}
