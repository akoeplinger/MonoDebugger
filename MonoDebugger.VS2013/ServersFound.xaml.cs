using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MonoDebugger.VS2013
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
