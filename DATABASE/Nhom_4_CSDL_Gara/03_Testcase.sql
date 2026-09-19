USE QL_Gara;
GO
DROP TABLE IF EXISTS dbo.Testcase_Nhom4;
CREATE TABLE dbo.Testcase_Nhom4(ID_Testcase varchar(20) PRIMARY KEY,MaBai varchar(10),MoTa nvarchar(200),DuLieuNhap nvarchar(200),ThuTu int);
INSERT dbo.Testcase_Nhom4 VALUES
('B91-01','B9.1',N'Thợ chưa nhận công việc',N'T01, T04, T06',1),('B92-01','B9.2',N'Hợp đồng đã nghiệm thu còn nợ',N'HD04, còn nợ 2.500.000',2),('B92-02','B9.2',N'Hợp đồng đủ tiền không xuất hiện',N'HD01 và HD03',3),
('B93-01','B9.3',N'Hợp đồng phải hoàn tất trước 31/12/2002',N'HD01 và HD04',4),
('B94-01','B9.4',N'Thợ làm nhiều công việc nhất',N'T02, 4 công việc; xử lý đồng hạng',5),('B95-01','B9.5',N'Thợ có tổng trị giá cao nhất',N'T02, tổng 5.800.000; xử lý đồng hạng',6),
('B9RB-01','B9.RB',N'Nhóm trưởng phải thuộc cùng nhóm',N'Thử hợp lệ cùng nhóm và sai khác nhóm',7),('B9RB-02','B9.RB',N'Không đổi nhóm của trưởng nhóm khi còn thành viên',N'UPDATE T01 sang nhóm 2 phải bị chặn',8),
('B9RB-03','B9.RB',N'Một xe không được có hai hợp đồng cùng ngày',N'INSERT trùng (NgayHD,SoXe) phải bị chặn',9),
('B9RB-04','B9.RB',N'Phiếu thu phải thuộc đúng khách của hợp đồng',N'INSERT SoHD=HD01,MaKH=KH02 phải bị chặn',10),
('B9RB-05','B9.RB',N'Cập nhật nhiều dòng cùng lúc vẫn giữ đúng nhóm trưởng',N'Đổi đồng thời trưởng nhóm và thành viên sang cùng nhóm được chấp nhận',11),
('B9RB-06','B9.RB',N'Cập nhật nhiều dòng không được lọt quan hệ khác nhóm',N'Đổi nhóm thành viên nhưng giữ trưởng nhóm cũ phải bị chặn',12),
('B94-02','B9.4',N'Nhiều thợ đồng hạng cao nhất phải trả đủ',N'Tạo dữ liệu hòa trong transaction rồi rollback',13),
('B95-02','B9.5',N'Nhiều thợ đồng tổng trị giá cao nhất phải trả đủ',N'Tạo dữ liệu hòa trong transaction rồi rollback',14);
GO
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom4 AS
SELECT ID_Testcase,MaBai,MoTa,DuLieuNhap FROM dbo.Testcase_Nhom4 ORDER BY ThuTu;
GO
