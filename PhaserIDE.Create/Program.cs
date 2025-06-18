using CommandLine;
using System.Diagnostics;
using System.Text.Json;

namespace PhaserIDE.Create
{
    class Program
    {
        public static string DefaultConfigName => "project.config.json";

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Init, Create>(args)
                .MapResult(
                (Init opts) => RunInit(opts),
                (Create opts) => RunCreate(opts),
                errs => 1);
        }

        private static int RunInit(Init options)
        {
            var ( name, output, config ) = ( options.Name, options.TargetDirectoryName, options.ConfigPath);
            string projectName = string.IsNullOrEmpty(name) ? Defaults.DefaultProjectName : name;
            string baseDir = string.IsNullOrEmpty(output) ? "." : output;
            InitProject(projectName, baseDir, config);

            return 0;
        }

        private static int RunCreate(Create opts)
        {
            Console.WriteLine("[CREATE] Not implemented yet.");
            return 0;
        }        

        private static bool InitProject(string projectName, string baseDir, string? configPath = null)
        {
            // Windows sucks and thats why we need to activate another codepage
            // to support UTF-8 characters in the console output
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                // Translate relative paths to absolute paths
                if (string.IsNullOrWhiteSpace(baseDir) || baseDir == ".")
                {
                    baseDir = Directory.GetCurrentDirectory();
                }
                else
                {
                    baseDir = Path.GetFullPath(baseDir);
                }
                var targetPath = Path.Combine(projectName, baseDir);

                if (Directory.Exists(targetPath))
                {
                    // Ensure that only empty directories are being used
                    if (Directory.GetFiles(targetPath).Length > 0 || Directory.GetDirectories(targetPath).Length > 0)
                    {
                        PrintError($"Target directory '{targetPath}' already exists and is not empty.");
                        return false;
                    }
                }
                else
                {
                    PrintInfo($"Creating projects directory: {targetPath}");
                    Directory.CreateDirectory(targetPath);
                }

                // Find template directory
                var templateDir = Path.Combine(AppContext.BaseDirectory, "templates");
                if (!CheckTemplateFolder(templateDir))
                {
                    PrintError($"Template directory '{templateDir}' does not exist or is invalid. Please ensure the template files are present.");
                    return false;
                }

                PrintInfo($"Create Project: {projectName}", "📁");
                PrintInfo($"Destination path: {targetPath}", "📁");
                PrintInfo($"Template path: {templateDir}", "📁");

                // Load configuration if provided
                var configContent = string.Empty;
                if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                {
                    configContent = File.ReadAllText(configPath);
                    PrintInfo($"Using configuration file: {configPath}", "📝");
                    PrintInfo($"Configuration content:\n{configContent}", "📝");
                }
                else
                {
                    var defaultConfigPath = Path.Combine(AppContext.BaseDirectory, DefaultConfigName);
                    PrintInfo($"Looking for a configuration file in application directory: {defaultConfigPath}", "🔍");
                    // Look for a default configuration file in the working directory
                    if (!File.Exists(defaultConfigPath))
                    {
                        PrintInfo($"No configuration file found in target directory. Using defaults.", "📝");
                    }
                    else
                    {
                        configPath = Path.Combine(AppContext.BaseDirectory, DefaultConfigName);
                        configContent = File.ReadAllText(configPath);
                        PrintInfo($"Using default configuration file: {configPath}", "📝");
                        PrintInfo($"Configuration content:\n{configContent}", "📝");
                    }
                }
                Dictionary<string, string>? replacements = ParseConfig(configContent);
                TemplateEngine? templateEngine = null;
                PrintInfo("Copying template files and replacing placeholders...", "📂");
                if (replacements != null)
                {
                    templateEngine = new TemplateEngine(replacements);
                }
                else
                {
                    // If no configuration was provided, use default values
                    templateEngine = new TemplateEngine(Defaults.DefaultReplacements);
                }
                templateEngine.RenderTemplates(templateDir, targetPath);

                // Try to run npm install if available
                if (IsToolAvailable("npm"))
                {
                    PrintInfo("NPM recognized. Initializing...", "📦");
                    var npmProcess = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c npm install",
                        WorkingDirectory = targetPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    };
                    using var npmProcessInstance = new Process { StartInfo = npmProcess };
                    if (npmProcessInstance != null)
                    {
                        npmProcessInstance.OutputDataReceived += (_, e) => { if (e.Data != null) PrintInfo($"{e.Data}"); };
                        npmProcessInstance.ErrorDataReceived += (_, e) => { if (e.Data != null) PrintError($"{e.Data}"); };
                        npmProcessInstance.Start();
                        npmProcessInstance.BeginOutputReadLine();
                        npmProcessInstance.BeginErrorReadLine();
                        Task.Run(() => npmProcessInstance.WaitForExitAsync()).GetAwaiter().GetResult();
                        if (npmProcessInstance.ExitCode != 0)
                        {
                            PrintError($"NPM install failed with exit code {npmProcessInstance.ExitCode}.");
                            return false;
                        }
                        PrintInfo("NPM install finished.", "📦");
                    }
                }
                else
                {
                    PrintInfo("NPM not found – step skipped.", "⚠️");
                }

                // Try to run git init if available
                if(IsToolAvailable("git"))
                {
                    PrintInfo("Git recognized. Initializing...", "🔧");
                    var gitProcess = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c git init",
                        WorkingDirectory = targetPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    };
                    using var gitProcessInstance = new Process { StartInfo = gitProcess };
                    if (gitProcessInstance != null)
                    {
                        gitProcessInstance.OutputDataReceived += (_, e) => { if (e.Data != null) PrintInfo($"{e.Data}"); };
                        gitProcessInstance.ErrorDataReceived += (_, e) => { if (e.Data != null) PrintError($"{e.Data}"); };
                        gitProcessInstance.Start();
                        gitProcessInstance.BeginOutputReadLine();
                        gitProcessInstance.BeginErrorReadLine();
                        Task.Run(() => gitProcessInstance.WaitForExitAsync()).GetAwaiter().GetResult();
                        if (gitProcessInstance.ExitCode != 0)
                        {
                            PrintError($"Git initialization failed with exit code {gitProcessInstance.ExitCode}.");
                            return false;
                        }
                        PrintInfo("Git initialization finished.", "🔧");
                    }
                }
                else
                {
                    PrintInfo("Git not found – step skipped.", "⚠️");
                }

                PrintSuccess($"Done. Project '{projectName}' successfully generated at: {targetPath}");

                return true;
            }
            catch (Exception ex)
            {
                PrintError($"An error occurred while initializing the project: {ex.Message}", "❌");
                return false;
            }
        }

        private static bool IsToolAvailable(string toolName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c where {toolName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if(process == null)
                {
                    return false;
                }

                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static Dictionary<string, string>? ParseConfig(string configContent)
        {
            if (!string.IsNullOrEmpty(configContent))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };
                return JsonSerializer.Deserialize<Dictionary<string, string>>(configContent, options);
            }

            return null;
        }

        private static bool CheckTemplateFolder(string path)
        {
            const string checksum = "a013492a04c60914b9186919d7c03ad2e0ce048ade77f61b357388539b77a15f";

            if (!Directory.Exists(path))
            {
                return false;
            }

            using var stream = File.OpenRead(Path.Combine(path, ".check"));
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(stream);
            var hashString = Convert.ToHexStringLower(hash);

            return checksum == hashString;
        }
        public static void PrintError(string message, string prefix = "❌")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();
        }
        public static void PrintInfo(string message, string prefix = "ℹ️")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();
        }
        public static void PrintSuccess(string message, string prefix = "✅")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();
        }

    }
}
