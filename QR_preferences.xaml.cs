﻿using System;
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
using static kiosk_snapprint.qrcode;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for QR_preferences.xaml
    /// </summary>
    public partial class QR_preferences : UserControl
    {

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PageSize { get; set; }

        public int PageCount { get; set; }

        public string ColorStatus { get; set; }
        public List<int> SelectedPages { get; set; }
        public int NumberOfSelectedPages { get; set; }

        public int CopyCount { get; set; } = 1; 


        public QR_preferences(string filePath, string fileName,  string pageSize, int pageCount, string colorStatus, List<int> selectedPages, int numberOfSelectedPages)
        {
            InitializeComponent();

            
            // Assign data to properties
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
            ColorStatus = colorStatus;
            SelectedPages = selectedPages;
            NumberOfSelectedPages = numberOfSelectedPages;
            LoadPdf(FilePath);

            Loaded += QR_preferences_Loaded;
            System.Diagnostics.Debug.WriteLine("QR_preferences_Loaded triggered.");


            // Debug log the properties
            System.Diagnostics.Debug.WriteLine($"FilePath: {FilePath}");
            System.Diagnostics.Debug.WriteLine($"FileName: {FileName}");
            System.Diagnostics.Debug.WriteLine($"PageSize: {PageSize}");
            System.Diagnostics.Debug.WriteLine($"Pagecount: {PageCount}");
            System.Diagnostics.Debug.WriteLine($"ColorStatus: {ColorStatus}");
            System.Diagnostics.Debug.WriteLine($"NumberOfSelectedPages: {NumberOfSelectedPages}");
            System.Diagnostics.Debug.WriteLine($"CopyCount: {CopyCount}");

            // Log the selected pages as a comma-separated list
            if (SelectedPages != null && SelectedPages.Any())
            {
                System.Diagnostics.Debug.WriteLine($"SelectedPages: {string.Join(", ", SelectedPages)}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SelectedPages: None");
            }

        }
        private void LoadPdf(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    pdfViewer.Load(filePath); // Load the file into the PdfViewerControl
                }
                else
                {
                    ShowError("The specified PDF file does not exist or the path is invalid.");
                }
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

        private void IncreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            if (CopyCount < 5)
            {
                CopyCount++;
            }

            // Update the CopyCountTextBox
            UpdateCopyCountDisplay();
        }

        private void DecreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            // Decrement the copy count, ensuring it doesn't go below 1
            if (CopyCount > 1)
            {
                CopyCount--;
            }

            // Update the CopyCountTextBox
            UpdateCopyCountDisplay();
        }

        private void UpdateCopyCountDisplay()
        {
            // Display the updated copy count in the TextBox
            CopyCountTextBox.Text = CopyCount.ToString();
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                PDFDisplay pdfDisplay;

                if (mainWindow.MainContent.Content is PDFDisplay existingPdfDisplay)
                {
                    pdfDisplay = existingPdfDisplay;
                    pdfDisplay.UpdatePdfDetails(FilePath, FileName, PageSize, PageCount, ColorStatus);
                }
                else
                {
                    pdfDisplay = new PDFDisplay(FilePath, FileName, PageSize, PageCount, ColorStatus);
                }

                // Reload the PDF explicitly
                await pdfDisplay.LoadPdfAsync(FilePath);

                mainWindow.MainContent.Content = pdfDisplay;
            }
            else
            {
                MessageBox.Show("MainWindow instance is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void QR_preferences_Loaded(object sender, RoutedEventArgs e)
        {
            // Only show the modal if ColorStatus equals "colored"
            if (ColorStatus.Equals("colored", StringComparison.OrdinalIgnoreCase))
            {
                // Show the color confirmation modal during load
                ColorConfirmationModal colorModal = new ColorConfirmationModal
                {
                    SelectedColorStatus = ColorStatus // Pass the current ColorStatus
                };

                // Set the owner of the modal to ensure it appears in front
                Window mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    colorModal.Owner = mainWindow;
                }

                bool? result = colorModal.ShowDialog();

                if (result == true && !string.IsNullOrEmpty(colorModal.SelectedColorStatus))
                {
                    // Update ColorStatus based on the user's choice in the modal
                    ColorStatus = colorModal.SelectedColorStatus;
                }
            }
        }



        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the qrcode UserControl and pass the session ID
             PricingQR pricingQR = new PricingQR(      
                filePath: FilePath,
                fileName: FileName,
                pageSize: PageSize,
                pageCount: PageCount,
                colorStatus: ColorStatus,
                numberOfSelectedPages: NumberOfSelectedPages,
                copyCount: CopyCount,
                selectedPages: SelectedPages);

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the QR page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = pricingQR;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }
        }
    }
}
