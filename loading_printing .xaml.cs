using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using Aspose.Pdf;
using Aspose.Pdf.Devices;




namespace kiosk_snapprint
{
    public partial class loading_printing : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public int PageCount { get; private set; }
        public string ColorStatus { get; private set; }
        public int NumberOfSelectedPages { get; private set; }
        public int CopyCount { get; private set; }
        public List<int> SelectedPages { get; private set; }
        public double TotalPrice { get; private set; }

        public loading_printing(string filePath, string fileName, string pageSize, int pageCount,
                                string colorStatus, int numberOfSelectedPages, int copyCount,
                                List<int> selectedPages, double totalPrice)
        {
            InitializeComponent();

            // Store passed values
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
            ColorStatus = colorStatus;
            NumberOfSelectedPages = numberOfSelectedPages;
            CopyCount = copyCount;
            SelectedPages = selectedPages;
            TotalPrice = totalPrice;

            // Start the printing task asynchronously
            Task.Run(() => StartPrintingAsync(FilePath));
        }

        private async Task StartPrintingAsync(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    // Perform the printing task here
                    PrintPdfFile(filePath);
                });

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Printing started successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"An error occurred while printing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        public void PrintPdfFile(string filePath)
        {
            // Load the PDF document
            Document pdfDocument = new Document(filePath);

            // Set up printer settings
            PrinterSettings printerSettings = new PrinterSettings
            {
                PrinterName = "EPSON L3110 Series", // Replace with your printer's name or leave empty for the default printer
                Copies = (short)CopyCount // Set the number of copies
            };

            // Iterate through pages and print each one
            for (int pageIndex = 1; pageIndex <= pdfDocument.Pages.Count; pageIndex++)
            {
                using (MemoryStream pageStream = new MemoryStream())
                {
                    // Convert each page to an image
                    Resolution resolution = new Resolution(300); // Set the resolution (DPI)
                    JpegDevice jpegDevice = new JpegDevice(resolution);
                    jpegDevice.Process(pdfDocument.Pages[pageIndex], pageStream);

                    // Print the image
                    PrintPage(pageStream, printerSettings);
                }
            }
        }


        private void PrintPage(Stream pageStream, PrinterSettings printerSettings)
        {
            // Convert the memory stream to an image
            using (var image = System.Drawing.Image.FromStream(pageStream))
            {
                // Create a PrintDocument object
                PrintDocument printDocument = new PrintDocument
                {
                    PrinterSettings = printerSettings
                };

                printDocument.PrintPage += (sender, e) =>
                {
                    // Draw the image on the page
                    e.Graphics.DrawImage(image, e.PageBounds);
                };

                // Print the page
                printDocument.Print();
            }
        }

        private void ShowLoading(bool isLoading)
        {
            if (isLoading)
            {
                LoadingOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}






