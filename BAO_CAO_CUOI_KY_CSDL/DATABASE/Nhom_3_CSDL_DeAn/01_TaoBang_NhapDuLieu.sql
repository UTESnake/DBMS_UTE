USE QL_DeAn;
GO
DROP TABLE IF EXISTS dbo.B7_ThanNhan,dbo.B7_PhanCong,dbo.B7_DeAn,dbo.B7_NhanVien,dbo.B7_PhongBan;
GO
CREATE TABLE dbo.B7_PhongBan(MaPB varchar(10) PRIMARY KEY,TenPB nvarchar(100) NOT NULL);
CREATE TABLE dbo.B7_NhanVien(MaNV varchar(10) PRIMARY KEY,HoTen nvarchar(100) NOT NULL,NgaySinh date NOT NULL,GioiTinh nvarchar(3) NOT NULL CHECK(GioiTinh IN (N'Nam',N'Nữ')),Luong decimal(18,2) NOT NULL CHECK(Luong>=0),MaPB varchar(10) NULL REFERENCES dbo.B7_PhongBan(MaPB));
CREATE TABLE dbo.B7_DeAn(MaDA varchar(10) PRIMARY KEY,TenDA nvarchar(150) NOT NULL,MaPB varchar(10) NOT NULL REFERENCES dbo.B7_PhongBan(MaPB));
CREATE TABLE dbo.B7_PhanCong(MaNV varchar(10) REFERENCES dbo.B7_NhanVien(MaNV),MaDA varchar(10) REFERENCES dbo.B7_DeAn(MaDA),SoGio decimal(8,2) NOT NULL CHECK(SoGio>=0),PRIMARY KEY(MaNV,MaDA));
CREATE TABLE dbo.B7_ThanNhan(MaNV varchar(10) REFERENCES dbo.B7_NhanVien(MaNV),HoTen nvarchar(100),QuanHe nvarchar(30),PRIMARY KEY(MaNV,HoTen));
INSERT dbo.B7_PhongBan VALUES('PB01',N'Nghiên cứu'),('PB02',N'Phát triển'),('PB03',N'Kiểm thử'),('PB05',N'Phòng số 5'),('PB00',N'Chưa có nhân viên');
INSERT dbo.B7_NhanVien VALUES
('NV01',N'Nguyễn An','1990-01-12',N'Nam',32000,'PB01'),('NV02',N'Trần Bình','1988-05-20',N'Nam',40000,'PB01'),('NV03',N'Lê Chi','1995-09-08',N'Nữ',28000,'PB02'),
('NV04',N'Phạm Dũng','1992-12-01',N'Nam',50000,'PB02'),('NV05',N'Võ Em','2000-03-15',N'Nữ',24000,'PB03'),('NV06',N'Đỗ Phúc','1985-07-23',N'Nam',36000,NULL),
('NV07',N'Bùi Giang','1997-04-11',N'Nữ',35000,'PB01'),('NV08',N'Hồ Hạnh','1999-06-21',N'Nữ',31000,'PB02'),('NV09',N'Ngô Khang','1994-08-09',N'Nam',27000,'PB05'),
('NV10',N'Đặng Lan','1996-10-18',N'Nữ',26000,'PB05'),('NV11',N'Tạ Minh','1991-11-30',N'Nam',25500,'PB05');
INSERT dbo.B7_DeAn VALUES('DA01',N'Hệ thống thư viện','PB01'),('DA02',N'Quản lý nhân sự','PB01'),('DA03',N'Cổng dữ liệu','PB02'),('DA04',N'Chưa phân công','PB03'),('DA05',N'Chuyển đổi số','PB05');
INSERT dbo.B7_PhanCong VALUES
('NV01','DA01',30),('NV01','DA02',31),('NV02','DA01',60),('NV02','DA03',61),('NV03','DA01',99),('NV03','DA03',100),('NV04','DA02',149),('NV04','DA03',150),('NV05','DA03',0),
('NV07','DA01',45),('NV08','DA03',75),('NV09','DA01',20),('NV09','DA05',55),('NV10','DA05',40),('NV11','DA05',65);
INSERT dbo.B7_ThanNhan VALUES('NV01',N'Nguyễn Minh',N'Con'),('NV01',N'Hoàng Lan',N'Vợ'),('NV03',N'Lê Hà',N'Con'),('NV04',N'Phạm Mai',N'Vợ');
GO
