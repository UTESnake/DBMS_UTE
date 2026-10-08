USE master;
GO
IF DB_ID(N'QL_Gara') IS NULL
    EXEC(N'CREATE DATABASE QL_Gara');
GO
USE QL_Gara;
GO
DROP TABLE IF EXISTS dbo.B9_PHIEUTHU,dbo.B9_CHITIET_HD,dbo.B9_HOPDONG,dbo.B9_CONGVIEC,dbo.B9_THO,dbo.B9_KHACHHANG;
GO
CREATE TABLE dbo.B9_THO
(
    MaTho varchar(10) NOT NULL,
    TenTho nvarchar(100) NOT NULL,
    Nhom int NOT NULL,
    NhomTruong varchar(10) NOT NULL,
    CONSTRAINT PK_B9_THO PRIMARY KEY(MaTho),
    CONSTRAINT CK_B9_THO_Chuoi CHECK(LEN(LTRIM(RTRIM(MaTho)))>0 AND LEN(LTRIM(RTRIM(TenTho)))>0 AND LEN(LTRIM(RTRIM(NhomTruong)))>0),
    -- Số nhóm phải là số nguyên dương
    CONSTRAINT CK_B9_THO_Nhom CHECK(Nhom>0),
    CONSTRAINT FK_B9_THO_NhomTruong FOREIGN KEY(NhomTruong) REFERENCES dbo.B9_THO(MaTho)
);
CREATE TABLE dbo.B9_CONGVIEC
(
    MaCV varchar(10) NOT NULL,
    NoiDungCV nvarchar(200) NOT NULL,
    CONSTRAINT PK_B9_CONGVIEC PRIMARY KEY(MaCV),
    CONSTRAINT CK_B9_CONGVIEC_Chuoi CHECK(LEN(LTRIM(RTRIM(MaCV)))>0 AND LEN(LTRIM(RTRIM(NoiDungCV)))>0)
);
CREATE TABLE dbo.B9_KHACHHANG
(
    MaKH varchar(10) NOT NULL,
    TenKH nvarchar(100) NOT NULL,
    DiaChi nvarchar(200) NOT NULL,
    DienThoai varchar(16) NOT NULL,
    CONSTRAINT PK_B9_KHACHHANG PRIMARY KEY(MaKH),
    CONSTRAINT CK_B9_KHACHHANG_Chuoi CHECK(LEN(LTRIM(RTRIM(MaKH)))>0 AND LEN(LTRIM(RTRIM(TenKH)))>0 AND LEN(LTRIM(RTRIM(DiaChi)))>0),
    CONSTRAINT CK_B9_KH_DienThoai CHECK
    (
        (DienThoai NOT LIKE '%[^0-9]%' AND LEN(DienThoai) BETWEEN 9 AND 15
         AND DATALENGTH(DienThoai)=LEN(DienThoai))
        OR
        (LEFT(DienThoai,1)='+' AND SUBSTRING(DienThoai,2,15) NOT LIKE '%[^0-9]%'
         AND LEN(DienThoai) BETWEEN 10 AND 16
         AND DATALENGTH(DienThoai)=LEN(DienThoai))
    )
);
CREATE TABLE dbo.B9_HOPDONG
(
    SoHD varchar(12) NOT NULL,
    NgayHD date NOT NULL,
    MaKH varchar(10) NOT NULL,
    SoXe varchar(20) NOT NULL,
    TriGiaHD decimal(18,2) NOT NULL,
    NgayGiaoDK date NOT NULL,
    NgayNgThu date NULL,
    CONSTRAINT PK_B9_HOPDONG PRIMARY KEY(SoHD),
    CONSTRAINT FK_B9_HOPDONG_KHACHHANG FOREIGN KEY(MaKH) REFERENCES dbo.B9_KHACHHANG(MaKH),
    CONSTRAINT UQ_B9_HOPDONG_SoHD_MaKH UNIQUE(SoHD,MaKH),
    -- 1 xe trong 1 ngày chỉ được kí 1 hợp đồng
    CONSTRAINT UQ_B9_HOPDONG_Ngay_SoXe UNIQUE(NgayHD,SoXe),
    CONSTRAINT CK_B9_HOPDONG_Chuoi CHECK(LEN(LTRIM(RTRIM(SoHD)))>0 AND LEN(LTRIM(RTRIM(MaKH)))>0 AND LEN(LTRIM(RTRIM(SoXe)))>0),
    CONSTRAINT CK_B9_HOPDONG_TriGiaHD CHECK(TriGiaHD>=0),
    CONSTRAINT CK_B9_HOPDONG_Ngay CHECK(NgayGiaoDK>=NgayHD AND (NgayNgThu IS NULL OR NgayNgThu>=NgayHD))
);
CREATE TABLE dbo.B9_CHITIET_HD
(
    SoHD varchar(12) NOT NULL,
    MaCV varchar(10) NOT NULL,
    TriGiaCV decimal(18,2) NOT NULL,
    MaTho varchar(10) NOT NULL,
    KhoanTho decimal(18,2) NOT NULL,
    CONSTRAINT PK_B9_CHITIET_HD PRIMARY KEY(SoHD,MaCV),
    CONSTRAINT FK_B9_CHITIET_HOPDONG FOREIGN KEY(SoHD) REFERENCES dbo.B9_HOPDONG(SoHD),
    CONSTRAINT FK_B9_CHITIET_CONGVIEC FOREIGN KEY(MaCV) REFERENCES dbo.B9_CONGVIEC(MaCV),
    CONSTRAINT FK_B9_CHITIET_THO FOREIGN KEY(MaTho) REFERENCES dbo.B9_THO(MaTho),
    CONSTRAINT CK_B9_CHITIET_Chuoi CHECK(LEN(LTRIM(RTRIM(SoHD)))>0 AND LEN(LTRIM(RTRIM(MaCV)))>0 AND LEN(LTRIM(RTRIM(MaTho)))>0),
    CONSTRAINT CK_B9_CHITIET_TriGiaCV CHECK(TriGiaCV>=0),
    CONSTRAINT CK_B9_CHITIET_KhoanTho CHECK(KhoanTho>=0)
);
CREATE TABLE dbo.B9_PHIEUTHU
(
    SoPT varchar(12) NOT NULL,
    NgayLapPT date NOT NULL,
    SoHD varchar(12) NOT NULL,
    MaKH varchar(10) NOT NULL,
    HoTen nvarchar(100) NOT NULL,
    SoTienThu decimal(18,2) NOT NULL,
    CONSTRAINT PK_B9_PHIEUTHU PRIMARY KEY(SoPT),
    CONSTRAINT CK_B9_PHIEUTHU_Chuoi CHECK(LEN(LTRIM(RTRIM(SoPT)))>0 AND LEN(LTRIM(RTRIM(SoHD)))>0 AND LEN(LTRIM(RTRIM(MaKH)))>0 AND LEN(LTRIM(RTRIM(HoTen)))>0),
    CONSTRAINT CK_B9_PHIEUTHU_SoTienThu CHECK(SoTienThu>0),
    CONSTRAINT FK_B9_PHIEUTHU_HOPDONG FOREIGN KEY(SoHD,MaKH) REFERENCES 
    dbo.B9_HOPDONG(SoHD,MaKH)
);
GO

-- Trigger 1: Kiểm tra Nhóm trưởng phải là một người thợ thuộc cùng nhóm
-- Và một nhóm chỉ có một nhóm trưởng chung.
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraNhomTruong ON dbo.B9_THO AFTER INSERT,
UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS
    (
        SELECT 1 FROM inserted i
        LEFT JOIN dbo.B9_THO nt ON nt.MaTho=i.NhomTruong AND nt.Nhom=i.Nhom
        WHERE nt.MaTho IS NULL
    )
    BEGIN
        RAISERROR(N'Nhóm trưởng phải là một người thợ thuộc cùng nhóm.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    -- INSERT phải giữ một nhóm trưởng chung cho toàn bộ thợ của mỗi nhóm.
    IF NOT EXISTS(SELECT 1 FROM deleted) AND EXISTS
    (
        SELECT 1
        FROM dbo.B9_THO t
        -- Lấy những nhóm vừa được thêm
        JOIN (SELECT DISTINCT Nhom FROM inserted) i ON i.Nhom=t.Nhom
        GROUP BY t.Nhom
        -- Đếm mã nhóm trưởng khác nhau trong cùng 1 nhóm
        HAVING COUNT(DISTINCT t.NhomTruong)>1
    )
    BEGIN
        RAISERROR(N'Mỗi nhóm chỉ được có một nhóm trưởng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger 2: Kiểm tra TH update làm sai quan hệ nhóm trưởng
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraDoiNhomTruong ON dbo.B9_THO AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Nhom) AND EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.B9_THO thanhvien ON thanhvien.NhomTruong=i.MaTho
        WHERE thanhvien.Nhom<>i.Nhom
    )
    BEGIN
        RAISERROR(N'Không thể đổi nhóm của nhóm trưởng khi thợ trong nhóm chưa được 
        cập nhật.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    -- UPDATE Nhom hoặc NhomTruong không được tạo hai trưởng nhóm trong cùng nhóm.
    IF EXISTS
    (
        SELECT 1
        FROM dbo.B9_THO t
        JOIN (SELECT DISTINCT Nhom FROM inserted) i ON i.Nhom=t.Nhom
        GROUP BY t.Nhom
        HAVING COUNT(DISTINCT t.NhomTruong)>1
    )
    BEGIN
        RAISERROR(N'Mỗi nhóm chỉ được có một nhóm trưởng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger 3: Kiểm tra TriGiaHD = tổng TriGiaCV và Tổng tiền đã thu <= TriGiaHD
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraTriGiaHopDong ON dbo.B9_HOPDONG
AFTER INSERT,UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        OUTER APPLY
        (
            SELECT SUM(ct.TriGiaCV) AS TongTriGia
            FROM dbo.B9_CHITIET_HD ct WHERE ct.SoHD=i.SoHD
        ) ct
        -- Nếu trị giá hợp đồng khác tổng trị giá công việc thì lỗi.
        WHERE i.TriGiaHD<>COALESCE(ct.TongTriGia,0)
    )
    BEGIN
        RAISERROR(N'Trị giá hợp đồng phải bằng tổng trị giá các công việc.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        OUTER APPLY
        (
            SELECT SUM(p.SoTienThu) AS TongDaThu
            FROM dbo.B9_PHIEUTHU p WHERE p.SoHD=i.SoHD
        ) p
        -- Coalesce trả về giá trị đầu tiên khác NULL trong danh sách
        WHERE COALESCE(p.TongDaThu,0)>i.TriGiaHD
    )
    BEGIN
        RAISERROR(N'Tổng tiền đã thu không được vượt trị giá hợp đồng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger 4: Khi thêm, sửa, xóa CHITIET_HD thì tính lại TriGiaHD
CREATE OR ALTER TRIGGER dbo.tg_B9_CapNhatTriGiaHopDong ON dbo.B9_CHITIET_HD
AFTER INSERT,UPDATE,DELETE AS
BEGIN
    SET NOCOUNT ON;
    -- CTE1: lấy các hợp đồng bị ảnh hưởng
    ;WITH HopDongBiAnhHuong AS
    (
        SELECT SoHD FROM inserted
        UNION
        SELECT SoHD FROM deleted
    ),
    -- CTE2: Tính lại tổng TriGiaCV của từng hợp đồng
    Tong AS
    (
        SELECT a.SoHD,COALESCE(SUM(ct.TriGiaCV),0) AS TriGia
        FROM HopDongBiAnhHuong a
        -- Dùng LJ để kể cả trường hợp vừa xóa hết chi tiết, hợp đồng vẫn xuất hiện và tổng trở về 0
        LEFT JOIN dbo.B9_CHITIET_HD ct ON ct.SoHD=a.SoHD
        -- Dùng GB để tính riêng cho từng hợp đồng
        GROUP BY a.SoHD
    )
    UPDATE h SET TriGiaHD=t.TriGia
    FROM dbo.B9_HOPDONG h JOIN Tong t ON t.SoHD=h.SoHD;
END;
GO

-- Trigger 5: Kiểm tra NgaylapPT không được trước NgayHD
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraNgayPhieuThu ON dbo.B9_PHIEUTHU
AFTER INSERT,UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.B9_HOPDONG h ON h.SoHD=i.SoHD
        WHERE i.NgayLapPT<h.NgayHD
    )
    BEGIN
        RAISERROR(N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger 6: Tổng tất cả phiếu thu của một hợp đồng không vượt trị giá hợp đồng
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraTongPhieuThu ON dbo.B9_PHIEUTHU
AFTER INSERT,UPDATE AS
BEGIN
    SET NOCOUNT ON;
    -- Tạo bảng biến tạm để chứa danh sách hợp đồng cần kiểm tra
    DECLARE @HopDongBiAnhHuong TABLE(SoHD varchar(12) PRIMARY KEY);
    INSERT @HopDongBiAnhHuong(SoHD)
    SELECT h.SoHD
    -- UPDLOCK: khóa các dòng hợp đồng theo kiểu chuẩn bị cập nhật
    -- HOLDLOCK: giữ khóa đến hết transaction
    -- Tránh hai giao dịch cùng lúc cùng thêm phiếu thu và đều tưởng rằng tổng tiền vẫn chưa vượt mức.
    FROM dbo.B9_HOPDONG h WITH (UPDLOCK,HOLDLOCK)
    WHERE h.SoHD IN (SELECT SoHD FROM inserted);

    IF EXISTS
    (
        SELECT 1
        FROM @HopDongBiAnhHuong a
        JOIN dbo.B9_HOPDONG h ON h.SoHD=a.SoHD
        OUTER APPLY
        (
            SELECT SUM(p.SoTienThu) AS TongDaThu
            FROM dbo.B9_PHIEUTHU p WHERE p.SoHD=a.SoHD
        ) p
        -- Coalesce trả về giá trị đầu tiên khác NULL trong danh sách
        WHERE COALESCE(p.TongDaThu,0)>h.TriGiaHD
    )
    BEGIN
        RAISERROR(N'Tổng tiền đã thu không được vượt trị giá hợp đồng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- Trigger 7: Kiểm tra ngày ký hợp đồng phải trước ngày lập phiếu
CREATE OR ALTER TRIGGER dbo.tg_B9_KiemTraNgayHopDongPhieuThu ON dbo.B9_HOPDONG
AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(NgayHD) AND EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.B9_PHIEUTHU p ON p.SoHD=i.SoHD
        WHERE p.NgayLapPT<i.NgayHD
    )
    BEGIN
        RAISERROR(N'Ngày lập phiếu thu không được trước ngày ký hợp đồng.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO


INSERT dbo.B9_THO VALUES
('T01',N'Nguyễn Văn Thợ',1,'T01'),('T02',N'Trần Văn Máy',1,'T01'),('T03',N'Đỗ Văn Điện',1,'T01'),
('T04',N'Lê Minh Sửa',2,'T04'),('T05',N'Võ Minh Sơn',2,'T04'),('T06',N'Phạm Quốc Bảo',3,'T06'),('T07',N'Bùi Anh Kiệt',3,'T06');
INSERT dbo.B9_CONGVIEC VALUES
('CV01',N'Thay nhớt'),('CV02',N'Sửa phanh'),('CV03',N'Bảo dưỡng động cơ'),('CV04',N'Sơn xe'),('CV05',N'Kiểm tra điện');
INSERT dbo.B9_KHACHHANG VALUES
('KH01',N'Công ty An Phát',N'Quận 1','0902000001'),('KH02',N'Nguyễn Hoàng',N'Quận 3','0902000002'),('KH03',N'Trần Mai',N'Quận 5','0902000003');
-- Hợp đồng bắt đầu ở 0; trigger chi tiết sẽ cộng trị giá công việc sau khi nhập.
INSERT dbo.B9_HOPDONG VALUES
('HD01','2002-12-01','KH01','51A-111.11',0,'2002-12-20','2002-12-18'),
('HD02','2002-12-10','KH02','51B-222.22',0,'2002-12-31',NULL),
('HD03','2003-01-05','KH03','51C-333.33',0,'2003-01-20','2003-01-19'),
('HD04','2002-11-15','KH01','51D-444.44',0,'2002-12-25','2002-12-24');
INSERT dbo.B9_CHITIET_HD VALUES
('HD01','CV01',500000,'T02',200000),('HD01','CV02',1000000,'T03',400000),('HD01','CV03',1500000,'T02',500000),
('HD02','CV01',800000,'T02',300000),('HD02','CV04',2500000,'T05',700000),
('HD03','CV05',2000000,'T07',600000),('HD04','CV02',1500000,'T03',500000),('HD04','CV03',3000000,'T02',900000);
INSERT dbo.B9_PHIEUTHU VALUES
('PT01','2002-12-05','HD01','KH01',N'Nguyễn A',1000000),('PT02','2002-12-18','HD01','KH01',N'Nguyễn A',2000000),
('PT03','2002-12-24','HD04','KH01',N'Lê B',2000000),('PT04','2003-01-19','HD03','KH03',N'Trần C',2000000);
GO
