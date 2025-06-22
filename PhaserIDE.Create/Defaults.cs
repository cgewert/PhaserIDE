namespace PhaserIDE.Create
{
    internal static class Defaults
    {
        public const string DefaultProjectName = "new-phaser-project";
        public const string ProjectConfigFile = "project.config.json";

        public static readonly Dictionary<string, string> DefaultReplacements = new()
        {
            { "TITLE", DefaultProjectName },
            { "AUTHOR", "Author <author@example.com>" },
            { "YEAR", DateTime.Now.Year.ToString() },
            { "WEBPACK_DEV_SERVER_PORT", "9000" },
            { "PACKAGE_JSON_DESCRIPTION", "An application skeleton for 2D Game development with Phaser 3" },
            { "PHASER_VERSION", "3.90.0" }
        };
    }
}
