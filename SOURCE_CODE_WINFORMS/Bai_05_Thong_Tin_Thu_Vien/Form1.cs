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
        private readonly Button btnLoadDuLieu = new Button();
        private readonly ComboBox cboBangDuLieu = new ComboBox();
        private readonly Label lblTrangThaiDuLieu = new Label();
        private readonly Panel pnlScrollableContent = new Panel();
        private readonly GroupBox grpDuLieuLienQuan = new GroupBox();
        private readonly DataGridView dgvDuLieuLienQuan = new DataGridView();

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

            CauHinhNutTaiDuLieu();

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
                txtInput.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        btnTraCuu.PerformClick();
                    }
                };

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

            CauHinhBangDuLieuLienQuan();

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
            dgvKetQua.DataBindingComplete += (_, _) =>
            {
                if (dgvKetQua.Columns["MaDocGia"] != null) dgvKetQua.Columns["MaDocGia"].HeaderText = "Mã độc giả";
                if (dgvKetQua.Columns["Ho"] != null) dgvKetQua.Columns["Ho"].HeaderText = "Họ";
                if (dgvKetQua.Columns["TenLot"] != null) dgvKetQua.Columns["TenLot"].HeaderText = "Tên lót";
                if (dgvKetQua.Columns["Ten"] != null) dgvKetQua.Columns["Ten"].HeaderText = "Tên";
                if (dgvKetQua.Columns["NgaySinh"] != null) dgvKetQua.Columns["NgaySinh"].HeaderText = "Ngày sinh";
                if (dgvKetQua.Columns["SoNha"] != null) dgvKetQua.Columns["SoNha"].HeaderText = "Số nhà";
                if (dgvKetQua.Columns["Duong"] != null) dgvKetQua.Columns["Duong"].HeaderText = "Đường";
                if (dgvKetQua.Columns["Quan"] != null) dgvKetQua.Columns["Quan"].HeaderText = "Quận";
                if (dgvKetQua.Columns["DienThoai"] != null) dgvKetQua.Columns["DienThoai"].HeaderText = "Điện thoại";
                if (dgvKetQua.Columns["HanSuDung"] != null) dgvKetQua.Columns["HanSuDung"].HeaderText = "Hạn SD";
                if (dgvKetQua.Columns["MaDocGiaNguoiLon"] != null) dgvKetQua.Columns["MaDocGiaNguoiLon"].HeaderText = "Mã ĐG bảo lãnh";
                if (dgvKetQua.Columns["LoaiDocGia"] != null) dgvKetQua.Columns["LoaiDocGia"].HeaderText = "Loại độc giả";
            };
            dgvKetQua.CellClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && dgvKetQua.Columns.Contains("MaDocGia"))
                {
                    string? ma = dgvKetQua.Rows[e.RowIndex].Cells["MaDocGia"].Value?.ToString();
                    if (!string.IsNullOrEmpty(ma))
                    {
                        txtInput.Text = ma;
                    }
                }
            };
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

            CauHinhNutTaiDuLieu();

            Label lblChonDauSach = TaoNhanBai5B(
                "Đầu sách:",
                35,
                112);
            grpTraCuu.Controls.Add(lblChonDauSach);

            cboDauSach.DropDownStyle = ComboBoxStyle.DropDown;
            cboDauSach.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboDauSach.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboDauSach.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnTraCuu.PerformClick();
                }
            };
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

            CauHinhBangDuLieuLienQuan();

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

        private void CauHinhNutTaiDuLieu()
        {
            Label lblBangDuLieu = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(225, 40),
                Text = "Bảng:"
            };
            grpTraCuu.Controls.Add(lblBangDuLieu);

            cboBangDuLieu.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBangDuLieu.Font = new Font("Segoe UI", 10F);
            cboBangDuLieu.Location = new Point(280, 34);
            cboBangDuLieu.Size = new Size(200, 31);
            cboBangDuLieu.Items.AddRange(LayDanhSachBangLienQuan());
            cboBangDuLieu.SelectedIndex = 0;
            cboBangDuLieu.SelectedIndexChanged +=
                cboBangDuLieu_SelectedIndexChanged;
            grpTraCuu.Controls.Add(cboBangDuLieu);

            btnLoadDuLieu.Text = "▣  Tải dữ liệu CSDL";
            btnLoadDuLieu.Size = new Size(200, 40);
            btnLoadDuLieu.Location = new Point(500, 30);
            btnLoadDuLieu.BackColor = Color.FromArgb(0, 120, 215);
            btnLoadDuLieu.ForeColor = Color.White;
            btnLoadDuLieu.FlatStyle = FlatStyle.Flat;
            btnLoadDuLieu.FlatAppearance.BorderSize = 0;
            btnLoadDuLieu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadDuLieu.Click += btnLoadDuLieu_Click;
            grpTraCuu.Controls.Add(btnLoadDuLieu);

            lblTrangThaiDuLieu.AutoEllipsis = true;
            lblTrangThaiDuLieu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTrangThaiDuLieu.ForeColor = Color.FromArgb(255, 125, 0);
            lblTrangThaiDuLieu.Location = new Point(720, 34);
            lblTrangThaiDuLieu.Size = new Size(285, 32);
            lblTrangThaiDuLieu.Text = "Chưa tải dữ liệu CSDL";
            lblTrangThaiDuLieu.TextAlign = ContentAlignment.MiddleLeft;
            grpTraCuu.Controls.Add(lblTrangThaiDuLieu);
        }

        private void CauHinhBangDuLieuLienQuan()
        {
            grpDuLieuLienQuan.Text = LayTieuDeDuLieuLienQuan();
            grpDuLieuLienQuan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpDuLieuLienQuan.Location = new Point(35, 10);
            grpDuLieuLienQuan.Size = new Size(1030, 300);
            pnlScrollableContent.Controls.Add(grpDuLieuLienQuan);

            CauHinhGrid(dgvDuLieuLienQuan);
            dgvDuLieuLienQuan.Location = new Point(20, 32);
            dgvDuLieuLienQuan.Size = new Size(990, 245);
            dgvDuLieuLienQuan.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvDuLieuLienQuan.DefaultCellStyle.WrapMode =
                DataGridViewTriState.False;
            dgvDuLieuLienQuan.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;
            dgvDuLieuLienQuan.CellClick += (_, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (dgvDuLieuLienQuan.Columns.Contains("ma_DocGia"))
                    {
                        string? ma = dgvDuLieuLienQuan.Rows[e.RowIndex].Cells["ma_DocGia"].Value?.ToString();
                        if (!string.IsNullOrEmpty(ma))
                        {
                            txtInput.Text = ma;
                            btnTraCuu.PerformClick();
                        }
                    }
                    else if (dgvDuLieuLienQuan.Columns.Contains("isbn"))
                    {
                        string? isbn = dgvDuLieuLienQuan.Rows[e.RowIndex].Cells["isbn"].Value?.ToString();
                        if (!string.IsNullOrEmpty(isbn))
                        {
                            if (maBai == "B5B")
                            {
                                cboDauSach.SelectedValue = isbn;
                                btnTraCuu.PerformClick();
                            }
                            else
                            {
                                txtInput.Text = isbn;
                                btnTraCuu.PerformClick();
                            }
                        }
                    }
                }
            };
            grpDuLieuLienQuan.Controls.Add(dgvDuLieuLienQuan);
        }

        private string LayTieuDeDuLieuLienQuan()
        {
            return maBai switch
            {
                "B5A" => "Dữ liệu CSDL liên quan: DocGia, Nguoilon, Treem",
                "B5B" => "Dữ liệu CSDL liên quan: Dausach, Tuasach, Cuonsach",
                "B5C" => "Dữ liệu CSDL liên quan: Muon, DocGia, Nguoilon",
                "B5D" => "Dữ liệu CSDL liên quan: Muon, DocGia, Nguoilon",
                "B5E" => "Dữ liệu CSDL liên quan: Nguoilon, Treem, DocGia, Muon",
                _ => "Dữ liệu CSDL liên quan"
            };
        }

        private object[] LayDanhSachBangLienQuan()
        {
            return maBai switch
            {
                "B5A" => new object[] { "DocGia", "Nguoilon", "Treem" },
                "B5B" => new object[] { "Dausach", "Tuasach", "Cuonsach" },
                "B5C" => new object[] { "Muon", "DocGia", "Nguoilon" },
                "B5D" => new object[] { "Muon", "DocGia", "Nguoilon" },
                "B5E" => new object[] { "Nguoilon", "Treem", "DocGia", "Muon" },
                _ => Array.Empty<object>()
            };
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
            string foundIsbn = row["ISBN"]?.ToString() ?? "";
            txtISBN.Text = foundIsbn;
            if (cboDauSach.SelectedValue?.ToString() != foundIsbn)
            {
                cboDauSach.SelectedValue = foundIsbn;
            }
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
            dgvKetQua.DataSource = null;
            if (maBai == "B5B") XoaThongTinBai5B();
            try
            {
                if (maBai == "B5B")
                {
                    string isbn =
                        (cboDauSach.SelectedIndex >= 0 ? cboDauSach.SelectedValue?.ToString() : cboDauSach.Text.Trim()) ?? "";
                    if (isbn.Contains(" - ")) isbn = isbn.Substring(0, isbn.IndexOf(" - ")).Trim();

                    if (string.IsNullOrWhiteSpace(isbn))
                    {
                        MessageBox.Show(
                            "Vui lòng chọn hoặc nhập đầu sách cần kiểm tra.",
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

                    if (!KetQuaDungMa(ketQua, "ISBN", isbn))
                    {
                        MessageBox.Show("Không tìm thấy đầu sách có ISBN này.",
                            "Thông báo tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

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

                    DataTable ketQua = TestcaseBai5Helper.ChayProcedure(
                        strCon, procedureName, maBai, input);
                    if (maBai == "B5A" && !KetQuaDungMa(ketQua, "MaDocGia", input))
                    {
                        MessageBox.Show("Không tìm thấy độc giả có mã này.",
                            "Thông báo tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    dgvKetQua.DataSource = ketQua;
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
            catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
            {
                MessageBox.Show(ex.Message, "Thông báo tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private static bool KetQuaDungMa(DataTable ketQua, string cotMa, string maNhap) =>
            ketQua.Rows.Count == 1 && ketQua.Columns.Contains(cotMa) &&
            string.Equals(ketQua.Rows[0][cotMa]?.ToString()?.Trim(), maNhap,
                StringComparison.OrdinalIgnoreCase);

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

        private void btnLoadDuLieu_Click(object? sender, EventArgs e)
        {
            try
            {
                string tenBang = cboBangDuLieu.SelectedItem?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(tenBang))
                {
                    MessageBox.Show(
                        "Vui lòng chọn bảng CSDL cần hiển thị.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                using SqlConnection conn = new SqlConnection(strCon);
                using SqlDataAdapter adapter = new SqlDataAdapter(
                    LayCauTruyVanBang(tenBang),
                    conn);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDuLieuLienQuan.DataSource = dt;

                grpDuLieuLienQuan.Text = $"Dữ liệu bảng {tenBang}";
                lblTrangThaiDuLieu.Text =
                    $"Đã tải {dt.Rows.Count} dòng từ {tenBang}";
                lblTrangThaiDuLieu.ForeColor = Color.SeaGreen;
            }
            catch (Exception ex)
            {
                lblTrangThaiDuLieu.Text = "Tải dữ liệu thất bại";
                lblTrangThaiDuLieu.ForeColor = Color.Firebrick;

                MessageBox.Show(
                    "Lỗi tải dữ liệu CSDL liên quan:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void cboBangDuLieu_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            string tenBang = cboBangDuLieu.SelectedItem?.ToString() ?? "";
            dgvDuLieuLienQuan.DataSource = null;
            grpDuLieuLienQuan.Text = string.IsNullOrWhiteSpace(tenBang)
                ? "Dữ liệu CSDL liên quan"
                : $"Dữ liệu bảng {tenBang}";
            lblTrangThaiDuLieu.Text = "Nhấn Tải dữ liệu CSDL để hiển thị";
            lblTrangThaiDuLieu.ForeColor = Color.FromArgb(255, 125, 0);
        }

        private static string LayCauTruyVanBang(string tenBang)
        {
            return tenBang switch
            {
                "DocGia" => "SELECT * FROM dbo.DocGia ORDER BY ma_DocGia;",
                "Nguoilon" => "SELECT * FROM dbo.Nguoilon ORDER BY ma_DocGia;",
                "Treem" => "SELECT * FROM dbo.Treem ORDER BY ma_DocGia;",
                "Dausach" => "SELECT * FROM dbo.Dausach ORDER BY isbn;",
                "Tuasach" => "SELECT * FROM dbo.Tuasach ORDER BY ma_tuasach;",
                "Cuonsach" =>
                    "SELECT * FROM dbo.Cuonsach ORDER BY isbn, ma_cuonsach;",
                "Muon" =>
                    "SELECT * FROM dbo.Muon ORDER BY ma_DocGia, isbn, ma_cuonsach;",
                _ => throw new InvalidOperationException(
                    "Bảng CSDL không hợp lệ: " + tenBang + ".")
            };
        }
    }
}
