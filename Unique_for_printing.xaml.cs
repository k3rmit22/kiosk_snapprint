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
using System.Windows.Shapes;

namespace kiosk_snapprint
{
    
    public partial class Unique_for_printing : Window
    {
        public byte[] FileBytes { get; }
        public string FileName { get; }
        public string PageSize { get; }
        public string ColorMode { get; }
        public List<int> SelectedPages { get; }
        public int CopyCount { get; }
        public int TotalPrice { get; }
        public Unique_for_printing(byte[] fileBytes, string fileName, string pageSize, string colorMode, List<int> selectedPages, int copyCount, int totalPrice)
        {
            InitializeComponent();
        }
    }


}
