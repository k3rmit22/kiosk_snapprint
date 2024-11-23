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
        public string FileName { get; }
        public string FilePath { get; }
        public string PageSize { get; }
        public int CopyCount { get; }
        public List<int> SelectedPages { get; private set; }
        public PricingQR(string fileName, string filePath, string pageSize, int copyCount, List<int> selectedPages)
        {
            InitializeComponent();
            FileName = fileName;
            FilePath = filePath;
            PageSize = pageSize;
            CopyCount = copyCount;
            SelectedPages = selectedPages;

            DisplayData();
        }

       

        private void DisplayData()
        {
            string selectedPagesText = string.Join(", ", SelectedPages); // Convert list of selected pages to a comma-separated string
            PageCountTextBlock.Text = $"Selected Page(s): {SelectedPages}";
            FileNameTextBlock.Text = $"File Name: {FileName}";
            CopyCountTextBlock.Text = $"Copy Count: {CopyCount}";
            
        }
    }
}
