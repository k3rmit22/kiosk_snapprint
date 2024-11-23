using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.PdfViewer; 


namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for PDFDisplay.xaml
    /// </summary>
    public partial class PDFDisplay : UserControl
    {
        private int copyCount = 1;
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public int PageCount { get; private set; }


        private List<int> selectedPages = new List<int>();



        public PDFDisplay(string filePath, string fileName, string pageSize, int pageCount)
        {
            InitializeComponent();
            

            // Store the file details
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;

            // Initialize the copy count display
            CopyCountTextBlock.Text = copyCount.ToString();
            PopulatePageSelection();

            pdfViewer.ZoomMode = Syncfusion.Windows.PdfViewer.ZoomMode.FitPage;




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


        // Method to populate the ComboBox with page options and 'Print All Pages'
     


        // Event handler for decreasing the number of copies
        private void DecreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            if (copyCount > 1) // Ensure copy count doesn't go below 1
            {
                copyCount--;
                UpdateCopyCountDisplay();
            }
        }

        private void IncreaseCopyCount_Click(object sender, RoutedEventArgs e)
        {
            if (copyCount < 5) // Limit the copy count to a maximum of 5
            {
                copyCount++;
                UpdateCopyCountDisplay();
            }

        }

        // Updates the display of the current copy count
        private void UpdateCopyCountDisplay()
        {
            CopyCountTextBlock.Text = copyCount.ToString();
        }

        private void PopulatePageSelection()
        {
            // Clear existing items
            PageSelectionListBox.Items.Clear();

            // Add options for each page
            for (int i = 1; i <= PageCount; i++)
            {
                PageSelectionListBox.Items.Add(new ListBoxItem { Content = $"Page {i}" });
            }

            // Add the 'Print All Pages' option
            PageSelectionListBox.Items.Add(new ListBoxItem { Content = "Print All Pages" });
        }


        private void PageSelectionListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var selectedPages = new List<int>(); // List to store the selected page numbers

            // Check if "Print All Pages" is selected
            bool printAllPagesSelected = PageSelectionListBox.SelectedItems.Contains(PageSelectionListBox.Items[PageCount]); // 'Print All Pages' index

            // If 'Print All Pages' is selected, include all pages
            if (printAllPagesSelected)
            {
                selectedPages.Clear(); // Clear existing selections
                for (int i = 1; i <= PageCount; i++) // Add all pages
                {
                    selectedPages.Add(i);
                }
            }
            else
            {
                // Loop through the selected items and add their corresponding page numbers
                foreach (ListBoxItem item in PageSelectionListBox.SelectedItems)
                {
                    // Try to extract the page number from the item content
                    var content = item.Content.ToString();
                    if (content.Contains("Page"))
                    {
                        var pageNumber = int.Parse(content.Replace("Page", "").Trim());
                        selectedPages.Add(pageNumber);  // Add the page number to the list
                    }
                }
            }

            // Now, selectedPages contains all the selected pages
            // You can use selectedPages for computation or sending to the next user control
            // For example, you can display the selected pages or use it for print processing:
            Console.WriteLine("Selected Pages: " + string.Join(", ", selectedPages));

            // Do something with selectedPages, e.g., pass it to the print function
        }

        private void PROCEED_Click(object sender, RoutedEventArgs e)
        {
            

            // Create an instance of the qrcode UserControl and pass the session I
            HomeUserControl HomeUserControl = new HomeUserControl();

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the QR page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = HomeUserControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }


        }
    }

}
