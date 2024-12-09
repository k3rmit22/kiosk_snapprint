using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Drawing.Printing;
using Aspose.Pdf;
using Aspose.Pdf.Devices;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for printing_try.xaml
    /// </summary>
    public partial class printing_try : Window
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

        private DispatcherTimer _timer; // Timer to navigate after 10 seconds

        public printing_try(string filePath, string fileName, string pageSize, int pageCount,
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

            // Initialize the timer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Set interval to 10 seconds
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private async Task StartPrintingAsync(string filePath)
        {
            try
            {
                // Show loading overlay
                Dispatcher.Invoke(() => ShowLoading(true));

                await Task.Run(() =>
                {
                    // Perform the printing task here
                    PrintPdfFile(filePath);
                });

                // Hide loading overlay and show success message
                Dispatcher.Invoke(() =>
                {
                    ShowLoading(false);  // Hide the loading overlay
                    MessageBox.Show("Printing started successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    ShowLoading(false);  // Hide the loading overlay
                    MessageBox.Show($"An error occurred while printing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
        public void PrintPdfFile(string filePath)
        {
            // Start loading overlay
            Dispatcher.Invoke(() => ShowLoading(true));

            try
            {
                // Load the PDF document
                Document pdfDocument = new Document(filePath);

                // Set up printer settings
                PrinterSettings printerSettings = new PrinterSettings
                {
                    PrinterName = "Canon E470 series",   // Replace with your printer's name
                    Copies = (short)CopyCount // Set the number of copies
                };

                // Iterate through the selected pages and print each one
                foreach (var pageIndex in SelectedPages)
                {
                    // Ensure the pageIndex is within the valid range (1-based index)
                    if (pageIndex >= 1 && pageIndex <= pdfDocument.Pages.Count)
                    {
                        using (MemoryStream pageStream = new MemoryStream())
                        {
                            // Convert the selected page to an image
                            Resolution resolution = new Resolution(300); // Set the resolution (DPI)

                            // Check the ColorStatus and decide whether to print in color or grayscale
                            if (ColorStatus.ToLower() == "colored")
                            {
                                // Use JpegDevice for color printing
                                JpegDevice jpegDevice = new JpegDevice(resolution);
                                jpegDevice.Process(pdfDocument.Pages[pageIndex], pageStream);
                            }
                            else if (ColorStatus.ToLower() == "greyscale")
                            {
                                // Use PngDevice for grayscale printing
                                PngDevice pngDevice = new PngDevice(resolution);
                                pngDevice.Process(pdfDocument.Pages[pageIndex], pageStream);
                            }
                            else
                            {
                                // Default to color if no valid status is set
                                JpegDevice jpegDevice = new JpegDevice(resolution);
                                jpegDevice.Process(pdfDocument.Pages[pageIndex], pageStream);
                            }

                            // Print the image of the selected page
                            PrintPage(pageStream, printerSettings);
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Page {pageIndex} is out of range.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"An error occurred while printing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                // Hide loading overlay after the printing is done
                Dispatcher.Invoke(() => ShowLoading(false));
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop(); // Stop the timer once it ticks
            NavigateToHome();
        }

        private void NavigateToHome()
        {
            // Navigate back to the home user control
            HomeUserControl home = new HomeUserControl();

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = home;
            }
            else
            {
                MessageBox.Show("MainWindow instance is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
