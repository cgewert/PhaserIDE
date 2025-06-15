using PhaserIDE.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhaserIDE.ViewModels
{
    public class ConsoleViewModel : INotifyPropertyChanged, IConsoleViewModel
    {
        private bool _isConsoleVisible = true;

        public ConsoleViewModel()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<(string Text, bool IsError)> Logs { get; } = new();
        public void AddLog(string text, bool isError = false)
        {
            Logs.Add((text, isError));
        }

        public bool IsConsoleVisible
        {
            get => _isConsoleVisible;
            set
            {
                _isConsoleVisible = value;
                OnPropertyChanged(nameof(IsConsoleVisible));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
