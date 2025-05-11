using System.Security.Principal;

using Potisan.Windows.Wmi;

namespace WmiView;

internal static class Program
{
	public static bool IsAdmin { get; private set; }

	[STAThread]
	static void Main()
	{
		// WMI用にCOMセキュリティを初期化します。
		WmiUtility.InitializeComSecurity();

		ApplicationConfiguration.Initialize();

		// 管理者権限の確認
		using (var identify = WindowsIdentity.GetCurrent())
		{
			var principal = new WindowsPrincipal(identify);
			IsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		ToolStripManager.Renderer = new ToolStripSystemRenderer();
		Application.Run(new MainForm());
	}
}