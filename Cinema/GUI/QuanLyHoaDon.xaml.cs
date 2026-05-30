using BUS;
using DAL.Models;
using DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cinema.GUI
{
    public partial class QuanLyHoaDon : UserControl
    {
        private readonly HoaDonBUS hoaDonBUS = new HoaDonBUS();
        private readonly BanVeBUS banVeBUS = new BanVeBUS();

        private HoaDonRowInfo? selectedHoaDon;
        private HoaDonDetailInfo? selectedDetail;

        public QuanLyHoaDon()
        {
            InitializeComponent();
            LoadHoaDons();
            ResetChiTiet();
        }

        private void LoadHoaDons()
        {
            int? maRap = Session.IsAdmin ? null : Session.MaRap;
            var list = hoaDonBUS.GetHoaDons(
                txtMaHoaDon.Text,
                txtSoDienThoai.Text,
                dpTuNgay.SelectedDate,
                dpDenNgay.SelectedDate,
                maRap);

            dgHoaDon.ItemsSource = list;
            txtTongHoaDon.Text = $"{list.Count} hóa đơn";

            if (list.Any())
            {
                dgHoaDon.SelectedIndex = 0;
            }
            else
            {
                selectedHoaDon = null;
                selectedDetail = null;
                ResetChiTiet();
            }
        }

        private void dgHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedHoaDon = dgHoaDon.SelectedItem as HoaDonRowInfo;
            selectedDetail = null;

            if (selectedHoaDon == null)
            {
                ResetChiTiet();
                return;
            }

            var detail = hoaDonBUS.GetHoaDonDetail(selectedHoaDon.MaDonHang, Session.IsAdmin ? null : Session.MaRap);
            if (detail == null)
            {
                ResetChiTiet();
                return;
            }

            selectedDetail = detail;
            txtMoTaChiTiet.Text = $"Chi tiết hóa đơn {detail.MaHoaDon}";
            txtTenPhimChiTiet.Text = detail.TenPhim;
            txtSuatChieuChiTiet.Text = detail.SuatChieu;
            txtPhongChieuChiTiet.Text = detail.PhongChieu;
            txtGheChiTiet.Text = string.IsNullOrWhiteSpace(detail.Ghe) ? "--" : detail.Ghe;
            lstDichVuChiTiet.ItemsSource = detail.DichVus.Any() ? detail.DichVus : new List<string> { "Không có dịch vụ" };
            txtTongTienChiTiet.Text = detail.TongTien;
            txtTrangThaiChiTiet.Text = detail.TrangThai;
            CapNhatTrangThaiChiTiet(detail.TrangThai);
            btnHuyVe.Content = detail.TrangThai == "Đã hủy" ? "Đã hủy" : "Hủy hóa đơn";
            btnHuyVe.IsEnabled = detail.TrangThai != "Đã hủy";
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
            if (selectedHoaDon == null || selectedDetail == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn.", "Thông báo");
                return;
            }

            if (selectedDetail.TrangThai == "Đã hủy")
            {
                MessageBox.Show("Hóa đơn này đã được hủy trước đó.", "Thông báo");
                return;
            }

            var dialog = new HuyHoaDonWindow(selectedDetail)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            var thanhCong = hoaDonBUS.HuyHoaDon(
                selectedHoaDon.MaDonHang,
                dialog.LyDoHuy,
                Session.MaNhanVien,
                Session.IsAdmin ? null : Session.MaRap);
            MessageBox.Show(
                thanhCong ? "Hủy hóa đơn thành công." : "Không thể hủy hóa đơn.",
                "Thông báo");

            LoadHoaDons();
        }

        private void CapNhatTrangThaiChiTiet(string trangThai)
        {
            if (trangThai == "Đã hủy")
            {
                txtTrangThaiChiTiet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#991B1B"));
            }
            else
            {
                txtTrangThaiChiTiet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F7A5A"));
            }
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
            txtTrangThaiChiTiet.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F7A5A"));
            btnHuyVe.Content = "Hủy hóa đơn";
            btnHuyVe.IsEnabled = false;
        }
    }
}
