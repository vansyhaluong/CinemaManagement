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

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for PreviewPhieuNhap.xaml
    /// </summary>
    public partial class PreviewPhieuNhap : Window
    {
        private FlowDocument _document;
        public PreviewPhieuNhap(FlowDocument document)
        {
            InitializeComponent();
            _document = document;

            viewer.Document = _document;
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)_document).DocumentPaginator,
                    "Phiếu nhập kho"
                );
            }
        }
    }
}
