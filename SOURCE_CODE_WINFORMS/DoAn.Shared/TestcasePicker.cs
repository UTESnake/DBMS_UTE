using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace DoAn.Shared;

public sealed record TestcaseOption(string Id, string Code, string Description, string Input, string Expected)
{
    public override string ToString() => $"{Id} - {Description}";
}

public sealed class TestcasePicker : Panel
{
    private readonly ComboBox choices = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label expected = new() { AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ToolTip tip = new();
    private Action<TestcaseOption>? apply;

    public TestcasePicker()
    {
        Height = 58;
        BackColor = Color.White;
        var title = new Label { Text = "Testcase:", AutoSize = true, Location = new Point(10, 19) };
        choices.Location = new Point(100, 13);
        choices.Size = new Size(390, 30);
        choices.SelectedIndexChanged += (_, _) =>
        {
            if (choices.SelectedItem is not TestcaseOption item) return;
            expected.Text = "Kỳ vọng: " + item.Expected;
            tip.SetToolTip(expected, item.Expected);
            apply?.Invoke(item);
        };
        expected.Location = new Point(510, 12);
        expected.Size = new Size(450, 32);
        Controls.AddRange([title, choices, expected]);
    }

    public void SetCases(IEnumerable<TestcaseOption> cases, Action<TestcaseOption> onSelect)
    {
        apply = onSelect;
        choices.Items.Clear();
        choices.Items.Add("Chọn testcase hoặc nhập dữ liệu trực tiếp");
        foreach (var item in cases) choices.Items.Add(item);
        choices.SelectedIndex = 0;
        expected.Text = choices.Items.Count == 1 ? "Chưa có testcase; hãy nạp danh mục." : "Có thể sửa dữ liệu sau khi chọn.";
    }

    public static TestcasePicker InsertBand(Form form, int top)
    {
        foreach (Control control in form.Controls.Cast<Control>().Where(x => x.Top >= top && x.Dock == DockStyle.None))
            control.Top += 58;
        form.ClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height + 58);
        form.AutoScroll = true;
        var picker = new TestcasePicker { Location = new Point(35, top), Width = form.ClientSize.Width - 70 };
        form.Controls.Add(picker);
        picker.BringToFront();
        return picker;
    }

    public static IEnumerable<TestcaseOption> FromTable(DataTable table, string code, string idColumn,
        string descriptionColumn, string expectedColumn, params string[] inputColumns)
    {
        foreach (DataRow row in table.Rows)
            yield return new TestcaseOption(row[idColumn]?.ToString() ?? "", code,
                row[descriptionColumn]?.ToString() ?? "",
                string.Join(";", inputColumns.Select(name => row[name] is DBNull ? "" : row[name]?.ToString() ?? "")),
                row[expectedColumn]?.ToString() ?? "");
    }

    public static IReadOnlyList<TestcaseOption> FromEmbeddedSql(Assembly assembly, string resourceSuffix, string prefix)
    {
        string? name = assembly.GetManifestResourceNames().FirstOrDefault(x =>
            x.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));
        if (name is null) return [];
        using var stream = assembly.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        var cases = new List<TestcaseOption>();
        while (reader.ReadLine() is { } line)
        {
            if (!line.TrimStart().StartsWith("('", StringComparison.Ordinal)) continue;
            var fields = Regex.Matches(line, @"N?'((?:''|[^'])*)'")
                .Select(x => x.Groups[1].Value.Replace("''", "'", StringComparison.Ordinal)).ToArray();
            if (fields.Length == 6 && fields[1].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                cases.Add(new TestcaseOption(fields[0], fields[1], fields[3], fields[4], fields[5]));
            else if (fields.Length == 8 && fields[1].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                cases.Add(new TestcaseOption(fields[0], fields[1], fields[4], fields[5], fields[6]));
        }
        return cases;
    }

    public static DataTable FilterRows(DataTable source, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return source;
        var result = source.Clone();
        foreach (DataRow row in source.Rows)
            if (row.ItemArray.Any(value => value?.ToString()?.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase) == true))
                result.ImportRow(row);
        return result;
    }

    public static IReadOnlyList<TestcaseOption> FromMathSql(Assembly assembly, string code)
    {
        string? name = assembly.GetManifestResourceNames().FirstOrDefault(x =>
            x.EndsWith("02_Testcase.sql", StringComparison.OrdinalIgnoreCase));
        if (name is null) return [];
        using var stream = assembly.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        var cases = new List<TestcaseOption>();
        while (reader.ReadLine() is { } line)
        {
            if (!line.TrimStart().StartsWith("('", StringComparison.Ordinal)) continue;
            var values = Regex.Matches(line, @"N?'((?:''|[^'])*)'|\bNULL\b|\b\d+\b")
                .Select(match => match.Groups[1].Success
                    ? match.Groups[1].Value.Replace("''", "'", StringComparison.Ordinal)
                    : match.Value == "NULL" ? "" : match.Value).ToArray();
            if (values.Length != 10 || values[1] != code) continue;
            string input = code switch
            {
                "B1" => values[3] + ";" + values[4],
                "B2" => values[3] + ";" + values[4] + ";" + values[5],
                _ => values[6]
            };
            cases.Add(new TestcaseOption(values[0], code, values[2], input, values[8]));
        }
        return cases;
    }
}
