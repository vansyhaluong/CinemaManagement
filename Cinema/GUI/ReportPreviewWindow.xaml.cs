using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;

namespace Cinema.GUI
{
    public partial class ReportPreviewWindow : Window
    {
        private readonly string _pdfPath;

        public ReportPreviewWindow(string pdfPath)
        {
            InitializeComponent();
            _pdfPath = pdfPath;
            Loaded += ReportPreviewWindow_Loaded;
        }

        private async void ReportPreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_pdfPath) || !File.Exists(_pdfPath))
            {
                MessageBox.Show("Không tìm thấy file PDF để xem trước.");
                Close();
                return;
            }

            await pdfViewer.EnsureCoreWebView2Async();
            pdfViewer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            pdfViewer.CoreWebView2.Settings.AreDevToolsEnabled = false;
            pdfViewer.CoreWebView2.Navigate(new Uri(_pdfPath).AbsoluteUri);
        }
    }
}
