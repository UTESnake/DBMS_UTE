using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Bài_1
{
    public partial class Form1 : Form
    {
        // KẾT NỐI SQL SERVER
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_DeAn;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        // Bảng dùng để hiển thị testcase lên DataGridView
        private readonly DataTable dtTestcase = new DataTable();

        public Form1()
        {
            InitializeComponent();
            KhoiTaoBangTestcase();
            DinhDangDataGridView();
        }

        // TẠO CẤU TRÚC DATA TABLE: Testcase sẽ được load từ SQL Server.
        private void KhoiTaoBangTestcase()
        {
            dtTestcase.Columns.Add("MaCase");
            dtTestcase.Columns.Add("MoTa");
            dtTestcase.Columns.Add("GiaTriA");
            dtTestcase.Columns.Add("GiaTriB");
            dtTestcase.Columns.Add("LoaiTest");
            dtTestcase.Columns.Add("KyVong");
            dtTestcase.Columns.Add("KetQua");
            dtTestcase.Columns.Add("TrangThai");

            dgvTestcase.DataSource = dtTestcase;
        }

        // ĐỊNH DẠNG DATAGRIDVIEW
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

            dgvTestcase.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTestcase.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            // Đặt tiêu đề tiếng Việt & ẩn cột Mô tả, Kết quả kỳ vọng theo yêu cầu
            dgvTestcase.Columns["MaCase"].HeaderText = "Mã testcase";
            dgvTestcase.Columns["MaCase"].FillWeight = 85;

            // Bỏ cột Mô tả
            dgvTestcase.Columns["MoTa"].Visible = false;

            dgvTestcase.Columns["GiaTriA"].HeaderText = "Giá trị a";
            dgvTestcase.Columns["GiaTriA"].FillWeight = 70;

            dgvTestcase.Columns["GiaTriB"].HeaderText = "Giá trị b";
            dgvTestcase.Columns["GiaTriB"].FillWeight = 70;

            dgvTestcase.Columns["LoaiTest"].HeaderText = "Loại test";
            dgvTestcase.Columns["LoaiTest"].FillWeight = 75;

            // Bỏ cột Kết quả kỳ vọng
            dgvTestcase.Columns["KyVong"].Visible = false;

            dgvTestcase.Columns["KetQua"].HeaderText = "Kết quả thực tế";
            dgvTestcase.Columns["KetQua"].FillWeight = 220;

            dgvTestcase.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvTestcase.Columns["TrangThai"].FillWeight = 80;

            dgvTestcase.CellFormatting +=
                dgvTestcase_CellFormatting;
        }

        // GIẢI PHƯƠNG TRÌNH BẰNG GIAO DIỆN CHÍNH
        private void btnGiai_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(txtA.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập hệ số a!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtA.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtB.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập hệ số b!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtB.Focus();
                return;
            }

            string strA = txtA.Text.Trim();
            string strB = txtB.Text.Trim();

            // 2. Kiểm tra phân số
            if (strA.Contains("/"))
            {
                MessageBox.Show(
                    "Hệ số a đang ở dạng phân số.\n" +
                    "Vui lòng quy đổi sang số thập phân.\n" +
                    "Ví dụ: 3/2 nhập thành 1.5",
                    "Thông báo định dạng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtA.SelectAll();
                txtA.Focus();
                return;
            }

            if (strB.Contains("/"))
            {
                MessageBox.Show(
                    "Hệ số b đang ở dạng phân số.\n" +
                    "Vui lòng quy đổi sang số thập phân.\n" +
                    "Ví dụ: 3/2 nhập thành 1.5",
                    "Thông báo định dạng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtB.SelectAll();
                txtB.Focus();
                return;
            }

            // 3. Kiểm tra số
            if (!TryParseSo(strA, out double a))
            {
                MessageBox.Show(
                    "Hệ số a không hợp lệ!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtA.SelectAll();
                txtA.Focus();
                return;
            }

            if (!TryParseSo(strB, out double b))
            {
                MessageBox.Show(
                    "Hệ số b không hợp lệ!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtB.SelectAll();
                txtB.Focus();
                return;
            }

            // 4. Kiểm tra NaN / Infinity
            if (double.IsNaN(a) || double.IsInfinity(a))
            {
                MessageBox.Show(
                    "Hệ số a không hợp lệ!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtA.SelectAll();
                txtA.Focus();
                return;
            }

            if (double.IsNaN(b) || double.IsInfinity(b))
            {
                MessageBox.Show(
                    "Hệ số b không hợp lệ!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtB.SelectAll();
                txtB.Focus();
                return;
            }

            // 5. Gọi Stored Procedure
            try
            {
                txtKetQua.Text =
                    GiaiPhuongTrinhSQL(a, b);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối CSDL:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

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

                // Chỉ load Bài 1
                cmd.Parameters.Add(
                    "@Bai",
                    SqlDbType.VarChar,
                    10
                ).Value = "B1";

                conn.Open();

                using SqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow row =
                        dtTestcase.NewRow();

                    row["MaCase"] =
                        reader["MaCase"]?.ToString()
                        ?? "";

                    row["MoTa"] =
                        reader["MoTa"]?.ToString()
                        ?? "";

                    row["GiaTriA"] =
                        reader["GiaTriA"] == DBNull.Value
                            ? ""
                            : reader["GiaTriA"]?.ToString();

                    row["GiaTriB"] =
                        reader["GiaTriB"] == DBNull.Value
                            ? ""
                            : reader["GiaTriB"]?.ToString();

                    row["LoaiTest"] =
                        reader["LoaiTest"]?.ToString()
                        ?? "";

                    row["KyVong"] =
                        reader["KyVong"]?.ToString()
                        ?? "";

                    row["KetQua"] = "";
                    row["TrangThai"] = "CHƯA CHẠY";

                    dtTestcase.Rows.Add(row);
                }

                lblThongKe.Text =
                    $"Đã load {dtTestcase.Rows.Count} testcase - Sẵn sàng kiểm thử";

                MessageBox.Show(
                    $"Đã load {dtTestcase.Rows.Count} testcase Bài 1.\n\n" +
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
                    "\n\nHãy chạy file 02_Testcase.sql trước.",
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnTestcase_Click(
            object sender,
            EventArgs e)
        {
            using var operation = DoAn.Shared.FormOperation.TryStart(this);
            if (operation is null) return;
            if (dtTestcase.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có testcase.\n" +
                    "Vui lòng bấm Load Testcase trước!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            btnLoadTestcase.Enabled = false;
            btnTestcase.Enabled = false;

            int passCount = 0;
            int failCount = 0;
            int errorCount = 0;
            int daChay = 0;

            try
            {
                using SqlConnection conn =
                    new SqlConnection(strCon);

                conn.Open();

                foreach (DataRow row in dtTestcase.Rows)
                {
                    string aText =
                        row["GiaTriA"]?.ToString()
                        ?? "";

                    string bText =
                        row["GiaTriB"]?.ToString()
                        ?? "";

                    string loaiTest =
                        row["LoaiTest"]?.ToString()
                        ?? "";

                    string kyVong =
                        row["KyVong"]?.ToString()
                        ?? "";

                    string maCase =
                        row["MaCase"]?.ToString()
                        ?? "";

                    row["KetQua"] = "";
                    row["TrangThai"] = "ĐANG CHẠY";

                    dgvTestcase.Refresh();
                    Application.DoEvents();

                    try
                    {
                        string ketQua = loaiTest == "WINFORMS"
                            ? ChayTestWinForms(aText, bText)
                            : ChayTestSQL(conn, aText, bText);

                        row["KetQua"] = ketQua;

                        if (SoSanhKetQua(ketQua, kyVong, maCase))
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

                    lblThongKe.Text =
                        $"Tổng: {dtTestcase.Rows.Count} | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                    dgvTestcase.Refresh();
                    Application.DoEvents();
                }

                txtKetQua.Text =
                    $"Tổng: {daChay} testcase | PASS: {passCount} | FAIL: {failCount} | ERROR: {errorCount}";

                MessageBox.Show(
                    $"Đã chạy xong {daChay} testcase Bài 1.\n\n" +
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
                    "Không chạy được testcase!\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLoadTestcase.Enabled = true;
                btnTestcase.Enabled = true;
            }
        }

        private static bool SoSanhKetQua(string actual, string expected, string maCase = "")
        {
            if (string.IsNullOrWhiteSpace(actual))
                return false;

            actual = actual.Trim();
            expected = (expected ?? "").Trim();

            if (!string.IsNullOrEmpty(expected) && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                return true;

            if (expected.Contains("Vui lòng nhập hệ số a") && actual.Contains("Vui lòng nhập hệ số a")) return true;
            if (expected.Contains("Vui lòng nhập hệ số b") && actual.Contains("Vui lòng nhập hệ số b")) return true;
            if (expected.Contains("Hệ số a không hợp lệ") && actual.Contains("Hệ số a không hợp lệ")) return true;
            if (expected.Contains("Hệ số b không hợp lệ") && actual.Contains("Hệ số b không hợp lệ")) return true;
            if (expected.Contains("phân số") && actual.Contains("phân số")) return true;
            if (expected.Contains("không được để trống") && actual.Contains("không được để trống")) return true;
            if (expected.Contains("vô số nghiệm") && actual.Contains("vô số nghiệm")) return true;
            if (expected.Contains("vô nghiệm") && actual.Contains("vô nghiệm")) return true;

            if (expected.Contains("x = ") && actual.Contains("x = "))
            {
                string expVal = expected.Substring(expected.IndexOf("x = ") + 4).Trim();
                string actVal = actual.Substring(actual.IndexOf("x = ") + 4).Trim();
                if (double.TryParse(expVal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double expNum) &&
                    double.TryParse(actVal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double actNum))
                {
                    return Math.Abs(expNum - actNum) <= 1e-10 * Math.Max(1d, Math.Abs(expNum));
                }
            }

            // Không coi kết quả là PASS chỉ dựa vào mã testcase.

            return false;
        }

        private string ChayTestWinForms(
            string strA,
            string strB)
        {
            // Rỗng
            if (string.IsNullOrWhiteSpace(strA))
            {
                return "Vui lòng nhập hệ số a!";
            }

            if (string.IsNullOrWhiteSpace(strB))
            {
                return "Vui lòng nhập hệ số b!";
            }

            // Lưu giá trị gốc trước khi Trim
            string aText = strA.Trim();
            string bText = strB.Trim();

            // Phân số
            if (aText.Contains("/"))
            {
                return
                    "Hệ số a đang ở dạng phân số. " +
                    "Vui lòng đổi sang số thập phân.";
            }

            if (bText.Contains("/"))
            {
                return
                    "Hệ số b đang ở dạng phân số. " +
                    "Vui lòng đổi sang số thập phân.";
            }

            // Không phải số
            if (!TryParseSo(aText, out double a))
            {
                return
                    "Hệ số a không hợp lệ!";
            }

            if (!TryParseSo(bText, out double b))
            {
                return
                    "Hệ số b không hợp lệ!";
            }

            // NaN
            if (double.IsNaN(a))
            {
                return
                    "Hệ số a không hợp lệ: NaN";
            }

            if (double.IsNaN(b))
            {
                return
                    "Hệ số b không hợp lệ: NaN";
            }

            // Infinity
            if (double.IsInfinity(a))
            {
                return
                    "Hệ số a không hợp lệ: Infinity";
            }

            if (double.IsInfinity(b))
            {
                return
                    "Hệ số b không hợp lệ: Infinity";
            }

            // Nếu dữ liệu hợp lệ thì chạy Stored Procedure thật
            return GiaiPhuongTrinhSQL(a, b);
        }

        // CHẠY 1 TESTCASE SQL
        private string ChayTestSQL(
            SqlConnection conn,
            string strA,
            string strB)
        {
            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_GiaiPTB1",
                    conn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            // -------------------------------------------------
            // A
            // -------------------------------------------------
            if (
                strA.Trim().Equals(
                    "NULL",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                cmd.Parameters.Add(
                    "@a",
                    SqlDbType.Float
                ).Value = DBNull.Value;
            }
            else
            {
                if (!TryParseSo(strA.Trim(), out double a))
                {
                    throw new Exception(
                        $"Giá trị a không hợp lệ: {strA}"
                    );
                }

                cmd.Parameters.Add(
                    "@a",
                    SqlDbType.Float
                ).Value = a;
            }

            // -------------------------------------------------
            // B
            // -------------------------------------------------
            if (
                strB.Trim().Equals(
                    "NULL",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                cmd.Parameters.Add(
                    "@b",
                    SqlDbType.Float
                ).Value = DBNull.Value;
            }
            else
            {
                if (!TryParseSo(strB.Trim(), out double b))
                {
                    throw new Exception(
                        $"Giá trị b không hợp lệ: {strB}"
                    );
                }

                cmd.Parameters.Add(
                    "@b",
                    SqlDbType.Float
                ).Value = b;
            }

            object result =
                cmd.ExecuteScalar();

            return
                result?.ToString()
                ?? "Không có kết quả";
        }

        // =====================================================
        // GỌI STORED PROCEDURE GIẢI PT BẬC 1
        // =====================================================
        private string GiaiPhuongTrinhSQL(
            double a, double b)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_GiaiPTB1",
                    conn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@a",
                SqlDbType.Float
            ).Value = a;

            cmd.Parameters.Add(
                "@b",
                SqlDbType.Float
            ).Value = b;

            object result =
                cmd.ExecuteScalar();

            return
                result?.ToString()
                ?? "Không có kết quả";
        }

        // PARSE SỐ
        // Hỗ trợ dấu chấm và dấu phẩy.
        private bool TryParseSo(
            string value,
            out double number)
        {
            return double.TryParse(
                value.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out number
            );
        }

        // LÀM MỚI
        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            txtA.Clear();
            txtB.Clear();
            txtKetQua.Clear();

            // Xóa luôn danh sách testcase
            dtTestcase.Rows.Clear();

            lblThongKe.Text =
                "Chưa load testcase";

            txtA.Focus();
        }

        // TÔ MÀU TRẠNG THÁI DATAGRIDVIEW
        private void dgvTestcase_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (
                dgvTestcase.Columns[e.ColumnIndex]
                    .DataPropertyName != "TrangThai"
            )
            {
                return;
            }

            string status =
                e.Value?.ToString()
                ?? "";

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

        private void Form1_Load(
            object sender,
            EventArgs e)
        {
            lblThongKe.Text =
                "Chưa load testcase";
        }
    }
}
