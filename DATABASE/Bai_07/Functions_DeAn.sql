USE QL_DeAn;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_LuongTrungBinhPhong(@MaPhg varchar(2))
RETURNS decimal(18,2)
AS
BEGIN
    RETURN
    (
        SELECT COALESCE(CAST(AVG(b.LuongCoBan) AS decimal(18,2)), 0)
        FROM dbo.NHANVIEN AS n
        JOIN dbo.BANGLUONG AS b ON b.MaNV=n.MaNV
        WHERE n.Phg=@MaPhg
    );
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_TongLuongNhanVienDeAn(@MaNV varchar(9), @MaDA varchar(2))
RETURNS decimal(18,2)
AS
BEGIN
    -- Declare: khai báo biến cục bộ
    DECLARE @Time_Total decimal(18,2);
    DECLARE @KetQua decimal(18,2);

    SELECT @Time_Total=SUM(ThoiGian)
    FROM dbo.PHANCONG WHERE MaNV=@MaNV;

    SELECT @KetQua=CAST(
        CAST(b.LuongCoBan AS decimal(18,2))*CAST(pc.ThoiGian AS decimal(18,2))
        /NULLIF(@Time_Total,0) AS decimal(18,2))
    FROM dbo.BANGLUONG AS b
    JOIN dbo.PHANCONG AS pc ON pc.MaNV=b.MaNV
    WHERE b.MaNV=@MaNV AND pc.SoDA=@MaDA;

    RETURN COALESCE(@KetQua, 0);
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_TongLuongTrungBinhCacPhong()
RETURNS decimal(18,2)
AS
BEGIN
    RETURN
    (
        SELECT COALESCE(CAST(SUM(LuongTB) AS decimal(18,2)), 0)
        FROM
        (
            SELECT AVG(b.LuongCoBan) AS LuongTB
            FROM dbo.NHANVIEN AS n
            JOIN dbo.BANGLUONG AS b ON b.MaNV=n.MaNV
            WHERE n.Phg IS NOT NULL
            GROUP BY n.Phg
        ) AS x
    );
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_TienThuong(@Time_Total decimal(38,2))
RETURNS decimal(18,2)
AS
BEGIN
    RETURN CASE
        WHEN @Time_Total>=30 AND @Time_Total<=60 THEN 500
        WHEN @Time_Total>60 AND @Time_Total<100 THEN 1000
        WHEN @Time_Total>=100 AND @Time_Total<150 THEN 1200
        WHEN @Time_Total>=150 THEN 1600
        ELSE 0
    END;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_SoDeAnTheoPhong()
RETURNS TABLE
AS
RETURN
(
    SELECT pb.MaPhg,pb.TenPhg,COUNT(da.MaDA) AS SoDeAn
    FROM dbo.PHONGBAN AS pb
    LEFT JOIN dbo.DEAN AS da ON da.Phong=pb.MaPhg
    GROUP BY pb.MaPhg,pb.TenPhg
);
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_ThongTinNhanVien_Inline()
RETURNS TABLE
AS
RETURN
(
    SELECT n.MaNV,
           CONCAT_WS(N' ',n.HoNV,n.TenLot,n.TenNV) AS HoTen,
           CAST(n.NgSinh AS date) AS NgSinh,
           tn.NguoiThan,
           dbo.fn_B7_LuongTrungBinhPhong(n.Phg) AS TongLuongTB
    FROM dbo.NHANVIEN AS n
    OUTER APPLY
    (
        SELECT STRING_AGG(
            CAST(t.TenTN+N' ('+COALESCE(t.QuanHe,N'')+N')' AS nvarchar(max)),N', ')
            WITHIN GROUP (ORDER BY t.TenTN) AS NguoiThan
        FROM dbo.THANNHAN AS t WHERE t.MaNV=n.MaNV
    ) AS tn
);
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_ThongTinNhanVien_Multi()
RETURNS @KetQua TABLE
(
    MaNV varchar(9),
    HoTen nvarchar(100),
    NgSinh date,
    NguoiThan nvarchar(max),
    TongLuongTB decimal(18,2)
)
AS
BEGIN
    INSERT @KetQua(MaNV, HoTen, NgSinh, NguoiThan, TongLuongTB)
    SELECT n.MaNV,
           CONCAT_WS(N' ',n.HoNV,n.TenLot,n.TenNV),
           CAST(n.NgSinh AS date),
           tn.NguoiThan,
           dbo.fn_B7_LuongTrungBinhPhong(n.Phg)
    FROM dbo.NHANVIEN AS n
    OUTER APPLY
    (
        SELECT STRING_AGG(
            CAST(t.TenTN+N' ('+COALESCE(t.QuanHe,N'')+N')' AS nvarchar(max)),N', ')
            WITHIN GROUP (ORDER BY t.TenTN) AS NguoiThan
        FROM dbo.THANNHAN AS t WHERE t.MaNV=n.MaNV
    ) AS tn;
    RETURN;
END;
GO
