USE master;
GO
IF DB_ID(N'QL_DeAn') IS NULL
    EXEC(N'CREATE DATABASE QL_DeAn');
GO
USE QL_DeAn;
GO

-- Sáu bảng Đề Án dùng tên cột thống nhất và bảng lương tổng hợp BANGLUONG.
-- Bảng lương được tạo và nạp ngay trong file này, không phụ thuộc thứ tự tạo function Bài 7–8.
DROP VIEW IF EXISTS dbo.v_LuongNhanVienTheoDeAn;
DROP VIEW IF EXISTS dbo.v_BangLuongChiTiet;
IF OBJECT_ID(N'dbo.B7_ThanNhan',N'V') IS NOT NULL DROP VIEW dbo.B7_ThanNhan;
IF OBJECT_ID(N'dbo.B7_PhanCong',N'V') IS NOT NULL DROP VIEW dbo.B7_PhanCong;
IF OBJECT_ID(N'dbo.B7_DeAn',N'V') IS NOT NULL DROP VIEW dbo.B7_DeAn;
IF OBJECT_ID(N'dbo.B7_NhanVien',N'V') IS NOT NULL DROP VIEW dbo.B7_NhanVien;
IF OBJECT_ID(N'dbo.B7_PhongBan',N'V') IS NOT NULL DROP VIEW dbo.B7_PhongBan;
GO
IF OBJECT_ID(N'dbo.FK_PHONGBAN_TRPHG',N'F') IS NOT NULL
    ALTER TABLE dbo.PHONGBAN DROP CONSTRAINT FK_PHONGBAN_TRPHG;
DROP TABLE IF EXISTS dbo.BANGLUONG,dbo.LUONG,dbo.THANNHAN,dbo.PHANCONG,dbo.DIADIEM_PHG,dbo.DEAN,dbo.NHANVIEN,dbo.PHONGBAN;
GO

CREATE TABLE dbo.PHONGBAN
(
    MaPhg varchar(2) NOT NULL CONSTRAINT PK_PHONGBAN PRIMARY KEY,
    TenPhg nvarchar(20) NULL,
    TrPhg varchar(9) NULL,
    Ng_NhanChuc smalldatetime NULL
);

CREATE TABLE dbo.NHANVIEN
(
    MaNV varchar(9) NOT NULL CONSTRAINT PK_NHANVIEN PRIMARY KEY,
    HoNV nvarchar(15) NULL,
    TenLot nvarchar(30) NULL,
    TenNV nvarchar(30) NULL,
    NgSinh smalldatetime NULL,
    DChi nvarchar(150) NULL,
    Phai nvarchar(3) NULL,
    Luong numeric(18,0) NULL CONSTRAINT CK_NHANVIEN_LUONG CHECK(Luong>=0),
    Ma_NQL varchar(9) NULL,
    Phg varchar(2) NULL,
    CONSTRAINT FK_NHANVIEN_NQL FOREIGN KEY(Ma_NQL) REFERENCES dbo.NHANVIEN(MaNV),
    CONSTRAINT FK_NHANVIEN_PHG FOREIGN KEY(Phg) REFERENCES dbo.PHONGBAN(MaPhg)
);

ALTER TABLE dbo.PHONGBAN ADD CONSTRAINT FK_PHONGBAN_TRPHG
    FOREIGN KEY(TrPhg) REFERENCES dbo.NHANVIEN(MaNV);

CREATE TABLE dbo.DEAN
(
    MaDA varchar(2) NOT NULL CONSTRAINT PK_DEAN PRIMARY KEY,
    TenDA nvarchar(50) NULL,
    DDIEM_DA varchar(20) NULL,
    Phong varchar(2) NULL,
    Truong_DA varchar(9) NULL,
    CONSTRAINT FK_DEAN_PHONGBAN FOREIGN KEY(Phong) REFERENCES dbo.PHONGBAN(MaPhg),
    CONSTRAINT FK_DEAN_TRUONG FOREIGN KEY(Truong_DA) REFERENCES dbo.NHANVIEN(MaNV)
);

CREATE TABLE dbo.DIADIEM_PHG
(
    MaPhg varchar(2) NOT NULL,
    DiaDiem varchar(20) NOT NULL,
    CONSTRAINT PK_DIADIEM_PHG PRIMARY KEY(MaPhg,DiaDiem),
    CONSTRAINT FK_DIADIEM_PHG_PHONGBAN FOREIGN KEY(MaPhg) REFERENCES dbo.PHONGBAN(MaPhg)
);

CREATE TABLE dbo.THANNHAN
(
    MaNV varchar(9) NOT NULL,
    TenTN nvarchar(20) NOT NULL,
    NgSinh smalldatetime NULL,
    Phai nvarchar(3) NULL,
    QuanHe nvarchar(15) NULL,
    CONSTRAINT PK_THANNHAN PRIMARY KEY(MaNV,TenTN),
    CONSTRAINT FK_THANNHAN_NHANVIEN FOREIGN KEY(MaNV) REFERENCES dbo.NHANVIEN(MaNV)
);

CREATE TABLE dbo.PHANCONG
(
    MaNV varchar(9) NOT NULL,
    SoDA varchar(2) NOT NULL,
    ThoiGian numeric(18,0) NULL CONSTRAINT CK_PHANCONG_THOIGIAN CHECK(ThoiGian>=0),
    CONSTRAINT PK_PHANCONG PRIMARY KEY(MaNV,SoDA),
    CONSTRAINT FK_PHANCONG_NHANVIEN FOREIGN KEY(MaNV) REFERENCES dbo.NHANVIEN(MaNV),
    CONSTRAINT FK_PHANCONG_DEAN FOREIGN KEY(SoDA) REFERENCES dbo.DEAN(MaDA)
);

CREATE TABLE dbo.BANGLUONG
(
    MaNV varchar(9) NOT NULL CONSTRAINT PK_BANGLUONG PRIMARY KEY,
    HoTen nvarchar(100) NOT NULL,
    TenPhong nvarchar(20) NULL,
    LuongCoBan numeric(18,0) NULL CONSTRAINT CK_BANGLUONG_LuongCoBan CHECK(LuongCoBan>=0),
    Time_Total numeric(38,0) NOT NULL,
    TienThuong decimal(18,2) NOT NULL,
    LuongTBPhong decimal(18,2) NOT NULL,
    NguoiThan nvarchar(max) NOT NULL,
    TongThuNhap decimal(38,2) NOT NULL,
    NgayTinhLuong date NOT NULL,
    NgayNhanLuong date NOT NULL,
    CONSTRAINT FK_BANGLUONG_NHANVIEN FOREIGN KEY(MaNV)
        REFERENCES dbo.NHANVIEN(MaNV) ON DELETE CASCADE
);
GO

INSERT dbo.PHONGBAN(MaPhg,TenPhg) VALUES
('01',N'Nghiên cứu'),('02',N'Phát triển'),('03',N'Kiểm thử'),
('04',N'Phòng biên 30 nghìn'),('05',N'Phòng số 5'),
('06',N'Phòng nữ'),('00',N'Chưa có nhân viên');

INSERT dbo.NHANVIEN(MaNV,HoNV,TenLot,TenNV,NgSinh,DChi,Phai,Luong,Ma_NQL,Phg) VALUES
('NV01',N'Nguyễn',NULL,N'An','1990-01-12',N'Hà Nội',N'nam',32000,NULL,'01'),
('NV02',N'Trần',NULL,N'Bình','1988-05-20',N'Hà Nội',N'NAM',40000,NULL,'01'),
('NV03',N'Lê',NULL,N'Chi','1995-09-08',N'Đà Nẵng',N'Nữ',28000,NULL,'02'),
('NV04',N'Phạm',NULL,N'Dũng','1992-12-01',N'Đà Nẵng',N'Nam',50000,NULL,'02'),
('NV05',N'Võ',NULL,N'Em','2000-03-15',N'TP. Hồ Chí Minh',N'Nữ',24000,NULL,'03'),
('NV06',N'Đỗ',NULL,N'Phúc','1985-07-23',N'Hà Nội',N'Nam',36000,NULL,NULL),
('NV07',N'Bùi',NULL,N'Giang','1997-04-11',N'Hà Nội',N'Nữ',35000,NULL,'01'),
('NV08',N'Hồ',NULL,N'Hạnh','1999-06-21',N'Đà Nẵng',N'Nữ',31000,NULL,'02'),
('NV09',N'Ngô',NULL,N'Khang','1994-08-09',N'TP. Hồ Chí Minh',N'Nam',27000,NULL,'03'),
('NV10',N'Đặng',NULL,N'Lan','1996-10-18',N'TP. Hồ Chí Minh',N'Nữ',26000,NULL,'03'),
('NV11',N'Tạ',NULL,N'Minh','1991-11-30',N'TP. Hồ Chí Minh',N'Nam',25500,NULL,'03'),
('NV12',N'Phan',NULL,N'Ngọc','1998-02-14',N'Huế',N'Nữ',25000,NULL,'04'),
('NV13',N'Lý',NULL,N'Quang','1993-07-01',N'Huế',N'Nam',35000,NULL,'04'),
('NV14',N'Trịnh',NULL,N'Sương','1990-06-10',N'Cần Thơ',NULL,32000,NULL,'06'),
('NV15',N'Vương',NULL,N'Thảo','1995-05-21',N'Cần Thơ',N'Nữ',33000,NULL,'06'),
('NV16',N'Cao',NULL,N'Uyên','1996-09-03',N'Cần Thơ',N'Nữ',34000,NULL,'06'),
('NV17',N'Đinh',NULL,N'Vân','2001-04-17',N'TP. Hồ Chí Minh',N'Nữ',NULL,NULL,'03'),
('NV18',N'Mai',NULL,N'Xuân','2002-08-29',N'TP. Hồ Chí Minh',N'Nam',0,NULL,'03'),
('NV19',N'Ninh',N'Thị',N'Yến','1999-12-04',N'TP. Hồ Chí Minh',N'Nữ',25000,NULL,'03');

UPDATE dbo.NHANVIEN SET Ma_NQL='NV01' WHERE MaNV IN ('NV02','NV07');
UPDATE dbo.NHANVIEN SET Ma_NQL='NV04' WHERE MaNV IN ('NV03','NV08');
UPDATE dbo.NHANVIEN SET Ma_NQL='NV09' WHERE MaNV IN ('NV10','NV11');
UPDATE dbo.NHANVIEN SET Ma_NQL='NV13' WHERE MaNV='NV12';
UPDATE dbo.NHANVIEN SET Ma_NQL='NV14' WHERE MaNV IN ('NV15','NV16');
UPDATE dbo.NHANVIEN SET Ma_NQL='NV05' WHERE MaNV IN ('NV17','NV18','NV19');
UPDATE dbo.PHONGBAN SET TrPhg='NV01',Ng_NhanChuc='2000-01-01' WHERE MaPhg='01';
UPDATE dbo.PHONGBAN SET TrPhg='NV04',Ng_NhanChuc='2001-01-01' WHERE MaPhg='02';
UPDATE dbo.PHONGBAN SET TrPhg='NV05',Ng_NhanChuc='2002-01-01' WHERE MaPhg='03';
UPDATE dbo.PHONGBAN SET TrPhg='NV13',Ng_NhanChuc='2003-01-01' WHERE MaPhg='04';
UPDATE dbo.PHONGBAN SET TrPhg='NV09',Ng_NhanChuc='2003-01-01' WHERE MaPhg='05';
UPDATE dbo.PHONGBAN SET TrPhg='NV14',Ng_NhanChuc='2004-01-01' WHERE MaPhg='06';

INSERT dbo.DEAN(MaDA,TenDA,DDIEM_DA,Phong) VALUES
('01',N'Hệ thống thư viện','Ha Noi','01'),
('02',N'Quản lý nhân sự','Ha Noi','01'),
('03',N'Cổng dữ liệu','Da Nang','02'),
('04',N'Chưa phân công','TP.HCM','03'),
('05',N'Chuyển đổi số','TP.HCM','05'),
('06',N'Thử phân công 0 giờ','Hue','04');
UPDATE dbo.DEAN SET Truong_DA='NV01' WHERE MaDA='01';

INSERT dbo.DIADIEM_PHG(MaPhg,DiaDiem) VALUES
('01','Ha Noi'),('02','Da Nang'),('03','TP.HCM'),
('04','Hue'),('05','TP.HCM'),('06','Can Tho');

INSERT dbo.PHANCONG(MaNV,SoDA,ThoiGian) VALUES
('NV01','01',30),('NV01','02',31),('NV02','01',60),('NV02','03',61),
('NV03','01',99),('NV03','03',100),('NV04','02',149),('NV04','03',150),
('NV05','03',0),('NV07','01',45),('NV08','03',75),('NV09','01',20),
('NV09','05',55),('NV10','05',40),('NV11','05',65),
('NV12','06',0),('NV17','06',10);

INSERT dbo.THANNHAN(MaNV,TenTN,NgSinh,Phai,QuanHe) VALUES
('NV01',N'Nguyễn Minh','2015-01-01',N'Nam',N'Con'),
('NV01',N'Hoàng Lan','1991-01-01',N'Nữ',N'Vợ'),
('NV03',N'Lê Hà','2020-01-01',N'Nữ',N'Con'),
('NV04',N'Phạm Mai','1994-01-01',N'Nữ',N'Vợ');

GO

-- =========================================================
-- VIEW v_BangLuongChiTiet
-- Mỗi nhân viên chỉ tạo một dòng kết quả.
-- View tổng hợp: lương cơ bản, tổng giờ, thưởng, lương TB phòng,
-- người thân và tổng thu nhập.
-- =========================================================
CREATE OR ALTER VIEW dbo.v_BangLuongChiTiet
AS
SELECT
       -- Mã nhân viên.
       n.MaNV,

       -- Ghép họ tên đầy đủ; CONCAT_WS tự bỏ qua thành phần NULL.
       CONCAT_WS(N' ',n.HoNV,n.TenLot,n.TenNV) AS HoTen,

       -- Tên phòng mà nhân viên đang làm việc.
       p.TenPhg AS TenPhong,

       -- Lương cơ bản lấy từ bảng NHANVIEN.
       n.Luong AS LuongCoBan,

       -- Tổng số giờ tham gia các đề án.
       -- COALESCE(...,0): nếu nhân viên chưa có phân công thì xem tổng giờ = 0.
       COALESCE(g.Time_Total,CONVERT(numeric(38,0),0)) AS Time_Total,

       -- Tiền thưởng được tính ở OUTER APPLY bên dưới theo Time_Total.
       thuong.TienThuong,

       -- Lương trung bình của phòng ban nhân viên đang thuộc.
       phong.LuongTBPhong,

       -- Danh sách người thân.
       -- Nếu không có người thân thì hiển thị '(Không có)'.
       COALESCE(t.NguoiThan,N'(Không có)') AS NguoiThan,

       -- Tổng thu nhập = lương cơ bản + tiền thưởng.
       -- COALESCE(n.Luong,0) tránh NULL khi nhân viên chưa có lương.
       CONVERT(decimal(38,2),COALESCE(n.Luong,0))
       + thuong.TienThuong AS TongThuNhap

FROM dbo.NHANVIEN AS n

-- LEFT JOIN để vẫn giữ nhân viên chưa thuộc phòng.
LEFT JOIN dbo.PHONGBAN AS p
    ON p.MaPhg=n.Phg

-- OUTER APPLY chạy truy vấn con riêng cho từng nhân viên n.
-- Tính tổng số giờ nhân viên tham gia tất cả đề án.
OUTER APPLY
(
    SELECT SUM(pc.ThoiGian) AS Time_Total
    FROM dbo.PHANCONG AS pc
    WHERE pc.MaNV=n.MaNV
) AS g

-- Tính tiền thưởng dựa trên tổng giờ theo đúng yêu cầu câu 7.4.
OUTER APPLY
(
    SELECT CONVERT(decimal(18,2),
        CASE
            -- Từ 30 đến 60 giờ: thưởng 500.
            WHEN g.Time_Total>=30 AND g.Time_Total<=60 THEN 500

            -- Trên 60 và dưới 100 giờ: thưởng 1000.
            WHEN g.Time_Total>60 AND g.Time_Total<100 THEN 1000

            -- Từ 100 đến dưới 150 giờ: thưởng 1200.
            WHEN g.Time_Total>=100 AND g.Time_Total<150 THEN 1200

            -- Từ 150 giờ trở lên: thưởng 1600.
            WHEN g.Time_Total>=150 THEN 1600

            -- Các trường hợp còn lại: không thưởng.
            ELSE 0
        END
    ) AS TienThuong
) AS thuong

-- Tính lương trung bình của phòng mà nhân viên hiện tại đang thuộc.
OUTER APPLY
(
    SELECT
        -- AVG tính trung bình lương.
        -- CAST định kiểu kết quả, COALESCE đưa NULL về 0.
        COALESCE(CAST(AVG(np.Luong) AS decimal(18,2)),0) AS LuongTBPhong
    FROM dbo.NHANVIEN AS np
    WHERE np.Phg=n.Phg
) AS phong

-- Gộp nhiều người thân thành một chuỗi duy nhất.
OUTER APPLY
(
    SELECT
        -- STRING_AGG nối tên người thân bằng dấu phẩy.
        -- WITHIN GROUP ORDER BY sắp tên trước khi ghép.
        STRING_AGG(CONVERT(nvarchar(max),tn.TenTN),N', ')
            WITHIN GROUP (ORDER BY tn.TenTN) AS NguoiThan
    FROM dbo.THANNHAN AS tn
    WHERE tn.MaNV=n.MaNV
) AS t;
GO



-- =========================================================
-- VIEW v_LuongNhanVienTheoDeAn
-- Phục vụ câu 7.2: tính phần lương nhân viên nhận theo từng đề án,
-- phân bổ theo tỷ trọng số giờ tham gia đề án / tổng số giờ của nhân viên.
-- =========================================================
CREATE OR ALTER VIEW dbo.v_LuongNhanVienTheoDeAn
AS
SELECT
       -- Mã nhân viên.
       n.MaNV,

       -- Họ tên đầy đủ của nhân viên.
       CONCAT_WS(N' ',n.HoNV,n.TenLot,n.TenNV) AS HoTen,

       -- Mã và tên đề án.
       d.MaDA,
       d.TenDA,

       -- Số giờ nhân viên tham gia riêng đề án hiện tại.
       pc.ThoiGian AS SoGioThamGia,

       -- Tổng số giờ nhân viên tham gia tất cả đề án.
       g.Time_Total AS TongGioNhanVien,

       -- Lương cơ bản của nhân viên.
       b.LuongCoBan,

       -- Tính phần lương của nhân viên được phân bổ cho đề án hiện tại.
       CASE
            -- Nếu thiếu lương, thiếu thời gian hoặc tổng giờ <= 0
            -- thì trả về 0 để tránh tính sai/chia cho 0.
            WHEN b.LuongCoBan IS NULL
              OR pc.ThoiGian IS NULL
              OR COALESCE(g.Time_Total,0)<=0
            THEN CONVERT(decimal(18,2),0)

            ELSE
                -- Công thức:
                -- LuongTheoDeAn = LuongCoBan * SoGioThamGia / TongGioNhanVien
                CAST(
                    CAST(b.LuongCoBan AS decimal(18,2))
                    * CAST(pc.ThoiGian AS decimal(18,2))
                    / NULLIF(g.Time_Total,0)
                    AS decimal(18,2)
                )
       END AS LuongTheoDeAn

FROM dbo.PHANCONG AS pc

-- Lấy thông tin nhân viên tương ứng với phân công.
JOIN dbo.NHANVIEN AS n
    ON n.MaNV=pc.MaNV

-- Lấy lương cơ bản của nhân viên.
JOIN dbo.BANGLUONG AS b
    ON b.MaNV=n.MaNV

-- Lấy thông tin đề án.
JOIN dbo.DEAN AS d
    ON d.MaDA=pc.SoDA

-- Tính tổng số giờ của từng nhân viên trên tất cả đề án.
OUTER APPLY
(
    SELECT SUM(p.ThoiGian) AS Time_Total
    FROM dbo.PHANCONG AS p
    WHERE p.MaNV=n.MaNV
) AS g;
GO



-- =========================================================
-- PROCEDURE: sp_TinhVaCapNhatBangLuong
-- Mục đích:
-- 1. Lấy dữ liệu lương mới nhất từ view v_BangLuongChiTiet.
-- 2. Cập nhật các nhân viên đã có trong BANGLUONG.
-- 3. Thêm các nhân viên chưa có trong BANGLUONG.
-- 4. Xóa các dòng lương không còn tương ứng với nhân viên hiện tại.
-- 5. Nếu có lỗi thì rollback toàn bộ transaction.
-- 6. Có thể chọn xuất kết quả sau khi cập nhật.
-- =========================================================

CREATE OR ALTER PROCEDURE dbo.sp_TinhVaCapNhatBangLuong

    -- Ngày dùng để tính lương.
    -- Nếu không truyền vào thì mặc định NULL.
    @NgayTinhLuong date = NULL,

    -- Có xuất kết quả sau khi cập nhật hay không.
    -- 1 = có xuất kết quả
    -- 0 = không xuất kết quả
    @XuatKetQua bit = 1

AS

BEGIN

    -- Không hiển thị các thông báo kiểu:
    -- "(1 row affected)"
    SET NOCOUNT ON;

    -- Nếu có lỗi runtime xảy ra trong transaction
    -- thì SQL Server tự động đánh dấu transaction để rollback.
    SET XACT_ABORT ON;

    -- Nếu @NgayTinhLuong là NULL
    -- thì lấy ngày hiện tại bằng GETDATE().
    -- CONVERT(date,GETDATE()) chỉ lấy phần ngày, bỏ phần giờ.
    SET @NgayTinhLuong =
        COALESCE(
            @NgayTinhLuong,
            CONVERT(date,GETDATE())
        );


    -- =====================================================
    -- BẮT ĐẦU KHỐI XỬ LÝ CÓ KIỂM SOÁT LỖI
    -- =====================================================
    BEGIN TRY

        -- Bắt đầu transaction.
        -- Tất cả UPDATE / INSERT / DELETE phía dưới
        -- được xem như một khối công việc thống nhất.
        BEGIN TRANSACTION;


        -- =================================================
        -- BƯỚC 1: TẠO BẢNG TẠM CHỨA BẢNG LƯƠNG MỚI
        -- =================================================

        SELECT
            MaNV,
            HoTen,
            TenPhong,
            LuongCoBan,
            Time_Total,
            TienThuong,
            LuongTBPhong,
            NguoiThan,
            TongThuNhap

        -- SELECT ... INTO tạo mới bảng tạm #BangLuongMoi
        -- từ kết quả của câu SELECT.
        INTO #BangLuongMoi

        -- Lấy dữ liệu đã được tính toán sẵn từ view.
        FROM dbo.v_BangLuongChiTiet;


        -- =================================================
        -- BƯỚC 2: CẬP NHẬT NHỮNG NHÂN VIÊN ĐÃ CÓ
        -- TRONG BANGLUONG
        -- =================================================

        UPDATE b

        SET
            -- Cập nhật họ tên.
            b.HoTen = m.HoTen,

            -- Cập nhật tên phòng.
            b.TenPhong = m.TenPhong,

            -- Cập nhật lương cơ bản.
            b.LuongCoBan = m.LuongCoBan,

            -- Cập nhật tổng số giờ tham gia dự án.
            b.Time_Total = m.Time_Total,

            -- Cập nhật tiền thưởng.
            b.TienThuong = m.TienThuong,

            -- Cập nhật lương trung bình phòng.
            b.LuongTBPhong = m.LuongTBPhong,

            -- Cập nhật danh sách người thân.
            b.NguoiThan = m.NguoiThan,

            -- Cập nhật tổng thu nhập.
            b.TongThuNhap = m.TongThuNhap,

            -- Gán ngày tính lương bằng tham số @NgayTinhLuong.
            b.NgayTinhLuong = @NgayTinhLuong,

            -- Ngày nhận lương = ngày tính lương + 1 tháng.
            b.NgayNhanLuong =
                DATEADD(month,1,@NgayTinhLuong)

        -- b là bảng BANGLUONG hiện tại.
        FROM dbo.BANGLUONG AS b

        -- m là dữ liệu mới từ bảng tạm.
        -- JOIN theo MaNV để cập nhật đúng nhân viên.
        JOIN #BangLuongMoi AS m
            ON m.MaNV = b.MaNV;


        -- =================================================
        -- BƯỚC 3: THÊM NHỮNG NHÂN VIÊN CHƯA CÓ
        -- TRONG BANGLUONG
        -- =================================================

        INSERT dbo.BANGLUONG
        (
            MaNV,
            HoTen,
            TenPhong,
            LuongCoBan,
            Time_Total,
            TienThuong,
            LuongTBPhong,
            NguoiThan,
            TongThuNhap,
            NgayTinhLuong,
            NgayNhanLuong
        )

        SELECT
            m.MaNV,
            m.HoTen,
            m.TenPhong,
            m.LuongCoBan,
            m.Time_Total,
            m.TienThuong,
            m.LuongTBPhong,
            m.NguoiThan,
            m.TongThuNhap,

            -- Ngày tính lương.
            @NgayTinhLuong,

            -- Ngày nhận lương sau ngày tính lương 1 tháng.
            DATEADD(month,1,@NgayTinhLuong)

        FROM #BangLuongMoi AS m

        -- Chỉ thêm nếu nhân viên chưa có trong BANGLUONG.
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.BANGLUONG AS b
            WHERE b.MaNV = m.MaNV
        );


        -- =================================================
        -- BƯỚC 4: XÓA NHỮNG DÒNG LƯƠNG KHÔNG CÒN
        -- TRONG DỮ LIỆU MỚI
        -- =================================================

        DELETE b

        FROM dbo.BANGLUONG AS b

        -- Nếu một MaNV đang có trong BANGLUONG
        -- nhưng không còn xuất hiện trong #BangLuongMoi
        -- thì xóa dòng đó.
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM #BangLuongMoi AS m
            WHERE m.MaNV = b.MaNV
        );


        -- Nếu tất cả các bước đều thành công
        -- thì xác nhận transaction.
        COMMIT TRANSACTION;

    END TRY


    -- =====================================================
    -- XỬ LÝ KHI CÓ LỖI
    -- =====================================================
    BEGIN CATCH

        -- XACT_STATE() cho biết trạng thái transaction.
        -- Nếu khác 0 nghĩa là transaction vẫn đang tồn tại
        -- thì rollback để quay lại trạng thái trước khi chạy procedure.
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        -- Ném lại chính lỗi vừa xảy ra
        -- để SQL Server hoặc Frontend có thể nhận được lỗi.
        THROW;

    END CATCH;


    -- =====================================================
    -- BƯỚC 5: XUẤT KẾT QUẢ NẾU @XuatKetQua = 1
    -- =====================================================

    IF @XuatKetQua = 1

        SELECT
            MaNV,
            HoTen,
            TenPhong,
            LuongCoBan,
            Time_Total,
            TienThuong,
            LuongTBPhong,
            NguoiThan,
            TongThuNhap,
            NgayTinhLuong,
            NgayNhanLuong

        FROM dbo.BANGLUONG

        -- Sắp xếp kết quả theo mã nhân viên.
        ORDER BY MaNV;

END;
GO

EXEC dbo.sp_TinhVaCapNhatBangLuong;
