using Syncfusion.Windows.PdfViewer;
using System;
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
        public int SelectedPages { get; set; }

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
        public void SetPreferences(string fileName, string pageSize, string colorMode, int selectedPages, byte[] fileBytes)
        {
            Debug.WriteLine($"SetPreferences called with the following details:");
            Debug.WriteLine($"FileName: {fileName}");
            Debug.WriteLine($"PageSize: {pageSize}");
            Debug.WriteLine($"ColorMode: {colorMode}");
            Debug.WriteLine($"SelectedPages: {selectedPages}");
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
            int currentCount = int.Parse(CopyCountTextBox.Text);
            Debug.WriteLine($"Increase copy count: {currentCount} -> {currentCount + 1}");
            CopyCountTextBox.Text = (currentCount + 1).ToString();
        }

        private void DecreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            int currentCount = int.Parse(CopyCountTextBox.Text);
            if (currentCount > 1)
            {
                Debug.WriteLine($"Decrease copy count: {currentCount} -> {currentCount - 1}");
                CopyCountTextBox.Text = (currentCount - 1).ToString();
            }
            else
            {
                Debug.WriteLine("Copy count cannot go below 1.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Back button clicked.");
            // Handle Back button click
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Next button clicked.");

            // Get the copy count from the CopyCountTextBox
            int copyCount = int.Parse(CopyCountTextBox.Text);
            Debug.WriteLine($"Copy Count: {copyCount}");

            // Create the uniquePricing control
            uniquePricing pricingPage = new uniquePricing(
                FileName,
                PageSize,
                ColorMode,
                SelectedPages,
                copyCount,
                GetFileBytes()
            );

            // Access the MainWindow and set the content
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content of MainContent to the new UserControl (uniquePricing)
                mainWindow.MainContent.Content = pricingPage;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }
        }

    }
}
