using BUS;
using Cinema.Models;
using DAL.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cinema.GUI
{
    public partial class BanVe : UserControl
    {
        private readonly Phim phim;
        private readonly MovieBUS movieBUS = new MovieBUS();
        private readonly SuatChieuBUS suatChieuBUS = new SuatChieuBUS();
        private readonly DichVuBUS dichVuBUS = new DichVuBUS();
        private readonly BanVeBUS banVeBUS = new BanVeBUS();
        private readonly DispatcherTimer holdTimer = new DispatcherTimer();
        private readonly List<DichVuTam> dichVus = new List<DichVuTam>();
        private readonly Dictionary<int, GheBanVeInfo> gheDangChon = new Dictionary<int, GheBanVeInfo>();

        private SuatChieu? suatChieuDangChon;

        public BanVe(Phim p)
        {
            InitializeComponent();

            phim = p;
            holdTimer.Interval = TimeSpan.FromSeconds(5);
            holdTimer.Tick += (_, _) => RefreshGheDangChon();
            holdTimer.Start();

            LoadPhim();
            LoadSuatChieu();
            LoadDichVu();
            TinhTien();
        }

        private void LoadPhim()
        {
            txtTenPhim.Text = phim.TieuDe;
            txtThongTin.Text = $"{movieBUS.getTheLoaiByPhim(phim.MaPhim)} • {phim.ThoiLuong} phút";
        }

        private void LoadSuatChieu()
        {
            panelNgaySuatChieu.Children.Clear();

            var dsSuat = suatChieuBUS.getSuatChieuByPhim(phim.MaPhim)
                .Where(x => x.ThoiGianBatDau != null)
                .OrderBy(x => x.ThoiGianBatDau)
                .ToList();

            if (!dsSuat.Any())
            {
                panelNgaySuatChieu.Children.Add(new TextBlock
                {
                    Text = "Chưa có suất chiếu",
                    Foreground = new SolidColorBrush(Color.FromRgb(167, 176, 192))
                });
                return;
            }

            foreach (var ngay in dsSuat.GroupBy(x => x.ThoiGianBatDau!.Value.Date))
            {
                panelNgaySuatChieu.Children.Add(new TextBlock
                {
                    Text = ngay.Key.ToString("dd/MM/yyyy"),
                    Foreground = new SolidColorBrush(Color.FromRgb(69, 196, 134)),
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 10, 0, 8)
                });

                var wrap = new WrapPanel();
                foreach (var sc in ngay)
                {
                    var btn = new Button
                    {
                        Content = sc.ThoiGianBatDau!.Value.ToString("HH:mm"),
                        Style = (Style)FindResource("TimeButton"),
                        Tag = sc
                    };
                    btn.Click += btnSuatChieu_Click;
                    wrap.Children.Add(btn);
                }

                panelNgaySuatChieu.Children.Add(wrap);
            }
        }

        private void btnSuatChieu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not SuatChieu sc)
                return;

            suatChieuDangChon = sc;
            gheDangChon.Clear();
            LoadGhe(sc.MaSuatChieu);
            TinhTien();
        }

        private void LoadGhe(int maSuatChieu)
        {
            spGhe.Children.Clear();

            var list = banVeBUS.GetGheTheoSuatChieu(maSuatChieu);
            var maGheHopLe = list
                .Where(g => g.TrangThai == "DangGiu")
                .Select(g => g.MaGhe)
                .ToHashSet();

            foreach (var maGhe in gheDangChon.Keys.ToList())
            {
                if (!maGheHopLe.Contains(maGhe))
                    gheDangChon.Remove(maGhe);
            }

            txtSoGhe.Text = list.Any()
                ? $"{list.Count} ghế"
                : "Phòng này chưa có ghế";

            foreach (var hang in list.GroupBy(g => g.HangGhe).OrderBy(g => g.Key))
            {
                var row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 6, 0, 6),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                row.Children.Add(new TextBlock
                {
                    Text = hang.Key,
                    Width = 34,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                });

                foreach (var ghe in hang.OrderBy(g => g.SoGhe))
                {
                    row.Children.Add(TaoNutGhe(ghe));
                }

                spGhe.Children.Add(row);
            }
        }

        private Button TaoNutGhe(GheBanVeInfo ghe)
        {
            var dangChon = gheDangChon.ContainsKey(ghe.MaGhe);
            var btn = new Button
            {
                Content = dangChon ? $"{ghe.SoGhe}\n{ghe.GiayConLai}s" : ghe.SoGhe?.ToString(),
                Width = 54,
                Height = 46,
                Margin = new Thickness(5),
                Tag = ghe,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = $"{ghe.HangGhe}{ghe.SoGhe} - {ghe.TenLoaiGhe} - {ghe.Gia:N0} đ"
            };

            if (ghe.TrangThai == "BaoTri")
            {
                btn.Background = Mau("#64748B");
                btn.IsEnabled = false;
                btn.Content = $"{ghe.SoGhe}\nBT";
            }
            else if (ghe.TrangThai == "DaBan")
            {
                btn.Background = Mau("#991B1B");
                btn.IsEnabled = false;
                btn.Content = $"{ghe.SoGhe}\nBán";
            }
            else if (dangChon)
            {
                btn.Background = Mau("#22C55E");
            }
            else if (ghe.TrangThai == "DangGiu")
            {
                btn.Background = Mau("#CA8A04");
                btn.Content = $"{ghe.SoGhe}\nGiữ";
            }
            else
            {
                btn.Background = Mau(ghe.MaLoaiGhe == 2 ? "#2563EB" : ghe.MaLoaiGhe == 3 ? "#7C3AED" : "#334155");
            }

            btn.Click += Seat_Click;
            return btn;
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            if (suatChieuDangChon == null || sender is not Button btn || btn.Tag is not GheBanVeInfo ghe)
                return;

            if (gheDangChon.ContainsKey(ghe.MaGhe))
            {
                banVeBUS.BoGiuGhe(suatChieuDangChon.MaSuatChieu, ghe.MaGhe);
                gheDangChon.Remove(ghe.MaGhe);
                LoadGhe(suatChieuDangChon.MaSuatChieu);
                TinhTien();
                return;
            }

            if (ghe.TrangThai == "DangGiu")
            {
                MessageBox.Show("Ghế này đang được giữ. Vui lòng chọn ghế khác.", "Thông báo");
                return;
            }

            var thanhCong = banVeBUS.GiuGhe(suatChieuDangChon.MaSuatChieu, ghe.MaGhe);
            if (!thanhCong)
            {
                MessageBox.Show("Không thể giữ ghế này. Vui lòng tải lại và chọn ghế khác.", "Thông báo");
                LoadGhe(suatChieuDangChon.MaSuatChieu);
                return;
            }

            ghe.TrangThai = "DangGiu";
            ghe.GiayConLai = 60;
            gheDangChon[ghe.MaGhe] = ghe;
            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void LoadDichVu()
        {
            var list = dichVuBUS.getDichVu()
                .Where(x => x.TrangThai == "Đang bán")
                .ToList();

            list.Insert(0, new SanPham { MaSanPham = 0, Ten = "---Chọn sản phẩm---" });
            cbDichVu.ItemsSource = list;
            cbDichVu.DisplayMemberPath = "Ten";
            cbDichVu.SelectedValuePath = "MaSanPham";
            cbDichVu.SelectedIndex = 0;
        }

        private void btnCong_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not DichVuTam item)
                return;

            item.SoLuong++;
            lvDichVu.Items.Refresh();
            TinhTien();
        }

        private void btnTru_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not DichVuTam item)
                return;

            if (item.SoLuong > 1)
                item.SoLuong--;
            else
                dichVus.Remove(item);

            lvDichVu.Items.Refresh();
            TinhTien();
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not DichVuTam item)
                return;

            dichVus.Remove(item);
            lvDichVu.Items.Refresh();
            TinhTien();
        }

        private void btnThemDichVu_Click(object sender, RoutedEventArgs e)
        {
            if (cbDichVu.SelectedItem is not SanPham dv || dv.MaSanPham == 0)
                return;

            var exist = dichVus.FirstOrDefault(x => x.MaSanPham == dv.MaSanPham);
            if (exist != null)
            {
                exist.SoLuong++;
            }
            else
            {
                dichVus.Add(new DichVuTam
                {
                    MaSanPham = dv.MaSanPham,
                    Ten = dv.Ten,
                    Gia = dv.Gia,
                    SoLuong = 1
                });
            }

            lvDichVu.ItemsSource = null;
            lvDichVu.ItemsSource = dichVus;
            TinhTien();
        }

        private void btnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (suatChieuDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn suất chiếu.", "Thông báo");
                return;
            }

            var phuongThuc = (cbPhuongThuc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "TienMat";
            var ketQua = banVeBUS.ThanhToan(
                suatChieuDangChon.MaSuatChieu,
                gheDangChon.Keys,
                dichVus,
                txtKhachHang.Text,
                txtSoDienThoai.Text,
                phuongThuc);

            if (!ketQua.ThanhCong)
            {
                MessageBox.Show(ketQua.ThongBao, "Thông báo");
                LoadGhe(suatChieuDangChon.MaSuatChieu);
                TinhTien();
                return;
            }

            if (ketQua.MaDonHang.HasValue)
            {
                var tickets = banVeBUS.GetVeTheoDonHang(ketQua.MaDonHang.Value);
                var preview = new TicketPreviewWindow(tickets)
                {
                    Owner = Window.GetWindow(this)
                };
                preview.ShowDialog();
            }

            gheDangChon.Clear();
            dichVus.Clear();
            lvDichVu.ItemsSource = null;
            txtKhachHang.Clear();
            txtSoDienThoai.Clear();
            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void RefreshGheDangChon()
        {
            if (suatChieuDangChon == null)
                return;

            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void TinhTien()
        {
            var tongVe = gheDangChon.Values.Sum(x => x.Gia);
            var tongDichVu = dichVus.Sum(x => x.SoLuong * x.Gia);
            var tong = tongVe + tongDichVu;

            txtTongTienDichVu.Text = DinhDangTien(tongDichVu);
            txtTongDichVuTomTat.Text = DinhDangTien(tongDichVu);
            txtTongTienVe.Text = DinhDangTien(tongVe);
            txtTongThanhToan.Text = DinhDangTien(tong);

            txtGheDangChon.Text = gheDangChon.Any()
                ? string.Join(", ", gheDangChon.Values.OrderBy(g => g.HangGhe).ThenBy(g => g.SoGhe).Select(g => $"{g.HangGhe}{g.SoGhe}"))
                : "Chưa chọn ghế";
        }

        private static string DinhDangTien(decimal value)
        {
            return value.ToString("N0") + " đ";
        }

        private static Brush Mau(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }
}
