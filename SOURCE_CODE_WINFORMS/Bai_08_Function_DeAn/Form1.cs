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
}
