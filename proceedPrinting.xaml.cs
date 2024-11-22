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
    public partial class proceedPrinting : Window
    {
        private string FilePath { get; set; }
        private string FileName { get; set; }

        public proceedPrinting(string filePath)
        {
            InitializeComponent();
            FilePath = filePath;

            // Extract the file name from the path
            FileName = System.IO.Path.GetFileName(filePath);

            // Use the FilePath for further processing (e.g., displaying the file details)
            DisplayFileDetails();
        }

        private void DisplayFileDetails()
        {
            // Display the file name
            selectedFilePathTextBlock.Text = $"{FileName}";
        }

        // Confirm button click handler
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Proceeding to print the file.");
            // Insert printing logic here
            this.DialogResult = true; // Close the modal with a positive result
            this.Close();
        }

        // Cancel button click handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Printing canceled.");
            this.DialogResult = false; // Close the modal with a negative result
            this.Close();
        }
    }
}