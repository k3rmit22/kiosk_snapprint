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
        private int selectedPage = 0;
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

            // Initialize the copy count display
            CopyCountTextBlock.Text = copyCount.ToString();
            PopulatePageSelection();
          

           

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
        private void PopulatePageSelection()
        {
            // Clear existing items
            PageSelectionComboBox.Items.Clear();

            // Add options for each page
            for (int i = 1; i <= PageCount; i++)
            {
                PageSelectionComboBox.Items.Add(new ComboBoxItem { Content = $"Page {i}" });
            }

            // Add the 'Print All Pages' option
            PageSelectionComboBox.Items.Add(new ComboBoxItem { Content = "Print All Pages" });

            // Set the default selection to 'Print All Pages'
            PageSelectionComboBox.SelectedIndex = PageCount;  // 'Print All Pages' by default
        }

        // Handle ComboBox selection change
        private void PageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure selectedPage reflects the user's selection
            if (PageSelectionComboBox.SelectedItem != null)
            {
                // Assuming the ComboBox contains integers as items
                bool isParsed = int.TryParse(PageSelectionComboBox.SelectedItem.ToString(), out int page);
                selectedPage = isParsed ? page : 0; // Set selectedPage only if parsing succeeds
            }
            else
            {
                selectedPage = 0; // Default or fallback value
            }
        }






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




        private void PROCEED_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void NavigateToPricingQR(PricingQR pricingQR)
        {
           
        }







    }

}
