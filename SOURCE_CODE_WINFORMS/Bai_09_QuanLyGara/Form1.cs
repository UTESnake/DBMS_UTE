using System.Drawing;
using DoAn.Shared;

namespace Bai_09_QuanLyGara;

public sealed class Form1 : ExerciseQueryFormBase
{
    protected override string DatabaseName => "QL_Gara";
    protected override string WindowTitle => "Bài 9 - Quản lý sửa chữa gara";
    protected override string HeaderTitle => "FUNCTION VÀ RÀNG BUỘC CƠ SỞ DỮ LIỆU GARA";
    protected override Color HeaderColor => Color.FromArgb(217, 119, 6);
    protected override string ObjectNamePattern => "%B9_%";
    protected override int RequiredObjectCount => 7;
    protected override string[] ScriptResourceSuffixes => ["01_TaoBang_NhapDuLieu.sql", "02_Functions.sql"];
    protected override QueryItem[] Queries =>
    [
        new("9.1", "Thợ không tham gia hợp đồng nào", "", "SELECT * FROM dbo.fn_B9_ThoKhongThamGiaHopDong() ORDER BY MaTho"),
        new("9.2", "Hợp đồng đã thanh lý nhưng chưa trả đủ", "", "SELECT * FROM dbo.fn_B9_HopDongDaThanhLyChuaDuTien() ORDER BY SoHD"),
        new("9.3", "Hợp đồng phải hoàn tất trước 31/12/2002", "", "SELECT * FROM dbo.fn_B9_HopDongCanHoanTatTruoc() ORDER BY NgayGiaoDK"),
        new("9.4", "Người thợ thực hiện nhiều công việc nhất", "", "SELECT * FROM dbo.fn_B9_ThoNhieuCongViecNhat() ORDER BY MaTho"),
        new("9.5", "Người thợ có tổng trị giá được giao cao nhất", "", "SELECT * FROM dbo.fn_B9_ThoTongTriGiaCaoNhat() ORDER BY MaTho"),
        new("9.RB", "Kiểm chứng nhóm trưởng phải cùng nhóm", "", """
            DECLARE @HopLe nvarchar(20)=N'Không đạt', @HopLeChiTiet nvarchar(4000)=N'';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_THO(MaTho,TenTho,Nhom,NhomTruong) VALUES('T98',N'Thợ thử hợp lệ',1,'T01');
                SET @HopLe=N'Đạt';
                SET @HopLeChiTiet=N'CSDL chấp nhận nhóm trưởng T01 cho thợ cùng nhóm 1.';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @HopLeChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @KhacNhom nvarchar(20)=N'Không đạt', @KhacNhomChiTiet nvarchar(4000)=N'CSDL đã nhận dữ liệu sai.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_THO(MaTho,TenTho,Nhom,NhomTruong) VALUES('T99',N'Thợ thử sai nhóm',2,'T01');
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @KhacNhom=N'Đạt';
                SET @KhacNhomChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            SELECT N'Thợ và nhóm trưởng cùng nhóm' AS CaKiemChung,@HopLe AS KetQua,@HopLeChiTiet AS ChiTiet
            UNION ALL
            SELECT N'Thợ và nhóm trưởng khác nhóm',@KhacNhom,@KhacNhomChiTiet;
            """, "✓  Kiểm chứng ràng buộc")
    ];
}
