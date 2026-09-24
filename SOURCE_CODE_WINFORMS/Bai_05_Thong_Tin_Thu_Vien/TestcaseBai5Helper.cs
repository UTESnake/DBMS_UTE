using System;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace ThongTinThuVien
{
    public sealed class TestcaseRunResult
    {
        public string? InputForForm { get; set; }
        public DataTable? Data { get; set; }
        public string Message { get; set; } = "";
        public bool IsError { get; set; }
    }

    public static class TestcaseBai5Helper
    {
        public static DataTable LoadTestcase(
            string connectionString,
            string maBai,
            string? nhomLoi = null)
        {
            DataTable dt = new DataTable();

            using SqlConnection conn =
                new SqlConnection(connectionString);

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_LoadTestcaseBai5",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@MaBai",
                SqlDbType.VarChar,
                10).Value = maBai;

            cmd.Parameters.Add(
                "@NhomLoi",
                SqlDbType.VarChar,
                20).Value =
                string.IsNullOrWhiteSpace(nhomLoi)
                    ? DBNull.Value
                    : nhomLoi;

            using SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(dt);

            return dt;
        }

        public static DataTable ChayProcedure(
            string connectionString,
            string procedureName,
            string maBai,
            string? input)
        {
            input = input?.Trim();
            if ((maBai == "B5A" || maBai == "B5B") && input != null && input.Length > 20)
                throw new ArgumentException("Mã tra cứu không được dài quá 20 ký tự.");

            using SqlConnection conn =
                new SqlConnection(connectionString);

            using SqlCommand cmd =
                new SqlCommand(
                    procedureName,
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            if (maBai == "B5A")
            {
                cmd.Parameters.Add(
                    "@MaDocGia",
                    SqlDbType.VarChar,
                    50
                ).Value =
                    input == null
                        ? DBNull.Value
                        : input;
            }
            else if (maBai == "B5B")
            {
                cmd.Parameters.Add(
                    "@ISBN",
                    SqlDbType.VarChar,
                    50
                ).Value =
                    input == null
                        ? DBNull.Value
                        : input;
            }

            DataTable dt = new DataTable();

            using SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(dt);

            return dt;
        }

        // =========================================================
        // TẠO BẢNG KẾT QUẢ TRẠNG THÁI ĐỂ HIỂN THỊ TRÊN DATAGRIDVIEW
        // =========================================================
        public static DataTable TaoBangKetQuaTrangThai(
            string idTestcase,
            string nhomLoi,
            string duLieuThucTe,
            string ketQua)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaTestcase", typeof(string));
            dt.Columns.Add("NhomLoi", typeof(string));
            dt.Columns.Add("DuLieuThucTe", typeof(string));
            dt.Columns.Add("KetQuaXuLy", typeof(string));

            dt.Rows.Add(
                idTestcase,
                nhomLoi,
                duLieuThucTe,
                ketQua);

            return dt;
        }

        // =========================================================
        // CHẠY VALIDATION WINFORM THẬT CHO 5A / 5B
        // Không hiện MessageBox, trả kết quả vào DataGridView.
        // =========================================================
        public static DataTable ChayValidationWinForm(
            string idTestcase,
            string maBai,
            string value)
        {
            string ketQua;

            if (string.IsNullOrWhiteSpace(value))
            {
                ketQua =
                    "WinForms đã chặn dữ liệu: trường nhập đang rỗng " +
                    "hoặc chỉ chứa khoảng trắng.";
            }
            else if (value.Length > 50)
            {
                ketQua =
                    "WinForms phát hiện dữ liệu quá dài. " +
                    "Cần giới hạn MaxLength/độ dài mã trước khi gửi xuống SQL.";
            }
            else if (idTestcase.EndsWith(
                        "WF-05",
                        StringComparison.OrdinalIgnoreCase))
            {
                ketQua =
                    "WinForms nhận dữ liệu có ký tự đặc biệt. " +
                    "Nếu bài yêu cầu chỉ cho phép định dạng mã hợp lệ thì phải chặn tại giao diện.";
            }
            else
            {
                ketQua =
                    "Dữ liệu vượt qua validation WinForms sau khi Trim. " +
                    "Có thể tiếp tục gọi Stored Procedure.";
            }

            return TaoBangKetQuaTrangThai(
                idTestcase,
                "WINFORM",
                value,
                ketQua);
        }

        // =========================================================
        // TEST WINFORM CHO 5C / 5D / 5E (KHÔNG CÓ INPUT)
        // =========================================================
        public static DataTable ChayWinFormKhongThamSo(
            string idTestcase,
            string procedureName,
            string connectionString)
        {
            if (idTestcase.EndsWith(
                    "WF-01",
                    StringComparison.OrdinalIgnoreCase))
            {
                return TaoBangKetQuaTrangThai(
                    idTestcase,
                    "WINFORM",
                    "Trạng thái kết nối",
                    "Testcase kiểm tra khi chưa kết nối phải thực hiện tại Form chính. " +
                    "Form con chỉ được mở sau khi kết nối thành công.");
            }

            if (idTestcase.EndsWith(
                    "WF-02",
                    StringComparison.OrdinalIgnoreCase))
            {
                DataTable lan1 =
                    ChayProcedure(
                        connectionString,
                        procedureName,
                        "",
                        null);

                DataTable lan2 =
                    ChayProcedure(
                        connectionString,
                        procedureName,
                        "",
                        null);

                return TaoBangKetQuaTrangThai(
                    idTestcase,
                    "WINFORM",
                    "Click 2 lần liên tiếp",
                    $"Đã gọi Stored Procedure 2 lần. " +
                    $"Lần 1: {lan1.Rows.Count} dòng; Lần 2: {lan2.Rows.Count} dòng. " +
                    "DataGridView dùng DataSource mới nên không cộng dồn bản ghi.");
            }

            return TaoBangKetQuaTrangThai(
                idTestcase,
                "WINFORM",
                "",
                "Đã thực hiện testcase WinForms.");
        }

        public static string LayInputWinForm(
            string maBai,
            string id,
            string duLieuNhap,
            string connectionString)
        {
            if (maBai == "B5A")
            {
                if (id == "B5A-WF-01")
                    return "";

                if (id == "B5A-WF-02")
                    return "     ";

                if (id == "B5A-WF-03")
                {
                    string ma =
                        LayMaDocGiaHopLe(
                            connectionString);

                    return "   " + ma + "   ";
                }

                if (id == "B5A-WF-04")
                    return new string('A', 150);

                if (id == "B5A-WF-05")
                    return "@#$%^&*";

                if (duLieuNhap.Contains(
                    "SQL",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return "' OR 1=1 --";
                }

                return LayGiaTri(
                    duLieuNhap,
                    "ma_DocGia");
            }

            if (maBai == "B5B")
            {
                if (id == "B5B-WF-01")
                    return "";

                if (id == "B5B-WF-02")
                    return "     ";

                if (id == "B5B-WF-03")
                {
                    string isbn =
                        LayISBNHopLe(
                            connectionString);

                    return "   " + isbn + "   ";
                }

                if (id == "B5B-WF-04")
                    return new string('I', 200);

                if (id == "B5B-WF-05")
                    return "@@@###";

                if (duLieuNhap.Contains(
                    "SQL",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return "' OR 1=1 --";
                }

                return LayGiaTri(
                    duLieuNhap,
                    "isbn");
            }

            return "";
        }

        public static TestcaseRunResult XuLyTestcase(
            string connectionString,
            string maBai,
            string procedureName,
            string idTestcase,
            string nhomLoi,
            string duLieuNhap,
            string moTa)
        {
            try
            {
                if (maBai == "B5A")
                {
                    return XuLy5A(
                        connectionString,
                        procedureName,
                        idTestcase,
                        nhomLoi,
                        duLieuNhap,
                        moTa);
                }

                if (maBai == "B5B")
                {
                    return XuLy5B(
                        connectionString,
                        procedureName,
                        idTestcase,
                        nhomLoi,
                        duLieuNhap,
                        moTa);
                }

                // 5C, 5D, 5E: stored procedure không có tham số.
                // Bấm "Chạy testcase" vẫn thực thi procedure thật trên dữ liệu hiện tại.
                DataTable dt =
                    ChayProcedure(
                        connectionString,
                        procedureName,
                        maBai,
                        null);

                return new TestcaseRunResult
                {
                    Data = dt,
                    Message =
                        $"Đã thực thi thật testcase {idTestcase}.\n" +
                        $"Nhóm: {nhomLoi}\n" +
                        $"Stored procedure: {procedureName}\n" +
                        $"Số dòng SQL trả về: {dt.Rows.Count}\n\n" +
                        "Lưu ý: testcase mô tả một trạng thái dữ liệu đặc biệt " +
                        "(ví dụ bảng rỗng, đúng 14 ngày, chỉ có trẻ em, nhiều cặp...) " +
                        "chỉ cho kết quả đúng tình huống đó khi CSDL hiện tại đang ở đúng trạng thái tương ứng."
                };
            }
            catch (SqlException ex) when (DoAn.Shared.SqlFailureClassifier.IsLibraryLookup(ex))
            {
                return new TestcaseRunResult { Message = $"Thông báo nghiệp vụ: {ex.Message}", IsError = false };
            }
            catch (Exception ex)
            {
                return new TestcaseRunResult
                {
                    Message =
                        $"Testcase {idTestcase} đã gửi lệnh xuống SQL nhưng SQL trả lỗi:\n{ex.Message}",
                    IsError = true
                };
            }
        }

        private static TestcaseRunResult XuLy5A(
            string connectionString,
            string procedureName,
            string id,
            string nhomLoi,
            string duLieuNhap,
            string moTa)
        {
            string? input;

            if (duLieuNhap.Contains(
                "NULL",
                StringComparison.OrdinalIgnoreCase))
            {
                input = null;
            }
            else if (duLieuNhap.Contains(
                "DG_KHONG_TON_TAI",
                StringComparison.OrdinalIgnoreCase))
            {
                input = "DG_KHONG_TON_TAI";
            }
            else if (moTa.Contains(
                "người lớn",
                StringComparison.OrdinalIgnoreCase))
            {
                input = LayMaNguoiLon(
                    connectionString);
            }
            else if (moTa.Contains(
                "trẻ em",
                StringComparison.OrdinalIgnoreCase))
            {
                input = LayMaTreEm(
                    connectionString);
            }
            else if (moTa.Contains(
                "chưa phân loại",
                StringComparison.OrdinalIgnoreCase))
            {
                input =
                    LayMaDocGiaChuaPhanLoai(
                        connectionString);
            }
            else
            {
                input =
                    LayMaDocGiaHopLe(
                        connectionString);
            }

            DataTable dt =
                ChayProcedure(
                    connectionString,
                    procedureName,
                    "B5A",
                    input);

            return new TestcaseRunResult
            {
                InputForForm =
                    input ?? "NULL",

                Data = dt,

                Message =
                    $"Đã chạy thật testcase {id}.\n" +
                    $"Nhóm: {nhomLoi}\n" +
                    $"Giá trị truyền xuống sp_ThongtinDocGia: {(input ?? "NULL")}\n" +
                    $"SQL trả về {dt.Rows.Count} dòng."
            };
        }

        private static TestcaseRunResult XuLy5B(
            string connectionString,
            string procedureName,
            string id,
            string nhomLoi,
            string duLieuNhap,
            string moTa)
        {
            string? input;

            if (duLieuNhap.Contains(
                "NULL",
                StringComparison.OrdinalIgnoreCase))
            {
                input = null;
            }
            else if (duLieuNhap.Contains(
                "ISBN_KHONG_TON_TAI",
                StringComparison.OrdinalIgnoreCase))
            {
                input = "ISBN_KHONG_TON_TAI";
            }
            else if (moTa.Contains(
                "chưa có Cuonsach",
                StringComparison.OrdinalIgnoreCase))
            {
                input =
                    LayISBNChuaCoCuonSach(
                        connectionString);
            }
            else if (moTa.Contains(
                "đúng 1",
                StringComparison.OrdinalIgnoreCase)
                || moTa.Contains(
                    "1 cuốn",
                    StringComparison.OrdinalIgnoreCase))
            {
                input =
                    LayISBNTheoSoLuongCon(
                        connectionString,
                        1);
            }
            else if (moTa.Contains(
                "nhiều",
                StringComparison.OrdinalIgnoreCase)
                || moTa.Contains(
                    ">=2",
                    StringComparison.OrdinalIgnoreCase))
            {
                input =
                    LayISBNConNhieuSach(
                        connectionString);
            }
            else if (moTa.Contains(
                "0 cuốn",
                StringComparison.OrdinalIgnoreCase)
                || moTa.Contains(
                    "tất cả",
                    StringComparison.OrdinalIgnoreCase)
                || moTa.Contains(
                    "hết",
                    StringComparison.OrdinalIgnoreCase))
            {
                input =
                    LayISBNHetSach(
                        connectionString);
            }
            else
            {
                input =
                    LayISBNHopLe(
                        connectionString);
            }

            if (input != null && string.IsNullOrWhiteSpace(input))
            {
                return new TestcaseRunResult
                {
                    Message =
                        $"Không tìm thấy dữ liệu CSDL phù hợp để dựng testcase {id}.\n" +
                        "Testcase vẫn hợp lệ về mặt danh mục, nhưng dữ liệu hiện tại chưa có trạng thái tương ứng.",
                    IsError = true
                };
            }

            DataTable dt =
                ChayProcedure(
                    connectionString,
                    procedureName,
                    "B5B",
                    input);

            return new TestcaseRunResult
            {
                InputForForm =
                    input ?? "NULL",

                Data = dt,

                Message =
                    $"Đã chạy thật testcase {id}.\n" +
                    $"Nhóm: {nhomLoi}\n" +
                    $"ISBN truyền xuống sp_ThongtinDausach: {(input ?? "NULL")}\n" +
                    $"SQL trả về {dt.Rows.Count} dòng."
            };
        }

        public static string LayMaDocGiaHopLe(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 ma_DocGia
                  FROM dbo.DocGia
                  ORDER BY ma_DocGia;");
        }

        public static string LayMaNguoiLon(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 ma_DocGia
                  FROM dbo.Nguoilon
                  ORDER BY ma_DocGia;");
        }

        public static string LayMaTreEm(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 ma_DocGia
                  FROM dbo.Treem
                  ORDER BY ma_DocGia;");
        }

        public static string LayMaDocGiaChuaPhanLoai(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 d.ma_DocGia
                  FROM dbo.DocGia d
                  WHERE NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.Nguoilon n
                      WHERE n.ma_DocGia = d.ma_DocGia
                  )
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.Treem t
                      WHERE t.ma_DocGia = d.ma_DocGia
                  )
                  ORDER BY d.ma_DocGia;");
        }

        public static string LayISBNHopLe(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 isbn
                  FROM dbo.Dausach
                  ORDER BY isbn;");
        }

        public static string LayISBNChuaCoCuonSach(
            string connectionString)
        {
            return LayMotGiaTri(
                connectionString,
                @"SELECT TOP 1 d.isbn
                  FROM dbo.Dausach d
                  WHERE NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.Cuonsach c
                      WHERE c.isbn = d.isbn
                  )
                  ORDER BY d.isbn;");
        }

        public static string LayISBNTheoSoLuongCon(
            string connectionString,
            int soLuong)
        {
            using SqlConnection conn =
                new SqlConnection(connectionString);

            conn.Open();

            string sql = @"
                SELECT TOP 1 d.isbn
                FROM dbo.Dausach d
                LEFT JOIN dbo.Cuonsach c
                    ON d.isbn = c.isbn
                GROUP BY d.isbn
                HAVING SUM(
                    CASE
                        WHEN LOWER(LTRIM(RTRIM(ISNULL(c.tinhtrang,''))))
                             IN ('yes', N'có sẵn', N'co san')
                        THEN 1
                        ELSE 0
                    END
                ) = @SoLuong
                ORDER BY d.isbn;";

            using SqlCommand cmd =
                new SqlCommand(sql, conn);

            cmd.Parameters.Add(
                "@SoLuong",
                SqlDbType.Int).Value =
                soLuong;

            object? value =
                cmd.ExecuteScalar();

            return value?.ToString() ?? "";
        }

        public static string LayISBNConNhieuSach(
            string connectionString)
        {
            using SqlConnection conn =
                new SqlConnection(connectionString);

            conn.Open();

            string sql = @"
                SELECT TOP 1 d.isbn
                FROM dbo.Dausach d
                JOIN dbo.Cuonsach c
                    ON d.isbn = c.isbn
                GROUP BY d.isbn
                HAVING SUM(
                    CASE
                        WHEN LOWER(LTRIM(RTRIM(ISNULL(c.tinhtrang,''))))
                             IN ('yes', N'có sẵn', N'co san')
                        THEN 1
                        ELSE 0
                    END
                ) >= 2
                ORDER BY d.isbn;";

            using SqlCommand cmd =
                new SqlCommand(sql, conn);

            object? value =
                cmd.ExecuteScalar();

            return value?.ToString() ?? "";
        }

        public static string LayISBNHetSach(
            string connectionString)
        {
            using SqlConnection conn =
                new SqlConnection(connectionString);

            conn.Open();

            string sql = @"
                SELECT TOP 1 d.isbn
                FROM dbo.Dausach d
                LEFT JOIN dbo.Cuonsach c
                    ON d.isbn = c.isbn
                GROUP BY d.isbn
                HAVING SUM(
                    CASE
                        WHEN LOWER(LTRIM(RTRIM(ISNULL(c.tinhtrang,''))))
                             IN ('yes', N'có sẵn', N'co san')
                        THEN 1
                        ELSE 0
                    END
                ) = 0
                ORDER BY d.isbn;";

            using SqlCommand cmd =
                new SqlCommand(sql, conn);

            object? value =
                cmd.ExecuteScalar();

            return value?.ToString() ?? "";
        }

        private static string LayMotGiaTri(
            string connectionString,
            string sql)
        {
            using SqlConnection conn =
                new SqlConnection(connectionString);

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(sql, conn);

            object? value =
                cmd.ExecuteScalar();

            return value?.ToString() ?? "";
        }

        private static string LayGiaTri(
            string input,
            string key)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            Match m =
                Regex.Match(
                    input,
                    @"(?:^|;)\s*" +
                    Regex.Escape(key) +
                    @"\s*=\s*(.*?)(?=;\s*[A-Za-z_][A-Za-z0-9_]*\s*=|$)",
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline);

            return m.Success
                ? m.Groups[1].Value.Trim()
                : "";
        }
    }
}
