namespace VNA_Data_Grabber;

partial class Form1
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

    private void InitializeComponent()
    {
        label1 = new Label();
        txtIPAddress = new TextBox();
        label2 = new Label();
        txtOrderNo = new TextBox();
        label3 = new Label();
        txtTraceList = new TextBox();
        btnReadData = new Button();
        btnSave = new Button();
        txtDisplay = new RichTextBox(); // 改為 RichTextBox
        statusStrip1 = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        label4 = new Label();
        txtMarkerCount = new TextBox();
        statusStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 15);
        label1.Name = "label1";
        label1.Size = new Size(71, 15);
        label1.TabIndex = 0;
        label1.Text = "儀器 IP 位址:";
        // 
        // txtIPAddress
        // 
        txtIPAddress.Location = new Point(89, 12);
        txtIPAddress.Name = "txtIPAddress";
        txtIPAddress.Size = new Size(100, 23);
        txtIPAddress.TabIndex = 1;
        txtIPAddress.Text = "192.168.1.100";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(200, 15);
        label2.Name = "label2";
        label2.Size = new Size(58, 15);
        label2.TabIndex = 2;
        label2.Text = "工單號碼:";
        // 
        // txtOrderNo
        // 
        txtOrderNo.Location = new Point(264, 12);
        txtOrderNo.Name = "txtOrderNo";
        txtOrderNo.Size = new Size(90, 23);
        txtOrderNo.TabIndex = 3;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(365, 15);
        label3.Name = "label3";
        label3.Size = new Size(95, 15);
        label3.TabIndex = 4;
        label3.Text = "Trace (如 1,2):";
        // 
        // txtTraceList
        // 
        txtTraceList.Location = new Point(466, 12);
        txtTraceList.Name = "txtTraceList";
        txtTraceList.Size = new Size(60, 23);
        txtTraceList.TabIndex = 5;
        txtTraceList.Text = "1";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(535, 15);
        label4.Name = "label4";
        label4.Size = new Size(60, 15);
        label4.TabIndex = 10;
        label4.Text = "Marker 數:";
        // 
        // txtMarkerCount
        // 
        txtMarkerCount.Location = new Point(601, 12);
        txtMarkerCount.Name = "txtMarkerCount";
        txtMarkerCount.Size = new Size(30, 23);
        txtMarkerCount.TabIndex = 11;
        txtMarkerCount.Text = "5";
        // 
        // btnReadData
        // 
        btnReadData.Location = new Point(642, 11);
        btnReadData.Name = "btnReadData";
        btnReadData.Size = new Size(85, 25);
        btnReadData.TabIndex = 6;
        btnReadData.Text = "讀取資料";
        btnReadData.UseVisualStyleBackColor = true;
        btnReadData.Click += btnReadData_Click;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(733, 11);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(70, 25);
        btnSave.TabIndex = 7;
        btnSave.Text = "儲存";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // txtDisplay
        // 
        txtDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtDisplay.Location = new Point(12, 45);
        txtDisplay.Name = "txtDisplay";
        txtDisplay.ReadOnly = true;
        txtDisplay.ScrollBars = RichTextBoxScrollBars.Both; // 改為 RichTextBox 屬性
        txtDisplay.Size = new Size(791, 360);
        txtDisplay.TabIndex = 8;
        txtDisplay.Text = "";
        // 
        // statusStrip1
        // 
        statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip1.Location = new Point(0, 418);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(815, 22);
        statusStrip1.TabIndex = 9;
        statusStrip1.Text = "statusStrip1";
        // 
        // lblStatus
        // 
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(71, 17);
        lblStatus.Text = "狀態: 待機中";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(815, 440);
        Controls.Add(txtMarkerCount);
        Controls.Add(label4);
        Controls.Add(statusStrip1);
        Controls.Add(txtDisplay);
        Controls.Add(btnSave);
        Controls.Add(btnReadData);
        Controls.Add(txtTraceList);
        Controls.Add(label3);
        Controls.Add(txtOrderNo);
        Controls.Add(label2);
        Controls.Add(txtIPAddress);
        Controls.Add(label1);
        MinimumSize = new Size(830, 480);
        Name = "Form1";
        Text = "VNA Data Grabber - 多 Trace 讀取版";
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtIPAddress;
    private Label label2;
    private TextBox txtOrderNo;
    private Label label3;
    private TextBox txtTraceList;
    private Button btnReadData;
    private Button btnSave;
    private RichTextBox txtDisplay; // 改為 RichTextBox
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel lblStatus;
    private Label label4;
    private TextBox txtMarkerCount;
}
