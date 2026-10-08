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
CREATE OR ALTER FUNCTION dbo.fn_B10_GiaoVienGacThiHocKy()
RETURNS TABLE AS RETURN
(
    SELECT DISTINCT g.MaGV,g.TenGV
    FROM dbo.B10_GV g JOIN dbo.B10_PC_COI_THI pc ON pc.MaGV=g.MaGV
    WHERE pc.HKY=1
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_GiaoVienKhongGacThiHocKy()
RETURNS TABLE AS RETURN
(
    -- Không cần disticnt vì chỉ lấy dữ liệu từ bảng GV mà có mã GV là khóa chính của bảng
    SELECT g.MaGV,g.TenGV
    FROM dbo.B10_GV g
    WHERE NOT EXISTS(SELECT 1 FROM dbo.B10_PC_COI_THI pc WHERE pc.MaGV=g.MaGV 
    AND pc.HKY=1)
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_LichThiMon()
RETURNS TABLE AS RETURN
(
    SELECT b.HKY,b.Ngay,b.Gio,b.Phg,m.MaMH,m.TenMH,b.TGThi
    FROM dbo.B10_BUOITHI b JOIN dbo.B10_MHOC m ON m.MaMH=b.MaMH
    WHERE m.TenMH = N'VĂN HỌC'
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B10_BuoiGacThiCuaGiaoVienChuNhiemMon()
RETURNS TABLE AS RETURN
(
    SELECT g.MaGV,g.TenGV,pc.HKY,pc.Ngay,pc.Gio,pc.Phg,mt.TenMH AS MonThi
    FROM dbo.B10_GV g
    JOIN dbo.B10_PC_COI_THI pc ON pc.MaGV=g.MaGV
    JOIN dbo.B10_BUOITHI b ON b.HKY=pc.HKY AND b.Ngay=pc.Ngay AND b.Gio=pc.Gio AND b.Phg=pc.Phg
    JOIN dbo.B10_MHOC mt ON mt.MaMH=b.MaMH
    WHERE g.MaMH = (SELECT MaMH FROM dbo.B10_MHOC WHERE TenMH=N'VĂN HỌC')
);
GO
