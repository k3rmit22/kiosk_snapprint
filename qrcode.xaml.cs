using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;
using QRCoder;

namespace kiosk_snapprint
{
    /// <summary>
    /// Interaction logic for qrcode.xaml
    /// </summary>
    public partial class qrcode : UserControl
    {
        private string _sessionId;
        public qrcode(string sessionId)
        {
            InitializeComponent();
            _sessionId = sessionId;
            DisplayQRCode();

            SessionIdTextBlock.Text = $"Session ID: {_sessionId}";



        }

        private void DisplayQRCode()
        {
            // Generate the QR code URL with the session ID
            string url = $"http://192.168.1.1/HtmlFiles/upload/index?sessionId={_sessionId}";


            // Create the QR code image
            Bitmap qrCodeImage = GenerateQRCode(url);

            // Display the QR code in the Image control (assuming an Image control named QrCodeImageControl)
            QrCodeImageControl.Source = BitmapToImageSource(qrCodeImage);
        }

        private Bitmap GenerateQRCode(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(20); // Adjust the size as needed
        }

        // Helper function to convert Bitmap to ImageSource for WPF Image control
        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }









        }
    }
}
