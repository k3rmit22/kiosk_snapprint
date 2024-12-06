using iTextSharp.text.pdf;
using iTextSharp.text;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace kiosk_snapprint
{
    public partial class PdfDisplayPage : UserControl
    {
        private MemoryStream _pdfStream;
        private string _fileName;
        private string _pageSize; // Change from _fileSize to _pageSize
        private string _colorMode;
        private int _totalPages;

        public PdfDisplayPage(byte[] fileBytes, string fileName = null)
        {
            InitializeComponent();
            DataContext = this;

            // Check if fileBytes are null or empty
            if (fileBytes == null || fileBytes.Length == 0)
            {
                MessageBox.Show("PDF data is empty or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // If fileName is provided, use it; otherwise, use a default name
            _fileName = !string.IsNullOrEmpty(fileName) ? fileName : "Unknown PDF";

            // Debugging the final value of _fileName
            Console.WriteLine($"Final FileName: {_fileName}");

            // Display the PDF with the filename
            DisplayPdf(fileBytes, _fileName);
        }

        private void DisplayPdf(byte[] fileBytes, string fileName)
        {
            try
            {
                _pdfStream = new MemoryStream(fileBytes);

                // Check if the stream is valid
                if (_pdfStream.Length == 0)
                {
                    MessageBox.Show("Invalid PDF stream.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Set the file name
                _fileName = fileName;

                // Load the PDF into the PdfViewerControl (Syncfusion)
                PdfViewerControl.Load(_pdfStream);

                // Get total pages from the PDF
                _totalPages = PdfViewerControl.PageCount;

                // Detect color mode and page size using iTextSharp
                _colorMode = DetectColorMode(fileBytes);
                _pageSize = DetectPageSize(fileBytes); // Store the page size here

                PopulatePageSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string DetectColorMode(byte[] fileBytes)
        {
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    var page = reader.GetPageN(i);
                    var resources = page.GetAsDict(PdfName.RESOURCES);
                    var xObject = resources?.GetAsDict(PdfName.XOBJECT);

                    // Check if there are any color elements in the page
                    if (xObject != null)
                    {
                        foreach (var key in xObject.Keys)
                        {
                            var obj = xObject.GetAsStream(key);
                            if (obj != null && obj.Length > 0)
                            {
                                var colorSpace = obj.Get(PdfName.COLORSPACE);
                                if (colorSpace != null && colorSpace.ToString().Contains("DeviceRGB"))
                                {
                                    // If an image or graphics are in color, return "Colored"
                                    return "Colored";
                                }
                            }
                        }
                    }

                    // Now check for colored text in the page
                    var contentBytes = reader.GetPageContent(i);
                    string content = System.Text.Encoding.Default.GetString(contentBytes);

                    // Check if the content contains any color values (non-black colors)
                    if (content.Contains("rg") || content.Contains("RG"))  // "rg" or "RG" is used in PDF for color definitions
                    {
                        return "Colored"; // If color values are found in the content, mark as "Colored"
                    }
                }
            }

            return "Grayscale"; // If no color elements found, consider it grayscale
        }

        // Detect the page size using iTextSharp
        private string DetectPageSize(byte[] fileBytes)
        {
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                var page = reader.GetPageSizeWithRotation(1); // Get the size of the first page
                float pageWidth = page.Width;
                float pageHeight = page.Height;

                // Add a tolerance to handle minor variations in size
                float tolerance = 2.0f; // Allow for a small difference

                // Standard page sizes (in points)
                if (Math.Abs(pageWidth - 595) < tolerance && Math.Abs(pageHeight - 842) < tolerance)
                    return "A4"; // A4
                else if (Math.Abs(pageWidth - 612) < tolerance && Math.Abs(pageHeight - 792) < tolerance)
                    return "Short"; // Letter
                else if (Math.Abs(pageWidth - 612) < tolerance && Math.Abs(pageHeight - 1008) < tolerance)
                    return "Long"; // Legal
                else if (Math.Abs(pageWidth - 2550) < tolerance && Math.Abs(pageHeight - 3300) < tolerance)
                    return "A3"; // A3 (if you want to support more sizes)

                // Return "Unknown" if the page size doesn't match any known sizes
                return "Unknown";
            }
        }

        // Populate the ComboBox with page numbers
        private void PopulatePageSelection()
        {
            for (int i = 1; i <= _totalPages; i++)
            {
                PageSelectionComboBox.Items.Add(i);
            }

            if (_totalPages > 0)
            {
                PageSelectionComboBox.SelectedIndex = 0; // Select the first page
            }
        }

        // Handle page selection event
        private void PageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSelectionComboBox.SelectedItem != null)
            {
                int selectedPage = (int)PageSelectionComboBox.SelectedItem;
                PdfViewerControl.GotoPage(selectedPage);
            }
        }

        // Back button click handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = new uniquecode();
        }

        // Proceed button click handlertt
        // Inside the PROCEED_Click event handler, modify it to pass the fileBytes along with other preferences
        private void PROCEED_Click(object sender, RoutedEventArgs e)
        {
            // Determine the selected pages
            int selectedPages = 0;
            if (PrintAllPagesCheckBox.IsChecked == true)
            {
                selectedPages = _totalPages; // All pages are selected
            }
            else if (PageSelectionComboBox.SelectedItem != null)
            {
                selectedPages = (int)PageSelectionComboBox.SelectedItem; // Selected page
            }

            // Log the selected pages for debugging
            Console.WriteLine($"Selected Pages: {selectedPages}");

            // Ensure fileBytes are extracted from the MemoryStream
            byte[] fileBytes = _pdfStream.ToArray();

            // Create an instance of uniquePreferences and pass all values including fileBytes
            uniquePreferences preferencesPage = new uniquePreferences();
            preferencesPage.SetPreferences(_fileName, _pageSize, _colorMode, selectedPages, fileBytes);

            // Navigate to uniquePreferences
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = preferencesPage; // Assuming MainContent is a container for UserControls
        }





        // Dispose of the MemoryStream when the UserControl is unloaded
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _pdfStream?.Dispose();
        }

        // Print all pages checkbox change event
        // Handle checkbox change for enabling/disabling the combo box
        private void PrintAllPagesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // When the checkbox is checked, disable the page selection combo box
            PageSelectionComboBox.IsEnabled = !PrintAllPagesCheckBox.IsChecked.GetValueOrDefault();
        }

        // Handle checkbox change for enabling/disabling the combo box
        private void PrintAllPagesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // When the checkbox is unchecked, enable the page selection combo box
            PageSelectionComboBox.IsEnabled = true;
        }

        private int _copyCount = 1;
        private const int _maxCopies = 10;

        
    }
}