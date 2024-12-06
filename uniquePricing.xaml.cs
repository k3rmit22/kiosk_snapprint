using System;
using System.Windows;
using System.Windows.Controls;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for uniquePricing.xaml
    /// </summary>
    public partial class uniquePricing : UserControl
    {
        // Public properties to hold the received data
        public string FileName { get; set; }
        public string PageSize { get; set; }
        public string ColorMode { get; set; }
        public int SelectedPages { get; set; }
        public int CopyCount { get; set; }
        public byte[] FileBytes { get; set; }

        // Constructor to initialize the component and receive data
        public uniquePricing(string fileName, string pageSize, string colorMode, int selectedPages, int copyCount, byte[] fileBytes)
        {
            InitializeComponent();

            // Store the received data in the properties
            FileName = fileName;
            PageSize = pageSize;
            ColorMode = colorMode;
            SelectedPages = selectedPages;
            CopyCount = copyCount;
            FileBytes = fileBytes;

            // Optionally, display the data on the UI, such as in labels or text boxes
            DisplayData();
        }

        // Method to display the received data on the UI
        private void DisplayData()
        {
            // Assuming you have labels or text blocks to display the data
            FileNameLabel.Content = FileName;
            PageSizeLabel.Content = PageSize;
            ColorModeLabel.Content = ColorMode;
            SelectedPagesLabel.Content = SelectedPages.ToString();
            CopyCountLabel.Content = CopyCount.ToString();

            // You can handle the file bytes here (for example, showing a preview or other actions)
            // Example: Displaying the file size
            FileSizeLabel.Content = FileBytes?.Length > 0 ? $"{FileBytes.Length / 1024} KB" : "No file";
        }
    }
}
