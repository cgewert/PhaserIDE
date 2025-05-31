using PhaserIDE.Services;
using System.Windows.Controls;

namespace PhaserIDE.Views
{
    /// <summary>
    /// Interaktionslogik für EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool ShowLineNumbers
        {
            get => (bool)SettingsService.ShowLineNumbers;
        }

        public double EditorFontSize
        {
            get => (double)SettingsService.EditorFontSize;
        }
    }
}
