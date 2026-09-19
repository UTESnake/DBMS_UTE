-- ============================================================
-- QL_ThuVien - TẠO BẢNG VÀ NHẬP DỮ LIỆU
-- Chỉ gồm:
--   1. CREATE TABLE
--   2. INSERT dữ liệu mẫu
--   3. DEFAULT / FOREIGN KEY cần thiết cho cấu trúc bảng
-- Không gồm Stored Procedure, Function, Trigger hay cấu hình Database.
-- ============================================================

-- Chọn cơ sở dữ liệu QL_ThuVien để tạo bảng và nhập dữ liệu.
USE [QL_ThuVien]
GO
/****** Object:  Table [dbo].[Cuonsach]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Cuonsach
-- Chức năng: Lưu từng cuốn sách vật lý thuộc một đầu sách.
-- Khóa chính: (isbn, ma_cuonsach)
-- Các cột:
--   isbn        : mã ISBN của đầu sách.
--   ma_cuonsach : mã riêng của từng cuốn sách.
--   tinhtrang   : trạng thái hiện tại, ví dụ 'Có sẵn', 'Đang mượn'.
-- ============================================================
CREATE TABLE [dbo].[Cuonsach](
	[isbn] [varchar](20) NOT NULL,
	[ma_cuonsach] [varchar](20) NOT NULL,
	[tinhtrang] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Cuonsach] PRIMARY KEY CLUSTERED 
(
	[isbn] ASC,
	[ma_cuonsach] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DangKy]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: DangKy
-- Chức năng: Lưu thông tin độc giả đăng ký/chờ mượn một đầu sách.
-- Khóa chính: (isbn, ma_DocGia, ngay_dk)
-- Các cột:
--   isbn      : đầu sách được đăng ký.
--   ma_DocGia : độc giả đăng ký.
--   ngay_dk   : ngày đăng ký.
--   ghichu    : nội dung ghi chú.
-- ============================================================
CREATE TABLE [dbo].[DangKy](
	[isbn] [varchar](20) NOT NULL,
	[ma_DocGia] [varchar](10) NOT NULL,
	[ngay_dk] [date] NOT NULL,
	[ghichu] [nvarchar](255) NULL,
 CONSTRAINT [PK_DangKy] PRIMARY KEY CLUSTERED 
(
	[isbn] ASC,
	[ma_DocGia] ASC,
	[ngay_dk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dausach]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Dausach
-- Chức năng: Lưu thông tin của từng đầu sách.
-- Khóa chính: isbn
-- Các cột:
--   isbn       : mã ISBN của đầu sách.
--   ma_tuasach : mã tựa sách liên kết sang bảng Tuasach.
--   ngonngu    : ngôn ngữ của sách.
--   bia        : loại bìa.
--   trangthai  : trạng thái phục vụ của đầu sách.
-- ============================================================
CREATE TABLE [dbo].[Dausach](
	[isbn] [varchar](20) NOT NULL,
	[ma_tuasach] [varchar](10) NOT NULL,
	[ngonngu] [nvarchar](30) NULL,
	[bia] [nvarchar](50) NULL,
	[trangthai] [nvarchar](30) NULL,
 CONSTRAINT [PK_Dausach] PRIMARY KEY CLUSTERED 
(
	[isbn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocGia]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: DocGia
-- Chức năng: Lưu thông tin chung của tất cả độc giả.
-- Khóa chính: ma_DocGia
-- Các cột:
--   ma_DocGia : mã độc giả.
--   ho        : họ.
--   tenlot    : tên lót.
--   ten       : tên.
--   ngaysinh  : ngày sinh.
-- ============================================================
CREATE TABLE [dbo].[DocGia](
	[ma_DocGia] [varchar](10) NOT NULL,
	[ho] [nvarchar](30) NOT NULL,
	[tenlot] [nvarchar](30) NULL,
	[ten] [nvarchar](30) NOT NULL,
	[ngaysinh] [date] NOT NULL,
 CONSTRAINT [PK_DocGia] PRIMARY KEY CLUSTERED 
(
	[ma_DocGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Muon]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Muon
-- Chức năng: Lưu các cuốn sách hiện đang được mượn.
-- Khóa chính: (isbn, ma_cuonsach, ma_DocGia)
-- Các cột:
--   isbn        : mã đầu sách.
--   ma_cuonsach : mã cuốn sách.
--   ma_DocGia   : mã độc giả đang mượn.
--   ngay_muon   : ngày bắt đầu mượn.
--   ngay_hethan : ngày hết hạn mượn.
-- ============================================================
CREATE TABLE [dbo].[Muon](
	[isbn] [varchar](20) NOT NULL,
	[ma_cuonsach] [varchar](20) NOT NULL,
	[ma_DocGia] [varchar](10) NOT NULL,
	[ngay_muon] [date] NOT NULL,
	[ngay_hethan] [date] NOT NULL,
 CONSTRAINT [PK_Muon] PRIMARY KEY CLUSTERED 
(
	[isbn] ASC,
	[ma_cuonsach] ASC,
	[ma_DocGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Muon_CuonDangMuon] UNIQUE ([isbn], [ma_cuonsach]),
 CONSTRAINT [CK_Muon_ThoiHan] CHECK ([ngay_hethan] >= [ngay_muon])
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nguoilon]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Nguoilon
-- Chức năng: Lưu thông tin bổ sung của độc giả người lớn.
-- Khóa chính: ma_DocGia
-- Các cột:
--   ma_DocGia : mã độc giả người lớn.
--   sonha     : số nhà.
--   duong     : tên đường.
--   quan      : quận/khu vực.
--   dienthoai : số điện thoại.
--   han_sd    : hạn sử dụng thẻ.
-- ============================================================
CREATE TABLE [dbo].[Nguoilon](
	[ma_DocGia] [varchar](10) NOT NULL,
	[sonha] [nvarchar](20) NULL,
	[duong] [nvarchar](100) NULL,
	[quan] [nvarchar](50) NULL,
	[dienthoai] [varchar](15) NULL,
	[han_sd] [date] NULL,
 CONSTRAINT [PK_Nguoilon] PRIMARY KEY CLUSTERED 
(
	[ma_DocGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuaTrinhMuon]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: QuaTrinhMuon
-- Chức năng: Lưu lịch sử/quá trình mượn và trả sách.
-- Khóa chính: (isbn, ma_cuonsach, ngay_muon, ma_DocGia)
-- Các cột:
--   isbn        : mã đầu sách.
--   ma_cuonsach : mã cuốn sách.
--   ngay_muon   : ngày mượn.
--   ma_DocGia   : mã độc giả.
--   ngay_hethan : ngày hết hạn.
--   ngay_tra    : ngày trả thực tế, NULL nếu chưa trả.
--   tien_muon   : tiền mượn.
--   tien_datra  : số tiền đã trả.
--   tien_datcoc : tiền đặt cọc.
--   ghichu      : ghi chú tình trạng mượn/trả.
-- ============================================================
CREATE TABLE [dbo].[QuaTrinhMuon](
	[isbn] [varchar](20) NOT NULL,
	[ma_cuonsach] [varchar](20) NOT NULL,
	[ngay_muon] [date] NOT NULL,
	[ma_DocGia] [varchar](10) NOT NULL,
	[ngay_hethan] [date] NOT NULL,
	[ngay_tra] [date] NULL,
	[tien_muon] [decimal](18, 2) NULL,
	[tien_datra] [decimal](18, 2) NULL,
	[tien_datcoc] [decimal](18, 2) NULL,
	[ghichu] [nvarchar](255) NULL,
 CONSTRAINT [PK_QuaTrinhMuon] PRIMARY KEY CLUSTERED 
(
	[isbn] ASC,
	[ma_cuonsach] ASC,
	[ngay_muon] ASC,
	[ma_DocGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tuasach]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Tuasach
-- Chức năng: Lưu thông tin nội dung của tựa sách.
-- Khóa chính: ma_tuasach
-- Các cột:
--   ma_tuasach : mã tựa sách.
--   tuasach    : tên tựa sách.
--   tacgia     : tác giả.
--   tomtat     : phần tóm tắt nội dung.
-- ============================================================
CREATE TABLE [dbo].[Tuasach](
	[ma_tuasach] [varchar](10) NOT NULL,
	[tuasach] [nvarchar](200) NOT NULL,
	[tacgia] [nvarchar](150) NOT NULL,
	[tomtat] [nvarchar](1000) NULL,
 CONSTRAINT [PK_Tuasach] PRIMARY KEY CLUSTERED 
(
	[ma_tuasach] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Treem]    Script Date: 07/09/2026 10:10:02 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================
-- BẢNG: Treem
-- Chức năng: Lưu độc giả trẻ em và người lớn bảo lãnh tương ứng.
-- Khóa chính: ma_DocGia
-- Các cột:
--   ma_DocGia           : mã độc giả trẻ em.
--   ma_DocGia_nguoilon  : mã độc giả người lớn bảo lãnh.
-- ============================================================
CREATE TABLE [dbo].[Treem](
	[ma_DocGia] [varchar](10) NOT NULL,
	[ma_DocGia_nguoilon] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Treem] PRIMARY KEY CLUSTERED 
(
	[ma_DocGia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Cuonsach
-- Mục đích: Tạo dữ liệu các cuốn sách cụ thể để kiểm thử trạng thái
-- 'Có sẵn' và 'Đang mượn'.
-- ============================================================
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN001', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN001', N'CS002', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN001', N'CS003', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN001', N'CS004', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN001', N'CS005', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN002', N'CS001', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN002', N'CS002', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN002', N'CS003', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN003', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN003', N'CS002', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN003', N'CS003', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN003', N'CS004', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN004', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN004', N'CS002', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN004', N'CS003', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN005', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN005', N'CS002', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN006', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN007', N'CS001', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN007', N'CS002', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN008', N'CS001', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN008', N'CS002', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN009', N'CS001', N'Đang mượn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN009', N'CS002', N'Có sẵn')
INSERT [dbo].[Cuonsach] ([isbn], [ma_cuonsach], [tinhtrang]) VALUES (N'ISBN009', N'CS003', N'Đang mượn')
GO
-- ============================================================
-- NHẬP DỮ LIỆU: DangKy
-- Mục đích: Tạo dữ liệu các lượt đăng ký/chờ mượn sách của độc giả.
-- ============================================================
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN001', N'DG003', CAST(N'2026-08-01' AS Date), N'Đăng ký chờ sách')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN001', N'TE003', CAST(N'2026-08-02' AS Date), N'Đăng ký mượn')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN002', N'DG004', CAST(N'2026-08-03' AS Date), N'Đang chờ')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN003', N'DG005', CAST(N'2026-08-04' AS Date), N'Đăng ký tham khảo')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN004', N'TE004', CAST(N'2026-08-05' AS Date), N'Chờ trả sách')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN005', N'DG007', CAST(N'2026-08-06' AS Date), N'Đầu sách ngừng phục vụ')
INSERT [dbo].[DangKy] ([isbn], [ma_DocGia], [ngay_dk], [ghichu]) VALUES (N'ISBN008', N'DG008', CAST(N'2026-08-07' AS Date), N'Đăng ký sách tiếng Anh')
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Dausach
-- Mục đích: Tạo danh sách đầu sách để liên kết với Tuasach và Cuonsach.
-- ============================================================
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN001', N'TS01', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN002', N'TS02', N'Tiếng Việt', N'Bìa cứng', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN003', N'TS03', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN004', N'TS04', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN005', N'TS05', N'Tiếng Việt', N'Bìa cứng', N'Ngừng phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN006', N'TS06', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN007', N'TS07', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN008', N'TS08', N'Tiếng Anh', N'Bìa cứng', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN009', N'TS09', N'Tiếng Việt', N'Bìa mềm', N'Đang phục vụ')
INSERT [dbo].[Dausach] ([isbn], [ma_tuasach], [ngonngu], [bia], [trangthai]) VALUES (N'ISBN010', N'TS10', N'Tiếng Việt', N'Bìa cứng', N'Đang phục vụ')
GO
-- ============================================================
-- NHẬP DỮ LIỆU: DocGia
-- Mục đích: Tạo dữ liệu độc giả người lớn (DG...) và trẻ em (TE...).
-- ============================================================
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG001', N'Nguyễn', N'Văn', N'An', CAST(N'1985-03-15' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG002', N'Trần', N'Thị', N'Bình', CAST(N'1990-07-20' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG003', N'Lê', N'Hoàng', N'Nam', CAST(N'1982-11-05' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG004', N'Phạm', N'Minh', N'Khang', CAST(N'1988-09-10' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG005', N'Võ', N'Thị', N'Lan', CAST(N'1992-01-25' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG006', N'Đặng', N'Quốc', N'Huy', CAST(N'1980-06-18' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG007', N'Bùi', N'Thanh', N'Tùng', CAST(N'1987-04-14' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'DG008', N'Hoàng', N'Ngọc', N'Mai', CAST(N'1991-12-03' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE001', N'Nguyễn', N'Gia', N'Bảo', CAST(N'2015-04-12' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE002', N'Nguyễn', N'Minh', N'Châu', CAST(N'2014-12-03' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE003', N'Trần', N'Ngọc', N'Anh', CAST(N'2016-08-25' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE004', N'Lê', N'Gia', N'Hân', CAST(N'2015-09-10' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE005', N'Phạm', N'Tuấn', N'Kiệt', CAST(N'2017-05-19' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE006', N'Võ', N'Ngọc', N'Linh', CAST(N'2016-02-28' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE007', N'Đặng', N'Minh', N'Quân', CAST(N'2014-07-07' AS Date))
INSERT [dbo].[DocGia] ([ma_DocGia], [ho], [tenlot], [ten], [ngaysinh]) VALUES (N'TE008', N'Hoàng', N'Khánh', N'Vy', CAST(N'2015-11-22' AS Date))
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Muon
-- Mục đích: Tạo dữ liệu các cuốn đang được mượn để phục vụ truy vấn
-- tình trạng mượn, quá hạn và các stored procedure của bài sau.
-- ============================================================
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN001', N'CS004', N'DG001', CAST(N'2026-07-20' AS Date), CAST(N'2026-08-03' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN001', N'CS005', N'DG002', CAST(N'2026-08-20' AS Date), CAST(N'2026-09-03' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN002', N'CS001', N'TE001', CAST(N'2026-08-01' AS Date), CAST(N'2026-08-15' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN002', N'CS002', N'TE003', CAST(N'2026-08-22' AS Date), CAST(N'2026-09-05' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN002', N'CS003', N'DG006', CAST(N'2026-08-03' AS Date), CAST(N'2026-08-17' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN004', N'CS002', N'DG003', CAST(N'2026-09-01' AS Date), CAST(N'2026-09-15' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN005', N'CS002', N'TE005', CAST(N'2026-08-10' AS Date), CAST(N'2026-08-24' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN007', N'CS001', N'DG005', CAST(N'2026-08-05' AS Date), CAST(N'2026-08-19' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN009', N'CS001', N'TE006', CAST(N'2026-08-06' AS Date), CAST(N'2026-08-20' AS Date))
INSERT [dbo].[Muon] ([isbn], [ma_cuonsach], [ma_DocGia], [ngay_muon], [ngay_hethan]) VALUES (N'ISBN009', N'CS003', N'DG006', CAST(N'2026-08-01' AS Date), CAST(N'2026-08-15' AS Date))
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Nguoilon
-- Mục đích: Bổ sung thông tin địa chỉ, điện thoại và hạn sử dụng thẻ
-- cho các độc giả người lớn.
-- ============================================================
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG001', N'12', N'Lê Lợi', N'Quận 1', N'0901111111', CAST(N'2027-12-31' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG002', N'25', N'Nguyễn Trãi', N'Quận 5', N'0902222222', CAST(N'2027-06-30' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG003', N'48', N'Điện Biên Phủ', N'Bình Thạnh', N'0903333333', CAST(N'2028-01-15' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG004', N'100', N'Phan Văn Trị', N'Gò Vấp', N'0904444444', CAST(N'2027-09-20' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG005', N'15', N'Quang Trung', N'Gò Vấp', N'0905555555', CAST(N'2028-05-10' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG006', N'75', N'Cách Mạng Tháng 8', N'Quận 3', N'0906666666', CAST(N'2027-11-15' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG007', N'39', N'Xô Viết Nghệ Tĩnh', N'Bình Thạnh', N'0907777777', CAST(N'2028-03-01' AS Date))
INSERT [dbo].[Nguoilon] ([ma_DocGia], [sonha], [duong], [quan], [dienthoai], [han_sd]) VALUES (N'DG008', N'21', N'Võ Văn Tần', N'Quận 3', N'0908888888', CAST(N'2027-08-25' AS Date))
GO
-- ============================================================
-- NHẬP DỮ LIỆU: QuaTrinhMuon
-- Mục đích: Tạo lịch sử mượn/trả gồm trường hợp đúng hạn, trễ hạn,
-- chưa trả, tiền mượn, tiền đã trả và tiền đặt cọc.
-- ============================================================
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN001', N'CS004', CAST(N'2026-07-20' AS Date), N'DG001', CAST(N'2026-08-03' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Đang mượn quá hạn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN001', N'CS005', CAST(N'2026-08-20' AS Date), N'DG002', CAST(N'2026-09-03' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN002', N'CS001', CAST(N'2026-08-01' AS Date), N'TE001', CAST(N'2026-08-15' AS Date), NULL, CAST(5000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), N'Đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN002', N'CS002', CAST(N'2026-08-22' AS Date), N'TE003', CAST(N'2026-09-05' AS Date), NULL, CAST(5000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), N'Đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN002', N'CS003', CAST(N'2026-08-03' AS Date), N'DG006', CAST(N'2026-08-17' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Người lớn mượn nhiều sách')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN003', N'CS001', CAST(N'2026-05-01' AS Date), N'DG001', CAST(N'2026-05-15' AS Date), CAST(N'2026-05-14' AS Date), CAST(10000.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Trả đúng hạn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN003', N'CS002', CAST(N'2026-05-10' AS Date), N'DG002', CAST(N'2026-05-24' AS Date), CAST(N'2026-05-30' AS Date), CAST(10000.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Trả trễ 6 ngày')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN003', N'CS003', CAST(N'2026-06-01' AS Date), N'TE001', CAST(N'2026-06-15' AS Date), CAST(N'2026-06-14' AS Date), CAST(5000.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), N'Trẻ em trả đúng hạn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN004', N'CS002', CAST(N'2026-09-01' AS Date), N'DG003', CAST(N'2026-09-15' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Đang mượn đúng hạn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN005', N'CS002', CAST(N'2026-08-10' AS Date), N'TE005', CAST(N'2026-08-24' AS Date), NULL, CAST(5000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), N'Trẻ em đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN007', N'CS001', CAST(N'2026-08-05' AS Date), N'DG005', CAST(N'2026-08-19' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Người lớn đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN009', N'CS001', CAST(N'2026-08-06' AS Date), N'TE006', CAST(N'2026-08-20' AS Date), NULL, CAST(5000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), N'Trẻ em đang mượn')
INSERT [dbo].[QuaTrinhMuon] ([isbn], [ma_cuonsach], [ngay_muon], [ma_DocGia], [ngay_hethan], [ngay_tra], [tien_muon], [tien_datra], [tien_datcoc], [ghichu]) VALUES (N'ISBN009', N'CS003', CAST(N'2026-08-01' AS Date), N'DG006', CAST(N'2026-08-15' AS Date), NULL, CAST(10000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), N'Người lớn mượn nhiều sách')
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Tuasach
-- Mục đích: Tạo thông tin tên sách, tác giả và tóm tắt cho từng tựa.
-- ============================================================
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS01', N'Lập trình C# căn bản', N'Nguyễn Văn A', N'Tài liệu giới thiệu nền tảng lập trình C#.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS02', N'Cơ sở dữ liệu', N'Trần Minh B', N'Kiến thức cơ bản và nâng cao về cơ sở dữ liệu.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS03', N'SQL Server thực hành', N'Lê Hoàng C', N'Hướng dẫn thực hành SQL Server.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS04', N'Cấu trúc dữ liệu và giải thuật', N'Phạm Quốc D', N'Tổng hợp các cấu trúc dữ liệu và thuật toán cơ bản.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS05', N'Lập trình hướng đối tượng', N'Võ Thanh E', N'Giới thiệu các nguyên lý của lập trình hướng đối tượng.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS06', N'Mạng máy tính', N'Nguyễn Minh F', N'Các kiến thức nền tảng về mạng máy tính.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS07', N'Hệ điều hành', N'Trần Quốc G', N'Tổng quan về hệ điều hành và quản lý tài nguyên.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS08', N'Trí tuệ nhân tạo', N'Lê Minh H', N'Kiến thức nhập môn về trí tuệ nhân tạo.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS09', N'Công nghệ phần mềm', N'Phạm Thành I', N'Quy trình phát triển và quản lý phần mềm.')
INSERT [dbo].[Tuasach] ([ma_tuasach], [tuasach], [tacgia], [tomtat]) VALUES (N'TS10', N'An toàn thông tin', N'Hoàng Quốc K', N'Các nguyên lý cơ bản về bảo mật và an toàn thông tin.')
GO
-- ============================================================
-- NHẬP DỮ LIỆU: Treem
-- Mục đích: Xác định mỗi độc giả trẻ em được người lớn nào bảo lãnh.
-- ============================================================
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE001', N'DG001')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE002', N'DG001')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE003', N'DG002')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE004', N'DG003')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE005', N'DG004')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE006', N'DG005')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE007', N'DG006')
INSERT [dbo].[Treem] ([ma_DocGia], [ma_DocGia_nguoilon]) VALUES (N'TE008', N'DG008')
GO
-- DEFAULT: tien_muon
-- Nếu không nhập tiền mượn thì SQL Server tự gán giá trị 0.
ALTER TABLE [dbo].[QuaTrinhMuon] ADD  DEFAULT ((0)) FOR [tien_muon]
GO
-- DEFAULT: tien_datra
-- Nếu không nhập tiền đã trả thì SQL Server tự gán giá trị 0.
ALTER TABLE [dbo].[QuaTrinhMuon] ADD  DEFAULT ((0)) FOR [tien_datra]
GO
-- DEFAULT: tien_datcoc
-- Nếu không nhập tiền đặt cọc thì SQL Server tự gán giá trị 0.
ALTER TABLE [dbo].[QuaTrinhMuon] ADD  DEFAULT ((0)) FOR [tien_datcoc]
GO
-- KHÓA NGOẠI: FK_Cuonsach_Dausach
-- Bảo đảm mỗi isbn trong Cuonsach phải tồn tại trong Dausach.
ALTER TABLE [dbo].[Cuonsach]  WITH CHECK ADD  CONSTRAINT [FK_Cuonsach_Dausach] FOREIGN KEY([isbn])
REFERENCES [dbo].[Dausach] ([isbn])
GO
ALTER TABLE [dbo].[Cuonsach] CHECK CONSTRAINT [FK_Cuonsach_Dausach]
GO
-- KHÓA NGOẠI: FK_DangKy_Dausach
-- Bảo đảm đầu sách được đăng ký phải tồn tại trong bảng Dausach.
ALTER TABLE [dbo].[DangKy]  WITH CHECK ADD  CONSTRAINT [FK_DangKy_Dausach] FOREIGN KEY([isbn])
REFERENCES [dbo].[Dausach] ([isbn])
GO
ALTER TABLE [dbo].[DangKy] CHECK CONSTRAINT [FK_DangKy_Dausach]
GO
-- KHÓA NGOẠI: FK_DangKy_DocGia
-- Bảo đảm độc giả đăng ký phải tồn tại trong bảng DocGia.
ALTER TABLE [dbo].[DangKy]  WITH CHECK ADD  CONSTRAINT [FK_DangKy_DocGia] FOREIGN KEY([ma_DocGia])
REFERENCES [dbo].[DocGia] ([ma_DocGia])
GO
ALTER TABLE [dbo].[DangKy] CHECK CONSTRAINT [FK_DangKy_DocGia]
GO
-- KHÓA NGOẠI: FK_Dausach_Tuasach
-- Bảo đảm mỗi đầu sách phải liên kết tới một tựa sách đã tồn tại.
ALTER TABLE [dbo].[Dausach]  WITH CHECK ADD  CONSTRAINT [FK_Dausach_Tuasach] FOREIGN KEY([ma_tuasach])
REFERENCES [dbo].[Tuasach] ([ma_tuasach])
GO
ALTER TABLE [dbo].[Dausach] CHECK CONSTRAINT [FK_Dausach_Tuasach]
GO
-- KHÓA NGOẠI: FK_Muon_Cuonsach
-- Bảo đảm cuốn sách được mượn phải tồn tại trong bảng Cuonsach.
ALTER TABLE [dbo].[Muon]  WITH CHECK ADD  CONSTRAINT [FK_Muon_Cuonsach] FOREIGN KEY([isbn], [ma_cuonsach])
REFERENCES [dbo].[Cuonsach] ([isbn], [ma_cuonsach])
GO
ALTER TABLE [dbo].[Muon] CHECK CONSTRAINT [FK_Muon_Cuonsach]
GO
-- KHÓA NGOẠI: FK_Muon_DocGia
-- Bảo đảm người đang mượn phải là độc giả có trong bảng DocGia.
ALTER TABLE [dbo].[Muon]  WITH CHECK ADD  CONSTRAINT [FK_Muon_DocGia] FOREIGN KEY([ma_DocGia])
REFERENCES [dbo].[DocGia] ([ma_DocGia])
GO
ALTER TABLE [dbo].[Muon] CHECK CONSTRAINT [FK_Muon_DocGia]
GO
-- KHÓA NGOẠI: FK_Nguoilon_DocGia
-- Bảo đảm người lớn cũng phải tồn tại trong bảng thông tin chung DocGia.
ALTER TABLE [dbo].[Nguoilon]  WITH CHECK ADD  CONSTRAINT [FK_Nguoilon_DocGia] FOREIGN KEY([ma_DocGia])
REFERENCES [dbo].[DocGia] ([ma_DocGia])
GO
ALTER TABLE [dbo].[Nguoilon] CHECK CONSTRAINT [FK_Nguoilon_DocGia]
GO
-- KHÓA NGOẠI: FK_QuaTrinhMuon_Cuonsach
-- Bảo đảm cuốn sách xuất hiện trong lịch sử mượn phải tồn tại trong Cuonsach.
ALTER TABLE [dbo].[QuaTrinhMuon]  WITH CHECK ADD  CONSTRAINT [FK_QuaTrinhMuon_Cuonsach] FOREIGN KEY([isbn], [ma_cuonsach])
REFERENCES [dbo].[Cuonsach] ([isbn], [ma_cuonsach])
GO
ALTER TABLE [dbo].[QuaTrinhMuon] CHECK CONSTRAINT [FK_QuaTrinhMuon_Cuonsach]
GO
-- KHÓA NGOẠI: FK_QuaTrinhMuon_DocGia
-- Bảo đảm độc giả trong lịch sử mượn phải tồn tại trong DocGia.
ALTER TABLE [dbo].[QuaTrinhMuon]  WITH CHECK ADD  CONSTRAINT [FK_QuaTrinhMuon_DocGia] FOREIGN KEY([ma_DocGia])
REFERENCES [dbo].[DocGia] ([ma_DocGia])
GO
ALTER TABLE [dbo].[QuaTrinhMuon] CHECK CONSTRAINT [FK_QuaTrinhMuon_DocGia]
GO
-- KHÓA NGOẠI: FK_Treem_DocGia
-- Bảo đảm trẻ em cũng phải có thông tin chung trong bảng DocGia.
ALTER TABLE [dbo].[Treem]  WITH CHECK ADD  CONSTRAINT [FK_Treem_DocGia] FOREIGN KEY([ma_DocGia])
REFERENCES [dbo].[DocGia] ([ma_DocGia])
GO
ALTER TABLE [dbo].[Treem] CHECK CONSTRAINT [FK_Treem_DocGia]
GO
-- KHÓA NGOẠI: FK_Treem_Nguoilon
-- Bảo đảm mã người bảo lãnh của trẻ em phải tồn tại trong bảng Nguoilon.
ALTER TABLE [dbo].[Treem]  WITH CHECK ADD  CONSTRAINT [FK_Treem_Nguoilon] FOREIGN KEY([ma_DocGia_nguoilon])
REFERENCES [dbo].[Nguoilon] ([ma_DocGia])
GO
ALTER TABLE [dbo].[Treem] CHECK CONSTRAINT [FK_Treem_Nguoilon]
GO
