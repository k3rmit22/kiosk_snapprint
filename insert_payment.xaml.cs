using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace kiosk_snapprint
{
    public partial class insert_payment : UserControl, IDisposable
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

        private SerialPort _serialPort;
        private int _insertedAmount; // Tracks the inserted amount

        public insert_payment(string filePath, string fileName, string pageSize, int pageCount,
                              string colorStatus, int numberOfSelectedPages, int copyCount,
                              List<int> selectedPages, double totalPrice)
        {
            InitializeComponent();

            // Store passed values
            FilePath = filePath;
            FileName = fileName;
            PageSize = pageSize;
            PageCount = pageCount;
            ColorStatus = colorStatus;
            NumberOfSelectedPages = numberOfSelectedPages;
            CopyCount = copyCount;
            SelectedPages = selectedPages;
            TotalPrice = totalPrice;

            Loadsummary(FileName, TotalPrice);
            InitializeSerialPort();
            ResetInsertedAmount(); // Initialize the reset logic
            this.Unloaded += UserControl_Unloaded;
        }

        private void ResetInsertedAmount()
        {
            try
            {
                _insertedAmount = 0; // Reset the C# application state
                inserted_amount_label.Text = $"{_insertedAmount:F2}";

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.WriteLine("RESET"); // Send reset command to Arduino
                    Debug.WriteLine("Reset command sent to Arduino.");
                }
                else
                {
                    Debug.WriteLine("Serial port is not open. Cannot send reset command.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during reset: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            HomeUserControl home = new HomeUserControl(
               
            );

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = home;
            }
            else
            {
                MessageBox.Show("MainWindow instance is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Loadsummary(string fileName, double totalPrice)
        {
            name_label.Text = fileName ?? "Unknown File";
            total_label.Text = $"{totalPrice:F2}";
        }

        private void InitializeSerialPort()
        {
            try
            {
                _serialPort = new SerialPort("COM8", 115200); // Adjust COM port as needed
                _serialPort.DataReceived += SerialPort_DataReceived;

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Debug.WriteLine("Serial port connected successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the serial port: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort.ReadLine().Trim();

                if (int.TryParse(data, out int amount))
                {
                    if (amount >= 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _insertedAmount = amount;
                            inserted_amount_label.Text = $"{_insertedAmount:F2}";
                            Debug.WriteLine($"Amount updated: {_insertedAmount}");

                           
                        });
                    }
                }
                else
                {
                    Debug.WriteLine($"Invalid data received: {data}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading serial port data: {ex.Message}");
            }
        }

       
       


        public void Dispose()
        {
            if (_serialPort != null)
            {
                try
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;

                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }

                    _serialPort.Dispose();
                    _serialPort = null;

                    Debug.WriteLine("Serial port disposed and event handler removed.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error disposing serial port: {ex.Message}");
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("UserControl_Unloaded triggered.");
            Dispose();
        }

        

    }
}
