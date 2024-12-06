﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace kiosk_snapprint
{
    public partial class PdfDisplayPage : UserControl
    {
        private MemoryStream _pdfStream;
        private string _fileName;
        private string _pageSize;
        private string _colorMode;
        private int _totalPages;
        private List<int> selectedPages = new List<int>();

        public PdfDisplayPage(byte[] fileBytes, string fileName = null)
        {
            InitializeComponent();
            DataContext = this;

            if (fileBytes == null || fileBytes.Length == 0)
            {
                MessageBox.Show("PDF data is empty or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _fileName = !string.IsNullOrEmpty(fileName) ? fileName : "Unknown PDF";
            Console.WriteLine($"Final FileName: {_fileName}");
            DisplayPdf(fileBytes, _fileName);
        }

        private void DisplayPdf(byte[] fileBytes, string fileName)
        {
            try
            {
                _pdfStream = new MemoryStream(fileBytes);

                if (_pdfStream.Length == 0)
                {
                    MessageBox.Show("Invalid PDF stream.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _fileName = fileName;
                PdfViewerControl.Load(_pdfStream);

                _totalPages = PdfViewerControl.PageCount;
                _colorMode = DetectColorMode(fileBytes);
                _pageSize = DetectPageSize(fileBytes);

                PopulatePageCheckboxes();  // Call the method to populate checkboxes
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string DetectColorMode(byte[] fileBytes)
        {
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    var page = reader.GetPageN(i);
                    var resources = page.GetAsDict(PdfName.RESOURCES);
                    var xObject = resources?.GetAsDict(PdfName.XOBJECT);

                    if (xObject != null)
                    {
                        foreach (var key in xObject.Keys)
                        {
                            var obj = xObject.GetAsStream(key);
                            if (obj != null && obj.Length > 0)
                            {
                                var colorSpace = obj.Get(PdfName.COLORSPACE);
                                if (colorSpace != null && colorSpace.ToString().Contains("DeviceRGB"))
                                {
                                    return "Colored";
                                }
                            }
                        }
                    }

                    var contentBytes = reader.GetPageContent(i);
                    string content = System.Text.Encoding.Default.GetString(contentBytes);
                    if (content.Contains("rg") || content.Contains("RG"))
                    {
                        return "Colored";
                    }
                }
            }

            return "Grayscale";
        }

        private string DetectPageSize(byte[] fileBytes)
        {
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                var page = reader.GetPageSizeWithRotation(1);
                float pageWidth = page.Width;
                float pageHeight = page.Height;
                float tolerance = 2.0f;

                if (Math.Abs(pageWidth - 595) < tolerance && Math.Abs(pageHeight - 842) < tolerance)
                    return "A4";
                else if (Math.Abs(pageWidth - 612) < tolerance && Math.Abs(pageHeight - 792) < tolerance)
                    return "Short";
                else if (Math.Abs(pageWidth - 612) < tolerance && Math.Abs(pageHeight - 1008) < tolerance)
                    return "Long";
                else if (Math.Abs(pageWidth - 2550) < tolerance && Math.Abs(pageHeight - 3300) < tolerance)
                    return "A3";

                return "Unknown";
            }
        }

        private void PopulatePageCheckboxes()
        {
            PageSelectionStackPanel.Children.Clear();

            // Create "Select All Pages" checkbox
            CheckBox selectAllCheckBox = new CheckBox
            {
                Content = "Select All Pages",
                FontSize = 15,
                Tag = "SelectAll"
            };

            selectAllCheckBox.Checked += SelectAllCheckBox_Checked;
            selectAllCheckBox.Unchecked += SelectAllCheckBox_Unchecked;
            PageSelectionStackPanel.Children.Add(selectAllCheckBox);

            // Create a checkbox for each page
            for (int i = 1; i <= _totalPages; i++)
            {
                CheckBox pageCheckBox = new CheckBox
                {
                    Content = $"Page {i}",
                    FontSize = 15,
                    Tag = i
                };

                pageCheckBox.Checked += PageCheckBox_Checked;
                pageCheckBox.Unchecked += PageCheckBox_Unchecked;
                PageSelectionStackPanel.Children.Add(pageCheckBox);
            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var child in PageSelectionStackPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.Tag is int)
                {
                    checkBox.IsChecked = true;
                }
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var child in PageSelectionStackPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.Tag is int)
                {
                    checkBox.IsChecked = false;
                }
            }
        }

        private void PageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is int pageNumber)
            {
                if (!selectedPages.Contains(pageNumber))
                {
                    selectedPages.Add(pageNumber); // Add page to selected pages list
                }
                UpdateSelectAllCheckboxState();
            }
        }

        private void PageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is int pageNumber)
            {
                selectedPages.Remove(pageNumber); // Remove page from selected pages list
            }
            UpdateSelectAllCheckboxState();
        }

        private void UpdateSelectAllCheckboxState()
        {
            bool allChecked = true;

            foreach (var child in PageSelectionStackPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.Tag is int)
                {
                    // If any page is unchecked, mark the 'Select All' checkbox as unchecked
                    if (checkBox.IsChecked != true)
                    {
                        allChecked = false;
                        break;
                    }
                }
            }

            var selectAllCheckBox = PageSelectionStackPanel.Children[0] as CheckBox;
            if (selectAllCheckBox != null)
            {
                selectAllCheckBox.Checked -= SelectAllCheckBox_Checked;
                selectAllCheckBox.Unchecked -= SelectAllCheckBox_Unchecked;
                selectAllCheckBox.IsChecked = allChecked;
                selectAllCheckBox.Checked += SelectAllCheckBox_Checked;
                selectAllCheckBox.Unchecked += SelectAllCheckBox_Unchecked;
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = new uniquecode();
        }

        private void PROCEED_Click(object sender, RoutedEventArgs e)
        {
            int selectedPagesCount = selectedPages.Count > 0 ? selectedPages.Count : 1;
            Console.WriteLine($"Selected Pages: {selectedPagesCount}");

            byte[] fileBytes = _pdfStream.ToArray();

            uniquePreferences preferencesPage = new uniquePreferences();
            preferencesPage.SetPreferences(_fileName, _pageSize, _colorMode, selectedPagesCount, fileBytes);

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = preferencesPage;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _pdfStream?.Dispose();
        }

        

        private void PrintAllPagesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PageSelectionStackPanel.IsEnabled = true;
        }

        private int _copyCount = 1;
        private const int _maxCopies = 10;
    }
}
