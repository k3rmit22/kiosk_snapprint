using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for browseFlashdrive.xaml
    /// </summary>
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

                    // Get all PDF files in the root of the flash drive (you can expand this to subdirectories if needed)
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

        // Handle double-click event to open the selected PDF file
        private void pdfFileListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedFile = pdfFileListView.SelectedItem as FileItem;
            if (selectedFile != null)
            {
                // You can open the selected file or perform other actions
                MessageBox.Show($"Selected PDF: {selectedFile.FileName}\nPath: {selectedFile.FilePath}");
                // Open the PDF or do further processing as required
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
