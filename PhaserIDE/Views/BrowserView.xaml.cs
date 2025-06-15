using System.Windows.Controls;

namespace PhaserIDE.Views
{
    /// <summary>
    /// Interaktionslogik für BrowserView.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        public BrowserView()
        {
            InitializeComponent();
            Browser.FrameLoadEnd += Browser_FrameLoadEnd;
            Browser.Loaded += Browser_Loaded;
            Browser.LoadError += Browser_LoadError;
        }

        private void Browser_LoadError(object? sender, CefSharp.LoadErrorEventArgs e)
        {
            ;
        }

        // Fires when browser was initialized
        private void Browser_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ;
        }

        // Fires when a document was loaded completely
        private void Browser_FrameLoadEnd(object? sender, CefSharp.FrameLoadEndEventArgs e)
        {
            ;
        }
    }
}
