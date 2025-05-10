namespace WmiView;

partial class MainForm
{
	private System.ComponentModel.IContainer components = null;

	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		namespaceTreeView = new TreeView();
		label1 = new Label();
		classListView = new ListView();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		columnHeader4 = new ColumnHeader();
		panel1 = new Panel();
		flowLayoutPanel1 = new FlowLayoutPanel();
		searchSubNamespacesCheckBox = new CheckBox();
		hiddenSystemClassCheckBox = new CheckBox();
		label2 = new Label();
		namespaceClassNameFilterTextBox = new TextBox();
		hasPropOnlyCheckBox = new CheckBox();
		label3 = new Label();
		tabControl1 = new TabControl();
		tabPage1 = new TabPage();
		instanceListView = new ListView();
		tabPage2 = new TabPage();
		methodListView = new ListView();
		columnHeader3 = new ColumnHeader();
		columnHeader5 = new ColumnHeader();
		columnHeader6 = new ColumnHeader();
		label4 = new Label();
		splitContainer3 = new SplitContainer();
		splitContainer4 = new SplitContainer();
		menuStrip1 = new MenuStrip();
		toolMenuItem = new ToolStripMenuItem();
		toolCopyNamespaceNameMenuItem = new ToolStripMenuItem();
		toolCopyNamespacePathMenuItem = new ToolStripMenuItem();
		toolStripMenuItem1 = new ToolStripSeparator();
		toolSaveClassListToCsvFileMenuItem = new ToolStripMenuItem();
		toolSaveInstanceListToCsvFileMenuItem = new ToolStripMenuItem();
		toolStripMenuItem2 = new ToolStripSeparator();
		toolUpdateMenuItem = new ToolStripMenuItem();
		toolStripMenuItem3 = new ToolStripSeparator();
		toolCloseToolStripMenuItem = new ToolStripMenuItem();
		csvSaveFileDialog = new SaveFileDialog();
		panel1.SuspendLayout();
		flowLayoutPanel1.SuspendLayout();
		tabControl1.SuspendLayout();
		tabPage1.SuspendLayout();
		tabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
		splitContainer3.Panel1.SuspendLayout();
		splitContainer3.Panel2.SuspendLayout();
		splitContainer3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
		splitContainer4.Panel1.SuspendLayout();
		splitContainer4.Panel2.SuspendLayout();
		splitContainer4.SuspendLayout();
		menuStrip1.SuspendLayout();
		SuspendLayout();
		// 
		// namespaceTreeView
		// 
		namespaceTreeView.Dock = DockStyle.Fill;
		namespaceTreeView.HideSelection = false;
		namespaceTreeView.Location = new Point(0, 15);
		namespaceTreeView.Name = "namespaceTreeView";
		namespaceTreeView.Size = new Size(379, 259);
		namespaceTreeView.TabIndex = 0;
		namespaceTreeView.BeforeExpand += namespaceTreeView_BeforeExpand;
		namespaceTreeView.AfterSelect += namespaceTreeView_AfterSelect;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Dock = DockStyle.Top;
		label1.Location = new Point(0, 0);
		label1.Name = "label1";
		label1.Size = new Size(55, 15);
		label1.TabIndex = 1;
		label1.Text = "名前空間";
		// 
		// classListView
		// 
		classListView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader4 });
		classListView.Dock = DockStyle.Fill;
		classListView.FullRowSelect = true;
		classListView.GridLines = true;
		classListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
		classListView.Location = new Point(0, 44);
		classListView.Name = "classListView";
		classListView.Size = new Size(757, 230);
		classListView.TabIndex = 1;
		classListView.UseCompatibleStateImageBehavior = false;
		classListView.View = View.Details;
		classListView.VirtualMode = true;
		classListView.RetrieveVirtualItem += classListView_RetrieveVirtualItem;
		classListView.SelectedIndexChanged += classListView_SelectedIndexChanged;
		// 
		// columnHeader1
		// 
		columnHeader1.Text = "名前";
		// 
		// columnHeader2
		// 
		columnHeader2.Text = "名前空間";
		// 
		// columnHeader4
		// 
		columnHeader4.Text = "メソッド数";
		// 
		// panel1
		// 
		panel1.AutoSize = true;
		panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		panel1.Controls.Add(flowLayoutPanel1);
		panel1.Dock = DockStyle.Top;
		panel1.Location = new Point(0, 15);
		panel1.Name = "panel1";
		panel1.Size = new Size(757, 29);
		panel1.TabIndex = 2;
		// 
		// flowLayoutPanel1
		// 
		flowLayoutPanel1.AutoSize = true;
		flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		flowLayoutPanel1.Controls.Add(searchSubNamespacesCheckBox);
		flowLayoutPanel1.Controls.Add(hiddenSystemClassCheckBox);
		flowLayoutPanel1.Controls.Add(label2);
		flowLayoutPanel1.Controls.Add(namespaceClassNameFilterTextBox);
		flowLayoutPanel1.Controls.Add(hasPropOnlyCheckBox);
		flowLayoutPanel1.Dock = DockStyle.Fill;
		flowLayoutPanel1.Location = new Point(0, 0);
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		flowLayoutPanel1.Size = new Size(757, 29);
		flowLayoutPanel1.TabIndex = 1;
		flowLayoutPanel1.WrapContents = false;
		// 
		// searchSubNamespacesCheckBox
		// 
		searchSubNamespacesCheckBox.Checked = true;
		searchSubNamespacesCheckBox.CheckState = CheckState.Checked;
		searchSubNamespacesCheckBox.Location = new Point(3, 3);
		searchSubNamespacesCheckBox.Name = "searchSubNamespacesCheckBox";
		searchSubNamespacesCheckBox.Size = new Size(131, 23);
		searchSubNamespacesCheckBox.TabIndex = 0;
		searchSubNamespacesCheckBox.Text = "下位名前空間も検索";
		searchSubNamespacesCheckBox.UseVisualStyleBackColor = true;
		searchSubNamespacesCheckBox.CheckedChanged += searchSubNamespacesCheckBox_CheckedChanged;
		// 
		// hiddenSystemClassCheckBox
		// 
		hiddenSystemClassCheckBox.Checked = true;
		hiddenSystemClassCheckBox.CheckState = CheckState.Checked;
		hiddenSystemClassCheckBox.Location = new Point(140, 3);
		hiddenSystemClassCheckBox.Name = "hiddenSystemClassCheckBox";
		hiddenSystemClassCheckBox.Size = new Size(120, 23);
		hiddenSystemClassCheckBox.TabIndex = 1;
		hiddenSystemClassCheckBox.Text = "システムクラスを隠す";
		hiddenSystemClassCheckBox.UseVisualStyleBackColor = true;
		hiddenSystemClassCheckBox.CheckedChanged += hiddenSystemClassCheckBox_CheckedChanged;
		// 
		// label2
		// 
		label2.Location = new Point(266, 3);
		label2.Margin = new Padding(3);
		label2.Name = "label2";
		label2.Size = new Size(70, 23);
		label2.TabIndex = 2;
		label2.Text = "名前フィルタ:";
		label2.TextAlign = ContentAlignment.MiddleRight;
		// 
		// namespaceClassNameFilterTextBox
		// 
		namespaceClassNameFilterTextBox.Location = new Point(342, 3);
		namespaceClassNameFilterTextBox.Name = "namespaceClassNameFilterTextBox";
		namespaceClassNameFilterTextBox.Size = new Size(172, 23);
		namespaceClassNameFilterTextBox.TabIndex = 3;
		namespaceClassNameFilterTextBox.WordWrap = false;
		namespaceClassNameFilterTextBox.TextChanged += namespaceClassNameFilterTextBox_TextChanged;
		// 
		// hasPropOnlyCheckBox
		// 
		hasPropOnlyCheckBox.Location = new Point(520, 3);
		hasPropOnlyCheckBox.Name = "hasPropOnlyCheckBox";
		hasPropOnlyCheckBox.Size = new Size(109, 23);
		hasPropOnlyCheckBox.TabIndex = 4;
		hasPropOnlyCheckBox.Text = "プロパティありのみ";
		hasPropOnlyCheckBox.UseVisualStyleBackColor = true;
		hasPropOnlyCheckBox.CheckedChanged += hasPropOnlyCheckBox_CheckedChanged;
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Dock = DockStyle.Top;
		label3.Location = new Point(0, 0);
		label3.Name = "label3";
		label3.Size = new Size(57, 15);
		label3.TabIndex = 0;
		label3.Text = "所属クラス";
		// 
		// tabControl1
		// 
		tabControl1.Controls.Add(tabPage1);
		tabControl1.Controls.Add(tabPage2);
		tabControl1.Dock = DockStyle.Fill;
		tabControl1.Location = new Point(0, 15);
		tabControl1.Name = "tabControl1";
		tabControl1.SelectedIndex = 0;
		tabControl1.Size = new Size(1146, 333);
		tabControl1.TabIndex = 1;
		// 
		// tabPage1
		// 
		tabPage1.Controls.Add(instanceListView);
		tabPage1.Location = new Point(4, 24);
		tabPage1.Name = "tabPage1";
		tabPage1.Padding = new Padding(3);
		tabPage1.Size = new Size(1138, 305);
		tabPage1.TabIndex = 0;
		tabPage1.Text = "インスタンス一覧";
		tabPage1.UseVisualStyleBackColor = true;
		// 
		// instanceListView
		// 
		instanceListView.Dock = DockStyle.Fill;
		instanceListView.FullRowSelect = true;
		instanceListView.GridLines = true;
		instanceListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
		instanceListView.Location = new Point(3, 3);
		instanceListView.Name = "instanceListView";
		instanceListView.Size = new Size(1132, 299);
		instanceListView.TabIndex = 0;
		instanceListView.UseCompatibleStateImageBehavior = false;
		instanceListView.View = View.Details;
		// 
		// tabPage2
		// 
		tabPage2.Controls.Add(methodListView);
		tabPage2.Location = new Point(4, 24);
		tabPage2.Name = "tabPage2";
		tabPage2.Padding = new Padding(3);
		tabPage2.Size = new Size(1138, 305);
		tabPage2.TabIndex = 1;
		tabPage2.Text = "メソッド一覧";
		tabPage2.UseVisualStyleBackColor = true;
		// 
		// methodListView
		// 
		methodListView.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader5, columnHeader6 });
		methodListView.Dock = DockStyle.Fill;
		methodListView.FullRowSelect = true;
		methodListView.GridLines = true;
		methodListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
		methodListView.Location = new Point(3, 3);
		methodListView.Name = "methodListView";
		methodListView.Size = new Size(1132, 299);
		methodListView.TabIndex = 0;
		methodListView.UseCompatibleStateImageBehavior = false;
		methodListView.View = View.Details;
		// 
		// columnHeader3
		// 
		columnHeader3.Text = "名前";
		// 
		// columnHeader5
		// 
		columnHeader5.Text = "入力";
		// 
		// columnHeader6
		// 
		columnHeader6.Text = "出力";
		// 
		// label4
		// 
		label4.AutoSize = true;
		label4.Dock = DockStyle.Top;
		label4.Location = new Point(0, 0);
		label4.Name = "label4";
		label4.Size = new Size(81, 15);
		label4.TabIndex = 0;
		label4.Text = "選択クラス情報";
		// 
		// splitContainer3
		// 
		splitContainer3.BorderStyle = BorderStyle.FixedSingle;
		splitContainer3.Dock = DockStyle.Fill;
		splitContainer3.Location = new Point(0, 24);
		splitContainer3.Name = "splitContainer3";
		splitContainer3.Orientation = Orientation.Horizontal;
		// 
		// splitContainer3.Panel1
		// 
		splitContainer3.Panel1.Controls.Add(splitContainer4);
		// 
		// splitContainer3.Panel2
		// 
		splitContainer3.Panel2.Controls.Add(tabControl1);
		splitContainer3.Panel2.Controls.Add(label4);
		splitContainer3.Size = new Size(1148, 634);
		splitContainer3.SplitterDistance = 276;
		splitContainer3.SplitterWidth = 8;
		splitContainer3.TabIndex = 1;
		// 
		// splitContainer4
		// 
		splitContainer4.BorderStyle = BorderStyle.FixedSingle;
		splitContainer4.Dock = DockStyle.Fill;
		splitContainer4.Location = new Point(0, 0);
		splitContainer4.Name = "splitContainer4";
		// 
		// splitContainer4.Panel1
		// 
		splitContainer4.Panel1.Controls.Add(namespaceTreeView);
		splitContainer4.Panel1.Controls.Add(label1);
		// 
		// splitContainer4.Panel2
		// 
		splitContainer4.Panel2.Controls.Add(classListView);
		splitContainer4.Panel2.Controls.Add(panel1);
		splitContainer4.Panel2.Controls.Add(label3);
		splitContainer4.Size = new Size(1148, 276);
		splitContainer4.SplitterDistance = 381;
		splitContainer4.SplitterWidth = 8;
		splitContainer4.TabIndex = 0;
		// 
		// menuStrip1
		// 
		menuStrip1.Items.AddRange(new ToolStripItem[] { toolMenuItem });
		menuStrip1.Location = new Point(0, 0);
		menuStrip1.Name = "menuStrip1";
		menuStrip1.Size = new Size(1148, 24);
		menuStrip1.TabIndex = 2;
		menuStrip1.Text = "menuStrip1";
		// 
		// toolMenuItem
		// 
		toolMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolCopyNamespaceNameMenuItem, toolCopyNamespacePathMenuItem, toolStripMenuItem1, toolSaveClassListToCsvFileMenuItem, toolSaveInstanceListToCsvFileMenuItem, toolStripMenuItem2, toolUpdateMenuItem, toolStripMenuItem3, toolCloseToolStripMenuItem });
		toolMenuItem.Name = "toolMenuItem";
		toolMenuItem.Size = new Size(60, 20);
		toolMenuItem.Text = "ツール(&T)";
		// 
		// toolCopyNamespaceNameMenuItem
		// 
		toolCopyNamespaceNameMenuItem.Name = "toolCopyNamespaceNameMenuItem";
		toolCopyNamespaceNameMenuItem.Size = new Size(260, 22);
		toolCopyNamespaceNameMenuItem.Text = "名前空間の名前をコピー";
		toolCopyNamespaceNameMenuItem.Click += toolCopyNamespaceNameMenuItem_Click;
		// 
		// toolCopyNamespacePathMenuItem
		// 
		toolCopyNamespacePathMenuItem.Name = "toolCopyNamespacePathMenuItem";
		toolCopyNamespacePathMenuItem.Size = new Size(260, 22);
		toolCopyNamespacePathMenuItem.Text = "名前空間のパスをコピー";
		toolCopyNamespacePathMenuItem.Click += toolCopyNamespacePathMenuItem_Click;
		// 
		// toolStripMenuItem1
		// 
		toolStripMenuItem1.Name = "toolStripMenuItem1";
		toolStripMenuItem1.Size = new Size(257, 6);
		// 
		// toolSaveClassListToCsvFileMenuItem
		// 
		toolSaveClassListToCsvFileMenuItem.Name = "toolSaveClassListToCsvFileMenuItem";
		toolSaveClassListToCsvFileMenuItem.Size = new Size(260, 22);
		toolSaveClassListToCsvFileMenuItem.Text = "表示中の所属クラスをCSV出力...";
		toolSaveClassListToCsvFileMenuItem.Click += toolSaveClassListToCsvFileMenuItem_Click;
		// 
		// toolSaveInstanceListToCsvFileMenuItem
		// 
		toolSaveInstanceListToCsvFileMenuItem.Name = "toolSaveInstanceListToCsvFileMenuItem";
		toolSaveInstanceListToCsvFileMenuItem.Size = new Size(260, 22);
		toolSaveInstanceListToCsvFileMenuItem.Text = "表示中のインスタンス一覧をCSV出力...";
		toolSaveInstanceListToCsvFileMenuItem.Click += toolSaveInstanceListToCsvFileMenuItem_Click;
		// 
		// toolStripMenuItem2
		// 
		toolStripMenuItem2.Name = "toolStripMenuItem2";
		toolStripMenuItem2.Size = new Size(257, 6);
		// 
		// toolUpdateMenuItem
		// 
		toolUpdateMenuItem.Name = "toolUpdateMenuItem";
		toolUpdateMenuItem.ShortcutKeys = Keys.F5;
		toolUpdateMenuItem.Size = new Size(260, 22);
		toolUpdateMenuItem.Text = "最新の情報に更新(&U)";
		toolUpdateMenuItem.Click += toolUpdateMenuItem_Click;
		// 
		// toolStripMenuItem3
		// 
		toolStripMenuItem3.Name = "toolStripMenuItem3";
		toolStripMenuItem3.Size = new Size(257, 6);
		// 
		// toolCloseToolStripMenuItem
		// 
		toolCloseToolStripMenuItem.Name = "toolCloseToolStripMenuItem";
		toolCloseToolStripMenuItem.Size = new Size(260, 22);
		toolCloseToolStripMenuItem.Text = "閉じる(&X)";
		toolCloseToolStripMenuItem.Click += toolCloseToolStripMenuItem_Click;
		// 
		// csvSaveFileDialog
		// 
		csvSaveFileDialog.DefaultExt = "csv";
		csvSaveFileDialog.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
		csvSaveFileDialog.SupportMultiDottedExtensions = true;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(1148, 658);
		Controls.Add(splitContainer3);
		Controls.Add(menuStrip1);
		MainMenuStrip = menuStrip1;
		Name = "MainForm";
		StartPosition = FormStartPosition.WindowsDefaultBounds;
		Text = "WmiView";
		Load += MainForm_Load;
		panel1.ResumeLayout(false);
		panel1.PerformLayout();
		flowLayoutPanel1.ResumeLayout(false);
		flowLayoutPanel1.PerformLayout();
		tabControl1.ResumeLayout(false);
		tabPage1.ResumeLayout(false);
		tabPage2.ResumeLayout(false);
		splitContainer3.Panel1.ResumeLayout(false);
		splitContainer3.Panel2.ResumeLayout(false);
		splitContainer3.Panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
		splitContainer3.ResumeLayout(false);
		splitContainer4.Panel1.ResumeLayout(false);
		splitContainer4.Panel1.PerformLayout();
		splitContainer4.Panel2.ResumeLayout(false);
		splitContainer4.Panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
		splitContainer4.ResumeLayout(false);
		menuStrip1.ResumeLayout(false);
		menuStrip1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion
	private TreeView namespaceTreeView;
	private Label label1;
	private Label label3;
	private ListView classListView;
	private ColumnHeader columnHeader1;
	private ColumnHeader columnHeader2;
	private Panel panel1;
	private CheckBox searchSubNamespacesCheckBox;
	private FlowLayoutPanel flowLayoutPanel1;
	private CheckBox hiddenSystemClassCheckBox;
	private Label label2;
	private TextBox namespaceClassNameFilterTextBox;
	private Label label4;
	private TabControl tabControl1;
	private TabPage tabPage1;
	private ListView instanceListView;
	private SplitContainer splitContainer3;
	private SplitContainer splitContainer4;
	private MenuStrip menuStrip1;
	private ToolStripMenuItem toolMenuItem;
	private ToolStripMenuItem toolCloseToolStripMenuItem;
	private ToolStripMenuItem toolCopyNamespaceNameMenuItem;
	private ToolStripMenuItem toolCopyNamespacePathMenuItem;
	private ToolStripSeparator toolStripMenuItem1;
	private ToolStripMenuItem toolSaveInstanceListToCsvFileMenuItem;
	private ToolStripMenuItem toolSaveClassListToCsvFileMenuItem;
	private ToolStripSeparator toolStripMenuItem2;
	private SaveFileDialog csvSaveFileDialog;
	private TabPage tabPage2;
	private ListView methodListView;
	private ColumnHeader columnHeader3;
	private ColumnHeader columnHeader4;
	private CheckBox hasPropOnlyCheckBox;
	private ColumnHeader columnHeader5;
	private ColumnHeader columnHeader6;
	private ToolStripMenuItem toolUpdateMenuItem;
	private ToolStripSeparator toolStripMenuItem3;
}
