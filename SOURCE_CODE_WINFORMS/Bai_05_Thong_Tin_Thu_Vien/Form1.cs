using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace ThongTinThuVien
{
    public partial class Form1 : Form
    {
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_ThuVien;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        private bool daKetNoi = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            daKetNoi = false;
            btnKetNoi.Text = "Kết nối CSDL";
            lblTrangThai.Text = "● Chưa kết nối cơ sở dữ liệu";
            lblTrangThai.ForeColor = Color.Firebrick;
            CapNhatTrangThaiNut();
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(strCon);
                conn.Open();

                daKetNoi = true;
                btnKetNoi.Text = "✓ Đã kết nối";
                lblTrangThai.Text = "● Đã kết nối QL_ThuVien";
                lblTrangThai.ForeColor = Color.SeaGreen;
                CapNhatTrangThaiNut();

                MessageBox.Show(
                    "Kết nối cơ sở dữ liệu thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                daKetNoi = false;
                btnKetNoi.Text = "Kết nối CSDL";
                lblTrangThai.Text = "● Kết nối cơ sở dữ liệu thất bại";
                lblTrangThai.ForeColor = Color.Firebrick;
                CapNhatTrangThaiNut();

                MessageBox.Show(
                    "Không thể kết nối CSDL!\n\n" + ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatTrangThaiNut()
        {
            btnCauA.Enabled = daKetNoi;
            btnCauB.Enabled = daKetNoi;
            btnCauC.Enabled = daKetNoi;
            btnCauD.Enabled = daKetNoi;
            btnCauE.Enabled = daKetNoi;
        }

        private bool KiemTraDaKetNoi()
        {
            if (daKetNoi) return true;

            MessageBox.Show(
                "Bạn chưa kết nối cơ sở dữ liệu!\nVui lòng nhấn Kết nối CSDL trước.",
                "Chưa kết nối",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private void MoFormCon(
            string maBai,
            string tieuDeCuaSo,
            string tieuDe,
            string nhanInput,
            string procedureName,
            bool coThamSo)
        {
            if (!KiemTraDaKetNoi()) return;

            using FrmChucNangBai5 frm = new FrmChucNangBai5(
                strCon,
                maBai,
                tieuDeCuaSo,
                tieuDe,
                nhanInput,
                procedureName,
                coThamSo);

            frm.ShowDialog(this);
        }

        private void btnCauA_Click(object sender, EventArgs e)
        {
            MoFormCon(
                "B5A",
                "5a - Thông tin độc giả",
                "TRA CỨU THÔNG TIN ĐỘC GIẢ",
                "Mã độc giả:",
                "dbo.sp_ThongtinDocGia",
                true);
        }

        private void btnCauB_Click(object sender, EventArgs e)
        {
            MoFormCon(
                "B5B",
                "5b - Thông tin đầu sách",
                "TRA CỨU THÔNG TIN ĐẦU SÁCH",
                "ISBN:",
                "dbo.sp_ThongtinDausach",
                true);
        }

        private void btnCauC_Click(object sender, EventArgs e)
        {
            MoFormCon(
                "B5C",
                "5c - Người lớn đang mượn sách",
                "NGƯỜI LỚN ĐANG MƯỢN SÁCH",
                "",
                "dbo.sp_ThongtinNguoilonDangmuon",
                false);
        }

        private void btnCauD_Click(object sender, EventArgs e)
        {
            MoFormCon(
                "B5D",
                "5d - Người lớn mượn quá hạn",
                "NGƯỜI LỚN MƯỢN SÁCH QUÁ HẠN",
                "",
                "dbo.sp_ThongtinNguoilonQuahan",
                false);
        }

        private void btnCauE_Click(object sender, EventArgs e)
        {
            MoFormCon(
                "B5E",
                "5e - Người lớn có trẻ em cùng mượn",
                "NGƯỜI LỚN CÓ TRẺ EM CÙNG MƯỢN SÁCH",
                "",
                "dbo.sp_DocGiaCoTreEmMuon",
                false);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult kq = MessageBox.Show(
                "Bạn có chắc muốn thoát chương trình?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (kq == DialogResult.Yes)
                Close();
        }
    }

    public class FrmChucNangBai5 : Form
    {
        private readonly string strCon;
        private readonly string maBai;
        private readonly string procedureName;
        private readonly bool coThamSo;

        private readonly Panel pnlFixedTop = new Panel();
        private readonly Panel pnlHeader = new Panel();
        private readonly Label lblTitle = new Label();
        private readonly GroupBox grpTraCuu = new GroupBox();
        private readonly Button btnTrangThaiKetNoi = new Button();
        private readonly Label lblInput = new Label();
        private readonly TextBox txtInput = new TextBox();
        private readonly Button btnTraCuu = new Button();
        private readonly Button btnLamMoi = new Button();
        private readonly GroupBox grpKetQua = new GroupBox();
        private readonly DataGridView dgvKetQua = new DataGridView();
        private readonly Button btnLoadTestcase = new Button();
        private readonly Button btnChayTestcase = new Button();
        private readonly ComboBox cboNhomLoi = new ComboBox();
        private readonly Label lblTrangThaiTestcase = new Label();
        private readonly Panel pnlScrollableContent = new Panel();
        private readonly GroupBox grpDanhSachTestcase = new GroupBox();
        private readonly DataGridView dgvTestcase = new DataGridView();

        // Các điều khiển riêng của Bài 5b, bố trí giống hệt Bài 3.
        private readonly ComboBox cboDauSach = new ComboBox();
        private readonly GroupBox grpThongTinDauSach = new GroupBox();
        private readonly TextBox txtISBN = new TextBox();
        private readonly TextBox txtMaTuaSach = new TextBox();
        private readonly TextBox txtTuaSach = new TextBox();
        private readonly TextBox txtTacGia = new TextBox();
        private readonly TextBox txtNgonNgu = new TextBox();
        private readonly TextBox txtBia = new TextBox();
        private readonly TextBox txtTrangThaiDauSach = new TextBox();
        private readonly TextBox txtTomTat = new TextBox();
        private readonly GroupBox grpSoLuong = new GroupBox();
        private readonly TextBox txtSoLuongChuaMuon = new TextBox();

        public FrmChucNangBai5(
            string connectionString,
            string maBai,
            string windowTitle,
            string title,
            string inputLabel,
            string procedureName,
            bool coThamSo)
        {
            strCon = connectionString;
            this.maBai = maBai;
            this.procedureName = procedureName;
            this.coThamSo = coThamSo;

            KhoiTaoGiaoDien(windowTitle, title, inputLabel);
        }

        private void KhoiTaoGiaoDien(
            string windowTitle,
            string title,
            string inputLabel)
        {
            Text = windowTitle;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1100, 900);
            MinimumSize = new Size(1000, 700);

            if (maBai == "B5B")
            {
                KhoiTaoGiaoDienBai5B(title);
                LoadDanhSachDauSachBai5B();
                return;
            }

            pnlFixedTop.Dock = DockStyle.Top;
            pnlFixedTop.Height = 290;
            pnlFixedTop.BackColor = Color.WhiteSmoke;

            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.BackColor = Color.FromArgb(0, 120, 215);

            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlHeader.Controls.Add(lblTitle);
            pnlFixedTop.Controls.Add(pnlHeader);

            grpTraCuu.Text = coThamSo
                ? "Kết nối và nhập thông tin tra cứu"
                : "Kết nối và thực hiện stored procedure";

            grpTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpTraCuu.Location = new Point(35, 105);
            grpTraCuu.Size = new Size(1030, 165);
            pnlFixedTop.Controls.Add(grpTraCuu);

            btnTrangThaiKetNoi.Text = "☑ Đã kết nối";
            btnTrangThaiKetNoi.Size = new Size(180, 40);
            btnTrangThaiKetNoi.Location = new Point(30, 30);
            btnTrangThaiKetNoi.BackColor = Color.FromArgb(0, 120, 215);
            btnTrangThaiKetNoi.ForeColor = Color.White;
            btnTrangThaiKetNoi.FlatStyle = FlatStyle.Flat;
            btnTrangThaiKetNoi.FlatAppearance.BorderSize = 0;
            btnTrangThaiKetNoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTrangThaiKetNoi.TabStop = false;
            grpTraCuu.Controls.Add(btnTrangThaiKetNoi);

            btnLoadTestcase.Text = "▣  Load Testcase";
            btnLoadTestcase.Size = new Size(180, 40);
            btnLoadTestcase.Location = new Point(225, 30);
            btnLoadTestcase.BackColor = Color.FromArgb(0, 120, 215);
            btnLoadTestcase.ForeColor = Color.White;
            btnLoadTestcase.FlatStyle = FlatStyle.Flat;
            btnLoadTestcase.FlatAppearance.BorderSize = 0;
            btnLoadTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadTestcase.Click += btnLoadTestcase_Click;
            grpTraCuu.Controls.Add(btnLoadTestcase);

            btnChayTestcase.Text = "🧪 Chạy Testcase";
            btnChayTestcase.Size = new Size(190, 40);
            btnChayTestcase.Location = new Point(420, 30);
            btnChayTestcase.BackColor = Color.FromArgb(255, 145, 0);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.FlatStyle = FlatStyle.Flat;
            btnChayTestcase.FlatAppearance.BorderSize = 0;
            btnChayTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChayTestcase.Click += btnChayTestcase_Click;
            grpTraCuu.Controls.Add(btnChayTestcase);

            cboNhomLoi.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNhomLoi.Items.AddRange(new object[]
            {
                "TẤT CẢ",
                "OK",
                "WINFORM",
                "SQL",
                "NGHIEPVU",
                "DULIEU",
                "BIEN"
            });
            cboNhomLoi.SelectedIndex = 0;
            cboNhomLoi.Font = new Font("Segoe UI", 9.5F);
            cboNhomLoi.Location = new Point(625, 35);
            cboNhomLoi.Size = new Size(145, 30);
            grpTraCuu.Controls.Add(cboNhomLoi);

            lblTrangThaiTestcase.AutoEllipsis = true;
            lblTrangThaiTestcase.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTrangThaiTestcase.ForeColor = Color.FromArgb(255, 125, 0);
            lblTrangThaiTestcase.Location = new Point(790, 34);
            lblTrangThaiTestcase.Size = new Size(220, 32);
            lblTrangThaiTestcase.Text = "Chưa load testcase";
            lblTrangThaiTestcase.TextAlign = ContentAlignment.MiddleLeft;
            grpTraCuu.Controls.Add(lblTrangThaiTestcase);

            if (coThamSo)
            {
                lblInput.Text = inputLabel;
                lblInput.AutoSize = true;
                lblInput.Font = new Font("Segoe UI", 10F);
                lblInput.Location = new Point(35, 112);

                txtInput.Font = new Font("Segoe UI", 10F);
                txtInput.Location = new Point(155, 106);
                txtInput.Size = new Size(620, 30);
                txtInput.MaxLength = 200;

                grpTraCuu.Controls.Add(lblInput);
                grpTraCuu.Controls.Add(txtInput);
            }

            btnTraCuu.Text = coThamSo ? "Tra cứu" : "Hiển thị dữ liệu";
            btnTraCuu.Size = new Size(180, 42);
            btnTraCuu.Location = new Point(coThamSo ? 795 : 425, 100);
            btnTraCuu.BackColor = Color.FromArgb(30, 170, 85);
            btnTraCuu.ForeColor = Color.White;
            btnTraCuu.FlatStyle = FlatStyle.Flat;
            btnTraCuu.FlatAppearance.BorderSize = 0;
            btnTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTraCuu.Click += btnTraCuu_Click;
            grpTraCuu.Controls.Add(btnTraCuu);

            pnlScrollableContent.AutoScroll = true;
            pnlScrollableContent.AutoScrollMinSize = new Size(0, 735);
            pnlScrollableContent.BackColor = Color.WhiteSmoke;
            pnlScrollableContent.Dock = DockStyle.Fill;

            grpDanhSachTestcase.Text = "Danh sách testcase " + maBai;
            grpDanhSachTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpDanhSachTestcase.Location = new Point(35, 10);
            grpDanhSachTestcase.Size = new Size(1030, 300);
            pnlScrollableContent.Controls.Add(grpDanhSachTestcase);

            CauHinhGrid(dgvTestcase);
            dgvTestcase.Location = new Point(20, 32);
            dgvTestcase.Size = new Size(990, 245);
            dgvTestcase.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTestcase.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTestcase.CellFormatting += dgvTestcase_CellFormatting;
            grpDanhSachTestcase.Controls.Add(dgvTestcase);

            grpKetQua.Text = "Kết quả thực thi Stored Procedure";
            grpKetQua.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpKetQua.Location = new Point(35, 330);
            grpKetQua.Size = new Size(1030, 300);
            pnlScrollableContent.Controls.Add(grpKetQua);

            CauHinhGrid(dgvKetQua);
            dgvKetQua.Location = new Point(20, 32);
            dgvKetQua.Size = new Size(990, 245);
            dgvKetQua.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvKetQua.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grpKetQua.Controls.Add(dgvKetQua);

            btnLamMoi.Text = "🔄 Làm mới";
            btnLamMoi.Size = new Size(150, 42);
            btnLamMoi.Location = new Point(475, 650);
            btnLamMoi.BackColor = Color.FromArgb(90, 100, 110);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLamMoi.Click += btnLamMoi_Click;
            pnlScrollableContent.Controls.Add(btnLamMoi);

            Controls.Add(pnlScrollableContent);
            Controls.Add(pnlFixedTop);
        }

        private void KhoiTaoGiaoDienBai5B(string title)
        {
            pnlFixedTop.Dock = DockStyle.Top;
            pnlFixedTop.Height = 290;
            pnlFixedTop.BackColor = Color.WhiteSmoke;

            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.BackColor = Color.FromArgb(0, 120, 215);

            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);
            pnlFixedTop.Controls.Add(pnlHeader);

            grpTraCuu.Text = "Kết nối và chọn đầu sách";
            grpTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpTraCuu.Location = new Point(35, 105);
            grpTraCuu.Size = new Size(1030, 165);
            pnlFixedTop.Controls.Add(grpTraCuu);

            btnTrangThaiKetNoi.Text = "☑ Đã kết nối";
            btnTrangThaiKetNoi.Size = new Size(180, 40);
            btnTrangThaiKetNoi.Location = new Point(30, 30);
            btnTrangThaiKetNoi.BackColor = Color.FromArgb(0, 120, 215);
            btnTrangThaiKetNoi.ForeColor = Color.White;
            btnTrangThaiKetNoi.FlatStyle = FlatStyle.Flat;
            btnTrangThaiKetNoi.FlatAppearance.BorderSize = 0;
            btnTrangThaiKetNoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTrangThaiKetNoi.TabStop = false;
            grpTraCuu.Controls.Add(btnTrangThaiKetNoi);

            btnLoadTestcase.Text = "▣  Load Testcase";
            btnLoadTestcase.Size = new Size(180, 40);
            btnLoadTestcase.Location = new Point(225, 30);
            btnLoadTestcase.BackColor = Color.FromArgb(0, 120, 215);
            btnLoadTestcase.ForeColor = Color.White;
            btnLoadTestcase.FlatStyle = FlatStyle.Flat;
            btnLoadTestcase.FlatAppearance.BorderSize = 0;
            btnLoadTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadTestcase.Click += btnLoadTestcase_Click;
            grpTraCuu.Controls.Add(btnLoadTestcase);

            btnChayTestcase.Text = "🧪 Chạy Testcase";
            btnChayTestcase.Size = new Size(190, 40);
            btnChayTestcase.Location = new Point(420, 30);
            btnChayTestcase.BackColor = Color.FromArgb(255, 145, 0);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.FlatStyle = FlatStyle.Flat;
            btnChayTestcase.FlatAppearance.BorderSize = 0;
            btnChayTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChayTestcase.Click += btnChayTestcase_Click;
            grpTraCuu.Controls.Add(btnChayTestcase);

            lblTrangThaiTestcase.AutoSize = true;
            lblTrangThaiTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTrangThaiTestcase.ForeColor = Color.FromArgb(255, 125, 0);
            lblTrangThaiTestcase.Location = new Point(630, 39);
            lblTrangThaiTestcase.Text = "Chưa load testcase";
            grpTraCuu.Controls.Add(lblTrangThaiTestcase);

            cboNhomLoi.Items.Add("TẤT CẢ");
            cboNhomLoi.SelectedIndex = 0;

            Label lblChonDauSach = TaoNhanBai5B(
                "Đầu sách:",
                35,
                112);
            grpTraCuu.Controls.Add(lblChonDauSach);

            cboDauSach.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDauSach.Font = new Font("Segoe UI", 10F);
            cboDauSach.Location = new Point(125, 106);
            cboDauSach.Size = new Size(700, 31);
            grpTraCuu.Controls.Add(cboDauSach);

            btnTraCuu.Text = "🔍 Kiểm tra";
            btnTraCuu.Size = new Size(150, 40);
            btnTraCuu.Location = new Point(845, 102);
            btnTraCuu.BackColor = Color.FromArgb(30, 170, 85);
            btnTraCuu.ForeColor = Color.White;
            btnTraCuu.FlatStyle = FlatStyle.Flat;
            btnTraCuu.FlatAppearance.BorderSize = 0;
            btnTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTraCuu.Click += btnTraCuu_Click;
            grpTraCuu.Controls.Add(btnTraCuu);

            pnlScrollableContent.AutoScroll = true;
            pnlScrollableContent.AutoScrollMinSize = new Size(0, 950);
            pnlScrollableContent.BackColor = Color.WhiteSmoke;
            pnlScrollableContent.Dock = DockStyle.Fill;

            grpDanhSachTestcase.Text = "Danh sách testcase Bài 5b";
            grpDanhSachTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpDanhSachTestcase.Location = new Point(35, 10);
            grpDanhSachTestcase.Size = new Size(1030, 300);
            pnlScrollableContent.Controls.Add(grpDanhSachTestcase);

            CauHinhGrid(dgvTestcase);
            dgvTestcase.Location = new Point(20, 32);
            dgvTestcase.Size = new Size(990, 245);
            dgvTestcase.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTestcase.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTestcase.CellFormatting += dgvTestcase_CellFormatting;
            grpDanhSachTestcase.Controls.Add(dgvTestcase);

            grpThongTinDauSach.Text = "Thông tin đầu sách và tựa sách";
            grpThongTinDauSach.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpThongTinDauSach.Location = new Point(35, 330);
            grpThongTinDauSach.Size = new Size(1030, 410);
            pnlScrollableContent.Controls.Add(grpThongTinDauSach);

            ThemDongThongTinBai5B("ISBN:", txtISBN, 45);
            ThemDongThongTinBai5B("Mã tựa sách:", txtMaTuaSach, 85);
            ThemDongThongTinBai5B("Tựa sách:", txtTuaSach, 125);
            ThemDongThongTinBai5B("Tác giả:", txtTacGia, 165);

            grpThongTinDauSach.Controls.Add(
                TaoNhanBai5B("Ngôn ngữ:", 30, 205));
            CauHinhODocBai5B(txtNgonNgu, 160, 201, 300, 30);
            grpThongTinDauSach.Controls.Add(txtNgonNgu);

            grpThongTinDauSach.Controls.Add(
                TaoNhanBai5B("Bìa:", 500, 205));
            CauHinhODocBai5B(txtBia, 570, 201, 425, 30);
            grpThongTinDauSach.Controls.Add(txtBia);

            ThemDongThongTinBai5B(
                "Trạng thái:",
                txtTrangThaiDauSach,
                245);

            grpThongTinDauSach.Controls.Add(
                TaoNhanBai5B("Tóm tắt:", 30, 290));
            CauHinhODocBai5B(txtTomTat, 160, 286, 835, 90);
            txtTomTat.Multiline = true;
            txtTomTat.ScrollBars = ScrollBars.Vertical;
            grpThongTinDauSach.Controls.Add(txtTomTat);

            grpSoLuong.Text = "Số lượng sách hiện chưa được mượn";
            grpSoLuong.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpSoLuong.Location = new Point(35, 760);
            grpSoLuong.Size = new Size(1030, 110);
            pnlScrollableContent.Controls.Add(grpSoLuong);

            grpSoLuong.Controls.Add(
                TaoNhanBai5B("Số cuốn có thể mượn:", 280, 50));

            CauHinhODocBai5B(
                txtSoLuongChuaMuon,
                500,
                40,
                220,
                39);
            txtSoLuongChuaMuon.BackColor = Color.White;
            txtSoLuongChuaMuon.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtSoLuongChuaMuon.ForeColor = Color.Firebrick;
            txtSoLuongChuaMuon.TextAlign = HorizontalAlignment.Center;
            grpSoLuong.Controls.Add(txtSoLuongChuaMuon);

            btnLamMoi.Text = "🔄 Làm mới";
            btnLamMoi.Size = new Size(150, 45);
            btnLamMoi.Location = new Point(475, 890);
            btnLamMoi.BackColor = Color.FromArgb(90, 100, 110);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLamMoi.Click += btnLamMoi_Click;
            pnlScrollableContent.Controls.Add(btnLamMoi);

            Controls.Add(pnlScrollableContent);
            Controls.Add(pnlFixedTop);
        }

        private static Label TaoNhanBai5B(
            string text,
            int x,
            int y)
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x, y),
                Text = text
            };
        }

        private static void CauHinhODocBai5B(
            TextBox textBox,
            int x,
            int y,
            int width,
            int height)
        {
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.Location = new Point(x, y);
            textBox.ReadOnly = true;
            textBox.Size = new Size(width, height);
        }

        private void ThemDongThongTinBai5B(
            string nhan,
            TextBox textBox,
            int y)
        {
            grpThongTinDauSach.Controls.Add(
                TaoNhanBai5B(nhan, 30, y));
            CauHinhODocBai5B(textBox, 160, y - 4, 835, 30);
            grpThongTinDauSach.Controls.Add(textBox);
        }

        private void LoadDanhSachDauSachBai5B()
        {
            try
            {
                const string sql = @"
                    SELECT
                        ds.isbn,
                        ds.isbn + N' - ' + ts.tuasach AS HienThi
                    FROM dbo.Dausach AS ds
                    INNER JOIN dbo.Tuasach AS ts
                        ON ds.ma_tuasach = ts.ma_tuasach
                    ORDER BY ds.isbn;";

                using SqlConnection conn = new SqlConnection(strCon);
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cboDauSach.DisplayMember = "HienThi";
                cboDauSach.ValueMember = "isbn";
                cboDauSach.DataSource = dt;
                cboDauSach.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được danh sách đầu sách!\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void HienThiThongTinBai5B(DataTable dt)
        {
            XoaThongTinBai5B();

            if (dt.Rows.Count == 0)
                return;

            DataRow row = dt.Rows[0];
            txtISBN.Text = row["ISBN"]?.ToString() ?? "";
            txtMaTuaSach.Text = row["MaTuaSach"]?.ToString() ?? "";
            txtTuaSach.Text = row["TuaSach"]?.ToString() ?? "";
            txtTacGia.Text = row["TacGia"]?.ToString() ?? "";
            txtNgonNgu.Text = row["NgonNgu"]?.ToString() ?? "";
            txtBia.Text = row["Bia"]?.ToString() ?? "";
            txtTrangThaiDauSach.Text = row["TrangThai"]?.ToString() ?? "";
            txtTomTat.Text = row["TomTat"]?.ToString() ?? "";
            txtSoLuongChuaMuon.Text =
                row["SoLuongChuaMuon"]?.ToString() ?? "0";
        }

        private void XoaThongTinBai5B()
        {
            txtISBN.Clear();
            txtMaTuaSach.Clear();
            txtTuaSach.Clear();
            txtTacGia.Clear();
            txtNgonNgu.Clear();
            txtBia.Clear();
            txtTrangThaiDauSach.Clear();
            txtTomTat.Clear();
            txtSoLuongChuaMuon.Clear();
        }

        private static void CauHinhGrid(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
        }

        private void btnTraCuu_Click(object? sender, EventArgs e)
        {
            try
            {
                if (maBai == "B5B")
                {
                    string isbn =
                        cboDauSach.SelectedValue?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(isbn))
                    {
                        MessageBox.Show(
                            "Vui lòng chọn đầu sách cần kiểm tra.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    DataTable ketQua =
                        TestcaseBai5Helper.ChayProcedure(
                            strCon,
                            procedureName,
                            maBai,
                            isbn);

                    HienThiThongTinBai5B(ketQua);
                    return;
                }

                if (coThamSo)
                {
                    string input = txtInput.Text.Trim();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        MessageBox.Show(
                            "Vui lòng nhập dữ liệu tra cứu.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtInput.Focus();
                        return;
                    }

                    dgvKetQua.DataSource =
                        TestcaseBai5Helper.ChayProcedure(
                            strCon,
                            procedureName,
                            maBai,
                            input);
                }
                else
                {
                    dgvKetQua.DataSource =
                        TestcaseBai5Helper.ChayProcedure(
                            strCon,
                            procedureName,
                            maBai,
                            null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thực hiện stored procedure:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object? sender, EventArgs e)
        {
            if (maBai == "B5B")
            {
                cboDauSach.SelectedIndex = -1;
                XoaThongTinBai5B();
                return;
            }

            txtInput.Clear();
            dgvKetQua.DataSource = null;
        }

        private void btnLoadTestcase_Click(object? sender, EventArgs e)
        {
            try
            {
                // Bảng kết quả chỉ được hiển thị sau khi chạy testcase.
                dgvKetQua.DataSource = null;

                string? nhom =
                    cboNhomLoi.Text == "TẤT CẢ"
                        ? null
                        : cboNhomLoi.Text;

                DataTable dt =
                    TestcaseBai5Helper.LoadTestcase(
                        strCon,
                        maBai,
                        nhom);

                if (!dt.Columns.Contains("KetQua"))
                    dt.Columns.Add("KetQua", typeof(string));
                if (!dt.Columns.Contains("TrangThai"))
                    dt.Columns.Add("TrangThai", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["KetQua"] = "";
                    row["TrangThai"] = "CHƯA CHẠY";
                }

                dgvTestcase.DataSource = dt;
                DinhDangGridTestcase();

                lblTrangThaiTestcase.Text =
                    $"Đã load {dt.Rows.Count} testcase - Chưa chạy";

                lblTrangThaiTestcase.ForeColor =
                    Color.SeaGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi load testcase:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnChayTestcase_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvTestcase.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Chưa có testcase.\nHãy nhấn Load Testcase trước.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtTongKetQua = new DataTable();
                dtTongKetQua.Columns.Add("MaTestcase", typeof(string));
                dtTongKetQua.Columns.Add("NhomLoi", typeof(string));
                dtTongKetQua.Columns.Add("DuLieuThucTe", typeof(string));
                dtTongKetQua.Columns.Add("KetQuaXuLy", typeof(string));

                foreach (DataGridViewRow row in dgvTestcase.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string id =
                        row.Cells["ID_Testcase"].Value?.ToString() ?? "";

                    string nhomLoi =
                        row.Cells["NhomLoi"].Value?.ToString() ?? "";

                    string duLieuNhap =
                        row.Cells["DuLieuNhap"].Value?.ToString() ?? "";

                    string moTa =
                        row.Cells["MoTa"].Value?.ToString() ?? "";

                    row.Cells["TrangThai"].Value = "ĐANG CHẠY";
                    dgvTestcase.Refresh();

                    try
                    {
                        // =========================================
                        // WINFORM - CÂU CÓ THAM SỐ (5A, 5B)
                        // =========================================
                        if (nhomLoi.Equals(
                                "WINFORM",
                                StringComparison.OrdinalIgnoreCase)
                            && coThamSo)
                        {
                            string value =
                                TestcaseBai5Helper.LayInputWinForm(
                                    maBai,
                                    id,
                                    duLieuNhap,
                                    strCon);

                            DataTable dt =
                                TestcaseBai5Helper.ChayValidationWinForm(
                                    id,
                                    maBai,
                                    value);

                            foreach (DataRow r in dt.Rows)
                            {
                                dtTongKetQua.Rows.Add(
                                    r["MaTestcase"],
                                    r["NhomLoi"],
                                    r["DuLieuThucTe"],
                                    r["KetQuaXuLy"]);
                            }

                            row.Cells["KetQua"].Value = dt.Rows.Count > 0
                                ? dt.Rows[0]["KetQuaXuLy"]?.ToString() : "Đã xử lý";
                            row.Cells["TrangThai"].Value = "ĐÃ CHẠY";

                            continue;
                        }

                        // =========================================
                        // WINFORM - CÂU KHÔNG THAM SỐ (5C,5D,5E)
                        // =========================================
                        if (nhomLoi.Equals(
                                "WINFORM",
                                StringComparison.OrdinalIgnoreCase)
                            && !coThamSo)
                        {
                            DataTable dt =
                                TestcaseBai5Helper.ChayWinFormKhongThamSo(
                                    id,
                                    procedureName,
                                    strCon);

                            foreach (DataRow r in dt.Rows)
                            {
                                dtTongKetQua.Rows.Add(
                                    r["MaTestcase"],
                                    r["NhomLoi"],
                                    r["DuLieuThucTe"],
                                    r["KetQuaXuLy"]);
                            }

                            row.Cells["KetQua"].Value = dt.Rows.Count > 0
                                ? dt.Rows[0]["KetQuaXuLy"]?.ToString() : "Đã xử lý";
                            row.Cells["TrangThai"].Value = "ĐÃ CHẠY";

                            continue;
                        }

                        // =========================================
                        // OK / SQL / NGHIEPVU / BIEN / DULIEU
                        // =========================================
                        TestcaseRunResult result =
                            TestcaseBai5Helper.XuLyTestcase(
                                strCon,
                                maBai,
                                procedureName,
                                id,
                                nhomLoi,
                                duLieuNhap,
                                moTa);

                        string duLieuThucTe =
                            result.InputForForm ?? duLieuNhap;

                        string ketQua = result.Message;

                        if (result.Data != null)
                        {
                            ketQua +=
                                Environment.NewLine +
                                "Số dòng trả về: " +
                                result.Data.Rows.Count;
                        }

                        dtTongKetQua.Rows.Add(
                            id,
                            nhomLoi,
                            duLieuThucTe,
                            ketQua);
                        row.Cells["KetQua"].Value = ketQua;
                        row.Cells["TrangThai"].Value = "ĐÃ CHẠY";
                    }
                    catch (Exception exCase)
                    {
                        dtTongKetQua.Rows.Add(
                            id,
                            nhomLoi,
                            duLieuNhap,
                            "Lỗi khi chạy testcase: " + exCase.Message);
                        row.Cells["KetQua"].Value = exCase.Message;
                        row.Cells["TrangThai"].Value = "LỖI";
                    }

                    dgvTestcase.Refresh();
                }

                int soLoi = dgvTestcase.Rows
                    .Cast<DataGridViewRow>()
                    .Count(row =>
                        !row.IsNewRow &&
                        string.Equals(
                            row.Cells["TrangThai"].Value?.ToString(),
                            "LỖI",
                            StringComparison.OrdinalIgnoreCase));

                lblTrangThaiTestcase.Text =
                    $"Đã chạy: {dtTongKetQua.Rows.Count}/{dgvTestcase.Rows.Count} | Lỗi: {soLoi}";

                lblTrangThaiTestcase.ForeColor = soLoi == 0
                    ? Color.FromArgb(255, 125, 0)
                    : Color.Firebrick;

                if (maBai != "B5B")
                {
                    dgvKetQua.DataSource = dtTongKetQua;
                    dgvKetQua.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;
                    dgvKetQua.AutoSizeRowsMode =
                        DataGridViewAutoSizeRowsMode.AllCells;
                    dgvKetQua.DefaultCellStyle.WrapMode =
                        DataGridViewTriState.True;

                    if (dgvKetQua.Columns.Contains("MaTestcase"))
                        dgvKetQua.Columns["MaTestcase"].HeaderText = "Mã testcase";

                    if (dgvKetQua.Columns.Contains("NhomLoi"))
                        dgvKetQua.Columns["NhomLoi"].HeaderText = "Nhóm";

                    if (dgvKetQua.Columns.Contains("DuLieuThucTe"))
                        dgvKetQua.Columns["DuLieuThucTe"].HeaderText = "Dữ liệu thực tế";

                    if (dgvKetQua.Columns.Contains("KetQuaXuLy"))
                        dgvKetQua.Columns["KetQuaXuLy"].HeaderText = "Kết quả xử lý";
                }

                MessageBox.Show(
                    $"Đã chạy xong {dtTongKetQua.Rows.Count} testcase của {maBai}.",
                    "Hoàn thành",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi chạy toàn bộ testcase:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string LayCell(string column)
        {
            if (!dgvTestcase.Columns.Contains(column))
                return "";

            return dgvTestcase.CurrentRow?
                .Cells[column]
                .Value?
                .ToString() ?? "";
        }

        private void DinhDangGridTestcase()
        {
            if (dgvTestcase.Columns.Contains("ID_Testcase"))
            {
                dgvTestcase.Columns["ID_Testcase"].HeaderText = "Mã testcase";
                dgvTestcase.Columns["ID_Testcase"].FillWeight = 75;
            }

            if (dgvTestcase.Columns.Contains("MaBai"))
            {
                dgvTestcase.Columns["MaBai"].Visible = false;
            }

            if (dgvTestcase.Columns.Contains("NhomLoi"))
            {
                dgvTestcase.Columns["NhomLoi"].HeaderText = "Loại test";
                dgvTestcase.Columns["NhomLoi"].FillWeight = 75;
            }

            if (dgvTestcase.Columns.Contains("ChucNang"))
            {
                dgvTestcase.Columns["ChucNang"].Visible = false;
            }

            if (dgvTestcase.Columns.Contains("MoTa"))
            {
                dgvTestcase.Columns["MoTa"].HeaderText = "Mô tả";
                dgvTestcase.Columns["MoTa"].FillWeight = 170;
            }

            if (dgvTestcase.Columns.Contains("DuLieuNhap"))
            {
                dgvTestcase.Columns["DuLieuNhap"].HeaderText =
                    "Dữ liệu test";

                dgvTestcase.Columns["DuLieuNhap"].FillWeight = 130;
            }

            // Không hiển thị cột Ghi chú trên danh sách testcase.
            // Kết quả chạy testcase được hiển thị ở dgvKetQua phía trên.
            if (dgvTestcase.Columns.Contains("GhiChu"))
            {
                dgvTestcase.Columns["GhiChu"].Visible = false;
            }

            if (dgvTestcase.Columns.Contains("KetQua"))
            {
                dgvTestcase.Columns["KetQua"].HeaderText = "Kết quả sau khi chạy";
                dgvTestcase.Columns["KetQua"].FillWeight = 200;
            }

            if (dgvTestcase.Columns.Contains("TrangThai"))
            {
                dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
                dgvTestcase.Columns["TrangThai"].FillWeight = 90;
            }
        }

        private void dgvTestcase_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 ||
                dgvTestcase.Columns[e.ColumnIndex]
                    .DataPropertyName != "TrangThai")
            {
                return;
            }

            string trangThai = e.Value?.ToString() ?? "";

            e.CellStyle.ForeColor = trangThai switch
            {
                "ĐANG CHẠY" => Color.Blue,
                "ĐÃ CHẠY" => Color.Green,
                "LỖI" => Color.Red,
                _ => Color.Gray
            };

            if (trangThai != "CHƯA CHẠY")
            {
                e.CellStyle.Font = new Font(
                    dgvTestcase.Font,
                    FontStyle.Bold);
            }
        }
    }
}
