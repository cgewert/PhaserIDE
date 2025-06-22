using System.Text.RegularExpressions;

namespace PhaserIDE.Create
{
    internal class TemplateEngine
    {
        private readonly Dictionary<string, string> _replacements;
        public const string TemplateExtension = ".template";
        public const string ChecksumFile = ".check";


        public TemplateEngine(Dictionary<string, string> replacements)
        {
            _replacements = replacements;
        }
        public void RenderTemplates(string templatesPath, string outputPath)
        {
            if (!Directory.Exists(templatesPath))
                throw new DirectoryNotFoundException($"Templates folder not found: {templatesPath}");

            if (!Directory.Exists(outputPath))
                throw new DirectoryNotFoundException($"Output folder not found: {outputPath}");

            var allFiles = Directory.EnumerateFiles(templatesPath, "*", SearchOption.AllDirectories).ToList();
            // Ignore checksum file
            allFiles.RemoveAll(file => file.EndsWith(ChecksumFile, StringComparison.OrdinalIgnoreCase));
            int total = allFiles.Count;
            int index = 1;

            foreach (var file in allFiles)
            {
                Program.PrintInfo($"Processing file {index++} / {total}", "📄");

                var relativePath = Path.GetRelativePath(templatesPath, file);
                var targetPath = Path.Combine(outputPath, relativePath);

                if (targetPath.EndsWith(".template", StringComparison.OrdinalIgnoreCase))
                    targetPath = targetPath[..^".template".Length];

                var targetDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir!);

                var content = File.ReadAllText(file);
                if (!PlaceholderDetector.TextContainsPlaceholders(content))
                {
                    File.Copy(file, targetPath, true);
                    continue;
                }
                foreach (var (key, value) in _replacements)
                {
                    content = content.Replace($"{{{{{key}}}}}", value, StringComparison.Ordinal);
                }
                File.WriteAllText(targetPath, content);
            }
        }
    }

    public static class PlaceholderDetector
    {
        private static readonly Regex _placeholderRegex = new(@"\{\{[A-Z_]+\}\}");

        public static bool TextContainsPlaceholders(string content)
        {
            return _placeholderRegex.IsMatch(content);
        }
    }
}
