# Kiểm thử hành vi trường hợp đặc biệt

Chạy tại thư mục gốc bằng PowerShell, với SQL Server `.\SQLEXPRESS02`:

```powershell
dotnet build Do_An_Cuoi_Ky_CSDL.slnx
dotnet build tools/BehaviorChecks/BehaviorChecks.csproj
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Invoke-SqlAudit.ps1 -BehaviorChecks
```

Script tạo các CSDL `Audit_*`, cài dữ liệu mẫu, chạy kiểm thử rồi xóa các CSDL tạm trong `finally`. Không chạy kiểm thử thay đổi dữ liệu trên CSDL ứng dụng. Có thể truyền `-Server` để đổi SQL Server.

Các ca hồi quy bổ sung kiểm tra thông báo nghiệp vụ Bài 3/5, FK/CHECK/PK/UNIQUE và rollback Bài 6, validation và các biên tiền thưởng Bài 7, cùng việc không tính PASS nhầm khi Bài 9/10 gặp sai FK, trùng khóa hoặc trigger bị tắt. Bộ kiểm thử gọi mã helper và truy vấn của ứng dụng; không tự đóng hộp thoại WinForms.

Kiểm tra giao diện thủ công:

- Bài 3: nhập ISBN không tồn tại trực tiếp vào ComboBox; nhận thông báo tra cứu. Rỗng hoặc quá 20 ký tự được chặn trước SQL.
- Bài 5a: nhập mã không tồn tại; hộp thoại là **Thông báo tra cứu**, không phải **Lỗi**. Kết quả lần trước được xóa.
- Bài 6.2/6.4: chọn lỗi dự kiến ở ô **Kỳ vọng** trước khi chạy testcase âm. Chỉ lỗi đúng ràng buộc được chọn mới hiện **Đã chặn đúng / PASS**. Chấp nhận dữ liệu sai hoặc báo sai ràng buộc phải hiện **FAIL**. Khi chưa chọn kỳ vọng, dữ liệu vi phạm được giải thích bằng thông báo cảnh báo, không tính PASS.
- Bài 7: mã không tồn tại, giờ rỗng, `abc`, `NaN`, `Infinity`, âm hoặc quá phạm vi phải báo dữ liệu không hợp lệ; `0` giờ vẫn hợp lệ. Chấp nhận dấu phẩy hoặc dấu chấm thập phân, tối đa hai chữ số sau dấu phân cách.
- Bài 8: bảng rỗng phải hiện **Không có kết quả phù hợp**, không được kết luận kiểm thử đạt. Truy vấn thống kê có dữ liệu cũng không tự tính PASS.
- Bài 9/10: nút kiểm chứng chỉ hiện **Đạt** cho đúng lỗi từ đúng trigger; xem cột chi tiết khi **Không đạt**.
