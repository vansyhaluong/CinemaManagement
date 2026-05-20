using BUS;
using DAL.Models;
using System.Windows;
using System.Windows.Controls;

namespace Cinema.GUI
{
    public partial class QuanLyHoaDon : UserControl
    {
        private readonly HoaDonBUS hoaDonBUS = new HoaDonBUS();
        private readonly BanVeBUS banVeBUS = new BanVeBUS();

        private HoaDonRowInfo? selectedHoaDon;

        public QuanLyHoaDon()
        {
            InitializeComponent();
            LoadHoaDons();
            ResetChiTiet();
        }

        private void LoadHoaDons()
        {
            var list = hoaDonBUS.GetHoaDons(
                txtMaHoaDon.Text,
                txtSoDienThoai.Text,
                dpTuNgay.SelectedDate,
                dpDenNgay.SelectedDate);

            dgHoaDon.ItemsSource = list;
            txtTongHoaDon.Text = $"{list.Count} hóa đơn";

            if (list.Any())
            {
                dgHoaDon.SelectedIndex = 0;
            }
            else
            {
                selectedHoaDon = null;
                ResetChiTiet();
            }
        }

        private void dgHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedHoaDon = dgHoaDon.SelectedItem as HoaDonRowInfo;
            if (selectedHoaDon == null)
            {
                ResetChiTiet();
                return;
            }

            var detail = hoaDonBUS.GetHoaDonDetail(selectedHoaDon.MaDonHang);
            if (detail == null)
            {
                ResetChiTiet();
                return;
            }

            txtMoTaChiTiet.Text = $"Chi tiết hóa đơn {detail.MaHoaDon}";
            txtTenPhimChiTiet.Text = detail.TenPhim;
            txtSuatChieuChiTiet.Text = detail.SuatChieu;
            txtPhongChieuChiTiet.Text = detail.PhongChieu;
            txtGheChiTiet.Text = string.IsNullOrWhiteSpace(detail.Ghe) ? "--" : detail.Ghe;
            lstDichVuChiTiet.ItemsSource = detail.DichVus.Any() ? detail.DichVus : new List<string> { "Không có dịch vụ" };
            txtTongTienChiTiet.Text = detail.TongTien;
            txtTrangThaiChiTiet.Text = detail.TrangThai;
        }

        private void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            LoadHoaDons();
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            txtMaHoaDon.Clear();
            txtSoDienThoai.Clear();
            dpTuNgay.SelectedDate = null;
            dpDenNgay.SelectedDate = null;
            LoadHoaDons();
        }

        private void btnXemVe_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHoaDon == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn.", "Thông báo");
                return;
            }

            var tickets = banVeBUS.GetVeTheoDonHang(selectedHoaDon.MaDonHang);
            if (!tickets.Any())
            {
                MessageBox.Show("Không tìm thấy vé của hóa đơn này.", "Thông báo");
                return;
            }

            var preview = new TicketPreviewWindow(tickets)
            {
                Owner = Window.GetWindow(this)
            };
            preview.ShowDialog();
        }

        private void btnXuatPdfLai_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHoaDon == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn.", "Thông báo");
                return;
            }

            var tickets = banVeBUS.GetVeTheoDonHang(selectedHoaDon.MaDonHang);
            if (!tickets.Any())
            {
                MessageBox.Show("Không tìm thấy vé để xuất lại PDF.", "Thông báo");
                return;
            }

            var preview = new TicketPreviewWindow(tickets)
            {
                Owner = Window.GetWindow(this)
            };
            preview.ExportPdfDirect();
        }

        private void btnHuyVe_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHoaDon == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn.", "Thông báo");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn hủy hóa đơn {selectedHoaDon.MaHoaDon}?",
                "Xác nhận hủy",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var thanhCong = hoaDonBUS.HuyHoaDon(selectedHoaDon.MaDonHang);
            MessageBox.Show(
                thanhCong ? "Hủy hóa đơn thành công." : "Không thể hủy hóa đơn.",
                "Thông báo");

            LoadHoaDons();
        }

        private void ResetChiTiet()
        {
            txtMoTaChiTiet.Text = "Chọn một hóa đơn để xem thông tin chi tiết.";
            txtTenPhimChiTiet.Text = "Chưa có dữ liệu";
            txtSuatChieuChiTiet.Text = "--/--/---- --:--";
            txtPhongChieuChiTiet.Text = "--";
            txtGheChiTiet.Text = "--";
            lstDichVuChiTiet.ItemsSource = new List<string> { "Không có dữ liệu" };
            txtTongTienChiTiet.Text = "0 đ";
            txtTrangThaiChiTiet.Text = "Chưa chọn";
        }
    }
}
