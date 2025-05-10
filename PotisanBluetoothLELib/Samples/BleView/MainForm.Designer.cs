namespace BleView;

partial class MainForm
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
		splitContainer1 = new SplitContainer();
		DeviceListView = new ListView();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		columnHeader3 = new ColumnHeader();
		columnHeader4 = new ColumnHeader();
		DeviceInfoTreeView = new TreeView();
		statusStrip1 = new StatusStrip();
		statusMessageLbel = new ToolStripStatusLabel();
		((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
		splitContainer1.Panel1.SuspendLayout();
		splitContainer1.Panel2.SuspendLayout();
		splitContainer1.SuspendLayout();
		statusStrip1.SuspendLayout();
		SuspendLayout();
		// 
		// splitContainer1
		// 
		splitContainer1.Dock = DockStyle.Fill;
		splitContainer1.Location = new Point(0, 0);
		splitContainer1.Name = "splitContainer1";
		splitContainer1.Orientation = Orientation.Horizontal;
		// 
		// splitContainer1.Panel1
		// 
		splitContainer1.Panel1.Controls.Add(DeviceListView);
		// 
		// splitContainer1.Panel2
		// 
		splitContainer1.Panel2.Controls.Add(DeviceInfoTreeView);
		splitContainer1.Size = new Size(800, 428);
		splitContainer1.SplitterDistance = 132;
		splitContainer1.SplitterWidth = 8;
		splitContainer1.TabIndex = 0;
		// 
		// DeviceListView
		// 
		DeviceListView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
		DeviceListView.Dock = DockStyle.Fill;
		DeviceListView.FullRowSelect = true;
		DeviceListView.GridLines = true;
		DeviceListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
		DeviceListView.Location = new Point(0, 0);
		DeviceListView.Name = "DeviceListView";
		DeviceListView.Size = new Size(800, 132);
		DeviceListView.TabIndex = 0;
		DeviceListView.UseCompatibleStateImageBehavior = false;
		DeviceListView.View = View.Details;
		DeviceListView.SelectedIndexChanged += DeviceListView_SelectedIndexChanged;
		// 
		// columnHeader1
		// 
		columnHeader1.Text = "種類";
		// 
		// columnHeader2
		// 
		columnHeader2.Text = "GUID";
		// 
		// columnHeader3
		// 
		columnHeader3.Text = "デバイスクラス";
		// 
		// columnHeader4
		// 
		columnHeader4.Text = "パス";
		// 
		// DeviceInfoTreeView
		// 
		DeviceInfoTreeView.Dock = DockStyle.Fill;
		DeviceInfoTreeView.HideSelection = false;
		DeviceInfoTreeView.Location = new Point(0, 0);
		DeviceInfoTreeView.Name = "DeviceInfoTreeView";
		DeviceInfoTreeView.Size = new Size(800, 288);
		DeviceInfoTreeView.TabIndex = 0;
		// 
		// statusStrip1
		// 
		statusStrip1.Items.AddRange(new ToolStripItem[] { statusMessageLbel });
		statusStrip1.Location = new Point(0, 428);
		statusStrip1.Name = "statusStrip1";
		statusStrip1.Size = new Size(800, 22);
		statusStrip1.TabIndex = 1;
		statusStrip1.Text = "statusStrip1";
		// 
		// statusMessageLbel
		// 
		statusMessageLbel.Name = "statusMessageLbel";
		statusMessageLbel.Size = new Size(785, 17);
		statusMessageLbel.Spring = true;
		statusMessageLbel.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(splitContainer1);
		Controls.Add(statusStrip1);
		Name = "MainForm";
		Text = "BleView";
		Load += MainForm_Load;
		splitContainer1.Panel1.ResumeLayout(false);
		splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
		splitContainer1.ResumeLayout(false);
		statusStrip1.ResumeLayout(false);
		statusStrip1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private SplitContainer splitContainer1;
	private ListView DeviceListView;
	private ColumnHeader columnHeader1;
	private ColumnHeader columnHeader2;
	private ColumnHeader columnHeader3;
	private ColumnHeader columnHeader4;
	private TreeView DeviceInfoTreeView;
	private StatusStrip statusStrip1;
	private ToolStripStatusLabel statusMessageLbel;
}
