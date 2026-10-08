USE [QL_DeAn];
GO

-- Danh mục và kiểm tra testcase Bài 7, Bài 8, bảng lương theo dữ liệu trong 01_TaoBang_NhapDuLieu.sql.
-- Function được cài riêng tại DATABASE/Bai_07 và DATABASE/Bai_08.
DROP TABLE IF EXISTS dbo.Testcase_Nhom3;
GO

CREATE TABLE dbo.Testcase_Nhom3
(
    ID_Testcase varchar(30) NOT NULL PRIMARY KEY,
    MaBai varchar(10) NOT NULL,
    NhomLoi varchar(20) NOT NULL,
    MoTa nvarchar(500) NOT NULL,
    DuLieuNhap nvarchar(500) NULL,
    KyVong nvarchar(500) NULL,
    ThuTu int NOT NULL
);
GO

INSERT dbo.Testcase_Nhom3(ID_Testcase,MaBai,NhomLoi,MoTa,DuLieuNhap,KyVong,ThuTu) VALUES
('B7-ADD-01','B7.1','BỔ SUNG',N'Mã phòng có khoảng trắng',N'  01  ',N'Form bỏ khoảng trắng; kết quả 35.666,67.',1),
('B7-ADD-02','B7.1','BỔ SUNG',N'Mã phòng NULL',N'NULL',N'Hàm trả 0; Form yêu cầu nhập mã.',2),
('B7-ADD-03','B7.1','BỔ SUNG',N'Phòng có lương 0 và NULL',N'03: 24.000, NULL, 0, 25.000',N'AVG bỏ NULL, vẫn tính 0; kết quả 16.333,33.',3),
('B71-01','B7.1','SQL/Form',N'Phòng hợp lệ',N'01',N'Lương trung bình 35.666,67.',4),
('B71-02','B7.1','SQL/Form',N'Phòng không tồn tại',N'99',N'Hàm trả 0; Form báo mã phòng không tồn tại.',5),
('B71-03','B7.1','SQL/Form',N'Phòng không có nhân viên',N'00',N'Hàm trả 0; Form báo phòng chưa có nhân viên.',6),
('B71-04','B7.1','SQL/Form',N'Mã phòng rỗng',N'',N'Form chặn trước khi gọi SQL.',7),
('B7-ADD-09','B7.1','BỔ SUNG',N'Lương trung bình đúng 30.000',N'04',N'Kết quả 30.000.',33),

('B7-ADD-04','B7.2','BỔ SUNG',N'Mã nhân viên hoặc đề án rỗng',N';01 và NV01;',N'Form chặn trường rỗng.',8),
('B7-ADD-05','B7.2','BỔ SUNG',N'Khoảng trắng quanh hai mã',N' NV01 ; 01 ',N'Form bỏ khoảng trắng; kết quả 15.737,70.',9),
('B72-01','B7.2','SQL/Form',N'Có tham gia đề án',N'NV01;01',N'32.000 × 30 / 61 = 15.737,70.',10),
('B72-02','B7.2','SQL/Form',N'Không tham gia đề án',N'NV01;03',N'Hàm trả 0; Form báo không tham gia.',11),
('B72-03','B7.2','SQL/Form',N'Nhân viên không tồn tại',N'XXX;01',N'Hàm trả 0; Form báo nhân viên không tồn tại.',12),
('B72-04','B7.2','SQL/Form',N'Đề án không tồn tại',N'NV01;99',N'Hàm trả 0; Form báo đề án không tồn tại.',13),
('B72-05','B7.2','SQL/Form',N'Tổng giờ phân công bằng 0',N'NV12;06',N'Hàm trả 0, không chia cho 0.',14),
('B7-ADD-10','B7.2','BỔ SUNG',N'Nhân viên tham gia nhưng lương NULL',N'NV17;06',N'Hàm trả 0, không lỗi.',34),

('B73-01','B7.3','SQL/Form',N'Tổng các mức lương trung bình theo phòng',N'',N'177.500; bỏ phòng rỗng, lương NULL và nhân viên Phg=NULL.',15),

('B7-ADD-06','B7.4','BỔ SUNG',N'Giá trị không phải số',N'abc',N'Form báo sai định dạng.',16),
('B7-ADD-07','B7.4','BỔ SUNG',N'Số thập phân tại biên',N'29.99;30;60.01;99.99;149.99',N'Lần lượt 0;500;1.000;1.000;1.200.',17),
('B74-01','B7.4','SQL/Form',N'Dưới 30 giờ',N'29',N'Thưởng 0.',18),
('B74-02','B7.4','SQL/Form',N'Đúng 30 giờ',N'30',N'Thưởng 500.',19),
('B74-03','B7.4','SQL/Form',N'Đúng 60 giờ',N'60',N'Thưởng 500.',20),
('B74-04','B7.4','SQL/Form',N'Trên 60 giờ',N'61',N'Thưởng 1.000.',21),
('B74-05','B7.4','SQL/Form',N'Dưới 100 giờ',N'99',N'Thưởng 1.000.',22),
('B74-06','B7.4','SQL/Form',N'Đúng 100 giờ',N'100',N'Thưởng 1.200.',23),
('B74-07','B7.4','SQL/Form',N'Dưới 150 giờ',N'149',N'Thưởng 1.200.',24),
('B74-08','B7.4','SQL/Form',N'Đúng 150 giờ',N'150',N'Thưởng 1.600.',25),
('B74-09','B7.4','SQL/Form',N'Giờ âm',N'-1',N'Hàm trả 0.',26),
('B74-10','B7.4','SQL/Form',N'NULL hoặc ô nhập rỗng',N'',N'Hàm nhận NULL trả 0; Form chặn ô rỗng.',27),
('B7.4-20_00','B7.4','BỔ SUNG',N'Dưới ngưỡng thưởng trong ví dụ ZIP',N'20',N'Thưởng 0.',36),
('B7.4-60_01','B7.4','BỔ SUNG',N'Sát trên 60 giờ',N'60.01',N'Thưởng 1.000.',37),
('B7.4-70_00','B7.4','BỔ SUNG',N'Giữa khoảng 60 và 100 giờ',N'70',N'Thưởng 1.000.',38),
('B7.4-99_99','B7.4','BỔ SUNG',N'Sát dưới 100 giờ',N'99.99',N'Thưởng 1.000.',39),
('B7.4-120_00','B7.4','BỔ SUNG',N'Giữa khoảng 100 và 150 giờ',N'120',N'Thưởng 1.200.',40),
('B7.4-149_99','B7.4','BỔ SUNG',N'Sát dưới 150 giờ',N'149.99',N'Thưởng 1.200.',41),

('B75-01','B7.5','SQL/Form',N'Mọi phòng, kể cả phòng không có đề án',N'',N'7 phòng: 00=0, 01=2, 02=1, 03=1, 04=1, 05=1, 06=0.',28),

('B7-ADD-08','B7.6','BỔ SUNG',N'Nhân viên không có phòng và người thân',N'NV06',N'Vẫn xuất hiện; NguoiThan=NULL, TongLuongTB=0.',29),
('B76-03','B7.6','SQL/Form',N'Hai TVF cho kết quả như nhau',N'',N'EXCEPT theo cả hai chiều đều trả 0 dòng.',30),
('B76A-01','B7.6A','SQL/Form',N'Inline TVF',N'',N'Đủ 19 nhân viên; NV01 có 2 người thân, NV03/NV04 có 1.',31),
('B76B-01','B7.6B','SQL/Form',N'Multistatement TVF',N'',N'Kết quả giống hoàn toàn Inline TVF.',32),
('B7-ADD-11','B7.6','BỔ SUNG',N'Họ tên có họ, tên lót và tên',N'NV19',N'HoTen=Ninh Thị Yến trong cả hai TVF.',35),
('B7.6-EQUIVALENT','B7.6','BỔ SUNG',N'So sánh hai TVF theo đủ năm cột và cả hai chiều',N'',N'Không có dòng khác biệt theo cả hai chiều; mỗi TVF có 19 dòng.',42),

('B8-ADD-01','B8.1','BỔ SUNG',N'Đề án có đúng 3 nhân viên',N'05',N'Xuất hiện với số lượng 3.',101),
('B8-ADD-02','B8.1','BỔ SUNG',N'Không có phân công nào',N'Xóa PHANCONG trong transaction',N'Trả 0 dòng, không lỗi; rollback sau kiểm tra.',102),
('B81-01','B8.1','SQL/Form',N'Đề án có hơn 2 nhân viên',N'01;03;05',N'01=5, 03=5, 05=3.',103),
('B81-02','B8.1','SQL/Form',N'Đề án đúng 2 nhân viên',N'02;06',N'Cả hai không xuất hiện.',104),
('B8.1-EXACT','B8.1','BỔ SUNG',N'Tập kết quả đầy đủ và số người theo đề án',N'',N'01:5;03:5;05:3.',118),

('B8-ADD-03','B8.2','BỔ SUNG',N'Phòng đúng 2 nhân viên',N'04',N'Không xuất hiện dù có lương trên 25.000.',105),
('B8-ADD-04','B8.2','BỔ SUNG',N'Lương đúng 25.000',N'03 có NV19',N'Không tính NV19 vào cột lương >25.000.',106),
('B82-01','B8.2','SQL/Form',N'Điều kiện hơn 2 người tính trên toàn phòng',N'01;02;03;05;06',N'Trả đủ 5 phòng, kể cả 03 có lương NULL.',107),
('B82-02','B8.2','SQL/Form',N'Đếm nhân viên có lương trên 25.000',N'01;02;03;05;06',N'Kết quả lần lượt 3;3;0;3;3.',108),
('B8.2-EXACT','B8.2','BỔ SUNG',N'Tập phòng và số người lương trên 25.000',N'',N'01:3;02:3;03:0;05:3;06:3.',119),

('B8-ADD-05','B8.3','BỔ SUNG',N'Lương trung bình đúng 30.000',N'04',N'Không xuất hiện vì điều kiện >30.000.',109),
('B83-01','B8.3','SQL/Form',N'Phòng có lương trung bình trên 30.000',N'01;02;06',N'Trả 01, 02, 06 với số nhân viên đều bằng 3.',110),
('B8.3-EXACT','B8.3','BỔ SUNG',N'Tập phòng và tổng số nhân viên',N'',N'01:3;02:3;06:3.',120),

('B8-ADD-06','B8.4','BỔ SUNG',N'Phòng đạt trung bình nhưng không có nam',N'06',N'Xuất hiện với SoLuongNhanVienNam=0.',111),
('B84-01','B8.4','SQL/Form',N'Đếm nam trong các phòng đạt điều kiện',N'01;02',N'01=2 nam, 02=1 nam.',112),
('B8.4-EXACT','B8.4','BỔ SUNG',N'Tập phòng và số nhân viên nam',N'',N'01:2;02:1;06:0.',121),

('B8-ADD-07','B8.5','BỔ SUNG',N'Đề án không có phân công',N'04',N'Vẫn xuất hiện với số lượng 0.',113),
('B8-ADD-08','B8.5','BỔ SUNG',N'Có phân công nhưng không có nhân viên phòng 05',N'03',N'Vẫn xuất hiện với số lượng 0.',114),
('B85-01','B8.5','SQL/Form',N'Mọi đề án đều xuất hiện',N'01..06',N'Đủ 6 đề án; 02, 03, 04 và 06 có số lượng 0.',115),
('B85-02','B8.5','SQL/Form',N'Đếm nhân viên phòng 05 theo đề án',N'01;05',N'01=1, 05=3.',116),
('B8-ADD-09','B8.5','BỔ SUNG',N'Có phân công từ phòng khác',N'06 có NV12 và NV17',N'Đề án 06 xuất hiện với số lượng 0.',117),
('B8.5-EXACT','B8.5','BỔ SUNG',N'Mọi đề án và số người phòng 05',N'',N'01:1;02:0;03:0;04:0;05:3;06:0.',122);
GO

INSERT dbo.Testcase_Nhom3(ID_Testcase,MaBai,NhomLoi,MoTa,DuLieuNhap,KyVong,ThuTu) VALUES
('B7-EDGE-CROSS-SALARY','B7.1','BỔ SUNG',N'Nhân viên làm đề án chéo phòng',N'NV09 thuộc phòng 05, làm đề án 01 của phòng 01',N'Lương trung bình phòng 05 tính NV09; phòng 01 không cộng NV09.',201),
('B7-EDGE-CROSS-PROJECT','B7.5','BỔ SUNG',N'Đếm đề án theo phòng chủ trì',N'NV09 phòng 05 tham gia đề án 01 thuộc phòng 01',N'Đề án 01 chỉ tính cho phòng 01; phòng 05 không được cộng thêm.',202),
('B7-EDGE-NEG-HOUR','B7.4','BỔ SUNG',N'Giờ công âm bị từ chối khi lưu',N'PHANCONG.ThoiGian=-1; tham số fn_B7_TienThuong=-1',N'CHECK chặn -1 trong PHANCONG; hàm scalar trả thưởng 0.',203),
('B8-EDGE-GENDER','B8.4','BỔ SUNG',N'Giới tính nam viết hoa, thường hoặc NULL',N'UPDATE Phai thành nam, NAM, NULL trong transaction',N'nam/NAM tính là nam, NULL tính 0; rollback sau thử.',204),
('B8-EDGE-LEADER','B8.5','BỔ SUNG',N'Đề án chưa có trưởng đề án',N'DEAN.Truong_DA=NULL',N'Đề án vẫn hiển thị; thống kê nhân viên phòng 05 giữ nguyên.',205);
GO

CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom3 @MaBai varchar(10)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase,MaBai,NhomLoi,MoTa,DuLieuNhap,KyVong
    FROM dbo.Testcase_Nhom3
    WHERE @MaBai IS NULL OR MaBai=@MaBai
    ORDER BY ThuTu;
END;
GO

-- Kiểm tra bổ sung Bài 7 và Bài 8 trên dữ liệu mẫu.
SELECT dbo.fn_B7_LuongTrungBinhPhong('05') AS LuongPhongNhanVienNV09,
       dbo.fn_B7_TienThuong(-1) AS ThuongGioAm;
SELECT * FROM dbo.fn_B7_SoDeAnTheoPhong() WHERE MaPhg IN ('01','05');
SELECT * FROM dbo.fn_B8_PhongLuongTBLon_Nam() WHERE MaPhg IN ('01','06');
SELECT * FROM dbo.fn_B8_DeAnCoNhanVienPhong5() WHERE MaDA='04';
SELECT MaDA,TenDA,Truong_DA FROM dbo.DEAN WHERE MaDA IN ('01','04');
IF NOT EXISTS (SELECT 1 FROM dbo.PHANCONG WHERE MaNV='NV09' AND SoDA='01')
    THROW 51001,N'Thiếu phân công chéo phòng NV09/đề án 01.',1;
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_PHANCONG_THOIGIAN' AND is_disabled=0)
    THROW 51002,N'Thiếu CHECK giờ công không âm.',1;
GO

-- Kiểm tra bảng lương; các thay đổi dữ liệu thử đều ROLLBACK.
BEGIN TRY
    BEGIN TRANSACTION;

    EXEC dbo.sp_TinhVaCapNhatBangLuong @NgayTinhLuong = '2026-10-31';

    IF (SELECT COUNT(*) FROM dbo.BANGLUONG) <> (SELECT COUNT(*) FROM dbo.NHANVIEN)
        THROW 51010, N'Bảng lương phải có đúng một dòng cho mỗi nhân viên.', 1;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.BANGLUONG
        WHERE MaNV='NV01' AND LuongCoBan=32000 AND Time_Total=61
          AND TienThuong=1000 AND LuongTBPhong=35666.67 AND TongThuNhap=33000
          AND NgayTinhLuong='2026-10-31' AND NgayNhanLuong='2026-11-30'
          AND NguoiThan LIKE N'%Hoàng Lan%' AND NguoiThan LIKE N'%Nguyễn Minh%'
    ) THROW 51011, N'Sai lương, thưởng, thân nhân hoặc ngày nhận lương NV01.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                   WHERE MaNV='NV04' AND Time_Total=299 AND TienThuong=1600
                     AND TongThuNhap=51600)
        THROW 51012, N'Sai thưởng từ 150 giờ trở lên.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                   WHERE MaNV='NV05' AND Time_Total=0 AND TienThuong=0
                     AND NguoiThan=N'(Không có)')
        THROW 51013, N'Sai ca phân công 0 giờ hoặc không có người thân.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                   WHERE MaNV='NV06' AND Time_Total=0 AND LuongTBPhong=0)
        THROW 51014, N'Sai ca nhân viên không thuộc phòng/không có phân công.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                   WHERE MaNV='NV17' AND LuongCoBan IS NULL AND TongThuNhap=0)
       OR NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                      WHERE MaNV='NV18' AND LuongCoBan=0 AND TongThuNhap=0)
        THROW 51015, N'Sai ca lương NULL hoặc lương 0.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.v_LuongNhanVienTheoDeAn
                   WHERE MaNV='NV01' AND MaDA='01' AND SoGioThamGia=30
                     AND TongGioNhanVien=61 AND LuongTheoDeAn=15737.70)
       OR NOT EXISTS (SELECT 1 FROM dbo.v_LuongNhanVienTheoDeAn
                      WHERE MaNV='NV12' AND MaDA='06' AND LuongTheoDeAn=0)
       OR NOT EXISTS (SELECT 1 FROM dbo.v_LuongNhanVienTheoDeAn
                      WHERE MaNV='NV17' AND MaDA='06' AND LuongTheoDeAn=0)
        THROW 51018, N'Sai phân bổ lương theo đề án hoặc ca giờ/lương bằng 0.', 1;

    -- Thay dữ liệu nguồn rồi làm mới: lương, giờ, lương TB phòng và thân nhân
    -- phải cùng đổi; gọi lại lần hai không tạo bản ghi trùng.
    UPDATE dbo.NHANVIEN SET Luong=33000 WHERE MaNV='NV01';
    UPDATE dbo.PHANCONG SET ThoiGian=29 WHERE MaNV='NV01' AND SoDA='02';
    INSERT dbo.THANNHAN(MaNV,TenTN) VALUES ('NV01',N'Thử mới');
    EXEC dbo.sp_TinhVaCapNhatBangLuong @NgayTinhLuong = '2026-10-31';
    EXEC dbo.sp_TinhVaCapNhatBangLuong @NgayTinhLuong = '2026-10-31';

    IF NOT EXISTS (SELECT 1 FROM dbo.BANGLUONG
                   WHERE MaNV='NV01' AND LuongCoBan=33000 AND Time_Total=59
                     AND TienThuong=500 AND LuongTBPhong=36000
                     AND TongThuNhap=33500 AND NguoiThan LIKE N'%Thử mới%')
       OR (SELECT COUNT(*) FROM dbo.BANGLUONG WHERE MaNV='NV01') <> 1
        THROW 51016, N'Làm mới bảng lương sau thay đổi dữ liệu nguồn không đúng.', 1;

    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

BEGIN TRY
    BEGIN TRANSACTION;
    DELETE dbo.PHANCONG;
    EXEC dbo.sp_TinhVaCapNhatBangLuong @NgayTinhLuong = '2026-10-31';

    IF EXISTS (SELECT 1 FROM dbo.BANGLUONG WHERE Time_Total<>0 OR TienThuong<>0)
       OR EXISTS (SELECT 1 FROM dbo.fn_B8_DeAnNhieuNhanVien())
        THROW 51017, N'Khi không còn phân công, giờ/thưởng phải bằng 0 và Bài 8.1 rỗng.', 1;

    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
