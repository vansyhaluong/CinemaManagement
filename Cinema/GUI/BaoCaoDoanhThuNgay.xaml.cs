using BUS;
using Cinema.Models;
using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using Microsoft.Win32;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.IO;
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
using System.Diagnostics;
using ClosedXML.Excel;



namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for BaoCaoDoanhThuNgay.xaml
    /// </summary>
    public partial class BaoCaoDoanhThuNgay : UserControl
    {
        private BaoCaoDoanhThuBUS baoCaoBUS = new BaoCaoDoanhThuBUS();
        private RapBUS rapBUS = new RapBUS();
        public BaoCaoDoanhThuNgay()
        {
            InitializeComponent();
            LoadComboRap();
            LoadBaoCao();
        }
        private void LoadComboRap()
        {
            var dsRap = rapBUS.getAllRap();

            dsRap.Insert(0, new Rap
            {
                MaRap = 0,
                TenRap = "Tất cả rạp"
            });

            cbRap.ItemsSource = dsRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.SelectedIndex = 0;
        }
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //    Report report = new Report();

        //    report.Load("Reports/BaoCaoDoanhThuNgay.frx");


        //}
        private void btnXuatExcel_Click(object sender, RoutedEventArgs e)
        {
            if (cbRap.SelectedValue == null ||
                Convert.ToInt32(cbRap.SelectedValue) == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 rạp!");
                return;
            }

            DateTime ngay = dpNgayBaoCao.SelectedDate ?? DateTime.Today;
            int maRap = Convert.ToInt32(cbRap.SelectedValue);
            int countNgay = baoCaoBUS.CountDonHangTheoNgay(ngay, maRap);
            int countThanhToan = baoCaoBUS.CountDonHangDaThanhToan(ngay, maRap);
  
            var data = baoCaoBUS.GetChiTietExcel(ngay, maRap);
 

            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất Excel!");
                return;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Excel file (*.xlsx)|*.xlsx";
            save.FileName = $"BaoCaoChiTiet_{cbRap.Text}_{ngay:ddMMyyyy}.xlsx";

            if (save.ShowDialog() != true)
                return;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("BaoCaoChiTiet");

                // TITLE
                ws.Cell("A1").Value = "BÁO CÁO DOANH THU CHI TIẾT THEO NGÀY";
                ws.Range("A1:L1").Merge();
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 16;
                ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("A3").Value = "Rạp:";
                ws.Cell("B3").Value = cbRap.Text;

                ws.Cell("A4").Value = "Ngày báo cáo:";
                ws.Cell("B4").Value = ngay.ToString("dd/MM/yyyy");

                // HEADER
                int headerRow = 6;

                string[] headers =
                {
            "Mã hóa đơn",
            "Ngày đặt",
            "Khách hàng",
            "SĐT",
            "Tên phim",
            "Suất chiếu",
            "Ghế",
            "SL vé",
            "Dịch vụ đã mua",
            "Tiền vé",
            "Tiền dịch vụ",
            "Tổng tiền"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(headerRow, i + 1).Value = headers[i];
                    ws.Cell(headerRow, i + 1).Style.Font.Bold = true;
                    ws.Cell(headerRow, i + 1).Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    ws.Cell(headerRow, i + 1).Style.Font.FontColor = XLColor.White;
                    ws.Cell(headerRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                // DATA
                int row = headerRow + 1;

                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.MaHoaDon;
                    ws.Cell(row, 2).Value = item.NgayDat?.ToString("dd/MM/yyyy HH:mm");
                    ws.Cell(row, 3).Value = item.KhachHang;
                    ws.Cell(row, 4).Value = item.SoDienThoai;
                    ws.Cell(row, 5).Value = item.TenPhim;
                    ws.Cell(row, 6).Value = item.SuatChieu;
                    ws.Cell(row, 7).Value = item.Ghe;
                    ws.Cell(row, 8).Value = item.SoLuongVe;
                    ws.Cell(row, 9).Value = item.DichVuDaMua;
                    ws.Cell(row, 10).Value = item.TienVe;
                    ws.Cell(row, 11).Value = item.TienDichVu;
                    ws.Cell(row, 12).Value = item.TongTien;

                    for (int col = 1; col <= 12; col++)
                    {
                        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                }

                // FORMAT MONEY
                ws.Column(10).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                ws.Column(11).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                ws.Column(12).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";

                // SUMMARY
                row += 1;

                ws.Cell(row, 10).Value = "Tổng cộng:";
                ws.Cell(row, 10).Style.Font.Bold = true;

                ws.Cell(row, 11).Value = data.Sum(x => x.TienDichVu);
                ws.Cell(row, 12).Value = data.Sum(x => x.TongTien);

                ws.Cell(row + 1, 10).Value = "Tổng tiền vé:";
                ws.Cell(row + 1, 11).Value = data.Sum(x => x.TienVe);

                ws.Cell(row + 2, 10).Value = "Tổng số vé:";
                ws.Cell(row + 2, 11).Value = data.Sum(x => x.SoLuongVe);

                ws.Cell(row + 3, 10).Value = "Tổng hóa đơn:";
                ws.Cell(row + 3, 11).Value = data.Count;

                ws.Columns().AdjustToContents();

                workbook.SaveAs(save.FileName);
            }

            MessageBox.Show("Xuất Excel thành công!");
        }

        private void btnXemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            LoadBaoCao();
        }

        private void LoadBaoCao()
        {
            DateTime ngay = dpNgayBaoCao.SelectedDate ?? DateTime.Today;

            int maRap = cbRap.SelectedValue == null? 0: Convert.ToInt32(cbRap.SelectedValue);

            var data = baoCaoBUS.GetBaoCaoTheoNgay(ngay, maRap);

            dgBaoCao.ItemsSource = data;

            cardTongDoanhThu.Value = data.Sum(x => x.TongDoanhThu).ToString("N0") + " VNĐ";
            cardTongHoaDon.Value = data.Sum(x => x.SoDonHang).ToString();
            cardVeBan.Value = data.Sum(x => x.SoVeBan).ToString();
            cardDichVu.Value = data.Sum(x => x.DoanhThuDichVu).ToString("N0") + " VNĐ";
            decimal tongDoanhThuVe = data.Sum(x => x.DoanhThuVe);
            decimal tongDichVu = data.Sum(x => x.DoanhThuDichVu);

            LoadPieChart(tongDoanhThuVe, tongDichVu);
        }
        private void LoadPieChart(decimal doanhThuVe, decimal doanhThuDichVu)
        {
            double ve = Convert.ToDouble(doanhThuVe);
            double dichVu = Convert.ToDouble(doanhThuDichVu);

            pieDoanhThu.Series = new ISeries[]
            {
        new PieSeries<double>
        {
            Name = "Vé",
            Values = new double[] { ve },
            Fill = new SolidColorPaint(SKColor.Parse("#0EA5E9")),
            DataLabelsSize = 13,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
        },

        new PieSeries<double>
        {
            Name = "Dịch vụ",
            Values = new double[] { dichVu },
            Fill = new SolidColorPaint(SKColor.Parse("#F59E0B")),
            DataLabelsSize = 13,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
        }
            };

            double tong = ve + dichVu;

            if (tong == 0)
            {
                txtTyLeDoanhThu.Text = "Chưa có dữ liệu doanh thu";
                return;
            }

            double tiLeVe = ve / tong * 100;
            double tiLeDichVu = dichVu / tong * 100;

            txtTyLeDoanhThu.Text = $"Vé {tiLeVe:0.#}% • Dịch vụ {tiLeDichVu:0.#}%";
        }
        private void btnXuatPdf_Click(object sender, RoutedEventArgs e)
        {

            if (cbRap.SelectedValue == null ||
                Convert.ToInt32(cbRap.SelectedValue) == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 rạp!");
                return;
            }

            DateTime ngay = dpNgayBaoCao.SelectedDate ?? DateTime.Today;
            int maRap = Convert.ToInt32(cbRap.SelectedValue);

            var data = baoCaoBUS.GetChiTietReport(ngay, maRap);


            if (data == null || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu báo cáo!");
                return;
            }

            Report report = new Report();

            string path = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Reports",
                "BaoCaoDoanhThuNgay.frx"
            );

            report.Load(path);

            report.RegisterData(data, "BaoCao");
            report.GetDataSource("BaoCao").Enabled = true;

            report.SetParameterValue("NgayBaoCao", ngay.ToString("dd/MM/yyyy"));
            report.SetParameterValue("TenRap", cbRap.Text);

            decimal tongVe = data.Sum(x => x.TienVe);
            decimal tongDichVu = data.Sum(x => x.TienDichVu);
            decimal tongDoanhThu = data.Sum(x => x.TongTien);

            report.SetParameterValue("TongTienVe", tongVe.ToString("N0") + " VNĐ");
            report.SetParameterValue("TongTienDichVu", tongDichVu.ToString("N0") + " VNĐ");
            report.SetParameterValue("TongDoanhThu", tongDoanhThu.ToString("N0") + " VNĐ");
            report.SetParameterValue("TongDonHang", data.Count.ToString());

            report.Prepare();

            string tempPdf = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "BaoCaoTam.pdf"
            );

            PDFSimpleExport pdf = new PDFSimpleExport();

            report.Export(pdf, tempPdf);
            ReportPreviewWindow preview = new ReportPreviewWindow(tempPdf);
            preview.ShowDialog();


        }
    }
}
