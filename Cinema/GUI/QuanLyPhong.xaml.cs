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
    /// Interaction logic for QuanLyGhe.xaml
    /// </summary>
    public partial class QuanLyPhong : UserControl
    {
        private PhongChieuBUS bus = new PhongChieuBUS();
        private GheBUS gheBus = new GheBUS();
        private RapBUS rapBus=new RapBUS();
        private SuatChieuBUS suatChieuBus = new SuatChieuBUS();
        private readonly int? maRapDangNhap = Session.IsAdmin ? null : Session.MaRap;
        private sealed class SuatChieuItem
        {
            public int MaSuatChieu { get; set; }
            public string DisplayText { get; set; } = string.Empty;
        }
        
        public QuanLyPhong()
        {
            InitializeComponent();
            dgPhong.ItemsSource = bus.getPhongChieu(maRapDangNhap);
            loadComboRap();
        }
        public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemPhong themPhong = new ThemPhong();
            themPhong.ShowDialog();
            dgPhong.ItemsSource = bus.getPhongChieu(maRapDangNhap);
        }
        public void btnSua_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as FrameworkElement;
            var phong = border?.DataContext as PhongChieu;
            if (phong == null)
            {
                MessageBox.Show("Không tìm thấy phòng chiếu để sửa!");
                return;
            }
            var item = bus.getPhongChieuById(phong.MaPhong, maRapDangNhap);
            ThemPhong edit = new ThemPhong(item);
            if (edit.ShowDialog() == true)
            {
                dgPhong.ItemsSource = bus.getPhongChieu(maRapDangNhap);
            }
        }
        public void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var phong = btn?.Tag as PhongChieu;
            if (phong == null)
            {
                MessageBox.Show("Không tìm thấy phòng chiếu để xóa!");
                return;
            }
            var msg = new CustomMessageBox(
                "Xác nhận xóa",
                $"Bạn có chắc muốn xóa phòng chiếu '{phong.TenPhong}'?");
            msg.ShowDialog();
            if (msg.Result)
            {
                if (bus.removePhongChieu(phong.MaPhong))
                {
                    MessageBox.Show("Xóa phòng chiếu thành công!");
                    dgPhong.ItemsSource = bus.getPhongChieu(maRapDangNhap);
                }
                else
                {
                    MessageBox.Show("Xóa phòng chiếu thất bại!");
                }
            }
        }
        private void dgPhong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPhong.SelectedItem is PhongChieu phong)
            {
                txtTenPhong.Text = "🎬"+phong.TenPhong;
                //var phong2 = bus.getPhongChieuById(phong.MaPhong);
                txtTenRap.Text = "Rạp: "+phong.MaRapNavigation?.TenRap;
                loadSuatChieu(phong.MaPhong);
                loadGhe();
                loadThongTinGhe(phong.MaPhong);
            }
            
        }
        public void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maRap = (int)(cbRap.SelectedValue ?? 0);
            if (maRap == 0)
            {
                dgPhong.ItemsSource = bus.getPhongChieu(maRapDangNhap);
            }
            else
            {
                dgPhong.ItemsSource = bus.getPhongChieuByRap(maRap);
            }
        }
        private void cbSuatChieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPhong.SelectedItem is PhongChieu phong)
            {
                loadGhe();
                loadThongTinGhe(phong.MaPhong);
            }
        }
        private void loadSuatChieu(int maPhong)
        {
            var dsSuat = suatChieuBus.getSuatChieuByPhong(maPhong, maRapDangNhap)
                .Where(x => x.ThoiGianBatDau.HasValue && x.ThoiGianBatDau.Value.Date >= DateTime.Today)
                .Select(x => new SuatChieuItem
                {
                    MaSuatChieu = x.MaSuatChieu,
                    DisplayText = $"{x.ThoiGianBatDau:dd/MM/yyyy HH:mm}"
                })
                .ToList();

            cbSuatChieu.ItemsSource = dsSuat;
            cbSuatChieu.SelectedIndex = dsSuat.Count > 0 ? 0 : -1;
            cbSuatChieu.IsEnabled = dsSuat.Count > 0;
        }
        private void loadGhe()
        {
            var dsGhe = gheBus.getGheByPhongId((dgPhong.SelectedItem as PhongChieu)?.MaPhong ?? 0);
            var maSuatChieu = cbSuatChieu.SelectedValue as int?;
            var gheDaBan = maSuatChieu.HasValue
                ? gheBus.getMaGheDaBanTheoSuat(maSuatChieu.Value)
                : new List<int>();

            spSeat.Children.Clear();
            var dsHang=dsGhe.GroupBy(g => g.HangGhe).OrderBy(g => g.Key);
            foreach (var hang in dsHang)
            {
                StackPanel spHang = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
                TextBlock txtHang = new TextBlock { Text = $"Hàng {hang.Key}:", FontWeight = FontWeights.Bold,Foreground=Brushes.White, Margin = new Thickness(0, 0, 10, 0) };
                spHang.Children.Add(txtHang);
                foreach (var ghe in hang)
                {
                    Button btnGhe = new Button
                    {
                        Content = ghe.SoGhe,
                        Width = 40,
                        Height = 40,
                        Margin = new Thickness(5),
                        Background = Brushes.Transparent,
                        Foreground = Brushes.White,
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569")),
                        BorderThickness = new Thickness(1.5),
                        FontWeight = FontWeights.SemiBold
                    };

                    if (ghe.TrangThai == "Bảo trì")
                    {
                        btnGhe.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));
                        btnGhe.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569"));
                    }
                    else if (gheDaBan.Contains(ghe.MaGhe))
                    {
                        btnGhe.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8"));
                        btnGhe.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8"));
                        btnGhe.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"));
                    }

                    spHang.Children.Add(btnGhe);
                }
                spSeat.Children.Add(spHang);
            }

        }
        public void loadThongTinGhe(int ma)
        {
            var dsGhe = gheBus.getGheByPhongId(ma);
            var tongGhe = dsGhe.Count;
            var gheBaoTri = dsGhe.Count(x => x.TrangThai == "Bảo trì");
            var maSuatChieu = cbSuatChieu.SelectedValue as int?;
            var gheDaBan = maSuatChieu.HasValue
                ? gheBus.getMaGheDaBanTheoSuat(maSuatChieu.Value).Count
                : 0;
            var gheTrong = tongGhe - gheBaoTri - gheDaBan;

            txtGheBaoTri.Text = gheBaoTri.ToString();
            txtTongGhe.Text = tongGhe.ToString();
            txtGheHoatDong.Text = gheDaBan.ToString();
            txtGheTrong.Text = Math.Max(gheTrong, 0).ToString();
        }
        public void loadComboRap()
        {
            var dsRap = maRapDangNhap.HasValue && maRapDangNhap.Value > 0
                ? rapBus.getAllRap().Where(x => x.MaRap == maRapDangNhap.Value).ToList()
                : rapBus.getAllRap();

            if (!maRapDangNhap.HasValue)
            {
                dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "🏢 Tất cả rạp" });
            }

            cbRap.ItemsSource = dsRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            if (maRapDangNhap.HasValue && maRapDangNhap.Value > 0)
            {
                cbRap.SelectedValue = maRapDangNhap.Value;
                cbRap.IsEnabled = false;
            }
            else
            {
                cbRap.SelectedIndex = 0;
            }
        }

    }
}
