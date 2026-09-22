using System.Drawing;
using DoAn.Shared;

namespace Bai_10_TruongPhoThong;

public sealed class Form1 : ExerciseQueryFormBase
{
    protected override string DatabaseName => "QL_TruongPhoThong";
    protected override string WindowTitle => "Bài 10 - Quản lý thi trường phổ thông";
    protected override string HeaderTitle => "FUNCTION, TRIGGER VÀ RÀNG BUỘC LỊCH THI";
    protected override Color HeaderColor => Color.FromArgb(190, 24, 93);
    protected override string ObjectNamePattern => "%B10_%";
    protected override int RequiredObjectCount => 9;
    protected override string[] ScriptResourceSuffixes => ["01_TaoBang_NhapDuLieu.sql", "02_Functions.sql"];
    protected override QueryItem[] Queries =>
    [
        new("10.1.a", "GV không gác thi môn mình chủ nhiệm", "", """
            DECLARE @KetQua nvarchar(20)=N'Không đạt', @ChiTiet nvarchar(4000)=N'CSDL đã nhận phân công sai.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B10_PC_COI_THI(MaGV,HKY,Ngay,Gio,Phg)
                VALUES('GV01',1,'2026-05-10','07:30','P101');
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @KetQua=N'Đạt'; SET @ChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;
            SELECT N'GV01 chủ nhiệm VĂN HỌC được thử gác buổi thi VĂN HỌC' AS CaKiemChung,@KetQua AS KetQua,@ChiTiet AS ChiTiet;
            """, "✓  Kiểm chứng ràng buộc"),
        new("10.1.b", "Môn 30 tiết phải thi 120 phút", "", """
            DECLARE @KetQua nvarchar(20)=N'Không đạt', @ChiTiet nvarchar(4000)=N'CSDL đã nhận thời lượng sai.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B10_BUOITHI(HKY,Ngay,Gio,Phg,MaMH,TGThi)
                VALUES(3,'2099-12-01','07:30','Z301','MH02',150);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @KetQua=N'Đạt'; SET @ChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;
            SELECT N'Thử cho môn TOÁN 30 tiết thi 150 phút' AS CaKiemChung,@KetQua AS KetQua,@ChiTiet AS ChiTiet;
            """, "✓  Kiểm chứng ràng buộc"),
        new("10.1.c", "Môn từ 45 tiết phải thi 150 phút", "", """
            DECLARE @KetQua nvarchar(20)=N'Không đạt', @ChiTiet nvarchar(4000)=N'CSDL đã nhận thời lượng sai.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B10_BUOITHI(HKY,Ngay,Gio,Phg,MaMH,TGThi)
                VALUES(3,'2099-12-01','13:30','Z302','MH01',120);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @KetQua=N'Đạt'; SET @ChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;
            SELECT N'Thử cho môn VĂN HỌC 45 tiết thi 120 phút' AS CaKiemChung,@KetQua AS KetQua,@ChiTiet AS ChiTiet;
            """, "✓  Kiểm chứng ràng buộc"),
        new("10.2.a", "Giáo viên dạy môn từ 45 tiết", "", "SELECT * FROM dbo.fn_B10_GiaoVienMonTu45Tiet() ORDER BY MaGV"),
        new("10.2.b", "Giáo viên được gác thi học kỳ 1", "", "SELECT * FROM dbo.fn_B10_GiaoVienGacThiHocKy(1) ORDER BY MaGV"),
        new("10.2.c", "Giáo viên không gác thi học kỳ 1", "", "SELECT * FROM dbo.fn_B10_GiaoVienKhongGacThiHocKy(1) ORDER BY MaGV"),
        new("10.2.d", "Lịch thi môn VĂN HỌC", "", "SELECT * FROM dbo.fn_B10_LichThiMon(N'VĂN HỌC') ORDER BY HKY,Ngay,Gio"),
        new("10.2.e", "Buổi gác của giáo viên chủ nhiệm VĂN HỌC", "", "SELECT * FROM dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon(N'VĂN HỌC') ORDER BY HKY,Ngay,Gio")
    ];

    protected override string[] SourceTablesFor(string code) => code switch
    {
        "10.1.a" => ["B10_GV", "B10_MHOC", "B10_BUOITHI", "B10_PC_COI_THI"],
        "10.1.b" or "10.1.c" => ["B10_MHOC", "B10_BUOITHI"],
        "10.2.a" => ["B10_GV", "B10_MHOC"],
        "10.2.b" or "10.2.c" => ["B10_GV", "B10_PC_COI_THI", "B10_BUOITHI"],
        "10.2.d" => ["B10_MHOC", "B10_BUOITHI"],
        "10.2.e" => ["B10_GV", "B10_MHOC", "B10_PC_COI_THI", "B10_BUOITHI"],
        _ => []
    };

    protected override string SourceSql(string table) => table switch
    {
        "B10_GV" => "SELECT * FROM dbo.B10_GV ORDER BY MaGV",
        "B10_MHOC" => "SELECT * FROM dbo.B10_MHOC ORDER BY MaMH",
        "B10_BUOITHI" => "SELECT * FROM dbo.B10_BUOITHI ORDER BY HKY, Ngay, Gio, Phg",
        "B10_PC_COI_THI" => "SELECT * FROM dbo.B10_PC_COI_THI ORDER BY HKY, MaGV, Ngay, Gio, Phg",
        _ => throw new InvalidOperationException("Bảng dữ liệu không thuộc Bài 10.")
    };
}
