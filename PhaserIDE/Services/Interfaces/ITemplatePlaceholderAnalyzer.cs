namespace PhaserIDE.Services.Interfaces
{
    public interface ITemplatePlaceholderAnalyzer
    {
        Task<Dictionary<string, List<string>>> AnalyzeAsync(string rootDirectory);
    }

}
