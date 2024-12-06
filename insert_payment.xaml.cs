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
using System.IO;
using System.Collections.ObjectModel;
using System.IO.Ports;



namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for insert_payment.xamla
    /// </summary>
    public partial class insert_payment : UserControl
    {
        
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string PageSize { get; private set; }
        public int PageCount { get; private set; }
        public string ColorStatus { get; private set; }
        public int NumberOfSelectedPages { get; private set; }
        public int CopyCount { get; private set; }
        public List<int> SelectedPages { get; private set; }
        public double TotalPrice { get; private set; }


        public insert_payment(string filePath, string fileName, string pageSize, int pageCount,
                              string colorStatus, int numberOfSelectedPages, int copyCount,
                              List<int> selectedPages, double totalPrice)
        {
            InitializeComponent();

            
            // Store the passed values
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
            ColorStatus = colorStatus;
            NumberOfSelectedPages = numberOfSelectedPages;
            CopyCount = copyCount;
            SelectedPages = selectedPages;
            TotalPrice = totalPrice;


            // For debugging purposes, you can print out the values (optional)
            System.Diagnostics.Debug.WriteLine($"FilePath: {FilePath}");
            System.Diagnostics.Debug.WriteLine($"FileName: {FileName}");
            System.Diagnostics.Debug.WriteLine($"PageSize: {PageSize}");
            System.Diagnostics.Debug.WriteLine($"PageCount: {PageCount}");
            System.Diagnostics.Debug.WriteLine($"ColorStatus: {ColorStatus}");
            System.Diagnostics.Debug.WriteLine($"NumberOfSelectedPages: {NumberOfSelectedPages}");
            System.Diagnostics.Debug.WriteLine($"CopyCount: {CopyCount}");
            System.Diagnostics.Debug.WriteLine($"SelectedPages: {string.Join(", ", SelectedPages)}");
            System.Diagnostics.Debug.WriteLine($"TotalPrice: {TotalPrice}");

            Loadsummary(FileName, TotalPrice);

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of PricingQR, passing the necessary properties
            PricingQR pricingQRControl = new PricingQR(
                filePath: FilePath,
                fileName: FileName,
                pageSize: PageSize,
                colorStatus: ColorStatus,
                numberOfSelectedPages: NumberOfSelectedPages,
                copyCount: CopyCount,
                selectedPages: SelectedPages,
                pageCount: PageCount // Ensure this property is correctly passed
            );

            // Access the MainWindow instance and set the content to PricingQR
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = pricingQRControl;
            }
            else
            {
                // Handle error if MainWindow is null
                MessageBox.Show("MainWindow instance is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void Loadsummary(string fileName, double totalPrice)
        {
            // Ensure name_label and total_label are valid UI elements
            if (name_label != null)
                name_label.Text = fileName;

            if (total_label != null)
                total_label.Text = $"{totalPrice:F2}"; // Display with 2 decimal places
        }
    }
}
