using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.PdfViewer;
using iText.Kernel.Pdf; 
using iText.Layout;
using iText.Layout.Element;


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

        public string Colorstatus { get; private set; }

        private List<int> selectedPages = new List<int>();

        public PDFDisplay(string filePath, string fileName, string pageSize, int pageCount, string colorstatus)
        {
            InitializeComponent();
            

            // Store the file details
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
            Colorstatus = colorstatus; 

            // Initialize the copy count display
            pdfViewer.ZoomMode = Syncfusion.Windows.PdfViewer.ZoomMode.FitPage;

            PopulatePageCheckboxes(filePath);
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

        // Method to populate checkboxes for each page in the PDF
        private void PopulatePageCheckboxes(string filePath)
        {
            try
            {
                // Open the PDF to get the page count using iText7
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(filePath)))
                {
                    PageCount = pdfDoc.GetNumberOfPages(); // Get the total page count
                    for (int i = 1; i <= PageCount; i++)
                    {
                        // Create a CheckBox for each page
                        CheckBox pageCheckBox = new CheckBox
                        {
                            Content = $"Page {i}",
                            FontSize = 15,
                            Tag = i // Store the page number in the Tag for later retrieval
                        };

                        // Add an event handler for checking/unchecking
                        pageCheckBox.Checked += PageCheckBox_Checked;
                        pageCheckBox.Unchecked += PageCheckBox_Unchecked;

                        // Add the CheckBox to the ListBox (or StackPanel, depending on your design)
                        PageSelectionStackPanel.Children.Add(pageCheckBox); // Assuming you use a StackPanel
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error reading PDF: {ex.Message}");
            }
        }

        // Event handler for when a checkbox is checked
        private void PageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag != null)
            {
                int pageNumber = (int)checkBox.Tag;
                if (!selectedPages.Contains(pageNumber)) // Prevent duplicate entries
                {
                    selectedPages.Add(pageNumber);
                }
            }
        }
        private void PageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag != null)
            {
                int pageNumber = (int)checkBox.Tag;
                selectedPages.Remove(pageNumber);
            }
        }




        private void PROCEED_Click(object sender, RoutedEventArgs e)
        {
            // Get the number of selected pages
            int numberOfSelectedPages = selectedPages.Count;

            // Create an instance of the qrcode UserControl and pass the session I
            QR_preferences QR_preferences = new QR_preferences(FilePath, FileName, PageSize, PageCount, Colorstatus, selectedPages, numberOfSelectedPages);

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the QR page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = QR_preferences;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }


        }
    }

}
