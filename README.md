# Tài liệu ôn vấn đáp đồ án Cơ sở dữ liệu — Bài 1 đến Bài 10

## Chọn testcase và nhập truy vấn trên các form

Các form Bài 1–9 cho chọn testcase từ combobox và nhập hoặc sửa dữ liệu trước khi chạy. Bài 1, 2, 4 có thể chọn ngay từ danh mục đóng gói; nút **Load Testcase** vẫn nạp bảng kiểm thử từ CSDL. Bài 3, 5, 6 điền dữ liệu của testcase vào các ô tra cứu hoặc kiểm tra trigger. Bài 7, 8 dùng danh mục Đề án; Bài 9 có ô lọc kết quả nhập tay bên cạnh danh mục testcase. Các ca mô tả nhiều thao tác hoặc kiểm tra trạng thái hiển thị kỳ vọng để đối chiếu và có thể cần chỉnh ô nhập trước khi chạy. Form Bài 10 chỉ chọn yêu cầu rồi chạy truy vấn hoặc kiểm chứng ràng buộc.

## Cách trình bày chung khi vấn đáp

Với mỗi bài nên nói theo thứ tự: **đề bài → dữ liệu vào → xử lý SQL → kết quả → testcase biên → cách WinForms gọi SQL**.

- `USE database`: chọn CSDL mà đối tượng SQL sẽ được tạo và thực thi.
- `GO`: dấu phân cách batch của công cụ SQL, không phải câu lệnh T-SQL gửi tới Server.
- `CREATE OR ALTER`: tạo mới nếu chưa tồn tại, cập nhật nếu đã tồn tại; giúp script chạy lại được.
- `SET NOCOUNT ON`: không gửi thông báo “n dòng bị ảnh hưởng”, tránh gây nhiễu khi ứng dụng đọc kết quả.
- `@Ten`: biến hoặc tham số SQL.
- `NVARCHAR` và chuỗi `N'...'`: lưu Unicode, cần thiết cho tiếng Việt.
- `DBNull.Value`: giá trị `NULL` của SQL khi gọi từ C#; khác chuỗi rỗng `""`.
- Mọi input từ WinForms phải truyền bằng `SqlParameter`, không nối chuỗi SQL, để đúng kiểu dữ liệu và tránh SQL injection.

---

## Bài 1 — Stored procedure giải phương trình bậc nhất

### Yêu cầu

Giải `ax + b = 0` với `a`, `b` bất kỳ bằng stored procedure `dbo.sp_GiaiPTB1`.

### Giải thích SQL

- Procedure nhận `@a FLOAT`, `@b FLOAT`.
- `CASE` phân nhánh và trả một dòng, một cột `KetQua`.
- `@a IS NULL OR @b IS NULL`: không đủ dữ liệu.
- `a = 0, b = 0`: đẳng thức `0 = 0`, vô số nghiệm.
- `a = 0, b <> 0`: mâu thuẫn `b = 0`, vô nghiệm.
- `a <> 0, b = 0`: nghiệm bằng 0.
- Trường hợp còn lại: `x = -b/a`.
- `FORMAT(..., 'G17', 'en-US')` hiển thị nghiệm số thực; nghiệm âm không được chuẩn hóa thành `0`.

### Cách gọi

```sql
EXEC dbo.sp_GiaiPTB1 @a = 2, @b = -4;
```

WinForms dùng `CommandType.StoredProcedure`, thêm hai parameter rồi đọc `reader["KetQua"]`.

### Testcase cần nhớ

1. `a=0,b=0` — vô số nghiệm.
2. `a=0,b=5` — vô nghiệm.
3. `a=2,b=-4` — nghiệm `2`.
4. `a=-2,b=8`; `a=-2,b=-8` — kiểm tra dấu.
5. `b=0` — nghiệm 0.
6. Số thập phân, số rất lớn/rất nhỏ.
7. `NULL`, rỗng, khoảng trắng, chữ, phân số nhập dạng `3/2` — validation WinForms.

### Câu hỏi dễ gặp

- **Vì sao procedure thay vì function?** Đề yêu cầu stored procedure; procedure phù hợp thao tác nghiệp vụ và trả result set.
- **Rủi ro của FLOAT?** Là số gần đúng; so sánh bằng 0 có thể sai với kết quả tính toán. Với dữ liệu tài chính nên dùng `DECIMAL`.

---

## Bài 2 — Function giải phương trình bậc hai

### Yêu cầu và xử lý

Function `dbo.fn_GiaiPTB2(@a,@b,@c)` trả `NVARCHAR(255)`.

- Nếu có tham số `NULL`: trả thông báo thiếu dữ liệu.
- Nếu `a=0`: phương trình suy biến thành `bx+c=0`, xử lý giống Bài 1.
- Nếu `a<>0`: chia cả ba hệ số cho độ lớn lớn nhất rồi mới tính biệt thức, tránh tràn `b*b`.
- `Delta < 0`: vô nghiệm trong tập số thực.
- `Delta = 0`: nghiệm kép `-b/(2a)`; biệt thức âm rất nhỏ vẫn là vô nghiệm thực.
- `Delta > 0`: tính nghiệm lớn bằng công thức ổn định, nghiệm còn lại qua tích nghiệm `c/a` để tránh triệt tiêu.

### Cách gọi

```sql
SELECT dbo.fn_GiaiPTB2(1, -3, 2) AS KetQua;
```

Function vô hướng phải được gọi trong `SELECT`, không dùng `EXEC` như procedure.

### Testcase

- `a=0,b=0,c=0`; `a=0,b=0,c<>0`; `a=0,b<>0`.
- Delta âm, bằng 0, dương.
- Hai nghiệm nguyên, nghiệm thập phân, hệ số âm, `b=0`, `c=0`.
- Delta dương/âm rất gần 0, `b=1e160` và `b=1e16,c=1`.
- `NULL`, rỗng, chữ, khoảng trắng, số quá lớn.

### Câu hỏi dễ gặp

- **Procedure và scalar function khác nhau thế nào?** Function trả một giá trị và dùng trong biểu thức `SELECT`; function bị hạn chế tác dụng phụ. Procedure được `EXEC`, có thể trả result set và thực hiện nhiều thao tác.
- **Vì sao không dùng epsilon cố định?** Nó có thể biến biệt thức âm rất nhỏ thành nghiệm kép sai; testcase kiểm tra hai phía của mốc 0.

---

## Bài 3 — Tra cứu thông tin đầu sách

### Yêu cầu

`sp_ThongTinDauSach(@ISBN)` trả thông tin `Dausach`, `Tuasach` và số cuốn hiện chưa mượn.

### Giải thích SQL

- `LTRIM(RTRIM(@ISBN))`: bỏ khoảng trắng đầu/cuối.
- `IF NOT EXISTS (SELECT 1...)`: kiểm tra ISBN tồn tại trước khi truy vấn.
- `RAISERROR(...,16,1)`: phát sinh lỗi nghiệp vụ để C# bắt bằng `SqlException`.
- `INNER JOIN Tuasach`: mỗi đầu sách phải có tựa sách tương ứng.
- `LEFT JOIN Cuonsach`: vẫn giữ đầu sách kể cả chưa có cuốn sách nào.
- `SUM(CASE WHEN tinhtrang=... THEN 1 ELSE 0 END)`: đếm có điều kiện.
- `GROUP BY`: gom các cuốn theo một đầu sách để dùng `SUM`.

### Cách gọi

```sql
EXEC dbo.sp_ThongTinDauSach @ISBN = 'ISBN001';
```

### Testcase

- ISBN tồn tại có 0, 1, nhiều cuốn sẵn sàng.
- ISBN không tồn tại, `NULL`, rỗng, toàn khoảng trắng.
- Khoảng trắng đầu/cuối, chữ hoa/thường, ký tự đặc biệt, quá độ dài.
- Đầu sách không có dòng `Cuonsach` để kiểm tra `LEFT JOIN`.
- Unicode ở tên sách/tác giả/tóm tắt.
- Chưa kết nối, chưa chọn ComboBox, làm mới giao diện.

### Điểm cần chú ý khi trả lời

Procedure và trigger đều dùng trạng thái `Có sẵn` để xác định cuốn được mượn. Cuốn `Hỏng` hoặc `Mất` không được cộng vào số có sẵn; ISBN được so khớp không phân biệt chữ hoa/thường.

---

## Bài 4 — Function tính tuổi

### Giải thích SQL

- Function nhận ngày sinh đầy đủ bằng kiểu `DATE` và lấy ngày hiện tại trên SQL Server.
- Kiểm tra `NULL` và ngày sinh lớn hơn ngày hiện tại.
- Tuổi trên 120 trả `NULL` và form báo giới hạn; sinh trong ngày trả 0 tuổi.
- Dùng `DATEDIFF(YEAR, ...)`, sau đó trừ 1 nếu sinh nhật năm hiện tại chưa đến.

```sql
SELECT dbo.fn_TinhTuoi(CAST('2000-09-20' AS DATE)) AS KetQua;
```

### Testcase

- Sinh nhật hôm nay, trước sinh nhật, sau sinh nhật và ngày tương lai.
- `NULL`, rỗng, chữ, ngày không tồn tại và chỉ nhập năm.
- Ngày nhuận hợp lệ `29/02/2000` và không hợp lệ `29/02/2001`.

### Câu hỏi dễ gặp

- **Vì sao không chỉ lấy năm hiện tại trừ năm sinh?** Vì người dùng có thể chưa đến sinh nhật trong năm hiện tại; function phải so sánh cả ngày và tháng.

---

## Bài 5 — Các stored procedure quản lý thư viện

### 5a — `sp_ThongtinDocGia`

- Kiểm tra mã rỗng và không tồn tại.
- Dùng `EXISTS` xác định độc giả thuộc `Nguoilon` hay `Treem`.
- Người lớn: join `DocGia` với `Nguoilon` để lấy địa chỉ, điện thoại, hạn sử dụng.
- Trẻ em: join `DocGia` với `Treem`, trả thêm mã người lớn bảo lãnh.
- Nếu có `DocGia` nhưng không thuộc hai bảng phân loại thì báo dữ liệu không hợp lệ.

Testcase: người lớn, trẻ em, không tồn tại, `NULL`/rỗng/khoảng trắng, mã dài/ký tự đặc biệt, Unicode, người lớn hết hạn, trẻ em thiếu người bảo lãnh, dữ liệu phân loại mâu thuẫn.

### 5b — `sp_ThongtinDausach`

Logic tương tự Bài 3: validate ISBN, join tựa sách, left join cuốn sách, đếm cuốn chưa mượn.

Testcase: ISBN hợp lệ/không tồn tại, không có cuốn, tất cả đang mượn, nhiều cuốn, Unicode, dữ liệu đầu sách thiếu tựa sách.

### 5c — `sp_ThongtinNguoilonDangmuon`

- Join `DocGia → Nguoilon → Muon`.
- Chỉ người lớn có dòng mượn hiện tại mới xuất hiện.
- Một độc giả mượn nhiều cuốn có thể xuất hiện nhiều dòng nếu không gom nhóm.

Testcase: không ai mượn, một/nhiều người mượn, một người nhiều cuốn, trẻ em mượn nhưng không được trả trong danh sách người lớn, bản ghi trùng hoặc khóa ngoại sai.

### 5d — `sp_ThongtinNguoilonQuahan`

- Trả từng cuốn quá hạn, kèm số ngày `DATEDIFF(DAY, ngay_hethan, CAST(GETDATE() AS DATE))`.
- `ngay_hethan < hôm nay` mới là quá hạn; `CK_Muon_ThoiHan` chặn hạn trả sớm hơn ngày mượn.

Testcase biên quan trọng: chưa tới hạn, đúng hạn, quá 1 ngày, đúng 14 ngày, 15 ngày, nhiều cuốn có ít nhất một cuốn quá hạn, ngày hết hạn `NULL`, ngày tương lai.

### 5e — `sp_DocGiaCoTreEmMuon`

- Join `Nguoilon`, `Treem` qua `ma_DocGia_nguoilon` và lấy họ tên từ `DocGia`.
- Dùng hai điều kiện `EXISTS` trên `Muon` để kiểm tra từng người có mượn sách, không nhân bản cặp khi họ mượn nhiều cuốn.
- Trả `MaNguoiLon`, `HoTenNguoiLon`, `MaTreEm`, `HoTenTreEm`.
- Điều kiện cần đồng thời: người lớn đang mượn và ít nhất một trẻ do họ bảo lãnh cũng đang mượn.

Testcase: cả hai cùng mượn, chỉ người lớn mượn, chỉ trẻ mượn, nhiều trẻ, nhiều người bảo lãnh, quan hệ sai, dữ liệu Unicode.

### Câu hỏi dễ gặp

- **EXISTS khác JOIN?** `EXISTS` kiểm tra sự tồn tại và thường tránh tạo nhiều dòng kết quả; `JOIN` dùng khi cần lấy cột từ bảng liên quan.
- **LEFT JOIN khác INNER JOIN?** `INNER JOIN` chỉ giữ dòng khớp; `LEFT JOIN` giữ toàn bộ bảng trái và trả `NULL` nếu bên phải không có dữ liệu.

---

## Bài 6 — Trigger quản lý thư viện

### Kiến thức chung

Trigger SQL Server chạy **một lần cho mỗi câu lệnh**, không phải một lần cho mỗi dòng. Vì vậy phải xử lý `inserted`/`deleted` như bảng nhiều dòng, không gán vào một biến đơn.

### 6.1 — `tg_delMuon`

- Chạy `AFTER DELETE` trên `Muon`.
- Bảng ảo `deleted` chứa các phiếu mượn vừa xóa.
- Join `Cuonsach` với `deleted` bằng ISBN và mã cuốn, cập nhật tình trạng thành `yes`.

Testcase: xóa một dòng, nhiều dòng, dòng không tồn tại; một độc giả mượn nhiều cuốn; xác nhận chỉ cuốn bị xóa đổi trạng thái.

### 6.2 — `tg_insMuon`

- Chạy `AFTER INSERT`.
- `inserted` chứa phiếu vừa thêm.
- Cập nhật đúng các cuốn liên quan thành `no`.

Testcase: thêm hợp lệ, nhiều dòng cùng lúc, cuốn/ISBN/độc giả không tồn tại, mượn trùng, ngày không hợp lệ, ngày hết hạn trước ngày mượn.

### 6.3 — `tg_updCuonSach`

- `IF NOT UPDATE(tinhtrang) RETURN`: bỏ qua khi câu UPDATE không đụng cột tình trạng.
- CTE/union lấy các ISBN bị ảnh hưởng từ cả `inserted` và `deleted`.
- `EXISTS` kiểm tra ISBN còn cuốn `yes` hay không rồi cập nhật trạng thái `Dausach`.

Testcase: đổi `no→yes`, `yes→no`, cuốn cuối cùng, vẫn còn cuốn `yes`, update nhiều cuốn cùng/multiple ISBN, update cột khác.

### 6.4 — `tg_InfThongBao`

- Chạy sau `INSERT, UPDATE` trên `Tuasach`.
- Insert được nhận biết khi dòng có trong `inserted` nhưng không có trong `deleted`.
- `UPDATE(tacgia)` và `UPDATE(tuasach)` cho biết câu lệnh có cập nhật cột đó.
- `PRINT` gửi thông báo tiếng Việt; C# nhận qua sự kiện `InfoMessage`.

Testcase: insert một/nhiều dòng, sửa tựa sách, sửa tác giả, sửa cả hai, chỉ sửa tóm tắt, khóa chính trùng, dòng không tồn tại.

### Vì sao dùng transaction khi test?

```sql
BEGIN TRAN;
-- INSERT / UPDATE / DELETE để kích hoạt trigger
ROLLBACK;
```

Trigger vẫn chạy và có thể kiểm tra kết quả trong transaction, nhưng `ROLLBACK` trả dữ liệu thật về trạng thái ban đầu.

---

## Bài 7 — Function CSDL Đề án

### Cấu trúc dữ liệu

- Sáu bảng `PHONGBAN`, `NHANVIEN`, `DEAN`, `PHANCONG`, `THANNHAN`, `DIADIEM_PHG` theo lược đồ trong ảnh, gồm khóa chính và khóa ngoại giữa phòng, nhân viên, đề án, phân công, thân nhân và địa điểm phòng.
- `NHANVIEN.Luong` là dữ liệu đầu vào để nạp bảng `BANGLUONG`; không còn bảng `LUONG` riêng. Các function tính lương Bài 7–8 đọc `BANGLUONG.LuongCoBan`.
- `01_TaoBang_NhapDuLieu.sql` tạo bảng tổng hợp `BANGLUONG` (mỗi nhân viên một dòng), view `v_BangLuongChiTiet` và thủ tục `sp_TinhVaCapNhatBangLuong`. Bảng có họ tên, phòng, lương cơ bản, tổng giờ phân công, thưởng, lương trung bình phòng, người thân, tổng thu nhập và hai ngày lương. `TongThuNhap = COALESCE(LuongCoBan,0) + TienThuong`; lương trung bình phòng chỉ để đối chiếu, không cộng thêm vào thu nhập. Người không có phân công nhận 0 giờ/0 thưởng; lương `NULL` được giữ nguyên trong `LuongCoBan` để `AVG` bỏ qua, còn tổng thu nhập dùng 0.
- View `v_LuongNhanVienTheoDeAn` hiển thị từng phân công và phần lương của câu 7.2 theo tỷ trọng giờ; form Bài 7 có mục dữ liệu liên quan **Lương theo đề án** để xem các dòng này.
- Tên cột theo sơ đồ: `PHONGBAN.MaPhg`, `NHANVIEN.Phg`, `DEAN.Phong`, `PHANCONG.SoDA`; bảng lương dùng `Time_Total`. Function Bài 7–8 truy vấn trực tiếp các bảng gốc; mã phòng và đề án vẫn là hai ký tự như `01`, `05`.
- Dữ liệu mẫu có phòng rỗng, phòng đúng 2/3/4 nhân viên, lương 0/25.000/NULL, mức trung bình đúng 30.000, phòng đạt trung bình nhưng không có nam, đề án có 0/2/3/5 nhân viên và phân công 0 giờ.

### 7.1 — Lương trung bình một phòng

`AVG(BANGLUONG.LuongCoBan)` theo `NHANVIEN.Phg=@MaPhg`. Hàm trả 0 khi phòng không tồn tại, rỗng hoặc không có mức lương; `AVG` bỏ qua mức lương `NULL`.

Testcase: phòng `01` trả 35.666,67; `03` trả 16.333,33 dù có lương 0 và NULL; `04` trả đúng 30.000; `00` và mã không tồn tại trả 0.

### 7.2 — Tổng lương nhân viên theo đề án

Đề không nêu công thức phân bổ lương theo đề án. Theo testcase đang dùng, hàm tính `BANGLUONG.LuongCoBan × ThoiGian của đề án / tổng ThoiGian của nhân viên`. Hàm trả 0 khi không tham gia, tổng giờ bằng 0 hoặc lương NULL.

Testcase: có/không phân công, nhân viên không tồn tại, đề án không tồn tại, 0 giờ, số giờ lớn.

### 7.3 — Tổng lương trung bình các phòng

Subquery `GROUP BY Phg` tính trung bình từng phòng; query ngoài cộng các mức trung bình. Nhân viên chưa có phòng và phòng rỗng không đóng góp. Dữ liệu mẫu trả 177.500.

Testcase: nhiều phòng, một phòng, phòng rỗng, nhân viên không thuộc phòng, không có dữ liệu.

### 7.4 — Tiền thưởng theo tổng giờ

`CASE` theo đúng các khoảng:

- `<30` hoặc NULL: 0 USD.
- `30..60`: 500 USD.
- `>60 và <100`: 1000 USD.
- `100..149.x`: 1200 USD.
- `>=150`: 1600 USD.

Testcase quan trọng nhất: 29, 30, 60, 61, 99, 100, 149, 150, số âm, NULL và số thập phân sát biên.

### 7.5 — Số đề án theo mỗi phòng

- `LEFT JOIN` để phòng chưa có đề án vẫn xuất hiện.
- `COUNT(da.MaDA)` trả 0 cho phòng không có đề án; không dùng `COUNT(*)` vì sẽ đếm cả dòng bên trái. Dữ liệu mẫu trả 7 phòng.

### 7.6 — Hai loại table-valued function

- Inline TVF: `RETURNS TABLE AS RETURN (SELECT...)`; ngắn, optimizer dễ tối ưu như một view có tham số.
- Multistatement TVF: khai báo biến bảng `@K`, `INSERT @K`, rồi `RETURN`; linh hoạt cho nhiều bước nhưng thường ước lượng cardinality kém hơn.
- `STRING_AGG` ghép nhiều người thân thành một chuỗi.
- `OUTER APPLY` lấy danh sách người thân và giữ cả nhân viên không có người thân. Hai TVF trả cùng 19 nhân viên.

Testcase: nhân viên có một/nhiều/không có người thân, Unicode, so sánh kết quả hai TVF phải tương đương.

### Thứ tự cài đặt Bài 7

1. `Nhom_3_CSDL_DeAn/01_TaoBang_NhapDuLieu.sql` để tạo dữ liệu và bảng lương tổng hợp.
2. `Bai_07/Functions_DeAn.sql` để tạo các function Bài 7.
3. `Bai_08/02_Functions.sql` để tạo các function Bài 8.
4. `Nhom_3_CSDL_DeAn/02_Testcase.sql` để tạo danh mục testcase và chạy các kiểm tra bổ sung Bài 7, Bài 8, bảng lương trong giao dịch rồi rollback.

Chạy script dữ liệu trước các script function. File `02_Testcase.sql` tạo danh mục và chạy kiểm tra, không tạo function.
Sau khi sửa `NHANVIEN.Luong`, phân công hoặc người thân, gọi `EXEC dbo.sp_TinhVaCapNhatBangLuong;` để cập nhật bảng vật lý trước khi chạy các function Bài 7–8. Có thể truyền `@NgayTinhLuong` để tính ngày nhận lương cùng ngày tháng sau, ví dụ `EXEC dbo.sp_TinhVaCapNhatBangLuong @NgayTinhLuong='2026-10-31';` cho ngày nhận `2026-11-30`. Chạy lại thủ tục cập nhật dòng hiện có và thêm nhân viên mới, không tạo dòng trùng.

### Sử dụng form Bài 7 và 8

- Form Bài 7 chọn function, nhập trực tiếp mã phòng, mã nhân viên, mã đề án hoặc tổng giờ rồi nhấn **Thực hiện**. Ca nhiều mốc giờ của 7.4 dùng dấu `;` và hiển thị từng mốc trong bảng kết quả.
- Form Bài 8 có combobox chọn testcase; nội dung kỳ vọng hiện cạnh testcase và nút thực hiện trả kết quả truy vấn từ SQL Server.
- Bài 8 cho phép nhập mã phòng hoặc mã đề án gồm hai chữ số để lọc; để trống sẽ hiện toàn bộ kết quả của function. Với testcase gồm nhiều mã, bảng kết quả hiển thị đầy đủ để đối chiếu.
- Hai form có thể chọn `BANGLUONG` ở phần dữ liệu CSDL liên quan để xem bảng lương đã nạp. Câu 8.1 trả bảng rỗng khi không có đề án đủ hơn 2 nhân viên, không phát sinh lỗi SQL.
- Form Bài 8 đóng gói danh mục testcase từ `02_Testcase.sql` nên vẫn chọn được ngay cả khi chưa cài bảng `Testcase_Nhom3`. Muốn truy vấn danh mục trong SQL Server, chạy bước 4 ở trên.

---

## Bài 8 — Function thống kê CSDL Đề án

### 8.1 — Dự án có hơn 2 nhân viên

Join `DEAN` với `PHANCONG`, nhóm theo dự án và dùng `HAVING COUNT(DISTINCT MaNV)>2`. Dữ liệu mẫu chọn đề án `01`, `03`, `05`.

### 8.2 — Phòng có hơn 2 nhân viên, đếm người lương trên 25000

- `HAVING COUNT(MaNV)>2` lọc phòng theo toàn bộ nhân viên, kể cả người có lương NULL.
- `SUM(CASE WHEN BANGLUONG.LuongCoBan>25000 THEN 1 ELSE 0 END)` đếm người đủ điều kiện. Phòng `03` có 4 nhân viên nhưng kết quả bằng 0; mức lương đúng 25.000 không được tính.

### 8.3 và 8.4 — Phòng có lương trung bình trên 30000

Hai function đều dùng `HAVING AVG(BANGLUONG.LuongCoBan)>30000`. Câu 8.3 đếm tất cả nhân viên; câu 8.4 chỉ đếm nam bằng `SUM(CASE WHEN Phai=N'Nam'...)`. Phòng `04` đúng 30.000 bị loại; phòng `06` xuất hiện với 0 nam.

### 8.5 — Nhân viên phòng 5 tham gia từng dự án

Dùng chuỗi `LEFT JOIN` để cả 6 đề án đều xuất hiện. `COUNT(DISTINCT CASE WHEN Phg='05' THEN MaNV END)` trả 0 nếu đề án không có nhân viên phòng 5.

Testcase: nhóm vừa đủ 2 người phải bị loại, nhóm 3 người được chọn, lương đúng 25000 không thuộc điều kiện `>25000`, phòng không có nam, dự án không có phân công và dự án không có nhân viên phòng 5.

---

## Bài 9 — Quản lý sửa chữa và bảo trì xe

### Ràng buộc dữ liệu

- Sáu PK định danh thợ, công việc, khách hàng, hợp đồng, chi tiết hợp đồng và phiếu thu; các FK giữ đúng quan hệ giữa những bảng này.
- FK ghép `(SoHD,MaKH)` trên phiếu thu bảo đảm khách hàng nộp tiền đúng là khách của hợp đồng.
- `UNIQUE(NgayHD,SoXe)` không cho một xe ký hai hợp đồng trong cùng ngày.
- `CHECK` yêu cầu `SoTienThu > 0`; `TriGiaHD`, `TriGiaCV`, `KhoanTho` không âm. Các `CHECK` khác chặn ngày giao trước ngày ký và chuỗi rỗng ở các cột văn bản. Điện thoại gồm 9–15 chữ số, có thể bắt đầu bằng một dấu `+`.
- Trigger trên `PHIEUTHU` và `HOPDONG` giữ `NgayLapPT >= NgayHD` khi thêm, sửa phiếu thu hoặc đổi ngày ký hợp đồng. Phiếu thu vẫn có thể lập trước hoặc sau ngày nghiệm thu.
- `tg_B9_KiemTraNhomTruong` và `tg_B9_KiemTraDoiNhomTruong` bảo đảm nhóm trưởng thuộc đúng nhóm và mọi thợ trong một nhóm có cùng nhóm trưởng.
- `TriGiaHD` bắt đầu bằng 0 khi tạo hợp đồng. Trigger trên `CHITIET_HD` cập nhật nó thành tổng `TriGiaCV` sau mỗi lần thêm, sửa hoặc xóa; trigger trên `HOPDONG` chặn việc sửa tổng tiền thành giá trị khác tổng chi tiết. HD02 trong dữ liệu mẫu có tổng đúng là 3.300.000.
- Trigger trên `PHIEUTHU` giữ tổng tiền thu không vượt `TriGiaHD`; trigger trên `HOPDONG` cũng chặn việc giảm trị giá xuống dưới số tiền đã thu. Tên công việc và số điện thoại có thể trùng; tiền khoán chỉ cần không âm theo phần ràng buộc này.

### Các function 9.1–9.5

- 9.1 dùng `NOT EXISTS` tìm thợ chưa từng được giao chi tiết công việc.
- 9.2 gom phiếu thu theo hợp đồng đã nghiệm thu, dùng `HAVING SUM(SoTienThu)<TriGiaHD` để tìm hợp đồng còn nợ.
- 9.3 nhận một ngày, chỉ lấy hợp đồng chưa nghiệm thu có ngày giao dự kiến trước mốc đó.
- 9.4 đếm công việc theo thợ và dùng `DENSE_RANK` lấy tất cả người đồng hạng cao nhất.
- 9.5 tương tự nhưng xếp hạng theo tổng `TriGiaCV`.

Testcase: thợ chưa có việc, hợp đồng đã thanh lý trả đủ/chưa đủ/chưa có phiếu thu, hợp đồng chưa nghiệm thu, ngày tham số rỗng, và trường hợp nhiều thợ đồng hạng.

---

## Bài 10 — Quản lý lịch thi trường phổ thông

### Ràng buộc 10.1

- Khóa chính của buổi thi là `(HKY,Ngay,Gio,Phg)`; phân công tham chiếu đủ bốn cột này.
- Unique `(MaGV,HKY,Ngay,Gio)` ngăn một giáo viên gác hai phòng cùng lúc.
- Unique có điều kiện trên `GV.MaMH` bảo đảm một môn chỉ có một giáo viên chủ nhiệm.
- `tg_B10_KiemTraPhanCong`, `tg_B10_KiemTraGiaoVien` và `tg_B10_KiemTraBuoiThi` cùng bảo vệ quy tắc giáo viên không gác môn mình chủ nhiệm, kể cả khi phân công hoặc môn học bị sửa sau đó.
- Trigger thời lượng bắt buộc môn 30 tiết thi 120 phút và môn từ 45 tiết thi 150 phút. Trigger trên `MHOC` ngăn sửa số tiết làm dữ liệu lịch thi cũ sai.
- Môn ngoài hai mốc trên (ví dụ 20 hoặc 40 tiết) nhận thời lượng dương do trường chọn; dữ liệu mẫu dùng 90 phút.
- `B10_LOP` và `B10_PHONG_LOP_THI` ghi lớp có mặt trong phòng; trigger trên phân công, phòng–lớp và lớp ngăn giáo viên chủ nhiệm coi thi phòng có học sinh lớp mình.

### Các function 10.2

- 10.2.a join giáo viên–môn học và lọc `SoTiet>=45`.
- 10.2.b là hàm không tham số, lọc cố định `HKY=1` và dùng `DISTINCT` vì một giáo viên có thể gác nhiều buổi trong học kỳ 1.
- 10.2.c là hàm không tham số, dùng `NOT EXISTS` với `HKY=1` để tìm giáo viên không có phân công học kỳ 1.
- 10.2.d là hàm không tham số, lọc cố định `TENMH=N'VĂN HỌC'` để lấy lịch thi môn Văn ở mọi học kỳ.
- 10.2.e là hàm không tham số; truy vấn con tìm `MaMH` của `VĂN HỌC`, sau đó lấy giáo viên chủ nhiệm và các buổi gác của họ. Bốn bảng trong phần `FROM` cung cấp các cột cần trả về; cách viết này chưa tự bảo đảm nhanh hơn nếu không so sánh execution plan.

Dữ liệu mẫu có GV01 gác bốn buổi khác môn chủ nhiệm trong HK1 và một buổi ở HK3; GV05 không chủ nhiệm môn nào và GV10 chỉ gác ở HK2. Hai hàm 10.2.b/c chỉ xét HK1, còn 10.2.e lấy đủ năm buổi gác của GV01 ở mọi học kỳ.

Testcase ràng buộc nên bọc trong transaction: thử phân công giáo viên gác đúng môn mình chủ nhiệm; tạo môn 30 tiết nhưng thi khác 120 phút; tạo môn 45 tiết nhưng thi khác 150 phút; sau đó `ROLLBACK`.

Các action SQL kiểm tra 10.1.a–c nằm trong `SOURCE_CODE_WINFORMS/Bai_10_TruongPhoThong/Form1.cs`. Ứng dụng chạy từng action trong transaction, đối chiếu đúng trigger và thông báo lỗi rồi rollback.

### Kiểm thử các ca biên bổ sung

Các danh mục testcase tại `DATABASE/Nhom_1_CSDL_ToanHoc/02_Testcase.sql`, `Nhom_2_CSDL_ThuVien/02_Testcase.sql`, `Nhom_3_CSDL_DeAn/02_Testcase.sql`, `Nhom_4_CSDL_Gara/03_Testcase.sql` và `Nhom_5_CSDL_TruongPhoThong/03_Testcase.sql` có các mã `EDGE` mới. Với Nhóm 3, kiểm tra bổ sung đã nằm ngay cuối `02_Testcase.sql`. Các nhóm còn lại dùng file kiểm tra bổ sung riêng trong thư mục tương ứng. Ca hai thủ thư mượn cùng cuốn cần chạy từ hai kết nối; `UQ_Muon_CuonDangMuon` quyết định chỉ một giao dịch được lưu.

Để kiểm tra đồng thời Bài 6, mở hai cửa sổ SQL trên `QL_ThuVien`. Cửa sổ A chạy `BEGIN TRAN; INSERT dbo.Muon VALUES('ISBN003','CS004','DG001',CAST(GETDATE() AS date),DATEADD(day,14,CAST(GETDATE() AS date)));` và giữ giao dịch mở. Cửa sổ B chạy cùng lệnh với độc giả `DG002`; lệnh B phải chờ. `COMMIT` ở A thì B phải bị unique key từ chối. Sau đó xóa dòng mượn thử của A để trả lại trạng thái cuốn sách.

### Thứ tự cài đặt và chạy

- Giữ nguyên cấu trúc từng bài tại `DATABASE/Bai_08`, `DATABASE/Bai_09` và `DATABASE/Bai_10`.
- Mỗi thư mục bài có `01_TaoBang_NhapDuLieu.sql` và `02_Functions.sql`.
- Danh mục testcase Bài 8 nằm trong `DATABASE/Nhom_3_CSDL_DeAn/02_Testcase.sql` và được đóng gói vào form. Bài Gara có danh mục testcase trên form; testcase Trường phổ thông nằm trong script SQL của nhóm và không hiển thị trên form Bài 10.
- Với câu thống kê: bấm **Kết nối CSDL** → chọn yêu cầu → **Thống kê CSDL**.
- Với câu ràng buộc của Bài 9 và 10: chọn yêu cầu tương ứng → **Kiểm chứng ràng buộc**. Dữ liệu thử luôn nằm trong transaction và được rollback.

---

## Checklist trước khi vấn đáp

- Biết bài nào dùng procedure, scalar function, table-valued function và trigger.
- Giải thích được `INNER JOIN`, `LEFT JOIN`, `EXISTS`, `GROUP BY`, `COUNT`, `SUM(CASE...)`.
- Phân biệt `inserted` và `deleted`.
- Giải thích vì sao trigger phải xử lý nhiều dòng.
- Biết `NULL` khác chuỗi rỗng và vì sao dùng parameter.
- Biết testcase bình thường, testcase biên, testcase dữ liệu sai và testcase giao diện.
- Bài nghiệp vụ có thể chạy theo luồng kết nối → chọn testcase → chạy testcase; Bài 8 còn cho phép nhập mã lọc và chọn testcase trong combobox.
- Khi giáo viên hỏi hạn chế, trả lời trung thực: Bài 4 lấy ngày hiện tại của SQL Server; Bài 3/5/6 cần thống nhất miền giá trị tình trạng sách; Bài 9.3 dùng điều kiện “trước ngày” (`<`) chứ không gồm chính ngày mốc.
