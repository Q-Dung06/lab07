using System.Data.SqlClient;
using System.Diagnostics;
using Lab07.DAL;

namespace Lab07_ADONET_Connection;

public sealed class MainForm : Form
{
    private readonly DatabaseService _databaseService = new();
    private readonly SqlScriptRunner _scriptRunner = new();
    private readonly TextBox _serverTextBox = new();
    private readonly TextBox _databaseTextBox = new();
    private readonly ComboBox _authenticationComboBox = new();
    private readonly TextBox _usernameTextBox = new();
    private readonly TextBox _passwordTextBox = new();
    private readonly Button _checkButton = new();
    private readonly Button _initializeButton = new();
    private readonly Button _scriptsButton = new();
    private readonly Label _statusLabel = new();
    private readonly Label _serverValueLabel = new();
    private readonly Label _databaseValueLabel = new();
    private readonly Label _versionValueLabel = new();
    private readonly Label _timeValueLabel = new();
    private readonly Label _countsLabel = new();
    private readonly Label _footerLabel = new();
    private readonly DataGridView _studentsGrid = new();

    private bool UseSqlAuthentication => _authenticationComboBox.SelectedIndex == 1;

    public MainForm()
    {
        Text = "Lab07 - ADO.NET và kết nối SQL Server";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1050, 730);
        Size = new Size(1240, 850);
        BackColor = Color.FromArgb(241, 245, 249);
        Font = new Font("Segoe UI", 9.5F);

        BuildInterface();
        LoadDefaultsFromConfiguration();
        _authenticationComboBox.SelectedIndexChanged += (_, _) => UpdateAuthenticationFields();
        _checkButton.Click += async (_, _) => await CheckAndLoadAsync();
        _initializeButton.Click += async (_, _) => await InitializeDatabaseAsync();
        _scriptsButton.Click += (_, _) => OpenSqlScriptsFolder();
        Shown += async (_, _) => await CheckAndLoadAsync();
        UpdateAuthenticationFields();
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 94));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 206));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 156));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        Controls.Add(root);
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildConnectionCard(), 0, 1);
        root.Controls.Add(BuildStatusCard(), 0, 2);
        root.Controls.Add(BuildStudentsCard(), 0, 3);

        _footerLabel.Text = "Dữ liệu được đọc qua ADO.NET; mật khẩu SQL chỉ dùng trong phiên demo và không lưu vào App.config.";
        _footerLabel.Dock = DockStyle.Fill;
        _footerLabel.ForeColor = Color.FromArgb(83, 101, 123);
        _footerLabel.TextAlign = ContentAlignment.MiddleLeft;
        root.Controls.Add(_footerLabel, 0, 4);
    }

    private Control BuildHeader()
    {
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(22, 54, 91),
            Padding = new Padding(22, 12, 22, 10),
            Margin = new Padding(0, 0, 0, 14)
        };
        var title = new Label
        {
            Text = "LAB 07  |  KẾT NỐI SQL SERVER",
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 17F, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 38
        };
        var subtitle = new Label
        {
            Text = "Quản lý kết nối QLSinhVien • Xác thực Windows/SQL • Trạng thái server và dữ liệu mẫu",
            ForeColor = Color.FromArgb(213, 226, 242),
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F)
        };
        header.Controls.Add(subtitle);
        header.Controls.Add(title);
        return header;
    }

    private Control BuildConnectionCard()
    {
        var card = CreateCard();
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 4,
            BackColor = Color.White
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 94));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(grid);

        var heading = CreateSectionTitle("THIẾT LẬP KẾT NỐI");
        grid.Controls.Add(heading, 0, 0);
        grid.SetColumnSpan(heading, 6);

        _serverTextBox.Dock = DockStyle.Fill;
        _serverTextBox.Margin = new Padding(4, 5, 12, 5);
        _databaseTextBox.Dock = DockStyle.Fill;
        _databaseTextBox.Margin = new Padding(4, 5, 12, 5);
        _authenticationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _authenticationComboBox.Items.AddRange(["Windows Authentication", "SQL Server Authentication"]);
        _authenticationComboBox.Dock = DockStyle.Fill;
        _authenticationComboBox.Margin = new Padding(4, 5, 4, 5);
        AddField(grid, "Máy chủ", _serverTextBox, 0, 1);
        AddField(grid, "Database", _databaseTextBox, 2, 1);
        AddField(grid, "Xác thực", _authenticationComboBox, 4, 1);

        _usernameTextBox.Dock = DockStyle.Fill;
        _usernameTextBox.Margin = new Padding(4, 5, 12, 5);
        _passwordTextBox.Dock = DockStyle.Fill;
        _passwordTextBox.Margin = new Padding(4, 5, 12, 5);
        _passwordTextBox.UseSystemPasswordChar = true;
        AddField(grid, "Tài khoản", _usernameTextBox, 0, 2);
        AddField(grid, "Mật khẩu", _passwordTextBox, 2, 2);

        StyleButton(_checkButton, "Kiểm tra", Color.FromArgb(37, 99, 235));
        _checkButton.Margin = new Padding(4, 4, 8, 4);
        StyleButton(_initializeButton, "Tạo CSDL", Color.FromArgb(5, 126, 105));
        _initializeButton.Margin = new Padding(4, 4, 4, 4);
        grid.Controls.Add(_checkButton, 4, 2);
        grid.Controls.Add(_initializeButton, 5, 2);

        var note = new Label
        {
            Text = "Lần đầu demo: bấm “Tạo CSDL”. Script chỉ thêm dữ liệu mẫu còn thiếu.",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(83, 101, 123),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(4, 0, 8, 0)
        };
        grid.Controls.Add(note, 0, 3);
        grid.SetColumnSpan(note, 4);
        StyleButton(_scriptsButton, "Mở thư mục SQL", Color.FromArgb(71, 85, 105), outlined: true);
        grid.Controls.Add(_scriptsButton, 4, 3);
        grid.SetColumnSpan(_scriptsButton, 2);
        return card;
    }

    private Control BuildStatusCard()
    {
        var card = CreateCard();
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(layout);
        layout.Controls.Add(CreateSectionTitle("TRẠNG THÁI KẾT NỐI"), 0, 0);

        _statusLabel.Text = "Chưa kết nối - cấu hình máy chủ rồi chọn Kiểm tra kết nối.";
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        _statusLabel.ForeColor = Color.FromArgb(71, 85, 105);
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(_statusLabel, 0, 1);

        var details = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(0, 3, 0, 0)
        };
        details.Controls.Add(CreateInfoTile("DATA SOURCE", _serverValueLabel, 160));
        details.Controls.Add(CreateInfoTile("DATABASE", _databaseValueLabel, 130));
        details.Controls.Add(CreateInfoTile("SERVER VERSION", _versionValueLabel, 175));
        details.Controls.Add(CreateInfoTile("THỜI GIAN", _timeValueLabel, 120));
        details.Controls.Add(CreateInfoTile("DỮ LIỆU MẪU", _countsLabel, 275));
        layout.Controls.Add(details, 0, 2);
        return card;
    }

    private Control BuildStudentsCard()
    {
        var card = CreateCard();
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(layout);
        layout.Controls.Add(CreateSectionTitle("DANH SÁCH SINH VIÊN ĐỌC TỪ SQL SERVER (TOP 50)"), 0, 0);

        _studentsGrid.Dock = DockStyle.Fill;
        _studentsGrid.ReadOnly = true;
        _studentsGrid.AllowUserToAddRows = false;
        _studentsGrid.AllowUserToDeleteRows = false;
        _studentsGrid.AllowUserToResizeRows = false;
        _studentsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _studentsGrid.BackgroundColor = Color.White;
        _studentsGrid.BorderStyle = BorderStyle.None;
        _studentsGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        _studentsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 238, 248);
        _studentsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 52, 77);
        _studentsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        _studentsGrid.EnableHeadersVisualStyles = false;
        _studentsGrid.RowHeadersVisible = false;
        _studentsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _studentsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        _studentsGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
        _studentsGrid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        _studentsGrid.RowTemplate.Height = 32;
        layout.Controls.Add(_studentsGrid, 0, 1);
        return card;
    }

    private static Panel CreateCard() => new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White,
        Padding = new Padding(16, 12, 16, 12),
        Margin = new Padding(0, 0, 0, 14)
    };

    private static Label CreateSectionTitle(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        ForeColor = Color.FromArgb(30, 52, 77),
        Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static void AddField(TableLayoutPanel grid, string title, Control input, int labelColumn, int row)
    {
        var label = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(71, 85, 105),
            Margin = new Padding(2)
        };
        grid.Controls.Add(label, labelColumn, row);
        grid.Controls.Add(input, labelColumn + 1, row);
    }

    private static void StyleButton(Button button, string text, Color color, bool outlined = false)
    {
        button.Text = text;
        button.Dock = DockStyle.Fill;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = outlined ? 1 : 0;
        button.FlatAppearance.BorderColor = color;
        button.BackColor = outlined ? Color.White : color;
        button.ForeColor = outlined ? color : Color.White;
        button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    private static Control CreateInfoTile(string title, Label valueLabel, int width)
    {
        var tile = new Panel
        {
            Width = width,
            Height = 45,
            Margin = new Padding(0, 0, 12, 0),
            BackColor = Color.FromArgb(248, 250, 252)
        };
        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 16,
            ForeColor = Color.FromArgb(100, 116, 139),
            Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
            Padding = new Padding(7, 0, 0, 0)
        };
        valueLabel.Text = "—";
        valueLabel.Dock = DockStyle.Fill;
        valueLabel.ForeColor = Color.FromArgb(30, 41, 59);
        valueLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        valueLabel.Padding = new Padding(7, 0, 4, 0);
        valueLabel.AutoEllipsis = true;
        tile.Controls.Add(valueLabel);
        tile.Controls.Add(titleLabel);
        return tile;
    }

    private void LoadDefaultsFromConfiguration()
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(DbConnectionFactory.GetConfiguredConnectionString());
            _serverTextBox.Text = builder.DataSource;
            _databaseTextBox.Text = builder.InitialCatalog;
            _authenticationComboBox.SelectedIndex = builder.IntegratedSecurity ? 0 : 1;
            if (!builder.IntegratedSecurity)
                _usernameTextBox.Text = builder.UserID;
        }
        catch (Exception ex)
        {
            _serverTextBox.Text = @".\SQLEXPRESS";
            _databaseTextBox.Text = "QLSinhVien";
            _authenticationComboBox.SelectedIndex = 0;
            _footerLabel.Text = "Không đọc được App.config: " + ex.Message;
        }
    }

    private void UpdateAuthenticationFields()
    {
        var sqlAuth = UseSqlAuthentication;
        _usernameTextBox.Enabled = sqlAuth;
        _passwordTextBox.Enabled = sqlAuth;
        _usernameTextBox.BackColor = sqlAuth ? Color.White : Color.FromArgb(241, 245, 249);
        _passwordTextBox.BackColor = sqlAuth ? Color.White : Color.FromArgb(241, 245, 249);
    }

    private string BuildCurrentConnectionString(string? initialCatalog = null)
    {
        using var connection = DbConnectionFactory.Create(
            _serverTextBox.Text,
            _databaseTextBox.Text,
            UseSqlAuthentication,
            _usernameTextBox.Text,
            _passwordTextBox.Text,
            initialCatalog);
        return connection.ConnectionString;
    }

    private async Task InitializeDatabaseAsync()
    {
        var answer = MessageBox.Show(
            this,
            "Chạy script tạo QLSinhVien và dữ liệu mẫu? Script không xóa dữ liệu hiện có, nhưng tài khoản Windows/SQL cần quyền tạo database.",
            "Khởi tạo database Lab07",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
            return;

        SetBusy(true, "Đang chạy script tạo database và dữ liệu mẫu...");
        try
        {
            var masterConnectionString = BuildCurrentConnectionString("master");
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Database", "01_QLSinhVien.sql");
            var batchCount = await _scriptRunner.ExecuteAsync(masterConnectionString, scriptPath);
            _footerLabel.Text = $"Khởi tạo hoàn tất: đã thực thi {batchCount} batch SQL.";
            await CheckAndLoadAsync();
        }
        catch (Exception ex)
        {
            SetConnectionFailure(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task CheckAndLoadAsync()
    {
        SetBusy(true, "Đang mở kết nối và đọc trạng thái SQL Server...");
        try
        {
            var connectionString = BuildCurrentConnectionString();
            var diagnostics = await _databaseService.CheckConnectionAsync(connectionString);
            _statusLabel.Text = "●  KẾT NỐI THÀNH CÔNG  •  " + diagnostics.State;
            _statusLabel.ForeColor = Color.FromArgb(5, 126, 105);
            _serverValueLabel.Text = diagnostics.DataSource;
            _databaseValueLabel.Text = diagnostics.Database;
            _versionValueLabel.Text = diagnostics.ServerVersion;
            _timeValueLabel.Text = $"{diagnostics.Elapsed.TotalMilliseconds:N0} ms";
            _countsLabel.Text = $"{diagnostics.FacultyCount} khoa  |  {diagnostics.StudentCount} SV  |  {diagnostics.CourseCount} HP  |  {diagnostics.RegistrationCount} đăng ký";

            var students = await _databaseService.GetStudentsAsync(connectionString);
            _studentsGrid.DataSource = students.Select(s => new
            {
                MãSV = s.StudentId,
                HọTên = s.FullName,
                NgàySinh = s.BirthDate.ToString("dd/MM/yyyy"),
                GiớiTính = s.Gender,
                Khoa = s.Faculty,
                ĐiểmTB = s.AverageScore,
                Email = s.Email
            }).ToList();
            _footerLabel.Text = $"ADO.NET SqlConnection.OpenAsync hoàn tất trong {diagnostics.Elapsed.TotalMilliseconds:N0} ms; đã đọc {students.Count} sinh viên.";
        }
        catch (Exception ex)
        {
            SetConnectionFailure(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _checkButton.Enabled = !busy;
        _initializeButton.Enabled = !busy;
        _scriptsButton.Enabled = !busy;
        _authenticationComboBox.Enabled = !busy;
        _serverTextBox.Enabled = !busy;
        _databaseTextBox.Enabled = !busy;
        if (message is not null)
        {
            _statusLabel.Text = "◌  " + message;
            _statusLabel.ForeColor = Color.FromArgb(37, 99, 235);
        }
    }

    private void SetConnectionFailure(Exception exception)
    {
        _statusLabel.Text = "●  KẾT NỐI/THAO TÁC THẤT BẠI";
        _statusLabel.ForeColor = Color.FromArgb(185, 28, 28);
        _serverValueLabel.Text = "—";
        _databaseValueLabel.Text = "—";
        _versionValueLabel.Text = "—";
        _timeValueLabel.Text = "—";
        _countsLabel.Text = "Chưa đọc được dữ liệu";
        _footerLabel.Text = exception is SqlException sqlException
            ? $"SQL Error {sqlException.Number}: {sqlException.Message}"
            : exception.Message;
        if (exception is FileNotFoundException)
            _footerLabel.Text += " | Hãy chạy lại từ project để copy thư mục Database vào output.";
    }

    private void OpenSqlScriptsFolder()
    {
        var scriptsPath = Path.Combine(AppContext.BaseDirectory, "Database");
        if (!Directory.Exists(scriptsPath))
        {
            MessageBox.Show(this, "Không tìm thấy thư mục Database trong thư mục chạy ứng dụng.", "Thiếu script SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Process.Start(new ProcessStartInfo { FileName = scriptsPath, UseShellExecute = true });
    }
}
