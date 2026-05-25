using BUS;
using Cinema.Models;
using DTO;
using FastReport;
using FastReport.Export.PdfSimple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;


namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for NhapKhoWindow.xaml
    /// </summary>
    public partial class NhapKhoWindow : Window
    {
        private SanPhamBUS spBUS = new SanPhamBUS();

        private List<SanPhamKhoDTO> dsSanPham = new();
        private List<ChiTietNhapKhoDTO> dsChiTiet = new();

        public NhapKhoWindow()
        {
            InitializeComponent();
            
            LoadSanPham();
            CapNhatTongKet();
        }
        

        private void LoadSanPham()
        {
            dsSanPham = spBUS.GetDanhSachKho();

            cbSanPham.ItemsSource = dsSanPham;

            if (cbSanPham.Items.Count > 0)
                cbSanPham.SelectedIndex = 0;
        }

        private void BtnThemDong_Click(object sender, RoutedEventArgs e)
        {
            if (cbSanPham.SelectedItem is not SanPhamKhoDTO sp)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng nhập không hợp lệ!");
                return;
            }

            var dongCu = dsChiTiet.FirstOrDefault(x => x.MaSanPham == sp.MaSanPham);

            if (dongCu != null)
            {
                dongCu.SoLuong += soLuong;
            }
            else
            {
                dsChiTiet.Add(new ChiTietNhapKhoDTO
                {
                    MaSanPham = sp.MaSanPham,
                    TenSanPham = sp.Ten,
                    TenLoai = sp.TenLoai,
                    SoLuong = soLuong,
                    Gia = sp.Gia
                });
            }

            icChiTietNhap.ItemsSource = null;
            icChiTietNhap.ItemsSource = dsChiTiet;

            txtSoLuong.Clear();
            txtSoLuong.Focus();

            CapNhatTongKet();
        }

        private void BtnXoaDong_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.DataContext is ChiTietNhapKhoDTO ct)
            {
                dsChiTiet.Remove(ct);

                icChiTietNhap.ItemsSource = null;
                icChiTietNhap.ItemsSource = dsChiTiet;

                CapNhatTongKet();
            }
        }

        private void CapNhatTongKet()
        {
            txtTongMatHang.Text = dsChiTiet.Count.ToString();
            txtTongSoLuong.Text = dsChiTiet.Sum(x => x.SoLuong).ToString();
            txtTongTien.Text = dsChiTiet.Sum(x => x.ThanhTien).ToString("N0") + " đ";
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnLuuPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (dsChiTiet.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm vào phiếu nhập!");
                return;
            }

            int maNhanVien = TaiKhoanDTO.MaNhanVien;

            if (maNhanVien <= 0)
            {
                MessageBox.Show("Không xác định được nhân viên đang đăng nhập!");
                return;
            }

            int kq = spBUS.TaoPhieuNhapKho(maNhanVien, dsChiTiet);

            if (kq>0)
            {
                MessageBox.Show("Lưu phiếu nhập thành công!");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Lưu phiếu nhập thất bại!");
            }
        }
        private void BtnInPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (dsChiTiet.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào trong phiếu nhập!");
                return;
            }

            FlowDocument doc = TaoPhieuNhapDocument();

            PreviewPhieuNhap preview = new PreviewPhieuNhap(doc);
            preview.ShowDialog();
        }
        private FlowDocument TaoPhieuNhapDocument()
        {
            FlowDocument doc = new FlowDocument
            {
                PageWidth = 793,
                PageHeight = 1122,
                ColumnWidth = 700,
                PagePadding = new Thickness(50),
                FontFamily = new FontFamily("Arial"),
                FontSize = 13
            };

            doc.Blocks.Add(new Paragraph(new Run("PHIẾU NHẬP KHO"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            doc.Blocks.Add(new Paragraph(new Run($"Ngày nhập: {DateTime.Now:dd/MM/yyyy HH:mm}")));
            doc.Blocks.Add(new Paragraph(
    new Run($"Nhân viên nhập: {TaiKhoanDTO.HoTen}")
));

            Table table = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0, 20, 0, 10)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(230) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(120) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });

            TableRowGroup group = new TableRowGroup();

            TableRow header = new TableRow();
            header.Cells.Add(TaoCell("Sản phẩm", true));
            header.Cells.Add(TaoCell("SL", true));
            header.Cells.Add(TaoCell("Đơn giá", true));
            header.Cells.Add(TaoCell("Thành tiền", true));
            group.Rows.Add(header);

            foreach (var item in dsChiTiet)
            {
                TableRow row = new TableRow();
                row.Cells.Add(TaoCell(item.TenSanPham));
                row.Cells.Add(TaoCell(item.SoLuong.ToString()));
                row.Cells.Add(TaoCell(item.Gia.ToString("N0") + " đ"));
                row.Cells.Add(TaoCell(item.ThanhTien.ToString("N0") + " đ"));
                group.Rows.Add(row);
            }

            table.RowGroups.Add(group);
            doc.Blocks.Add(table);

            doc.Blocks.Add(new Paragraph(new Run($"Tổng số lượng: {dsChiTiet.Sum(x => x.SoLuong)}"))
            {
                TextAlignment = TextAlignment.Right,
                FontWeight = FontWeights.Bold
            });

            doc.Blocks.Add(new Paragraph(new Run($"Tổng tiền: {dsChiTiet.Sum(x => x.ThanhTien):N0} đ"))
            {
                TextAlignment = TextAlignment.Right,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });

            doc.Blocks.Add(new Paragraph(new Run("\nNgười lập phiếu\n\n\n(Ký, ghi rõ họ tên)"))
            {
                TextAlignment = TextAlignment.Right
            });

            return doc;
        }
        private TableCell TaoCell(string text, bool isHeader = false)
        {
            return new TableCell(new Paragraph(new Run(text)))
            {
                Padding = new Thickness(6),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5),
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal
            };
        }
    }
}
