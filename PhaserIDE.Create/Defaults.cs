namespace PhaserIDE.Create
{
    internal static class Defaults
    {
        public const string DefaultProjectName = "new-phaser-project";
        public const string ProjectConfigFile = "project.config.json";

        public static string Title { get; set; } = DefaultProjectName;
        public static string Author { get; set; } = "Author <author@example.com>";
        public static string Year { get; set; } = System.DateTime.Now.Year.ToString();
        public static string WebpackDevServerPort { get; set; } = "9000";
        public static string PackageJsonDescription { get; set; } = "An application skeleton for 2D Game development with Phaser 3";
        public static string SceneClassName { get; set; } = "DefaultScene";
        public static string SceneKey { get; set; } = "defaultScene";

        public static string PhaserVersion { get; set; } = "3.90.0";
    }
}
