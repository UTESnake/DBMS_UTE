# Kiểm thử SQL Bài 1–10

Các file `Nhom_*/*Testcase.sql` hiện đăng ký **291 testcase mô tả/đầu vào**,
không trùng mã. Một phần trong số đó là kịch bản WinForms hoặc trạng thái dữ
liệu giả lập nên không tự kết luận PASS/FAIL.

Bộ oracle tự động nằm tại `tools/Invoke-SqlAudit.ps1`. Bộ này:

- tạo bốn database tạm có hậu tố GUID trên SQL Server;
- nạp toàn bộ script schema, procedure, function, trigger và testcase;
- chạy assertion dương, âm và biên cho đủ Bài 1–10;
- chạy các mutation trong transaction rồi rollback;
- luôn xóa bốn database tạm trong khối `finally`.

Chạy từ thư mục gốc dự án:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-SqlAudit.ps1
```

Nếu SQL Server không phải instance mặc định của dự án:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-SqlAudit.ps1 -Server '.\SQLEXPRESS02'
```

Kết quả kiểm tra ngày 19/09/2026 trên SQL Server 2025 Express:

```text
TOTAL=62 PASS=62 FAIL=0
```

Các sửa chữa quan trọng được khóa bằng testcase gồm: delta âm sát 0 ở Bài 2;
`fn_TinhTuoi` nhận ngày sinh đầy đủ, xét mốc sinh nhật và trả số/NULL ở Bài 4; mốc quá hạn và mượn trùng cuốn ở Bài 5–6;
phân bổ lương theo tỷ trọng giờ và hai TVF tương đương ở Bài 7; dữ liệu giới
tính/phòng 5 ở Bài 8; ràng buộc gara và cập nhật nhiều dòng ở Bài 9; tham số
học kỳ/tên môn cùng các đường INSERT/UPDATE trigger ở Bài 10.
