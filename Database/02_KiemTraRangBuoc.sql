/* Lab07 - Thử ràng buộc: mỗi thao tác vi phạm được rollback trong TRY/CATCH. */
USE QLSinhVien;
GO

PRINT N'Kiểm tra UNIQUE Email - cần phát sinh lỗi trùng email:';
BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES ('TEST_EMAIL', N'Sinh viên thử email', '2005-01-01', 1, 'CNTT', 5.00, 'an@eaut.edu.vn');
    ROLLBACK TRANSACTION;
    PRINT N'Không phát hiện lỗi: cần kiểm tra lại index UX_SinhVien_Email.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT N'Đúng - UNIQUE Email đã chặn dữ liệu trùng: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'Kiểm tra CHECK điểm trung bình - cần phát sinh lỗi điểm ngoài 0-10:';
BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES ('TEST_SCORE', N'Sinh viên thử điểm', '2005-01-01', 1, 'CNTT', 10.50, 'test_score@eaut.edu.vn');
    ROLLBACK TRANSACTION;
    PRINT N'Không phát hiện lỗi: cần kiểm tra lại CK_SinhVien_Diem.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT N'Đúng - CHECK điểm đã chặn dữ liệu không hợp lệ: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'Kiểm tra CHECK học kỳ - cần phát sinh lỗi học kỳ ngoài 1-3:';
BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.DangKy(MaSV, MaHP, HocKy, Diem)
    VALUES ('SV001', 'HP005', 4, 8.00);
    ROLLBACK TRANSACTION;
    PRINT N'Không phát hiện lỗi: cần kiểm tra lại CK_DangKy_HocKy.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT N'Đúng - CHECK học kỳ đã chặn dữ liệu không hợp lệ: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'Ngày đăng ký được mặc định nếu lệnh INSERT bỏ qua NgayDangKy:';
SELECT TOP (5) MaSV, MaHP, HocKy, NgayDangKy FROM dbo.DangKy ORDER BY MaSV, MaHP;
GO
