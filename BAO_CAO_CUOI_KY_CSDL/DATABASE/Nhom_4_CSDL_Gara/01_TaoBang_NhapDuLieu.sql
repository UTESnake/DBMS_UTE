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
    MaTho varchar(10) PRIMARY KEY,
    TenTho nvarchar(100) NOT NULL,
    Nhom int NOT NULL CHECK(Nhom>0),
    NhomTruong varchar(10) NOT NULL,
    CONSTRAINT FK_B9_THO_NhomTruong FOREIGN KEY(NhomTruong) REFERENCES dbo.B9_THO(MaTho)
);
CREATE TABLE dbo.B9_CONGVIEC
(
    MaCV varchar(10) PRIMARY KEY,
    NoiDungCV nvarchar(200) NOT NULL UNIQUE
);
CREATE TABLE dbo.B9_KHACHHANG
(
    MaKH varchar(10) PRIMARY KEY,
    TenKH nvarchar(100) NOT NULL,
    DiaChi nvarchar(200) NOT NULL,
    DienThoai varchar(15) NOT NULL UNIQUE,
    CONSTRAINT CK_B9_KH_DienThoai CHECK(DienThoai NOT LIKE '%[^0-9+]%')
);
CREATE TABLE dbo.B9_HOPDONG
(
    SoHD varchar(12) PRIMARY KEY,
    NgayHD date NOT NULL,
    MaKH varchar(10) NOT NULL REFERENCES dbo.B9_KHACHHANG(MaKH),
    SoXe varchar(20) NOT NULL,
    TriGiaHD decimal(18,2) NOT NULL CHECK(TriGiaHD>=0),
    NgayGiaoDK date NOT NULL,
    NgayNgThu date NULL,
    CONSTRAINT UQ_B9_HOPDONG_SoHD_MaKH UNIQUE(SoHD,MaKH),
    CONSTRAINT UQ_B9_HOPDONG_Ngay_SoXe UNIQUE(NgayHD,SoXe),
    CONSTRAINT CK_B9_HOPDONG_Ngay CHECK(NgayGiaoDK>=NgayHD AND (NgayNgThu IS NULL OR NgayNgThu>=NgayHD))
);
CREATE TABLE dbo.B9_CHITIET_HD
(
    SoHD varchar(12) NOT NULL REFERENCES dbo.B9_HOPDONG(SoHD),
    MaCV varchar(10) NOT NULL REFERENCES dbo.B9_CONGVIEC(MaCV),
    TriGiaCV decimal(18,2) NOT NULL CHECK(TriGiaCV>=0),
    MaTho varchar(10) NOT NULL REFERENCES dbo.B9_THO(MaTho),
    KhoanTho decimal(18,2) NOT NULL,
    CONSTRAINT PK_B9_CHITIET_HD PRIMARY KEY(SoHD,MaCV),
    CONSTRAINT CK_B9_CHITIET_KhoanTho CHECK(KhoanTho>=0 AND KhoanTho<=TriGiaCV)
);
CREATE TABLE dbo.B9_PHIEUTHU
(
    SoPT varchar(12) PRIMARY KEY,
    NgayLapPT date NOT NULL,
    SoHD varchar(12) NOT NULL,
    MaKH varchar(10) NOT NULL,
    HoTen nvarchar(100) NOT NULL,
    SoTienThu decimal(18,2) NOT NULL CHECK(SoTienThu>0),
    CONSTRAINT FK_B9_PHIEUTHU_HOPDONG FOREIGN KEY(SoHD,MaKH) REFERENCES 
    dbo.B9_HOPDONG(SoHD,MaKH)
);
GO
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
    END
END;
GO
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
INSERT dbo.B9_HOPDONG VALUES
('HD01','2002-12-01','KH01','51A-111.11',3000000,'2002-12-20','2002-12-18'),
('HD02','2002-12-10','KH02','51B-222.22',5000000,'2002-12-31',NULL),
('HD03','2003-01-05','KH03','51C-333.33',2000000,'2003-01-20','2003-01-19'),
('HD04','2002-11-15','KH01','51D-444.44',4500000,'2002-12-25','2002-12-24');
INSERT dbo.B9_CHITIET_HD VALUES
('HD01','CV01',500000,'T02',200000),('HD01','CV02',1000000,'T03',400000),('HD01','CV03',1500000,'T02',500000),
('HD02','CV01',800000,'T02',300000),('HD02','CV04',2500000,'T05',700000),
('HD03','CV05',2000000,'T07',600000),('HD04','CV02',1500000,'T03',500000),('HD04','CV03',3000000,'T02',900000);
INSERT dbo.B9_PHIEUTHU VALUES
('PT01','2002-12-05','HD01','KH01',N'Nguyễn A',1000000),('PT02','2002-12-18','HD01','KH01',N'Nguyễn A',2000000),
('PT03','2002-12-24','HD04','KH01',N'Lê B',2000000),('PT04','2003-01-19','HD03','KH03',N'Trần C',2000000);
GO

