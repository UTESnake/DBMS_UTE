USE QL_Gara;
GO
CREATE OR ALTER FUNCTION dbo.fn_B9_ThoKhongThamGiaHopDong()
RETURNS TABLE AS RETURN
(
    SELECT t.MaTho,t.TenTho,t.Nhom,t.NhomTruong
    FROM dbo.B9_THO t
    WHERE NOT EXISTS(SELECT 1 FROM dbo.B9_CHITIET_HD ct WHERE ct.MaTho=t.MaTho)
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B9_HopDongDaThanhLyChuaDuTien()
RETURNS TABLE AS RETURN
(
    SELECT h.SoHD,h.NgayHD,h.MaKH,k.TenKH,h.TriGiaHD,h.NgayNgThu,
           CAST(ISNULL(SUM(p.SoTienThu),0) AS decimal(18,2)) AS DaThanhToan,
           CAST(h.TriGiaHD-ISNULL(SUM(p.SoTienThu),0) AS decimal(18,2)) AS ConNo
    FROM dbo.B9_HOPDONG h JOIN dbo.B9_KHACHHANG k ON k.MaKH=h.MaKH
    LEFT JOIN dbo.B9_PHIEUTHU p ON p.SoHD=h.SoHD
    WHERE h.NgayNgThu IS NOT NULL
    GROUP BY h.SoHD,h.NgayHD,h.MaKH,k.TenKH,h.TriGiaHD,h.NgayNgThu
    HAVING ISNULL(SUM(p.SoTienThu),0)<h.TriGiaHD
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B9_HopDongCanHoanTatTruoc()
RETURNS TABLE AS RETURN
(
    SELECT h.SoHD,h.NgayHD,h.MaKH,k.TenKH,h.SoXe,h.NgayGiaoDK,h.NgayNgThu,h.TriGiaHD
    FROM dbo.B9_HOPDONG h JOIN dbo.B9_KHACHHANG k ON k.MaKH=h.MaKH
    WHERE h.NgayGiaoDK<CONVERT(date,'2002-12-31',23)
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B9_ThoNhieuCongViecNhat()
RETURNS TABLE AS RETURN
(
    SELECT MaTho,TenTho,Nhom,SoCongViec FROM
    (
        SELECT t.MaTho,t.TenTho,t.Nhom,COUNT(ct.MaCV) AS SoCongViec,
               DENSE_RANK() OVER(ORDER BY COUNT(ct.MaCV) DESC) AS ThuHang
        FROM dbo.B9_THO t LEFT JOIN dbo.B9_CHITIET_HD ct ON ct.MaTho=t.MaTho
        GROUP BY t.MaTho,t.TenTho,t.Nhom
    ) x WHERE ThuHang=1
);
GO
CREATE OR ALTER FUNCTION dbo.fn_B9_ThoTongTriGiaCaoNhat()
RETURNS TABLE AS RETURN
(
    SELECT MaTho,TenTho,Nhom,TongTriGia FROM
    (
        SELECT t.MaTho,t.TenTho,t.Nhom,CAST(ISNULL(SUM(ct.TriGiaCV),0) AS decimal(18,2))
         AS TongTriGia,
               DENSE_RANK() OVER(ORDER BY ISNULL(SUM(ct.TriGiaCV),0) DESC) AS ThuHang
        FROM dbo.B9_THO t LEFT JOIN dbo.B9_CHITIET_HD ct ON ct.MaTho=t.MaTho
        GROUP BY t.MaTho,t.TenTho,t.Nhom
    ) x WHERE ThuHang=1
);
GO


