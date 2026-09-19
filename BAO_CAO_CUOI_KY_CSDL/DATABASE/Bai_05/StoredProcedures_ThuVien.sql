USE [QL_ThuVien]
GO

-- ============================================================
-- CÂU 5A: Liệt kê thông tin của độc giả theo mã độc giả.
-- TÊN: sp_ThongtinDocGia
-- Xử lý:
-- 1. Kiểm tra mã độc giả có NULL/rỗng không.
-- 2. Kiểm tra độc giả có tồn tại không.
-- 3. Nếu là người lớn:
--      trả về thông tin DocGia + Nguoilon.
-- 4. Nếu là trẻ em:
--      trả về thông tin DocGia + Treem.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_ThongtinDocGia
    @MaDocGia VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Nếu mã độc giả là NULL hoặc chuỗi rỗng thì báo lỗi.
    IF @MaDocGia IS NULL
       OR LTRIM(RTRIM(@MaDocGia)) = ''
    BEGIN
        RAISERROR(
            N'Mã độc giả không được để trống.',
            16,
            1
        );
        RETURN;
    END;

    -- BƯỚC 2: LOẠI BỎ KHOẢNG TRẮNG THỪA
    SET @MaDocGia = LTRIM(RTRIM(@MaDocGia));

    -- KIỂM TRA ĐỘC GIẢ CÓ TỒN TẠI KHÔNG
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.DocGia
        WHERE ma_DocGia = @MaDocGia
    )
    BEGIN
        RAISERROR(
            N'Không tìm thấy độc giả có mã này.',
            16,
            1
        );
        RETURN;
    END;

    -- KIỂM TRA ĐỘC GIẢ CÓ PHẢI NGƯỜI LỚN KHÔNG
    IF EXISTS
    (
        SELECT 1
        FROM dbo.Nguoilon
        WHERE ma_DocGia = @MaDocGia
    )
    BEGIN
        SELECT
            dg.ma_DocGia AS MaDocGia,
            dg.ho AS Ho,
            dg.tenlot AS TenLot,
            dg.ten AS Ten,
            dg.ngaysinh AS NgaySinh,

            nl.sonha AS SoNha,
            nl.duong AS Duong,
            nl.quan AS Quan,
            nl.dienthoai AS DienThoai,
            nl.han_sd AS HanSuDung,

            N'Người lớn' AS LoaiDocGia

        FROM dbo.DocGia AS dg

        INNER JOIN dbo.Nguoilon AS nl
            ON dg.ma_DocGia = nl.ma_DocGia

        WHERE dg.ma_DocGia = @MaDocGia;

        RETURN;
    END;

    -- KIỂM TRA ĐỘC GIẢ CÓ PHẢI TRẺ EM KHÔNG
    IF EXISTS
    (
        SELECT 1
        FROM dbo.Treem
        WHERE ma_DocGia = @MaDocGia
    )
    BEGIN
        SELECT
            dg.ma_DocGia AS MaDocGia,
            dg.ho AS Ho,
            dg.tenlot AS TenLot,
            dg.ten AS Ten,
            dg.ngaysinh AS NgaySinh,

            te.ma_DocGia_nguoilon AS MaDocGiaNguoiLon,

            N'Trẻ em' AS LoaiDocGia

        FROM dbo.DocGia AS dg

        INNER JOIN dbo.Treem AS te
            ON dg.ma_DocGia = te.ma_DocGia

        WHERE dg.ma_DocGia = @MaDocGia;

        RETURN;
    END;

    -- TRƯỜNG HỢP DỮ LIỆU BẤT THƯỜNG: Có trong DocGia nhưng không có trong Nguoilon hoặc Treem.
    RAISERROR(
        N'Độc giả tồn tại nhưng chưa được phân loại.',
        16,
        1
    );
END;
GO


-- ============================================================
-- CÂU 5B
-- Liệt kê:
-- 1. Thông tin đầu sách.
-- 2. Thông tin tựa sách.
-- 3. Số lượng cuốn sách hiện chưa được mượn.

-- TÊN: sp_ThongtinDausach
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_ThongtinDausach
    @ISBN VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- KIỂM TRA ISBN NULL HOẶC RỖNG
    IF @ISBN IS NULL
       OR LTRIM(RTRIM(@ISBN)) = ''
    BEGIN
        RAISERROR(
            N'ISBN không được để trống.',
            16,
            1
        );
        RETURN;
    END;

    -- LOẠI BỎ KHOẢNG TRẮNG THỪA
    SET @ISBN = LTRIM(RTRIM(@ISBN));

    -- KIỂM TRA ISBN CÓ TỒN TẠI TRONG Dausach KHÔNG
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

    -- LẤY THÔNG TIN ĐẦU SÁCH + TỰA SÁCH
    --
    -- Dausach nối Tuasach qua ma_tuasach.
    -- Dausach nối Cuonsach qua isbn.
    --
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
        ts.tuasach,ts.tacgia,ts.tomtat,
        ds.ngonngu, ds.bia, ds.trangthai;
END;
GO



-- ============================================================
-- CÂU 5C: Liệt kê tất cả độc giả NGƯỜI LỚN hiện đang mượn sách.
-- TÊN: sp_ThongtinNguoilonDangmuon

-- Cách xác định:
-- - Có trong bảng Nguoilon.
-- - Có ít nhất một bản ghi trong bảng Muon.
--
-- DISTINCT:
-- Một độc giả có thể mượn nhiều sách.
-- DISTINCT giúp mỗi người lớn chỉ xuất hiện một lần.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_ThongtinNguoilonDangmuon
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        dg.ma_DocGia AS MaDocGia,
        dg.ho AS Ho,
        dg.tenlot AS TenLot,
        dg.ten AS Ten,
        dg.ngaysinh AS NgaySinh,

        nl.sonha AS SoNha,
        nl.duong AS Duong,
        nl.quan AS Quan,
        nl.dienthoai AS DienThoai,
        nl.han_sd AS HanSuDung

    FROM dbo.DocGia AS dg

    -- Chỉ lấy các độc giả thuộc nhóm người lớn.
    INNER JOIN dbo.Nguoilon AS nl
        ON dg.ma_DocGia = nl.ma_DocGia

    -- Có bản ghi trong Muon nghĩa là người này đang mượn sách.
    INNER JOIN dbo.Muon AS m
        ON dg.ma_DocGia = m.ma_DocGia

    ORDER BY dg.ma_DocGia;
END;
GO



-- ============================================================
-- CÂU 5D: Liệt kê độc giả NGƯỜI LỚN vẫn giữ sách sau ngày hết hạn.
-- TÊN: sp_ThongtinNguoilonQuahan
-- Công thức:
-- Thời hạn mượn 14 ngày đã được biểu diễn bởi cột ngay_hethan.
-- Vì vậy đến hạn hôm nay chưa quá hạn; từ ngày kế tiếp mới quá hạn.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_ThongtinNguoilonQuahan
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        dg.ma_DocGia AS MaDocGia, dg.ho AS Ho, dg.tenlot AS TenLot,
        dg.ten AS Ten, dg.ngaysinh AS NgaySinh,
        nl.sonha AS SoNha, nl.duong AS Duong, nl.quan AS Quan,
        nl.dienthoai AS DienThoai, nl.han_sd AS HanSuDung,
        m.isbn AS ISBN, m.ma_cuonsach AS MaCuonSach,
        m.ngay_muon AS NgayMuon, m.ngay_hethan AS NgayHetHan,
        DATEDIFF(DAY, m.ngay_hethan, CAST(GETDATE() AS DATE)) AS SoNgayQuaHan

    FROM dbo.DocGia AS dg

    -- Chỉ xét độc giả người lớn.
    INNER JOIN dbo.Nguoilon AS nl
        ON dg.ma_DocGia = nl.ma_DocGia

    INNER JOIN dbo.Muon AS m
        ON m.ma_DocGia = dg.ma_DocGia
       AND m.ngay_hethan < CAST(GETDATE() AS DATE)

    ORDER BY SoNgayQuaHan DESC, dg.ma_DocGia, m.isbn, m.ma_cuonsach;
END;
GO



-- ============================================================
-- CÂU 5E: Liệt kê những độc giả người lớn đang mượn sách có trẻ em cũng đang mượn sách
-- TÊN: sp_DocGiaCoTreEmMuon
-- Quan hệ:
-- Nguoilon
--    |
--    | ma_DocGia
--    v
-- Treem.ma_DocGia_nguoilon
--
-- Sau đó:
-- Treem.ma_DocGia -> Muon.ma_DocGia
--
-- Điều kiện cuối: cả người lớn và trẻ em đều phải có bản ghi trong Muon.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_DocGiaCoTreEmMuon
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT

        -- THÔNG TIN NGƯỜI LỚN
        dgNL.ma_DocGia AS MaDocGiaNguoiLon,
        dgNL.ho AS HoNguoiLon,
        dgNL.tenlot AS TenLotNguoiLon,
        dgNL.ten AS TenNguoiLon,
        dgNL.ngaysinh AS NgaySinhNguoiLon,

        nl.sonha AS SoNha,
        nl.duong AS Duong,
        nl.quan AS Quan,
        nl.dienthoai AS DienThoai,
        nl.han_sd AS HanSuDung,

        -- THÔNG TIN TRẺ EM ĐƯỢC NGƯỜI LỚN BẢO LÃNH
        dgTE.ma_DocGia AS MaDocGiaTreEm,
        dgTE.ho AS HoTreEm,
        dgTE.tenlot AS TenLotTreEm,
        dgTE.ten AS TenTreEm,
        dgTE.ngaysinh AS NgaySinhTreEm

    FROM dbo.Nguoilon AS nl

    -- LẤY THÔNG TIN CHUNG CỦA NGƯỜI LỚN
    INNER JOIN dbo.DocGia AS dgNL
        ON nl.ma_DocGia = dgNL.ma_DocGia

    -- NGƯỜI LỚN PHẢI ĐANG MƯỢN SÁCH
    INNER JOIN dbo.Muon AS mNL
        ON nl.ma_DocGia = mNL.ma_DocGia

    -- TÌM CÁC TRẺ EM DO NGƯỜI LỚN NÀY BẢO LÃNH
    INNER JOIN dbo.Treem AS te
        ON nl.ma_DocGia = te.ma_DocGia_nguoilon

    -- LẤY THÔNG TIN CHUNG CỦA TRẺ EM
    INNER JOIN dbo.DocGia AS dgTE
        ON te.ma_DocGia = dgTE.ma_DocGia

    -- TRẺ EM CŨNG PHẢI ĐANG MƯỢN SÁCH
    INNER JOIN dbo.Muon AS mTE
        ON te.ma_DocGia = mTE.ma_DocGia

    ORDER BY
        dgNL.ma_DocGia,
        dgTE.ma_DocGia;
END;
GO
