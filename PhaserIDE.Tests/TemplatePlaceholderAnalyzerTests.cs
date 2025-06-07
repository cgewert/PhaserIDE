using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using PhaserIDE.Services;

namespace PhaserIDE.Tests
{
    public class TemplatePlaceholderAnalyzerTests
    {
        [Fact]
        public async Task AnalyzeAsync_FindsPlaceholdersAndFiles()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var subDir = Path.Combine(tempDir, "sub");
            Directory.CreateDirectory(subDir);
            var file1 = Path.Combine(tempDir, "file1.template");
            var file2 = Path.Combine(subDir, "file2.template");

            await File.WriteAllTextAsync(file1, "Hello {{name}} and {{age}}");
            await File.WriteAllTextAsync(file2, "Another {{name}}");

            try
            {
                var analyzer = new TemplatePlaceholderAnalyzer();
                var result = await analyzer.AnalyzeAsync(tempDir);

                Assert.True(result.ContainsKey("name"));
                Assert.Contains(file1, result["name"]);
                Assert.Contains(file2, result["name"]);

                Assert.True(result.ContainsKey("age"));
                Assert.Contains(file1, result["age"]);
                Assert.Single(result["age"]);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
