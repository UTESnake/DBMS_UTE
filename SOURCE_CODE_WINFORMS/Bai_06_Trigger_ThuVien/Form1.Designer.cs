using System.Drawing;
using System.Windows.Forms;

namespace Bai_06_Trigger_ThuVien
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;

        private Label lblTieuDe;
        private Label lblPhuDe;

        private GroupBox grpKetNoi;

        private Button btnKetNoi;
        private Label lblTrangThai;

        private GroupBox grpChucNang;

        private Button btnCau61;
        private Button btnCau62;
        private Button btnCau63;
        private Button btnCau64;

        private Button btnThoat;


        protected override void Dispose(bool disposing)
        {
            if (disposing &&
                components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            components =
                new System.ComponentModel.Container();

            pnlHeader = new Panel();

            lblTieuDe = new Label();
            lblPhuDe = new Label();

            grpKetNoi = new GroupBox();

            btnKetNoi = new Button();
            lblTrangThai = new Label();

            grpChucNang = new GroupBox();

            btnCau61 = new Button();
            btnCau62 = new Button();
            btnCau63 = new Button();
            btnCau64 = new Button();

            btnThoat = new Button();


            pnlHeader.SuspendLayout();
            grpKetNoi.SuspendLayout();
            grpChucNang.SuspendLayout();

            SuspendLayout();


            // =====================================================
            // HEADER
            // =====================================================
            pnlHeader.BackColor =
                Color.FromArgb(30, 64, 175);

            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(lblPhuDe);

            pnlHeader.Dock =
                DockStyle.Top;

            pnlHeader.Location =
                new Point(0, 0);

            pnlHeader.Name =
                "pnlHeader";

            pnlHeader.Size =
                new Size(980, 145);


            // =====================================================
            // TIÊU ĐỀ
            // =====================================================
            lblTieuDe.Font =
                new Font(
                    "Segoe UI",
                    24F,
                    FontStyle.Bold
                );

            lblTieuDe.ForeColor =
                Color.White;

            lblTieuDe.Location =
                new Point(60, 20);

            lblTieuDe.Name =
                "lblTieuDe";

            lblTieuDe.Size =
                new Size(860, 60);

            lblTieuDe.Text =
                "HỆ THỐNG QUẢN LÝ THƯ VIỆN";

            lblTieuDe.TextAlign =
                ContentAlignment.MiddleCenter;


            // =====================================================
            // PHỤ ĐỀ
            // =====================================================
            lblPhuDe.Font =
                new Font(
                    "Segoe UI",
                    11F
                );

            lblPhuDe.ForeColor =
                Color.FromArgb(220, 230, 255);

            lblPhuDe.Location =
                new Point(120, 88);

            lblPhuDe.Name =
                "lblPhuDe";

            lblPhuDe.Size =
                new Size(740, 28);

            lblPhuDe.Text =
                "Kiểm tra Trigger trong cơ sở dữ liệu thư viện";

            lblPhuDe.TextAlign =
                ContentAlignment.MiddleCenter;


            // =====================================================
            // GROUP KẾT NỐI
            // =====================================================
            grpKetNoi.Controls.Add(btnKetNoi);
            grpKetNoi.Controls.Add(lblTrangThai);

            grpKetNoi.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            grpKetNoi.ForeColor =
                Color.FromArgb(45, 55, 75);

            grpKetNoi.Location =
                new Point(60, 170);

            grpKetNoi.Name =
                "grpKetNoi";

            grpKetNoi.Size =
                new Size(860, 120);

            grpKetNoi.TabStop =
                false;

            grpKetNoi.Text =
                "Kết nối cơ sở dữ liệu";


            // =====================================================
            // BUTTON KẾT NỐI
            // =====================================================
            btnKetNoi.BackColor =
                Color.FromArgb(37, 99, 235);

            btnKetNoi.Cursor =
                Cursors.Hand;

            btnKetNoi.FlatAppearance.BorderSize =
                0;

            btnKetNoi.FlatStyle =
                FlatStyle.Flat;

            btnKetNoi.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnKetNoi.ForeColor =
                Color.White;

            btnKetNoi.Location =
                new Point(35, 42);

            btnKetNoi.Name =
                "btnKetNoi";

            btnKetNoi.Size =
                new Size(220, 48);

            btnKetNoi.Text =
                "Kết nối CSDL";

            btnKetNoi.UseVisualStyleBackColor =
                false;

            btnKetNoi.Click +=
                btnKetNoi_Click;


            // =====================================================
            // TRẠNG THÁI
            // =====================================================
            lblTrangThai.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            lblTrangThai.ForeColor =
                Color.Firebrick;

            lblTrangThai.Location =
                new Point(290, 44);

            lblTrangThai.Name =
                "lblTrangThai";

            lblTrangThai.Size =
                new Size(520, 40);

            lblTrangThai.Text =
                "● Chưa kết nối cơ sở dữ liệu";

            lblTrangThai.TextAlign =
                ContentAlignment.MiddleLeft;


            // =====================================================
            // GROUP CHỨC NĂNG
            // =====================================================
            grpChucNang.Controls.Add(btnCau61);
            grpChucNang.Controls.Add(btnCau62);
            grpChucNang.Controls.Add(btnCau63);
            grpChucNang.Controls.Add(btnCau64);

            grpChucNang.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            grpChucNang.ForeColor =
                Color.FromArgb(45, 55, 75);

            grpChucNang.Location =
                new Point(60, 315);

            grpChucNang.Name =
                "grpChucNang";

            grpChucNang.Size =
                new Size(860, 330);

            grpChucNang.TabStop =
                false;

            grpChucNang.Text =
                "Chức năng";


            // =====================================================
            // 6.1
            // =====================================================
            btnCau61.BackColor =
                Color.FromArgb(16, 185, 129);

            btnCau61.Cursor =
                Cursors.Hand;

            btnCau61.FlatAppearance.BorderSize =
                0;

            btnCau61.FlatStyle =
                FlatStyle.Flat;

            btnCau61.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnCau61.ForeColor =
                Color.White;

            btnCau61.Location =
                new Point(230, 48);

            btnCau61.Name =
                "btnCau61";

            btnCau61.Size =
                new Size(400, 52);

            btnCau61.Text =
                "6.1 - Trigger tg_delMuon";

            btnCau61.UseVisualStyleBackColor =
                false;

            btnCau61.Click +=
                btnCau61_Click;


            // =====================================================
            // 6.2
            // =====================================================
            btnCau62.BackColor =
                Color.FromArgb(14, 165, 233);

            btnCau62.Cursor =
                Cursors.Hand;

            btnCau62.FlatAppearance.BorderSize =
                0;

            btnCau62.FlatStyle =
                FlatStyle.Flat;

            btnCau62.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnCau62.ForeColor =
                Color.White;

            btnCau62.Location =
                new Point(230, 110);

            btnCau62.Name =
                "btnCau62";

            btnCau62.Size =
                new Size(400, 52);

            btnCau62.Text =
                "6.2 - Trigger tg_insMuon";

            btnCau62.UseVisualStyleBackColor =
                false;

            btnCau62.Click +=
                btnCau62_Click;


            // =====================================================
            // 6.3
            // =====================================================
            btnCau63.BackColor =
                Color.FromArgb(99, 102, 241);

            btnCau63.Cursor =
                Cursors.Hand;

            btnCau63.FlatAppearance.BorderSize =
                0;

            btnCau63.FlatStyle =
                FlatStyle.Flat;

            btnCau63.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnCau63.ForeColor =
                Color.White;

            btnCau63.Location =
                new Point(230, 172);

            btnCau63.Name =
                "btnCau63";

            btnCau63.Size =
                new Size(400, 52);

            btnCau63.Text =
                "6.3 - Trigger tg_updCuonSach";

            btnCau63.UseVisualStyleBackColor =
                false;

            btnCau63.Click +=
                btnCau63_Click;


            // =====================================================
            // 6.4
            // =====================================================
            btnCau64.BackColor =
                Color.FromArgb(219, 39, 119);

            btnCau64.Cursor =
                Cursors.Hand;

            btnCau64.FlatAppearance.BorderSize =
                0;

            btnCau64.FlatStyle =
                FlatStyle.Flat;

            btnCau64.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnCau64.ForeColor =
                Color.White;

            btnCau64.Location =
                new Point(230, 234);

            btnCau64.Name =
                "btnCau64";

            btnCau64.Size =
                new Size(400, 52);

            btnCau64.Text =
                "6.4 - Trigger tg_InfThongBao";

            btnCau64.UseVisualStyleBackColor =
                false;

            btnCau64.Click +=
                btnCau64_Click;


            // =====================================================
            // THOÁT
            // =====================================================
            btnThoat.BackColor =
                Color.FromArgb(100, 116, 139);

            btnThoat.Cursor =
                Cursors.Hand;

            btnThoat.FlatAppearance.BorderSize =
                0;

            btnThoat.FlatStyle =
                FlatStyle.Flat;

            btnThoat.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            btnThoat.ForeColor =
                Color.White;

            btnThoat.Location =
                new Point(400, 675);

            btnThoat.Name =
                "btnThoat";

            btnThoat.Size =
                new Size(180, 48);

            btnThoat.Text =
                "Thoát";

            btnThoat.UseVisualStyleBackColor =
                false;

            btnThoat.Click +=
                btnThoat_Click;


            // =====================================================
            // FORM1
            // =====================================================
            AutoScaleDimensions =
                new SizeF(8F, 20F);

            AutoScaleMode =
                AutoScaleMode.Font;

            BackColor =
                Color.FromArgb(248, 250, 252);

            ClientSize =
                new Size(980, 755);

            Controls.Add(btnThoat);
            Controls.Add(grpChucNang);
            Controls.Add(grpKetNoi);
            Controls.Add(pnlHeader);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox =
                false;

            Name =
                "Form1";

            StartPosition =
                FormStartPosition.CenterScreen;

            Text =
                "Quản lý thư viện - Bài 6";

            Load +=
                Form1_Load;


            pnlHeader.ResumeLayout(false);

            grpKetNoi.ResumeLayout(false);

            grpChucNang.ResumeLayout(false);

            ResumeLayout(false);
        }
    }
}