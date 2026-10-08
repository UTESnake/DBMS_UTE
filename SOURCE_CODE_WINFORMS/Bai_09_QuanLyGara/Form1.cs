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
    protected override int RequiredObjectCount => 12;
    protected override string[] ScriptResourceSuffixes => ["01_TaoBang_NhapDuLieu.sql", "02_Functions.sql"];
    protected override QueryItem[] Queries =>
    [
        new("9.1", "Thợ không tham gia hợp đồng nào", "", "SELECT * FROM dbo.fn_B9_ThoKhongThamGiaHopDong() ORDER BY MaTho"),
        new("9.2", "Hợp đồng đã thanh lý nhưng chưa trả đủ", "", "SELECT * FROM dbo.fn_B9_HopDongDaThanhLyChuaDuTien() ORDER BY SoHD"),
        new("9.3", "Hợp đồng phải hoàn tất trước 31/12/2002", "", "SELECT * FROM dbo.fn_B9_HopDongCanHoanTatTruoc() ORDER BY NgayGiaoDK"),
        new("9.4", "Người thợ thực hiện nhiều công việc nhất", "", "SELECT * FROM dbo.fn_B9_ThoNhieuCongViecNhat() ORDER BY MaTho"),
        new("9.5", "Người thợ có tổng trị giá được giao cao nhất", "", "SELECT * FROM dbo.fn_B9_ThoTongTriGiaCaoNhat() ORDER BY MaTho"),
        new("9.RB", "Kiểm chứng ràng buộc gara", "", """
            DECLARE @HopLe nvarchar(20)=N'Không đạt', @HopLeChiTiet nvarchar(4000)=N'';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_THO(MaTho,TenTho,Nhom,NhomTruong) VALUES('T98',N'Thợ thử hợp lệ',1,'T01');
                SET @HopLe=N'Đạt';
                SET @HopLeChiTiet=N'CSDL chấp nhận nhóm trưởng T01 cho thợ cùng nhóm 1.';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @HopLe=CASE WHEN ERROR_NUMBER() IN (50000,547,2601,2627) THEN N'Không đạt' ELSE N'ERROR' END;
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
                SET @KhacNhom=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraNhomTruong',N'dbo.tg_B9_KiemTraNhomTruong')
                    AND ERROR_MESSAGE()=N'Nhóm trưởng phải là một người thợ thuộc cùng nhóm.'
                    THEN N'Đạt' ELSE CASE WHEN ERROR_NUMBER() IN (50000,547,2601,2627) THEN N'Không đạt' ELSE N'ERROR' END END;
                SET @KhacNhomChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @HaiTruong nvarchar(20)=N'Không đạt', @HaiTruongChiTiet nvarchar(4000)=N'CSDL đã nhận hai nhóm trưởng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_THO SET NhomTruong='T02' WHERE MaTho='T02';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @HaiTruong=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraDoiNhomTruong',N'dbo.tg_B9_KiemTraDoiNhomTruong')
                    AND ERROR_MESSAGE()=N'Mỗi nhóm chỉ được có một nhóm trưởng.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @HaiTruongChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @SaiTong nvarchar(20)=N'Không đạt', @SaiTongChiTiet nvarchar(4000)=N'CSDL đã nhận trị giá hợp đồng sai.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_HOPDONG SET TriGiaHD=TriGiaHD+1 WHERE SoHD='HD01';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @SaiTong=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraTriGiaHopDong',N'dbo.tg_B9_KiemTraTriGiaHopDong')
                    AND ERROR_MESSAGE()=N'Trị giá hợp đồng phải bằng tổng trị giá các công việc.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @SaiTongChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @DongBoTong nvarchar(20)=N'Không đạt', @DongBoChiTiet nvarchar(4000)=N'Tổng trị giá chưa được cập nhật.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_CHITIET_HD SET TriGiaCV=TriGiaCV+100000 WHERE SoHD='HD02' AND MaCV='CV01';
                IF (SELECT TriGiaHD FROM dbo.B9_HOPDONG WHERE SoHD='HD02')=3400000
                BEGIN
                    SET @DongBoTong=N'Đạt';
                    SET @DongBoChiTiet=N'Trị giá HD02 được đồng bộ thành 3.400.000.';
                END;
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @DongBoTong=N'ERROR';
                SET @DongBoChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @TrungXe nvarchar(20)=N'Không đạt', @TrungXeChiTiet nvarchar(4000)=N'CSDL đã nhận hợp đồng trùng xe/ngày.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_HOPDONG VALUES('HD99','2002-12-01','KH01','51A-111.11',0,'2002-12-20',NULL);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @TrungXe=CASE WHEN ERROR_NUMBER() IN (2601,2627)
                    AND ERROR_MESSAGE() LIKE N'%UQ_B9_HOPDONG_Ngay_SoXe%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @TrungXeChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @SaiChu nvarchar(20)=N'Không đạt', @SaiChuChiTiet nvarchar(4000)=N'CSDL đã nhận phiếu thu sai khách hàng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_PHIEUTHU VALUES('PT99','2002-12-10','HD01','KH02',N'Thử',1);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @SaiChu=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%FK_B9_PHIEUTHU_HOPDONG%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @SaiChuChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @TrungPK nvarchar(20)=N'Không đạt', @TrungPKChiTiet nvarchar(4000)=N'CSDL đã nhận mã thợ trùng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_THO VALUES('T01',N'Trùng mã',1,'T01');
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @TrungPK=CASE WHEN ERROR_NUMBER() IN (2601,2627)
                    AND ERROR_MESSAGE() LIKE N'%PK_B9_THO%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @TrungPKChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @SaiFK nvarchar(20)=N'Không đạt', @SaiFKChiTiet nvarchar(4000)=N'CSDL đã nhận chi tiết có mã thợ không tồn tại.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_CHITIET_HD VALUES('HD02','CV05',100,'TNOEXIST',0);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @SaiFK=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%FK_B9_CHITIET_THO%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @SaiFKChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @TienAm nvarchar(20)=N'Không đạt', @TienAmChiTiet nvarchar(4000)=N'CSDL đã nhận số tiền thu âm.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-10','HD01','KH01',N'Thử',-1);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @TienAm=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%CK_B9_PHIEUTHU_SoTienThu%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @TienAmChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @TienBangKhong nvarchar(20)=N'Không đạt', @TienBangKhongChiTiet nvarchar(4000)=N'CSDL đã nhận phiếu thu 0 đồng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-10','HD01','KH01',N'Thử',0);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @TienBangKhong=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%CK_B9_PHIEUTHU_SoTienThu%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @TienBangKhongChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @PhieuTruocHD nvarchar(20)=N'Không đạt', @PhieuTruocHDChiTiet nvarchar(4000)=N'CSDL đã nhận phiếu thu trước ngày ký.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-11-30','HD01','KH01',N'Thử',1);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @PhieuTruocHD=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraNgayPhieuThu',N'dbo.tg_B9_KiemTraNgayPhieuThu')
                    AND ERROR_MESSAGE()=N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @PhieuTruocHDChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @DoiNgayHD nvarchar(20)=N'Không đạt', @DoiNgayHDChiTiet nvarchar(4000)=N'CSDL đã nhận ngày ký sau ngày lập phiếu thu.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_HOPDONG SET NgayHD='2002-12-06' WHERE SoHD='HD01';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @DoiNgayHD=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraNgayHopDongPhieuThu',N'dbo.tg_B9_KiemTraNgayHopDongPhieuThu')
                    AND ERROR_MESSAGE()=N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @DoiNgayHDChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @ThuVuot nvarchar(20)=N'Không đạt', @ThuVuotChiTiet nvarchar(4000)=N'CSDL đã nhận tổng tiền thu vượt trị giá hợp đồng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_PHIEUTHU VALUES('PT98','2002-12-24','HD04','KH01',N'Thử',2500001);
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @ThuVuot=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraTongPhieuThu',N'dbo.tg_B9_KiemTraTongPhieuThu')
                    AND ERROR_MESSAGE()=N'Tổng tiền đã thu không được vượt trị giá hợp đồng.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @ThuVuotChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @GiamTriGia nvarchar(20)=N'Không đạt', @GiamTriGiaChiTiet nvarchar(4000)=N'CSDL đã giảm trị giá hợp đồng dưới số tiền đã thu.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_CHITIET_HD SET TriGiaCV=TriGiaCV-1 WHERE SoHD='HD01' AND MaCV='CV01';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @GiamTriGia=CASE WHEN ERROR_NUMBER()=50000
                    AND ERROR_PROCEDURE() IN (N'tg_B9_KiemTraTriGiaHopDong',N'dbo.tg_B9_KiemTraTriGiaHopDong')
                    AND ERROR_MESSAGE()=N'Tổng tiền đã thu không được vượt trị giá hợp đồng.'
                    THEN N'Đạt' ELSE N'Không đạt' END;
                SET @GiamTriGiaChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @TenRong nvarchar(20)=N'Không đạt', @TenRongChiTiet nvarchar(4000)=N'CSDL đã nhận tên thợ toàn khoảng trắng.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_THO SET TenTho=N'   ' WHERE MaTho='T02';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @TenRong=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%CK_B9_THO_Chuoi%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @TenRongChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @DienThoaiSai nvarchar(20)=N'Không đạt', @DienThoaiSaiChiTiet nvarchar(4000)=N'CSDL đã nhận số điện thoại có dấu + ở giữa.';
            BEGIN TRY
                BEGIN TRANSACTION;
                UPDATE dbo.B9_KHACHHANG SET DienThoai='090+2000001' WHERE MaKH='KH01';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @DienThoaiSai=CASE WHEN ERROR_NUMBER()=547
                    AND ERROR_MESSAGE() LIKE N'%CK_B9_KH_DienThoai%' THEN N'Đạt' ELSE N'Không đạt' END;
                SET @DienThoaiSaiChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @XeQuayLai nvarchar(20)=N'Không đạt',@XeQuayLaiChiTiet nvarchar(4000)=N'Không nhận hợp đồng ngày khác.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_HOPDONG VALUES('HD97','2002-12-15','KH01','51A-111.11',0,'2002-12-25',NULL);
                SET @XeQuayLai=N'Đạt';
                SET @XeQuayLaiChiTiet=N'Cùng xe với HD01 ngày 01/12, hợp đồng mới ngày 15/12 được nhận.';
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @XeQuayLaiChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            DECLARE @BaDot nvarchar(20)=N'Không đạt',@BaDotChiTiet nvarchar(4000)=N'Tổng ba phiếu không bằng 10 triệu.';
            BEGIN TRY
                BEGIN TRANSACTION;
                INSERT dbo.B9_HOPDONG VALUES('HD98','2002-12-15','KH01','51Z-999.99',0,'2002-12-25',NULL);
                INSERT dbo.B9_CHITIET_HD VALUES('HD98','CV01',10000000,'T02',1000000);
                INSERT dbo.B9_PHIEUTHU VALUES
                ('PT97A','2002-12-16','HD98','KH01',N'Thử',3000000),
                ('PT97B','2002-12-17','HD98','KH01',N'Thử',4000000),
                ('PT97C','2002-12-18','HD98','KH01',N'Thử',3000000);
                IF (SELECT SUM(SoTienThu) FROM dbo.B9_PHIEUTHU WHERE SoHD='HD98')=10000000
                BEGIN SET @BaDot=N'Đạt'; SET @BaDotChiTiet=N'Ba phiếu 3+4+3 triệu được nhận.'; END;
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END TRY
            BEGIN CATCH
                SET @BaDotChiTiet=ERROR_MESSAGE();
                IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
            END CATCH;

            SELECT N'Thợ và nhóm trưởng cùng nhóm' AS CaKiemChung,@HopLe AS KetQua,@HopLeChiTiet AS ChiTiet
            UNION ALL
            SELECT N'Thợ và nhóm trưởng khác nhóm',@KhacNhom,@KhacNhomChiTiet
            UNION ALL
            SELECT N'Mỗi nhóm chỉ có một nhóm trưởng',@HaiTruong,@HaiTruongChiTiet
            UNION ALL
            SELECT N'Chặn sửa tổng trị giá hợp đồng sai',@SaiTong,@SaiTongChiTiet
            UNION ALL
            SELECT N'Đồng bộ tổng khi sửa chi tiết',@DongBoTong,@DongBoChiTiet
            UNION ALL
            SELECT N'Một xe chỉ có một hợp đồng mỗi ngày',@TrungXe,@TrungXeChiTiet
            UNION ALL
            SELECT N'Phiếu thu thuộc đúng khách hợp đồng',@SaiChu,@SaiChuChiTiet
            UNION ALL
            SELECT N'Khóa chính mã thợ',@TrungPK,@TrungPKChiTiet
            UNION ALL
            SELECT N'Khóa ngoại thợ trong chi tiết',@SaiFK,@SaiFKChiTiet
            UNION ALL
            SELECT N'Chặn số tiền thu âm',@TienAm,@TienAmChiTiet
            UNION ALL
            SELECT N'Chặn số tiền thu bằng 0',@TienBangKhong,@TienBangKhongChiTiet
            UNION ALL
            SELECT N'Chặn phiếu thu trước ngày ký',@PhieuTruocHD,@PhieuTruocHDChiTiet
            UNION ALL
            SELECT N'Chặn đổi ngày ký sau ngày thu',@DoiNgayHD,@DoiNgayHDChiTiet
            UNION ALL
            SELECT N'Chặn tổng tiền thu vượt trị giá',@ThuVuot,@ThuVuotChiTiet
            UNION ALL
            SELECT N'Chặn giảm trị giá dưới tiền đã thu',@GiamTriGia,@GiamTriGiaChiTiet
            UNION ALL
            SELECT N'Chặn tên thợ rỗng',@TenRong,@TenRongChiTiet
            UNION ALL
            SELECT N'Chặn số điện thoại sai mẫu',@DienThoaiSai,@DienThoaiSaiChiTiet
            UNION ALL
            SELECT N'Một xe trở lại sửa vào ngày khác',@XeQuayLai,@XeQuayLaiChiTiet
            UNION ALL
            SELECT N'Hợp đồng 10 triệu thu ba đợt 3+4+3 triệu',@BaDot,@BaDotChiTiet;
            """, "✓  Kiểm chứng ràng buộc")
    ];

    protected override string[] SourceTablesFor(string code) => code switch
    {
        "9.1" => ["B9_THO", "B9_CHITIET_HD"],
        "9.2" => ["B9_HOPDONG", "B9_PHIEUTHU", "B9_KHACHHANG"],
        "9.3" => ["B9_HOPDONG", "B9_KHACHHANG"],
        "9.4" or "9.5" => ["B9_THO", "B9_CHITIET_HD", "B9_CONGVIEC"],
        "9.RB" => ["B9_THO", "B9_HOPDONG", "B9_CHITIET_HD", "B9_PHIEUTHU", "B9_KHACHHANG"],
        _ => []
    };

    protected override string SourceSql(string table) => table switch
    {
        "B9_THO" => "SELECT * FROM dbo.B9_THO ORDER BY Nhom, MaTho",
        "B9_CHITIET_HD" => "SELECT * FROM dbo.B9_CHITIET_HD ORDER BY MaTho, SoHD, MaCV",
        "B9_HOPDONG" => "SELECT * FROM dbo.B9_HOPDONG ORDER BY NgayGiaoDK, SoHD",
        "B9_PHIEUTHU" => "SELECT * FROM dbo.B9_PHIEUTHU ORDER BY SoHD, NgayLapPT, SoPT",
        "B9_KHACHHANG" => "SELECT * FROM dbo.B9_KHACHHANG ORDER BY MaKH",
        "B9_CONGVIEC" => "SELECT * FROM dbo.B9_CONGVIEC ORDER BY MaCV",
        _ => throw new InvalidOperationException("Bảng dữ liệu không thuộc Bài 9.")
    };
}
