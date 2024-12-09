using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace kiosk_snapprint
{
    public partial class uniquePreferences : UserControl
    {
        // Public properties to hold the values passed from PdfDisplayPage
        public string FileName { get; set; }
        public string PageSize { get; set; }
        public string ColorMode { get; set; }
        public List<int> SelectedPages { get; set; }
        private PricingQR _pricingQR;
        // PdfViewerControl to display the PDF
        private PdfViewerControl _pdfViewerControl;

        // Private field to hold the file bytes
        private byte[] _fileBytes;

        public uniquePreferences()
        {
            InitializeComponent();

            // Initialize the PdfViewerControl
            _pdfViewerControl = new PdfViewerControl();
            PdfContainer.Children.Add(_pdfViewerControl);  // Assuming you have a container named PdfContainer in XAML
        }

        // Method to set preferences and display the PDF
        public void SetPreferences(string fileName, string pageSize, string colorMode, List<int> selectedPages, byte[] fileBytes)
        {
            Debug.WriteLine($"SetPreferences called with the following details:");
            Debug.WriteLine($"FileName: {fileName}");
            Debug.WriteLine($"PageSize: {pageSize}");
            Debug.WriteLine($"ColorMode: {colorMode}");
            Debug.WriteLine($"SelectedPages: {string.Join(", ", selectedPages)}");
            Debug.WriteLine($"FileBytes Length: {fileBytes?.Length ?? 0}");

            // Assign values to properties
            FileName = fileName;
            PageSize = pageSize;
            ColorMode = colorMode;
            SelectedPages = selectedPages;

            // Store the file bytes in the private field
            _fileBytes = fileBytes;

            // Display the PDF in PdfViewerControl
            if (_fileBytes != null && _fileBytes.Length > 0)
            {
                Debug.WriteLine("Loading PDF into PdfViewerControl...");
                MemoryStream pdfStream = new MemoryStream(_fileBytes);
                _pdfViewerControl.Load(pdfStream);
            }
            else
            {
                MessageBox.Show("Invalid PDF data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine("Error: Invalid PDF data.");
            }
        }

        // Method to access the stored file bytes
        public byte[] GetFileBytes()
        {
            return _fileBytes;
        }
        private void IncreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CopyCountTextBox.Text, out int currentCount) && currentCount < 10)
            {
                UpdateCopyCount(1);
            }
            else
            {
                MessageBox.Show("The maximum copy count is 10.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Decrease the copy count
        private void DecreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            UpdateCopyCount(-1);
        }

        // Helper method to update the copy count
        private void UpdateCopyCount(int increment)
        {
            if (int.TryParse(CopyCountTextBox.Text, out int currentCount) && currentCount > 0)
            {
                currentCount += increment;
                if (currentCount < 1) currentCount = 1;  // Ensure the count is not less than 1
                Debug.WriteLine($"Updated copy count: {currentCount}");
                CopyCountTextBox.Text = currentCount.ToString();
            }
            else
            {
                Debug.WriteLine("Invalid copy count.");
            }
        }

        // Back button click handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Back button clicked.");
            // Handle Back button click (e.g., navigate back)
        }

        // Next button click handler
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Next button clicked.");

            // Get the copy count from the CopyCountTextBox
            if (!int.TryParse(CopyCountTextBox.Text, out int copyCount) || copyCount < 1)
            {
                MessageBox.Show("Please specify a valid copy count.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Debug.WriteLine($"Copy Count: {copyCount}");


            // Create a List<int> for selected pages
            List<int> selectedPageList = SelectedPages;

            // Create the uniquePricing control
            _pricingQR = new PricingQR(
                 filePath: FileName,           // FilePath
                 fileName: FileName,           // FileName
                 pageSize: PageSize,           // PageSize
                 colorStatus: ColorMode,       // ColorMode
                 numberOfSelectedPages: SelectedPages.Count, // SelectedPages (Corrected to number of selected pages)
                 copyCount: copyCount,         // CopyCount
                 selectedPages: selectedPageList, // SelectedPages list
                 pageCount: SelectedPages.Count  // PageCount (assuming SelectedPages is the correct count)
             );

            // Set the filename in the PricingQR control
            _pricingQR.SetFileName(FileName);

            // Access the MainWindow and set the content
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content of MainContent to the new UserControl (PricingQR)
                mainWindow.MainContent.Content = _pricingQR;
            }
            else
            {
                MessageBox.Show("MainWindow instance is not available.");
            }

            // Now, process preferences via PreferencesProcessor
            PreferencesProcessor.ProcessPreferences(
                filePath: FileName,
                fileName: FileName,
                pageSize: PageSize,
                pageCount: SelectedPages.Count,
                colorStatus: ColorMode,
                selectedPages: SelectedPages.Count,
                numberOfSelectedPages: SelectedPages.Count,
                copyCount: copyCount
            );
        }

        // Helper method to generate selected pages list


        // PreferencesProcessor class definition
        public static class PreferencesProcessor
        {
            public static void ProcessPreferences(string filePath, string fileName, string pageSize, int pageCount, string colorStatus, int selectedPages, int numberOfSelectedPages, int copyCount)
            {
                // Your logic to process preferences here
                Debug.WriteLine($"Processing Preferences for {fileName}:");
                Debug.WriteLine($"File Path: {filePath}");
                Debug.WriteLine($"FileName: {fileName}");
                Debug.WriteLine($"PageSize: {pageSize}");
                Debug.WriteLine($"ColorStatus: {colorStatus}");
                Debug.WriteLine($"Selected Pages: {selectedPages}");
                Debug.WriteLine($"Copy Count: {copyCount}");

                // Additional logic can be added here based on these values, like saving preferences or calculating pricing.
            }
        }
    }
}