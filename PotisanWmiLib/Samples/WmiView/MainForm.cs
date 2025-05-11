using System.Diagnostics;
using System.Text;

using Potisan.Windows.Wmi;

namespace WmiView;

internal partial class MainForm : Form
{
	private sealed record class NamespaceClassInfo(string ClassName, string NameSpace, string Path, int MethodCount);

	private readonly WbemLocator _wmiLocator = new();
	private readonly List<NamespaceClassInfo> _nsClassInfos = [];
	private readonly List<NamespaceClassInfo> _nsClassInfosDisplay = [];

	public MainForm()
	{
		InitializeComponent();
	}

	private void AddNodes(TreeNode node, int depth = 0)
	{
		try
		{
			foreach (var ns in _wmiLocator.ConnectServer(node.FullPath).GetInstanceEnumerable("__NAMESPACE"))
			{
				var subNode = node.Nodes.Add(ns.Name);
				if (depth != 0)
				{
					AddNodes(subNode, depth > 0 ? depth - 1 : depth);
				}
				else
				{
					try
					{
						using var subNamespace = _wmiLocator.ConnectServer(subNode.FullPath).GetInstanceEnumerable("__NAMESPACE");
						if (subNamespace.Any())
						{
							subNode.Nodes.Add(new TreeNode() { Text = "dummy", Tag = "dummy" });
						}
					}
					catch (Exception ex)
					{
						subNode.ForeColor = Color.Gray;
						Debug.WriteLine(ex.Message);
					}
				}
			}
			node.Expand();
		}
		catch (Exception ex)
		{
			node.ForeColor = Color.Gray;
			Debug.WriteLine(ex.Message);
		}
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		const string msg = """
			管理者権限で実行しない場合、動作が遅くなり、取得できる情報が減少します。
			本当に実行しますか？
			""";
		if (!Program.IsAdmin && MessageBox.Show(msg, "WmiView", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			Close();
			return;
		}

		UpdateNamespaceClassInfos();
	}

	private void namespaceTreeView_AfterSelect(object sender, TreeViewEventArgs e)
	{
		UpdateNamespaceClassListView();
	}

	private void namespaceTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
	{
		if (e.Node is not { } node || node.Nodes.Count != 1 || node.FirstNode!.Tag is not "dummy")
			return;

		UseWaitCursor = true;
		node.Nodes.Clear();
		AddNodes(node);
		UseWaitCursor = false;
	}

	private void searchSubNamespacesCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		// クラス情報の再取得
		UpdateNamespaceClassInfos();
	}

	private void hiddenSystemClassCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		// 表示情報のみ更新
		UpdateNamespaceClassListView();
	}

	private void hasPropOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		// 表示情報のみ更新
		UpdateNamespaceClassListView();
	}

	private void UpdateNamespaceClassInfos()
	{
		UseWaitCursor = true;
		Application.DoEvents();

		void SearchNodes(TreeNode node)
		{
			try
			{
				using var ns = _wmiLocator.ConnectServer(node.FullPath);
				foreach (var classInfo in ns.GetClassEnumerable())
				{
					_nsClassInfos.Add(new(
						classInfo.ClassName != null ? string.Intern(classInfo.ClassName) : "",
						classInfo.Namespace != null ? string.Intern(classInfo.Namespace) : "",
						classInfo.Path ?? "",
						classInfo.AllMethodNames.Length));
				}
			}
			catch
			{
				// TODO LOGGING
			}

			try
			{
				foreach (TreeNode subNode in node.Nodes)
				{
					if (subNode.Tag is not "dummy")
						SearchNodes(subNode);
				}
			}
			catch
			{
				// TODO LOGGING
			}
		}

		// 古い情報の消去（インスタンス一覧とメソッド一覧）
		classListView.SelectedIndices.Clear();

		namespaceTreeView.BeginUpdate();
		namespaceTreeView.Nodes.Clear();
		_nsClassInfos.Clear();
		var root = namespaceTreeView.Nodes.Add("ROOT");

		// 名前空間ツリービューの更新
		if (Program.IsAdmin)
		{
			AddNodes(root, -1);
		}
		else
		{
			// ROOT/CIMV2/SECURITY以下は管理者権限がないと止まるので、
			// ノードを展開しません。
			AddNodes(root, 1);
		}
		SearchNodes(root);

		namespaceTreeView.EndUpdate();

		namespaceTreeView.SelectedNode = root;

		UseWaitCursor = false;
	}

	private void UpdateNamespaceClassListView()
	{
		var classNameFilter = namespaceClassNameFilterTextBox.Text;
		var namespaceName = namespaceTreeView.SelectedNode!.FullPath;
		var hiddenSystemClass = hiddenSystemClassCheckBox.Checked;
		var hasPropsOnly = hasPropOnlyCheckBox.Checked;

		_nsClassInfosDisplay.Clear();
		_nsClassInfosDisplay.AddRange(_nsClassInfos
			.Where(info => info.NameSpace.StartsWith(namespaceName, StringComparison.OrdinalIgnoreCase)
				&& (!hiddenSystemClass || !info.ClassName.StartsWith("__"))
				&& (!hasPropsOnly || info.MethodCount != 0)
				&& (classNameFilter == null || info.ClassName.Contains(classNameFilter, StringComparison.OrdinalIgnoreCase))));

		// 降順で並べ替え。
		_nsClassInfosDisplay.Sort((a, b) =>
		{
			var i = a.ClassName.CompareTo(b.ClassName);
			return i != 0 ? i : a.NameSpace.CompareTo(b.NameSpace);
		});

		classListView.VirtualListSize = _nsClassInfosDisplay.Count;
		classListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		UseWaitCursor = false;
	}

	private void classListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
	{
		if (e.ItemIndex == -1) return;

		var info = _nsClassInfosDisplay[e.ItemIndex];
		var item = new ListViewItem([info.ClassName, info.NameSpace, info.MethodCount.ToString()]);

		e.Item = item;
	}

	private void namespaceClassNameFilterTextBox_TextChanged(object sender, EventArgs e)
	{
		UpdateNamespaceClassListView();
	}

	private void classListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (classListView.SelectedIndices.Count != 1)
		{
			instanceListView.Items.Clear();
			instanceListView.Columns.Clear(); // 列も都度作成
			methodListView.Items.Clear();
			tabPage1.Text = tabPage1.Tag as string ?? tabPage1.Text;
			tabPage2.Text = tabPage2.Tag as string ?? tabPage2.Text;
			return;
		}

		UseWaitCursor = true;
		Application.DoEvents();

		var classInfo = _nsClassInfosDisplay[classListView.SelectedIndices[0]];

		instanceListView.BeginUpdate();
		instanceListView.Items.Clear();
		instanceListView.Columns.Clear();

		using var ns = _wmiLocator.ConnectServer(classInfo.NameSpace);

		using var class_ = ns.GetObject(classInfo.Path);
		// インスタンス一覧の作成
		{
			// 列の作成
			var propNames = class_.AllPropertyNames;
			instanceListView.Columns.AddRange([.. propNames.Select(s => new ColumnHeader { Text = s })]);

			// インスタンス情報の割り当て
			try
			{
				using var instances = ns.GetInstanceEnumerable(classInfo.ClassName, timeout: new(5000));
				foreach (var (i, instance) in instances.Index())
				{
					if (i == 1000)
					{
						if (TaskDialog.ShowDialog(this, new()
						{
							Text = "検索結果が1000件を越えました。検索を継続しますか？（以降は最後まで検索します）",
							Icon = TaskDialogIcon.Information,
							Buttons = [TaskDialogButton.Yes, TaskDialogButton.No],
						}
						) != TaskDialogButton.Yes)
						{
							break;
						}
					}

					instanceListView.Items.Add(new ListViewItem([.. propNames
						.Select(s => instance[s].Value switch
						{
							IEnumerable<object> enum_ => string.Join(", ", enum_),
							{ } v => v.ToString() ?? "",
							_ => "",
						})]));
				}
			}
			catch
			{
				// インスタンスが存在しない場合等
			}

			instanceListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			instanceListView.EndUpdate();

			// タブに個数の追加
			tabPage1.Tag ??= tabPage1.Text;
			tabPage1.Text = $"{tabPage1.Tag} ({instanceListView.Items.Count})";
		}
		// メソッド一覧の作成
		{
			methodListView.BeginUpdate();
			methodListView.Items.Clear();
			foreach (var (name, inSig, outSig) in class_.EnumerateMethodInfos())
			{
				methodListView.Items.Add(new ListViewItem([
					name,
					inSig?.NonSystemPropertyNames is { } inArgs ? string.Join(", ", inArgs) : "",
					outSig?.NonSystemPropertyNames is { } outArgs ? string.Join(", ", outArgs) : "",
				]));
			}
			methodListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			methodListView.EndUpdate();

			// タブに個数の追加
			tabPage2.Tag ??= tabPage2.Text;
			tabPage2.Text = $"{tabPage2.Tag} ({methodListView.Items.Count})";
		}

		UseWaitCursor = false;
	}

	private void toolCloseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void toolUpdateMenuItem_Click(object sender, EventArgs e)
	{
		UpdateNamespaceClassInfos();
	}

	private void toolCopyNamespaceNameMenuItem_Click(object sender, EventArgs e)
	{
		if (namespaceTreeView.SelectedNode is not { } node)
			return;
		Clipboard.Clear();
		Clipboard.SetText(node.Text);
	}

	private void toolCopyNamespacePathMenuItem_Click(object sender, EventArgs e)
	{
		if (namespaceTreeView.SelectedNode is not { } node)
			return;
		Clipboard.Clear();
		Clipboard.SetText(node.FullPath);
	}

	private void toolSaveClassListToCsvFileMenuItem_Click(object sender, EventArgs e)
	{
		if (namespaceTreeView.SelectedNode is not { } node)
		{
			TaskDialog.ShowDialog(this, new() { Text = "名前空間が未選択です。", Icon = TaskDialogIcon.Error });
			return;
		}
		csvSaveFileDialog.FileName = $"{node.FullPath.Replace('\\', '_')}.csv";

		if (csvSaveFileDialog.ShowDialog(this) != DialogResult.OK)
			return;
		var path = csvSaveFileDialog.FileName;

		using var f = File.OpenWrite(path);
		var writer = new StreamWriter(f, Encoding.UTF8);
		writer.WriteLine("名前,名前空間,メソッド数");
		foreach (var item in _nsClassInfosDisplay)
		{
			writer.WriteLine($"{item.ClassName},{item.NameSpace}");
		}
		f.Flush(true);
	}

	private void toolSaveInstanceListToCsvFileMenuItem_Click(object sender, EventArgs e)
	{
		// リストビューの列と項目をそのまま出力します。
		if (classListView.SelectedIndices.Count != 1)
		{
			TaskDialog.ShowDialog(this, new() { Text = "クラスが未選択か複数選択されています。", Icon = TaskDialogIcon.Error });
			return;
		}
		var classInfo = _nsClassInfosDisplay[classListView.SelectedIndices[0]];

		csvSaveFileDialog.FileName = $"{classInfo.ClassName} ({classInfo.NameSpace.Replace('\\', '_')}).csv";
		if (csvSaveFileDialog.ShowDialog(this) != DialogResult.OK)
			return;
		var path = csvSaveFileDialog.FileName;

		using var f = File.OpenWrite(path);
		var writer = new StreamWriter(f, Encoding.UTF8);

		// 列名の書き出し
		// タイプミス確認のためにOfTypeではなくCastを使います。
		writer.WriteLine(string.Join(",",
			instanceListView.Columns.Cast<ColumnHeader>().Select(column => $"\"{column.Text}\"")));

		// アイテムの書き出し
		foreach (ListViewItem item in instanceListView.Items)
		{
			var s = string.Join(",", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(item => $"\"{item.Text.Replace("\"", "\"\"")}\""));
			writer.WriteLine(s);
		}

		f.Flush(true);
	}
}
