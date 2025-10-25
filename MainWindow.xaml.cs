using System.Windows;
using CoursesDekstopApp.viewModels; // Наш ViewModel

namespace CoursesDekstopApp
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            
            DataContext = viewModel;
        }
    }
}