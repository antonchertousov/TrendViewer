using System.Windows;
using TrendViewer.ViewModels;

namespace TrendViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IMainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            this.DataContext = mainWindowViewModel;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}