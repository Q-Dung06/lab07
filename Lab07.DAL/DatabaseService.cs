using System.Data.SqlClient;
using System.Diagnostics;

namespace Lab07.DAL;

/// <summary>ADO.NET operations used by the Lab07 connection and status demonstration.</summary>
public sealed class DatabaseService
{
    public async Task<ConnectionDiagnostics> CheckConnectionAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(connectionString);
        var timer = Stopwatch.StartNew();
        await connection.OpenAsync(cancellationToken);
        timer.Stop();

        var counts = await GetCountsAsync(connection, cancellationToken);
        return new ConnectionDiagnostics(
            connection.DataSource,
            connection.Database,
            connection.ServerVersion,
            connection.State.ToString(),
            timer.Elapsed,
            counts.Faculties,
            counts.Students,
            counts.Courses,
            counts.Registrations);
    }

    public async Task<IReadOnlyList<StudentPreview>> GetStudentsAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (50)
                sv.MaSV,
                sv.HoTen,
                sv.NgaySinh,
                sv.GioiTinh,
                k.TenKhoa,
                sv.DiemTB,
                sv.Email
            FROM dbo.SinhVien AS sv
            INNER JOIN dbo.Khoa AS k ON k.MaKhoa = sv.MaKhoa
            ORDER BY sv.MaSV;
            """;

        var students = new List<StudentPreview>();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            students.Add(new StudentPreview(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetDateTime(2),
                reader.GetBoolean(3) ? "Nam" : "Nữ",
                reader.GetString(4),
                reader.GetDecimal(5),
                reader.IsDBNull(6) ? null : reader.GetString(6)));
        }

        return students;
    }

    private static async Task<(int Faculties, int Students, int Courses, int Registrations)> GetCountsAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                (SELECT COUNT(*) FROM dbo.Khoa),
                (SELECT COUNT(*) FROM dbo.SinhVien),
                (SELECT COUNT(*) FROM dbo.HocPhan),
                (SELECT COUNT(*) FROM dbo.DangKy);
            """;

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            throw new InvalidOperationException("Không đọc được số liệu tổng quan của database.");

        return (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
    }
}
