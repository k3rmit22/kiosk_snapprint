using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QRCoder;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace kiosk_snapprint
{
    public partial class qrcode : UserControl
    {
        private string sessionId;
        private HubConnection hubConnection;

        public qrcode(string sessionId)
        {
            InitializeComponent();
            this.sessionId = sessionId;

            DisplayQRCode();
            SessionIdTextBlock.Text = $"Session ID: {sessionId}";

            SetupSignalR();    // Initialize SignalR connection
        }
        private void DisplayQRCode()
        {
            string url = $"http://192.168.137.1:5082/Upload/Index?sessionId={sessionId}";
            Bitmap qrCodeImage = GenerateQRCode(url);
            QrCodeImageControl.Source = BitmapToImageSource(qrCodeImage);
        }

        private async void SetupSignalR()
        {
            // Set up connection to the SignalR hub
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://192.168.137.1:5082/Hubs/fileUploadHub") // Ensure this matches your actual hub URL
                .Build();

            // Register a method to receive messages from the server
            hubConnection.On<string>("ReceiveMessage", async (message) =>
            {
                // Check if the message indicates a successful file upload for this session
                if (message.Contains($"File uploaded successfully for session {sessionId}"))
                {
                    // Fetch file details after receiving the SignalR notification
                    await FetchFileDetails();
                }
            });

            // Start the SignalR connection
            try
            {
                await hubConnection.StartAsync();
                Console.WriteLine("Connected to SignalR hub.");
            }
            catch (Exception ex)
            {
                ShowError($"Error connecting to SignalR hub: {ex.Message}");
            }
        }

        private async Task FetchFileDetails()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"http://192.168.137.1:5082/api/upload/getfileinfo?sessionId={sessionId}";

                    // Send request to retrieve file details
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize file details
                        var fileDetails = await response.Content.ReadFromJsonAsync<FileDetails>();

                        // Verify the session ID to ensure the correct file
                        if (fileDetails != null && fileDetails.SessionId == sessionId)
                        {
                            // Navigate to the PDF display with file details
                            NavigateToPDFDisplay(fileDetails);
                        }
                        else
                        {
                            ShowError("Session ID mismatch.");
                        }
                    }
                    else
                    {
                        ShowError("Unable to retrieve file details.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error fetching file details: " + ex.Message);
            }
        }




        private Bitmap GenerateQRCode(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(20); // Size customization
        }

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

        private void NavigateToPDFDisplay(FileDetails fileDetails)
        {
            var pdfDisplay = new PDFDisplay(fileDetails.FilePath, fileDetails.FileName, fileDetails.PageSize, fileDetails.PageCount);

            // Retrieve the current main window instance and set the content
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = pdfDisplay; // Update MainContent in the MainWindow instance
            }
            else
            {
                ShowError("Main window instance not found.");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up the SignalR connection when the control is unloaded
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }

    public class FileDetails
    {
        public string SessionId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
