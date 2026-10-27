namespace Lab07.DAL;

public sealed record ConnectionDiagnostics(
    string DataSource,
    string Database,
    string ServerVersion,
    string State,
    TimeSpan Elapsed,
    int FacultyCount,
    int StudentCount,
    int CourseCount,
    int RegistrationCount);

public sealed record StudentPreview(
    string StudentId,
    string FullName,
    DateTime BirthDate,
    string Gender,
    string Faculty,
    decimal AverageScore,
    string? Email);
