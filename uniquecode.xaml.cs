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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for uniquecode.xaml
    /// </summary>
    public partial class uniquecode : UserControl
    {
        public uniquecode()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
           
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            
            mainWindow.MainContent.Content = new HomeUserControl(); 
        }
    }
}
