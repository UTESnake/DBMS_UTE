USE [QL_DeAn];
GO

/*============================================================
  03_Testcase.sql - TESTCASE BÀI 7 & BÀI 8 (CSDL ĐỀ ÁN)
  - Đầy đủ 32 testcase Bài 7 và 16 testcase Bài 8 (Tổng cộng: 48 testcases)
  - Có cột KyVong lưu kết quả mong đợi chuẩn xác theo BO_TESTCASE_CHI_TIET.md
============================================================*/

DROP TABLE IF EXISTS dbo.Testcase_Nhom3;
GO

CREATE TABLE dbo.Testcase_Nhom3
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

/*========================== BÀI 7.1 (7 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B7-ADD-01','B7.1','BỔ SUNG',N'Mã phòng có khoảng trắng',N'  PB01  ',N'Form trim và trả 35,666.67.',1),
('B7-ADD-02','B7.1','BỔ SUNG',N'Mã phòng NULL gọi trực tiếp SQL',N'NULL',N'Hàm trả 0; Form phải coi là thiếu input.',2),
('B7-ADD-03','B7.1','BỔ SUNG',N'Phòng có nhân viên lương bằng 0',N'Dựng PBX + NV lương 0',N'Trả 0 nhưng Form phải phân biệt với phòng không tồn tại/phòng rỗng.',3),
('B71-01','B7.1','SQL/Form',N'Phòng hợp lệ',N'PB01',N'SQL trả 35,666.67 cho PB01.',4),
('B71-02','B7.1','SQL/Form',N'Phòng không tồn tại',N'XXX',N'Hàm hiện trả 0; Form đúng phải báo "Mã phòng không tồn tại", không được coi 0 là lương trung bình thật.',5),
('B71-03','B7.1','SQL/Form',N'Phòng không nhân viên',N'PB00',N'SQL trả 0; Form nên ghi rõ phòng tồn tại nhưng chưa có nhân viên.',6),
('B71-04','B7.1','SQL/Form',N'Mã phòng rỗng',N'',N'Form phải chặn mã rỗng trước khi gọi SQL.',7);
GO

/*========================== BÀI 7.2 (7 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B7-ADD-04','B7.2','BỔ SUNG',N'Mã nhân viên hoặc đề án rỗng',N';DA01 và NV01;',N'Form chặn trường tương ứng.',8),
('B7-ADD-05','B7.2','BỔ SUNG',N'Khoảng trắng quanh hai mã',N' NV01 ; DA01 ',N'Trim và trả 15,737.70.',9),
('B72-01','B7.2','SQL/Form',N'Có tham gia đề án',N'NV01;DA01',N'SQL trả 15,737.70 (= 32,000 × 30 / 61).',10),
('B72-02','B7.2','SQL/Form',N'Không tham gia đề án',N'NV01;DA03',N'SQL trả 0; Form ghi rõ nhân viên không tham gia đề án.',11),
('B72-03','B7.2','SQL/Form',N'Nhân viên không tồn tại',N'XXX;DA01',N'Hàm hiện trả 0; Form phải phân biệt nhân viên không tồn tại.',12),
('B72-04','B7.2','SQL/Form',N'Đề án không tồn tại',N'NV01;XXX',N'Hàm hiện trả 0; Form phải phân biệt đề án không tồn tại.',13),
('B72-05','B7.2','SQL/Form',N'Tổng giờ phân công bằng 0 không được chia cho 0',N'NV05;DA03',N'SQL trả 0, không phát sinh lỗi chia cho 0.',14);
GO

/*========================== BÀI 7.3 (1 testcase) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B73-01','B7.3','SQL/Form',N'Tổng trung bình các phòng',N'',N'SQL trả 122,166.67; bỏ nhân viên MaPB=NULL và không cộng phòng rỗng.',15);
GO

/*========================== BÀI 7.4 (12 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B7-ADD-06','B7.4','BỔ SUNG',N'Giá trị không phải số',N'abc',N'Form báo sai định dạng; không được TRY_CONVERT thành NULL rồi hiển thị thưởng 0.',16),
('B7-ADD-07','B7.4','BỔ SUNG',N'Số thập phân tại biên',N'29.99; 30.00; 60.01; 99.99; 149.99',N'Lần lượt: 0; 500; 1,000; 1,000; 1,200.',17),
('B74-01','B7.4','SQL/Form',N'Dưới 30 giờ',N'29',N'Thưởng 0.',18),
('B74-02','B7.4','SQL/Form',N'Biên 30 giờ',N'30',N'Thưởng 500.',19),
('B74-03','B7.4','SQL/Form',N'Biên 60 giờ',N'60',N'Thưởng 500.',20),
('B74-04','B7.4','SQL/Form',N'Trên 60 giờ',N'61',N'Thưởng 1,000.',21),
('B74-05','B7.4','SQL/Form',N'Biên 99 giờ',N'99',N'Thưởng 1,000.',22),
('B74-06','B7.4','SQL/Form',N'Biên 100 giờ',N'100',N'Thưởng 1,200.',23),
('B74-07','B7.4','SQL/Form',N'Biên 149 giờ',N'149',N'Thưởng 1,200.',24),
('B74-08','B7.4','SQL/Form',N'Biên 150 giờ',N'150',N'Thưởng 1,600.',25),
('B74-09','B7.4','SQL/Form',N'Giờ âm',N'-1',N'Thưởng 0.',26),
('B74-10','B7.4','SQL/Form',N'NULL/rỗng',N'',N'SQL NULL cho hàm trả 0; Form rỗng phải được validation theo đặc tả UI.',27);
GO

/*========================== BÀI 7.5 (1 testcase) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B75-01','B7.5','SQL/Form',N'Mọi phòng kể cả 0 đề án',N'',N'Trả 5 phòng: PB01=2, PB02=1, PB03=1, PB05=1, PB00=0.',28);
GO

/*========================== BÀI 7.6 (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B7-ADD-08','B7.6','BỔ SUNG',N'Nhân viên không có phòng và không người thân',N'NV06',N'Vẫn xuất hiện; NguoiThan=NULL; TongLuongTB=0.',29),
('B76-03','B7.6','SQL/Form',N'Hai TVF phải giống nhau theo hai chiều EXCEPT',N'',N'Hai phép EXCEPT đều trả 0 dòng.',30);
GO

/*========================== BÀI 7.6A & 7.6B (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B76A-01','B7.6A','SQL/Form',N'Inline TVF',N'',N'Trả đủ 11 nhân viên; NV01 có 2 người thân, NV03/NV04 có 1, người khác NULL.',31),
('B76B-01','B7.6B','SQL/Form',N'Multistatement TVF',N'',N'Kết quả giống hoàn toàn B7.6A.',32);
GO

/*========================== BÀI 8.1 (4 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B8-ADD-01','B8.1','BỔ SUNG',N'Đề án có đúng 3 nhân viên',N'DA05',N'Phải xuất hiện với 3; đây là ca biên >2 bị bộ gốc bỏ sót ở expected.',101),
('B8-ADD-02','B8.1','BỔ SUNG',N'Không có phân công nào',N'Xóa PhanCong trong transaction',N'Trả 0 dòng, không lỗi.',102),
('B81-01','B8.1','SQL/Form',N'Đề án có hơn 2 nhân viên',N'DA01, DA03',N'Theo dữ liệu seed phải có DA01=5, DA03=5 và DA05=3.',103),
('B81-02','B8.1','SQL/Form',N'Đề án đúng 2 nhân viên phải bị loại',N'DA02',N'DA02 có đúng 2 nhân viên nên không xuất hiện.',104);
GO

/*========================== BÀI 8.2 (4 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B8-ADD-03','B8.2','BỔ SUNG',N'Phòng đúng 2 nhân viên',N'Dựng PBX có 2 NV',N'Không xuất hiện dù cả hai lương cao.',105),
('B8-ADD-04','B8.2','BỔ SUNG',N'Lương đúng 25,000',N'Dựng NV lương 25000',N'Không được tính vào cột >25000.',106),
('B82-01','B8.2','SQL/Form',N'Điều kiện hơn 2 người tính trên toàn phòng',N'PB01, PB02, PB05',N'Chỉ PB01, PB02, PB05 xuất hiện vì mỗi phòng có 3 nhân viên.',107),
('B82-02','B8.2','SQL/Form',N'Chỉ đếm nhân viên lương trên 25000',N'PB05 = 3',N'Cột đếm lương >25,000: PB01=3, PB02=3, PB05=3.',108);
GO

/*========================== BÀI 8.3 (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B8-ADD-05','B8.3','BỔ SUNG',N'Lương trung bình đúng 30,000',N'Dựng PBX avg=30000',N'Không xuất hiện vì điều kiện là >30000.',109),
('B83-01','B8.3','SQL/Form',N'Phòng có lương trung bình trên 30000',N'PB01, PB02',N'Trả đúng PB01 (35,666.67) và PB02 (36,333.33).',110);
GO

/*========================== BÀI 8.4 (2 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B8-ADD-06','B8.4','BỔ SUNG',N'Phòng đạt avg nhưng không có nam',N'Dựng PBX toàn Nữ, avg>30000',N'Xuất hiện với SoLuongNhanVienNam=0.',111),
('B84-01','B8.4','SQL/Form',N'Đếm đúng nhân viên nam trong phòng đạt điều kiện',N'PB01 = 2; PB02 = 1',N'PB01=2 nam; PB02=1 nam.',112);
GO

/*========================== BÀI 8.5 (4 testcases) ==========================*/
INSERT dbo.Testcase_Nhom3 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B8-ADD-07','B8.5','BỔ SUNG',N'Đề án không có phân công',N'DA04',N'Vẫn xuất hiện với 0.',113),
('B8-ADD-08','B8.5','BỔ SUNG',N'Đề án có phân công nhưng không có NV PB05',N'DA03',N'Vẫn xuất hiện với 0; bộ expected gốc bỏ sót.',114),
('B85-01','B8.5','SQL/Form',N'Mỗi đề án đều xuất hiện kể cả kết quả bằng 0',N'DA02 và DA04 = 0',N'Trả đủ DA01..DA05; giá trị 0 gồm DA02, DA03 và DA04.',115),
('B85-02','B8.5','SQL/Form',N'Đếm DISTINCT nhân viên phòng PB05 theo đề án',N'DA01 = 1; DA05 = 3',N'DA01=1 và DA05=3; đếm DISTINCT nhân viên PB05.',116);
GO

/*============================================================
  PROCEDURE LOAD TESTCASE BÀI 7 & BÀI 8
============================================================*/
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom3
    @MaBai VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong
    FROM dbo.Testcase_Nhom3
    WHERE (@MaBai IS NULL OR MaBai = @MaBai)
    ORDER BY ThuTu;
END
GO
