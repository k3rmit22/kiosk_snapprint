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
           

            // Assuming the main window contains a placeholder (like a Grid or a StackPanel) to display the PDFControl
            // Get the main window instance
            var mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Create an instance of the PDFDisplay UserControl and pass the file path
                var pdfDisplay = new PDFDisplay(FilePath, FileName, "A4", 5);  // Pass file details and sample page count
                pdfDisplay.LoadPdfAsync(FilePath);  // Load the PDF into the viewer

                // Assuming there is a container in the main window (e.g., a Grid) named 'MainContent'
                mainWindow.MainContent.Content = pdfDisplay;  // Add the PDFDisplay control to the main window

           
                this.Close();
            }
            // Create an instance of the qrcode UserControl and pass the session I
            HomeUserControl HomeUserControl = new HomeUserControl();

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the QR page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = HomeUserControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }
        }




        // Cancel button click handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
             // Create an instance of the qrcode UserControl and pass the session I
            HomeUserControl HomeUserControl = new HomeUserControl();

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the QR page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = HomeUserControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }
            
        }
    }
}