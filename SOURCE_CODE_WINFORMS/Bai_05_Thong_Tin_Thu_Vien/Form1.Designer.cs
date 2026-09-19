namespace ThongTinThuVien
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
            pnlHeader = new Panel();
            lblTieuDe = new Label();
            lblPhuDe = new Label();
            grpKetNoi = new GroupBox();
            btnKetNoi = new Button();
            lblTrangThai = new Label();
            grpChucNang = new GroupBox();
            btnCauA = new Button();
            btnCauB = new Button();
            btnCauC = new Button();
            btnCauD = new Button();
            btnCauE = new Button();
            btnThoat = new Button();
            pnlHeader.SuspendLayout();
            grpKetNoi.SuspendLayout();
            grpChucNang.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(30, 64, 175);
            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(lblPhuDe);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(980, 145);
            pnlHeader.TabIndex = 0;
            // 
            // lblTieuDe
            // 
            lblTieuDe.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.Location = new Point(60, 20);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(860, 60);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "HỆ THỐNG QUẢN LÝ THƯ VIỆN";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPhuDe
            // 
            lblPhuDe.Font = new Font("Segoe UI", 11F);
            lblPhuDe.ForeColor = Color.FromArgb(220, 230, 255);
            lblPhuDe.Location = new Point(120, 88);
            lblPhuDe.Name = "lblPhuDe";
            lblPhuDe.Size = new Size(740, 28);
            lblPhuDe.TabIndex = 1;
            lblPhuDe.Text = "Tra cứu và thống kê thông tin thư viện";
            lblPhuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpKetNoi
            // 
            grpKetNoi.Controls.Add(btnKetNoi);
            grpKetNoi.Controls.Add(lblTrangThai);
            grpKetNoi.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            grpKetNoi.ForeColor = Color.FromArgb(45, 55, 75);
            grpKetNoi.Location = new Point(60, 170);
            grpKetNoi.Name = "grpKetNoi";
            grpKetNoi.Size = new Size(860, 120);
            grpKetNoi.TabIndex = 1;
            grpKetNoi.TabStop = false;
            grpKetNoi.Text = "Kết nối cơ sở dữ liệu";
            // 
            // btnKetNoi
            // 
            btnKetNoi.BackColor = Color.FromArgb(37, 99, 235);
            btnKetNoi.Cursor = Cursors.Hand;
            btnKetNoi.FlatAppearance.BorderSize = 0;
            btnKetNoi.FlatStyle = FlatStyle.Flat;
            btnKetNoi.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnKetNoi.ForeColor = Color.White;
            btnKetNoi.Location = new Point(35, 42);
            btnKetNoi.Name = "btnKetNoi";
            btnKetNoi.Size = new Size(220, 48);
            btnKetNoi.TabIndex = 0;
            btnKetNoi.Text = "Kết nối CSDL";
            btnKetNoi.UseVisualStyleBackColor = false;
            btnKetNoi.Click += btnKetNoi_Click;
            // 
            // lblTrangThai
            // 
            lblTrangThai.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTrangThai.ForeColor = Color.Firebrick;
            lblTrangThai.Location = new Point(290, 44);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(520, 40);
            lblTrangThai.TabIndex = 1;
            lblTrangThai.Text = "● Chưa kết nối cơ sở dữ liệu";
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpChucNang
            // 
            grpChucNang.Controls.Add(btnCauA);
            grpChucNang.Controls.Add(btnCauB);
            grpChucNang.Controls.Add(btnCauC);
            grpChucNang.Controls.Add(btnCauD);
            grpChucNang.Controls.Add(btnCauE);
            grpChucNang.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            grpChucNang.ForeColor = Color.FromArgb(45, 55, 75);
            grpChucNang.Location = new Point(60, 315);
            grpChucNang.Name = "grpChucNang";
            grpChucNang.Size = new Size(860, 395);
            grpChucNang.TabIndex = 2;
            grpChucNang.TabStop = false;
            grpChucNang.Text = "Chức năng";
            // 
            // btnCauA
            // 
            btnCauA.BackColor = Color.FromArgb(16, 185, 129);
            btnCauA.Cursor = Cursors.Hand;
            btnCauA.FlatAppearance.BorderSize = 0;
            btnCauA.FlatStyle = FlatStyle.Flat;
            btnCauA.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCauA.ForeColor = Color.White;
            btnCauA.Location = new Point(260, 47);
            btnCauA.Name = "btnCauA";
            btnCauA.Size = new Size(350, 52);
            btnCauA.TabIndex = 0;
            btnCauA.Text = "a. Thông tin độc giả";
            btnCauA.UseVisualStyleBackColor = false;
            btnCauA.Click += btnCauA_Click;
            // 
            // btnCauB
            // 
            btnCauB.BackColor = Color.FromArgb(14, 165, 233);
            btnCauB.Cursor = Cursors.Hand;
            btnCauB.FlatAppearance.BorderSize = 0;
            btnCauB.FlatStyle = FlatStyle.Flat;
            btnCauB.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCauB.ForeColor = Color.White;
            btnCauB.Location = new Point(260, 105);
            btnCauB.Name = "btnCauB";
            btnCauB.Size = new Size(350, 52);
            btnCauB.TabIndex = 1;
            btnCauB.Text = "b. Thông tin đầu sách";
            btnCauB.UseVisualStyleBackColor = false;
            btnCauB.Click += btnCauB_Click;
            // 
            // btnCauC
            // 
            btnCauC.BackColor = Color.FromArgb(99, 102, 241);
            btnCauC.Cursor = Cursors.Hand;
            btnCauC.FlatAppearance.BorderSize = 0;
            btnCauC.FlatStyle = FlatStyle.Flat;
            btnCauC.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCauC.ForeColor = Color.White;
            btnCauC.Location = new Point(260, 163);
            btnCauC.Name = "btnCauC";
            btnCauC.Size = new Size(350, 52);
            btnCauC.TabIndex = 2;
            btnCauC.Text = "c. Người lớn đang mượn sách";
            btnCauC.UseVisualStyleBackColor = false;
            btnCauC.Click += btnCauC_Click;
            // 
            // btnCauD
            // 
            btnCauD.BackColor = Color.FromArgb(245, 158, 11);
            btnCauD.Cursor = Cursors.Hand;
            btnCauD.FlatAppearance.BorderSize = 0;
            btnCauD.FlatStyle = FlatStyle.Flat;
            btnCauD.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCauD.ForeColor = Color.White;
            btnCauD.Location = new Point(260, 221);
            btnCauD.Name = "btnCauD";
            btnCauD.Size = new Size(350, 52);
            btnCauD.TabIndex = 3;
            btnCauD.Text = "d. Người lớn mượn quá hạn";
            btnCauD.UseVisualStyleBackColor = false;
            btnCauD.Click += btnCauD_Click;
            // 
            // btnCauE
            // 
            btnCauE.BackColor = Color.FromArgb(219, 39, 119);
            btnCauE.Cursor = Cursors.Hand;
            btnCauE.FlatAppearance.BorderSize = 0;
            btnCauE.FlatStyle = FlatStyle.Flat;
            btnCauE.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCauE.ForeColor = Color.White;
            btnCauE.Location = new Point(260, 279);
            btnCauE.Name = "btnCauE";
            btnCauE.Size = new Size(350, 52);
            btnCauE.TabIndex = 4;
            btnCauE.Text = "e. Người lớn có trẻ em cùng mượn";
            btnCauE.UseVisualStyleBackColor = false;
            btnCauE.Click += btnCauE_Click;
            // 
            // btnThoat
            // 
            btnThoat.BackColor = Color.FromArgb(100, 116, 139);
            btnThoat.Cursor = Cursors.Hand;
            btnThoat.FlatAppearance.BorderSize = 0;
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnThoat.ForeColor = Color.White;
            btnThoat.Location = new Point(400, 730);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(180, 48);
            btnThoat.TabIndex = 3;
            btnThoat.Text = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(980, 810);
            Controls.Add(btnThoat);
            Controls.Add(grpChucNang);
            Controls.Add(grpKetNoi);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý thư viện";
            Load += Form1_Load;
            pnlHeader.ResumeLayout(false);
            grpKetNoi.ResumeLayout(false);
            grpChucNang.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;

        private Label lblTieuDe;
        private Label lblPhuDe;

        private GroupBox grpKetNoi;
        private Button btnKetNoi;
        private Label lblTrangThai;

        private GroupBox grpChucNang;

        private Button btnCauA;
        private Button btnCauB;
        private Button btnCauC;
        private Button btnCauD;
        private Button btnCauE;

        private Button btnThoat;
    }
}