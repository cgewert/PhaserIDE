using CommandLine;

namespace PhaserIDE.Create
{
    class Program
    {
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
                        Console.WriteLine($"[ERROR] Target directory '{targetPath}' already exists and is not empty.");
                        return false;
                    }
                } 
                else
                {
                    Console.WriteLine($"Creating projects directory: {targetPath}");
                    Directory.CreateDirectory(targetPath);
                }

                // Find template directory
                var templateDir = Path.Combine(AppContext.BaseDirectory, "templates");
                Console.WriteLine($"📁 Create Project: {projectName}");
                Console.WriteLine($"📂 Destination path: {targetPath}");
                Console.WriteLine($"📂 Template path: {templateDir}");

                //    // Load configuration if provided
                //    if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                //    {
                //        var configContent = File.ReadAllText(configPath);
                //        Console.WriteLine($"Configuration loaded from: {configPath}\n{configContent}");
                //    }
                //    else
                //    {
                //        Console.WriteLine("No configuration file provided or found. Using defaults.");
                //    }
                // Optional: config file anlegen
                //var configPath = !string.IsNullOrWhiteSpace(options.ConfigPath)
                //    ? options.ConfigPath
                //    : Path.Combine(targetPath, "project.config.json");

                //File.WriteAllText(configPath, "{ \"project\": \"" + options.Name + "\" }");

                Console.WriteLine($"✅ Done. Project folder created at: {targetPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR] {ex.Message}");
                return false;
            }
        }
        

        //                let replacements: Record<string, string> = { }
        //                ;

        //                if (configPath)
        //                {
        //                    // Prio 1: Transfer user-defined config
        //                    replacements = await loadProjectConfig(configPath);
        //                    console.log("📝 Config loaded from parameter:", configPath);
        //                }
        //                else
        //                {
        //                    const found = await findProjectConfigFile(projectPath);
        //                    if (found)
        //                    {
        //                        // Prio 2: Use automatically found config
        //                        replacements = await loadProjectConfig(found);
        //                        console.log("🔍 Config automatically found:", relative(baseDir, found));
        //                    }
        //                    else
        //                    {
        //                        // Prio 3: Use default values
        //                        replacements = DEFAULT_REPLACEMENTS;
        //                        console.log("📄 No config found – using default values.");
        //                    }
        //                }

        //                // Create all files in the project directories
        //                const engine = new TemplateEngine(replacements);
        //                const runner = new TaskRunner();

        //                // Task 1: Generating templates
        //                runner.add("Generating project files", async () => {
        //                    await engine.processAll(templateDir, projectPath);
        //                });

        //                // Task 2: Initializing NPM (if available)
        //                runner.add("Installing NPM", async () => {
        //                if (await isCommandAvailable("npm"))
        //                {
        //                    console.log("📦 NPM recognized. Initializing...");
        //                    const cmd = new Deno.Command("npm", {
        //        args: ["install"],
        //        cwd: projectPath,
        //        stdout: "inherit",
        //        stderr: "inherit",
        //      });
        //                await cmd.output();
        //            } else
        //            {
        //                console.warn("⚠️  NPM not found – step skipped.");
        //            }
        //        });

        //  // Task 3: Initializing Git (if available)
        //  runner.add("Initializing Git", async() => {
        //    if (await isCommandAvailable("git")) {
        //      console.log("🔧 Git recognized. Initializing...");
        //      const cmd = new Deno.Command("git", {
        //        args: ["init"],
        //        cwd: projectPath,
        //        stdout: "inherit",
        //        stderr: "inherit",
        //      });
        //      await cmd.output();
        //} else
        //{
        //    console.warn("⚠️  Git not found – step skipped.");
        //}
        //  });

        //// Execute tasks
        //await runner.runAll();

        //console.log(
        //    `✅ Project '${projectName}' successfully generated at: ${ projectPath}`
        //  );
        //}
    }
}
