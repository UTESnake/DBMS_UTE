USE [QL_Gara];
GO

/*============================================================
  03_Testcase.sql - TESTCASE BÀI 9 (CSDL QUẢN LÝ GARA)
  - Đầy đủ 33 testcase Bài 9 (B9.RB, B9.1, B9.2, B9.3, B9.4, B9.5)
  - Có cột KyVong lưu kết quả mong đợi chuẩn xác theo BO_TESTCASE_CHI_TIET.md
============================================================*/

DROP TABLE IF EXISTS dbo.Testcase_Nhom4;
GO

CREATE TABLE dbo.Testcase_Nhom4
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

/*========================== BÀI 9.RB (19 testcases) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B9-ADD-07','B9.RB','BỔ SUNG',N'Lỗi không liên quan trigger mong đợi',N'Cố ý sai tên bảng/cột',N'Phải là ERROR, tuyệt đối không đánh dấu Đạt.',1),
('B9RB-01','B9.RB','SQL/Form',N'Nhóm trưởng phải thuộc cùng nhóm',N'Thử hợp lệ cùng nhóm và sai khác nhóm',N'Dòng cùng nhóm được chấp nhận; dòng khác nhóm bị đúng trigger tg_B9_KiemTraNhomTruong chặn; rollback.',2),
('B9RB-02','B9.RB','SQL/Form',N'Không đổi nhóm của trưởng nhóm khi còn thành viên',N'UPDATE T01 sang nhóm 2 phải bị chặn',N'Bị đúng trigger tg_B9_KiemTraDoiNhomTruong chặn; nhóm T01 không đổi.',3),
('B9RB-03','B9.RB','SQL/Form',N'Một xe không được có hai hợp đồng cùng ngày',N'INSERT trùng (NgayHD,SoXe) phải bị chặn',N'Bị unique (NgayHD,SoXe) chặn; không thêm hợp đồng.',4),
('B9RB-04','B9.RB','SQL/Form',N'Phiếu thu phải thuộc đúng khách của hợp đồng',N'INSERT SoHD=HD01,MaKH=KH02 phải bị chặn',N'Bị FK ghép (SoHD,MaKH) chặn; không thêm phiếu thu.',5),
('B9RB-05','B9.RB','SQL/Form',N'Cập nhật nhiều dòng cùng lúc vẫn giữ đúng nhóm trưởng',N'Đổi đồng thời trưởng nhóm và thành viên sang cùng nhóm được chấp nhận',N'Cập nhật đồng thời hợp lệ phải thành công trong transaction rồi rollback.',6),
('B9RB-06','B9.RB','SQL/Form',N'Cập nhật nhiều dòng không được lọt quan hệ khác nhóm',N'Đổi nhóm thành viên nhưng giữ trưởng nhóm cũ phải bị chặn',N'Cập nhật nhiều dòng còn quan hệ khác nhóm phải bị trigger chặn toàn bộ.',7),
('B9RB-07','B9.RB','SQL/Form',N'Một nhóm không có hai nhóm trưởng',N'Đổi T02 thành nhóm trưởng của chính mình trong nhóm 1',N'Bị tg_B9_KiemTraDoiNhomTruong chặn; nhóm 1 vẫn do T01 trưởng nhóm.',8),
('B9RB-08','B9.RB','SQL/Form',N'Đổi trưởng nhóm cho cả nhóm trong một lệnh',N'Đổi NhomTruong của T01,T02,T03 thành T02',N'Được phép vì mọi thợ nhóm 1 cùng chỉ T02; sau thử nghiệm rollback.',9),
('B9RB-09','B9.RB','SQL/Form',N'Không sửa trực tiếp trị giá hợp đồng sai tổng chi tiết',N'Tăng TriGiaHD của HD01 thêm 1',N'Bị tg_B9_KiemTraTriGiaHopDong chặn.',10),
('B9RB-10','B9.RB','SQL/Form',N'Sửa chi tiết tự đồng bộ tổng hợp đồng',N'Tăng TriGiaCV của HD02/CV01 thêm 100.000',N'TriGiaHD của HD02 đổi từ 3.300.000 thành 3.400.000; sau thử nghiệm rollback.',11),
('B9RB-11','B9.RB','SQL/Form',N'Khóa chính không cho trùng mã thợ',N'Thêm thợ có MaTho=T01',N'Bị PK_B9_THO chặn; không tính lỗi trigger nhóm trưởng là PASS.',12),
('B9RB-12','B9.RB','SQL/Form',N'Khóa ngoại chi tiết phải trỏ đến thợ có thật',N'Thêm chi tiết HD02/CV05 với MaTho=TNOEXIST',N'Bị FK_B9_CHITIET_THO chặn.',13),
('B9RB-13','B9.RB','SQL/Form',N'Số tiền thu phải dương',N'Thêm phiếu thu SoTienThu=-1 và SoTienThu=0',N'Cả hai bị CK_B9_PHIEUTHU_SoTienThu chặn.',14),
('B9RB-14','B9.RB','SQL/Form',N'Không lập phiếu thu trước ngày ký hợp đồng',N'Thêm/sửa phiếu thu trước NgayHD; đổi NgayHD sau phiếu thu đã lập',N'Hai trigger ngày phiếu thu chặn các thay đổi sai; dữ liệu được rollback.',15),
('B9RB-15','B9.RB','SQL/Form',N'Tổng tiền thu không vượt trị giá hợp đồng',N'Thêm/sửa/chuyển phiếu thu gây vượt; thử hai phiếu trong một lệnh; thử đúng bằng trị giá',N'Chặn mọi trường hợp vượt; chấp nhận tổng bằng trị giá.',16),
('B9RB-16','B9.RB','SQL/Form',N'Không giảm trị giá dưới tiền đã thu',N'Giảm TriGiaCV của HD01 để TriGiaHD còn 2.999.999, trong khi đã thu 3.000.000',N'Trigger trên HOPDONG chặn thay đổi và rollback.',17),
('B9RB-17','B9.RB','SQL/Form',N'Chuỗi không được rỗng và điện thoại đúng mẫu',N'Tên thợ toàn khoảng trắng; điện thoại có dấu + ở giữa; điện thoại +84902000001',N'Chặn hai giá trị sai, nhận số điện thoại quốc tế hợp lệ.',18),
('B9RB-18','B9.RB','SQL/Form',N'Không áp đặt ràng buộc ngoài đề',N'Trùng số điện thoại; trùng nội dung công việc; KhoanTho lớn hơn TriGiaCV',N'Các trường hợp này được phép vì không có ràng buộc tương ứng trong đề.',19);
GO

/*========================== BÀI 9.1 (1 testcase) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B91-01','B9.1','SQL/Form',N'Thợ chưa nhận công việc',N'T01, T04, T06',N'Trả đúng T01, T04, T06.',20);
GO

/*========================== BÀI 9.2 (4 testcases) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B9-ADD-01','B9.2','BỔ SUNG',N'Hợp đồng đã nghiệm thu chưa thu lần nào',N'Dựng HD NgayNgThu!=NULL, có chi tiết công việc, 0 phiếu thu',N'Xuất hiện; ĐãThanhToán=0; CònNợ=TrịGiáHD.',21),
('B9-ADD-02','B9.2','BỔ SUNG',N'Tổng phiếu thu lớn hơn trị giá',N'Dựng thu vượt trong transaction',N'Trigger chặn khoản thu vượt; hợp đồng và phiếu thu giữ nguyên.',22),
('B92-01','B9.2','SQL/Form',N'Hợp đồng đã nghiệm thu còn nợ',N'HD04, còn nợ 2.500.000',N'Chỉ HD04; ĐãThanhToán=2,000,000; CònNợ=2,500,000.',23),
('B92-02','B9.2','SQL/Form',N'Hợp đồng đủ tiền không xuất hiện',N'HD01 và HD03',N'HD01 và HD03 không xuất hiện; HD02 chưa nghiệm thu cũng không xuất hiện.',24);
GO

/*========================== BÀI 9.3 (3 testcases) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B9-ADD-03','B9.3','BỔ SUNG',N'Ngày giao đúng 31/12/2002',N'NgayGiaoDK=2002-12-31',N'Không xuất hiện vì điều kiện hiện là < 31/12/2002.',25),
('B9-ADD-04','B9.3','BỔ SUNG',N'Chưa nghiệm thu nhưng giao trước mốc',N'NgayNgThu=NULL; NgayGiaoDK<2002-12-31',N'Hàm hiện vẫn trả; nếu đề yêu cầu "chưa hoàn tất" thì cần xác nhận nghiệp vụ.',26),
('B93-01','B9.3','SQL/Form',N'Hợp đồng phải hoàn tất trước 31/12/2002',N'HD01 và HD04',N'Theo hàm hiện tại trả HD01, HD04. Lưu ý hàm không kiểm tra NgayNgThu IS NULL.',27);
GO

/*========================== BÀI 9.4 (3 testcases) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B9-ADD-05','B9.4','BỔ SUNG',N'Không có chi tiết hợp đồng',N'Xóa CHITIET_HD trong transaction',N'Mọi thợ đồng hạng 0 và đều được trả về.',28),
('B94-01','B9.4','SQL/Form',N'Thợ làm nhiều công việc nhất',N'T02, 4 công việc; xử lý đồng hạng',N'Chỉ T02, SoCongViec=4.',29),
('B94-02','B9.4','SQL/Form',N'Nhiều thợ đồng hạng cao nhất phải trả đủ',N'Tạo dữ liệu hòa trong transaction rồi rollback',N'Trả tất cả thợ đồng hạng nhất, không dùng TOP 1; rollback dữ liệu dựng thêm.',30);
GO

/*========================== BÀI 9.5 (3 testcases) ==========================*/
INSERT dbo.Testcase_Nhom4 (ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong, ThuTu) VALUES
('B9-ADD-06','B9.5','BỔ SUNG',N'Công việc trị giá 0',N'TriGiaCV=0',N'Tính tổng đúng 0; không bỏ thợ.',31),
('B95-01','B9.5','SQL/Form',N'Thợ có tổng trị giá cao nhất',N'T02, tổng 5.800.000; xử lý đồng hạng',N'Chỉ T02, TongTriGia=5,800,000.',32),
('B95-02','B9.5','SQL/Form',N'Nhiều thợ đồng tổng trị giá cao nhất phải trả đủ',N'Tạo dữ liệu hòa trong transaction rồi rollback',N'Trả tất cả thợ đồng tổng trị giá cao nhất; không dùng TOP 1; rollback dữ liệu dựng thêm.',33);
GO

/*============================================================
  PROCEDURE LOAD TESTCASE BÀI 9
============================================================*/
INSERT dbo.Testcase_Nhom4(ID_Testcase,MaBai,NhomLoi,MoTa,DuLieuNhap,KyVong,ThuTu) VALUES
('B9-EDGE-INSTALLMENTS','B9.RB','BỔ SUNG',N'Thanh toán ba đợt lẻ đủ trị giá',N'Hợp đồng 10 triệu; phiếu 3+4+3 triệu; thử thêm 1 đồng',N'Ba phiếu đầu được nhận; phiếu vượt bị trigger chặn theo tổng lũy kế.',201),
('B9-EDGE-REPEAT-CAR','B9.RB','BỔ SUNG',N'Một xe quay lại sửa ngày khác',N'Cùng SoXe, NgayHD=2002-12-01 và 2002-12-15',N'Hai hợp đồng khác ngày hợp lệ; cùng ngày bị unique chặn.',202);
GO

CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcase_Nhom4
    @MaBai VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase, MaBai, NhomLoi, MoTa, DuLieuNhap, KyVong
    FROM dbo.Testcase_Nhom4
    WHERE (@MaBai IS NULL OR MaBai = @MaBai)
    ORDER BY ThuTu;
END
GO
