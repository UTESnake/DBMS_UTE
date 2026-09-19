IF DB_NAME() <> N'QL_TruongPhoThong'
    THROW 50000, N'Hãy kết nối tới database QL_TruongPhoThong trước khi chạy testcase.', 1;
GO
DROP TABLE IF EXISTS dbo.Testcase_Nhom5;
CREATE TABLE dbo.Testcase_Nhom5(ID_Testcase varchar(20) PRIMARY KEY,MaBai varchar(10),MoTa nvarchar(200),DuLieuNhap nvarchar(200),ThuTu int);
INSERT dbo.Testcase_Nhom5 VALUES
('B101A-01','B10.1a',N'Cho phép một giáo viên gác nhiều buổi trong học kỳ 1',N'GV01 gác TOÁN, TIN HỌC, HÓA HỌC',1),
('B101A-02','B10.1a',N'Chặn giáo viên gác môn mình chủ nhiệm',N'Thử phân GV01 vào buổi VĂN HỌC P101',2),
('B101A-03','B10.1a',N'Chặn giáo viên gác hai phòng cùng thời điểm',N'Thử phân GV02 vào P109 lúc 2026-05-10 07:30',3),
('B101B-01','B10.1b',N'Đúng biên 30 tiết phải thi 120 phút',N'MH02: 30 tiết, 120 phút',4),
('B101B-02','B10.1b',N'Dưới 30 tiết không áp dụng quy tắc 120 phút',N'MH05: 29 tiết, 60 phút',5),
('B101B-03','B10.1b',N'Chặn môn 30 tiết thi khác 120 phút',N'Thử thêm MH02 với TGThi=119',6),
('B101C-01','B10.1c',N'Dưới biên 45 tiết không bắt buộc 150 phút',N'MH06: 44 tiết, 90 phút',7),
('B101C-02','B10.1c',N'Đúng biên 45 tiết phải thi 150 phút',N'MH01: 45 tiết, 150 phút',8),
('B101C-03','B10.1c',N'Trên 45 tiết phải thi 150 phút',N'MH03/MH07: 60/46 tiết, 150 phút',9),
('B101C-04','B10.1c',N'Chặn môn từ 45 tiết thi khác 150 phút',N'Thử thêm MH07 với TGThi=149',10),
('B102A-01','B10.2a',N'Giáo viên chủ nhiệm môn từ 45 tiết',N'GV01, GV03, GV09',11),
('B102A-02','B10.2a',N'Không lấy môn dưới 45 tiết hoặc giáo viên chưa chủ nhiệm',N'GV02, GV04, GV05, GV06, GV07, GV08, GV10',12),
('B102B-01','B10.2b',N'Giáo viên gác học kỳ 1; không trùng khi gác nhiều buổi',N'GV01 chỉ xuất hiện một lần',13),
('B102C-01','B10.2c',N'Giáo viên không gác học kỳ 1',N'GV05, GV07, GV10',14),
('B102D-01','B10.2d',N'Lấy mọi lịch thi môn VĂN HỌC',N'P101, P201, P203',15),
('B102E-01','B10.2e',N'Các buổi gác của giáo viên chủ nhiệm VĂN HỌC',N'GV01 gác TOÁN, TIN HỌC, HÓA HỌC, TIẾNG ANH',16),
('B101A-04','B10.1a',N'UPDATE phân công sang đúng giáo viên khác phải lưu khóa mới',N'Đổi một phân công hợp lệ trong transaction',17),
('B101A-05','B10.1a',N'UPDATE nhiều phân công có một dòng vi phạm phải rollback cả lệnh',N'Đổi nhiều MaGV, một người chủ nhiệm đúng môn thi',18),
('B101A-06','B10.1a',N'Đổi môn chủ nhiệm làm vi phạm lịch coi thi phải bị chặn',N'UPDATE B10_GV.MaMH',19),
('B101B-04','B10.1b',N'UPDATE thời lượng môn 30 tiết sang 119 phải bị chặn',N'UPDATE B10_BUOITHI.TGThi',20),
('B101C-05','B10.1c',N'UPDATE thời lượng môn từ 45 tiết sang 149 phải bị chặn',N'UPDATE B10_BUOITHI.TGThi',21),
('B101C-06','B10.1c',N'Đổi số tiết làm lịch thi hiện có sai phải bị chặn',N'UPDATE B10_MHOC.SoTiet',22),
('B102B-02','B10.2b',N'Tham số học kỳ phải được áp dụng, không khóa cứng học kỳ 1',N'Học kỳ 2: GV05, GV10',23),
('B102C-02','B10.2c',N'Giáo viên chỉ gác học kỳ 2 vẫn thuộc nhóm không gác học kỳ 1',N'GV05, GV10',24),
('B102D-02','B10.2d',N'Tham số tên môn phải lọc được môn khác VĂN HỌC',N'TOÁN',25),
('B102E-02','B10.2e',N'Tham số môn chủ nhiệm phải lọc được giáo viên môn khác',N'TOÁN -> GV02',26);
GO
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom5 AS
SELECT ID_Testcase,MaBai,MoTa,DuLieuNhap FROM dbo.Testcase_Nhom5 ORDER BY ThuTu;
GO
