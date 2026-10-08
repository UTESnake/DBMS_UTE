USE [QL_ThuVien]
GO

/*========================================================
  BÀI 6: TẠO CÁC TRIGGER CHO CSDL QUẢN LÝ THƯ VIỆN
========================================================*/


/*********************************************************
  6.1. TRIGGER tg_delMuon

  Nội dung:
  Khi xóa phiếu mượn trong bảng Muon
  -> cập nhật tình trạng cuốn sách thành "Có sẵn"

  Ý nghĩa:
  Khi độc giả trả sách và xóa phiếu mượn
  thì cuốn sách được trả về trạng thái có thể mượn.
*********************************************************/

CREATE OR ALTER TRIGGER dbo.tg_delMuon
ON dbo.Muon
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra cuốn sách trong phiếu mượn phải tồn tại trong thư viện
    IF EXISTS
    (
        SELECT 1
        FROM deleted d
        LEFT JOIN dbo.Cuonsach cs
            ON cs.isbn = d.isbn AND cs.ma_cuonsach = d.ma_cuonsach
        WHERE cs.isbn IS NULL
    )
    BEGIN
        RAISERROR(N'Cuốn sách trong phiếu mượn không tồn tại trong thư viện.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    UPDATE cs
    SET tinhtrang = N'Có sẵn'
    FROM dbo.Cuonsach cs
    INNER JOIN deleted d
        ON cs.isbn = d.isbn
       AND cs.ma_cuonsach = d.ma_cuonsach;

END;
GO



/*********************************************************
  6.2. TRIGGER tg_insMuon

  Nội dung:
  Khi thêm mới phiếu mượn
  -> cập nhật tình trạng cuốn sách thành "Đang mượn"

  Ý nghĩa:
  Khi độc giả mượn sách thì cuốn sách đó
  không còn trong trạng thái có sẵn.
*********************************************************/

CREATE OR ALTER TRIGGER dbo.tg_insMuon
ON dbo.Muon
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Kiểm tra cuốn sách phải ở trạng thái 'Có sẵn'
    IF EXISTS
    (
        SELECT 1
        FROM inserted AS i
        INNER JOIN dbo.Cuonsach AS cs
            ON cs.isbn = i.isbn AND cs.ma_cuonsach = i.ma_cuonsach
        WHERE cs.tinhtrang <> N'Có sẵn' OR cs.tinhtrang IS NULL
    )
    BEGIN
        RAISERROR(N'Cuốn sách không ở trạng thái Có sẵn để cho mượn (đang được mượn hoặc hỏng/mất).', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- 2. Kiểm tra ngày hết hạn không được nhỏ hơn ngày mượn
    IF EXISTS
    (
        SELECT 1
        FROM inserted AS i
        WHERE i.ngay_hethan < i.ngay_muon
    )
    BEGIN
        RAISERROR(N'Ngày hết hạn trả sách không thể nhỏ hơn ngày mượn.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    UPDATE cs
    SET tinhtrang = N'Đang mượn'
    FROM dbo.Cuonsach cs
    INNER JOIN inserted i
        ON cs.isbn = i.isbn
       AND cs.ma_cuonsach = i.ma_cuonsach;

END;
GO



/*********************************************************
  6.3. TRIGGER tg_updCuonSach

  Nội dung:
  Khi thuộc tính tình trạng trên bảng Cuonsach
  được cập nhật
  -> cập nhật trạng thái của Dausach theo.

  Quy tắc:
  - Nếu còn ít nhất 1 cuốn "Có sẵn"
        -> Đầu sách "Đang phục vụ"

  - Nếu tất cả cuốn đều "Đang mượn"
        -> Đầu sách "Ngừng phục vụ"
*********************************************************/

CREATE OR ALTER TRIGGER dbo.tg_updCuonSach
ON dbo.Cuonsach
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi cột tinhtrang thay đổi
    IF NOT UPDATE(tinhtrang)
        RETURN;

    -- Kiểm tra miền giá trị hợp lệ của tinhtrang
    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE tinhtrang IS NULL
           OR tinhtrang NOT IN (N'Có sẵn', N'Đang mượn', N'Hỏng', N'Mất')
    )
    BEGIN
        RAISERROR(N'Tình trạng cuốn sách không hợp lệ (chỉ chấp nhận: Có sẵn, Đang mượn, Hỏng, Mất).', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- Lấy danh sách đầu sách bị ảnh hưởng
    ;WITH Affected AS
    (
        SELECT isbn
        FROM inserted

        UNION

        SELECT isbn
        FROM deleted
    )

    UPDATE ds
    SET trangthai =
        CASE
            -- Nếu còn cuốn sách có sẵn
            WHEN EXISTS
            (
                SELECT 1
                FROM dbo.Cuonsach cs
                WHERE cs.isbn = ds.isbn
                AND cs.tinhtrang = N'Có sẵn'
            )
            THEN N'Đang phục vụ'

            -- Không còn cuốn nào có sẵn
            ELSE N'Ngừng phục vụ'
        END
    FROM dbo.Dausach ds
    INNER JOIN Affected a
        ON ds.isbn = a.isbn;

END;
GO




/*********************************************************
  6.4. TRIGGER tg_InfThongBao

  Nội dung:
  Khi:
      - Thêm mới tựa sách
      - Sửa tên tác giả
      - Sửa tên tựa sách

  -> In ra thông báo tiếng Việt.
*********************************************************/

CREATE OR ALTER TRIGGER dbo.tg_InfThongBao
ON dbo.Tuasach
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra tựa sách không được để trống
    IF EXISTS
    (
        SELECT 1
        FROM inserted
        WHERE tuasach IS NULL OR LTRIM(RTRIM(tuasach)) = ''
    )
    BEGIN
        RAISERROR(N'Tên tựa sách không được để trống.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    /*
       Kiểm tra thêm mới tựa sách
       inserted có dữ liệu nhưng deleted không có
       => dữ liệu mới được thêm vào
    */
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        LEFT JOIN deleted d
            ON i.ma_tuasach = d.ma_tuasach
        WHERE d.ma_tuasach IS NULL
    )
    BEGIN
        PRINT N'Đã thêm mới tựa sách';
    END;

    /*
       Kiểm tra sửa tên tác giả
    */
    IF UPDATE(tacgia)
       AND EXISTS
       (
            SELECT 1
            FROM deleted
       )
    BEGIN
        PRINT N'Đã sửa tên tác giả';
    END;

    /*
       Kiểm tra sửa tên tựa sách
    */
    IF UPDATE(tuasach)
       AND EXISTS
       (
            SELECT 1
            FROM deleted
       )
    BEGIN
        PRINT N'Đã sửa tựa sách';
    END;

END;
GO



/*********************************************************
  KIỂM TRA DANH SÁCH TRIGGER ĐÃ TẠO
*********************************************************/

SELECT 
    name AS TenTrigger
FROM sys.triggers
WHERE parent_id IN
(
    OBJECT_ID('dbo.Muon'),
    OBJECT_ID('dbo.Cuonsach'),
    OBJECT_ID('dbo.Tuasach')
);
GO
