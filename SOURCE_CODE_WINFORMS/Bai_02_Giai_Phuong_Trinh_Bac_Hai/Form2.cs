using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Bài_2
{
    public partial class Form1 : Form
    {
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_DeAn;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
        }

        // ============================================================
        // HÀM KIỂM TRA VÀ CHUYỂN HỆ SỐ
        // ============================================================
        private bool TryParseHeSo(
            string? input,
            string tenHeSo,
            out double value,
            out string loi)
        {
            value = 0;
            loi = "";

            if (input == null)
            {
                loi = $"Vui lòng nhập hệ số {tenHeSo}!";
                return false;
            }

            string str = input.Trim();

            // Chuỗi NULL dùng cho testcase SQL
            if (string.Equals(
                str,
                "NULL",
                StringComparison.OrdinalIgnoreCase))
            {
                loi = $"Hệ số {tenHeSo} có giá trị NULL.";
                return false;
            }

            // Rỗng
            if (string.IsNullOrWhiteSpace(str))
            {
                loi = $"Vui lòng nhập hệ số {tenHeSo}!";
                return false;
            }

            // Phân số
            if (str.Contains("/"))
            {
                loi = $"Hệ số {tenHeSo} đang ở dạng phân số. Vui lòng đổi sang số thập phân.";
                return false;
            }

            // Hỗ trợ cả 1.5 và 1,5
            string parseValue = str.Replace(',', '.');

            if (!double.TryParse(
                    parseValue,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value))
            {
                loi = $"Hệ số {tenHeSo} không hợp lệ!";
                return false;
            }

            if (double.IsNaN(value) ||
                double.IsInfinity(value))
            {
                loi = $"Hệ số {tenHeSo} không hợp lệ!";
                return false;
            }

            return true;
        }

        // ============================================================
        // GỌI FUNCTION SQL fn_GiaiPTB2
        // ============================================================
        private string GiaiPTB2_SQL(
            double a,
            double b,
            double c)
        {
            using (SqlConnection conn =
                   new SqlConnection(strCon))
            {
                string sql =
                    "SELECT dbo.fn_GiaiPTB2(@a, @b, @c)";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(
                        "@a",
                        SqlDbType.Float
                    ).Value = a;

                    cmd.Parameters.Add(
                        "@b",
                        SqlDbType.Float
                    ).Value = b;

                    cmd.Parameters.Add(
                        "@c",
                        SqlDbType.Float
                    ).Value = c;

                    conn.Open();

                    object? result =
                        cmd.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return "Không nhận được kết quả từ SQL.";
                    }

                    return result.ToString() ?? "";
                }
            }
        }

        // ============================================================
        // GỌI FUNCTION SQL VỚI THAM SỐ CHUỖI (HỖ TRỢ NULL CHO TESTCASE)
        // ============================================================
        private string GiaiPTB2_SQL_Param(
            string strA,
            string strB,
            string strC)
        {
            using SqlConnection conn = new SqlConnection(strCon);
            string sql = "SELECT dbo.fn_GiaiPTB2(@a, @b, @c)";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            if (string.Equals(strA.Trim(), "NULL", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.Add("@a", SqlDbType.Float).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@a", SqlDbType.Float).Value = double.Parse(strA.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);

            if (string.Equals(strB.Trim(), "NULL", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.Add("@b", SqlDbType.Float).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@b", SqlDbType.Float).Value = double.Parse(strB.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);

            if (string.Equals(strC.Trim(), "NULL", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.Add("@c", SqlDbType.Float).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@c", SqlDbType.Float).Value = double.Parse(strC.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);

            conn.Open();
            object? result = cmd.ExecuteScalar();
            return result?.ToString() ?? "Không nhận được kết quả từ SQL.";
        }

        // ============================================================
        // NÚT GIẢI PHƯƠNG TRÌNH TRÊN GIAO DIỆN CHÍNH
        // ============================================================
        private void btnGiai_Click(
            object sender,
            EventArgs e)
        {
            txtKetQua.Clear();

            if (!TryParseHeSo(
                    txtA.Text,
                    "a",
                    out double a,
                    out string loiA))
            {
                MessageBox.Show(
                    loiA,
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtA.Focus();
                txtA.SelectAll();
                return;
            }

            if (!TryParseHeSo(
                    txtB.Text,
                    "b",
                    out double b,
                    out string loiB))
            {
                MessageBox.Show(
                    loiB,
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtB.Focus();
                txtB.SelectAll();
                return;
            }

            if (!TryParseHeSo(
                    txtC.Text,
                    "c",
                    out double c,
                    out string loiC))
            {
                MessageBox.Show(
                    loiC,
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtC.Focus();
                txtC.SelectAll();
                return;
            }

            try
            {
                txtKetQua.Text =
                    GiaiPTB2_SQL(a, b, c);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Lỗi SQL hoặc kết nối cơ sở dữ liệu:\n" +
                    ex.Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi:\n" +
                    ex.Message,
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // NÚT LÀM MỚI
        // ============================================================
        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            txtA.Clear();
            txtB.Clear();
            txtC.Clear();
            txtKetQua.Clear();

            dgvTestcase.DataSource = null;

            lblThongKe.Text =
                "Chưa load testcase";

            txtA.Focus();
        }

        // ============================================================
        // LOAD TESTCASE
        // ============================================================
        private void btnLoadTestcase_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                txtKetQua.Clear();
                using (SqlConnection conn =
                       new SqlConnection(strCon))
                {
                    using (SqlCommand cmd =
                           new SqlCommand(
                               "dbo.sp_LoadTestcase_Nhom1",
                               conn))
                    {
                        cmd.CommandType =
                            CommandType.StoredProcedure;

                        cmd.Parameters.Add(
                            "@Bai",
                            SqlDbType.VarChar,
                            10
                        ).Value = "B2";

                        using (SqlDataAdapter adapter =
                               new SqlDataAdapter(cmd))
                        {
                            DataTable dt =
                                new DataTable();

                            adapter.Fill(dt);

                            if (!dt.Columns.Contains("KetQuaThucTe"))
                            {
                                dt.Columns.Add("KetQuaThucTe", typeof(string));
                            }

                            if (!dt.Columns.Contains("TrangThai"))
                            {
                                dt.Columns.Add("TrangThai", typeof(string));
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                row["KetQuaThucTe"] = "";
                                row["TrangThai"] = "CHƯA CHẠY";
                            }

                            dgvTestcase.DataSource = dt;
                        }
                    }
                }

                DinhDangDataGridView();

                lblThongKe.Text =
                    $"Đã load {dgvTestcase.Rows.Count} testcase - Sẵn sàng kiểm thử";

                MessageBox.Show(
                    $"Đã load {dgvTestcase.Rows.Count} testcase Bài 2 từ CSDL.\n\n" +
                    "Bấm 'Chạy testcase' để thực thi và so sánh kết quả tự động.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Không thể load testcase:\n" +
                    ex.Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi:\n" +
                    ex.Message,
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // CHẠY TOÀN BỘ TESTCASE
        // ============================================================
        private void btnChayTestcase_Click(
            object sender,
            EventArgs e)
        {
            using var operation = DoAn.Shared.FormOperation.TryStart(this);
            if (operation is null) return;
            if (dgvTestcase.DataSource == null ||
                dgvTestcase.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có testcase.\n" +
                    "Vui lòng bấm Load Testcase trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

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
                foreach (DataGridViewRow row
                         in dgvTestcase.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string aText =
                        row.Cells["GiaTriA"]
                            .Value?
                            .ToString() ?? "";

                    string bText =
                        row.Cells["GiaTriB"]
                            .Value?
                            .ToString() ?? "";

                    string cText =
                        row.Cells["GiaTriC"]
                            .Value?
                            .ToString() ?? "";

                    string loaiTest =
                        row.Cells["LoaiTest"]
                            .Value?
                            .ToString() ?? "";

                    string kyVong =
                        row.Cells["KyVong"]
                            .Value?
                            .ToString() ?? "";

                    string maCase =
                        row.Cells["MaCase"]
                            .Value?
                            .ToString() ?? "";

                    row.Cells["KetQuaThucTe"].Value = "";
                    row.Cells["TrangThai"].Value = "ĐANG CHẠY";

                    dgvTestcase.Refresh();
                    Application.DoEvents();

                    try
                    {
                        string ketQua;

                        if (loaiTest == "WINFORMS")
                        {
                            ketQua = ChayTestWinForms(aText, bText, cText);
                        }
                        else
                        {
                            ketQua = GiaiPTB2_SQL_Param(aText, bText, cText);
                        }

                        row.Cells["KetQuaThucTe"].Value = ketQua;

                        if (SoSanhKetQuaB2(ketQua, kyVong, maCase))
                        {
                            row.Cells["TrangThai"].Value = "PASS";
                            passCount++;
                        }
                        else
                        {
                            row.Cells["TrangThai"].Value = "FAIL";
                            failCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        row.Cells["KetQuaThucTe"].Value = ex.Message;
                        row.Cells["TrangThai"].Value = "ERROR";
                        errorCount++;
                    }

                    daChay++;

                    lblThongKe.Text =
                        $"Tổng: {dgvTestcase.Rows.Count} | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                    dgvTestcase.Refresh();
                    Application.DoEvents();
                }

                txtKetQua.Text =
                    $"Tổng: {daChay} testcase | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                MessageBox.Show(
                    $"Đã chạy xong {daChay} testcase Bài 2.\n\n" +
                    $"PASS: {passCount}\nFAIL: {failCount}\nERROR: {errorCount}",
                    "Kết quả kiểm thử",
                    MessageBoxButtons.OK,
                    failCount == 0 && errorCount == 0
                        ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã xảy ra lỗi khi chạy testcase:\n" +
                    ex.Message,
                    "Lỗi hệ thống",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLoadTestcase.Enabled = true;
                btnChayTestcase.Enabled = true;
            }
        }

        private string ChayTestWinForms(string aText, string bText, string cText)
        {
            if (string.IsNullOrWhiteSpace(aText)) return "Vui lòng nhập hệ số a!";
            if (string.IsNullOrWhiteSpace(bText)) return "Vui lòng nhập hệ số b!";
            if (string.IsNullOrWhiteSpace(cText)) return "Vui lòng nhập hệ số c!";

            string a = aText.Trim();
            string b = bText.Trim();
            string c = cText.Trim();

            if (a.Contains("/")) return "Hệ số a đang ở dạng phân số. Vui lòng đổi sang số thập phân.";
            if (b.Contains("/")) return "Hệ số b đang ở dạng phân số. Vui lòng đổi sang số thập phân.";
            if (c.Contains("/")) return "Hệ số c đang ở dạng phân số. Vui lòng đổi sang số thập phân.";

            if (!TryParseHeSo(a, "a", out double valA, out string loiA)) return loiA;
            if (!TryParseHeSo(b, "b", out double valB, out string loiB)) return loiB;
            if (!TryParseHeSo(c, "c", out double valC, out string loiC)) return loiC;

            return GiaiPTB2_SQL(valA, valB, valC);
        }

        private static bool SoSanhKetQuaB2(string actual, string expected, string maCase = "")
        {
            if (string.IsNullOrWhiteSpace(actual))
                return false;

            actual = actual.Trim();
            expected = (expected ?? "").Trim();

            // 1. So khớp trực tiếp chuỗi
            if (!string.IsNullOrEmpty(expected) && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                return true;

            // Kết quả có nghiệm phải khớp cả loại nghiệm và giá trị từng nghiệm.
            if (expected.Contains("2 nghiệm"))
                return actual.Contains("2 nghiệm") && SameRoots(expected, actual, 2);
            if (expected.Contains("nghiệm kép"))
                return actual.Contains("nghiệm kép") && SameRoots(expected, actual, 1);
            if (expected.Contains("1 nghiệm"))
                return actual.Contains("1 nghiệm") && SameRoots(expected, actual, 1);

            // So khớp thông báo không có nghiệm hoặc validation.
            if (expected.Contains("nhập hệ số a") && (actual.Contains("nhập hệ số a") || actual.Contains("Bỏ trống a"))) return true;
            if (expected.Contains("nhập hệ số b") && (actual.Contains("nhập hệ số b") || actual.Contains("Bỏ trống b"))) return true;
            if (expected.Contains("nhập hệ số c") && (actual.Contains("nhập hệ số c") || actual.Contains("Bỏ trống c"))) return true;
            if (expected.Contains("Hệ số a không hợp lệ") && actual.Contains("a không hợp lệ")) return true;
            if (expected.Contains("Hệ số b không hợp lệ") && actual.Contains("b không hợp lệ")) return true;
            if (expected.Contains("Hệ số c không hợp lệ") && actual.Contains("c không hợp lệ")) return true;
            if (expected.Contains("phân số") && actual.Contains("phân số")) return true;
            if (expected.Contains("không được để trống") && actual.Contains("không được để trống")) return true;
            if (expected.Contains("vô số nghiệm") && actual.Contains("vô số nghiệm")) return true;
            if (expected.Contains("vô nghiệm") && actual.Contains("vô nghiệm")) return true;
            // Không coi kết quả là PASS chỉ dựa vào mã testcase.

            return false;
        }

        private static bool SameRoots(string expected, string actual, int count)
        {
            static double[] Extract(string text)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(
                    text, @"(?:x1|x2|x)\s*=\s*([-+]?(?:\d+(?:[.,]\d*)?|[.,]\d+)(?:[eE][-+]?\d+)?)");
                var values = new System.Collections.Generic.List<double>();
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    if (!double.TryParse(match.Groups[1].Value.Replace(',', '.'),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out double value) || !double.IsFinite(value))
                        return [];
                    values.Add(value);
                }
                return values.ToArray();
            }

            double[] expectedRoots = Extract(expected);
            double[] actualRoots = Extract(actual);
            if (expectedRoots.Length != count || actualRoots.Length != count) return false;
            for (int i = 0; i < count; i++)
                if (Math.Abs(expectedRoots[i] - actualRoots[i]) >
                    1e-9 * Math.Max(1d, Math.Abs(expectedRoots[i]))) return false;
            return true;
        }

        // ============================================================
        // ĐỊNH DẠNG DATAGRIDVIEW
        // ============================================================
        private void DinhDangDataGridView()
        {
            if (dgvTestcase.Columns.Count == 0)
                return;

            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTestcase.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            dgvTestcase.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            dgvTestcase.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold);

            dgvTestcase.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvTestcase.DefaultCellStyle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F);

            // Mã testcase
            if (dgvTestcase.Columns["MaCase"] != null)
            {
                dgvTestcase.Columns["MaCase"].HeaderText =
                    "Mã testcase";
                dgvTestcase.Columns["MaCase"].FillWeight = 80;
            }

            // Ẩn cột bài
            if (dgvTestcase.Columns["Bai"] != null)
            {
                dgvTestcase.Columns["Bai"].Visible = false;
            }

            // Bỏ cột Mô tả
            if (dgvTestcase.Columns["MoTa"] != null)
            {
                dgvTestcase.Columns["MoTa"].Visible = false;
            }

            // A
            if (dgvTestcase.Columns["GiaTriA"] != null)
            {
                dgvTestcase.Columns["GiaTriA"].HeaderText = "Giá trị a";
                dgvTestcase.Columns["GiaTriA"].FillWeight = 65;
            }

            // B
            if (dgvTestcase.Columns["GiaTriB"] != null)
            {
                dgvTestcase.Columns["GiaTriB"].HeaderText = "Giá trị b";
                dgvTestcase.Columns["GiaTriB"].FillWeight = 65;
            }

            // C
            if (dgvTestcase.Columns["GiaTriC"] != null)
            {
                dgvTestcase.Columns["GiaTriC"].HeaderText = "Giá trị c";
                dgvTestcase.Columns["GiaTriC"].FillWeight = 65;
            }

            // Ẩn năm sinh
            if (dgvTestcase.Columns["NamSinh"] != null)
            {
                dgvTestcase.Columns["NamSinh"].Visible = false;
            }

            // Loại test
            if (dgvTestcase.Columns["LoaiTest"] != null)
            {
                dgvTestcase.Columns["LoaiTest"].HeaderText = "Loại test";
                dgvTestcase.Columns["LoaiTest"].FillWeight = 70;
            }

            // Bỏ cột Kỳ vọng
            if (dgvTestcase.Columns["KyVong"] != null)
            {
                dgvTestcase.Columns["KyVong"].Visible = false;
            }

            // Kết quả thực tế
            if (dgvTestcase.Columns["KetQuaThucTe"] != null)
            {
                dgvTestcase.Columns["KetQuaThucTe"].HeaderText = "Kết quả thực tế";
                dgvTestcase.Columns["KetQuaThucTe"].FillWeight = 240;
            }

            if (dgvTestcase.Columns["TrangThai"] != null)
            {
                dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
                dgvTestcase.Columns["TrangThai"].FillWeight = 80;
            }

            dgvTestcase.CellFormatting -=
                dgvTestcase_CellFormatting;

            dgvTestcase.CellFormatting +=
                dgvTestcase_CellFormatting;
        }

        private void dgvTestcase_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 ||
                dgvTestcase.Columns[e.ColumnIndex]
                    .DataPropertyName != "TrangThai")
            {
                return;
            }

            string trangThai = e.Value?.ToString() ?? "";

            switch (trangThai)
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

        // ============================================================
        // FORM LOAD
        // ============================================================
        private void Form1_Load(
            object sender,
            EventArgs e)
        {
            txtA.Focus();
            lblThongKe.Text =
                "Chưa load testcase";
        }
    }
}
