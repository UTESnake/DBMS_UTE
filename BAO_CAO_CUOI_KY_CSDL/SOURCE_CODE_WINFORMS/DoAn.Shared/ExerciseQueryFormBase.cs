using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace DoAn.Shared;

public abstract class ExerciseQueryFormBase : Form
{
    protected sealed record QueryItem(string Code, string Title, string ParameterLabel, string Sql, string? ActionText = null);

    private const string Server = @".\SQLEXPRESS02";
    private readonly ComboBox _queries = new();
    private readonly TextBox _parameter = new();
    private readonly Label _parameterLabel = new();
    private readonly Label _status = new();
    private readonly DataGridView _result = CreateGrid();
    private readonly Button _run = CreateButton("▥  Thống kê CSDL", Color.FromArgb(16, 185, 129));
    private bool _connected;

    protected abstract string DatabaseName { get; }
    protected abstract string WindowTitle { get; }
    protected abstract string HeaderTitle { get; }
    protected abstract Color HeaderColor { get; }
    protected abstract string ObjectNamePattern { get; }
    protected abstract int RequiredObjectCount { get; }
    protected abstract QueryItem[] Queries { get; }
    protected abstract string[] ScriptResourceSuffixes { get; }

    protected ExerciseQueryFormBase()
    {
        BuildInterface();
    }

    private string DatabaseConnectionString => $"Data Source={Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
    private static string MasterConnectionString => $"Data Source={Server};Initial Catalog=master;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

    private void BuildInterface()
    {
        Text = WindowTitle;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(1050, 620);
        Size = new Size(1200, 760);
        BackColor = Color.FromArgb(248, 250, 252);
        Font = new Font("Segoe UI", 10);

        var header = new Panel { Dock = DockStyle.Top, Height = 94, BackColor = HeaderColor };
        header.Controls.Add(new Label { Text = HeaderTitle, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold), ForeColor = Color.White });

        var connection = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.White, Padding = new Padding(28, 8, 28, 8) };
        var connect = CreateButton("⌁  Kết nối CSDL", Color.FromArgb(37, 99, 235));
        connect.Dock = DockStyle.Left;
        connect.Width = 185;
        connect.Click += ConnectAsync;
        _status.Text = $"● Chưa kết nối {DatabaseName}";
        _status.Dock = DockStyle.Fill;
        _status.Padding = new Padding(18, 0, 0, 0);
        _status.TextAlign = ContentAlignment.MiddleLeft;
        _status.ForeColor = Color.Firebrick;
        connection.Controls.Add(_status);
        connection.Controls.Add(connect);

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28, 18, 28, 18), RowCount = 2, ColumnCount = 1 };
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var input = new GroupBox { Text = "Chọn yêu cầu và nhập tham số", Dock = DockStyle.Fill, Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold) };
        _queries.DropDownStyle = ComboBoxStyle.DropDownList;
        _queries.Location = new Point(25, 43);
        _queries.Size = new Size(485, 30);
        _queries.Items.AddRange(Queries.Select(x => $"{x.Code} - {x.Title}").ToArray());
        _parameterLabel.Location = new Point(540, 25);
        _parameterLabel.AutoSize = true;
        _parameter.Location = new Point(540, 51);
        _parameter.Size = new Size(210, 30);
        _run.Location = new Point(790, 40);
        _run.Size = new Size(210, 43);
        _run.Click += ExecuteAsync;
        input.Controls.AddRange([_queries, _parameterLabel, _parameter, _run]);
        body.Controls.Add(input, 0, 0);

        var resultBox = new GroupBox { Text = "Kết quả trả về", Dock = DockStyle.Fill, Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold), Padding = new Padding(12) };
        _result.Dock = DockStyle.Fill;
        resultBox.Controls.Add(_result);
        body.Controls.Add(resultBox, 0, 1);

        Controls.Add(body);
        Controls.Add(connection);
        Controls.Add(header);
        _queries.SelectedIndexChanged += (_, _) => ShowParameters();
        _queries.SelectedIndex = 0;
        EnableActions(false);
    }

    private async void ConnectAsync(object? sender, EventArgs e)
    {
        try
        {
            EnableActions(false);
            _status.Text = $"● Đang khởi tạo {DatabaseName}...";
            _status.ForeColor = Color.DarkOrange;
            await EnsureDatabaseAsync();
            await using var connection = new SqlConnection(DatabaseConnectionString);
            await connection.OpenAsync();
            int count = await CountObjectsAsync(connection);
            if (count < RequiredObjectCount)
            {
                _status.Text = "● Đang tự động cài đặt dữ liệu, ràng buộc và function...";
                Application.DoEvents();
                await InstallScriptsAsync(connection);
                count = await CountObjectsAsync(connection);
            }
            if (count < RequiredObjectCount)
                throw new InvalidOperationException($"Cài đặt chưa hoàn tất ({count}/{RequiredObjectCount} đối tượng SQL). ");
            _connected = true;
            _status.Text = $"● Đã kết nối {DatabaseName} • Đủ {count} đối tượng SQL";
            _status.ForeColor = Color.SeaGreen;
            EnableActions(true);
            MessageBox.Show("CSDL đã sẵn sàng. Hãy chọn yêu cầu rồi bấm Thống kê CSDL.", "Kết nối thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            _connected = false;
            EnableActions(false);
            _status.Text = $"● Không thể khởi tạo {DatabaseName}";
            _status.ForeColor = Color.Firebrick;
            MessageBox.Show("Không thể khởi tạo CSDL.\n\n" + exception.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task EnsureDatabaseAsync()
    {
        await using var connection = new SqlConnection(MasterConnectionString);
        await connection.OpenAsync();
        const string sql = "IF DB_ID(@name) IS NULL BEGIN DECLARE @q nvarchar(max)=N'CREATE DATABASE '+QUOTENAME(@name); EXEC sys.sp_executesql @q; END";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@name", SqlDbType.NVarChar, 128).Value = DatabaseName;
        await command.ExecuteNonQueryAsync();
    }

    private async Task<int> CountObjectsAsync(SqlConnection connection)
    {
        const string sql = "SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF','TR') AND name LIKE @pattern";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@pattern", SqlDbType.NVarChar, 128).Value = ObjectNamePattern;
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private async Task InstallScriptsAsync(SqlConnection connection)
    {
        Assembly assembly = GetType().Assembly;
        foreach (string suffix in ScriptResourceSuffixes)
        {
            string? resource = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            if (resource is null) throw new InvalidOperationException("Không tìm thấy script nhúng: " + suffix);
            await using Stream stream = assembly.GetManifestResourceStream(resource)!;
            using var reader = new StreamReader(stream);
            string script = await reader.ReadToEndAsync();
            string[] batches = Regex.Split(script, @"^\s*GO\s*$(?:\r?\n)?", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (string batch in batches.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                await using var command = new SqlCommand(batch, connection) { CommandTimeout = 60 };
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private void EnableActions(bool enabled) => _queries.Enabled = _run.Enabled = enabled;

    private void ShowParameters()
    {
        if (_queries.SelectedIndex < 0) return;
        QueryItem query = Queries[_queries.SelectedIndex];
        string label = query.ParameterLabel;
        _parameterLabel.Text = label;
        _parameter.Visible = _parameterLabel.Visible = label.Length > 0;
        _run.Text = string.IsNullOrWhiteSpace(query.ActionText) ? "▥  Thống kê CSDL" : query.ActionText;
    }

    private async void ExecuteAsync(object? sender, EventArgs e)
    {
        if (!_connected || _queries.SelectedIndex < 0) return;
        try { _result.DataSource = await QueryAsync(Queries[_queries.SelectedIndex], _parameter.Text.Trim()); }
        catch (Exception exception) { ShowSqlError(exception); }
    }

    private async Task<DataTable> QueryAsync(QueryItem query, string parameter)
    {
        await using var connection = new SqlConnection(DatabaseConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query.Sql, connection);
        command.Parameters.Add("@p1", SqlDbType.NVarChar, 100).Value = parameter;
        await using var reader = await command.ExecuteReaderAsync();
        var data = new DataTable();
        data.Load(reader);
        return data;
    }

    private static void ShowSqlError(Exception exception) => MessageBox.Show("Không thể thực hiện truy vấn. Hãy kết nối để chương trình tự cài đặt đủ script.\n\n" + exception.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
    private static Button CreateButton(string text, Color color) => new() { Text = text, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold), Cursor = Cursors.Hand, Height = 42 };
    private static DataGridView CreateGrid() => new() { ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BackgroundColor = Color.White };
}
