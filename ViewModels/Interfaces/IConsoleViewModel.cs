using System.Collections.ObjectModel;

namespace PhaserIDE.ViewModels.Interfaces
{
    public interface IConsoleViewModel
    {
        public bool IsConsoleVisible { get; set; }
        public ObservableCollection<(string Text, bool IsError)> Logs { get; }
        public void AddLog(string text, bool isError = false);
    }
}
