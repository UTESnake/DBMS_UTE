using System;
using System.Data;
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
                loi = $"{tenHeSo} bị NULL.";
                return false;
            }

            string str = input.Trim();

            // Chuỗi NULL dùng cho testcase SQL
            if (string.Equals(
                str,
                "NULL",
                StringComparison.OrdinalIgnoreCase))
            {
                loi = $"{tenHeSo} có giá trị NULL.";
                return false;
            }

            // Rỗng
            if (string.IsNullOrWhiteSpace(str))
            {
                loi = $"Bỏ trống {tenHeSo}.";
                return false;
            }

            // Phân số
            if (str.Contains("/"))
            {
                loi = $"{tenHeSo} đang ở dạng phân số.";
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
                loi = $"{tenHeSo} không phải số thực hợp lệ.";
                return false;
            }

            if (double.IsNaN(value) ||
                double.IsInfinity(value))
            {
                loi = $"{tenHeSo} là giá trị đặc biệt không hợp lệ.";
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
        // TESTCASE NULL PHÍA SQL
        // ============================================================
        private string GiaiPTB2_SQL_NullA(
            string bText,
            string cText)
        {
            if (!TryParseHeSo(
                    bText,
                    "b",
                    out double b,
                    out string loiB))
            {
                return loiB;
            }

            if (!TryParseHeSo(
                    cText,
                    "c",
                    out double c,
                    out string loiC))
            {
                return loiC;
            }

            using (SqlConnection conn =
                   new SqlConnection(strCon))
            {
                string sql =
                    "SELECT dbo.fn_GiaiPTB2(NULL, @b, @c)";

                using (SqlCommand cmd =
                       new SqlCommand(sql, conn))
                {
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
                        return "NULL";
                    }

                    return result.ToString() ?? "";
                }
            }
        }

        // ============================================================
        // NÚT GIẢI PHƯƠNG TRÌNH
        // ============================================================
        private void btnGiai_Click(
            object sender,
            EventArgs e)
        {
            // A
            if (!TryParseHeSo(
                    txtA.Text,
                    "Hệ số a",
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

            // B
            if (!TryParseHeSo(
                    txtB.Text,
                    "Hệ số b",
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

            // C
            if (!TryParseHeSo(
                    txtC.Text,
                    "Hệ số c",
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

                            // Kết quả chỉ sinh ra khi chạy testcase
                            if (!dt.Columns.Contains(
                                    "KetQuaThucTe"))
                            {
                                dt.Columns.Add(
                                    "KetQuaThucTe",
                                    typeof(string));
                            }

                            dgvTestcase.DataSource = dt;
                        }
                    }
                }

                DinhDangDataGridView();

                MessageBox.Show(
                    "Đã load testcase Bài 2 từ CSDL.",
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

                    // ============================================
                    // NULL phía SQL
                    // ============================================
                    if (string.Equals(
                            aText.Trim(),
                            "NULL",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        row.Cells["KetQuaThucTe"].Value =
                            GiaiPTB2_SQL_NullA(
                                bText,
                                cText);

                        continue;
                    }

                    // ============================================
                    // Kiểm tra a
                    // ============================================
                    if (!TryParseHeSo(
                            aText,
                            "a",
                            out double a,
                            out string loiA))
                    {
                        row.Cells["KetQuaThucTe"].Value =
                            loiA;

                        continue;
                    }

                    // ============================================
                    // Kiểm tra b
                    // ============================================
                    if (!TryParseHeSo(
                            bText,
                            "b",
                            out double b,
                            out string loiB))
                    {
                        row.Cells["KetQuaThucTe"].Value =
                            loiB;

                        continue;
                    }

                    // ============================================
                    // Kiểm tra c
                    // ============================================
                    if (!TryParseHeSo(
                            cText,
                            "c",
                            out double c,
                            out string loiC))
                    {
                        row.Cells["KetQuaThucTe"].Value =
                            loiC;

                        continue;
                    }

                    // ============================================
                    // Gọi Function SQL
                    // ============================================
                    string ketQua =
                        GiaiPTB2_SQL(a, b, c);

                    row.Cells["KetQuaThucTe"].Value =
                        ketQua;
                }

                MessageBox.Show(
                    "Đã chạy xong toàn bộ testcase Bài 2.",
                    "Hoàn tất",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Lỗi SQL khi chạy testcase:\n" +
                    ex.Message,
                    "Lỗi SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
        }

        // ============================================================
        // ĐỊNH DẠNG DATAGRIDVIEW
        // ============================================================
        private void DinhDangDataGridView()
        {
            if (dgvTestcase.Columns.Count == 0)
                return;

            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None;

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

                dgvTestcase.Columns["MaCase"].Width =
                    100;
            }

            // Ẩn cột bài
            if (dgvTestcase.Columns["Bai"] != null)
            {
                dgvTestcase.Columns["Bai"].Visible =
                    false;
            }

            // Mô tả
            if (dgvTestcase.Columns["MoTa"] != null)
            {
                dgvTestcase.Columns["MoTa"].HeaderText =
                    "Mô tả";

                dgvTestcase.Columns["MoTa"].Width =
                    210;
            }

            // A
            if (dgvTestcase.Columns["GiaTriA"] != null)
            {
                dgvTestcase.Columns["GiaTriA"].HeaderText =
                    "a";

                dgvTestcase.Columns["GiaTriA"].Width =
                    95;
            }

            // B
            if (dgvTestcase.Columns["GiaTriB"] != null)
            {
                dgvTestcase.Columns["GiaTriB"].HeaderText =
                    "b";

                dgvTestcase.Columns["GiaTriB"].Width =
                    95;
            }

            // C
            if (dgvTestcase.Columns["GiaTriC"] != null)
            {
                dgvTestcase.Columns["GiaTriC"].HeaderText =
                    "c";

                dgvTestcase.Columns["GiaTriC"].Width =
                    95;
            }

            // Ẩn năm sinh
            if (dgvTestcase.Columns["NamSinh"] != null)
            {
                dgvTestcase.Columns["NamSinh"].Visible =
                    false;
            }

            // Loại test
            if (dgvTestcase.Columns["LoaiTest"] != null)
            {
                dgvTestcase.Columns["LoaiTest"].HeaderText =
                    "Loại test";

                dgvTestcase.Columns["LoaiTest"].Width =
                    100;
            }

            // Kết quả
            if (dgvTestcase.Columns["KetQuaThucTe"] != null)
            {
                dgvTestcase.Columns["KetQuaThucTe"].HeaderText =
                    "Kết quả thực tế";

                dgvTestcase.Columns["KetQuaThucTe"].Width =
                    380;

                dgvTestcase.Columns["KetQuaThucTe"]
                    .DefaultCellStyle.WrapMode =
                    DataGridViewTriState.True;
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
        }
    }
}
