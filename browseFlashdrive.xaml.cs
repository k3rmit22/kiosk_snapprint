using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace kiosk_snapprint
{
    public partial class browseFlashdrive : UserControl
    {
        // ObservableCollection to bind the ListView
        public ObservableCollection<FileItem> PdfFiles { get; set; }

        public browseFlashdrive()
        {
            InitializeComponent();

            // Initialize the collection for the ListView binding
            PdfFiles = new ObservableCollection<FileItem>();
            pdfFileListView.ItemsSource = PdfFiles;

            // Get the flash drive path and populate the PDF files
            PopulatePdfFiles();
        }

        private void PopulatePdfFiles()
        {
            try
            {
                // Find the removable drive (flash drive) by checking for connected drives
                var flashDrive = DriveInfo.GetDrives()
                                          .FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Removable);

                if (flashDrive != null)
                {
                    string flashDrivePath = flashDrive.RootDirectory.FullName;

                    // Get all PDF files in the root of the flash drive
                    var pdfFiles = Directory.GetFiles(flashDrivePath, "*.pdf", SearchOption.TopDirectoryOnly)
                                             .Select(filePath => new FileItem
                                             {
                                                 FileName = Path.GetFileName(filePath),
                                                 FilePath = filePath
                                             })
                                             .ToList();

                    // Populate the ListView with the PDF files
                    PdfFiles.Clear();
                    foreach (var pdfFile in pdfFiles)
                    {
                        PdfFiles.Add(pdfFile);
                    }
                }
                else
                {
                    MessageBox.Show("No removable flash drive detected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during file access
                MessageBox.Show($"Error accessing flash drive: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

      
        // Handle single click (selection) to load proceedPrinting UserControl
        private void pdfFileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFile = pdfFileListView.SelectedItem as FileItem;
            if (selectedFile != null)
            {
                // Create a new instance of proceedPrinting UserControl and pass the file path to it
                var proceedPrintingControl = new proceedPrinting(selectedFile.FilePath);

                // Assuming you have a ContentControl in the parent (such as MainWindow or another container)
                var parentControl = this.Parent as ContentControl;
                if (parentControl != null)
                {
                    // Set the new UserControl as the content of the parent control
                    parentControl.Content = proceedPrintingControl;
                }
            }
        }

    }

    // Helper class to hold file information
    public class FileItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
