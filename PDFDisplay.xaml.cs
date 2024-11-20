using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.PdfViewer; // Ensure Syncfusion libraries are included

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for PDFDisplay.xaml
    /// </summary>
    public partial class PDFDisplay : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public int PageCount { get; private set; }

        public PDFDisplay(string filePath, string fileName, string pageSize, int pageCount)
        {
            InitializeComponent();

            // Store the file details
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
        }

        public async Task LoadPdfAsync(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    // Ensure pdfViewer is accessed on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        if (pdfViewer != null)
                        {
                            pdfViewer.Load(filePath);
                        }
                        else
                        {
                            throw new InvalidOperationException("PDF Viewer instance is not initialized.");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, invalid format)
                ShowError($"Error loading PDF: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            // Implement a mechanism to show errors, such as a message box or logging
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
