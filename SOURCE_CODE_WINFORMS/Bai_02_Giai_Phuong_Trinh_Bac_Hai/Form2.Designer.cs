namespace Bài_2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            grpInput = new GroupBox();
            lblA = new Label();
            txtA = new TextBox();
            lblB = new Label();
            txtB = new TextBox();
            lblC = new Label();
            txtC = new TextBox();
            btnGiai = new Button();
            btnLamMoi = new Button();
            btnLoadTestcase = new Button();
            btnChayTestcase = new Button();
            lblThongKe = new Label();
            dgvTestcase = new DataGridView();
            grpOutput = new GroupBox();
            txtKetQua = new TextBox();
            pnlHeader.SuspendLayout();
            grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTestcase).BeginInit();
            grpOutput.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(0, 122, 204);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1050, 85);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(30, 24);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(605, 35);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "GIẢI PHƯƠNG TRÌNH BẬC HAI (ax² + bx + c = 0)";
            // 
            // grpInput
            // 
            grpInput.Controls.Add(lblA);
            grpInput.Controls.Add(txtA);
            grpInput.Controls.Add(lblB);
            grpInput.Controls.Add(txtB);
            grpInput.Controls.Add(lblC);
            grpInput.Controls.Add(txtC);
            grpInput.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold);
            grpInput.Location = new Point(35, 105);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(980, 175);
            grpInput.TabIndex = 1;
            grpInput.TabStop = false;
            grpInput.Text = "Nhập tham số";
            // 
            // lblA
            // 
            lblA.AutoSize = true;
            lblA.Font = new Font("Segoe UI", 10F);
            lblA.Location = new Point(60, 35);
            lblA.Name = "lblA";
            lblA.Size = new Size(71, 23);
            lblA.TabIndex = 0;
            lblA.Text = "Hệ số a:";
            // 
            // txtA
            // 
            txtA.Font = new Font("Segoe UI", 11F);
            txtA.Location = new Point(150, 31);
            txtA.Name = "txtA";
            txtA.PlaceholderText = "Nhập số a...";
            txtA.Size = new Size(760, 32);
            txtA.TabIndex = 1;
            // 
            // lblB
            // 
            lblB.AutoSize = true;
            lblB.Font = new Font("Segoe UI", 10F);
            lblB.Location = new Point(60, 81);
            lblB.Name = "lblB";
            lblB.Size = new Size(72, 23);
            lblB.TabIndex = 2;
            lblB.Text = "Hệ số b:";
            // 
            // txtB
            // 
            txtB.Font = new Font("Segoe UI", 11F);
            txtB.Location = new Point(150, 77);
            txtB.Name = "txtB";
            txtB.PlaceholderText = "Nhập số b...";
            txtB.Size = new Size(760, 32);
            txtB.TabIndex = 3;
            // 
            // lblC
            // 
            lblC.AutoSize = true;
            lblC.Font = new Font("Segoe UI", 10F);
            lblC.Location = new Point(60, 127);
            lblC.Name = "lblC";
            lblC.Size = new Size(70, 23);
            lblC.TabIndex = 4;
            lblC.Text = "Hệ số c:";
            // 
            // txtC
            // 
            txtC.Font = new Font("Segoe UI", 11F);
            txtC.Location = new Point(150, 123);
            txtC.Name = "txtC";
            txtC.PlaceholderText = "Nhập số c...";
            txtC.Size = new Size(760, 32);
            txtC.TabIndex = 5;
            // 
            // btnGiai
            // 
            btnGiai.BackColor = Color.FromArgb(40, 167, 69);
            btnGiai.Cursor = Cursors.Hand;
            btnGiai.FlatAppearance.BorderSize = 0;
            btnGiai.FlatStyle = FlatStyle.Flat;
            btnGiai.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnGiai.ForeColor = Color.White;
            btnGiai.Location = new Point(175, 295);
            btnGiai.Name = "btnGiai";
            btnGiai.Size = new Size(220, 45);
            btnGiai.TabIndex = 2;
            btnGiai.Text = "⚡ Giải Phương Trình";
            btnGiai.UseVisualStyleBackColor = false;
            btnGiai.Click += btnGiai_Click;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BackColor = Color.FromArgb(108, 117, 125);
            btnLamMoi.Cursor = Cursors.Hand;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.Location = new Point(410, 295);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Size = new Size(160, 45);
            btnLamMoi.TabIndex = 3;
            btnLamMoi.Text = "🔄 Làm Mới";
            btnLamMoi.UseVisualStyleBackColor = false;
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // btnLoadTestcase
            // 
            btnLoadTestcase.BackColor = Color.FromArgb(0, 123, 255);
            btnLoadTestcase.Cursor = Cursors.Hand;
            btnLoadTestcase.FlatAppearance.BorderSize = 0;
            btnLoadTestcase.FlatStyle = FlatStyle.Flat;
            btnLoadTestcase.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnLoadTestcase.ForeColor = Color.White;
            btnLoadTestcase.Location = new Point(585, 295);
            btnLoadTestcase.Name = "btnLoadTestcase";
            btnLoadTestcase.Size = new Size(190, 45);
            btnLoadTestcase.TabIndex = 4;
            btnLoadTestcase.Text = "📋 Load Testcase";
            btnLoadTestcase.UseVisualStyleBackColor = false;
            btnLoadTestcase.Click += btnLoadTestcase_Click;
            // 
            // btnChayTestcase
            // 
            btnChayTestcase.BackColor = Color.FromArgb(255, 153, 0);
            btnChayTestcase.Cursor = Cursors.Hand;
            btnChayTestcase.FlatAppearance.BorderSize = 0;
            btnChayTestcase.FlatStyle = FlatStyle.Flat;
            btnChayTestcase.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.Location = new Point(790, 295);
            btnChayTestcase.Name = "btnChayTestcase";
            btnChayTestcase.Size = new Size(200, 45);
            btnChayTestcase.TabIndex = 5;
            btnChayTestcase.Text = "🧪 Chạy Testcase";
            btnChayTestcase.UseVisualStyleBackColor = false;
            btnChayTestcase.Click += btnChayTestcase_Click;
            // 
            // lblThongKe
            // 
            lblThongKe.AutoSize = true;
            lblThongKe.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblThongKe.ForeColor = Color.FromArgb(64, 64, 64);
            lblThongKe.Location = new Point(35, 360);
            lblThongKe.Name = "lblThongKe";
            lblThongKe.Size = new Size(168, 23);
            lblThongKe.TabIndex = 6;
            lblThongKe.Text = "Chưa load testcase";
            // 
            // dgvTestcase
            // 
            dgvTestcase.AllowUserToAddRows = false;
            dgvTestcase.AllowUserToDeleteRows = false;
            dgvTestcase.AllowUserToResizeRows = false;
            dgvTestcase.BackgroundColor = Color.White;
            dgvTestcase.BorderStyle = BorderStyle.Fixed3D;
            dgvTestcase.ColumnHeadersHeight = 40;
            dgvTestcase.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTestcase.Location = new Point(35, 395);
            dgvTestcase.MultiSelect = false;
            dgvTestcase.Name = "dgvTestcase";
            dgvTestcase.ReadOnly = true;
            dgvTestcase.RowHeadersVisible = false;
            dgvTestcase.RowHeadersWidth = 51;
            dgvTestcase.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTestcase.Size = new Size(980, 270);
            dgvTestcase.TabIndex = 7;
            // 
            // grpOutput
            // 
            grpOutput.Controls.Add(txtKetQua);
            grpOutput.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold);
            grpOutput.Location = new Point(35, 680);
            grpOutput.Name = "grpOutput";
            grpOutput.Size = new Size(980, 100);
            grpOutput.TabIndex = 8;
            grpOutput.TabStop = false;
            grpOutput.Text = "Kết quả thực thi từ SQL Server";
            // 
            // txtKetQua
            // 
            txtKetQua.BackColor = Color.FromArgb(248, 249, 250);
            txtKetQua.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            txtKetQua.ForeColor = Color.FromArgb(192, 0, 0);
            txtKetQua.Location = new Point(20, 40);
            txtKetQua.Name = "txtKetQua";
            txtKetQua.ReadOnly = true;
            txtKetQua.Size = new Size(940, 32);
            txtKetQua.TabIndex = 0;
            txtKetQua.TextAlign = HorizontalAlignment.Center;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1050, 820);
            Controls.Add(grpOutput);
            Controls.Add(dgvTestcase);
            Controls.Add(lblThongKe);
            Controls.Add(btnChayTestcase);
            Controls.Add(btnLoadTestcase);
            Controls.Add(btnLamMoi);
            Controls.Add(btnGiai);
            Controls.Add(grpInput);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 2 - Giải phương trình bậc hai";
            Load += Form1_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTestcase).EndInit();
            grpOutput.ResumeLayout(false);
            grpOutput.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitle;
        private GroupBox grpInput;
        private Label lblA;
        private TextBox txtA;
        private Label lblB;
        private TextBox txtB;
        private Label lblC;
        private TextBox txtC;
        private Button btnGiai;
        private Button btnLamMoi;
        private Button btnLoadTestcase;
        private Button btnChayTestcase;
        private Label lblThongKe;
        private DataGridView dgvTestcase;
        private GroupBox grpOutput;
        private TextBox txtKetQua;
    }
}
