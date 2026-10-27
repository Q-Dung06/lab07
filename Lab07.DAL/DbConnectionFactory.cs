using System.Configuration;
using System.Data.SqlClient;

namespace Lab07.DAL;

/// <summary>Creates ADO.NET SQL Server connections from the current UI settings or App.config.</summary>
public static class DbConnectionFactory
{
    public static string GetConfiguredConnectionString()
    {
        var entry = ConfigurationManager.ConnectionStrings["QLSinhVien"];
        if (entry is null || string.IsNullOrWhiteSpace(entry.ConnectionString))
        {
            throw new ConfigurationErrorsException(
                "Không tìm thấy connection string 'QLSinhVien' trong App.config.");
        }

        return entry.ConnectionString;
    }

    public static SqlConnection CreateConfigured() =>
        new(GetConfiguredConnectionString());

    public static SqlConnection Create(
        string server,
        string database,
        bool useSqlAuthentication,
        string? username = null,
        string? password = null,
        string? initialCatalogOverride = null)
    {
        if (string.IsNullOrWhiteSpace(server))
            throw new ArgumentException("Tên máy chủ/instance không được để trống.", nameof(server));
        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Tên database không được để trống.", nameof(database));

        if (useSqlAuthentication && string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Hãy nhập tài khoản SQL Authentication.", nameof(username));

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server.Trim(),
            InitialCatalog = initialCatalogOverride ?? database.Trim(),
            IntegratedSecurity = !useSqlAuthentication,
            ConnectTimeout = 8,
            ApplicationName = "Lab07_ADONET_Connection",
            Encrypt = true,
            TrustServerCertificate = true,
            PersistSecurityInfo = false
        };

        if (useSqlAuthentication)
        {
            builder.UserID = username!.Trim();
            builder.Password = password ?? string.Empty;
        }

        return new SqlConnection(builder.ConnectionString);
    }
}
