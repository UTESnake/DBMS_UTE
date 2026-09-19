USE QL_DeAn;
GO

/* File cài đặt Bài 7 cho luồng chạy 01 -> 02 -> 03 của Nhóm 3. */

CREATE OR ALTER FUNCTION dbo.fn_B7_LuongTrungBinhPhong(@MaPB varchar(10))
RETURNS decimal(18,2)
AS
BEGIN
    RETURN
    (
        SELECT COALESCE(CAST(AVG(Luong) AS decimal(18,2)), 0)
        FROM dbo.B7_NhanVien
        WHERE MaPB = @MaPB
    );
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_TongLuongNhanVienDeAn(@MaNV varchar(10), @MaDA varchar(10))
RETURNS decimal(18,2)
AS
BEGIN
    DECLARE @TongGio decimal(18,2);
    DECLARE @KetQua decimal(18,2);
    SELECT @TongGio = SUM(SoGio) FROM dbo.B7_PhanCong WHERE MaNV = @MaNV;

    SELECT @KetQua = CAST(nv.Luong * pc.SoGio / NULLIF(@TongGio, 0) AS decimal(18,2))
        FROM dbo.B7_NhanVien AS nv
        LEFT JOIN dbo.B7_PhanCong AS pc
          ON pc.MaNV = nv.MaNV
         AND pc.MaDA = @MaDA
        WHERE nv.MaNV = @MaNV;

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
            SELECT AVG(Luong) AS LuongTB
            FROM dbo.B7_NhanVien
            WHERE MaPB IS NOT NULL
            GROUP BY MaPB
        ) AS x
    );
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_TienThuong(@Time_Total decimal(10,2))
RETURNS decimal(18,2)
AS
BEGIN
    RETURN CASE
        WHEN @Time_Total IS NULL OR @Time_Total < 30 THEN 0
        WHEN @Time_Total <= 60 THEN 500
        WHEN @Time_Total < 100 THEN 1000
        WHEN @Time_Total < 150 THEN 1200
        ELSE 1600
    END;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_SoDeAnTheoPhong()
RETURNS TABLE
AS
RETURN
(
    SELECT pb.MaPB, pb.TenPB, COUNT(da.MaDA) AS SoDeAn
    FROM dbo.B7_PhongBan AS pb
    LEFT JOIN dbo.B7_DeAn AS da ON da.MaPB = pb.MaPB
    GROUP BY pb.MaPB, pb.TenPB
);
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_ThongTinNhanVien_Inline()
RETURNS TABLE
AS
RETURN
(
    SELECT nv.MaNV,
           nv.HoTen,
           nv.NgaySinh,
           STRING_AGG(tn.HoTen + N' (' + COALESCE(tn.QuanHe, N'') + N')', N', ')
             WITHIN GROUP (ORDER BY tn.HoTen) AS NguoiThan,
           dbo.fn_B7_LuongTrungBinhPhong(nv.MaPB) AS TongLuongTB
    FROM dbo.B7_NhanVien AS nv
    LEFT JOIN dbo.B7_ThanNhan AS tn ON tn.MaNV = nv.MaNV
    GROUP BY nv.MaNV, nv.HoTen, nv.NgaySinh, nv.MaPB
);
GO

CREATE OR ALTER FUNCTION dbo.fn_B7_ThongTinNhanVien_Multi()
RETURNS @KetQua TABLE
(
    MaNV varchar(10),
    HoTen nvarchar(100),
    NgaySinh date,
    NguoiThan nvarchar(max),
    TongLuongTB decimal(18,2)
)
AS
BEGIN
    INSERT @KetQua(MaNV, HoTen, NgaySinh, NguoiThan, TongLuongTB)
    SELECT nv.MaNV,
           nv.HoTen,
           nv.NgaySinh,
           STRING_AGG(tn.HoTen + N' (' + COALESCE(tn.QuanHe, N'') + N')', N', ')
             WITHIN GROUP (ORDER BY tn.HoTen),
           dbo.fn_B7_LuongTrungBinhPhong(nv.MaPB)
    FROM dbo.B7_NhanVien AS nv
    LEFT JOIN dbo.B7_ThanNhan AS tn ON tn.MaNV = nv.MaNV
    GROUP BY nv.MaNV, nv.HoTen, nv.NgaySinh, nv.MaPB;
    RETURN;
END;
GO
