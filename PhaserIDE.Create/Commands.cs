using CommandLine;

namespace PhaserIDE.Create
{
    [Verb("init", HelpText = "Creates a new phaser project.")]
    internal class Init
    {
        [Option('n', "name", Required = true, HelpText = "Project name / Game name")]
        public string Name { get; set; } = Defaults.DefaultProjectName;

        [Option('o', "output", Required = true, HelpText = "Target directory")]
        public string TargetDirectoryName { get; set; } = string.Empty;

        [Option('c', "config", Required = false, HelpText = "Path to a configuration file. If not specified, defaults to 'project.config.json' in the target directory.")]
        public string ConfigPath { get; set; } = string.Empty;
    }

    [Verb("create", HelpText = "Create a new project component.")]
    internal class Create
    {

    }
}