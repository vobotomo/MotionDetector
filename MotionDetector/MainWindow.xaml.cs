using System.Windows;
using MotionDetector.ViewModels;

namespace MotionDetector
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
        }

        private void Connect_Click(object sender, RoutedEventArgs e) => _vm.Connect();
        private void Disconnect_Click(object sender, RoutedEventArgs e) => _vm.Disconnect();
        private void RefreshPorts_Click(object sender, RoutedEventArgs e) => _vm.RefreshPorts();
    }
}