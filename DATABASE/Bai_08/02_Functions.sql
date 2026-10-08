USE QL_DeAn;
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_DeAnNhieuNhanVien()
RETURNS TABLE AS RETURN
(
    SELECT d.MaDA,d.TenDA,
           COUNT(DISTINCT pc.MaNV) AS SoLuongNhanVien
    FROM dbo.DEAN AS d
    JOIN dbo.PHANCONG AS pc ON pc.SoDA=d.MaDA
    GROUP BY d.MaDA,d.TenDA
    HAVING COUNT(DISTINCT pc.MaNV)>2
);
GO

CREATE OR ALTER FUNCTION dbo.fn_B8_PhongNhieuNhanVienLuongCao()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPhg,
        -- nếu LuongCoBan > 25000 → trả về 1
        -- nếu không → trả về 0
           SUM(CASE WHEN b.LuongCoBan>25000 THEN 1 ELSE 0 END) AS SoNhanVienLuongTren25000
    FROM dbo.PHONGBAN AS p
    JOIN dbo.NHANVIEN AS n ON n.Phg=p.MaPhg
    LEFT JOIN dbo.BANGLUONG AS b ON b.MaNV=n.MaNV
    GROUP BY p.MaPhg
    HAVING COUNT(n.MaNV)>2
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_PhongLuongTBLon()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPhg,p.TenPhg,COUNT(n.MaNV) AS SoLuongNhanVien
    FROM dbo.PHONGBAN AS p
    JOIN dbo.NHANVIEN AS n ON n.Phg=p.MaPhg
    LEFT JOIN dbo.BANGLUONG AS b ON b.MaNV=n.MaNV
    GROUP BY p.MaPhg,p.TenPhg
    HAVING AVG(b.LuongCoBan)>30000
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_PhongLuongTBLon_Nam()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPhg,p.TenPhg,
           SUM(CASE WHEN n.Phai = N'Nam' THEN 1 ELSE 0 END) AS SoLuongNhanVienNam
    FROM dbo.PHONGBAN AS p
    JOIN dbo.NHANVIEN AS n ON n.Phg=p.MaPhg
    LEFT JOIN dbo.BANGLUONG AS b ON b.MaNV=n.MaNV
    GROUP BY p.MaPhg,p.TenPhg
    HAVING AVG(b.LuongCoBan)>30000
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_DeAnCoNhanVienPhong5()
RETURNS TABLE AS RETURN
(
    SELECT d.MaDA,d.TenDA,
           COUNT(n.MaNV) AS SoNhanVienPhong5
    FROM dbo.DEAN AS d
    LEFT JOIN dbo.PHANCONG AS pc ON pc.SoDA=d.MaDA
    LEFT JOIN dbo.NHANVIEN AS n ON n.MaNV=pc.MaNV AND n.Phg = '05'
    GROUP BY d.MaDA,d.TenDA
);
GO
