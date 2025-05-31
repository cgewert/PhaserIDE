using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PhaserIDE.Controls
{
    public partial class Console : UserControl, INotifyPropertyChanged
    {
        public static readonly Brush ConErrorBrush = Brushes.OrangeRed;

        private bool _isConsoleVisible = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Console()
        {
            InitializeComponent();
            DataContext = this;
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

        public void Clear()
        {
            ConsoleOutputRichBox.Document.Blocks.Clear();
            ConsoleOutputRichBox.Document.Blocks.Add(new Paragraph());
        }

        public void AppendLine(string text, bool isError = false)
        {
            var para = (Paragraph)ConsoleOutputRichBox.Document.Blocks.FirstBlock;

            var run = new Run(text) { Foreground = isError ? ConErrorBrush : Foreground };
            para.Inlines.Add(run);
            para.Inlines.Add(new LineBreak());
            ConsoleOutputRichBox.ScrollToEnd();
        }

        private void ConsoleOutputRichBox_Loaded(object sender, RoutedEventArgs e)
        {
            AppendLine("Welcome to Phaser IDE!\n\n" +
                "This is the console output area. You can see the output of the Phaser CLI here.\n" +
                "If you encounter any errors, they will be displayed in red.\n\n" +
                "To create a new Phaser project, fill out the form and click 'Create Project'.\n\n");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
