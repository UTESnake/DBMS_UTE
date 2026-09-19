USE [QL_DeAn]
GO

-- ============================================================
-- NHÓM 1 - DANH SÁCH TESTCASE
-- BÀI 1, BÀI 2, BÀI 4
--
-- QUY TẮC:
-- - File này CHỈ lưu dữ liệu đầu vào testcase.
-- - Không lưu kết quả mong đợi.
-- - Không lưu PASS / FAIL.
-- - Kết quả chỉ sinh ra khi người dùng bấm "Chạy testcase".
-- ============================================================


-- ============================================================
-- 1. TẠO BẢNG TESTCASE
-- ============================================================

IF OBJECT_ID('dbo.Testcase_Nhom1', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Testcase_Nhom1
    (
        MaCase      VARCHAR(20) PRIMARY KEY,
        Bai         VARCHAR(10) NOT NULL,

        MoTa        NVARCHAR(250),

        -- Lưu dữ liệu dạng chuỗi để có thể chứa:
        -- số, NULL, rỗng, abc, NaN, Infinity, 3/2...
        GiaTriA     NVARCHAR(100) NULL,
        GiaTriB     NVARCHAR(100) NULL,
        GiaTriC     NVARCHAR(100) NULL,

        NamSinh     NVARCHAR(100) NULL,

        LoaiTest    VARCHAR(30),

        ThuTu       INT
    );
END
GO


-- ============================================================
-- XÓA TESTCASE CŨ
-- Để chạy lại file nhiều lần không bị trùng khóa chính.
-- ============================================================

DELETE FROM dbo.Testcase_Nhom1;
GO


-- ============================================================
-- BÀI 1
-- PHƯƠNG TRÌNH BẬC NHẤT ax + b = 0
--
-- CHỈ NHẬP DỮ LIỆU TEST
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC01','B1',
 N'a = 0, b = 0',
 N'0',N'0',NULL,NULL,
 'SQL',1);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC02','B1',
 N'a = 0, b khác 0',
 N'0',N'5',NULL,NULL,
 'SQL',2);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC03','B1',
 N'Nghiệm duy nhất',
 N'2',N'-4',NULL,NULL,
 'SQL',3);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC04','B1',
 N'Kiểm tra dấu: a âm, b dương',
 N'-2',N'8',NULL,NULL,
 'SQL',4);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC05','B1',
 N'Kiểm tra dấu: a âm, b âm',
 N'-2',N'-8',NULL,NULL,
 'SQL',5);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC06','B1',
 N'b bằng 0',
 N'5',N'0',NULL,NULL,
 'SQL',6);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC07','B1',
 N'Số thập phân',
 N'0.5',N'-1.25',NULL,NULL,
 'SQL',7);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC08','B1',
 N'Kết quả lẻ',
 N'3',N'-1',NULL,NULL,
 'SQL',8);


-- ============================================================
-- BÀI 1 - TESTCASE NHẬP LIỆU WINFORMS
-- Vẫn chỉ lưu dữ liệu đầu vào.
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC09','B1',
 N'Bỏ trống hệ số a',
 N'',N'2',NULL,NULL,
 'WINFORMS',9);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC10','B1',
 N'Nhập chữ vào hệ số a',
 N'abc',N'2',NULL,NULL,
 'WINFORMS',10);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC11','B1',
 N'Nhập phân số',
 N'3/2',N'2',NULL,NULL,
 'WINFORMS',11);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC12','B1',
 N'Dữ liệu có khoảng trắng',
 N' 2 ',N' -4 ',NULL,NULL,
 'WINFORMS',12);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC13','B1',
 N'Giá trị đặc biệt NaN',
 N'NaN',N'2',NULL,NULL,
 'WINFORMS',13);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC14','B1',
 N'Giá trị đặc biệt Infinity',
 N'Infinity',N'2',NULL,NULL,
 'WINFORMS',14);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC15','B1',
 N'NULL phía SQL',
 N'NULL',N'2',NULL,NULL,
 'SQL',15);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC16','B1',
 N'Hệ số a rất nhỏ nhưng khác 0',
 N'0.000001',N'-0.000002',NULL,NULL,
 'SQL',16);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B1-TC17','B1',
 N'Nghiệm âm không nguyên',
 N'2',N'1',NULL,NULL,
 'SQL',17);


-- ============================================================
-- BÀI 2
-- PHƯƠNG TRÌNH BẬC HAI ax² + bx + c = 0
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC01','B2',
 N'a=0, b=0, c=0',
 N'0',N'0',N'0',NULL,
 'SQL',101);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC02','B2',
 N'a=0, b=0, c khác 0',
 N'0',N'0',N'5',NULL,
 'SQL',102);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC03','B2',
 N'Suy biến thành bậc nhất',
 N'0',N'3',N'5',NULL,
 'SQL',103);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC04','B2',
 N'Delta lớn hơn 0',
 N'1',N'-3',N'2',NULL,
 'SQL',104);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC05','B2',
 N'Delta bằng 0',
 N'1',N'-2',N'1',NULL,
 'SQL',105);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC06','B2',
 N'Delta nhỏ hơn 0',
 N'1',N'1',N'1',NULL,
 'SQL',106);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC07','B2',
 N'b bằng 0',
 N'1',N'0',N'-4',NULL,
 'SQL',107);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC08','B2',
 N'c bằng 0',
 N'1',N'-5',N'0',NULL,
 'SQL',108);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC09','B2',
 N'Hệ số âm',
 N'-1',N'3',N'-2',NULL,
 'SQL',109);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC10','B2',
 N'Số thập phân',
 N'0.5',N'-1.5',N'1',NULL,
 'SQL',110);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC11','B2',
 N'Nghiệm lẻ',
 N'2',N'1',N'-1',NULL,
 'SQL',111);


-- ------------------------------------------------------------
-- Testcase nhập liệu WinForms Bài 2
-- ------------------------------------------------------------

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC12','B2',
 N'Bỏ trống hệ số a',
 N'',N'2',N'3',NULL,
 'WINFORMS',112);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC13','B2',
 N'Nhập chữ',
 N'abc',N'2',N'3',NULL,
 'WINFORMS',113);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC14','B2',
 N'Nhập phân số',
 N'1/2',N'2',N'3',NULL,
 'WINFORMS',114);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC15','B2',
 N'Dữ liệu có khoảng trắng',
 N' 1 ',N' -3 ',N' 2 ',NULL,
 'WINFORMS',115);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC16','B2',
 N'NaN',
 N'NaN',N'2',N'3',NULL,
 'WINFORMS',116);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC17','B2',
 N'Infinity',
 N'Infinity',N'2',N'3',NULL,
 'WINFORMS',117);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC18','B2',
 N'Delta âm rất gần 0 vẫn phải vô nghiệm thực',
 N'1',N'2',N'1.00000001',NULL,
 'SQL',118);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC19','B2',
 N'Giá trị lớn',
 N'100000000',
 N'-300000000',
 N'200000000',
 NULL,
 'SQL',119);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC20','B2',
 N'Ký hiệu khoa học',
 N'1e0',N'-3e0',N'2e0',NULL,
 'SQL',120);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC21','B2',
 N'NULL phía SQL',
 N'NULL',N'2',N'3',NULL,
 'SQL',121);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC22','B2',
 N'Nghiệm vô tỉ cần kiểm tra sai số',
 N'1',N'0',N'-2',NULL,
 'SQL',122);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B2-TC23','B2',
 N'Suy biến bậc nhất có nghiệm bằng 0',
 N'0',N'5',N'0',NULL,
 'SQL',123);


-- ============================================================
-- BÀI 4
-- FUNCTION TÍNH TUỔI
-- ============================================================

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC01','B4',
 N'Ngày sinh hợp lệ, đã qua sinh nhật trong năm',
 NULL,NULL,NULL,N'15/03/2006',
 'SQL',201);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC02','B4',
 N'Sinh đúng ngày hiện tại',
 NULL,NULL,NULL,N'HOMNAY',
 'SQL',202);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC03','B4',
 N'Ngày sinh ở tương lai',
 NULL,NULL,NULL,N'NGAYMAI',
 'SQL',203);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC04','B4',
 N'Ngày sinh đầu năm',
 NULL,NULL,NULL,N'01/01/2000',
 'SQL',204);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC05','B4',
 N'Ngày sinh cuối năm để kiểm tra chưa đến sinh nhật',
 NULL,NULL,NULL,N'31/12/2000',
 'SQL',205);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC06','B4',
 N'NULL phía SQL',
 NULL,NULL,NULL,N'NULL',
 'SQL',206);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC07','B4',
 N'Bỏ trống ngày sinh',
 NULL,NULL,NULL,N'',
 'WINFORMS',207);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC08','B4',
 N'Nhập chữ',
 NULL,NULL,NULL,N'abc',
 'WINFORMS',208);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC09','B4',
 N'Ngày không tồn tại',
 NULL,NULL,NULL,N'31/02/2005',
 'WINFORMS',209);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC10','B4',
 N'Dữ liệu có khoảng trắng',
 NULL,NULL,NULL,N' 15/03/2006 ',
 'WINFORMS',210);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC11','B4',
 N'Ngày nhuận hợp lệ',
 NULL,NULL,NULL,N'29/02/2000',
 'SQL',211);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC12','B4',
 N'Định dạng ISO yyyy-MM-dd',
 NULL,NULL,NULL,N'2005-03-15',
 'WINFORMS',212);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC13','B4',
 N'Ngày nhuận không hợp lệ ở năm không nhuận',
 NULL,NULL,NULL,N'29/02/2001',
 'WINFORMS',213);

INSERT INTO dbo.Testcase_Nhom1
VALUES
('B4-TC14','B4',
 N'Chỉ nhập năm, thiếu ngày và tháng',
 NULL,NULL,NULL,N'2005',
 'WINFORMS',214);
GO


-- ============================================================
-- PROCEDURE LOAD TESTCASE
--
-- CHỈ trả dữ liệu đầu vào.
-- Không chạy Function/Stored Procedure.
-- Không có kết quả thực tế.
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

        LoaiTest

    FROM dbo.Testcase_Nhom1

    WHERE Bai = @Bai

    ORDER BY ThuTu;
END
GO
