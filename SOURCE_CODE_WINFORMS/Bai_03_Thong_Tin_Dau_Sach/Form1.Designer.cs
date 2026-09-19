namespace SoLuongSachChuaMuon
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
            pnlFixedTop = new Panel();
            pnlHeader = new Panel();
            lblTitle = new Label();

            grpChonDauSach = new GroupBox();

            btnKetNoi = new Button();
            btnLoadTestcase = new Button();
            btnChayTestcase = new Button();

            lblTrangThaiTestcase = new Label();

            lblChonDauSach = new Label();
            cboDauSach = new ComboBox();
            btnKiemTra = new Button();

            grpDanhSach = new GroupBox();
            dgvTestcase = new DataGridView();

            grpThongTin = new GroupBox();

            lblISBN = new Label();
            txtISBN = new TextBox();

            lblMaTuaSach = new Label();
            txtMaTuaSach = new TextBox();

            lblTuaSach = new Label();
            txtTuaSach = new TextBox();

            lblTacGia = new Label();
            txtTacGia = new TextBox();

            lblNgonNgu = new Label();
            txtNgonNgu = new TextBox();

            lblBia = new Label();
            txtBia = new TextBox();

            lblTrangThai = new Label();
            txtTrangThai = new TextBox();

            lblTomTat = new Label();
            txtTomTat = new TextBox();

            grpSoLuong = new GroupBox();
            lblSoLuong = new Label();
            txtSoLuongChuaMuon = new TextBox();

            btnLamMoi = new Button();

            pnlScrollableContent = new Panel();

            pnlFixedTop.SuspendLayout();
            pnlHeader.SuspendLayout();
            grpChonDauSach.SuspendLayout();
            pnlScrollableContent.SuspendLayout();
            grpDanhSach.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)dgvTestcase)
                .BeginInit();

            grpThongTin.SuspendLayout();
            grpSoLuong.SuspendLayout();

            SuspendLayout();

            // =================================================
            // pnlFixedTop
            // Giữ cố định tiêu đề và vùng chọn đầu sách.
            // =================================================
            pnlFixedTop.BackColor =
                Color.WhiteSmoke;

            pnlFixedTop.Controls.Add(
                grpChonDauSach
            );

            pnlFixedTop.Controls.Add(
                pnlHeader
            );

            pnlFixedTop.Dock =
                DockStyle.Top;

            pnlFixedTop.Location =
                new Point(0, 0);

            pnlFixedTop.Name =
                "pnlFixedTop";

            pnlFixedTop.Size =
                new Size(1100, 290);

            pnlFixedTop.TabIndex =
                0;

            // =================================================
            // pnlHeader
            // =================================================
            pnlHeader.BackColor =
                Color.FromArgb(0, 120, 215);

            pnlHeader.Controls.Add(lblTitle);

            pnlHeader.Dock =
                DockStyle.Top;

            pnlHeader.Location =
                new Point(0, 0);

            pnlHeader.Name =
                "pnlHeader";

            pnlHeader.Size =
                new Size(1100, 90);

            pnlHeader.TabIndex =
                0;

            pnlHeader.Paint +=
                pnlHeader_Paint;

            // =================================================
            // lblTitle
            // =================================================
            lblTitle.Dock =
                DockStyle.Fill;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    18F,
                    FontStyle.Bold
                );

            lblTitle.ForeColor =
                Color.White;

            lblTitle.Name =
                "lblTitle";

            lblTitle.Text =
                "TRA CỨU THÔNG TIN ĐẦU SÁCH";

            lblTitle.TextAlign =
                ContentAlignment.MiddleCenter;

            // =================================================
            // grpChonDauSach
            // =================================================
            grpChonDauSach.Controls.Add(
                btnKetNoi
            );

            grpChonDauSach.Controls.Add(
                btnLoadTestcase
            );

            grpChonDauSach.Controls.Add(
                btnChayTestcase
            );

            grpChonDauSach.Controls.Add(
                lblTrangThaiTestcase
            );

            grpChonDauSach.Controls.Add(
                lblChonDauSach
            );

            grpChonDauSach.Controls.Add(
                cboDauSach
            );

            grpChonDauSach.Controls.Add(
                btnKiemTra
            );

            grpChonDauSach.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpChonDauSach.Location =
                new Point(35, 110);

            grpChonDauSach.Name =
                "grpChonDauSach";

            grpChonDauSach.Size =
                new Size(1030, 165);

            grpChonDauSach.TabStop =
                false;

            grpChonDauSach.Text =
                "Kết nối và chọn đầu sách";

            // =================================================
            // btnKetNoi
            // =================================================
            btnKetNoi.BackColor =
                Color.FromArgb(0, 120, 215);

            btnKetNoi.Cursor =
                Cursors.Hand;

            btnKetNoi.FlatAppearance.BorderSize =
                0;

            btnKetNoi.FlatStyle =
                FlatStyle.Flat;

            btnKetNoi.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnKetNoi.ForeColor =
                Color.White;

            btnKetNoi.Location =
                new Point(30, 32);

            btnKetNoi.Name =
                "btnKetNoi";

            btnKetNoi.Size =
                new Size(180, 40);

            btnKetNoi.TabIndex =
                0;

            btnKetNoi.Text =
                "🔗 Kết nối CSDL";

            btnKetNoi.UseVisualStyleBackColor =
                false;

            btnKetNoi.Click +=
                btnKetNoi_Click;

            // =================================================
            // btnLoadTestcase
            // =================================================
            btnLoadTestcase.BackColor =
                Color.FromArgb(0, 123, 255);

            btnLoadTestcase.Cursor =
                Cursors.Hand;

            btnLoadTestcase.FlatAppearance.BorderSize =
                0;

            btnLoadTestcase.FlatStyle =
                FlatStyle.Flat;

            btnLoadTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnLoadTestcase.ForeColor =
                Color.White;

            btnLoadTestcase.Location =
                new Point(225, 32);

            btnLoadTestcase.Name =
                "btnLoadTestcase";

            btnLoadTestcase.Size =
                new Size(180, 40);

            btnLoadTestcase.TabIndex =
                1;

            btnLoadTestcase.Text =
                "📋 Load Testcase";

            btnLoadTestcase.UseVisualStyleBackColor =
                false;

            btnLoadTestcase.Click +=
                btnLoadTestcase_Click;

            // =================================================
            // btnChayTestcase
            // =================================================
            btnChayTestcase.BackColor =
                Color.FromArgb(255, 153, 0);

            btnChayTestcase.Cursor =
                Cursors.Hand;

            btnChayTestcase.FlatAppearance.BorderSize =
                0;

            btnChayTestcase.FlatStyle =
                FlatStyle.Flat;

            btnChayTestcase.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnChayTestcase.ForeColor =
                Color.White;

            btnChayTestcase.Location =
                new Point(420, 32);

            btnChayTestcase.Name =
                "btnChayTestcase";

            btnChayTestcase.Size =
                new Size(190, 40);

            btnChayTestcase.TabIndex =
                2;

            btnChayTestcase.Text =
                "🧪 Chạy Testcase";

            btnChayTestcase.UseVisualStyleBackColor =
                false;

            btnChayTestcase.Click +=
                btnChayTestcase_Click;

            // =================================================
            // lblTrangThaiTestcase
            // =================================================
            lblTrangThaiTestcase.AutoSize =
                true;

            lblTrangThaiTestcase.Font =
                new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Bold
                );

            lblTrangThaiTestcase.ForeColor =
                Color.DarkOrange;

            lblTrangThaiTestcase.Location =
                new Point(630, 42);

            lblTrangThaiTestcase.Name =
                "lblTrangThaiTestcase";

            lblTrangThaiTestcase.Size =
                new Size(150, 21);

            lblTrangThaiTestcase.TabIndex =
                3;

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";

            // =================================================
            // lblChonDauSach
            // =================================================
            lblChonDauSach.AutoSize =
                true;

            lblChonDauSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblChonDauSach.Location =
                new Point(30, 110);

            lblChonDauSach.Name =
                "lblChonDauSach";

            lblChonDauSach.Size =
                new Size(85, 23);

            lblChonDauSach.TabIndex =
                4;

            lblChonDauSach.Text =
                "Đầu sách:";

            // =================================================
            // cboDauSach
            // =================================================
            cboDauSach.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboDauSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            cboDauSach.Location =
                new Point(125, 106);

            cboDauSach.Name =
                "cboDauSach";

            cboDauSach.Size =
                new Size(700, 31);

            cboDauSach.TabIndex =
                5;

            cboDauSach.SelectedIndexChanged +=
                cboDauSach_SelectedIndexChanged;

            // =================================================
            // btnKiemTra
            // =================================================
            btnKiemTra.BackColor =
                Color.FromArgb(30, 170, 85);

            btnKiemTra.Cursor =
                Cursors.Hand;

            btnKiemTra.FlatAppearance.BorderSize =
                0;

            btnKiemTra.FlatStyle =
                FlatStyle.Flat;

            btnKiemTra.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnKiemTra.ForeColor =
                Color.White;

            btnKiemTra.Location =
                new Point(845, 102);

            btnKiemTra.Name =
                "btnKiemTra";

            btnKiemTra.Size =
                new Size(150, 40);

            btnKiemTra.TabIndex =
                6;

            btnKiemTra.Text =
                "🔍 Kiểm tra";

            btnKiemTra.UseVisualStyleBackColor =
                false;

            btnKiemTra.Click +=
                btnKiemTra_Click;

            // =================================================
            // grpDanhSach
            // =================================================
            grpDanhSach.Controls.Add(
                dgvTestcase
            );

            grpDanhSach.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpDanhSach.Location =
                new Point(35, 10);

            grpDanhSach.Name =
                "grpDanhSach";

            grpDanhSach.Size =
                new Size(1030, 300);

            grpDanhSach.TabIndex =
                2;

            grpDanhSach.TabStop =
                false;

            grpDanhSach.Text =
                "Danh sách testcase Bài 3";

            // =================================================
            // dgvTestcase
            // =================================================
            dgvTestcase.AllowUserToAddRows =
                false;

            dgvTestcase.AllowUserToDeleteRows =
                false;

            dgvTestcase.AllowUserToResizeRows =
                false;

            dgvTestcase.BackgroundColor =
                Color.White;

            dgvTestcase.BorderStyle =
                BorderStyle.Fixed3D;

            dgvTestcase.ColumnHeadersHeight =
                40;

            dgvTestcase.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvTestcase.Location =
                new Point(20, 32);

            dgvTestcase.Name =
                "dgvTestcase";

            dgvTestcase.ReadOnly =
                true;

            dgvTestcase.RowHeadersVisible =
                false;

            dgvTestcase.RowHeadersWidth =
                51;

            dgvTestcase.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvTestcase.MultiSelect =
                false;

            dgvTestcase.Size =
                new Size(990, 245);

            dgvTestcase.TabIndex =
                0;

            dgvTestcase.CellClick +=
                dgvTestcase_CellClick;

            // =================================================
            // grpThongTin
            // =================================================
            grpThongTin.Controls.Add(lblISBN);
            grpThongTin.Controls.Add(txtISBN);

            grpThongTin.Controls.Add(lblMaTuaSach);
            grpThongTin.Controls.Add(txtMaTuaSach);

            grpThongTin.Controls.Add(lblTuaSach);
            grpThongTin.Controls.Add(txtTuaSach);

            grpThongTin.Controls.Add(lblTacGia);
            grpThongTin.Controls.Add(txtTacGia);

            grpThongTin.Controls.Add(lblNgonNgu);
            grpThongTin.Controls.Add(txtNgonNgu);

            grpThongTin.Controls.Add(lblBia);
            grpThongTin.Controls.Add(txtBia);

            grpThongTin.Controls.Add(lblTrangThai);
            grpThongTin.Controls.Add(txtTrangThai);

            grpThongTin.Controls.Add(lblTomTat);
            grpThongTin.Controls.Add(txtTomTat);

            grpThongTin.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpThongTin.Location =
                new Point(35, 330);

            grpThongTin.Name =
                "grpThongTin";

            grpThongTin.Size =
                new Size(1030, 410);

            grpThongTin.TabIndex =
                3;

            grpThongTin.TabStop =
                false;

            grpThongTin.Text =
                "Thông tin đầu sách và tựa sách";

            // =================================================
            // ISBN
            // =================================================
            lblISBN.AutoSize =
                true;

            lblISBN.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblISBN.Location =
                new Point(30, 45);

            lblISBN.Name =
                "lblISBN";

            lblISBN.Text =
                "ISBN:";

            txtISBN.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtISBN.Location =
                new Point(160, 41);

            txtISBN.Name =
                "txtISBN";

            txtISBN.ReadOnly =
                true;

            txtISBN.Size =
                new Size(835, 30);

            // =================================================
            // MÃ TỰA SÁCH
            // =================================================
            lblMaTuaSach.AutoSize =
                true;

            lblMaTuaSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblMaTuaSach.Location =
                new Point(30, 85);

            lblMaTuaSach.Name =
                "lblMaTuaSach";

            lblMaTuaSach.Text =
                "Mã tựa sách:";

            txtMaTuaSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtMaTuaSach.Location =
                new Point(160, 81);

            txtMaTuaSach.Name =
                "txtMaTuaSach";

            txtMaTuaSach.ReadOnly =
                true;

            txtMaTuaSach.Size =
                new Size(835, 30);

            // =================================================
            // TỰA SÁCH
            // =================================================
            lblTuaSach.AutoSize =
                true;

            lblTuaSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblTuaSach.Location =
                new Point(30, 125);

            lblTuaSach.Name =
                "lblTuaSach";

            lblTuaSach.Text =
                "Tựa sách:";

            txtTuaSach.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtTuaSach.Location =
                new Point(160, 121);

            txtTuaSach.Name =
                "txtTuaSach";

            txtTuaSach.ReadOnly =
                true;

            txtTuaSach.Size =
                new Size(835, 30);

            // =================================================
            // TÁC GIẢ
            // =================================================
            lblTacGia.AutoSize =
                true;

            lblTacGia.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblTacGia.Location =
                new Point(30, 165);

            lblTacGia.Name =
                "lblTacGia";

            lblTacGia.Text =
                "Tác giả:";

            txtTacGia.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtTacGia.Location =
                new Point(160, 161);

            txtTacGia.Name =
                "txtTacGia";

            txtTacGia.ReadOnly =
                true;

            txtTacGia.Size =
                new Size(835, 30);

            // =================================================
            // NGÔN NGỮ
            // =================================================
            lblNgonNgu.AutoSize =
                true;

            lblNgonNgu.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblNgonNgu.Location =
                new Point(30, 205);

            lblNgonNgu.Name =
                "lblNgonNgu";

            lblNgonNgu.Text =
                "Ngôn ngữ:";

            txtNgonNgu.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtNgonNgu.Location =
                new Point(160, 201);

            txtNgonNgu.Name =
                "txtNgonNgu";

            txtNgonNgu.ReadOnly =
                true;

            txtNgonNgu.Size =
                new Size(300, 30);

            // =================================================
            // BÌA
            // =================================================
            lblBia.AutoSize =
                true;

            lblBia.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblBia.Location =
                new Point(500, 205);

            lblBia.Name =
                "lblBia";

            lblBia.Text =
                "Bìa:";

            txtBia.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtBia.Location =
                new Point(570, 201);

            txtBia.Name =
                "txtBia";

            txtBia.ReadOnly =
                true;

            txtBia.Size =
                new Size(425, 30);

            // =================================================
            // TRẠNG THÁI
            // =================================================
            lblTrangThai.AutoSize =
                true;

            lblTrangThai.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblTrangThai.Location =
                new Point(30, 245);

            lblTrangThai.Name =
                "lblTrangThai";

            lblTrangThai.Text =
                "Trạng thái:";

            txtTrangThai.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtTrangThai.Location =
                new Point(160, 241);

            txtTrangThai.Name =
                "txtTrangThai";

            txtTrangThai.ReadOnly =
                true;

            txtTrangThai.Size =
                new Size(835, 30);

            // =================================================
            // TÓM TẮT
            // =================================================
            lblTomTat.AutoSize =
                true;

            lblTomTat.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblTomTat.Location =
                new Point(30, 290);

            lblTomTat.Name =
                "lblTomTat";

            lblTomTat.Text =
                "Tóm tắt:";

            txtTomTat.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            txtTomTat.Location =
                new Point(160, 286);

            txtTomTat.Name =
                "txtTomTat";

            txtTomTat.Multiline =
                true;

            txtTomTat.ReadOnly =
                true;

            txtTomTat.ScrollBars =
                ScrollBars.Vertical;

            txtTomTat.Size =
                new Size(835, 90);

            // =================================================
            // grpSoLuong
            // =================================================
            grpSoLuong.Controls.Add(
                lblSoLuong
            );

            grpSoLuong.Controls.Add(
                txtSoLuongChuaMuon
            );

            grpSoLuong.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            grpSoLuong.Location =
                new Point(35, 760);

            grpSoLuong.Name =
                "grpSoLuong";

            grpSoLuong.Size =
                new Size(1030, 110);

            grpSoLuong.TabIndex =
                4;

            grpSoLuong.TabStop =
                false;

            grpSoLuong.Text =
                "Số lượng sách hiện chưa được mượn";

            // =================================================
            // lblSoLuong
            // =================================================
            lblSoLuong.AutoSize =
                true;

            lblSoLuong.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            lblSoLuong.Location =
                new Point(280, 50);

            lblSoLuong.Name =
                "lblSoLuong";

            lblSoLuong.Text =
                "Số cuốn có thể mượn:";

            // =================================================
            // txtSoLuongChuaMuon
            // =================================================
            txtSoLuongChuaMuon.BackColor =
                Color.White;

            txtSoLuongChuaMuon.Font =
                new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold
                );

            txtSoLuongChuaMuon.ForeColor =
                Color.Firebrick;

            txtSoLuongChuaMuon.Location =
                new Point(500, 40);

            txtSoLuongChuaMuon.Name =
                "txtSoLuongChuaMuon";

            txtSoLuongChuaMuon.ReadOnly =
                true;

            txtSoLuongChuaMuon.Size =
                new Size(220, 39);

            txtSoLuongChuaMuon.TextAlign =
                HorizontalAlignment.Center;

            // =================================================
            // btnLamMoi
            // =================================================
            btnLamMoi.BackColor =
                Color.FromArgb(90, 100, 110);

            btnLamMoi.Cursor =
                Cursors.Hand;

            btnLamMoi.FlatAppearance.BorderSize =
                0;

            btnLamMoi.FlatStyle =
                FlatStyle.Flat;

            btnLamMoi.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnLamMoi.ForeColor =
                Color.White;

            btnLamMoi.Location =
                new Point(475, 890);

            btnLamMoi.Name =
                "btnLamMoi";

            btnLamMoi.Size =
                new Size(150, 45);

            btnLamMoi.TabIndex =
                5;

            btnLamMoi.Text =
                "🔄 Làm mới";

            btnLamMoi.UseVisualStyleBackColor =
                false;

            btnLamMoi.Click +=
                btnLamMoi_Click;

            // =================================================
            // pnlScrollableContent
            // Chỉ vùng nội dung này cuộn; phần trên luôn cố định.
            // =================================================
            pnlScrollableContent.AutoScroll =
                true;

            pnlScrollableContent.AutoScrollMinSize =
                new Size(0, 950);

            pnlScrollableContent.BackColor =
                Color.WhiteSmoke;

            pnlScrollableContent.Controls.Add(
                btnLamMoi
            );

            pnlScrollableContent.Controls.Add(
                grpSoLuong
            );

            pnlScrollableContent.Controls.Add(
                grpThongTin
            );

            pnlScrollableContent.Controls.Add(
                grpDanhSach
            );

            pnlScrollableContent.Dock =
                DockStyle.Fill;

            pnlScrollableContent.Location =
                new Point(0, 290);

            pnlScrollableContent.Name =
                "pnlScrollableContent";

            pnlScrollableContent.Size =
                new Size(1100, 610);

            pnlScrollableContent.TabIndex =
                1;

            // =================================================
            // Form1
            // =================================================
            AutoScaleDimensions =
                new SizeF(8F, 20F);

            AutoScaleMode =
                AutoScaleMode.Font;

            BackColor =
                Color.WhiteSmoke;

            ClientSize =
                new Size(1100, 900);

            Controls.Add(
                pnlScrollableContent
            );

            Controls.Add(
                pnlFixedTop
            );

            Name =
                "Form1";

            StartPosition =
                FormStartPosition.CenterScreen;

            Text =
                "Bài 3 - Thông tin đầu sách";

            Load +=
                Form1_Load;

            pnlFixedTop.ResumeLayout(
                false
            );

            pnlHeader.ResumeLayout(
                false
            );

            grpChonDauSach.ResumeLayout(
                false
            );

            grpChonDauSach.PerformLayout();

            pnlScrollableContent.ResumeLayout(
                false
            );

            grpDanhSach.ResumeLayout(
                false
            );

            ((System.ComponentModel.ISupportInitialize)dgvTestcase)
                .EndInit();

            grpThongTin.ResumeLayout(
                false
            );

            grpThongTin.PerformLayout();

            grpSoLuong.ResumeLayout(
                false
            );

            grpSoLuong.PerformLayout();

            ResumeLayout(
                false
            );
        }

        #endregion

        private Panel pnlFixedTop;
        private Panel pnlHeader;
        private Label lblTitle;

        private GroupBox grpChonDauSach;

        private Button btnKetNoi;
        private Button btnLoadTestcase;
        private Button btnChayTestcase;

        private Label lblTrangThaiTestcase;

        private Label lblChonDauSach;
        private ComboBox cboDauSach;

        private Button btnKiemTra;

        private GroupBox grpDanhSach;
        private DataGridView dgvTestcase;

        private GroupBox grpThongTin;

        private Label lblISBN;
        private TextBox txtISBN;

        private Label lblMaTuaSach;
        private TextBox txtMaTuaSach;

        private Label lblTuaSach;
        private TextBox txtTuaSach;

        private Label lblTacGia;
        private TextBox txtTacGia;

        private Label lblNgonNgu;
        private TextBox txtNgonNgu;

        private Label lblBia;
        private TextBox txtBia;

        private Label lblTrangThai;
        private TextBox txtTrangThai;

        private Label lblTomTat;
        private TextBox txtTomTat;

        private GroupBox grpSoLuong;

        private Label lblSoLuong;
        private TextBox txtSoLuongChuaMuon;

        private Button btnLamMoi;
        private Panel pnlScrollableContent;
    }
}
