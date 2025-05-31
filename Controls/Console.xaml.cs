using PhaserIDE.ViewModels;
using System.Collections.ObjectModel;
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
        public ConsoleViewModel ConsoleViewModel { get; } = new();
        public static readonly Brush ConErrorBrush = Brushes.OrangeRed;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Console()
        {
            InitializeComponent();
            DataContext = ConsoleViewModel;
            Logs = ConsoleViewModel.Logs;
        }

        public ObservableCollection<(string line, bool isError)> Logs
        {
            get => (ObservableCollection<(string Line, bool isError)>)GetValue(LogsProperty);
            set => SetValue(LogsProperty, value);
        }

        public static readonly DependencyProperty LogsProperty =
           DependencyProperty.Register(
               nameof(Logs),
               typeof(ObservableCollection<(string Line, bool isError)>),
               typeof(Console),
               new PropertyMetadata(null, OnLogsChanged));

        private static void OnLogsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Console)d;
            if (e.OldValue is ObservableCollection<(string, bool)> oldLogs)
                oldLogs.CollectionChanged -= control.Logs_CollectionChanged;
            if (e.NewValue is ObservableCollection<(string, bool)> newLogs)
                newLogs.CollectionChanged += control.Logs_CollectionChanged;
        }

        private void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RenderLogs();
        }

        private void RenderLogs()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(RenderLogs);
                return;
            }

            var para = (Paragraph)ConsoleOutputRichBox.Document.Blocks.FirstBlock;
            if (Logs != null)
            {
                var (line, isError) = Logs.LastOrDefault();
                var run = new Run(line) { Foreground = isError ? ConErrorBrush : Foreground };
                para.Inlines.Add(run);
                para.Inlines.Add(new LineBreak());
            }
            ConsoleOutputRichBox.ScrollToEnd();
        }

        public void Clear()
        {
            ConsoleOutputRichBox.Document.Blocks.Clear();
            ConsoleOutputRichBox.Document.Blocks.Add(new Paragraph());
        }

        private void ConsoleOutputRichBox_Loaded(object sender, RoutedEventArgs e)
        {
            Clear();
            Logs.Add(("Welcome to Phaser IDE!\n\n" +
                "This is the console output area. You can see the output of the Phaser CLI here.\n" +
                "If you encounter any errors, they will be displayed in red.\n\n" +
                "To create a new Phaser project, fill out the form and click 'Create Project'.\n\n", false));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
