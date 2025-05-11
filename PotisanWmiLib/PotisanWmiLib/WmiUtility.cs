using System.ComponentModel;

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI関係の便利機能。
/// </summary>
public static class WmiUtility
{
	public static void InitializeComSecurity(
		ComAuthenticationLevel authnLevel = ComAuthenticationLevel.Default,
		ComImpersonateLevel impLevel = ComImpersonateLevel.Impersonate,
		OleAuthenticationCap authnCaps = OleAuthenticationCap.None)
	{
		[DllImport("ole32.dll")]
		static extern int CoInitializeSecurity(
			nint pSecDesc,
			int cAuthSvc,
			nint asAuthSvc,
			nint pReserved1,
			uint dwAuthnLevel,
			uint dwImpLevel,
			nint pAuthList,
			uint dwCapabilities,
			nint pReserved3);
		Marshal.ThrowExceptionForHR(CoInitializeSecurity(0, -1, 0, 0, (uint)authnLevel, (uint)impLevel, 0, (uint)authnCaps, 0));
	}

	internal static void SetProxyBlanket(object o, ComAuthenticationLevel authnLevel, ComImpersonateLevel impLevel, OleAuthenticationCap authnCaps)
	{
		[DllImport("ole32.dll")]
		static extern int CoSetProxyBlanket(
			[MarshalAs(UnmanagedType.IUnknown)] object pProxy,
			uint dwAuthnSvc,
			uint dwAuthzSvc,
			nint pServerPrincName,
			uint dwAuthnLevel,
			uint dwImpLevel,
			nint pAuthInfo,
			uint dwCapabilities);

		const uint RPC_C_AUTHN_DEFAULT = 0xffffffff;
		const uint RPC_C_AUTHZ_DEFAULT = 0xffffffff;
		const nint COLE_DEFAULT_PRINCIPAL = -1;
		const nint COLE_DEFAULT_AUTHINFO = -1;

		Marshal.ThrowExceptionForHR(CoSetProxyBlanket(
			o, RPC_C_AUTHN_DEFAULT, RPC_C_AUTHZ_DEFAULT, COLE_DEFAULT_PRINCIPAL,
			(uint)authnLevel, (uint)impLevel, COLE_DEFAULT_AUTHINFO, (uint)authnCaps));
	}
}

/// <summary>
/// 認証レベル。
/// </summary>
public enum ComAuthenticationLevel : uint
{
	/// <summary>
	/// 既定の方法で認証レベルを選択します。
	/// </summary>
	Default = 0,

	/// <summary>
	/// 認証しません。
	/// </summary>
	None = 1,

	/// <summary>
	/// サーバーとの関係を確立した場合のみクライアントの資格情報を認証します。
	/// </summary>
	Connect = 2,

	/// <summary>
	/// サーバー要求の受信後、リモートプロシージャ呼び出し開始時のみ認証します。
	/// <see cref="Connect"/>の認証も使用します。
	/// </summary>
	Call = 3,

	/// <summary>
	/// 受信データが適切なクライアントのデータか認証します。
	/// <see cref="Call"/>の認証も使用します。
	/// </summary>
	Packet = 4,

	/// <summary>
	/// 転送データが適切なクライアントのデータか認証します。
	/// </summary>
	/// <see cref="Packet"/>の認証も使用します。
	PacketIntegrity = 5,

	/// <summary>
	/// リモートプロシージャの引数を暗号化します。
	/// <see cref="PacketIntegrity"/>の認証も使用します。
	/// </summary>
	PacketPrivacy = 6,
}

/// <summary>
/// クライアント偽装時にサーバーに与えられる権限の量。
/// </summary>
public enum ComImpersonateLevel : uint
{
	/// <summary>
	/// 既定の方法で偽装レベルを選択します。
	/// </summary>
	Default = 0,

	/// <summary>
	/// サーバーに対して匿名です。
	/// サーバーはクライアントを偽装できますが、偽装トークンは使用できません。
	/// </summary>
	Anonymous = 1,

	/// <summary>
	/// サーバーはクライアントIDを取得できます。
	/// サーバーはACLチェック用にクライアントを偽装できますが、システムオブジェクトにクライアントとしてアクセスできません。
	/// </summary>
	Identify = 2,

	/// <summary>
	/// サーバーはクライアントのセキュリティコンテキストを偽装できます。
	/// 偽装トークンは1つのマシン境界のみ越えられます。
	/// </summary>
	Impersonate = 3,

	/// <summary>
	/// サーバーはクライアントのセキュリティコンテキストを偽装できます。
	/// 偽装トークンは任意のマシン境界を越えられます。
	/// </summary>
	Delegate = 4,
}

/// <summary>
/// 機能フラグ。EOLE_AUTHENTICATION_CAPABILITIES。
/// </summary>
[Flags]
public enum OleAuthenticationCap : uint
{
	/// <summary>
	/// 機能フラグなし。
	/// </summary>
	None = 0,

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("後方互換性のために残されています。")]
	MutalAuto = 0x1,

	/// <summary>
	/// 静的クローキングの設定。クライアントIDの決定時にスレッドトークンを使用します。
	/// スレッドトークンはプロキシの初回呼び出しまたは<c>CoSetProxyBlanket</c>呼び出し時に決定されます。
	/// <see cref="DynamicCloaking"/>と片方だけ指定できます。
	/// </summary>
	StaticCloaking = 0x20,

	/// <summary>
	/// 動的クローキングの設定。クライアントIDの決定時にスレッドトークンを使用します。
	/// プロキシの呼び出し毎にスレッドトークンが確認され、クライアントIDが変更されていれば再認証されます。
	/// <see cref="StaticCloaking"/>と片方だけ指定できます。
	/// </summary>
	DynamicCloaking = 0x40,
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("後方互換性のために残されています。")]
	AnyAuthority = 0x80,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// Schannelサーバープリンシパル名をfullsic形式にします。
	/// </summary>
	MakeFullsic = 0x100,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// <c>CoInitializeSecurity</c>の呼び出しで使用された<c>OleAuthenticationCap</c>を使用します。
	/// <c>IClientSecurity::SetBlanket</c>または<c>CoSetProxyBlanket</c>の呼び出しで使用されます。
	/// </summary>
	Default = 0x800,

	/// <summary>
	/// 悪意のあるユーザーによる使用中オブジェクトの解放を抑制するために分散参照カウント呼び出しを認証します。
	/// このフラグの指定時、認証レベルは0以外です。
	/// </summary>
	SecureRefs = 0x2,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// セキュリティデスクリプタは<c>IAccessControl</c>を指します。
	/// <see cref="AppID"/>とどちらかだけ指定できます。
	/// </summary>
	AccessControl = 0x4,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// セキュリティデスクリプタはAppIDのGUIDを指します。
	/// <see cref="AccessControl"/>とどちらかだけ指定できます。
	/// </summary>
	AppID = 0x8,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// システムの予約済みフラグ。
	/// </summary>
	Dynamic = 0x10,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// <c>SetProxyBlanket</c>でSchannelプリンシパル名にfullsic形式を要求します。
	/// </summary>
	RequireFullsic = 0x200,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// システムの予約済みフラグ。
	/// </summary>
	AutoInpersonate = 0x400,

	/// <summary>
	/// 呼び出し元IDによるサーバープロセス起動のアクティブ化(Activate-as-Activator)を失敗させます。
	/// 信用されていないコンポーネントによる特権アカウントで実行されるアプリケーションのID使用を抑制します。
	/// </summary>
	DisableAaa = 0x1000,

	/// <summary>
	/// CLSIDやカテゴリIDの制限によりサーバーのセキュリティを保護します。
	/// システム操作に不可欠なサービスで使用されます。
	/// </summary>
	NoCustomMarshal = 0x2000,

	[EditorBrowsable(EditorBrowsableState.Never)]
	/// <summary>
	/// システムの予約済みフラグ。
	/// </summary>
	Reserved1 = 0x4000
}
