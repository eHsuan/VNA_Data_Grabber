using System.Drawing;
using System.Windows.Forms;

namespace VNA_Data_Grabber
{
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTraceList = new System.Windows.Forms.TextBox();
            this.btnReadData = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtDisplay = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMarkerCount = new System.Windows.Forms.TextBox();
            this.grpMES = new System.Windows.Forms.GroupBox();
            this.lblMesUser = new System.Windows.Forms.Label();
            this.btnMesLogout = new System.Windows.Forms.Button();
            this.btnMesLogin = new System.Windows.Forms.Button();
            this.txtMesUserId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMesTrackOut = new System.Windows.Forms.Button();
            this.btnMesEdcUpload = new System.Windows.Forms.Button();
            this.btnMesTrackIn = new System.Windows.Forms.Button();
            this.btnMesMeasurePlan = new System.Windows.Forms.Button();
            this.btnMesQuery = new System.Windows.Forms.Button();
            this.grpOrderInfo = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtCheckInUser = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtWipQty = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtMatNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtRefInputQty = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtWorkCenterName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtWorkCenterNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOrderNo = new System.Windows.Forms.TextBox();
            this.grpManagement = new System.Windows.Forms.GroupBox();
            this.dgvManagement = new System.Windows.Forms.DataGridView();
            this.colInputCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            this.grpMES.SuspendLayout();
            this.grpOrderInfo.SuspendLayout();
            this.grpManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvManagement)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 315);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "儀器 IP 位址:";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(89, 312);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(100, 22);
            this.txtIPAddress.TabIndex = 1;
            this.txtIPAddress.Text = "192.168.1.100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(200, 315);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Trace (如 1,2):";
            // 
            // txtTraceList
            // 
            this.txtTraceList.Location = new System.Drawing.Point(284, 312);
            this.txtTraceList.Name = "txtTraceList";
            this.txtTraceList.Size = new System.Drawing.Size(100, 22);
            this.txtTraceList.TabIndex = 5;
            this.txtTraceList.Text = "1";
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(543, 311);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(120, 25);
            this.btnReadData.TabIndex = 6;
            this.btnReadData.Text = "讀取儀器並上傳";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(669, 311);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(134, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "手動存 Excel";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtDisplay
            // 
            this.txtDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisplay.BackColor = System.Drawing.Color.White;
            this.txtDisplay.Location = new System.Drawing.Point(12, 345);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.ReadOnly = true;
            this.txtDisplay.Size = new System.Drawing.Size(791, 110);
            this.txtDisplay.TabIndex = 8;
            this.txtDisplay.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 463);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(815, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(73, 17);
            this.lblStatus.Text = "狀態: 待機中";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(400, 315);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "Marker 數:";
            // 
            // txtMarkerCount
            // 
            this.txtMarkerCount.Location = new System.Drawing.Point(463, 312);
            this.txtMarkerCount.Name = "txtMarkerCount";
            this.txtMarkerCount.Size = new System.Drawing.Size(40, 22);
            this.txtMarkerCount.TabIndex = 11;
            this.txtMarkerCount.Text = "3";
            // 
            // grpMES
            // 
            this.grpMES.Controls.Add(this.lblMesUser);
            this.grpMES.Controls.Add(this.btnMesLogout);
            this.grpMES.Controls.Add(this.btnMesLogin);
            this.grpMES.Controls.Add(this.txtMesUserId);
            this.grpMES.Controls.Add(this.label5);
            this.grpMES.Controls.Add(this.btnMesTrackOut);
            this.grpMES.Controls.Add(this.btnMesEdcUpload);
            this.grpMES.Controls.Add(this.btnMesTrackIn);
            this.grpMES.Controls.Add(this.btnMesMeasurePlan);
            this.grpMES.Controls.Add(this.btnMesQuery);
            this.grpMES.Location = new System.Drawing.Point(12, 12);
            this.grpMES.Name = "grpMES";
            this.grpMES.Size = new System.Drawing.Size(791, 110);
            this.grpMES.TabIndex = 12;
            this.grpMES.TabStop = false;
            this.grpMES.Text = "MES 整合功能";
            // 
            // lblMesUser
            // 
            this.lblMesUser.AutoSize = true;
            this.lblMesUser.ForeColor = System.Drawing.Color.Blue;
            this.lblMesUser.Location = new System.Drawing.Point(10, 52);
            this.lblMesUser.Name = "lblMesUser";
            this.lblMesUser.Size = new System.Drawing.Size(83, 12);
            this.lblMesUser.TabIndex = 9;
            this.lblMesUser.Text = "登入狀態: 離線";
            // 
            // btnMesLogout
            // 
            this.btnMesLogout.Enabled = false;
            this.btnMesLogout.Location = new System.Drawing.Point(365, 18);
            this.btnMesLogout.Name = "btnMesLogout";
            this.btnMesLogout.Size = new System.Drawing.Size(60, 25);
            this.btnMesLogout.TabIndex = 8;
            this.btnMesLogout.Text = "登出";
            this.btnMesLogout.UseVisualStyleBackColor = true;
            this.btnMesLogout.Click += new System.EventHandler(this.btnMesLogout_Click);
            // 
            // btnMesLogin
            // 
            this.btnMesLogin.Location = new System.Drawing.Point(300, 18);
            this.btnMesLogin.Name = "btnMesLogin";
            this.btnMesLogin.Size = new System.Drawing.Size(60, 25);
            this.btnMesLogin.TabIndex = 7;
            this.btnMesLogin.Text = "登入";
            this.btnMesLogin.UseVisualStyleBackColor = true;
            this.btnMesLogin.Click += new System.EventHandler(this.btnMesLogin_Click);
            // 
            // txtMesUserId
            // 
            this.txtMesUserId.Location = new System.Drawing.Point(110, 20);
            this.txtMesUserId.Name = "txtMesUserId";
            this.txtMesUserId.Size = new System.Drawing.Size(180, 22);
            this.txtMesUserId.TabIndex = 4;
            this.txtMesUserId.Text = "{{{{791869{A1{{{984ebe3d5f244fc19554c83e416959cb";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "刷入人員二維碼:";
            // 
            // btnMesTrackOut
            // 
            this.btnMesTrackOut.Enabled = false;
            this.btnMesTrackOut.Location = new System.Drawing.Point(705, 18);
            this.btnMesTrackOut.Name = "btnMesTrackOut";
            this.btnMesTrackOut.Size = new System.Drawing.Size(75, 25);
            this.btnMesTrackOut.TabIndex = 2;
            this.btnMesTrackOut.Text = "3. 工單出站";
            this.btnMesTrackOut.UseVisualStyleBackColor = true;
            this.btnMesTrackOut.Click += new System.EventHandler(this.btnMesTrackOut_Click);
            // 
            // btnMesEdcUpload
            // 
            this.btnMesEdcUpload.Enabled = false;
            this.btnMesEdcUpload.Location = new System.Drawing.Point(543, 45);
            this.btnMesEdcUpload.Name = "btnMesEdcUpload";
            this.btnMesEdcUpload.Size = new System.Drawing.Size(120, 25);
            this.btnMesEdcUpload.TabIndex = 11;
            this.btnMesEdcUpload.Text = "2.1 資料上傳 (EDC)";
            this.btnMesEdcUpload.UseVisualStyleBackColor = true;
            this.btnMesEdcUpload.Click += new System.EventHandler(this.btnMesEdcUpload_Click);
            // 
            // btnMesTrackIn
            // 
            this.btnMesTrackIn.Enabled = false;
            this.btnMesTrackIn.Location = new System.Drawing.Point(624, 18);
            this.btnMesTrackIn.Name = "btnMesTrackIn";
            this.btnMesTrackIn.Size = new System.Drawing.Size(75, 25);
            this.btnMesTrackIn.TabIndex = 1;
            this.btnMesTrackIn.Text = "2. 工單進站";
            this.btnMesTrackIn.UseVisualStyleBackColor = true;
            this.btnMesTrackIn.Click += new System.EventHandler(this.btnMesTrackIn_Click);
            // 
            // btnMesMeasurePlan
            // 
            this.btnMesMeasurePlan.Enabled = false;
            this.btnMesMeasurePlan.Location = new System.Drawing.Point(435, 18);
            this.btnMesMeasurePlan.Name = "btnMesMeasurePlan";
            this.btnMesMeasurePlan.Size = new System.Drawing.Size(100, 25);
            this.btnMesMeasurePlan.TabIndex = 10;
            this.btnMesMeasurePlan.Text = "量測計畫查詢";
            this.btnMesMeasurePlan.UseVisualStyleBackColor = true;
            this.btnMesMeasurePlan.Click += new System.EventHandler(this.btnMesMeasurePlan_Click);
            // 
            // btnMesQuery
            // 
            this.btnMesQuery.Enabled = false;
            this.btnMesQuery.Location = new System.Drawing.Point(543, 18);
            this.btnMesQuery.Name = "btnMesQuery";
            this.btnMesQuery.Size = new System.Drawing.Size(75, 25);
            this.btnMesQuery.TabIndex = 0;
            this.btnMesQuery.Text = "1. 工單查詢";
            this.btnMesQuery.UseVisualStyleBackColor = true;
            this.btnMesQuery.Click += new System.EventHandler(this.btnMesQuery_Click);
            // 
            // grpOrderInfo
            // 
            this.grpOrderInfo.Controls.Add(this.label12);
            this.grpOrderInfo.Controls.Add(this.txtCheckInUser);
            this.grpOrderInfo.Controls.Add(this.label11);
            this.grpOrderInfo.Controls.Add(this.txtWipQty);
            this.grpOrderInfo.Controls.Add(this.label10);
            this.grpOrderInfo.Controls.Add(this.txtMatNo);
            this.grpOrderInfo.Controls.Add(this.label9);
            this.grpOrderInfo.Controls.Add(this.txtRefInputQty);
            this.grpOrderInfo.Controls.Add(this.label8);
            this.grpOrderInfo.Controls.Add(this.txtWorkCenterName);
            this.grpOrderInfo.Controls.Add(this.label7);
            this.grpOrderInfo.Controls.Add(this.txtWorkCenterNo);
            this.grpOrderInfo.Controls.Add(this.label2);
            this.grpOrderInfo.Controls.Add(this.txtOrderNo);
            this.grpOrderInfo.Location = new System.Drawing.Point(12, 93);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(791, 85);
            this.grpOrderInfo.TabIndex = 13;
            this.grpOrderInfo.TabStop = false;
            this.grpOrderInfo.Text = "工單資訊";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(540, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "進站人員:";
            // 
            // txtCheckInUser
            // 
            this.txtCheckInUser.BackColor = System.Drawing.SystemColors.Control;
            this.txtCheckInUser.Location = new System.Drawing.Point(602, 52);
            this.txtCheckInUser.Name = "txtCheckInUser";
            this.txtCheckInUser.ReadOnly = true;
            this.txtCheckInUser.Size = new System.Drawing.Size(170, 22);
            this.txtCheckInUser.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(360, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "工單數量:";
            // 
            // txtWipQty
            // 
            this.txtWipQty.BackColor = System.Drawing.SystemColors.Control;
            this.txtWipQty.Location = new System.Drawing.Point(422, 52);
            this.txtWipQty.Name = "txtWipQty";
            this.txtWipQty.ReadOnly = true;
            this.txtWipQty.Size = new System.Drawing.Size(100, 22);
            this.txtWipQty.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "料號:";
            // 
            // txtMatNo
            // 
            this.txtMatNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtMatNo.Location = new System.Drawing.Point(74, 52);
            this.txtMatNo.Name = "txtMatNo";
            this.txtMatNo.ReadOnly = true;
            this.txtMatNo.Size = new System.Drawing.Size(260, 22);
            this.txtMatNo.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(540, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "投入量:";
            // 
            // txtRefInputQty
            // 
            this.txtRefInputQty.BackColor = System.Drawing.SystemColors.Control;
            this.txtRefInputQty.Location = new System.Drawing.Point(602, 22);
            this.txtRefInputQty.Name = "txtRefInputQty";
            this.txtRefInputQty.ReadOnly = true;
            this.txtRefInputQty.Size = new System.Drawing.Size(170, 22);
            this.txtRefInputQty.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(360, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "製程名稱:";
            // 
            // txtWorkCenterName
            // 
            this.txtWorkCenterName.BackColor = System.Drawing.SystemColors.Control;
            this.txtWorkCenterName.Location = new System.Drawing.Point(422, 22);
            this.txtWorkCenterName.Name = "txtWorkCenterName";
            this.txtWorkCenterName.ReadOnly = true;
            this.txtWorkCenterName.Size = new System.Drawing.Size(100, 22);
            this.txtWorkCenterName.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(180, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "製程編號:";
            // 
            // txtWorkCenterNo
            // 
            this.txtWorkCenterNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtWorkCenterNo.Location = new System.Drawing.Point(242, 22);
            this.txtWorkCenterNo.Name = "txtWorkCenterNo";
            this.txtWorkCenterNo.ReadOnly = true;
            this.txtWorkCenterNo.Size = new System.Drawing.Size(92, 22);
            this.txtWorkCenterNo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "工單號碼:";
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.Location = new System.Drawing.Point(74, 22);
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.Size = new System.Drawing.Size(90, 22);
            this.txtOrderNo.TabIndex = 1;
            this.txtOrderNo.Text = "2F26429FCTZ1";
            // 
            // grpManagement
            // 
            this.grpManagement.Controls.Add(this.dgvManagement);
            this.grpManagement.Location = new System.Drawing.Point(12, 185);
            this.grpManagement.Name = "grpManagement";
            this.grpManagement.Size = new System.Drawing.Size(791, 115);
            this.grpManagement.TabIndex = 14;
            this.grpManagement.TabStop = false;
            this.grpManagement.Text = "管理項目";
            // 
            // dgvManagement
            // 
            this.dgvManagement.AllowUserToAddRows = false;
            this.dgvManagement.AllowUserToDeleteRows = false;
            this.dgvManagement.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvManagement.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvManagement.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colInputCode,
            this.colItemName,
            this.colResult});
            this.dgvManagement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvManagement.Location = new System.Drawing.Point(3, 18);
            this.dgvManagement.Name = "dgvManagement";
            this.dgvManagement.ReadOnly = true;
            this.dgvManagement.RowHeadersVisible = false;
            this.dgvManagement.RowTemplate.Height = 24;
            this.dgvManagement.Size = new System.Drawing.Size(785, 94);
            this.dgvManagement.TabIndex = 0;
            // 
            // colInputCode
            // 
            this.colInputCode.FillWeight = 50F;
            this.colInputCode.HeaderText = "編號";
            this.colInputCode.Name = "colInputCode";
            this.colInputCode.ReadOnly = true;
            // 
            // colItemName
            // 
            this.colItemName.HeaderText = "管理項目";
            this.colItemName.Name = "colItemName";
            this.colItemName.ReadOnly = true;
            // 
            // colResult
            // 
            this.colResult.FillWeight = 50F;
            this.colResult.HeaderText = "結果";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 485);
            this.Controls.Add(this.grpManagement);
            this.Controls.Add(this.grpOrderInfo);
            this.Controls.Add(this.grpMES);
            this.Controls.Add(this.txtMarkerCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnReadData);
            this.Controls.Add(this.txtTraceList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(830, 520);
            this.Name = "Form1";
            this.Text = "VNA Data Grabber - Cyntec MES 整合版";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpMES.ResumeLayout(false);
            this.grpMES.PerformLayout();
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            this.grpManagement.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvManagement)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTraceList;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RichTextBox txtDisplay;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMarkerCount;
        private System.Windows.Forms.GroupBox grpMES;
        private System.Windows.Forms.Button btnMesTrackOut;
        private System.Windows.Forms.Button btnMesEdcUpload;
        private System.Windows.Forms.Button btnMesTrackIn;
        private System.Windows.Forms.Button btnMesMeasurePlan;
        private System.Windows.Forms.Button btnMesQuery;
        private System.Windows.Forms.Button btnMesLogout;
        private System.Windows.Forms.Button btnMesLogin;
        private System.Windows.Forms.TextBox txtMesUserId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMesUser;
        private System.Windows.Forms.GroupBox grpOrderInfo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCheckInUser;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtWipQty;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtMatNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtRefInputQty;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWorkCenterName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtWorkCenterNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOrderNo;
        private System.Windows.Forms.GroupBox grpManagement;
        private System.Windows.Forms.DataGridView dgvManagement;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInputCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
    }
}
