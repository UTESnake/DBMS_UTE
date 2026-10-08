using System.Reflection;
using System.Text.RegularExpressions;

namespace DoAn.Shared;

public sealed record CompanyTestcase(string Id, string Code, string Description, string Input, string Expected)
{
    public override string ToString() => $"{Id} - {Description}";
}

public static class CompanyTestcaseCatalog
{
    // The same SQL file that creates the database registry is embedded in both forms.
    // This keeps the chooser usable before the optional registry script is installed.
    public static IReadOnlyList<CompanyTestcase> ReadEmbedded(Assembly assembly, string prefix)
    {
        string? resource = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("02_Testcase.sql", StringComparison.OrdinalIgnoreCase));
        if (resource is null) return [];
        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var reader = new StreamReader(stream);
        var cases = new List<CompanyTestcase>();
        while (reader.ReadLine() is { } line)
        {
            if (!line.TrimStart().StartsWith("('", StringComparison.Ordinal)) continue;
            var fields = Regex.Matches(line, @"N?'((?:''|[^'])*)'")
                .Select(match => match.Groups[1].Value.Replace("''", "'", StringComparison.Ordinal))
                .ToArray();
            if (fields.Length != 6 || !fields[1].StartsWith(prefix, StringComparison.Ordinal)) continue;
            cases.Add(new CompanyTestcase(fields[0], fields[1], fields[3], fields[4], fields[5]));
        }
        return cases;
    }
}
