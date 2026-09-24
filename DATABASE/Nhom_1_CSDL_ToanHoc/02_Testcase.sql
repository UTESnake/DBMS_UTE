USE [QL_DeAn]
GO

-- ============================================================
-- NHÓM 1 - DANH SÁCH TESTCASE ĐẦY ĐỦ
-- BÀI 1 (22 testcases), BÀI 2 (30 testcases), BÀI 4 (20 testcases)
-- Có cột KyVong (Kết quả kỳ vọng) để đối chiếu tự động
-- ============================================================

IF OBJECT_ID('dbo.Testcase_Nhom1', 'U') IS NOT NULL
    DROP TABLE dbo.Testcase_Nhom1;
GO

CREATE TABLE dbo.Testcase_Nhom1
(
    MaCase      VARCHAR(20) PRIMARY KEY,
    Bai         VARCHAR(10) NOT NULL,
    MoTa        NVARCHAR(250) NOT NULL,
    GiaTriA     NVARCHAR(100) NULL,
    GiaTriB     NVARCHAR(100) NULL,
    GiaTriC     NVARCHAR(100) NULL,
    NamSinh     NVARCHAR(100) NULL,
    LoaiTest    VARCHAR(30) NOT NULL,
    KyVong      NVARCHAR(500) NULL,
    ThuTu       INT NOT NULL
);
GO

-- ============================================================
-- BÀI 1: 22 TESTCASES (B1-ADD-01..05, B1-TC01..17)
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1 VALUES
('B1-ADD-01','B1',N'Bỏ trống hệ số b',N'2',N'',NULL,NULL,'WINFORMS',N'Vui lòng nhập hệ số b!',1),
('B1-ADD-02','B1',N'Nhập chữ vào hệ số b',N'2',N'abc',NULL,NULL,'WINFORMS',N'Hệ số b không hợp lệ!',2),
('B1-ADD-03','B1',N'Cả hai trường chỉ có khoảng trắng',N'   ',N'',NULL,NULL,'WINFORMS',N'Vui lòng nhập hệ số a!',3),
('B1-ADD-04','B1',N'Số vượt phạm vi double',N'1e309',N'1',NULL,NULL,'WINFORMS',N'Hệ số a không hợp lệ!',4),
('B1-ADD-05','B1',N'Dấu phân cách thập phân theo máy',N'0,5',N'-1,25',NULL,NULL,'WINFORMS',N'Phương trình có nghiệm: x = 2.5',5),
('B1-TC01','B1',N'a = 0, b = 0',N'0',N'0',NULL,NULL,'SQL',N'Phương trình có vô số nghiệm',6),
('B1-TC02','B1',N'a = 0, b khác 0',N'0',N'5',NULL,NULL,'SQL',N'Phương trình vô nghiệm',7),
('B1-TC03','B1',N'Nghiệm duy nhất',N'2',N'-4',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 2',8),
('B1-TC04','B1',N'Kiểm tra dấu: a âm, b dương',N'-2',N'8',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 4',9),
('B1-TC05','B1',N'Kiểm tra dấu: a âm, b âm',N'-2',N'-8',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = -4',10),
('B1-TC06','B1',N'b bằng 0',N'5',N'0',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 0',11),
('B1-TC07','B1',N'Số thập phân',N'0.5',N'-1.25',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 2.5',12),
('B1-TC08','B1',N'Kết quả lẻ',N'3',N'-1',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 0.333333333333333',13),
('B1-TC09','B1',N'Bỏ trống hệ số a',N'',N'2',NULL,NULL,'WINFORMS',N'Vui lòng nhập hệ số a!',14),
('B1-TC10','B1',N'Nhập chữ vào hệ số a',N'abc',N'2',NULL,NULL,'WINFORMS',N'Hệ số a không hợp lệ!',15),
('B1-TC11','B1',N'Nhập phân số',N'3/2',N'2',NULL,NULL,'WINFORMS',N'Hệ số a đang ở dạng phân số. Vui lòng đổi sang số thập phân.',16),
('B1-TC12','B1',N'Dữ liệu có khoảng trắng',N' 2 ',N' -4 ',NULL,NULL,'WINFORMS',N'Phương trình có nghiệm: x = 2',17),
('B1-TC13','B1',N'Giá trị đặc biệt NaN',N'NaN',N'2',NULL,NULL,'WINFORMS',N'Hệ số a không hợp lệ!',18),
('B1-TC14','B1',N'Giá trị đặc biệt Infinity',N'Infinity',N'2',NULL,NULL,'WINFORMS',N'Hệ số a không hợp lệ!',19),
('B1-TC15','B1',N'NULL phía SQL',N'NULL',N'2',NULL,NULL,'SQL',N'Hệ số a và b không được để trống',20),
('B1-TC16','B1',N'Hệ số a rất nhỏ nhưng khác 0',N'0.000001',N'-0.000002',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = 2',21),
('B1-TC17','B1',N'Nghiệm âm không nguyên',N'2',N'1',NULL,NULL,'SQL',N'Phương trình có nghiệm: x = -0.5',22);
GO

-- ============================================================
-- BÀI 2: 30 TESTCASES (B2-ADD-01..07, B2-TC01..23)
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1 VALUES
('B2-ADD-01','B2',N'Bỏ trống b',N'1',N'',N'2',NULL,'WINFORMS',N'Vui lòng nhập hệ số b!',101),
('B2-ADD-02','B2',N'Bỏ trống c',N'1',N'2',N'',NULL,'WINFORMS',N'Vui lòng nhập hệ số c!',102),
('B2-ADD-03','B2',N'b không phải số',N'1',N'abc',N'2',NULL,'WINFORMS',N'Hệ số b không hợp lệ!',103),
('B2-ADD-04','B2',N'c không phải số',N'1',N'2',N'abc',NULL,'WINFORMS',N'Hệ số c không hợp lệ!',104),
('B2-ADD-05','B2',N'Một hệ số vượt phạm vi double',N'1',N'1e309',N'1',NULL,'WINFORMS',N'Hệ số b không hợp lệ!',105),
('B2-ADD-06','B2',N'Delta dương rất gần 0',N'1',N'2',N'0.99999999',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = -0.9999 và x2 = -1.0001',106),
('B2-ADD-07','B2',N'Tất cả hệ số có khoảng trắng',N' 1 ',N' -3 ',N' 2 ',NULL,'WINFORMS',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',107),
('B2-TC01','B2',N'a=0, b=0, c=0',N'0',N'0',N'0',NULL,'SQL',N'Phương trình có vô số nghiệm',108),
('B2-TC02','B2',N'a=0, b=0, c khác 0',N'0',N'0',N'5',NULL,'SQL',N'Phương trình vô nghiệm',109),
('B2-TC03','B2',N'Suy biến thành bậc nhất',N'0',N'3',N'5',NULL,'SQL',N'Phương trình có 1 nghiệm: x = -1.66666666666667',110),
('B2-TC04','B2',N'Delta lớn hơn 0',N'1',N'-3',N'2',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',111),
('B2-TC05','B2',N'Delta bằng 0',N'1',N'-2',N'1',NULL,'SQL',N'Phương trình có nghiệm kép: x1 = x2 = 1',112),
('B2-TC06','B2',N'Delta nhỏ hơn 0',N'1',N'1',N'1',NULL,'SQL',N'Phương trình vô nghiệm',113),
('B2-TC07','B2',N'b bằng 0',N'1',N'0',N'-4',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = -2',114),
('B2-TC08','B2',N'c bằng 0',N'1',N'-5',N'0',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 5 và x2 = 0',115),
('B2-TC09','B2',N'Hệ số âm',N'-1',N'3',N'-2',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 1 và x2 = 2',116),
('B2-TC10','B2',N'Số thập phân',N'0.5',N'-1.5',N'1',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',117),
('B2-TC11','B2',N'Nghiệm lẻ',N'2',N'1',N'-1',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 0.5 và x2 = -1',118),
('B2-TC12','B2',N'Bỏ trống hệ số a',N'',N'2',N'3',NULL,'WINFORMS',N'Vui lòng nhập hệ số a!',119),
('B2-TC13','B2',N'Nhập chữ',N'abc',N'2',N'3',NULL,'WINFORMS',N'Hệ số a không hợp lệ!',120),
('B2-TC14','B2',N'Nhập phân số',N'1/2',N'2',N'3',NULL,'WINFORMS',N'Hệ số a đang ở dạng phân số. Vui lòng đổi sang số thập phân.',121),
('B2-TC15','B2',N'Dữ liệu có khoảng trắng',N' 1 ',N' -3 ',N' 2 ',NULL,'WINFORMS',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',122),
('B2-TC16','B2',N'NaN',N'NaN',N'2',N'3',NULL,'WINFORMS',N'Hệ số a không hợp lệ!',123),
('B2-TC17','B2',N'Infinity',N'Infinity',N'2',N'3',NULL,'WINFORMS',N'Hệ số a không hợp lệ!',124),
('B2-TC18','B2',N'Delta âm rất gần 0 vẫn phải vô nghiệm thực',N'1',N'2',N'1.00000001',NULL,'SQL',N'Phương trình vô nghiệm',125),
('B2-TC19','B2',N'Giá trị lớn',N'100000000',N'-300000000',N'200000000',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',126),
('B2-TC20','B2',N'Ký hiệu khoa học',N'1e0',N'-3e0',N'2e0',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 2 và x2 = 1',127),
('B2-TC21','B2',N'NULL phía SQL',N'NULL',N'2',N'3',NULL,'SQL',N'Hệ số a, b và c không được để trống',128),
('B2-TC22','B2',N'Nghiệm vô tỉ cần kiểm tra sai số',N'1',N'0',N'-2',NULL,'SQL',N'Phương trình có 2 nghiệm: x1 = 1.4142135623731 và x2 = -1.4142135623731',129),
('B2-TC23','B2',N'Suy biến bậc nhất có nghiệm bằng 0',N'0',N'5',N'0',NULL,'SQL',N'Phương trình có 1 nghiệm: x = 0',130);
GO

-- ============================================================
-- BÀI 4: 20 TESTCASES (B4-ADD-01..06, B4-TC01..14)
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1 VALUES
('B4-ADD-01','B4',N'Sinh nhật là hôm qua',NULL,NULL,NULL,N'HOMNAY_MINUS_20Y_1D','SQL',N'20',201),
('B4-ADD-02','B4',N'Sinh nhật là ngày mai',NULL,NULL,NULL,N'HOMNAY_MINUS_20Y_PLUS_1D','SQL',N'19',202),
('B4-ADD-03','B4',N'Người sinh 29/02 trong năm không nhuận',NULL,NULL,NULL,N'29/02/2000','SQL',N'Tuổi hợp lệ',203),
('B4-ADD-04','B4',N'Ngày 00/00/0000',NULL,NULL,NULL,N'00/00/0000','WINFORMS',N'Ngày sinh không tồn tại!',204),
('B4-ADD-05','B4',N'Ngày mơ hồ theo locale (03/04/2005)',NULL,NULL,NULL,N'03/04/2005','WINFORMS',N'Tuổi hợp lệ',205),
('B4-ADD-06','B4',N'Ngày nhỏ nhất SQL date (01/01/0001)',NULL,NULL,NULL,N'01/01/0001','SQL',N'Tuổi hợp lệ không overflow',206),
('B4-TC01','B4',N'Ngày sinh hợp lệ, đã qua sinh nhật trong năm',NULL,NULL,NULL,N'15/03/2006','SQL',N'Tuổi hợp lệ',207),
('B4-TC02','B4',N'Sinh đúng ngày hiện tại',NULL,NULL,NULL,N'HOMNAY','SQL',N'0',208),
('B4-TC03','B4',N'Ngày sinh ở tương lai',NULL,NULL,NULL,N'NGAYMAI','SQL',N'Ngày sinh không được lớn hơn ngày hiện tại!',209),
('B4-TC04','B4',N'Ngày sinh đầu năm',NULL,NULL,NULL,N'01/01/2000','SQL',N'Tuổi hợp lệ',210),
('B4-TC05','B4',N'Ngày sinh cuối năm để kiểm tra chưa đến sinh nhật',NULL,NULL,NULL,N'31/12/2000','SQL',N'Tuổi hợp lệ',211),
('B4-TC06','B4',N'NULL phía SQL',NULL,NULL,NULL,N'NULL','SQL',N'Ngày sinh không được để trống!',212),
('B4-TC07','B4',N'Bỏ trống ngày sinh',NULL,NULL,NULL,N'','WINFORMS',N'Vui lòng nhập ngày sinh!',213),
('B4-TC08','B4',N'Nhập chữ',NULL,NULL,NULL,N'abc','WINFORMS',N'Ngày sinh không đúng định dạng!',214),
('B4-TC09','B4',N'Ngày không tồn tại',NULL,NULL,NULL,N'31/02/2005','WINFORMS',N'Ngày sinh không tồn tại!',215),
('B4-TC10','B4',N'Dữ liệu có khoảng trắng',NULL,NULL,NULL,N'  15/03/2006  ','WINFORMS',N'Tuổi hợp lệ',216),
('B4-TC11','B4',N'Ngày nhuận hợp lệ',NULL,NULL,NULL,N'29/02/2000','SQL',N'Tuổi hợp lệ',217),
('B4-TC12','B4',N'Định dạng ISO yyyy-MM-dd',NULL,NULL,NULL,N'2005-03-15','WINFORMS',N'Tuổi hợp lệ',218),
('B4-TC13','B4',N'Ngày nhuận không hợp lệ ở năm không nhuận',NULL,NULL,NULL,N'29/02/2001','WINFORMS',N'Ngày sinh không tồn tại!',219),
('B4-TC14','B4',N'Chỉ nhập năm, thiếu ngày và tháng',NULL,NULL,NULL,N'2005','WINFORMS',N'Ngày sinh không đúng định dạng!',220);
GO

-- ============================================================
-- PROCEDURE LOAD TESTCASE
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom1
    @Bai VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MaCase,
        Bai,
        MoTa,
        GiaTriA,
        GiaTriB,
        GiaTriC,
        NamSinh,
        LoaiTest,
        KyVong
    FROM dbo.Testcase_Nhom1
    WHERE Bai = @Bai
    ORDER BY ThuTu;
END
GO
