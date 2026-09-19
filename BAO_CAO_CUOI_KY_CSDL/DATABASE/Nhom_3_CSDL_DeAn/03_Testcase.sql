USE QL_DeAn;
GO
DROP TABLE IF EXISTS dbo.Testcase_Nhom3;
CREATE TABLE dbo.Testcase_Nhom3(ID_Testcase varchar(20) PRIMARY KEY,MaBai varchar(10),MoTa nvarchar(200),DuLieuNhap nvarchar(200),ThuTu int);
INSERT dbo.Testcase_Nhom3 VALUES
('B71-01','B7.1',N'Phòng hợp lệ',N'PB01',1),('B71-02','B7.1',N'Phòng không tồn tại',N'XXX',2),('B71-03','B7.1',N'Phòng không nhân viên',N'PB00',3),('B71-04','B7.1',N'Mã phòng rỗng',N'',4),
('B72-01','B7.2',N'Có tham gia đề án',N'NV01;DA01',5),('B72-02','B7.2',N'Không tham gia đề án',N'NV01;DA03',6),('B72-03','B7.2',N'Nhân viên không tồn tại',N'XXX;DA01',7),('B72-04','B7.2',N'Đề án không tồn tại',N'NV01;XXX',8),
('B73-01','B7.3',N'Tổng trung bình các phòng',N'',9),('B74-01','B7.4',N'Dưới 30 giờ',N'29',10),('B74-02','B7.4',N'Biên 30 giờ',N'30',11),('B74-03','B7.4',N'Biên 60 giờ',N'60',12),('B74-04','B7.4',N'Trên 60 giờ',N'61',13),('B74-05','B7.4',N'Biên 99 giờ',N'99',14),('B74-06','B7.4',N'Biên 100 giờ',N'100',15),('B74-07','B7.4',N'Biên 149 giờ',N'149',16),('B74-08','B7.4',N'Biên 150 giờ',N'150',17),('B74-09','B7.4',N'Giờ âm',N'-1',18),('B74-10','B7.4',N'NULL/rỗng',N'',19),
('B75-01','B7.5',N'Mọi phòng kể cả 0 đề án',N'',20),('B76A-01','B7.6A',N'Inline TVF',N'',21),('B76B-01','B7.6B',N'Multistatement TVF',N'',22),
('B72-05','B7.2',N'Tổng giờ phân công bằng 0 không được chia cho 0',N'NV05;DA03',23),
('B76-03','B7.6',N'Hai TVF phải giống nhau theo hai chiều EXCEPT',N'',24),
('B81-01','B8.1',N'Đề án có hơn 2 nhân viên',N'DA01, DA03',101),
('B81-02','B8.1',N'Đề án đúng 2 nhân viên phải bị loại',N'DA02',102),
('B82-01','B8.2',N'Điều kiện hơn 2 người tính trên toàn phòng',N'PB01, PB02, PB05',103),
('B82-02','B8.2',N'Chỉ đếm nhân viên lương trên 25000',N'PB05 = 3',104),
('B83-01','B8.3',N'Phòng có lương trung bình trên 30000',N'PB01, PB02',105),
('B84-01','B8.4',N'Đếm đúng nhân viên nam trong phòng đạt điều kiện',N'PB01 = 2; PB02 = 1',106),
('B85-01','B8.5',N'Mỗi đề án đều xuất hiện kể cả kết quả bằng 0',N'DA02 và DA04 = 0',107),
('B85-02','B8.5',N'Đếm DISTINCT nhân viên phòng PB05 theo đề án',N'DA01 = 1; DA05 = 3',108);
GO
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom3 AS SELECT ID_Testcase,MaBai,MoTa,DuLieuNhap FROM dbo.Testcase_Nhom3 ORDER BY ThuTu;
GO
