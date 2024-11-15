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
using PdfiumViewer;
using Syncfusion.Windows.PdfViewer;





namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for PDFDisplay.xaml
    /// </summary>
    public partial class PDFDisplay : UserControl
    {
       
        public PDFDisplay(string filePath, string fileName, string pageSize, int pageCount)
        {
            InitializeComponent();
            LoadPDF(filePath);
        }
        private void LoadPDF(string filePath)
        {
            pdfViewer.Load(filePath);
        }

    }
}
