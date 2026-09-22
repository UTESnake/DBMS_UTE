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
    protected override string[] ScriptResourceSuffixes => ["01_TaoBang_NhapDuLieu.sql", "02_Functions.sql"];
    protected override QueryItem[] Queries =>
    [
        new("8.1", "Dự án có hơn 2 nhân viên", "", "SELECT * FROM dbo.fn_B8_DeAnNhieuNhanVien() ORDER BY MaDA"),
        new("8.2", "Phòng có hơn 2 nhân viên, đếm lương > 25000", "", "SELECT * FROM dbo.fn_B8_PhongNhieuNhanVienLuongCao() ORDER BY MaPB"),
        new("8.3", "Phòng có lương trung bình > 30000", "", "SELECT * FROM dbo.fn_B8_PhongLuongTBLon() ORDER BY MaPB"),
        new("8.4", "Số nhân viên nam của phòng lương TB > 30000", "", "SELECT * FROM dbo.fn_B8_PhongLuongTBLon_Nam() ORDER BY MaPB"),
        new("8.5", "Số nhân viên phòng 5 tham gia từng dự án", "", "SELECT * FROM dbo.fn_B8_DeAnCoNhanVienPhong5() ORDER BY MaDA")
    ];

    protected override string[] SourceTablesFor(string code) => code switch
    {
        "8.1" => ["B7_DeAn", "B7_PhanCong"],
        "8.2" or "8.3" or "8.4" => ["B7_PhongBan", "B7_NhanVien"],
        "8.5" => ["B7_DeAn", "B7_PhanCong", "B7_NhanVien"],
        _ => []
    };

    protected override string SourceSql(string table) => table switch
    {
        "B7_DeAn" => "SELECT * FROM dbo.B7_DeAn ORDER BY MaDA",
        "B7_PhanCong" => "SELECT * FROM dbo.B7_PhanCong ORDER BY MaDA, MaNV",
        "B7_PhongBan" => "SELECT * FROM dbo.B7_PhongBan ORDER BY MaPB",
        "B7_NhanVien" => "SELECT * FROM dbo.B7_NhanVien ORDER BY MaPB, MaNV",
        _ => throw new InvalidOperationException("Bảng dữ liệu không thuộc Bài 8.")
    };
}
