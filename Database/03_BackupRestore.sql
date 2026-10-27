/*
    Lab07 - Bài tập backup/restore.
    Chạy trên SQL Server Express bằng tài khoản có quyền BACKUP/RESTORE.
    File backup được đặt trong thư mục backup mặc định của SQL Server instance.
    Script tạo backup, xóa thử SV010 rồi phục hồi nguyên database từ backup.
*/
USE master;
GO

DECLARE @BackupDirectory NVARCHAR(4000) = CONVERT(NVARCHAR(4000), SERVERPROPERTY('InstanceDefaultBackupPath'));
IF NULLIF(@BackupDirectory, N'') IS NULL
    THROW 51000, N'Instance không công bố thư mục backup mặc định. Hãy thay bằng đường dẫn tuyệt đối mà tài khoản SQL Server service được phép ghi.', 1;

IF RIGHT(@BackupDirectory, 1) NOT IN (N'\', N'/')
    SET @BackupDirectory += N'\';

DECLARE @BackupFile NVARCHAR(4000) = @BackupDirectory + N'QLSinhVien_Lab07.bak';
PRINT N'File backup: ' + @BackupFile;
BACKUP DATABASE [QLSinhVien]
    TO DISK = @BackupFile
    WITH INIT, CHECKSUM, STATS = 10;
RESTORE VERIFYONLY FROM DISK = @BackupFile WITH CHECKSUM;
GO

/* Kiểm chứng backup: xóa SV010 sau khi đã backup. */
USE QLSinhVien;
GO
DELETE FROM dbo.DangKy WHERE MaSV = 'SV010';
DELETE FROM dbo.SinhVien WHERE MaSV = 'SV010';
SELECT N'SV010 sau khi xóa' AS Buoc, COUNT(*) AS SoLuong FROM dbo.SinhVien WHERE MaSV = 'SV010';
GO

/* Phục hồi snapshot đã backup; dữ liệu SV010 sẽ xuất hiện lại. */
USE master;
GO
DECLARE @BackupDirectory NVARCHAR(4000) = CONVERT(NVARCHAR(4000), SERVERPROPERTY('InstanceDefaultBackupPath'));
IF RIGHT(@BackupDirectory, 1) NOT IN (N'\', N'/') SET @BackupDirectory += N'\';
DECLARE @BackupFile NVARCHAR(4000) = @BackupDirectory + N'QLSinhVien_Lab07.bak';

ALTER DATABASE [QLSinhVien] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [QLSinhVien] FROM DISK = @BackupFile WITH REPLACE, CHECKSUM, RECOVERY;
ALTER DATABASE [QLSinhVien] SET MULTI_USER;
GO

USE QLSinhVien;
GO
SELECT N'SV010 sau khi restore - phải bằng 1' AS Buoc, COUNT(*) AS SoLuong
FROM dbo.SinhVien WHERE MaSV = 'SV010';
GO
