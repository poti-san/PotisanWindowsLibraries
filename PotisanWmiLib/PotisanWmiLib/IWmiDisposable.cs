namespace Potisan.Windows.Wmi;

/// <summary>
/// <see cref="IDisposable"/>実装と共通の処理インターフェイスです。
/// </summary>
/// <remarks>
/// 継承クラスにドキュメントコメントの省略を可能とします。
/// </remarks>
public interface IWmiDisposable : IDisposable
{
	/// <summary>
	/// 保持するRCWオブジェクトを取得します。
	/// </summary>
	object? WrappedObject { get; }

	/// <summary>
	/// 保持するRCWオブジェクトは解放済みか。
	/// </summary>
	bool IsDisposed { get; }

	/// <summary>
	/// 保持するRCWオブジェクトが解放済みの場合、例外を発生します。
	/// </summary>
	void ThrowIfDisposed();
}
