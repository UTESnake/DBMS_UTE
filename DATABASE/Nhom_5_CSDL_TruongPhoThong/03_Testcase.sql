USE [QL_TruongPhoThong];
GO

/*============================================================
  03_Testcase.sql - TESTCASE BÀI 10 (CSDL TRƯỜNG PHỔ THÔNG)
  - Đầy đủ 36 testcase Bài 10 (B10.1a..c, B10.2a..e)
  - Có cột KyVong lưu kết quả mong đợi chuẩn xác theo BO_TESTCASE_CHI_TIET.md
============================================================*/

DROP TABLE IF EXISTS dbo.Testcase_Nhom5;
GO

CREATE TABLE dbo.Testcase_Nhom5
(
    ID_Testcase VARCHAR(30) PRIMARY KEY,
    MaBai       VARCHAR(10) NOT NULL,
    NhomLoi     VARCHAR(20) NOT NULL,
    MoTa        NVARCHAR(500) NOT NULL,
    DuLieuNhap  NVARCHAR(500) NULL,
    KyVong      NVARCHAR(500) NULL,
    ThuTu       INT NOT NULL
);
GO

/*========================== BÀI 10.1a (8 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-01','B10.1a','BỔ SUNG',N'Giáo viên không chủ nhiệm môn nào',N'GV04 coi buổi hợp lệ',N'Được phép nếu không trùng giờ.',1),
('B10-ADD-10','B10.1a','BỔ SUNG',N'Exception không đúng quy tắc mong đợi',N'Sai tên bảng/cột hoặc mất kết nối',N'Đánh dấu ERROR, không phải PASS "đã chặn".',2),
('B101A-01','B10.1a','SQL/Form',N'Cho phép một giáo viên gác nhiều buổi trong học kỳ 1',N'GV01 gác TOÁN, TIN HỌC, HÓA HỌC',N'Hợp lệ: GV01 có thể gác nhiều buổi khác thời điểm và không phải môn VĂN HỌC.',3),
('B101A-02','B10.1a','SQL/Form',N'Chặn giáo viên gác môn mình chủ nhiệm',N'Thử phân GV01 vào buổi VĂN HỌC P101',N'Bị đúng trigger "không được gác môn chủ nhiệm" chặn; không thêm dòng.',4),
('B101A-03','B10.1a','SQL/Form',N'Chặn giáo viên gác hai phòng cùng thời điểm',N'Thử phân GV02 vào P109 lúc 2026-05-10 07:30',N'Bị unique (MaGV,HKY,Ngay,Gio) chặn; không thể gác hai phòng cùng lúc.',5),
('B101A-04','B10.1a','SQL/Form',N'UPDATE phân công sang đúng giáo viên khác phải lưu khóa mới',N'Đổi một phân công hợp lệ trong transaction',N'UPDATE hợp lệ lưu đúng MaGV mới; sau kiểm tra phải rollback.',6),
('B101A-05','B10.1a','SQL/Form',N'UPDATE nhiều phân công có một dòng vi phạm phải rollback cả lệnh',N'Đổi nhiều MaGV, một người chủ nhiệm đúng môn thi',N'Một dòng vi phạm làm rollback toàn bộ UPDATE nhiều dòng.',7),
('B101A-06','B10.1a','SQL/Form',N'Đổi môn chủ nhiệm làm vi phạm lịch coi thi phải bị chặn',N'UPDATE B10_GV.MaMH',N'Đổi MaMH gây xung đột phân công hiện có bị tg_B10_KiemTraGiaoVien chặn.',8);
GO

/*========================== BÀI 10.1b (5 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-02','B10.1b','BỔ SUNG',N'Môn 30 tiết thi 121 phút',N'MH02; TGThi=121',N'Bị trigger chặn giống 119 phút.',9),
('B101B-01','B10.1b','SQL/Form',N'Đúng biên 30 tiết phải thi 120 phút',N'MH02: 30 tiết, 120 phút',N'30 tiết + 120 phút được chấp nhận.',10),
('B101B-02','B10.1b','SQL/Form',N'Dưới 30 tiết không áp dụng quy tắc 120 phút',N'MH05: 29 tiết, 60 phút',N'29 tiết + 60 phút được chấp nhận.',11),
('B101B-03','B10.1b','SQL/Form',N'Chặn môn 30 tiết thi khác 120 phút',N'Thử thêm MH02 với TGThi=119',N'30 tiết + 119 phút bị đúng trigger chặn.',12),
('B101B-04','B10.1b','SQL/Form',N'UPDATE thời lượng môn 30 tiết sang 119 phải bị chặn',N'UPDATE B10_BUOITHI.TGThi',N'UPDATE 30 tiết từ 120 xuống 119 bị chặn.',13);
GO

/*========================== BÀI 10.1c (7 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-03','B10.1c','BỔ SUNG',N'Môn 45 tiết thi 151 phút',N'MH01; TGThi=151',N'Bị trigger chặn.',14),
('B101C-01','B10.1c','SQL/Form',N'Dưới biên 45 tiết không bắt buộc 150 phút',N'MH06: 44 tiết, 90 phút',N'44 tiết + 90 phút được chấp nhận.',15),
('B101C-02','B10.1c','SQL/Form',N'Đúng biên 45 tiết phải thi 150 phút',N'MH01: 45 tiết, 150 phút',N'45 tiết + 150 phút được chấp nhận.',16),
('B101C-03','B10.1c','SQL/Form',N'Trên 45 tiết phải thi 150 phút',N'MH03/MH07: 60/46 tiết, 150 phút',N'46/60 tiết + 150 phút được chấp nhận.',17),
('B101C-04','B10.1c','SQL/Form',N'Chặn môn từ 45 tiết thi khác 150 phút',N'Thử thêm MH07 với TGThi=149',N'Từ 45 tiết + 149 phút bị chặn.',18),
('B101C-05','B10.1c','SQL/Form',N'UPDATE thời lượng môn từ 45 tiết sang 149 phải bị chặn',N'UPDATE B10_BUOITHI.TGThi',N'UPDATE thời gian thi xuống 149 bị chặn.',19),
('B101C-06','B10.1c','SQL/Form',N'Đổi số tiết làm lịch thi hiện có sai phải bị chặn',N'UPDATE B10_MHOC.SoTiet',N'UPDATE SoTiet làm lịch cũ sai bị chặn.',20);
GO

/*========================== BÀI 10.2a (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B102A-01','B10.2a','SQL/Form',N'Giáo viên chủ nhiệm môn từ 45 tiết',N'GV01, GV03, GV09',N'Trả đúng GV01, GV03, GV09.',21),
('B102A-02','B10.2a','SQL/Form',N'Không lấy môn dưới 45 tiết hoặc giáo viên chưa chủ nhiệm',N'GV02, GV04, GV05, GV06, GV07, GV08, GV10',N'Không trả GV02, GV04, GV05, GV06, GV07, GV08, GV10.',22);
GO

/*========================== BÀI 10.2b (4 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-04','B10.2b','BỔ SUNG',N'Học kỳ không có phân công',N'HocKy=99 hoặc học kỳ hợp lệ nhưng rỗng',N'Tham số ngoài miền phải bị Form chặn; học kỳ hợp lệ rỗng trả 0 dòng.',23),
('B10-ADD-05','B10.2b','BỔ SUNG',N'NULL học kỳ gọi trực tiếp SQL',N'HocKy=NULL',N'TVF trả 0 dòng; Form phải báo thiếu tham số.',24),
('B102B-01','B10.2b','SQL/Form',N'Giáo viên gác học kỳ 1; không trùng khi gác nhiều buổi',N'GV01 chỉ xuất hiện một lần',N'HK1 trả 7 giáo viên: GV01, GV02, GV03, GV04, GV06, GV08, GV09; mỗi người một dòng.',25),
('B102B-02','B10.2b','SQL/Form',N'Tham số học kỳ phải được áp dụng, không khóa cứng học kỳ 1',N'Học kỳ 2: GV05, GV10',N'HK2 trả đúng GV05, GV10.',26);
GO

/*========================== BÀI 10.2c (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B102C-01','B10.2c','SQL/Form',N'Giáo viên không gác học kỳ 1',N'GV05, GV07, GV10',N'HK1 trả đúng GV05, GV07, GV10.',27),
('B102C-02','B10.2c','SQL/Form',N'Giáo viên chỉ gác học kỳ 2 vẫn thuộc nhóm không gác học kỳ 1',N'GV05, GV10',N'GV05 và GV10 vẫn thuộc kết quả không gác HK1 dù có gác HK2.',28);
GO

/*========================== BÀI 10.2d (5 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-06','B10.2d','BỔ SUNG',N'Tên môn không tồn tại',N'KHÔNG TỒN TẠI',N'Trả 0 dòng và Form báo không có dữ liệu.',29),
('B10-ADD-07','B10.2d','BỔ SUNG',N'Tên môn rỗng/NULL',N'"" và NULL',N'Form chặn trước SQL.',30),
('B10-ADD-08','B10.2d','BỔ SUNG',N'Khoảng trắng và khác hoa thường',N'toán',N'Form trim; kết quả phụ thuộc collation không phân biệt hoa thường, phải trả 2 lịch TOÁN.',31),
('B102D-01','B10.2d','SQL/Form',N'Lấy mọi lịch thi môn VĂN HỌC',N'P101, P201, P203',N'VĂN HỌC trả P101, P201, P203.',32),
('B102D-02','B10.2d','SQL/Form',N'Tham số tên môn phải lọc được môn khác VĂN HỌC',N'TOÁN',N'TOÁN trả P102 (HK1) và P202 (HK2).',33);
GO

/*========================== BÀI 10.2e (3 testcases) ==========================*/
INSERT dbo.Testcase_Nhom5 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B10-ADD-09','B10.2e','BỔ SUNG',N'Môn có GV chủ nhiệm nhưng GV không gác',N'VẬT LÝ -> GV07',N'Trả 0 dòng, không lỗi.',34),
('B102E-01','B10.2e','SQL/Form',N'Các buổi gác của giáo viên chủ nhiệm VĂN HỌC',N'GV01 gác TOÁN, TIN HỌC, HÓA HỌC, TIẾNG ANH',N'GV01: 4 buổi gác TOÁN, TIN HỌC, HÓA HỌC, TIẾNG ANH.',35),
('B102E-02','B10.2e','SQL/Form',N'Tham số môn chủ nhiệm phải lọc được giáo viên môn khác',N'TOÁN -> GV02',N'GV02: 2 buổi gác VĂN HỌC và SINH HỌC.',36);
GO

/*============================================================
  PROCEDURE LOAD TESTCASE BÀI 10
============================================================*/
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom5
    @MaBai VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong
    FROM dbo.Testcase_Nhom5
    WHERE (@MaBai IS NULL OR MaBai = @MaBai)
    ORDER BY ThuTu;
END
GO
