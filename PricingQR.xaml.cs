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
       

        public PricingQR( int copyCount, int selectedPages, int totalPages, string filePath, string fileName)
        {
            InitializeComponent();
        }

        private void DisplayDetails(string selectedColor, int copyCount, int pagesToPrint, string filePath, string fileName, int totalPrice)
        {
            // Ensure you have corresponding TextBlocks in your XAML to display these values
            FileNameTextBlock.Text = $"File Name: {fileName}";
           
            ColorModeTextBlock.Text = $"Color Mode: {selectedColor}";
            PageCountTextBlock.Text = $"Selected Page(s): {pagesToPrint}";
            CopyCountTextBlock.Text = $"Copy Count: {copyCount}";
            PriceTextBlock.Text = $"Total Price: {totalPrice} coins";
        }
    }
}
