USE [QL_ThuVien]
GO
/****** Object:  StoredProcedure [dbo].[sp_ThongTinDauSach]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_ThongTinDauSach]
    @ISBN VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra ISBN rỗng hoặc NULL
    IF @ISBN IS NULL OR LTRIM(RTRIM(@ISBN)) = ''
    BEGIN
        RAISERROR(
            N'ISBN không được để trống.',
            16,
            1
        );
        RETURN;
    END;

    -- Loại bỏ khoảng trắng đầu và cuối
    SET @ISBN = LTRIM(RTRIM(@ISBN));

    -- Kiểm tra ISBN có tồn tại hay không
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Dausach
        WHERE isbn = @ISBN
    )
    BEGIN
        RAISERROR(
            N'Không tìm thấy đầu sách có ISBN này.',
            16,
            1
        );
        RETURN;
    END;

    -- Trả về thông tin đầu sách,
    -- thông tin tựa sách và số lượng chưa mượn
    SELECT
        ds.isbn AS ISBN,
        ds.ma_tuasach AS MaTuaSach,
        ts.tuasach AS TuaSach,
        ts.tacgia AS TacGia,
        ts.tomtat AS TomTat,
        ds.ngonngu AS NgonNgu,
        ds.bia AS Bia,
        ds.trangthai AS TrangThai,

        SUM
        (
            CASE
                WHEN cs.tinhtrang = N'Có sẵn'
                    THEN 1
                ELSE 0
            END
        ) AS SoLuongChuaMuon

    FROM dbo.Dausach AS ds

    INNER JOIN dbo.Tuasach AS ts
        ON ds.ma_tuasach = ts.ma_tuasach

    LEFT JOIN dbo.Cuonsach AS cs
        ON ds.isbn = cs.isbn

    WHERE ds.isbn = @ISBN

    GROUP BY
        ds.isbn, ds.ma_tuasach,
        ts.tuasach,ts.tacgia, ts.tomtat,
        ds.ngonngu, ds.bia, ds.trangthai;
END;
GO
