using System;
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

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for insert_payment.xaml
    /// </summary>
    public partial class insert_payment : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public string ColorStatus { get; private set; }
        public int NumberOfSelectedPages { get; private set; }
        public int CopyCount { get; private set; }
        public List<int> SelectedPages { get; private set; }
        public double TotalPrice { get; }


        public insert_payment(string filePath, string fileName, string pageSize, string colorStatus, int numberOfSelectedPages, int copyCount, List<int> selectedPages, double totalPrice)
        {
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            ColorStatus = colorStatus;
            NumberOfSelectedPages = numberOfSelectedPages;
            CopyCount = copyCount;
            SelectedPages = selectedPages;
            TotalPrice = totalPrice;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the previous UserControl (PricingQR in this case)
            PricingQR pricingQRControl = new PricingQR(
                filePath: FilePath,
                fileName: FileName,
                pageSize: PageSize,
                colorStatus: ColorStatus,
                numberOfSelectedPages: NumberOfSelectedPages,
                copyCount: CopyCount,
                selectedPages: SelectedPages);

            // Access MainWindow and navigate back
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                // Set the content to display the insert_payment page (assuming a ContentControl named MainContent)
                mainWindow.MainContent.Content = pricingQRControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.");
            }

        }
    }
}
