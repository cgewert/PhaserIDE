using CommandLine;

namespace PhaserIDE.Create
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Init, Create>(args)
                .MapResult(
                (Init opts) => RunInit(opts),
                (Create opts) => RunCommitAndReturnExitCode(opts),
                errs => 1);
        }

        private static int RunCommitAndReturnExitCode(Create opts)
        {
            return 0;
        }

        private async static int RunInit(Init opts)
        {
            //.action(async (options) => {
            //     const { name, output, config } = options;
            //     const projectName = name ?? DEFAULT_PROJECT_NAME;
            //     const baseDir = output ?? ".";
            //     await initProject(projectName, baseDir, config);
            // });

//            async function initProject(
//  projectName: string,
//  baseDir: string,
//  configPath?: string
//) {
//                const projectPath = join(Deno.realPathSync(baseDir), projectName);
//                const execDir = dirname(Deno.execPath());
//                const templateDir = join(execDir, "src", "templates");

//                console.log(`📁 Create Project: ${ projectName}`);
//                console.log(`📂 Destination path: ${ projectPath}`);
//                console.log(`📂 Template path: ${ templateDir}`);

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
            Console.WriteLine($"Initializing project '{opts.Name}' in directory '{opts.TargetDirectoryName}' with config '{opts.ConfigPath}'");
            return 0;
        }
    }
}
