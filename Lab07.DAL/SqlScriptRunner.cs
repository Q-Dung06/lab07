using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Lab07.DAL;

/// <summary>Runs a GO-separated setup script using the configured SQL Server instance.</summary>
public sealed class SqlScriptRunner
{
    private static readonly Regex GoSeparator = new(
        @"^\s*GO(?:\s+\d+)?\s*(?:--.*)?$",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public async Task<int> ExecuteAsync(
        string connectionString,
        string scriptPath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(scriptPath))
            throw new FileNotFoundException("Không tìm thấy file script SQL.", scriptPath);

        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        var batches = GoSeparator.Split(script)
            .Select(batch => batch.Trim())
            .Where(batch => batch.Length > 0)
            .ToArray();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var executed = 0;

        foreach (var batch in batches)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var command = new SqlCommand(batch, connection)
            {
                CommandTimeout = 60
            };
            await command.ExecuteNonQueryAsync(cancellationToken);
            executed++;
        }

        return executed;
    }
}
