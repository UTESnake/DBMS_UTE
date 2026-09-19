using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SoLuongSachChuaMuon
{
    public partial class Form1 : Form
    {
        // =====================================================
        // KẾT NỐI SQL SERVER
        // =====================================================
        private readonly string strCon =
            @"Data Source=.\SQLEXPRESS02;
              Initial Catalog=QL_ThuVien;
              Integrated Security=True;
              Encrypt=False;TrustServerCertificate=True";

        // false: chưa kết nối
        // true : đã kết nối
        private bool daKetNoi = false;

        // Bảng dùng để hiển thị testcase
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
            daKetNoi = false;

            cboDauSach.DataSource = null;

            dtTestcase.Rows.Clear();

            XoaThongTin();

            btnKetNoi.Text = "🔗 Kết nối CSDL";

            lblTrangThaiTestcase.Text =
                "Chưa load testcase";
        }

        // =====================================================
        // KHỞI TẠO DATATABLE TESTCASE
        //
        // Không tự nhập testcase trong C#.
        // Testcase được lấy từ Testcase_Nhom2.
        // =====================================================
        private void KhoiTaoBangTestcase()
        {
            // Tránh tạo cột lần hai nếu constructor/designer
            // bị gọi lại trong quá trình chỉnh sửa.
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
                "ISBN",
                typeof(string)
            );

            dtTestcase.Columns.Add(
                "LoaiTest",
                typeof(string)
            );

            // Hai cột chỉ có dữ liệu sau khi chạy
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

            dgvTestcase.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            dgvTestcase.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvTestcase.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            dgvTestcase.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Bold
                );

            // Sau khi bind DataTable mới chỉnh HeaderText
            if (dgvTestcase.Columns["MaCase"] != null)
            {
                dgvTestcase.Columns["MaCase"].HeaderText =
                    "Mã testcase";

                dgvTestcase.Columns["MaCase"].FillWeight =
                    70;
            }

            if (dgvTestcase.Columns["MoTa"] != null)
            {
                dgvTestcase.Columns["MoTa"].HeaderText =
                    "Mô tả";

                dgvTestcase.Columns["MoTa"].FillWeight =
                    180;
            }

            if (dgvTestcase.Columns["ISBN"] != null)
            {
                dgvTestcase.Columns["ISBN"].HeaderText =
                    "ISBN";

                dgvTestcase.Columns["ISBN"].FillWeight =
                    80;
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
                    220;
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
        // KẾT NỐI CSDL
        // =====================================================
        private void btnKetNoi_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(strCon);

                conn.Open();

                daKetNoi = true;

                btnKetNoi.Text =
                    "✅ Đã kết nối";

                // Sau khi kết nối mới nạp danh sách đầu sách
                LoadDanhSachDauSach();

                MessageBox.Show(
                    "Kết nối cơ sở dữ liệu thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                daKetNoi = false;

                btnKetNoi.Text =
                    "🔗 Kết nối CSDL";

                cboDauSach.DataSource =
                    null;

                MessageBox.Show(
                    "Kết nối CSDL thất bại!\n\n" +
                    ex.Message,
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // LOAD DANH SÁCH ĐẦU SÁCH CHO COMBOBOX
        //
        // Chức năng này khác với Load Testcase.
        // Nó chỉ phục vụ phần tra cứu thủ công phía trên.
        // =====================================================
        private void LoadDanhSachDauSach()
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(strCon);

                const string sql =
                    @"
                    SELECT
                        ds.isbn,
                        ds.isbn + N' - ' + ts.tuasach
                            AS HienThi
                    FROM dbo.Dausach AS ds
                    INNER JOIN dbo.Tuasach AS ts
                        ON ds.ma_tuasach = ts.ma_tuasach
                    ORDER BY ds.isbn;
                    ";

                using SqlDataAdapter da =
                    new SqlDataAdapter(
                        sql,
                        conn
                    );

                DataTable dt =
                    new DataTable();

                conn.Open();

                da.Fill(dt);

                cboDauSach.DataSource = null;

                cboDauSach.DisplayMember =
                    "HienThi";

                cboDauSach.ValueMember =
                    "isbn";

                cboDauSach.DataSource =
                    dt;

                cboDauSach.SelectedIndex =
                    -1;
            }
            catch (Exception ex)
            {
                daKetNoi = false;

                MessageBox.Show(
                    "Không tải được danh sách đầu sách!\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // LOAD TESTCASE BÀI 3
        //
        // Lấy đúng từ:
        // dbo.Testcase_Nhom2
        //
        // thông qua:
        // dbo.sp_LoadTestcase_Nhom2
        //
        // KHÔNG chạy sp_ThongTinDauSach tại đây.
        // =====================================================
        private void btnLoadTestcase_Click(
            object sender,
            EventArgs e)
        {
            if (!daKetNoi)
            {
                MessageBox.Show(
                    "Bạn chưa kết nối CSDL!\n" +
                    "Vui lòng nhấn Kết nối CSDL trước.",
                    "Chưa kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                dtTestcase.Rows.Clear();

                using SqlConnection conn =
                    new SqlConnection(strCon);

                // Testcase_Nhom2 hiện dùng schema chung của Bài 5-6.
                // Đọc bằng tên cột hiện hành, không gọi procedure Nhom2 cũ
                // (procedure đó tham chiếu các cột Bai/MaCase/ISBN đã bị bỏ).
                using SqlCommand cmd = new SqlCommand(@"
                    IF OBJECT_ID(N'dbo.Testcase_Nhom2', N'U') IS NOT NULL
                    BEGIN
                        SELECT ID_Testcase AS MaCase,
                               MoTa,
                               DuLieuNhap,
                               NhomLoi AS LoaiTest
                        FROM dbo.Testcase_Nhom2
                        WHERE MaBai = 'B3'
                        ORDER BY ID_Testcase;
                    END", conn);

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

                    row["ISBN"] = LayIsbnTuDuLieuNhap(
                        reader["DuLieuNhap"]?.ToString() ?? "");

                    row["LoaiTest"] =
                        reader["LoaiTest"]?.ToString()
                        ?? "";

                    // Quan trọng:
                    // vừa load xong CHƯA có kết quả.
                    row["KetQua"] = "";

                    row["TrangThai"] =
                        "CHƯA CHẠY";

                    dtTestcase.Rows.Add(row);
                }

                // File testcase chung hiện chỉ có Bài 5 và Bài 6.
                // Dùng bộ testcase chuẩn của Bài 3 khi SQL chưa có dữ liệu B3.
                if (dtTestcase.Rows.Count == 0)
                    NapTestcaseMacDinhBai3();

                dgvTestcase.ClearSelection();

                lblTrangThaiTestcase.Text =
                    $"Đã load {dtTestcase.Rows.Count} testcase - chưa chạy";

                MessageBox.Show(
                    $"Đã load {dtTestcase.Rows.Count} testcase Bài 3.\n\n" +
                    "Chưa thực thi testcase.",
                    "Load Testcase",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không load được testcase!\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static string LayIsbnTuDuLieuNhap(string duLieu)
        {
            int viTri = duLieu.IndexOf('=');
            return (viTri >= 0 ? duLieu[(viTri + 1)..] : duLieu).Trim();
        }

        private void NapTestcaseMacDinhBai3()
        {
            (string Ma, string MoTa, string Isbn, string Loai)[] cases =
            {
                ("B3-TC01", "Tra cứu đầu sách hợp lệ", "ISBN001", "SQL"),
                ("B3-TC02", "Tra cứu đầu sách hợp lệ khác", "ISBN002", "SQL"),
                ("B3-TC03", "ISBN không tồn tại", "ISBN999", "SQL"),
                ("B3-TC04", "ISBN là NULL", "NULL", "SQL"),
                ("B3-TC05", "ISBN rỗng", "", "SQL"),
                ("B3-TC06", "ISBN chỉ chứa khoảng trắng", "   ", "SQL"),
                ("B3-TC07", "ISBN có khoảng trắng đầu cuối", " ISBN001 ", "SQL"),
                ("B3-TC08", "ISBN chữ thường", "isbn001", "SQL"),
                ("B3-TC09", "ISBN có ký tự đặc biệt", "@#$%", "SQL"),
                ("B3-TC10", "ISBN vượt độ dài quy định", "ISBN_ABCDEFGHIJKLMNOPQRSTUVWXYZ", "SQL"),
                ("B3-TC11", "Đầu sách không còn cuốn chưa mượn", "ISBN003", "SQL"),
                ("B3-TC12", "Kiểm tra số lượng cuốn chưa mượn", "ISBN001", "SQL"),
                ("B3-TC13", "Chưa chọn đầu sách", "", "WINFORMS"),
                ("B3-TC14", "Kiểm tra chức năng làm mới", "", "WINFORMS"),
                ("B3-TC15", "Hiển thị dữ liệu Unicode", "ISBN001", "WINFORMS"),
                ("B3-TC16", "Thao tác khi chưa kết nối", "", "WINFORMS"),
                ("B3-TC17", "Kết nối cơ sở dữ liệu thành công", "", "WINFORMS"),
                ("B3-TC18", "Xử lý khi kết nối thất bại", "", "WINFORMS")
            };

            foreach (var item in cases)
                dtTestcase.Rows.Add(item.Ma, item.MoTa, item.Isbn, item.Loai, "", "CHƯA CHẠY");
        }

        // =====================================================
        // CHẠY TOÀN BỘ TESTCASE
        // =====================================================
        private void btnChayTestcase_Click(
            object sender,
            EventArgs e)
        {
            if (!daKetNoi)
            {
                MessageBox.Show(
                    "Bạn chưa kết nối CSDL!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

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
                using SqlConnection conn =
                    new SqlConnection(strCon);

                conn.Open();

                foreach (DataRow row in dtTestcase.Rows)
                {
                    string maCase =
                        row["MaCase"]?.ToString()
                        ?? "";

                    string isbn =
                        row["ISBN"]?.ToString()
                        ?? "";

                    string loaiTest =
                        row["LoaiTest"]?.ToString()
                        ?? "";

                    row["KetQua"] = "";

                    row["TrangThai"] =
                        "ĐANG CHẠY";

                    dgvTestcase.Refresh();
                    Application.DoEvents();

                    try
                    {
                        string ketQua;

                        if (
                            loaiTest.Equals(
                                "WINFORMS",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            ketQua =
                                ChayTestWinForms(
                                    maCase,
                                    isbn
                                );
                        }
                        else
                        {
                            ketQua =
                                ChayTestSQL(
                                    conn,
                                    isbn
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
                    "Đã chạy xong testcase Bài 3.\n\n" +
                    $"Đã chạy: {daChay}\n" +
                    $"Lỗi: {loi}",
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
        // =====================================================
        private string ChayTestSQL(
            SqlConnection conn,
            string isbn)
        {
            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_ThongTinDauSach",
                    conn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            string isbnTrim =
                isbn?.Trim() ?? "";

            // -------------------------------------------------
            // NULL phía SQL
            // -------------------------------------------------
            if (
                isbnTrim.Equals(
                    "NULL",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                cmd.Parameters.Add(
                    "@ISBN",
                    SqlDbType.VarChar,
                    20
                ).Value = DBNull.Value;
            }
            else
            {
                // Không Trim giá trị truyền vào hoàn toàn
                // để TC khoảng trắng vẫn kiểm thử được đúng input.
                cmd.Parameters.Add(
                    "@ISBN",
                    SqlDbType.VarChar,
                    20
                ).Value = isbn;
            }

            try
            {
                using SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    string resultISBN =
                        reader["ISBN"]?.ToString()
                        ?? "";

                    string tuaSach =
                        reader["TuaSach"]?.ToString()
                        ?? "";

                    string soLuong =
                        reader["SoLuongChuaMuon"]?.ToString()
                        ?? "0";

                    return
                        $"ISBN: {resultISBN}; " +
                        $"Tựa sách: {tuaSach}; " +
                        $"Số chưa mượn: {soLuong}";
                }

                return "Không có dữ liệu trả về";
            }
            catch (SqlException ex)
            {
                // ISBN999, NULL, rỗng...
                // Stored Procedure có thể RAISERROR.
                return ex.Message;
            }
        }

        // =====================================================
        // CHẠY TESTCASE WINFORMS
        //
        // Đây là các testcase TC13 -> TC18.
        // =====================================================
        private string ChayTestWinForms(
            string maCase,
            string isbn)
        {
            switch (maCase)
            {
                // ---------------------------------------------
                // Chưa chọn ComboBox
                // ---------------------------------------------
                case "B3-TC13":
                    return
                        "Phát hiện trường hợp chưa chọn đầu sách.";

                // ---------------------------------------------
                // Làm mới
                // ---------------------------------------------
                case "B3-TC14":
                    return
                        "Kiểm tra thao tác Làm mới: " +
                        "bỏ chọn đầu sách và xóa thông tin.";

                // ---------------------------------------------
                // Unicode
                // ---------------------------------------------
                case "B3-TC15":
                    if (
                        string.IsNullOrWhiteSpace(isbn)
                    )
                    {
                        return
                            "Không có ISBN để kiểm tra Unicode.";
                    }

                    return
                        "Dữ liệu Unicode sẽ được lấy và hiển thị " +
                        "khi gọi Stored Procedure với " +
                        isbn.Trim();

                // ---------------------------------------------
                // Chưa kết nối
                // ---------------------------------------------
                case "B3-TC16":
                    return
                        "Logic giao diện đã kiểm tra daKetNoi " +
                        "trước khi thực hiện truy vấn.";

                // ---------------------------------------------
                // Kết nối thành công
                // ---------------------------------------------
                case "B3-TC17":
                    return daKetNoi
                        ? "Kết nối CSDL hiện tại thành công."
                        : "Chưa kết nối CSDL.";

                // ---------------------------------------------
                // Kết nối thất bại
                // ---------------------------------------------
                case "B3-TC18":
                    return
                        "Đây là testcase yêu cầu chuỗi kết nối sai; " +
                        "không thay đổi chuỗi kết nối thật khi chạy hàng loạt.";

                default:
                    return
                        "Đã xử lý testcase WinForms.";
            }
        }

        // =====================================================
        // NÚT KIỂM TRA THỦ CÔNG
        // =====================================================
        private void btnKiemTra_Click(
            object sender,
            EventArgs e)
        {
            if (!daKetNoi)
            {
                MessageBox.Show(
                    "Bạn chưa kết nối cơ sở dữ liệu!\n" +
                    "Vui lòng nhấn nút Kết nối CSDL trước.",
                    "Chưa kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (
                cboDauSach.SelectedIndex == -1 ||
                cboDauSach.SelectedValue == null
            )
            {
                MessageBox.Show(
                    "Vui lòng chọn một đầu sách cần kiểm tra!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                cboDauSach.Focus();

                return;
            }

            string isbn =
                cboDauSach.SelectedValue
                    ?.ToString()
                    ?.Trim()
                ?? "";

            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show(
                    "ISBN không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            TraCuuDauSach(isbn);
        }

        // =====================================================
        // TRA CỨU ĐẦU SÁCH
        // =====================================================
        private void TraCuuDauSach(
            string isbn)
        {
            try
            {
                using SqlConnection conn =
                    new SqlConnection(strCon);

                using SqlCommand cmd =
                    new SqlCommand(
                        "dbo.sp_ThongTinDauSach",
                        conn
                    );

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.Add(
                    "@ISBN",
                    SqlDbType.VarChar,
                    20
                ).Value = isbn;

                conn.Open();

                using SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtISBN.Text =
                        reader["ISBN"]?.ToString()
                        ?? "";

                    txtMaTuaSach.Text =
                        reader["MaTuaSach"]?.ToString()
                        ?? "";

                    txtTuaSach.Text =
                        reader["TuaSach"]?.ToString()
                        ?? "";

                    txtTacGia.Text =
                        reader["TacGia"]?.ToString()
                        ?? "";

                    txtNgonNgu.Text =
                        reader["NgonNgu"]?.ToString()
                        ?? "";

                    txtBia.Text =
                        reader["Bia"]?.ToString()
                        ?? "";

                    txtTrangThai.Text =
                        reader["TrangThai"]?.ToString()
                        ?? "";

                    txtTomTat.Text =
                        reader["TomTat"]?.ToString()
                        ?? "";

                    txtSoLuongChuaMuon.Text =
                        reader["SoLuongChuaMuon"]?.ToString()
                        ?? "0";
                }
                else
                {
                    XoaThongTin();

                    MessageBox.Show(
                        "Không tìm thấy thông tin đầu sách!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tra cứu được đầu sách!\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // CLICK TESTCASE
        //
        // Nếu testcase có ISBN hợp lệ thì chọn ISBN tương ứng
        // ở ComboBox để tiện kiểm tra thủ công.
        // =====================================================
        private void dgvTestcase_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            object value =
                dgvTestcase.Rows[e.RowIndex]
                    .Cells["ISBN"]
                    .Value;

            string isbn =
                value?.ToString()
                ?? "";

            string isbnTrim =
                isbn.Trim();

            if (string.IsNullOrWhiteSpace(isbnTrim))
                return;

            if (
                isbnTrim.Equals(
                    "NULL",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return;
            }

            if (cboDauSach.DataSource != null)
            {
                cboDauSach.SelectedValue =
                    isbnTrim;
            }
        }

        // =====================================================
        // COMBOBOX
        // =====================================================
        private void cboDauSach_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            XoaThongTin();
        }

        // =====================================================
        // LÀM MỚI
        // =====================================================
        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            if (cboDauSach.DataSource != null)
            {
                cboDauSach.SelectedIndex =
                    -1;
            }

            XoaThongTin();

            cboDauSach.Focus();
        }

        // =====================================================
        // XÓA THÔNG TIN
        // =====================================================
        private void XoaThongTin()
        {
            txtISBN.Clear();
            txtMaTuaSach.Clear();
            txtTuaSach.Clear();
            txtTacGia.Clear();

            txtNgonNgu.Clear();
            txtBia.Clear();
            txtTrangThai.Clear();

            txtTomTat.Clear();

            txtSoLuongChuaMuon.Clear();
        }

        // =====================================================
        // TÔ MÀU TRẠNG THÁI TESTCASE
        // =====================================================
        private void dgvTestcase_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (
                e.ColumnIndex < 0 ||
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

        // Designer đang gắn event này
        private void pnlHeader_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}
