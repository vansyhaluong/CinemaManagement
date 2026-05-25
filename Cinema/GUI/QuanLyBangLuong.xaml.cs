using BUS;
using Cinema.Models;
using DTO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLyBangLuong.xaml
    /// </summary>
    public partial class QuanLyBangLuong : UserControl
    {
        private readonly BangLuongBUS bangLuongBUS = new BangLuongBUS();
        private readonly RapBUS rapBUS = new RapBUS();

        private List<BangLuongDTO> dsLuongGoc = new();
        private List<BangLuongDTO> dsLuongHienThi = new();
        public QuanLyBangLuong()
        {
            InitializeComponent();
            Loaded += QuanLyBangLuong_Loaded;
            btnTinhLuong.Click += btnTinhLuong_Click;

            txtTimKiem.TextChanged += Filter_Changed;
            cbRap.SelectionChanged += Filter_Changed;
            cbTinhTrang.SelectionChanged += Filter_Changed;
        }
        private void QuanLyBangLuong_Loaded(object sender, RoutedEventArgs e)
        {
            dpTuNgay.SelectedDate = DateTime.Today.AddDays(-6);
            dpDenNgay.SelectedDate = DateTime.Today;

            LoadComboRap();

            cbTinhTrang.SelectedIndex = 0;

            LoadBangLuong();
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

        private void btnTinhLuong_Click(object sender, RoutedEventArgs e)
        {
            LoadBangLuong();
        }

        private void LoadBangLuong()
        {
            try
            {
                if (dpTuNgay.SelectedDate == null || dpDenNgay.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ từ ngày và đến ngày!");
                    return;
                }

                DateOnly tuNgay = DateOnly.FromDateTime(dpTuNgay.SelectedDate.Value);
                DateOnly denNgay = DateOnly.FromDateTime(dpDenNgay.SelectedDate.Value);

                if (tuNgay > denNgay)
                {
                    MessageBox.Show("Từ ngày không được lớn hơn đến ngày!");
                    return;
                }

                // GUI chỉ gọi BUS, không đụng DB
                dsLuongGoc = bangLuongBUS.TinhLuongTheoTuan(tuNgay, denNgay);

                LocDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng lương: " + ex.Message);
            }
        }

        private void LocDuLieu()
        {
            var ds = dsLuongGoc.AsEnumerable();

            string keyword = txtTimKiem.Text.Trim();

            if (!string.IsNullOrWhiteSpace(keyword) && keyword != "Tìm nhân viên...")
            {
                ds = ds.Where(x =>
                    !string.IsNullOrWhiteSpace(x.HoTen) &&
                    x.HoTen.ToLower().Contains(keyword.ToLower()));
            }

            if (cbRap.SelectedValue != null)
            {
                int maRap = Convert.ToInt32(cbRap.SelectedValue);

                if (maRap != 0)
                {
                    ds = ds.Where(x => x.MaRap == maRap);
                }
            }

            string tinhTrang = (cbTinhTrang.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (tinhTrang == "Có thưởng")
                ds = ds.Where(x => x.Thuong > 0);
            else if (tinhTrang == "Có kỷ luật")
                ds = ds.Where(x => x.Phat > 0);
            else if (tinhTrang == "Đi trễ")
                ds = ds.Where(x => x.SoLanTre > 0);
            else if (tinhTrang == "Nghỉ nhiều")
                ds = ds.Where(x => x.SoNgayNghi >= 2);

            dsLuongHienThi = ds.ToList();

            dgBangLuong.ItemsSource = null;
            dgBangLuong.ItemsSource = dsLuongHienThi;

            LoadThongKe();
            lblPageInfo.Text = $"Hiển thị {dsLuongHienThi.Count} / {dsLuongGoc.Count} nhân viên";
        }

        private void LoadThongKe()
        {
            cardNhanVien.Value = dsLuongHienThi.Count.ToString();
            cardGioLam.Value = dsLuongHienThi.Sum(x => x.TongGioLam).ToString("0.##");
            cardThuong.Value = dsLuongHienThi.Sum(x => x.Thuong).ToString("N0") + " đ";
            cardPhat.Value = dsLuongHienThi.Sum(x => x.Phat).ToString("N0") + " đ";
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            if (IsLoaded)
                LocDuLieu();
        }

        private void btnChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not BangLuongDTO item)
                return;

            MessageBox.Show(
                $"Nhân viên: {item.HoTen}\n" +
                $"Rạp: {item.TenRap}\n" +
                $"Ngày công: {item.TongNgayCong}\n" +
                $"Giờ làm: {item.TongGioLam:0.##}\n" +
                $"Trễ: {item.SoLanTre}\n" +
                $"Nghỉ: {item.SoNgayNghi}\n" +
                $"Thưởng: {item.Thuong:N0} đ\n" +
                $"Phạt: {item.Phat:N0} đ\n" +
                $"Tổng lương: {item.TongLuong:N0} đ",
                "Chi tiết bảng lương");
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not BangLuongDTO item)
                return;

            MessageBox.Show($"In phiếu lương cho nhân viên: {item.HoTen}");
        }
        public void btnInBangLuong_Click(object sender, RoutedEventArgs e)
        {
            var item = dgBangLuong.SelectedItem as BangLuongDTO;

            if (item == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên trong danh sách.");
                return;
            }

            PhieuLuong window = new PhieuLuong(item);
            window.ShowDialog();
        }
    }
}
