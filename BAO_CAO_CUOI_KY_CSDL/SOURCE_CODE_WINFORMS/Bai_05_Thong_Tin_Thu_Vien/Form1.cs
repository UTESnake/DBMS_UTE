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

        private readonly Panel pnlHeader = new Panel();
        private readonly Label lblTitle = new Label();
        private readonly GroupBox grpTraCuu = new GroupBox();
        private readonly Label lblInput = new Label();
        private readonly TextBox txtInput = new TextBox();
        private readonly Button btnTraCuu = new Button();
        private readonly Button btnLamMoi = new Button();
        private readonly GroupBox grpKetQua = new GroupBox();
        private readonly DataGridView dgvKetQua = new DataGridView();
        private readonly Panel pnlTestcase = new Panel();
        private readonly Button btnLoadTestcase = new Button();
        private readonly Button btnChayTestcase = new Button();
        private readonly ComboBox cboNhomLoi = new ComboBox();
        private readonly Label lblTrangThaiTestcase = new Label();
        private readonly GroupBox grpDanhSachTestcase = new GroupBox();
        private readonly DataGridView dgvTestcase = new DataGridView();

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
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1180, 820);
            MinimumSize = new Size(1000, 700);

            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.BackColor = Color.FromArgb(30, 64, 175);

            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlHeader.Controls.Add(lblTitle);
            Controls.Add(pnlHeader);

            grpTraCuu.Text = coThamSo
                ? "Thông tin tra cứu"
                : "Thực hiện stored procedure";

            grpTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpTraCuu.Location = new Point(35, 110);
            grpTraCuu.Size = new Size(1110, 115);
            Controls.Add(grpTraCuu);

            if (coThamSo)
            {
                lblInput.Text = inputLabel;
                lblInput.AutoSize = true;
                lblInput.Font = new Font("Segoe UI", 10F);
                lblInput.Location = new Point(35, 48);

                txtInput.Font = new Font("Segoe UI", 10F);
                txtInput.Location = new Point(155, 43);
                txtInput.Size = new Size(310, 30);
                txtInput.MaxLength = 200;

                grpTraCuu.Controls.Add(lblInput);
                grpTraCuu.Controls.Add(txtInput);
            }

            btnTraCuu.Text = coThamSo ? "Tra cứu" : "Hiển thị dữ liệu";
            btnTraCuu.Size = new Size(180, 42);
            btnTraCuu.Location = new Point(coThamSo ? 665 : 360, 37);
            btnTraCuu.BackColor = Color.FromArgb(16, 185, 129);
            btnTraCuu.ForeColor = Color.White;
            btnTraCuu.FlatStyle = FlatStyle.Flat;
            btnTraCuu.FlatAppearance.BorderSize = 0;
            btnTraCuu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTraCuu.Click += btnTraCuu_Click;
            grpTraCuu.Controls.Add(btnTraCuu);

            btnLamMoi.Text = "Làm mới";
            btnLamMoi.Size = new Size(150, 42);
            btnLamMoi.Location = new Point(coThamSo ? 865 : 560, 37);
            btnLamMoi.BackColor = Color.FromArgb(100, 116, 139);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLamMoi.Click += btnLamMoi_Click;
            grpTraCuu.Controls.Add(btnLamMoi);

            grpKetQua.Text = "Kết quả thực thi Stored Procedure";
            grpKetQua.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpKetQua.Location = new Point(35, 240);
            grpKetQua.Size = new Size(1110, 215);
            Controls.Add(grpKetQua);

            CauHinhGrid(dgvKetQua);
            dgvKetQua.Location = new Point(15, 30);
            dgvKetQua.Size = new Size(1080, 165);
            grpKetQua.Controls.Add(dgvKetQua);

            pnlTestcase.Location = new Point(35, 470);
            pnlTestcase.Size = new Size(1110, 55);
            pnlTestcase.BackColor = BackColor;
            Controls.Add(pnlTestcase);

            btnLoadTestcase.Text = "▣  Load Testcase";
            btnLoadTestcase.Size = new Size(175, 44);
            btnLoadTestcase.Location = new Point(0, 5);
            btnLoadTestcase.BackColor = Color.FromArgb(37, 99, 235);
            btnLoadTestcase.ForeColor = Color.White;
            btnLoadTestcase.FlatStyle = FlatStyle.Flat;
            btnLoadTestcase.FlatAppearance.BorderSize = 0;
            btnLoadTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadTestcase.Click += btnLoadTestcase_Click;
            pnlTestcase.Controls.Add(btnLoadTestcase);

            btnChayTestcase.Text = "▶  Chạy Testcase";
            btnChayTestcase.Size = new Size(220, 44);
            btnChayTestcase.Location = new Point(190, 5);
            btnChayTestcase.BackColor = Color.FromArgb(79, 70, 229);
            btnChayTestcase.ForeColor = Color.White;
            btnChayTestcase.FlatStyle = FlatStyle.Flat;
            btnChayTestcase.FlatAppearance.BorderSize = 0;
            btnChayTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnChayTestcase.Click += btnChayTestcase_Click;
            pnlTestcase.Controls.Add(btnChayTestcase);

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
            cboNhomLoi.Location = new Point(430, 11);
            cboNhomLoi.Size = new Size(150, 30);
            pnlTestcase.Controls.Add(cboNhomLoi);

            lblTrangThaiTestcase.AutoSize = true;
            lblTrangThaiTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTrangThaiTestcase.ForeColor = Color.FromArgb(75, 85, 99);
            lblTrangThaiTestcase.Location = new Point(610, 16);
            lblTrangThaiTestcase.Text = "Chưa load testcase";
            pnlTestcase.Controls.Add(lblTrangThaiTestcase);

            grpDanhSachTestcase.Text = "Danh sách testcase";
            grpDanhSachTestcase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpDanhSachTestcase.Location = new Point(35, 535);
            grpDanhSachTestcase.Size = new Size(1110, 250);
            Controls.Add(grpDanhSachTestcase);

            CauHinhGrid(dgvTestcase);
            dgvTestcase.Location = new Point(15, 30);
            dgvTestcase.Size = new Size(1080, 200);
            dgvTestcase.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTestcase.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grpDanhSachTestcase.Controls.Add(dgvTestcase);
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
        }

        private void btnTraCuu_Click(object? sender, EventArgs e)
        {
            try
            {
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
            txtInput.Clear();
            dgvKetQua.DataSource = null;
        }

        private void btnLoadTestcase_Click(object? sender, EventArgs e)
        {
            try
            {
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
                    row["TrangThai"] = "CHƯA CHẠY";

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

                lblTrangThaiTestcase.Text = $"Đã chạy {dtTongKetQua.Rows.Count}/{dgvTestcase.Rows.Count} testcase";

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
                dgvTestcase.Columns["ID_Testcase"].HeaderText = "Mã";
                dgvTestcase.Columns["ID_Testcase"].FillWeight = 65;
            }

            if (dgvTestcase.Columns.Contains("MaBai"))
            {
                dgvTestcase.Columns["MaBai"].HeaderText = "Bài";
                dgvTestcase.Columns["MaBai"].FillWeight = 42;
            }

            if (dgvTestcase.Columns.Contains("NhomLoi"))
            {
                dgvTestcase.Columns["NhomLoi"].HeaderText = "Nhóm lỗi";
                dgvTestcase.Columns["NhomLoi"].FillWeight = 65;
            }

            if (dgvTestcase.Columns.Contains("ChucNang"))
            {
                dgvTestcase.Columns["ChucNang"].HeaderText =
                    "Stored procedure";

                dgvTestcase.Columns["ChucNang"].FillWeight = 90;
            }

            if (dgvTestcase.Columns.Contains("MoTa"))
            {
                dgvTestcase.Columns["MoTa"].HeaderText = "Mô tả";
                dgvTestcase.Columns["MoTa"].FillWeight = 150;
            }

            if (dgvTestcase.Columns.Contains("DuLieuNhap"))
            {
                dgvTestcase.Columns["DuLieuNhap"].HeaderText =
                    "Dữ liệu test";

                dgvTestcase.Columns["DuLieuNhap"].FillWeight = 155;
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
                dgvTestcase.Columns["KetQua"].FillWeight = 180;
            }

            if (dgvTestcase.Columns.Contains("TrangThai"))
            {
                dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
                dgvTestcase.Columns["TrangThai"].FillWeight = 85;
            }
        }
    }
}
