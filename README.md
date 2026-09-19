# Tài liệu ôn vấn đáp đồ án Cơ sở dữ liệu — Bài 1 đến Bài 10

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
- `CAST(... AS NVARCHAR(50))` chuyển số thành chuỗi để ghép thông báo.

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
- Nếu `a<>0`: tính `Delta = b*b - 4*a*c`.
- `Delta < 0`: vô nghiệm trong tập số thực.
- `ABS(Delta) <= 0.0000001`: xem như Delta bằng 0 để tránh sai số `FLOAT`, nghiệm kép `-b/(2a)`.
- `Delta > 0`: hai nghiệm dùng `SQRT(Delta)`.

### Cách gọi

```sql
SELECT dbo.fn_GiaiPTB2(1, -3, 2) AS KetQua;
```

Function vô hướng phải được gọi trong `SELECT`, không dùng `EXEC` như procedure.

### Testcase

- `a=0,b=0,c=0`; `a=0,b=0,c<>0`; `a=0,b<>0`.
- Delta âm, bằng 0, dương.
- Hai nghiệm nguyên, nghiệm thập phân, hệ số âm, `b=0`, `c=0`.
- Delta rất gần 0 để kiểm tra epsilon.
- `NULL`, rỗng, chữ, khoảng trắng, số quá lớn.

### Câu hỏi dễ gặp

- **Procedure và scalar function khác nhau thế nào?** Function trả một giá trị và dùng trong biểu thức `SELECT`; function bị hạn chế tác dụng phụ. Procedure được `EXEC`, có thể trả result set và thực hiện nhiều thao tác.
- **Vì sao không so sánh `Delta = 0`?** `FLOAT` có sai số biểu diễn nên dùng một ngưỡng epsilon.

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

Giá trị tình trạng phải đồng nhất trong dữ liệu. Procedure hiện đếm chuỗi `Có sẵn`, trong khi trigger Bài 6 dùng `yes/no`. Khi triển khai thật nên chuẩn hóa bằng `BIT` hoặc một `CHECK CONSTRAINT` với duy nhất một quy ước.

---

## Bài 4 — Function tính tuổi

### Giải thích SQL

- Function nhận ngày sinh đầy đủ bằng kiểu `DATE` và lấy ngày hiện tại trên SQL Server.
- Kiểm tra `NULL` và ngày sinh lớn hơn ngày hiện tại.
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

- Dùng `EXISTS` để tránh nhân bản người lớn khi có nhiều phiếu mượn.
- `DATEDIFF(DAY, ngay_hethan, CAST(GETDATE() AS DATE)) > 14` xác định quá hạn hơn 14 ngày.

Testcase biên quan trọng: chưa tới hạn, đúng hạn, quá 1 ngày, đúng 14 ngày, 15 ngày, nhiều cuốn có ít nhất một cuốn quá hạn, ngày hết hạn `NULL`, ngày tương lai.

### 5e — `sp_DocGiaCoTreEmMuon`

- Join người lớn đang mượn với `Treem` qua `ma_DocGia_nguoilon`.
- Join tiếp độc giả trẻ em và `Muon` của trẻ.
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

- `B7_PhongBan`: phòng ban.
- `B7_NhanVien`: nhân viên, lương và phòng.
- `B7_DeAn`: đề án do phòng quản lý.
- `B7_PhanCong`: quan hệ nhiều-nhiều nhân viên–đề án, có số giờ.
- `B7_ThanNhan`: người thân của nhân viên.
- Khóa ngoại bảo đảm không thể phân công nhân viên/đề án không tồn tại.

### 7.1 — Lương trung bình một phòng

`AVG(Luong)` kết hợp `WHERE MaPB=@MaPB`. Phòng không tồn tại hoặc không có nhân viên trả `NULL`.

Testcase: PB hợp lệ, phòng không người, mã không tồn tại, rỗng/NULL, phòng có một/nhiều nhân viên.

### 7.2 — Tổng lương nhân viên theo đề án

Join `PhanCong` với `NhanVien`; công thức hiện dùng `Luong * SoGio / 160`, coi 160 giờ là một tháng chuẩn. Không có phân công phù hợp thì trả `NULL`.

Testcase: có/không phân công, nhân viên không tồn tại, đề án không tồn tại, 0 giờ, số giờ lớn.

### 7.3 — Tổng lương trung bình các phòng

Subquery `GROUP BY MaPB` tính trung bình từng phòng; query ngoài dùng `SUM` cộng các mức trung bình. Nhân viên chưa có phòng bị loại.

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
- `COUNT(da.MaDA)` trả 0 cho phòng không có đề án; không dùng `COUNT(*)` vì sẽ đếm cả dòng bên trái.

### 7.6 — Hai loại table-valued function

- Inline TVF: `RETURNS TABLE AS RETURN (SELECT...)`; ngắn, optimizer dễ tối ưu như một view có tham số.
- Multistatement TVF: khai báo biến bảng `@K`, `INSERT @K`, rồi `RETURN`; linh hoạt cho nhiều bước nhưng thường ước lượng cardinality kém hơn.
- `STRING_AGG` ghép nhiều người thân thành một chuỗi.
- `LEFT JOIN ThanNhan` giữ cả nhân viên không có người thân.

Testcase: nhân viên có một/nhiều/không có người thân, Unicode, so sánh kết quả hai TVF phải tương đương.

### Thứ tự cài đặt Bài 7

1. `Nhom_3_CSDL_DeAn/01_TaoBang_NhapDuLieu.sql`.
2. `Nhom_3_CSDL_DeAn/02_Functions.sql`.
3. `Nhom_3_CSDL_DeAn/03_Testcase.sql`.

WinForms cũng nhúng ba script này và tự cài khi kết nối nếu chưa đủ 7 function.

---

## Bài 8 — Function thống kê CSDL Đề án

### 8.1 — Dự án có hơn 2 nhân viên

Join `B7_DeAn` với `B7_PhanCong`, nhóm theo dự án và dùng `HAVING COUNT(DISTINCT MaNV)>2`. `DISTINCT` giúp mỗi nhân viên chỉ được đếm một lần.

### 8.2 — Phòng có hơn 2 nhân viên, đếm người lương trên 25000

- `HAVING COUNT(MaNV)>2` lọc phòng đủ tổng số nhân viên.
- `SUM(CASE WHEN Luong>25000 THEN 1 ELSE 0 END)` chỉ đếm nhân viên thỏa điều kiện lương.

### 8.3 và 8.4 — Phòng có lương trung bình trên 30000

Hai function đều dùng `HAVING AVG(Luong)>30000`. Câu 8.3 đếm tất cả nhân viên; câu 8.4 chỉ đếm nam bằng `SUM(CASE WHEN GioiTinh=N'Nam'...)`.

### 8.5 — Nhân viên phòng 5 tham gia từng dự án

Dùng chuỗi `LEFT JOIN` để mọi dự án đều xuất hiện. `COUNT(DISTINCT CASE WHEN MaPB='PB05' THEN MaNV END)` trả 0 nếu dự án không có nhân viên phòng 5.

Testcase: nhóm vừa đủ 2 người phải bị loại, nhóm 3 người được chọn, lương đúng 25000 không thuộc điều kiện `>25000`, phòng không có nam, dự án không có phân công và dự án không có nhân viên phòng 5.

---

## Bài 9 — Quản lý sửa chữa và bảo trì xe

### Ràng buộc dữ liệu

- PK định danh thợ, công việc, khách hàng, hợp đồng và phiếu thu.
- FK bảo đảm chi tiết hợp đồng tham chiếu đúng hợp đồng/công việc/thợ; phiếu thu phải đúng cặp hợp đồng–khách hàng.
- `UNIQUE(NgayHD,SoXe)` không cho một xe ký hai hợp đồng trong cùng ngày.
- `CHECK` chặn trị giá âm, ngày giao trước ngày ký và tiền khoán lớn hơn trị giá công việc.
- `tg_B9_KiemTraNhomTruong` bảo đảm nhóm trưởng là một người thợ thuộc chính nhóm đó.

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

### Các function 10.2

- 10.2.a join giáo viên–môn học và lọc `SoTiet>=45`.
- 10.2.b dùng `DISTINCT` vì một giáo viên có thể gác nhiều buổi trong học kỳ 1.
- 10.2.c dùng `NOT EXISTS` tìm giáo viên không có phân công học kỳ 1.
- 10.2.d tìm lịch thi theo tên môn `VĂN HỌC`.
- 10.2.e đi từ môn chủ nhiệm → giáo viên → phân công → buổi thi để liệt kê các buổi gác của giáo viên chủ nhiệm Văn.

Testcase ràng buộc nên bọc trong transaction: thử phân công giáo viên gác đúng môn mình chủ nhiệm; tạo môn 30 tiết nhưng thi khác 120 phút; tạo môn 45 tiết nhưng thi khác 150 phút; sau đó `ROLLBACK`.

### Thứ tự cài đặt và chạy

- Giữ nguyên cấu trúc từng bài tại `DATABASE/Bai_08`, `DATABASE/Bai_09` và `DATABASE/Bai_10`.
- Mỗi thư mục bài có `01_TaoBang_NhapDuLieu.sql` và `02_Functions.sql`.
- Testcase Bài 8–10 được gom riêng trong `DATABASE/Nhom_4_Testcase`; ứng dụng không nạp các file này khi kết nối.
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
- Bài nghiệp vụ có thể chạy theo luồng kết nối → load testcase → chạy testcase; các câu thống kê của Bài 8–10 chỉ dùng nút **Thống kê CSDL** và không nạp testcase.
- Khi giáo viên hỏi hạn chế, trả lời trung thực: Bài 4 lấy ngày hiện tại của SQL Server; Bài 3/5/6 cần thống nhất miền giá trị tình trạng sách; Bài 9.3 dùng điều kiện “trước ngày” (`<`) chứ không gồm chính ngày mốc.
