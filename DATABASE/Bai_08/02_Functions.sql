USE QL_DeAn;
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_DeAnNhieuNhanVien()
RETURNS TABLE AS RETURN
(
    SELECT d.MaDA,d.TenDA,COUNT(DISTINCT pc.MaNV) AS SoLuongNhanVien
    FROM dbo.B7_DeAn d JOIN dbo.B7_PhanCong pc ON pc.MaDA=d.MaDA
    GROUP BY d.MaDA,d.TenDA HAVING COUNT(DISTINCT pc.MaNV)>2
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_PhongNhieuNhanVienLuongCao()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPB,
           SUM(CASE WHEN n.Luong>25000 THEN 1 ELSE 0 END) AS SoNhanVienLuongTren25000
    FROM dbo.B7_PhongBan p JOIN dbo.B7_NhanVien n ON n.MaPB=p.MaPB
    GROUP BY p.MaPB HAVING COUNT(n.MaNV)>2
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_PhongLuongTBLon()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPB,p.TenPB,COUNT(n.MaNV) AS SoLuongNhanVien
    FROM dbo.B7_PhongBan p JOIN dbo.B7_NhanVien n ON n.MaPB=p.MaPB
    GROUP BY p.MaPB,p.TenPB HAVING AVG(n.Luong)>30000
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_PhongLuongTBLon_Nam()
RETURNS TABLE AS RETURN
(
    SELECT p.MaPB,p.TenPB,SUM(CASE WHEN n.GioiTinh=N'Nam' THEN 1 ELSE 0 END) 
    AS SoLuongNhanVienNam
    FROM dbo.B7_PhongBan p JOIN dbo.B7_NhanVien n ON n.MaPB=p.MaPB
    GROUP BY p.MaPB,p.TenPB HAVING AVG(n.Luong)>30000
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B8_DeAnCoNhanVienPhong5()
RETURNS TABLE AS RETURN
(
    SELECT d.MaDA,d.TenDA,
           COUNT(DISTINCT CASE WHEN n.MaPB='PB05' THEN pc.MaNV END) AS SoNhanVienPhong5
    FROM dbo.B7_DeAn d
    LEFT JOIN dbo.B7_PhanCong pc ON pc.MaDA=d.MaDA
    LEFT JOIN dbo.B7_NhanVien n ON n.MaNV=pc.MaNV
    GROUP BY d.MaDA,d.TenDA
);
GO

