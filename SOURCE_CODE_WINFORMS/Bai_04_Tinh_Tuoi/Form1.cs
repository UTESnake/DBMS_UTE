using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace TinhTuoi
{
    public partial class Form1 : Form
    {
        // =====================================================
        // CHUỖI KẾT NỐI SQL SERVER
        // =====================================================
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_DeAn;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        // =====================================================
        // DATATABLE DÙNG ĐỂ HIỂN THỊ TESTCASE
        // =====================================================
        private readonly DataTable dtTestcase = new DataTable();

        public Form1()
        {
            InitializeComponent();

            KhoiTaoBangTestcase();
            DinhDangDataGridView();
        }

        // =====================================================
        // FORM LOAD
        // =====================================================
        private void Form1_Load(object sender, EventArgs e)
        {
            txtNamSinh.Focus();
            txtKetQua.Clear();

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";
        }

        // =====================================================
        // KHỞI TẠO CẤU TRÚC DATATABLE
        // =====================================================
        private void KhoiTaoBangTestcase()
        {
            if (dtTestcase.Columns.Count > 0)
                return;

            dtTestcase.Columns.Add("MaCase", typeof(string));
            dtTestcase.Columns.Add("MoTa", typeof(string));
            dtTestcase.Columns.Add("NamSinh", typeof(string));
            dtTestcase.Columns.Add("LoaiTest", typeof(string));
            dtTestcase.Columns.Add("KyVong", typeof(string));
            dtTestcase.Columns.Add("KetQua", typeof(string));
            dtTestcase.Columns.Add("TrangThai", typeof(string));

            dgvTestcase.DataSource = dtTestcase;
        }

        // =====================================================
        // ĐỊNH DẠNG DATAGRIDVIEW
        // =====================================================
        private void DinhDangDataGridView()
        {
            dgvTestcase.AllowUserToAddRows = false;
            dgvTestcase.AllowUserToDeleteRows = false;
            dgvTestcase.AllowUserToResizeRows = false;

            dgvTestcase.ReadOnly = true;
            dgvTestcase.RowHeadersVisible = false;
            dgvTestcase.MultiSelect = false;

            dgvTestcase.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTestcase.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            dgvTestcase.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            dgvTestcase.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);

            if (dgvTestcase.Columns["MaCase"] != null)
            {
                dgvTestcase.Columns["MaCase"].HeaderText = "Mã testcase";
                dgvTestcase.Columns["MaCase"].FillWeight = 85;
            }

            // Bỏ cột Mô tả
            if (dgvTestcase.Columns["MoTa"] != null)
            {
                dgvTestcase.Columns["MoTa"].Visible = false;
            }

            if (dgvTestcase.Columns["NamSinh"] != null)
            {
                dgvTestcase.Columns["NamSinh"].HeaderText = "Ngày sinh";
                dgvTestcase.Columns["NamSinh"].FillWeight = 85;
            }

            if (dgvTestcase.Columns["LoaiTest"] != null)
            {
                dgvTestcase.Columns["LoaiTest"].HeaderText = "Loại test";
                dgvTestcase.Columns["LoaiTest"].FillWeight = 75;
            }

            // Bỏ cột Kết quả kỳ vọng
            if (dgvTestcase.Columns["KyVong"] != null)
            {
                dgvTestcase.Columns["KyVong"].Visible = false;
            }

            if (dgvTestcase.Columns["KetQua"] != null)
            {
                dgvTestcase.Columns["KetQua"].HeaderText = "Kết quả thực tế";
                dgvTestcase.Columns["KetQua"].FillWeight = 240;
            }

            if (dgvTestcase.Columns["TrangThai"] != null)
            {
                dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
                dgvTestcase.Columns["TrangThai"].FillWeight = 80;
            }

            dgvTestcase.CellFormatting -= dgvTestcase_CellFormatting;
            dgvTestcase.CellFormatting += dgvTestcase_CellFormatting;
        }

        // =====================================================
        // NÚT TÍNH TUỔI
        // =====================================================
        private void btnTinhTuoi_Click(
            object sender,
            EventArgs e)
        {
            txtKetQua.Clear();

            if (string.IsNullOrWhiteSpace(txtNamSinh.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập ngày sinh!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNamSinh.Focus();
                return;
            }

            string input = txtNamSinh.Text.Trim();

            if (!ThuChuyenNgaySinh(input, out DateTime ngaySinh))
            {
                MessageBox.Show(
                    ThongBaoNgayKhongHopLe(input) + "\n" +
                    "Vui lòng nhập đủ ngày/tháng/năm, ví dụ: 15/03/2005.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNamSinh.SelectAll();
                txtNamSinh.Focus();
                return;
            }

            try
            {
                txtKetQua.Text =
                    GoiFunctionTinhTuoi(ngaySinh);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    "Lỗi kết nối hoặc thực thi SQL:\n\n" +
                    sqlEx.Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi:\n\n" +
                    ex.Message,
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // NÚT LÀM MỚI
        // =====================================================
        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            txtNamSinh.Clear();
            txtKetQua.Clear();

            dtTestcase.Rows.Clear();

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";

            txtNamSinh.Focus();
        }

        // =====================================================
        // NÚT LOAD TESTCASE TỪ CSDL
        // =====================================================
        private void btnLoadTestcase_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                txtKetQua.Clear();
                dtTestcase.Rows.Clear();

                using SqlConnection conn =
                    new SqlConnection(strCon);

                using SqlCommand cmd =
                    new SqlCommand(
                        "dbo.sp_LoadTestcase_Nhom1",
                        conn
                    );

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.Add(
                    "@Bai",
                    SqlDbType.VarChar,
                    10
                ).Value = "B4";

                conn.Open();

                using SqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow row =
                        dtTestcase.NewRow();

                    row["MaCase"] =
                        reader["MaCase"]?.ToString() ?? "";

                    row["MoTa"] =
                        reader["MoTa"]?.ToString() ?? "";

                    row["NamSinh"] =
                        reader["NamSinh"] == DBNull.Value
                            ? ""
                            : reader["NamSinh"]?.ToString() ?? "";

                    row["LoaiTest"] =
                        reader["LoaiTest"]?.ToString() ?? "";

                    row["KyVong"] =
                        reader["KyVong"]?.ToString() ?? "";

                    row["KetQua"] = "";
                    row["TrangThai"] = "CHƯA CHẠY";

                    dtTestcase.Rows.Add(row);
                }

                DinhDangDataGridView();
                dgvTestcase.ClearSelection();

                lblTrangThaiTestcase.Text =
                    $"Đã load {dtTestcase.Rows.Count} testcase - Sẵn sàng kiểm thử";

                MessageBox.Show(
                    $"Đã load {dtTestcase.Rows.Count} testcase Bài 4.\n\n" +
                    "Bấm 'Chạy testcase' để thực thi và so sánh kết quả tự động.",
                    "Load testcase",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Không load được testcase!\n\n" +
                    ex.Message +
                    "\n\nHãy chạy file 02_Testcase.sql của Nhom_1_CSDL_ToanHoc trước.",
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi:\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // NÚT CHẠY TOÀN BỘ TESTCASE
        // =====================================================
        private void btnChayTestcase_Click(
            object sender,
            EventArgs e)
        {
            using var operation = DoAn.Shared.FormOperation.TryStart(this);
            if (operation is null) return;
            if (dtTestcase.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có testcase!\n" +
                    "Vui lòng bấm Load Testcase trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            btnLoadTestcase.Enabled = false;
            btnChayTestcase.Enabled = false;

            int daChay = 0;
            int passCount = 0;
            int failCount = 0;
            int errorCount = 0;

            try
            {
                foreach (DataRow row in dtTestcase.Rows)
                {
                    string namSinhText =
                        row["NamSinh"]?.ToString() ?? "";

                    string loaiTest =
                        row["LoaiTest"]?.ToString() ?? "";

                    string kyVong =
                        row["KyVong"]?.ToString() ?? "";

                    row["KetQua"] = "";
                    row["TrangThai"] = "ĐANG CHẠY";

                    dgvTestcase.Refresh();
                    Application.DoEvents();

                    try
                    {
                        string maCase = row["MaCase"]?.ToString() ?? "";
                        string ketQua = loaiTest.Equals("WINFORMS", StringComparison.OrdinalIgnoreCase)
                            ? ChayTestWinForms(namSinhText)
                            : ChayTestSQL(namSinhText);

                        row["KetQua"] = ketQua;

                        if (SoSanhKetQuaB4(ketQua, kyVong, namSinhText))
                        {
                            row["TrangThai"] = "PASS";
                            passCount++;
                        }
                        else
                        {
                            row["TrangThai"] = "FAIL";
                            failCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        row["KetQua"] = ex.Message;
                        row["TrangThai"] = "ERROR";
                        errorCount++;
                    }

                    daChay++;

                    lblTrangThaiTestcase.Text =
                        $"Tổng: {dtTestcase.Rows.Count} | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                    dgvTestcase.Refresh();
                    Application.DoEvents();
                }

                txtKetQua.Text =
                    $"Tổng: {daChay} testcase | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                MessageBox.Show(
                    $"Đã chạy xong {daChay} testcase Bài 4.\n\n" +
                    $"PASS: {passCount}\nFAIL: {failCount}\nERROR: {errorCount}",
                    "Kết quả kiểm thử",
                    MessageBoxButtons.OK,
                    failCount == 0 && errorCount == 0
                        ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không chạy được testcase!\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLoadTestcase.Enabled = true;
                btnChayTestcase.Enabled = true;
            }
        }

        private static bool SoSanhKetQuaB4(string actual, string expected, string ngaySinhText)
        {
            if (string.IsNullOrWhiteSpace(actual))
                return false;

            actual = actual.Trim();
            expected = (expected ?? "").Trim();

            // 1. So sánh trực tiếp chuỗi
            if (!string.IsNullOrEmpty(expected) && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                return true;

            // 2. So sánh từ khóa chuẩn
            if (expected.Contains("nhập ngày sinh") && actual.Contains("nhập ngày sinh")) return true;
            if (expected.Contains("không đúng định dạng") && actual.Contains("không đúng định dạng")) return true;
            if (expected.Contains("không tồn tại") && actual.Contains("không tồn tại")) return true;
            if (expected.Contains("không được để trống") && actual.Contains("không được để trống")) return true;
            if (expected.Contains("lớn hơn ngày hiện tại") && actual.Contains("lớn hơn ngày hiện tại")) return true;
            if (expected == "0" && (actual == "0" || actual.Contains("0 tuổi") || actual.Contains("Vừa sinh hôm nay"))) return true;

            if (int.TryParse(expected, out int expAge))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(actual, @"^" + expAge + @"(?: tuổi| năm|$)"))
                    return true;
            }

            if (expected.StartsWith("Tuổi hợp lệ", StringComparison.OrdinalIgnoreCase))
            {
                if (!ThuChuyenNgaySinh(ngaySinhText.Trim(), out DateTime birth)) return false;
                DateTime today = DateTime.Today;
                int age = today.Year - birth.Year;
                if (birth.AddYears(age) > today) age--;
                return birth <= today && ReadYears(actual) == age;
            }

            // Không coi kết quả là PASS chỉ dựa vào mã testcase.

            return false;
        }

        private static int? ReadYears(string actual)
        {
            if (actual == "Vừa sinh hôm nay") return 0;
            var match = System.Text.RegularExpressions.Regex.Match(actual, @"^(\d+)\s+(?:năm|tuổi)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int years)) return years;
            if (actual.Contains("tháng") || actual.Contains("ngày")) return 0;
            return null;
        }

        private static string ThongBaoNgayKhongHopLe(string input) =>
            System.Text.RegularExpressions.Regex.IsMatch(input, @"^(?:\d{1,2}/\d{1,2}/\d{4}|\d{4}-\d{2}-\d{2})$")
                ? "Ngày sinh không tồn tại!"
                : "Ngày sinh không đúng định dạng!";

        // =====================================================
        // CHẠY TESTCASE SQL
        // =====================================================
        private string ChayTestSQL(
            string namSinhText)
        {
            string value = namSinhText.Trim();

            DateTime? ngaySinh;

            if (value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = null;
            }
            else if (value.Equals("HOMNAY_MINUS_20Y_1D", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = DateTime.Today.AddYears(-20).AddDays(-1);
            }
            else if (value.Equals("HOMNAY_MINUS_20Y_PLUS_1D", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = DateTime.Today.AddYears(-20).AddDays(1);
            }
            else if (value.Equals("HOMNAY", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = DateTime.Today;
            }
            else if (value.Equals("NGAYMAI", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = DateTime.Today.AddDays(1);
            }
            else if (value.Equals("01/01/0001", StringComparison.OrdinalIgnoreCase))
            {
                ngaySinh = new DateTime(1, 1, 1);
            }
            else
            {
                if (!ThuChuyenNgaySinh(value, out DateTime ngay))
                {
                    return ThongBaoNgayKhongHopLe(value);
                }

                ngaySinh = ngay;
            }

            return GoiFunctionTinhTuoi(ngaySinh);
        }

        // =====================================================
        // CHẠY TESTCASE WINFORMS
        // =====================================================
        private string ChayTestWinForms(
            string namSinhText)
        {
            if (string.IsNullOrWhiteSpace(namSinhText))
            {
                return "Vui lòng nhập ngày sinh!";
            }

            string input = namSinhText.Trim();

            if (!ThuChuyenNgaySinh(input, out DateTime ngaySinh))
            {
                return ThongBaoNgayKhongHopLe(input);
            }

            return GoiFunctionTinhTuoi(ngaySinh);
        }

        private static bool ThuChuyenNgaySinh(
            string input,
            out DateTime ngaySinh)
        {
            string[] dinhDangHopLe =
            {
                "dd/MM/yyyy",
                "d/M/yyyy",
                "yyyy-MM-dd"
            };

            return DateTime.TryParseExact(
                input.Trim(),
                dinhDangHopLe,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out ngaySinh
            );
        }

        // =====================================================
        // GỌI FUNCTION SQL VÀ HIỂN THỊ ĐỦ NĂM, THÁNG, NGÀY
        // =====================================================
        private string GoiFunctionTinhTuoi(
            DateTime? ngaySinh)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            const string sqlQuery =
                "SELECT dbo.fn_TinhTuoi(@NgaySinh), CAST(GETDATE() AS DATE)";

            using SqlCommand cmd =
                new SqlCommand(
                    sqlQuery,
                    conn
                );

            cmd.CommandType =
                CommandType.Text;

            cmd.Parameters.Add(
                "@NgaySinh",
                SqlDbType.Date
            ).Value =
                ngaySinh.HasValue
                    ? ngaySinh.Value.Date
                    : DBNull.Value;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
                return "Không nhận được kết quả từ cơ sở dữ liệu.";

            DateTime ngayHienTai = reader.GetDateTime(1).Date;

            if (!reader.IsDBNull(0) && ngaySinh.HasValue)
                return DinhDangTuoi(
                    ngaySinh.Value.Date,
                    ngayHienTai,
                    reader.GetInt32(0)
                );

            if (!ngaySinh.HasValue)
                return "Ngày sinh không được để trống";

            if (ngaySinh.Value.Date > ngayHienTai)
                return "Ngày sinh không được lớn hơn ngày hiện tại";

            return "Không nhận được kết quả từ cơ sở dữ liệu.";
        }

        private static string DinhDangTuoi(
            DateTime ngaySinh,
            DateTime ngayHienTai,
            int soNam)
        {
            DateTime sauSoNam = ngaySinh.AddYears(soNam);
            int soThang = (ngayHienTai.Year - sauSoNam.Year) * 12
                + ngayHienTai.Month - sauSoNam.Month;

            if (sauSoNam.AddMonths(soThang) > ngayHienTai)
                soThang--;

            int soNgay = (ngayHienTai - sauSoNam.AddMonths(soThang)).Days;
            var thanhPhan = new System.Collections.Generic.List<string>();

            if (soNam > 0)
                thanhPhan.Add($"{soNam} năm");
            if (soThang > 0)
                thanhPhan.Add($"{soThang} tháng");
            if (soNgay > 0)
                thanhPhan.Add($"{soNgay} ngày");

            if (thanhPhan.Count == 0)
                return "Vừa sinh hôm nay";

            if (soNam > 0 && soThang == 0 && soNgay == 0)
                return $"{soNam} tuổi";

            return string.Join(" ", thanhPhan) + " tuổi";
        }

        private void dgvTestcase_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string namSinh =
                dgvTestcase.Rows[e.RowIndex]
                    .Cells["NamSinh"].Value?.ToString()
                ?? "";

            if (!string.IsNullOrWhiteSpace(namSinh))
            {
                txtNamSinh.Text = namSinh;
            }
        }

        private void dgvTestcase_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 ||
                dgvTestcase.Columns[e.ColumnIndex].DataPropertyName != "TrangThai")
            {
                return;
            }

            string status = e.Value?.ToString() ?? "";

            switch (status)
            {
                case "PASS":
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.BackColor = Color.FromArgb(220, 252, 231);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "FAIL":
                    e.CellStyle.ForeColor = Color.DarkRed;
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "ERROR":
                    e.CellStyle.ForeColor = Color.DarkOrange;
                    e.CellStyle.BackColor = Color.FromArgb(254, 243, 199);
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                case "ĐANG CHẠY":
                    e.CellStyle.ForeColor = Color.Blue;
                    e.CellStyle.Font = new Font(dgvTestcase.Font, FontStyle.Bold);
                    break;
                default:
                    e.CellStyle.ForeColor = Color.Gray;
                    break;
            }
        }
    }
}
