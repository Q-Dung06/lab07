/*
    Lab07 - Tài khoản SQL Authentication quyền đọc tối thiểu.
    1. Thay mật khẩu mẫu bằng mật khẩu riêng mạnh, không đưa mật khẩu thật lên GitHub.
    2. Bật SQL Server and Windows Authentication mode (Mixed Mode) nếu cần.
    3. Chạy bằng tài khoản quản trị SQL Server; sau đó đăng nhập ứng dụng với lab07_app.
*/
USE master;
GO
IF SUSER_ID(N'lab07_app') IS NULL
BEGIN
    CREATE LOGIN [lab07_app]
        WITH PASSWORD = N'CHANGE_THIS_Strong_Password_2026!',
             CHECK_POLICY = ON,
             CHECK_EXPIRATION = OFF;
END;
GO

USE QLSinhVien;
GO
IF USER_ID(N'lab07_app') IS NULL
    CREATE USER [lab07_app] FOR LOGIN [lab07_app];
GO

GRANT SELECT ON OBJECT::dbo.Khoa TO [lab07_app];
GRANT SELECT ON OBJECT::dbo.SinhVien TO [lab07_app];
GRANT SELECT ON OBJECT::dbo.HocPhan TO [lab07_app];
GRANT SELECT ON OBJECT::dbo.DangKy TO [lab07_app];
GO

PRINT N'Tài khoản lab07_app chỉ được SELECT các bảng cần cho màn hình demo.';
