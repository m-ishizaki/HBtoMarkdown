using System.Text.RegularExpressions;

var inputPath = args.FirstOrDefault() ?? string.Empty; if (!File.Exists(inputPath)) return;
var outPath = OutPath(inputPath); if (File.Exists(outPath)) return;

var inputText = File.ReadAllText(inputPath);
var outputText = ReplaceTag(inputText);

File.WriteAllText(outPath, outputText);

static string ReplaceTag(string src) => Regex.Matches(src, C.Pattern).Select(MatchToTag).Aggregate(src, (ag, m) => ag.Replace(m.Key, m.Value));

static KeyValuePair<string, string> MatchToTag(Match m)
{
    var (id, dt, ext) = (m.Groups["id"].Value, m.Groups["dt"].Value, m.Groups["ext"].Value);
    var (id1, date) = (new String(id.Take(1).ToArray()), new String(dt.Take(8).ToArray()));
    C.Extensions.TryGetValue(ext, out var extension);
    var url = string.Format(C.FormatUrl, id1, id, date, dt, extension);
    var tag = string.Format(C.FormatTag, url);
    return new KeyValuePair<string, string>(m.Value, tag);
}

static string OutPath(string inputPath) =>
    Path.Combine(Path.GetDirectoryName(inputPath)!, $"__2__{Path.GetFileName(inputPath)}");

static class C
{
    internal static string Pattern => "\\[f:id:(?<id>\\w+):(?<dt>\\d{14})((?<ext>p|b|j)):.*\\]";
    internal static string FormatUrl => "https://cdn-ak.f.st-hatena.com/images/fotolife/{0}/{1}/{2}/{3}.{4}";
    internal static string FormatTag => "![image]({0})";
    internal static System.Collections.ObjectModel.ReadOnlyDictionary<string, string> Extensions => new(new Dictionary<string, string>
    {
        {"p","png" },
        {"b","bmp" },
        {"j","jpg" },
    });
}
