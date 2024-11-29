using System;
using System.Windows;
using System.Windows.Controls;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for PricingQR.xaml
    /// </summary>
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
            { "letter", 5 },
            { "letter", 10 }  
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
            Loadsummary(FileName, PageSize, ColorStatus, SelectedPages,CopyCount) ;

            // Debug log the data (optional for troubleshooting)
            System.Diagnostics.Debug.WriteLine($"FilePath: {FilePath}");
            System.Diagnostics.Debug.WriteLine($"FileName: {FileName}");
            System.Diagnostics.Debug.WriteLine($"PageSize: {PageSize}");
            System.Diagnostics.Debug.WriteLine($"ColorStatus: {ColorStatus}");
            System.Diagnostics.Debug.WriteLine($"NumberOfSelectedPages: {NumberOfSelectedPages}");
            System.Diagnostics.Debug.WriteLine($"CopyCount: {CopyCount}");
            System.Diagnostics.Debug.WriteLine($"SelectedPages: {string.Join(", ", SelectedPages)}");

        }

        private void Loadsummary (string fileName, string pageSize, string colorStatus, List<int> selectedPages, int copyCount) 
        {

            filename.Text = fileName;
            color_label.Text = colorStatus;
            pagesize_label.Text = pageSize;
            Copies_label.Text = copyCount.ToString();
            if (selected_pages_label != null && selectedPages != null)
            {
                // Display the selected pages as a comma-separated string
                selected_pages_label.Text = string.Join(", ", selectedPages);
            }

            // Compute the total price
            double totalPrice = ComputeTotalPrice(pageSize, colorStatus, selectedPages.Count, copyCount);

            // Display the total price
            total_label.Text = $"${totalPrice}";

        }

        private double ComputeTotalPrice(string pageSize, string colorStatus, int numberOfSelectedPages, int copyCount)
        {
            // Get the numeric values for color status and page size
            if (ColorStatusValues.TryGetValue(colorStatus.ToLower(), out int colorValue) &&
                PageSizeValues.TryGetValue(pageSize.ToLower(), out int pageSizeValue))
            {
                // Formula: Paper Size + (Color Status * Number of Selected Pages * Copy Count)
                double totalPrice = pageSizeValue + (colorValue * numberOfSelectedPages * copyCount);
                return totalPrice;
            }
            else
            {
                return 0.0; // If the color status or page size is invalid, return 0.
            }
        }

    }
}
