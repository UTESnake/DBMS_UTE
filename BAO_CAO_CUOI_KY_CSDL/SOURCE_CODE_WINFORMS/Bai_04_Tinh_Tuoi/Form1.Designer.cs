namespace TinhTuoi
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            lblTieuDe = new Label();
            grpTinhTuoi = new GroupBox();
            lblNamSinh = new Label();
            txtNamSinh = new TextBox();
            btnTinhTuoi = new Button();
            btnLamMoi = new Button();
            lblKetQua = new Label();
            txtKetQua = new TextBox();
            pnlTestcase = new Panel();
            btnLoadTestcase = new Button();
            btnChayTestcase = new Button();
            lblTrangThaiTestcase = new Label();
            grpTestcase = new GroupBox();
            dgvTestcase = new DataGridView();
            grpTinhTuoi.SuspendLayout();
            pnlTestcase.SuspendLayout();
            grpTestcase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTestcase).BeginInit();
            SuspendLayout();
            // 
            // lblTieuDe
            // 
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.Navy;
            lblTieuDe.Location = new Point(0, 0);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(1100, 90);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "CHƯƠNG TRÌNH TÍNH TUỔI";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpTinhTuoi
            // 
            grpTinhTuoi.Controls.Add(lblNamSinh);
            grpTinhTuoi.Controls.Add(txtNamSinh);
            grpTinhTuoi.Controls.Add(btnTinhTuoi);
            grpTinhTuoi.Controls.Add(btnLamMoi);
            grpTinhTuoi.Controls.Add(lblKetQua);
            grpTinhTuoi.Controls.Add(txtKetQua);
            grpTinhTuoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpTinhTuoi.Location = new Point(45, 95);
            grpTinhTuoi.Name = "grpTinhTuoi";
            grpTinhTuoi.Size = new Size(1010, 230);
            grpTinhTuoi.TabIndex = 1;
            grpTinhTuoi.TabStop = false;
            grpTinhTuoi.Text = "Nhập dữ liệu và tính tuổi";
            // 
            // lblNamSinh
            // 
            lblNamSinh.AutoSize = true;
            lblNamSinh.Font = new Font("Segoe UI", 11F);
            lblNamSinh.Location = new Point(95, 50);
            lblNamSinh.Name = "lblNamSinh";
            lblNamSinh.Size = new Size(144, 25);
            lblNamSinh.TabIndex = 0;
            lblNamSinh.Text = "Nhập ngày sinh:";
            // 
            // txtNamSinh
            // 
            txtNamSinh.Font = new Font("Segoe UI", 11F);
            txtNamSinh.Location = new Point(255, 45);
            txtNamSinh.Name = "txtNamSinh";
            txtNamSinh.PlaceholderText = "Ví dụ: 15/03/2005";
            txtNamSinh.Size = new Size(520, 32);
            txtNamSinh.TabIndex = 1;
            // 
            // btnTinhTuoi
            // 
            btnTinhTuoi.BackColor = Color.FromArgb(46, 184, 114);
            btnTinhTuoi.Cursor = Cursors.Hand;
            btnTinhTuoi.FlatAppearance.BorderSize = 0;
            btnTinhTuoi.FlatStyle = FlatStyle.Flat;
            btnTinhTuoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTinhTuoi.ForeColor = Color.White;
            btnTinhTuoi.Location = new Point(314, 95);
            btnTinhTuoi.Name = "btnTinhTuoi";
            btnTinhTuoi.Size = new Size(190, 48);
            btnTinhTuoi.TabIndex = 2;
            btnTinhTuoi.Text = "Tính Tuổi";
            btnTinhTuoi.UseVisualStyleBackColor = false;
            btnTinhTuoi.Click += btnTinhTuoi_Click;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BackColor = Color.FromArgb(120, 120, 120);
            btnLamMoi.Cursor = Cursors.Hand;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.Location = new Point(526, 95);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Size = new Size(190, 48);
            btnLamMoi.TabIndex = 3;
            btnLamMoi.Text = "Làm Mới";
            btnLamMoi.UseVisualStyleBackColor = false;
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // lblKetQua
            // 
            lblKetQua.AutoSize = true;
            lblKetQua.Font = new Font("Segoe UI", 11F);
            lblKetQua.Location = new Point(95, 175);
            lblKetQua.Name = "lblKetQua";
            lblKetQua.Size = new Size(80, 25);
            lblKetQua.TabIndex = 4;
            lblKetQua.Text = "Kết quả:";
            // 
            // txtKetQua
            // 
            txtKetQua.BackColor = Color.WhiteSmoke;
            txtKetQua.BorderStyle = BorderStyle.FixedSingle;
            txtKetQua.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            txtKetQua.ForeColor = Color.Firebrick;
            txtKetQua.Location = new Point(255, 170);
            txtKetQua.Name = "txtKetQua";
            txtKetQua.ReadOnly = true;
            txtKetQua.Size = new Size(520, 32);
            txtKetQua.TabIndex = 5;
            txtKetQua.TabStop = false;
            // 
            // pnlTestcase
            // 
            pnlTestcase.Controls.Add(btnLoadTestcase);
            pnlTestcase.Controls.Add(btnChayTestcase);
            pnlTestcase.Controls.Add(lblTrangThaiTestcase);
            pnlTestcase.Location = new Point(45, 340);
            pnlTestcase.Name = "pnlTestcase";
            pnlTestcase.Size = new Size(1010, 65);
            pnlTestcase.TabIndex = 2;
            // 
            // btnLoadTestcase
            // 
            btnLoadTestcase.BackColor = Color.FromArgb(0, 123, 255);
            btnLoadTestcase.Cursor = Cursors.Hand;
            btnLoadTestcase.FlatAppearance.BorderSize = 0;
            btnLoadTestcase.FlatStyle = FlatStyle.Flat;
            btnLoadTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadTestcase.ForeColor = Color.White;
            btnLoadTestcase.Location = new Point(270, 8);
            btnLoadTestcase.Name = "btnLoadTestcase";
            btnLoadTestcase.Size = new Size(190, 45);
            btnLoadTestcase.TabIndex = 0;
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
            btnChayTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.Location = new Point(479, 8);
            btnChayTestcase.Name = "btnChayTestcase";
            btnChayTestcase.Size = new Size(190, 45);
            btnChayTestcase.TabIndex = 1;
            btnChayTestcase.Text = "\U0001f9ea Chạy Testcase";
            btnChayTestcase.UseVisualStyleBackColor = false;
            btnChayTestcase.Click += btnChayTestcase_Click;
            // 
            // lblTrangThaiTestcase
            // 
            lblTrangThaiTestcase.AutoSize = true;
            lblTrangThaiTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTrangThaiTestcase.ForeColor = Color.DarkOrange;
            lblTrangThaiTestcase.Location = new Point(723, 19);
            lblTrangThaiTestcase.Name = "lblTrangThaiTestcase";
            lblTrangThaiTestcase.Size = new Size(159, 23);
            lblTrangThaiTestcase.TabIndex = 2;
            lblTrangThaiTestcase.Text = "Chưa load testcase";
            // 
            // grpTestcase
            // 
            grpTestcase.Controls.Add(dgvTestcase);
            grpTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpTestcase.Location = new Point(45, 410);
            grpTestcase.Name = "grpTestcase";
            grpTestcase.Size = new Size(1010, 390);
            grpTestcase.TabIndex = 3;
            grpTestcase.TabStop = false;
            grpTestcase.Text = "Danh sách testcase Bài 4";
            // 
            // dgvTestcase
            // 
            dgvTestcase.AllowUserToAddRows = false;
            dgvTestcase.AllowUserToDeleteRows = false;
            dgvTestcase.AllowUserToResizeRows = false;
            dgvTestcase.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTestcase.BackgroundColor = Color.White;
            dgvTestcase.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTestcase.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(235, 239, 245);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(235, 239, 245);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvTestcase.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvTestcase.ColumnHeadersHeight = 42;
            dgvTestcase.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new Padding(4);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(220, 235, 252);
            dataGridViewCellStyle4.SelectionForeColor = Color.Black;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgvTestcase.DefaultCellStyle = dataGridViewCellStyle4;
            dgvTestcase.EnableHeadersVisualStyles = false;
            dgvTestcase.GridColor = Color.FromArgb(215, 215, 215);
            dgvTestcase.Location = new Point(18, 30);
            dgvTestcase.MultiSelect = false;
            dgvTestcase.Name = "dgvTestcase";
            dgvTestcase.ReadOnly = true;
            dgvTestcase.RowHeadersVisible = false;
            dgvTestcase.RowHeadersWidth = 51;
            dgvTestcase.RowTemplate.Height = 35;
            dgvTestcase.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTestcase.Size = new Size(974, 340);
            dgvTestcase.TabIndex = 0;
            dgvTestcase.CellClick += dgvTestcase_CellClick;
            // 
            // Form1
            // 
            AcceptButton = btnTinhTuoi;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(1100, 830);
            Controls.Add(grpTestcase);
            Controls.Add(pnlTestcase);
            Controls.Add(grpTinhTuoi);
            Controls.Add(lblTieuDe);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bài 4 - Tính Tuổi";
            Load += Form1_Load;
            grpTinhTuoi.ResumeLayout(false);
            grpTinhTuoi.PerformLayout();
            pnlTestcase.ResumeLayout(false);
            pnlTestcase.PerformLayout();
            grpTestcase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTestcase).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblTieuDe;

        private GroupBox grpTinhTuoi;

        private Label lblNamSinh;
        private TextBox txtNamSinh;

        private Button btnTinhTuoi;
        private Button btnLamMoi;

        private Label lblKetQua;
        private TextBox txtKetQua;

        private Panel pnlTestcase;

        private Button btnLoadTestcase;
        private Button btnChayTestcase;

        private Label lblTrangThaiTestcase;

        private GroupBox grpTestcase;
        private DataGridView dgvTestcase;
    }
}
