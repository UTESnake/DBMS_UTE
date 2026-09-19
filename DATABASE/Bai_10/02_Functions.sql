USE QL_TruongPhoThong;
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_GiaoVienMonTu45Tiet()
RETURNS TABLE AS RETURN
(
    SELECT g.MaGV,g.TenGV,m.MaMH,m.TenMH,m.SoTiet
    FROM dbo.B10_GV g JOIN dbo.B10_MHOC m ON m.MaMH=g.MaMH
    WHERE m.SoTiet>=45
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_GiaoVienGacThiHocKy(@HocKy tinyint)
RETURNS TABLE AS RETURN
(
    SELECT DISTINCT g.MaGV,g.TenGV
    FROM dbo.B10_GV g JOIN dbo.B10_PC_COI_THI pc ON pc.MaGV=g.MaGV
    WHERE pc.HKY=@HocKy
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_GiaoVienKhongGacThiHocKy(@HocKy tinyint)
RETURNS TABLE AS RETURN
(
    SELECT g.MaGV,g.TenGV
    FROM dbo.B10_GV g
    WHERE NOT EXISTS(SELECT 1 FROM dbo.B10_PC_COI_THI pc WHERE pc.MaGV=g.MaGV 
    AND pc.HKY=@HocKy)
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_LichThiMon(@TenMon nvarchar(100))
RETURNS TABLE AS RETURN
(
    SELECT b.HKY,b.Ngay,b.Gio,b.Phg,m.MaMH,m.TenMH,b.TGThi
    FROM dbo.B10_BUOITHI b JOIN dbo.B10_MHOC m ON m.MaMH=b.MaMH
    WHERE m.TenMH = @TenMon
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon(@TenMon nvarchar(100))
RETURNS TABLE AS RETURN
(
    SELECT g.MaGV,g.TenGV,pc.HKY,pc.Ngay,pc.Gio,pc.Phg,mt.TenMH AS MonThi
    FROM dbo.B10_GV g JOIN dbo.B10_MHOC mc ON mc.MaMH=g.MaMH
    JOIN dbo.B10_PC_COI_THI pc ON pc.MaGV=g.MaGV
    JOIN dbo.B10_BUOITHI b ON b.HKY=pc.HKY AND b.Ngay=pc.Ngay AND b.Gio=pc.Gio 
    AND b.Phg=pc.Phg
    JOIN dbo.B10_MHOC mt ON mt.MaMH=b.MaMH
    WHERE mc.TenMH = @TenMon
);
GO
