using System.Windows;

namespace MonoDebugger.VS2013.Views
{
    /// <summary>
    /// Interaktionslogik für ServersFound.xaml
    /// </summary>
    public partial class ServersFound : Window
    {
        public ServersFoundViewModel ViewModel { get; set; }
        public ServersFound()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            ViewModel = new ServersFoundViewModel();
            DataContext = ViewModel;
            Closing += (o, e) => ViewModel.StopLooking();
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
