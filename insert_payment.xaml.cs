﻿using System;
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

        private SerialPort _serialPort; // For communication with payment hardware
        private SerialPort _secondSerialPort; // For communication with second hardware (e.g., servo)

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
            InitializeSerialPorts(); // Initialize both serial ports
            ResetInsertedAmount(); // Initialize the reset logic
            this.Unloaded += UserControl_Unloaded;
        }

        private void InitializeSerialPorts()
        {
            try
            {
                // Initialize payment serial port
                _serialPort = new SerialPort("COM8", 115200);
                _serialPort.DataReceived += SerialPort_DataReceived;

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Debug.WriteLine("Payment serial port connected successfully.");
                }

                // Initialize second hardware serial port (e.g., servo)
                _secondSerialPort = new SerialPort("COM9", 115200);

                if (!_secondSerialPort.IsOpen)
                {
                    _secondSerialPort.Open();
                    Debug.WriteLine("Second hardware serial port connected successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the serial ports: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                            CheckForPaymentCompletion();
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

        private bool paymentCompleted = false; 

        private void CheckForPaymentCompletion()
        {
            // Check if the inserted amount meets or exceeds the total price and payment hasn't been completed already
            if (_insertedAmount >= TotalPrice && !paymentCompleted)
            {
                // Mark payment as completed to prevent multiple triggers
                paymentCompleted = true;

                // Send command to servo to move to 180 degrees
                SendServoCommand("servo0");

                // Proceed to the next step (navigate to the printing window)
                NavigateToPrintingTryWindow();
            }
        }

        //private void BackButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Send command to servo to reset to 0 degrees when back button is pressed
        //    SendServoCommand("servo180");

        //    // Navigate back to the home user control
        //    HomeUserControl home = new HomeUserControl();

        //    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        //    if (mainWindow != null)
        //    {
        //        mainWindow.MainContent.Content = home;
        //    }
        //    else
        //    {
        //        MessageBox.Show("MainWindow instance is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and display the cancel transaction modal
            cancel_transaction_modal cancelModal = new cancel_transaction_modal
            {
                Owner = Application.Current.MainWindow // Set the main window as the owner
            };

            // Pass the second serial port (COM9) to the modal so it can send a command if necessary
            cancelModal.SecondSerialPort = _secondSerialPort;

            // Show the modal dialog (blocks further interaction until closed)
            cancelModal.ShowDialog();
        }

        private void SendServoCommand(string command)
        {
            try
            {
                if (_secondSerialPort != null && _secondSerialPort.IsOpen)
                {
                    _secondSerialPort.WriteLine(command); // Send the command to the second hardware (e.g., servo)
                    Debug.WriteLine($"Sent command to servo: {command}");
                }
                else
                {
                    Debug.WriteLine("Second serial port is not open. Cannot send command.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending servo command: {ex.Message}");
            }
        }

        private void Loadsummary(string fileName, double totalPrice)
        {
            name_label.Text = fileName ?? "Unknown File";
            total_label.Text = $"{totalPrice:F2}";
        }

        private async void NavigateToPrintingTryWindow()
        {
            try
            {
                // Introduce a 3-second delay
                await Task.Delay(3000);

                // Create a new instance of the printing_try window
                printing_try printingWindow = new printing_try(
                    filePath: FilePath,
                    fileName: FileName,
                    pageSize: PageSize,
                    pageCount: PageCount,
                    colorStatus: ColorStatus,
                    numberOfSelectedPages: NumberOfSelectedPages,
                    copyCount: CopyCount,
                    selectedPages: SelectedPages,
                    totalPrice: TotalPrice
                );

                // Set the owner of the modal to the current main window
                printingWindow.Owner = Application.Current.MainWindow;

                // Show the window as a modal
                printingWindow.ShowDialog();

                // After modal closes, you can perform additional logic here if needed
            }
            catch (Exception ex)
            {
                // Handle any errors that occur
                MessageBox.Show($"An error occurred while opening the printing window: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
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

                    Debug.WriteLine("Payment serial port disposed and event handler removed.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error disposing payment serial port: {ex.Message}");
                }
            }

            if (_secondSerialPort != null)
            {
                try
                {
                    if (_secondSerialPort.IsOpen)
                    {
                        _secondSerialPort.Close();
                    }

                    _secondSerialPort.Dispose();
                    _secondSerialPort = null;

                    Debug.WriteLine("Second serial port disposed.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error disposing second serial port: {ex.Message}");
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
