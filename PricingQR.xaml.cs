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

        }







       

        
    }
}
