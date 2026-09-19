USE [QL_ThuVien]
GO

/*============================================================
  02_Testcase.sql - TESTCASE CHUNG BÀI 5 + BÀI 6
  - CHỈ LƯU/LOAD TESTCASE, KHÔNG LƯU Expected/Actual/PASS-FAIL.
  - Phân loại: OK / WINFORM / SQL / NGHIEPVU / BIEN / DULIEU.
  - Bài 5 được mở rộng theo các lớp tương đương và giá trị biên
    để bao phủ các trường hợp có ý nghĩa kiểm thử thực tế.
============================================================*/

IF OBJECT_ID(N'dbo.Testcase_Nhom2', N'U') IS NOT NULL
    DROP TABLE dbo.Testcase_Nhom2;
GO

CREATE TABLE dbo.Testcase_Nhom2
(
    ID_Testcase VARCHAR(30) NOT NULL PRIMARY KEY,
    MaBai       VARCHAR(10) NOT NULL,
    NhomLoi     VARCHAR(20) NOT NULL,
    ChucNang    NVARCHAR(100) NOT NULL,
    MoTa        NVARCHAR(500) NOT NULL,
    DuLieuNhap  NVARCHAR(1000) NULL,
    GhiChu      NVARCHAR(500) NULL
);
GO

/*========================== BÀI 5A ==========================*/
-- TEST ĐÚNG / CHẠY BÌNH THƯỜNG
INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5A-OK-01','B5A','OK',N'sp_ThongtinDocGia',N'Mã độc giả người lớn hợp lệ.',N'ma_DocGia=<mã người lớn hợp lệ>',N'Procedure phải trả thông tin DocGia + Nguoilon.'),
('B5A-OK-02','B5A','OK',N'sp_ThongtinDocGia',N'Mã độc giả trẻ em hợp lệ.',N'ma_DocGia=<mã trẻ em hợp lệ>',N'Procedure phải trả thông tin DocGia + Treem.'),
('B5A-OK-03','B5A','OK',N'sp_ThongtinDocGia',N'Mã hợp lệ ở đầu danh sách dữ liệu.',N'ma_DocGia=<mã hợp lệ đầu tiên>',N'Kiểm tra tra cứu bình thường.'),
('B5A-OK-04','B5A','OK',N'sp_ThongtinDocGia',N'Mã hợp lệ có dữ liệu họ tên/ngày sinh đầy đủ.',N'ma_DocGia=<mã hợp lệ đầy đủ thông tin>',N'Kiểm tra hiển thị đủ cột.');
GO

INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5A-WF-01','B5A','WINFORM',N'sp_ThongtinDocGia',N'Không nhập mã độc giả.',N'ma_DocGia=',N'Phải báo thiếu dữ liệu.'),
('B5A-WF-02','B5A','WINFORM',N'sp_ThongtinDocGia',N'Chỉ nhập khoảng trắng.',N'ma_DocGia=     ',N'Trim xong phải xem là rỗng.'),
('B5A-WF-03','B5A','WINFORM',N'sp_ThongtinDocGia',N'Mã có khoảng trắng đầu/cuối.',N'ma_DocGia=  <mã hợp lệ>  ',N'Phải Trim trước khi gọi SQL.'),
('B5A-WF-04','B5A','WINFORM',N'sp_ThongtinDocGia',N'Nhập chuỗi rất dài.',N'ma_DocGia=DG_ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789',N'Kiểm tra MaxLength/độ dài tham số.'),
('B5A-WF-05','B5A','WINFORM',N'sp_ThongtinDocGia',N'Nhập ký tự đặc biệt.',N'ma_DocGia=@#$%^&*',N'Không được làm ứng dụng lỗi.'),
('B5A-WF-06','B5A','WINFORM',N'sp_ThongtinDocGia',N'Nhập chuỗi giống SQL injection.',N'ma_DocGia=''; DROP TABLE DocGia;--',N'Phải dùng parameter, không nối chuỗi SQL.'),
('B5A-WF-07','B5A','WINFORM',N'sp_ThongtinDocGia',N'Nhấn Tra cứu nhiều lần liên tiếp.',N'Action=CLICK_MULTI',N'Không treo/không nhân dữ liệu.'),
('B5A-WF-08','B5A','WINFORM',N'sp_ThongtinDocGia',N'Nhấn Làm mới sau khi có kết quả.',N'Action=RESET',N'Phải xóa input và kết quả.'),
('B5A-WF-09','B5A','WINFORM',N'sp_ThongtinDocGia',N'Mở chức năng khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'Form chính phải chặn.'),
('B5A-SQL-01','B5A','SQL',N'sp_ThongtinDocGia',N'Truyền NULL trực tiếp.',N'ma_DocGia=NULL',N'Kiểm tra xử lý NULL trong procedure.'),
('B5A-SQL-02','B5A','SQL',N'sp_ThongtinDocGia',N'Mã không tồn tại.',N'ma_DocGia=DG_KHONG_TON_TAI',N'Không trả nhầm dữ liệu.'),
('B5A-SQL-03','B5A','SQL',N'sp_ThongtinDocGia',N'Chuỗi rỗng truyền xuống SQL.',N'ma_DocGia=',N'Procedure phải xử lý ổn định.'),
('B5A-DL-01','B5A','DULIEU',N'sp_ThongtinDocGia',N'DocGia tồn tại nhưng không thuộc Nguoilon hoặc Treem.',N'ma_DocGia=<DocGia chưa phân loại>',N'Dữ liệu bất thường nếu ràng buộc cho phép.'),
('B5A-DL-02','B5A','DULIEU',N'sp_ThongtinDocGia',N'Cùng mã xuất hiện ở cả Nguoilon và Treem.',N'ma_DocGia=<mã phân loại trùng>',N'Kiểm tra dữ liệu sai cấu trúc nếu FK/check không ngăn.'),
('B5A-DL-03','B5A','DULIEU',N'sp_ThongtinDocGia',N'Dòng Nguoilon tồn tại nhưng thiếu DocGia cha.',N'ma_DocGia=<orphan Nguoilon>',N'Kiểm tra JOIN khi FK bị tắt.'),
('B5A-DL-04','B5A','DULIEU',N'sp_ThongtinDocGia',N'Dòng Treem tồn tại nhưng thiếu DocGia cha.',N'ma_DocGia=<orphan Treem>',N'Kiểm tra JOIN khi FK bị tắt.'),
('B5A-NV-01','B5A','NGHIEPVU',N'sp_ThongtinDocGia',N'Độc giả người lớn hợp lệ.',N'ma_DocGia=<mã người lớn hợp lệ>',N'Phải có DocGia + Nguoilon.'),
('B5A-NV-02','B5A','NGHIEPVU',N'sp_ThongtinDocGia',N'Độc giả trẻ em hợp lệ.',N'ma_DocGia=<mã trẻ em hợp lệ>',N'Phải có DocGia + Treem.'),
('B5A-NV-03','B5A','NGHIEPVU',N'sp_ThongtinDocGia',N'Họ/tên có Unicode tiếng Việt.',N'ma_DocGia=<mã hợp lệ>',N'Không lỗi Unicode khi hiển thị.'),
('B5A-NV-04','B5A','NGHIEPVU',N'sp_ThongtinDocGia',N'Một số trường thông tin phụ NULL.',N'ma_DocGia=<mã hợp lệ>',N'Không được lỗi khi cột không bắt buộc NULL.');
GO

/*========================== BÀI 5B ==========================*/
-- TEST ĐÚNG / CHẠY BÌNH THƯỜNG
INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5B-OK-01','B5B','OK',N'sp_ThongtinDausach',N'ISBN hợp lệ còn đúng 1 cuốn có sẵn.',N'isbn=<ISBN còn 1 cuốn>',N'Phải trả thông tin đầu sách và số lượng chưa mượn = 1.'),
('B5B-OK-02','B5B','OK',N'sp_ThongtinDausach',N'ISBN hợp lệ còn từ 2 cuốn có sẵn trở lên.',N'isbn=<ISBN còn >=2 cuốn>',N'Phải đếm đúng số lượng.'),
('B5B-OK-03','B5B','OK',N'sp_ThongtinDausach',N'ISBN hợp lệ nhưng hiện không còn cuốn có sẵn.',N'isbn=<ISBN còn 0 cuốn>',N'Procedure vẫn chạy bình thường và số lượng = 0.'),
('B5B-OK-04','B5B','OK',N'sp_ThongtinDausach',N'ISBN hợp lệ có nhiều Cuonsach với trạng thái hỗn hợp.',N'isbn=<ISBN có cả cuốn đang mượn và có sẵn>',N'Chỉ đếm cuốn chưa mượn.'),
('B5B-OK-05','B5B','OK',N'sp_ThongtinDausach',N'ISBN hợp lệ và liên kết Tuasach đầy đủ.',N'isbn=<ISBN hợp lệ>',N'Phải trả được thông tin Dausach + Tuasach.');
GO

INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5B-WF-01','B5B','WINFORM',N'sp_ThongtinDausach',N'Không nhập ISBN.',N'isbn=',N'Phải báo thiếu dữ liệu.'),
('B5B-WF-02','B5B','WINFORM',N'sp_ThongtinDausach',N'ISBN chỉ khoảng trắng.',N'isbn=    ',N'Trim thành rỗng.'),
('B5B-WF-03','B5B','WINFORM',N'sp_ThongtinDausach',N'ISBN có khoảng trắng đầu/cuối.',N'isbn=  <ISBN hợp lệ>  ',N'Phải Trim.'),
('B5B-WF-04','B5B','WINFORM',N'sp_ThongtinDausach',N'ISBN quá dài.',N'isbn=ISBN_ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789',N'Kiểm tra giới hạn.'),
('B5B-WF-05','B5B','WINFORM',N'sp_ThongtinDausach',N'ISBN ký tự đặc biệt.',N'isbn=@#$%^&*',N'Không làm ứng dụng lỗi.'),
('B5B-WF-06','B5B','WINFORM',N'sp_ThongtinDausach',N'Chuỗi giống SQL injection.',N'isbn=''; DROP TABLE Dausach;--',N'Phải parameter hóa.'),
('B5B-WF-07','B5B','WINFORM',N'sp_ThongtinDausach',N'Nhấn Tra cứu nhiều lần.',N'Action=CLICK_MULTI',N'Không nhân dòng giả.'),
('B5B-WF-08','B5B','WINFORM',N'sp_ThongtinDausach',N'Nhấn Làm mới.',N'Action=RESET',N'Xóa input/kết quả.'),
('B5B-WF-09','B5B','WINFORM',N'sp_ThongtinDausach',N'Chưa kết nối CSDL.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),
('B5B-SQL-01','B5B','SQL',N'sp_ThongtinDausach',N'Truyền NULL.',N'isbn=NULL',N'Kiểm tra NULL ở SQL.'),
('B5B-SQL-02','B5B','SQL',N'sp_ThongtinDausach',N'ISBN không tồn tại.',N'isbn=ISBN_KHONG_TON_TAI',N'Không trả nhầm dữ liệu.'),
('B5B-SQL-03','B5B','SQL',N'sp_ThongtinDausach',N'Chuỗi rỗng xuống SQL.',N'isbn=',N'Procedure phải ổn định.'),
('B5B-DL-01','B5B','DULIEU',N'sp_ThongtinDausach',N'Dausach tham chiếu Tuasach không tồn tại.',N'isbn=<Dausach orphan>',N'Kiểm tra JOIN khi FK bị tắt.'),
('B5B-DL-02','B5B','DULIEU',N'sp_ThongtinDausach',N'Cuonsach tham chiếu ISBN không tồn tại.',N'isbn=<Cuonsach orphan>',N'Kiểm tra dữ liệu lỗi nếu FK bị tắt.'),
('B5B-DL-03','B5B','DULIEU',N'sp_ThongtinDausach',N'Cuonsach.tinhtrang NULL.',N'isbn=<ISBN có tinhtrang NULL>',N'Không được đếm nhầm là có sẵn.'),
('B5B-DL-04','B5B','DULIEU',N'sp_ThongtinDausach',N'Cuonsach.tinhtrang ngoài miền quy định.',N'isbn=<ISBN có trạng thái lạ>',N'Kiểm tra dữ liệu bất thường.'),
('B5B-NV-01','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'ISBN hợp lệ còn 0 cuốn chưa mượn.',N'isbn=<ISBN còn 0 cuốn>',N'COUNT phải bằng 0.'),
('B5B-NV-02','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'ISBN hợp lệ còn đúng 1 cuốn chưa mượn.',N'isbn=<ISBN còn 1 cuốn>',N'COUNT = 1.'),
('B5B-NV-03','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'ISBN hợp lệ còn nhiều cuốn chưa mượn.',N'isbn=<ISBN còn nhiều cuốn>',N'COUNT >= 2.'),
('B5B-NV-04','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'Dausach tồn tại nhưng chưa có Cuonsach.',N'isbn=<ISBN hợp lệ>',N'Số lượng chưa mượn phải 0.'),
('B5B-NV-05','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'Tất cả cuốn đều có sẵn.',N'isbn=<ISBN hợp lệ>',N'COUNT bằng tổng số cuốn.'),
('B5B-NV-06','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'Hỗn hợp cuốn có sẵn và đang mượn.',N'isbn=<ISBN hợp lệ>',N'Chỉ đếm cuốn chưa mượn.'),
('B5B-NV-07','B5B','NGHIEPVU',N'sp_ThongtinDausach',N'Tựa sách/tác giả Unicode.',N'isbn=<ISBN hợp lệ>',N'Hiển thị Unicode đúng.');
GO

/*========================== BÀI 5C ==========================*/
-- TEST ĐÚNG / CHẠY BÌNH THƯỜNG
INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5C-OK-01','B5C','OK',N'sp_ThongtinNguoilonDangmuon',N'Có 1 người lớn đang mượn 1 cuốn.',N'TrangThaiDuLieu=1 người lớn,1 dòng Muon',N'Procedure trả đúng độc giả đang mượn.'),
('B5C-OK-02','B5C','OK',N'sp_ThongtinNguoilonDangmuon',N'Có 1 người lớn đang mượn nhiều cuốn.',N'TrangThaiDuLieu=1 người lớn,nhiều dòng Muon',N'Phải hiển thị đủ các lượt/cuốn theo thiết kế.'),
('B5C-OK-03','B5C','OK',N'sp_ThongtinNguoilonDangmuon',N'Có nhiều người lớn cùng đang mượn.',N'TrangThaiDuLieu=nhiều người lớn đang mượn',N'Phải trả đủ danh sách.'),
('B5C-OK-04','B5C','OK',N'sp_ThongtinNguoilonDangmuon',N'Có cả người lớn và trẻ em đang mượn.',N'TrangThaiDuLieu=NL+TE cùng có trong Muon',N'Chỉ liệt kê người lớn.');
GO

INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5C-WF-01','B5C','WINFORM',N'sp_ThongtinNguoilonDangmuon',N'Nhấn tải dữ liệu khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'Form chính phải chặn.'),
('B5C-WF-02','B5C','WINFORM',N'sp_ThongtinNguoilonDangmuon',N'Nhấn tải nhiều lần liên tiếp.',N'Action=CLICK_MULTI',N'Không nhân đôi dữ liệu.'),
('B5C-WF-03','B5C','WINFORM',N'sp_ThongtinNguoilonDangmuon',N'Làm mới sau khi đã tải.',N'Action=RESET',N'Grid phải được làm sạch.'),
('B5C-SQL-01','B5C','SQL',N'sp_ThongtinNguoilonDangmuon',N'Bảng Muon rỗng.',N'Muon=Rong',N'Trả tập rỗng.'),
('B5C-DL-01','B5C','DULIEU',N'sp_ThongtinNguoilonDangmuon',N'Muon tham chiếu DocGia không tồn tại.',N'Muon.ma_DocGia=<orphan>',N'Không trả dữ liệu rác.'),
('B5C-DL-02','B5C','DULIEU',N'sp_ThongtinNguoilonDangmuon',N'Muon tham chiếu Cuonsach không tồn tại.',N'Muon.isbn/ma_cuonsach=<orphan>',N'Kiểm tra JOIN.'),
('B5C-NV-01','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Không có người lớn đang mượn.',N'TrangThaiDuLieu=0 NL',N'Kết quả rỗng.'),
('B5C-NV-02','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'1 người lớn mượn 1 cuốn.',N'TrangThaiDuLieu=1 NL,1 Muon',N'Ca cơ bản.'),
('B5C-NV-03','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'1 người lớn mượn nhiều cuốn.',N'TrangThaiDuLieu=1 NL,n Muon',N'Có nhiều dòng hoặc tổng hợp đúng thiết kế.'),
('B5C-NV-04','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Nhiều người lớn mỗi người mượn 1 cuốn.',N'TrangThaiDuLieu=n NL',N'Phải lấy đủ.'),
('B5C-NV-05','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Nhiều người lớn mượn nhiều cuốn.',N'TrangThaiDuLieu=n NL,n Muon',N'Phải lấy đủ.'),
('B5C-NV-06','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Chỉ trẻ em đang mượn.',N'TrangThaiDuLieu=chi Treem',N'Không được liệt kê.'),
('B5C-NV-07','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Cả người lớn và trẻ em đang mượn.',N'TrangThaiDuLieu=NL+TE',N'Chỉ lấy người lớn.'),
('B5C-NV-08','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Người lớn có thông tin phụ NULL.',N'TrangThaiDuLieu=NL fields NULL',N'Không làm mất dòng hợp lệ.'),
('B5C-NV-09','B5C','NGHIEPVU',N'sp_ThongtinNguoilonDangmuon',N'Người lớn có tên Unicode.',N'TrangThaiDuLieu=Unicode',N'Hiển thị đúng.');
GO

/*========================== BÀI 5D ==========================*/
-- TEST ĐÚNG / CHẠY BÌNH THƯỜNG
INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5D-OK-01','B5D','OK',N'sp_ThongtinNguoilonQuahan',N'Người lớn trễ hạn đúng 1 ngày.',N'ngay_hethan = hôm nay - 1',N'Phải thuộc danh sách quá hạn.'),
('B5D-OK-02','B5D','OK',N'sp_ThongtinNguoilonQuahan',N'Người lớn trễ hạn 14 ngày.',N'ngay_hethan = hôm nay - 14',N'Phải thuộc danh sách quá hạn.'),
('B5D-OK-03','B5D','OK',N'sp_ThongtinNguoilonQuahan',N'Người lớn quá hạn rất lâu.',N'ngay_hethan << hôm nay',N'Phải thuộc danh sách quá hạn.'),
('B5D-OK-04','B5D','OK',N'sp_ThongtinNguoilonQuahan',N'Nhiều người lớn cùng giữ sách sau hạn trả.',N'TrangThaiDuLieu=nhiều NL quá hạn',N'Phải trả đủ danh sách.');
GO

INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5D-WF-01','B5D','WINFORM',N'sp_ThongtinNguoilonQuahan',N'Chưa kết nối CSDL.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),
('B5D-WF-02','B5D','WINFORM',N'sp_ThongtinNguoilonQuahan',N'Nhấn tải nhiều lần.',N'Action=CLICK_MULTI',N'Không nhân đôi.'),
('B5D-WF-03','B5D','WINFORM',N'sp_ThongtinNguoilonQuahan',N'Làm mới.',N'Action=RESET',N'Xóa grid.'),
('B5D-SQL-01','B5D','SQL',N'sp_ThongtinNguoilonQuahan',N'Bảng Muon rỗng.',N'Muon=Rong',N'Trả rỗng.'),
('B5D-DL-01','B5D','DULIEU',N'sp_ThongtinNguoilonQuahan',N'ngay_hethan NULL.',N'ngay_hethan=NULL',N'Không coi NULL là quá hạn.'),
('B5D-DL-02','B5D','DULIEU',N'sp_ThongtinNguoilonQuahan',N'Muon tham chiếu độc giả không tồn tại.',N'Muon.ma_DocGia=<orphan>',N'Không trả dữ liệu rác.'),
('B5D-BIEN-01','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Ngày hết hạn ở tương lai.',N'ngay_hethan > hôm nay',N'Không quá hạn.'),
('B5D-BIEN-02','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Ngày hết hạn đúng hôm nay.',N'ngay_hethan = hôm nay',N'Chưa quá hạn, không lấy.'),
('B5D-BIEN-03','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Quá hạn đúng 1 ngày.',N'ngay_hethan = hôm nay - 1',N'Biên đầu tiên phải được lấy.'),
('B5D-BIEN-04','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Quá hạn đúng 13 ngày.',N'ngay_hethan = hôm nay - 13',N'Phải lấy vì đã qua ngày hết hạn.'),
('B5D-BIEN-05','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Quá hạn đúng 14 ngày.',N'ngay_hethan = hôm nay - 14',N'Phải lấy; 14 ngày là thời hạn mượn đã lưu trong ngay_hethan.'),
('B5D-BIEN-06','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Quá hạn đúng 15 ngày.',N'ngay_hethan = hôm nay - 15',N'Phải lấy.'),
('B5D-BIEN-07','B5D','BIEN',N'sp_ThongtinNguoilonQuahan',N'Quá hạn rất lâu.',N'ngay_hethan << hôm nay',N'Phải lấy.'),
('B5D-NV-01','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'Không có người lớn quá hạn.',N'TrangThaiDuLieu=0',N'Kết quả rỗng.'),
('B5D-NV-02','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'1 người lớn có 1 cuốn đã qua ngày hết hạn.',N'TrangThaiDuLieu=1 NL',N'Phải lấy.'),
('B5D-NV-03','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'1 người lớn mượn nhiều cuốn, chỉ 1 cuốn quá hạn.',N'TrangThaiDuLieu=1 qua han + n chua han',N'Chỉ dòng quá hạn phù hợp.'),
('B5D-NV-04','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'1 người lớn có nhiều cuốn đều quá hạn.',N'TrangThaiDuLieu=n qua han',N'Phải lấy đủ.'),
('B5D-NV-05','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'Nhiều người lớn cùng quá hạn.',N'TrangThaiDuLieu=n NL',N'Phải lấy đủ.'),
('B5D-NV-06','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'Trẻ em đã quá ngày hết hạn.',N'Treem; ngay_hethan < hôm nay',N'Không được liệt kê.'),
('B5D-NV-07','B5D','NGHIEPVU',N'sp_ThongtinNguoilonQuahan',N'Cả người lớn và trẻ em cùng quá hạn.',N'NL+TE',N'Chỉ lấy người lớn.');
GO

/*========================== BÀI 5E ==========================*/
-- TEST ĐÚNG / CHẠY BÌNH THƯỜNG
INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5E-OK-01','B5E','OK',N'sp_DocGiaCoTreEmMuon',N'1 người lớn đang mượn và 1 trẻ em do người đó bảo lãnh cũng đang mượn.',N'TrangThaiDuLieu=1 NL + 1 TE cùng mượn',N'Ca đúng yêu cầu cơ bản.'),
('B5E-OK-02','B5E','OK',N'sp_DocGiaCoTreEmMuon',N'1 người lớn có nhiều trẻ em được bảo lãnh cùng đang mượn.',N'TrangThaiDuLieu=1 NL + nhiều TE cùng mượn',N'Phải lấy đủ quan hệ phù hợp.'),
('B5E-OK-03','B5E','OK',N'sp_DocGiaCoTreEmMuon',N'Nhiều người lớn và trẻ em tương ứng cùng đang mượn.',N'TrangThaiDuLieu=nhiều cặp NL-TE',N'Phải trả đủ các cặp thỏa điều kiện.'),
('B5E-OK-04','B5E','OK',N'sp_DocGiaCoTreEmMuon',N'Người lớn và trẻ em cùng mượn nhiều cuốn.',N'TrangThaiDuLieu=NL và TE có nhiều dòng Muon',N'Kiểm tra procedure không bỏ sót dữ liệu.');
GO

INSERT INTO dbo.Testcase_Nhom2 VALUES
('B5E-WF-01','B5E','WINFORM',N'sp_DocGiaCoTreEmMuon',N'Chưa kết nối CSDL.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),
('B5E-WF-02','B5E','WINFORM',N'sp_DocGiaCoTreEmMuon',N'Nhấn tải nhiều lần.',N'Action=CLICK_MULTI',N'Không nhân đôi.'),
('B5E-WF-03','B5E','WINFORM',N'sp_DocGiaCoTreEmMuon',N'Làm mới.',N'Action=RESET',N'Xóa grid.'),
('B5E-SQL-01','B5E','SQL',N'sp_DocGiaCoTreEmMuon',N'Bảng Treem rỗng.',N'Treem=Rong',N'Trả rỗng.'),
('B5E-SQL-02','B5E','SQL',N'sp_DocGiaCoTreEmMuon',N'Bảng Muon rỗng.',N'Muon=Rong',N'Trả rỗng.'),
('B5E-DL-01','B5E','DULIEU',N'sp_DocGiaCoTreEmMuon',N'TreEm tham chiếu người bảo lãnh không tồn tại.',N'ma_DocGia_nguoilon=<orphan>',N'Không trả cặp sai.'),
('B5E-DL-02','B5E','DULIEU',N'sp_DocGiaCoTreEmMuon',N'Trẻ em thiếu DocGia cha.',N'Treem orphan DocGia',N'Kiểm tra JOIN.'),
('B5E-NV-01','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Không có cặp người lớn-trẻ em nào cùng mượn.',N'TrangThaiDuLieu=0 cap',N'Kết quả rỗng.'),
('B5E-NV-02','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Người lớn mượn, trẻ em được bảo lãnh không mượn.',N'NL=Muon; TE=Khong',N'Không lấy.'),
('B5E-NV-03','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Trẻ em mượn, người lớn bảo lãnh không mượn.',N'NL=Khong; TE=Muon',N'Không lấy.'),
('B5E-NV-04','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'1 người lớn và đúng 1 trẻ em cùng mượn.',N'1 NL + 1 TE',N'Ca cơ bản.'),
('B5E-NV-05','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'1 người lớn bảo lãnh nhiều trẻ em, chỉ 1 trẻ em mượn.',N'1 NL + 1/n TE muon',N'Chỉ lấy trẻ em đang mượn.'),
('B5E-NV-06','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'1 người lớn bảo lãnh nhiều trẻ em, nhiều trẻ em cùng mượn.',N'1 NL + n TE muon',N'Phải lấy đủ.'),
('B5E-NV-07','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Nhiều người lớn và các trẻ em tương ứng cùng mượn.',N'n cap NL-TE',N'Phải lấy đủ.'),
('B5E-NV-08','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Trẻ em A được NL1 bảo lãnh nhưng NL2 đang mượn.',N'TE->NL1; NL2 muon',N'Không ghép nhầm NL2.'),
('B5E-NV-09','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Người lớn đang mượn nhiều cuốn và trẻ em cũng mượn nhiều cuốn.',N'NL n Muon; TE n Muon',N'Kiểm tra nguy cơ nhân bản dòng do JOIN.'),
('B5E-NV-10','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Tên người lớn/trẻ em Unicode.',N'Unicode',N'Hiển thị đúng.'),
('B5E-NV-11','B5E','NGHIEPVU',N'sp_DocGiaCoTreEmMuon',N'Có nhiều trẻ em nhưng chỉ một trẻ thuộc đúng người bảo lãnh đang mượn.',N'QuanHeHonHop',N'Phải lọc đúng khóa quan hệ.');
GO

/*============================================================
  BÀI 6.1 - tg_delMuon
============================================================*/
INSERT INTO dbo.Testcase_Nhom2
(ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu)
VALUES
('B61-WF-01','B6.1','WINFORM',N'tg_delMuon',N'Không nhập ISBN.',N'isbn=',N'WinForms phải chặn.'),
('B61-WF-02','B6.1','WINFORM',N'tg_delMuon',N'Không nhập mã cuốn.',N'ma_cuonsach=',N'WinForms phải chặn.'),
('B61-WF-03','B6.1','WINFORM',N'tg_delMuon',N'Không nhập mã độc giả.',N'ma_DocGia=',N'WinForms phải chặn.'),
('B61-WF-04','B6.1','WINFORM',N'tg_delMuon',N'Nhập khoảng trắng ở một trường.',N'isbn=   ',N'Phải Trim.'),
('B61-WF-05','B6.1','WINFORM',N'tg_delMuon',N'Nhấn kiểm tra khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'WinForms phải chặn.'),

('B61-SQL-01','B6.1','SQL',N'tg_delMuon',N'Xóa phiếu mượn không tồn tại.',N'Action=DELETE; isbn=ISBN_KHONG_TON_TAI; ma_cuonsach=-1',N'Không có dòng bị xóa.'),
('B61-SQL-02','B6.1','SQL',N'tg_delMuon',N'Xóa nhiều dòng cùng một lệnh.',N'Action=DELETE_MULTI',N'Kiểm tra trigger xử lý deleted nhiều dòng.'),
('B61-SQL-03','B6.1','SQL',N'tg_delMuon',N'Xóa phiếu mượn hợp lệ trong transaction.',N'Action=DELETE; <dòng Muon hợp lệ>',N'Sau trigger Cuonsach.tinhtrang=yes rồi rollback.'),

('B61-NV-01','B6.1','NGHIEPVU',N'tg_delMuon',N'Độc giả mượn nhiều cuốn, chỉ xóa một cuốn.',N'Action=DELETE_ONE_OF_MANY',N'Chỉ cuốn bị xóa được đổi trạng thái.');
GO

/*============================================================
  BÀI 6.2 - tg_insMuon
============================================================*/
INSERT INTO dbo.Testcase_Nhom2
(ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu)
VALUES
('B62-WF-01','B6.2','WINFORM',N'tg_insMuon',N'Không nhập ISBN.',N'isbn=',N'WinForms phải chặn.'),
('B62-WF-02','B6.2','WINFORM',N'tg_insMuon',N'Không nhập mã cuốn.',N'ma_cuonsach=',N'WinForms phải chặn.'),
('B62-WF-03','B6.2','WINFORM',N'tg_insMuon',N'Không nhập mã độc giả.',N'ma_DocGia=',N'WinForms phải chặn.'),
('B62-WF-04','B6.2','WINFORM',N'tg_insMuon',N'Ngày mượn không đúng định dạng.',N'ngay_muon=abc',N'WinForms phải báo lỗi ngày.'),
('B62-WF-05','B6.2','WINFORM',N'tg_insMuon',N'Ngày hết hạn không đúng định dạng.',N'ngay_hethan=xyz',N'WinForms phải báo lỗi ngày.'),
('B62-WF-06','B6.2','WINFORM',N'tg_insMuon',N'Ngày hết hạn nhỏ hơn ngày mượn.',N'ngay_muon=2026-09-10; ngay_hethan=2026-09-09',N'WinForms nên chặn nghiệp vụ.'),
('B62-WF-07','B6.2','WINFORM',N'tg_insMuon',N'Nhấn kiểm tra khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),

('B62-SQL-01','B6.2','SQL',N'tg_insMuon',N'ISBN không tồn tại.',N'isbn=ISBN_KHONG_TON_TAI',N'Có thể bị FK chặn.'),
('B62-SQL-02','B6.2','SQL',N'tg_insMuon',N'Mã cuốn không tồn tại trong ISBN.',N'isbn=<hợp lệ>; ma_cuonsach=-1',N'Kiểm tra khóa ngoại/khóa ghép.'),
('B62-SQL-03','B6.2','SQL',N'tg_insMuon',N'Mã độc giả không tồn tại.',N'ma_DocGia=DG_KHONG_TON_TAI',N'Kiểm tra FK.'),
('B62-SQL-04','B6.2','SQL',N'tg_insMuon',N'Thêm nhiều phiếu mượn trong cùng một INSERT.',N'Action=INSERT_MULTI',N'Kiểm tra inserted nhiều dòng.'),
('B62-SQL-05','B6.2','SQL',N'tg_insMuon',N'Thêm phiếu mượn cho cuốn đã có dòng trong Muon.',N'<cuốn đang mượn>',N'Kiểm tra constraint/nghiệp vụ mượn trùng.'),

('B62-NV-01','B6.2','NGHIEPVU',N'tg_insMuon',N'Thêm phiếu mượn hợp lệ cho cuốn có sẵn.',N'<ISBN,cuốn,độc giả hợp lệ>',N'Sau trigger tinhtrang=no rồi rollback.');
GO

/*============================================================
  BÀI 6.3 - tg_updCuonSach
============================================================*/
INSERT INTO dbo.Testcase_Nhom2
(ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu)
VALUES
('B63-WF-01','B6.3','WINFORM',N'tg_updCuonSach',N'Không nhập ISBN.',N'isbn=',N'WinForms phải chặn.'),
('B63-WF-02','B6.3','WINFORM',N'tg_updCuonSach',N'Không nhập mã cuốn.',N'ma_cuonsach=',N'WinForms phải chặn.'),
('B63-WF-03','B6.3','WINFORM',N'tg_updCuonSach',N'Không nhập tình trạng mới.',N'tinhtrang_moi=',N'WinForms phải chặn.'),
('B63-WF-04','B6.3','WINFORM',N'tg_updCuonSach',N'Nhập tình trạng ngoài miền yes/no nếu thiết kế yêu cầu.',N'tinhtrang_moi=abc',N'WinForms nên validation miền giá trị.'),
('B63-WF-05','B6.3','WINFORM',N'tg_updCuonSach',N'Nhấn kiểm tra khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),

('B63-SQL-01','B6.3','SQL',N'tg_updCuonSach',N'Cập nhật cuốn không tồn tại.',N'isbn=ISBN_KHONG_TON_TAI; ma_cuonsach=-1',N'0 dòng cập nhật.'),
('B63-SQL-02','B6.3','SQL',N'tg_updCuonSach',N'Cập nhật nhiều cuốn cùng ISBN.',N'Action=UPDATE_MULTI_SAME_ISBN',N'Kiểm tra inserted/deleted nhiều dòng.'),
('B63-SQL-03','B6.3','SQL',N'tg_updCuonSach',N'Cập nhật nhiều cuốn thuộc nhiều ISBN.',N'Action=UPDATE_MULTI_ISBN',N'Không được chỉ xử lý một ISBN.'),
('B63-SQL-04','B6.3','SQL',N'tg_updCuonSach',N'Cập nhật thuộc tính khác, không đổi tinhtrang.',N'Action=UPDATE_OTHER_COLUMN',N'Trigger không nên xử lý thừa nếu có kiểm tra UPDATE(tinhtrang).'),

('B63-NV-01','B6.3','NGHIEPVU',N'tg_updCuonSach',N'Đổi cuốn đang mượn sang có sẵn.',N'tinhtrang_moi=yes',N'Cập nhật Dausach tương ứng.'),
('B63-NV-02','B6.3','NGHIEPVU',N'tg_updCuonSach',N'Đổi cuốn có sẵn cuối cùng sang no.',N'Action=UPDATE_LAST_AVAILABLE_TO_NO',N'Đầu sách chuyển hết sách.'),
('B63-NV-03','B6.3','NGHIEPVU',N'tg_updCuonSach',N'Đổi một cuốn sang no nhưng ISBN vẫn còn cuốn yes.',N'<ISBN có >=2 cuốn yes>',N'Đầu sách vẫn phản ánh còn sách.');
GO

/*============================================================
  BÀI 6.4 - tg_InfThongBao
============================================================*/
INSERT INTO dbo.Testcase_Nhom2
(ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu)
VALUES
('B64-WF-01','B6.4','WINFORM',N'tg_InfThongBao',N'Không chọn thao tác.',N'Action=',N'WinForms phải chặn.'),
('B64-WF-02','B6.4','WINFORM',N'tg_InfThongBao',N'INSERT nhưng không nhập mã tựa sách.',N'Action=INSERT; ma_tuasach=',N'WinForms phải chặn.'),
('B64-WF-03','B6.4','WINFORM',N'tg_InfThongBao',N'INSERT nhưng không nhập tên tựa sách.',N'Action=INSERT; tuasach=',N'WinForms nên chặn nếu cột bắt buộc.'),
('B64-WF-04','B6.4','WINFORM',N'tg_InfThongBao',N'UPDATE nhưng không nhập mã tựa sách.',N'Action=UPDATE_TUASACH; ma_tuasach=',N'WinForms phải chặn.'),
('B64-WF-05','B6.4','WINFORM',N'tg_InfThongBao',N'Nhấn kiểm tra khi chưa kết nối.',N'TrangThaiKetNoi=ChuaKetNoi',N'Phải chặn.'),

('B64-SQL-01','B6.4','SQL',N'tg_InfThongBao',N'INSERT mã tựa sách bị trùng khóa chính.',N'Action=INSERT; ma_tuasach=<mã đã tồn tại>',N'SQL phải báo lỗi PK.'),
('B64-SQL-02','B6.4','SQL',N'tg_InfThongBao',N'UPDATE mã tựa sách không tồn tại.',N'Action=UPDATE_TUASACH; ma_tuasach=TS_KHONG_TON_TAI',N'0 dòng cập nhật.'),
('B64-SQL-03','B6.4','SQL',N'tg_InfThongBao',N'INSERT nhiều tựa sách cùng lúc.',N'Action=INSERT_MULTI',N'Kiểm tra trigger với inserted nhiều dòng.'),
('B64-SQL-04','B6.4','SQL',N'tg_InfThongBao',N'UPDATE nhiều dòng cùng lúc.',N'Action=UPDATE_MULTI',N'Kiểm tra trigger không giả định một dòng.'),

('B64-NV-01','B6.4','NGHIEPVU',N'tg_InfThongBao',N'Thêm mới một tựa sách hợp lệ.',N'Action=INSERT; <dữ liệu hợp lệ>',N'Phải in thông báo theo đề.'),
('B64-NV-02','B6.4','NGHIEPVU',N'tg_InfThongBao',N'Sửa tên tựa sách.',N'Action=UPDATE_TUASACH',N'Phải kích hoạt thông báo.'),
('B64-NV-03','B6.4','NGHIEPVU',N'tg_InfThongBao',N'Sửa tên tác giả.',N'Action=UPDATE_TACGIA',N'Phải kích hoạt thông báo.'),
('B64-NV-04','B6.4','NGHIEPVU',N'tg_InfThongBao',N'Sửa đồng thời tựa sách và tác giả.',N'Action=UPDATE_BOTH',N'Kiểm tra update nhiều cột.'),
('B64-NV-05','B6.4','NGHIEPVU',N'tg_InfThongBao',N'Chỉ sửa tóm tắt.',N'Action=UPDATE_TOMTAT_ONLY',N'Dùng để kiểm tra trigger có thông báo ngoài phạm vi yêu cầu hay không.');
GO


/*============================================================
  PROCEDURE LOAD TESTCASE CHUNG
============================================================*/
CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcaseBai5
    @MaBai VARCHAR(10) = NULL,
    @NhomLoi VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu
    FROM dbo.Testcase_Nhom2
    WHERE MaBai LIKE 'B5%'
      AND (@MaBai IS NULL OR MaBai = @MaBai)
      AND (@NhomLoi IS NULL OR NhomLoi = @NhomLoi)
    ORDER BY MaBai,
      CASE NhomLoi WHEN 'WINFORM' THEN 1 WHEN 'SQL' THEN 2 WHEN 'DULIEU' THEN 3 WHEN 'BIEN' THEN 4 WHEN 'NGHIEPVU' THEN 5 ELSE 9 END,
      ID_Testcase;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_LoadTestcaseBai6
    @MaBai VARCHAR(10) = NULL,
    @NhomLoi VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Testcase, MaBai, NhomLoi, ChucNang, MoTa, DuLieuNhap, GhiChu
    FROM dbo.Testcase_Nhom2
    WHERE MaBai LIKE 'B6%'
      AND (@MaBai IS NULL OR MaBai = @MaBai)
      AND (@NhomLoi IS NULL OR NhomLoi = @NhomLoi)
    ORDER BY MaBai, NhomLoi, ID_Testcase;
END
GO

-- Kiểm tra nhanh:
-- EXEC dbo.sp_LoadTestcaseBai5 @MaBai='B5A';
-- EXEC dbo.sp_LoadTestcaseBai5 @MaBai='B5D', @NhomLoi='BIEN';
-- EXEC dbo.sp_LoadTestcaseBai6 @MaBai='B6.1';
GO
