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
        //
        // Testcase KHÔNG được tự nhập trong C#.
        // Dữ liệu sẽ được load từ:
        // dbo.Testcase_Nhom1
        //
        // thông qua:
        // dbo.sp_LoadTestcase_Nhom1
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
            // Tránh tạo cột trùng
            if (dtTestcase.Columns.Count > 0)
                return;

            dtTestcase.Columns.Add(
                "MaCase",
                typeof(string)
            );

            dtTestcase.Columns.Add(
                "MoTa",
                typeof(string)
            );

            dtTestcase.Columns.Add(
                "NamSinh",
                typeof(string)
            );

            dtTestcase.Columns.Add(
                "LoaiTest",
                typeof(string)
            );

            // Hai cột này KHÔNG có dữ liệu khi Load
            // Chỉ được điền sau khi bấm Chạy Testcase
            dtTestcase.Columns.Add(
                "KetQua",
                typeof(string)
            );

            dtTestcase.Columns.Add(
                "TrangThai",
                typeof(string)
            );

            dgvTestcase.DataSource =
                dtTestcase;
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
                new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Bold
                );

            if (dgvTestcase.Columns["MaCase"] != null)
            {
                dgvTestcase.Columns["MaCase"].HeaderText =
                    "Mã testcase";

                dgvTestcase.Columns["MaCase"].FillWeight =
                    75;
            }

            if (dgvTestcase.Columns["MoTa"] != null)
            {
                dgvTestcase.Columns["MoTa"].HeaderText =
                    "Mô tả";

                dgvTestcase.Columns["MoTa"].FillWeight =
                    170;
            }

            if (dgvTestcase.Columns["NamSinh"] != null)
            {
                dgvTestcase.Columns["NamSinh"].HeaderText =
                    "Ngày sinh";

                dgvTestcase.Columns["NamSinh"].FillWeight =
                    85;
            }

            if (dgvTestcase.Columns["LoaiTest"] != null)
            {
                dgvTestcase.Columns["LoaiTest"].HeaderText =
                    "Loại test";

                dgvTestcase.Columns["LoaiTest"].FillWeight =
                    75;
            }

            if (dgvTestcase.Columns["KetQua"] != null)
            {
                dgvTestcase.Columns["KetQua"].HeaderText =
                    "Kết quả sau khi chạy";

                dgvTestcase.Columns["KetQua"].FillWeight =
                    200;
            }

            if (dgvTestcase.Columns["TrangThai"] != null)
            {
                dgvTestcase.Columns["TrangThai"].HeaderText =
                    "Trạng thái";

                dgvTestcase.Columns["TrangThai"].FillWeight =
                    90;
            }

            dgvTestcase.CellFormatting -=
                dgvTestcase_CellFormatting;

            dgvTestcase.CellFormatting +=
                dgvTestcase_CellFormatting;
        }

        // =====================================================
        // NÚT TÍNH TUỔI
        // =====================================================
        private void btnTinhTuoi_Click(
            object sender,
            EventArgs e)
        {
            txtKetQua.Clear();

            // -------------------------------------------------
            // 1. KIỂM TRA BỎ TRỐNG
            // -------------------------------------------------
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

            string input =
                txtNamSinh.Text.Trim();

            // -------------------------------------------------
            // 2. KIỂM TRA NGÀY/THÁNG/NĂM
            // -------------------------------------------------
            if (!ThuChuyenNgaySinh(input, out DateTime ngaySinh))
            {
                MessageBox.Show(
                    "Ngày sinh không hợp lệ!\n" +
                    "Vui lòng nhập đủ ngày/tháng/năm, ví dụ: 15/03/2005.",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtNamSinh.SelectAll();
                txtNamSinh.Focus();

                return;
            }

            // -------------------------------------------------
            // 3. GỌI FUNCTION SQL
            // -------------------------------------------------
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
        // LOAD TESTCASE BÀI 4
        //
        // Gọi:
        // dbo.sp_LoadTestcase_Nhom1
        // @Bai = 'B4'
        //
        // CHỈ LOAD DỮ LIỆU.
        // KHÔNG CHẠY TEST.
        // KHÔNG CÓ KẾT QUẢ SẴN.
        // =====================================================
        private void btnLoadTestcase_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                // Load testcase không được giữ kết quả tính tuổi của lần trước.
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
                        reader["MaCase"]?.ToString()
                        ?? "";

                    row["MoTa"] =
                        reader["MoTa"]?.ToString()
                        ?? "";

                    row["NamSinh"] =
                        reader["NamSinh"] == DBNull.Value
                            ? ""
                            : reader["NamSinh"]?.ToString() ?? "";

                    row["LoaiTest"] =
                        reader["LoaiTest"]?.ToString()
                        ?? "";

                    // -----------------------------------------
                    // Load xong CHƯA CÓ KẾT QUẢ
                    // -----------------------------------------
                    row["KetQua"] = "";

                    row["TrangThai"] =
                        "CHƯA CHẠY";

                    dtTestcase.Rows.Add(row);
                }

                dgvTestcase.ClearSelection();

                lblTrangThaiTestcase.Text =
                    $"Đã load {dtTestcase.Rows.Count} testcase - chưa chạy";

                MessageBox.Show(
                    $"Đã load {dtTestcase.Rows.Count} testcase Bài 4.\n\n" +
                    "Các testcase chưa được thực thi.",
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
                    "\n\nHãy chạy file 02_Testcase.sql " +
                    "của Nhom_1_CSDL_ToanHoc trước.",
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
            // -------------------------------------------------
            // PHẢI LOAD TRƯỚC
            // -------------------------------------------------
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
            int loi = 0;

            try
            {
                foreach (DataRow row in dtTestcase.Rows)
                {
                    string namSinhText =
                        row["NamSinh"]?.ToString()
                        ?? "";

                    string loaiTest =
                        row["LoaiTest"]?.ToString()
                        ?? "";

                    // -----------------------------------------
                    // BẮT ĐẦU CHẠY
                    // -----------------------------------------
                    row["KetQua"] = "";

                    row["TrangThai"] =
                        "ĐANG CHẠY";

                    dgvTestcase.Refresh();

                    Application.DoEvents();

                    try
                    {
                        string ketQua;

                        // =====================================
                        // TESTCASE WINFORMS
                        // =====================================
                        if (
                            loaiTest.Equals(
                                "WINFORMS",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            ketQua =
                                ChayTestWinForms(
                                    namSinhText
                                );
                        }

                        // =====================================
                        // TESTCASE SQL
                        // =====================================
                        else
                        {
                            ketQua =
                                ChayTestSQL(
                                    namSinhText
                                );
                        }

                        row["KetQua"] =
                            ketQua;

                        row["TrangThai"] =
                            "ĐÃ CHẠY";

                        daChay++;
                    }
                    catch (Exception ex)
                    {
                        row["KetQua"] =
                            ex.Message;

                        row["TrangThai"] =
                            "LỖI";

                        loi++;
                    }

                    lblTrangThaiTestcase.Text =
                        $"Đã chạy: {daChay + loi}/{dtTestcase.Rows.Count}" +
                        $" | Lỗi: {loi}";

                    dgvTestcase.Refresh();

                    Application.DoEvents();
                }

                MessageBox.Show(
                    "Đã chạy xong testcase Bài 4.\n\n" +
                    $"Đã chạy thành công: {daChay}\n" +
                    $"Lỗi hệ thống: {loi}",
                    "Hoàn thành",
                    MessageBoxButtons.OK,
                    loi == 0
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
                btnChayTestcase.Enabled = true;
            }
        }

        // =====================================================
        // CHẠY TESTCASE SQL
        //
        // Các testcase:
        //
        // B4-TC01 = 15/03/2006
        // B4-TC02 = HOMNAY
        // B4-TC03 = NGAYMAI
        // B4-TC04 = 01/01/2000
        // B4-TC05 = 31/12/2000
        // B4-TC06 = NULL
        //
        // Không hard-code KẾT QUẢ.
        // Chỉ chuyển input thành giá trị SQL.
        // Function SQL tự trả kết quả.
        // =====================================================
        private string ChayTestSQL(
            string namSinhText)
        {
            string value =
                namSinhText.Trim();

            DateTime? ngaySinh;

            // -------------------------------------------------
            // NULL
            // -------------------------------------------------
            if (
                value.Equals(
                    "NULL",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                ngaySinh = null;
            }

            // -------------------------------------------------
            // NGÀY HIỆN TẠI
            // -------------------------------------------------
            else if (
                value.Equals(
                    "HOMNAY",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                ngaySinh = DateTime.Today;
            }

            // -------------------------------------------------
            // NGÀY TƯƠNG LAI
            // -------------------------------------------------
            else if (
                value.Equals(
                    "NGAYMAI",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                ngaySinh = DateTime.Today.AddDays(1);
            }

            // -------------------------------------------------
            // CÁC GIÁ TRỊ NGÀY
            // -------------------------------------------------
            else
            {
                if (!ThuChuyenNgaySinh(value, out DateTime ngay))
                {
                    throw new Exception(
                        "Không chuyển được ngày sinh theo định dạng dd/MM/yyyy hoặc yyyy-MM-dd: " +
                        namSinhText
                    );
                }

                ngaySinh = ngay;
            }

            // Function SQL tự quyết định kết quả.
            return GoiFunctionTinhTuoi(
                ngaySinh
            );
        }

        // =====================================================
        // CHẠY TESTCASE WINFORMS
        //
        // B4-TC07 = rỗng
        // B4-TC08 = abc
        // B4-TC09 = 31/02/2005
        // B4-TC10 = " 15/03/2006 "
        //
        // Không so sánh với kết quả mong đợi.
        // Chỉ chạy đúng logic nhập liệu hiện tại.
        // =====================================================
        private string ChayTestWinForms(
            string namSinhText)
        {
            // -------------------------------------------------
            // KIỂM TRA RỖNG
            // -------------------------------------------------
            if (string.IsNullOrWhiteSpace(namSinhText))
            {
                return
                    "Vui lòng nhập ngày sinh!";
            }

            string input =
                namSinhText.Trim();

            // -------------------------------------------------
            // KHÔNG PHẢI NGÀY HỢP LỆ
            // -------------------------------------------------
            if (!ThuChuyenNgaySinh(input, out DateTime ngaySinh))
            {
                return
                    "Ngày sinh không hợp lệ! " +
                    "Vui lòng nhập đủ ngày/tháng/năm.";
            }

            // -------------------------------------------------
            // Nếu hợp lệ, ví dụ " 15/03/2006 "
            // sau Trim sẽ gọi Function thật.
            // -------------------------------------------------
            return GoiFunctionTinhTuoi(
                ngaySinh
            );
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
        // GỌI FUNCTION SQL
        //
        // Hỗ trợ DateTime? để testcase NULL chạy thật.
        // =====================================================
        private string GoiFunctionTinhTuoi(
            DateTime? ngaySinh)
        {
            using SqlConnection conn =
                new SqlConnection(strCon);

            const string sqlQuery =
                "SELECT dbo.fn_TinhTuoi(@NgaySinh)";

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

            object result =
                cmd.ExecuteScalar();

            if (
                result != null &&
                result != DBNull.Value
            )
            {
                return "Tuổi: " + (result.ToString() ?? "");
            }

            if (!ngaySinh.HasValue)
                return "Ngày sinh không được để trống";

            if (ngaySinh.Value.Date > DateTime.Today)
                return "Ngày sinh không được lớn hơn ngày hiện tại";

            return "Không nhận được kết quả từ cơ sở dữ liệu.";
        }

        // =====================================================
        // CLICK VÀO TESTCASE
        //
        // Đưa input lên ô ngày sinh để xem.
        // =====================================================
        private void dgvTestcase_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string namSinh =
                dgvTestcase.Rows[e.RowIndex]
                    .Cells["NamSinh"]
                    .Value?.ToString()
                ?? "";

            txtNamSinh.Text =
                namSinh;

            txtKetQua.Clear();
        }

        // =====================================================
        // TÔ MÀU CỘT TRẠNG THÁI
        // =====================================================
        private void dgvTestcase_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            if (
                dgvTestcase.Columns[e.ColumnIndex]
                    .DataPropertyName != "TrangThai"
            )
            {
                return;
            }

            string trangThai =
                e.Value?.ToString()
                ?? "";

            switch (trangThai)
            {
                case "CHƯA CHẠY":

                    e.CellStyle.ForeColor =
                        Color.Gray;

                    break;

                case "ĐANG CHẠY":

                    e.CellStyle.ForeColor =
                        Color.Blue;

                    e.CellStyle.Font =
                        new Font(
                            dgvTestcase.Font,
                            FontStyle.Bold
                        );

                    break;

                case "ĐÃ CHẠY":

                    e.CellStyle.ForeColor =
                        Color.Green;

                    e.CellStyle.Font =
                        new Font(
                            dgvTestcase.Font,
                            FontStyle.Bold
                        );

                    break;

                case "LỖI":

                    e.CellStyle.ForeColor =
                        Color.Red;

                    e.CellStyle.Font =
                        new Font(
                            dgvTestcase.Font,
                            FontStyle.Bold
                        );

                    break;
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

            // Xóa danh sách testcase đang hiển thị
            dtTestcase.Rows.Clear();

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";

            txtNamSinh.Focus();
        }
    }
}
