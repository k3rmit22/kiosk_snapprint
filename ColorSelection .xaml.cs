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

namespace kiosk_snapprint
{
   
    public partial class ColorSelection : Window
    {
        public string SelectedColor { get; private set; }
        public ColorSelection()
        {
            InitializeComponent();
        }

        // Event handler for the "Colored" button
        private void Colored_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = "Colored"; // Set to "Colored"
            this.DialogResult = true;  // Close the modal and set result to true
        }
        private void Greyscale_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = "Greyscale"; // Set to "Greyscale"
            this.DialogResult = true;  // Close the modal and set result to true
        }
    }
}
