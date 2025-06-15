using PhaserIDE.Services.Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace PhaserIDE.Services
{
    internal class TemplatePlaceholderAnalyzer : ITemplatePlaceholderAnalyzer
    {
        private static readonly Regex PlaceholderRegex = new(@"\{\{(.[^\s]+?)\}\}", RegexOptions.Compiled);
        private static readonly string TemplateFileExtension = "*.template";

        /// <summary>
        /// Finds all placeholders within *.template files in rootDirectory.
        /// </summary>
        /// <param name="rootDirectory">Fully qualified path to target directory.</param>
        /// <returns>Dictionary containing all placeholder names as keys, list of file paths as value</returns>
        public async Task<Dictionary<string, List<string>>> AnalyzeAsync(string rootDirectory)
        {
            var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            var files = Directory.EnumerateFiles(rootDirectory, TemplateFileExtension, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string content;
                try
                {
                    content = await File.ReadAllTextAsync(file);
                }
                catch (Exception)
                {
                    // Skip files that cannot be read
                    continue;
                }

                foreach (Match match in PlaceholderRegex.Matches(content))
                {
                    var placeholder = match.Groups[1].Value;

                    if (!result.TryGetValue(placeholder, out var list))
                    {
                        list = new List<string>();
                        result[placeholder] = list;
                    }

                    if (!list.Contains(file))
                        list.Add(file);
                }
            }

            return result;
        }
    }
}
