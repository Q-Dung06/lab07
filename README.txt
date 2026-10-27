LAB 07 - TẠO CSDL SQL SERVER VÀ ADO.NET
Học phần: Lập trình .NET (IT3212)

DEMO NHANH
1. Cần Windows, Visual Studio 2022/.NET 10 và SQL Server 2019/2022 hoặc Express.
2. Mở Lab07_ADONET_Connection.sln. Project khởi động là Lab07_ADONET_Connection.
3. Mặc định kết nối đến .\SQLEXPRESS bằng Windows Authentication và database QLSinhVien.
4. Nếu đây là lần đầu, mở app và bấm Tạo CSDL. App chạy Database\01_QLSinhVien.sql,
   tạo bảng và nạp dữ liệu mẫu còn thiếu; script không xóa dữ liệu đang có.
5. App tự kiểm tra kết nối, hiện DataSource, Database, ServerVersion, State, thời gian
   kết nối, số lượng dữ liệu và danh sách sinh viên đọc từ SQL Server.

NEU TEN INSTANCE KHAC
- Sửa ô Máy chủ trên giao diện, hoặc sửa Data Source trong App.config.
- Đặt SQL Server service ở trạng thái Running. Nếu không dùng Windows Authentication,
  chọn SQL Server Authentication và nhập tài khoản/mật khẩu trong giao diện.
- Mật khẩu nhập trên giao diện chỉ dùng trong phiên chạy, không lưu vào App.config.

CAC SCRIPT SQL
- Database\01_QLSinhVien.sql: tạo QLSinhVien, Khoa, SinhVien, HocPhan, DangKy và dữ liệu mẫu.
- Database\02_KiemTraRangBuoc.sql: thử UNIQUE Email, CHECK điểm, CHECK học kỳ và DEFAULT NgayDangKy.
- Database\03_BackupRestore.sql: backup, xóa thử SV010 và restore lại database. Chỉ chạy khi muốn
  thực hiện bài tập backup/restore trên CSDL lab; script sẽ phục hồi QLSinhVien từ bản backup.
- Database\04_SQLAuthentication.sql: mẫu tạo login SQL chỉ có quyền SELECT. Đổi mật khẩu mẫu trước
  khi chạy, không commit mật khẩu thật; SQL Server cần bật Mixed Mode.

MO HINH DU LIEU
- Khoa 1-n SinhVien.
- SinhVien n-n HocPhan thông qua DangKy; khóa chính DangKy gồm MaSV và MaHP.
- Điểm trung bình và điểm học phần trong khoảng 0-10; số tín chỉ 1-6; học kỳ 1-3.
- Email có unique filtered index: email không NULL không được trùng; cho phép nhiều NULL.
- NgayDangKy có DEFAULT ngày hiện tại.

KIEM CHUNG TREN MAY PHAT TRIEN
- SQL Server: .\SQLEXPRESS (SQL Server 2022 Express).
- Dữ liệu mẫu: 3 khoa, 10 sinh viên, 5 học phần, 15 đăng ký.
- dotnet build Lab07_ADONET_Connection.sln: build thành công, 0 warning, 0 error.
- Đã chạy script ràng buộc và backup/restore để xác nhận database vẫn có SV010 sau khi restore.

Ảnh minh chứng và sơ đồ nằm trong thư mục Screenshots.
