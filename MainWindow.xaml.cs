//using PhaserIDE.Views;
using PhaserIDE.Views;
using System.Windows;
using System.Windows.Controls;

namespace PhaserIDE
{
    enum MenuEntries
    {
        Home,
        Editor,
        Browser,
        New,
        Projects,
        Settings
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Erste Ansicht laden
            LoadView(new HomeView());
            NavigationMenu.SelectedItem = Home;
        }

        private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationMenu.SelectedItem == null) return;

            var selectedItem = NavigationMenu.SelectedItem as ListBoxItem;
            var clickedElement = Enum.Parse<MenuEntries>(selectedItem.Name);
            PageTitle.Text = selectedItem.Name;
            switch (clickedElement)
            {
                case MenuEntries.Home:
                    LoadView(new HomeView());
                    break;
                case MenuEntries.Editor:
                    LoadView(new EditorView());
                    break;
                case MenuEntries.Browser:
                    LoadView(new BrowserView());
                    break;
                case MenuEntries.New:
                    LoadView(new NewView());
                    break;
                case MenuEntries.Projects:
                    LoadView(new ProjectsView());
                    break;
                case MenuEntries.Settings:
                    LoadView(new SettingsView());
                    break;
            }
        }

        private void LoadView(UserControl view)
        {
            MainContentPresenter.Content = view;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Neue Aktion wurde ausgelöst!", "Information",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}