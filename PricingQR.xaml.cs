using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace kiosk_snapprint
{
   
    public partial class PricingQR : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public string ColorStatus { get; private set; }
        public int NumberOfSelectedPages { get; private set; }
        public int CopyCount { get; private set; }
        public List<int> SelectedPages { get; private set; }

        private readonly Dictionary<string, int> ColorStatusValues = new Dictionary<string, int>
        {
            { "colored", 10 },
            { "greyscale", 5 }
        };

        private readonly Dictionary<string, int> PageSizeValues = new Dictionary<string, int>
        {
            { "a4", 5 },
            { "letter (short)", 5 },
            { "legal (long)", 10 }
        };

        public PricingQR(string filePath, string fileName, string pageSize, string colorStatus,
            int numberOfSelectedPages, int copyCount, List<int> selectedPages)
        {
            InitializeComponent();

            // Store data in properties
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            ColorStatus = colorStatus;
            NumberOfSelectedPages = numberOfSelectedPages;
            CopyCount = copyCount;
            SelectedPages = selectedPages;

            
            LoadSummary(FileName, PageSize, ColorStatus, SelectedPages, CopyCount);
        }

        private void LoadSummary(string fileName, string pageSize, string colorStatus, List<int> selectedPages, int copyCount)
        {
            // Normalize input values (automatically convert to lowercase and trim spaces)
            string normalizedPageSize = NormalizePageSize(pageSize);
            string normalizedColorStatus = NormalizeColorStatus(colorStatus);

            // Display data
            filename.Text = fileName;
            color_label.Text = normalizedColorStatus;
            pagesize_label.Text = normalizedPageSize;
            Copies_label.Text = copyCount.ToString();

            if (selected_pages_label != null && selectedPages != null)
            {
                // Display the selected pages as a comma-separated string
                selected_pages_label.Text = string.Join(", ", selectedPages);
            }

            // Compute the total price
            double totalPrice = ComputeTotalPrice(normalizedPageSize, normalizedColorStatus, selectedPages.Count, copyCount);
            System.Diagnostics.Debug.WriteLine($"totalprice: {totalPrice}");

            // Display the total price
            total_label.Text = $"{totalPrice}";
        }

        private string NormalizePageSize(string pageSize)
        {
            // Normalize: convert to lowercase and trim spaces
            return pageSize?.Trim().ToLower();
        }

        private string NormalizeColorStatus(string colorStatus)
        {
            // Normalize: convert to lowercase and trim spaces
            return colorStatus?.Trim().ToLower();
        }

        private double ComputeTotalPrice(string pageSize, string colorStatus, int numberOfSelectedPages, int copyCount)
        {
            // Get the numeric values for color status and page size
            if (ColorStatusValues.TryGetValue(colorStatus, out int colorValue) &&
                PageSizeValues.TryGetValue(pageSize, out int pageSizeValue))
            {
                // Formula: (Page Size Value + Color Value) * Number of Selected Pages * Copy Count
                double totalPrice = (pageSizeValue + colorValue) * numberOfSelectedPages * copyCount;
                return totalPrice;
            }
            else
            {
                // Return 0 if either the color status or page size is invalid
                return 0.0;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Calculate the total price
            double totalPrice = ComputeTotalPrice(PageSize, ColorStatus, SelectedPages.Count, CopyCount);

            // Create an instance of the insert_payment UserControl and pass the session ID along with totalPrice
            insert_payment insertPaymentControl = new insert_payment(filePath: FilePath,
                fileName: FileName,
                pageSize: PageSize,
                colorStatus: ColorStatus,
                numberOfSelectedPages: NumberOfSelectedPages,
                copyCount: CopyCount,
                selectedPages: SelectedPages,
                totalPrice: totalPrice); 

            // Access the MainWindow instance
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the insert_payment page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = insertPaymentControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
           

        }
    }
}
