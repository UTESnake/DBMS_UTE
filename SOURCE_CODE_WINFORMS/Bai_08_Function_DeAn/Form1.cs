using System.Drawing;
using DoAn.Shared;

namespace Bai_08_Function_DeAn;

public sealed class Form1 : ExerciseQueryFormBase
{
    protected override string DatabaseName => "QL_DeAn";
    protected override string WindowTitle => "Bài 8 - Function thống kê Đề án";
    protected override string HeaderTitle => "FUNCTION THỐNG KÊ CƠ SỞ DỮ LIỆU ĐỀ ÁN";
    protected override Color HeaderColor => Color.FromArgb(8, 145, 178);
    protected override string ObjectNamePattern => "fn_B8_%";
    protected override int RequiredObjectCount => 5;
    protected override bool ShowNoResultsMessage => true;
    protected override string DefaultParameter(string code) => "";
    protected override Microsoft.Data.SqlClient.SqlParameter CreateParameter(string code, string value)
    {
        value = value.Trim();
        if (value.Length > 2 || (value.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]{2}$")))
            throw new ArgumentException("Mã lọc phải là mã phòng/đề án gồm 2 chữ số, hoặc để trống để xem tất cả.");
        return new Microsoft.Data.SqlClient.SqlParameter("@p1", System.Data.SqlDbType.VarChar, 2) { Value = value };
    }
    protected override string[] ScriptResourceSuffixes => ["01_TaoBang_NhapDuLieu.sql", "02_Functions.sql"];
    protected override QueryItem[] Queries =>
    [
        new("8.1", "Dự án có hơn 2 nhân viên", "Lọc mã đề án (tùy chọn)", BuildQuery("fn_B8_DeAnNhieuNhanVien", "MaDA")),
        new("8.2", "Phòng có hơn 2 nhân viên, đếm lương > 25000", "Lọc mã phòng (tùy chọn)", BuildQuery("fn_B8_PhongNhieuNhanVienLuongCao", "MaPhg")),
        new("8.3", "Phòng có lương trung bình > 30000", "Lọc mã phòng (tùy chọn)", BuildQuery("fn_B8_PhongLuongTBLon", "MaPhg")),
        new("8.4", "Số nhân viên nam của phòng lương TB > 30000", "Lọc mã phòng (tùy chọn)", BuildQuery("fn_B8_PhongLuongTBLon_Nam", "MaPhg")),
        new("8.5", "Số nhân viên phòng 5 tham gia từng dự án", "Lọc mã đề án (tùy chọn)", BuildQuery85())
    ];

    private static string BuildQuery85() =>
        """
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.fn_B8_DeAnCoNhanVienPhong5()
            WHERE (@p1 = '' OR MaDA = @p1) AND SoNhanVienPhong5 > 0
        )
        BEGIN
            ;THROW 50001, N'Không có dữ liệu trả về (danh sách nhân viên phòng 5 tham gia rỗng).', 1;
        END;

        SELECT *
        FROM dbo.fn_B8_DeAnCoNhanVienPhong5()
        WHERE @p1 = '' OR MaDA = @p1
        ORDER BY MaDA;
        """;

    private static string BuildQuery(string functionName, string codeColumn) =>
        $"""
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.{functionName}()
            WHERE @p1 = '' OR {codeColumn} = @p1
        )
        BEGIN
            ;THROW 50001, N'Không có dữ liệu trả về.', 1;
        END;

        SELECT *
        FROM dbo.{functionName}()
        WHERE @p1 = '' OR {codeColumn} = @p1
        ORDER BY {codeColumn};
        """;

    protected override string[] SourceTablesFor(string code) => code switch
    {
        "8.1" => ["DEAN", "PHANCONG"],
        "8.2" or "8.3" or "8.4" => ["PHONGBAN", "NHANVIEN", "BANGLUONG"],
        "8.5" => ["DEAN", "PHANCONG", "NHANVIEN", "DIADIEM_PHG"],
        _ => []
    };

    protected override string SourceSql(string table) => table switch
    {
        "DEAN" => "SELECT * FROM dbo.DEAN ORDER BY MaDA",
        "PHANCONG" => "SELECT * FROM dbo.PHANCONG ORDER BY SoDA, MaNV",
        "PHONGBAN" => "SELECT * FROM dbo.PHONGBAN ORDER BY MaPhg",
        "NHANVIEN" => "SELECT * FROM dbo.NHANVIEN ORDER BY Phg, MaNV",
        "DIADIEM_PHG" => "SELECT * FROM dbo.DIADIEM_PHG ORDER BY MaPhg, DiaDiem",
        "BANGLUONG" => "SELECT * FROM dbo.BANGLUONG ORDER BY MaNV",
        _ => throw new InvalidOperationException("Bảng dữ liệu không thuộc Bài 8.")
    };
}
