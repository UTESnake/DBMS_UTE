USE master;
GO
IF DB_ID(N'QL_TruongPhoThong') IS NULL
    EXEC(N'CREATE DATABASE QL_TruongPhoThong');
GO
USE QL_TruongPhoThong;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Xóa các bảng cũ (bao gồm cả bảng phụ cũ nếu còn tồn tại trong DB)
DROP TABLE IF EXISTS dbo.B10_PHONG_LOP_THI, dbo.B10_LOP, dbo.B10_PC_COI_THI, dbo.B10_BUOITHI, dbo.B10_GV, dbo.B10_MHOC;
GO

CREATE TABLE dbo.B10_MHOC
(
    MaMH varchar(10) PRIMARY KEY,
    TenMH nvarchar(100) NOT NULL UNIQUE,
    SoTiet int NOT NULL CHECK(SoTiet>0)
);
GO

CREATE TABLE dbo.B10_GV
(
    MaGV varchar(10) PRIMARY KEY,
    TenGV nvarchar(100) NOT NULL,
    MaMH varchar(10) NULL REFERENCES dbo.B10_MHOC(MaMH)
);
GO

-- Đảm bảo mỗi môn chỉ có 1 chủ nhiệm và cho phép nhiều giáo viên có giá trị NULL
CREATE UNIQUE INDEX UX_B10_GV_MonChuNhiem ON dbo.B10_GV(MaMH) WHERE MaMH IS NOT NULL;
GO

CREATE TABLE dbo.B10_BUOITHI
(
    HKY tinyint NOT NULL CHECK(HKY BETWEEN 1 AND 3),
    Ngay date NOT NULL,
    Gio time(0) NOT NULL,
    Phg varchar(10) NOT NULL,
    MaMH varchar(10) NOT NULL REFERENCES dbo.B10_MHOC(MaMH),
    TGThi int NOT NULL CHECK(TGThi>0),
    CONSTRAINT PK_B10_BUOITHI PRIMARY KEY(HKY,Ngay,Gio,Phg)
);
GO

CREATE TABLE dbo.B10_PC_COI_THI
(
    MaGV varchar(10) NOT NULL REFERENCES dbo.B10_GV(MaGV),
    HKY tinyint NOT NULL,
    Ngay date NOT NULL,
    Gio time(0) NOT NULL,
    Phg varchar(10) NOT NULL,
    CONSTRAINT PK_B10_PC_COI_THI PRIMARY KEY(MaGV,HKY,Ngay,Gio,Phg),
    CONSTRAINT FK_B10_PC_BUOITHI FOREIGN KEY(HKY,Ngay,Gio,Phg) REFERENCES dbo.B10_BUOITHI(HKY,Ngay,Gio,Phg),
    CONSTRAINT UQ_B10_PC_KhongTrungGio UNIQUE(MaGV,HKY,Ngay,Gio)
);
GO

-- =========================================================
-- TRIGGER 1: KIỂM TRA BUỔI THI
-- Mục đích:
-- 1. Môn 30 tiết phải thi 120 phút.
-- 2. Môn >= 45 tiết phải thi 150 phút.
-- 3. Khi sửa buổi thi, không được làm cho giáo viên đang coi
--    trở thành người coi chính môn mình chủ nhiệm.
-- =========================================================

CREATE OR ALTER TRIGGER dbo.tg_B10_KiemTraBuoiThi
ON dbo.B10_BUOITHI
AFTER INSERT, UPDATE
AS
BEGIN
    -- Không hiển thị thông báo kiểu "(1 row affected)"
    SET NOCOUNT ON;

    -- Kiểm tra thời gian thi có đúng với số tiết của môn học hay không
    IF EXISTS
    (
        -- inserted chứa dữ liệu mới vừa INSERT
        -- hoặc dữ liệu mới sau UPDATE
        SELECT 1
        FROM inserted i

        -- JOIN sang bảng môn học để lấy SoTiet
        JOIN dbo.B10_MHOC m
            ON m.MaMH = i.MaMH

        -- Nếu môn 30 tiết nhưng thời gian thi khác 120
        -- HOẶC môn từ 45 tiết trở lên nhưng thời gian thi khác 150
        -- thì dữ liệu không hợp lệ
        WHERE (m.SoTiet = 30 AND i.TGThi <> 120)
           OR (m.SoTiet >= 45 AND i.TGThi <> 150)
    )
    BEGIN
        -- Phát sinh thông báo lỗi
        RAISERROR(
            N'Môn 30 tiết phải thi 120 phút;môn từ 45 tiết phải thi 150 phút.',
            16,1
        );

        -- Hủy thao tác INSERT hoặc UPDATE vừa thực hiện
        ROLLBACK TRANSACTION;

        -- Dừng trigger tại đây
        RETURN;
    END

    -- Kiểm tra giáo viên đang coi buổi thi có phải
    -- là giáo viên chủ nhiệm chính môn đó hay không
    IF EXISTS
    (
        SELECT 1
        FROM inserted i

        -- Tìm các giáo viên đã được phân công coi buổi thi này
        JOIN dbo.B10_PC_COI_THI pc
            ON pc.HKY = i.HKY
           AND pc.Ngay = i.Ngay
           AND pc.Gio = i.Gio
           AND pc.Phg = i.Phg

        -- Từ mã giáo viên tìm thông tin giáo viên
        JOIN dbo.B10_GV g
            ON g.MaGV = pc.MaGV

        -- Nếu môn giáo viên chủ nhiệm bằng môn của buổi thi
        -- thì vi phạm
        WHERE g.MaMH = i.MaMH
    )
    BEGIN
        RAISERROR(
            N'Giáo viên không được gác thi môn do mình chủ nhiệm.',
            16,1
        );

        ROLLBACK TRANSACTION;
    END
END;
GO

-- =========================================================
-- TRIGGER 2: KIỂM TRA PHÂN CÔNG COI THI
-- Mục đích:
-- Khi thêm hoặc sửa phân công coi thi,
-- giáo viên không được coi môn mình chủ nhiệm.
-- =========================================================

CREATE OR ALTER TRIGGER dbo.tg_B10_KiemTraPhanCong
ON dbo.B10_PC_COI_THI
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i

        -- Tìm giáo viên vừa được phân công
        JOIN dbo.B10_GV g
            ON g.MaGV = i.MaGV

        -- Tìm buổi thi tương ứng
        JOIN dbo.B10_BUOITHI b
            ON b.HKY = i.HKY
           AND b.Ngay = i.Ngay
           AND b.Gio = i.Gio
           AND b.Phg = i.Phg

        -- Nếu môn giáo viên chủ nhiệm = môn đang thi
        -- thì không được phép phân công
        WHERE g.MaMH = b.MaMH
    )
    BEGIN
        RAISERROR(
            N'Giáo viên không được gác thi môn do mình chủ nhiệm.',
            16,1
        );

        ROLLBACK TRANSACTION;
    END
END;
GO

-- =========================================================
-- TRIGGER 3: KIỂM TRA KHI ĐỔI MÔN CHỦ NHIỆM CỦA GIÁO VIÊN
-- Mục đích:
-- Không cho sửa MaMH của giáo viên nếu việc sửa đó
-- làm các phân công coi thi hiện có trở thành vi phạm.
-- =========================================================

CREATE OR ALTER TRIGGER dbo.tg_B10_KiemTraGiaoVien
ON dbo.B10_GV
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ thực hiện kiểm tra nếu câu UPDATE có tác động đến MaMH
    IF UPDATE(MaMH) AND EXISTS
    (
        SELECT 1
        FROM inserted i

        -- Tìm tất cả các buổi mà giáo viên này đang được phân công coi
        JOIN dbo.B10_PC_COI_THI pc
            ON pc.MaGV = i.MaGV

        -- Tìm môn của từng buổi thi
        JOIN dbo.B10_BUOITHI b
            ON b.HKY = pc.HKY
           AND b.Ngay = pc.Ngay
           AND b.Gio = pc.Gio
           AND b.Phg = pc.Phg

        -- Nếu môn chủ nhiệm mới của GV
        -- trùng với môn của một buổi GV đang coi
        -- thì vi phạm
        WHERE i.MaMH = b.MaMH
    )
    BEGIN
        RAISERROR(
            N'Đổi môn chủ nhiệm làm vi phạm phân công coi thi hiện có.',
            16,1
        );

        ROLLBACK TRANSACTION;
    END
END;
GO

-- =========================================================
-- TRIGGER 4: KIỂM TRA KHI ĐỔI SỐ TIẾT MÔN HỌC
-- Mục đích:
-- Không cho sửa SoTiet nếu các buổi thi hiện có
-- trở thành sai thời gian thi.
-- =========================================================

CREATE OR ALTER TRIGGER dbo.tg_B10_KiemTraSoTiet
ON dbo.B10_MHOC
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ kiểm tra nếu câu UPDATE có tác động đến cột SoTiet
    IF UPDATE(SoTiet) AND EXISTS
    (
        SELECT 1
        FROM inserted i

        -- Tìm tất cả buổi thi của môn vừa bị thay đổi số tiết
        JOIN dbo.B10_BUOITHI b
            ON b.MaMH = i.MaMH

        -- Kiểm tra thời gian thi hiện tại còn phù hợp
        -- với số tiết mới hay không
        WHERE (i.SoTiet = 30 AND b.TGThi <> 120)
           OR (i.SoTiet >= 45 AND b.TGThi <> 150)
    )
    BEGIN
        RAISERROR(
            N'Đổi số tiết làm thời gian thi hiện có không còn hợp lệ.',
            16,1
        );

        ROLLBACK TRANSACTION;
    END
END;
GO

-- Dữ liệu phủ các biên thời lượng: dưới 30, đúng 30, 31-44, đúng 45 và trên 45 tiết.
INSERT dbo.B10_MHOC VALUES
('MH01',N'VĂN HỌC',45),('MH02',N'TOÁN',30),('MH03',N'TIẾNG ANH',60),
('MH04',N'TIN HỌC',20),('MH05',N'VẬT LÝ',29),('MH06',N'HÓA HỌC',44),
('MH07',N'SINH HỌC',46),('MH08',N'ĐỊA LÝ',31);
INSERT dbo.B10_GV VALUES
('GV01',N'Nguyễn Thị An','MH01'),('GV02',N'Trần Văn Bình','MH02'),('GV03',N'Lê Minh Châu','MH03'),
('GV04',N'Phạm Quốc Dũng',NULL),('GV05',N'Võ Thanh Hà',NULL),('GV06',N'Đỗ Gia Khang','MH04'),
('GV07',N'Bùi Ngọc Lan','MH05'),('GV08',N'Hồ Minh Quân','MH06'),('GV09',N'Đặng Thu Trang','MH07'),
('GV10',N'Lý Quốc Việt',NULL);
INSERT dbo.B10_BUOITHI VALUES
(1,'2026-05-10','07:30','P101','MH01',150),(1,'2026-05-10','13:30','P102','MH02',120),
(1,'2026-05-10','07:30','P109','MH06',90),
(1,'2026-05-11','07:30','P103','MH03',150),(1,'2026-05-11','13:30','P104','MH04',90),
(1,'2026-05-12','07:30','P105','MH05',60),(1,'2026-05-12','13:30','P106','MH06',90),
(1,'2026-05-13','07:30','P107','MH07',150),(1,'2026-05-13','13:30','P108','MH08',90),
(1,'2026-05-14','07:30','P110','MH03',150),
(2,'2026-12-10','07:30','P201','MH01',150),(2,'2026-12-10','13:30','P202','MH02',120),
(2,'2026-12-11','07:30','P203','MH01',150),(2,'2026-12-11','13:30','P204','MH06',90),
(3,'2027-05-10','07:30','P301','MH03',150);
INSERT dbo.B10_PC_COI_THI VALUES
('GV02',1,'2026-05-10','07:30','P101'),('GV04',1,'2026-05-10','07:30','P101'),
('GV09',1,'2026-05-10','07:30','P109'),
('GV01',1,'2026-05-10','13:30','P102'),('GV03',1,'2026-05-10','13:30','P102'),
('GV06',1,'2026-05-11','07:30','P103'),('GV01',1,'2026-05-11','13:30','P104'),
('GV04',1,'2026-05-12','07:30','P105'),('GV08',1,'2026-05-12','07:30','P105'),
('GV01',1,'2026-05-12','13:30','P106'),('GV09',1,'2026-05-12','13:30','P106'),
('GV02',1,'2026-05-13','07:30','P107'),('GV04',1,'2026-05-13','13:30','P108'),
('GV05',2,'2026-12-10','07:30','P201'),('GV10',2,'2026-12-11','07:30','P203'),
('GV05',2,'2026-12-11','13:30','P204'),
('GV01',1,'2026-05-14','07:30','P110'),
('GV01',3,'2027-05-10','07:30','P301');
GO
