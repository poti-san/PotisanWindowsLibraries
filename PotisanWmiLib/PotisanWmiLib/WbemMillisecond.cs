namespace Potisan.Windows.Wmi;

/// <summary>
/// WMIの待機ミリ秒。
/// </summary>
/// <param name="Value"></param>
public record struct WbemMillisecond(uint Value)
{
	public static WbemMillisecond NoWait => new(0);
	public static WbemMillisecond Infinite => new(0xffffffff);

	public override readonly string? ToString()
		=> Value != 0xffffffff ? base.ToString() : "INFINITE";
}
