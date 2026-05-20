using DAL.Models;
using System.Windows;
using System.Windows.Controls;

namespace Cinema.GUI
{
    public partial class TicketPreviewWindow : Window
    {
        public TicketPreviewWindow(IEnumerable<TicketInfo> tickets)
        {
            InitializeComponent();
            TicketItems.ItemsSource = tickets;
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            ExportPdfDirect();
        }

        public void ExportPdfDirect()
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            TicketPrintArea.Measure(new Size(printDialog.PrintableAreaWidth, double.PositiveInfinity));
            TicketPrintArea.Arrange(new Rect(new Point(0, 0), TicketPrintArea.DesiredSize));
            TicketPrintArea.UpdateLayout();

            printDialog.PrintVisual(TicketPrintArea, "Cinema ticket");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
