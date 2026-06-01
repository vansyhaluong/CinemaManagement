using BUS;
using Cinema.Models;
using DAL.Models;
using DTO;
using System;
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
        private KhuyenMaiApDungInfo? khuyenMaiDangApDung;

        private SuatChieu? suatChieuDangChon;

        public BanVe(Phim p)
        {
            InitializeComponent();

            phim = p;
            holdTimer.Interval = TimeSpan.FromSeconds(5);
            holdTimer.Tick += HoldTimer_Tick;
            holdTimer.Start();

            Unloaded += BanVe_Unloaded;

            LoadPhim();
            LoadSuatChieu();
            LoadDichVu();
            lvDichVu.ItemsSource = dichVus;
            TinhTien();
        }

        private void LoadPhim()
        {
            txtTenPhim.Text = phim.TieuDe;
            txtThongTin.Text = $"{movieBUS.getTheLoaiByPhim(phim.MaPhim)} - {phim.ThoiLuong} phut";
        }

        private void LoadSuatChieu()
        {
            panelNgaySuatChieu.Children.Clear();

            int? maRap = Session.IsAdmin ? null : Session.MaRap;
            var dsSuat = suatChieuBUS.getSuatChieuByPhim(phim.MaPhim, maRap)
                .Where(x => x.ThoiGianBatDau != null && x.ThoiGianBatDau.Value >= DateTime.Now)
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

            if (suatChieuDangChon?.MaSuatChieu != sc.MaSuatChieu)
                GiaiPhongGheDangGiu();

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
                .Where(g => LaTrangThaiDangGiu(g.TrangThai))
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
                BorderThickness = new Thickness(1),
                BorderBrush = Mau("#475569"),
                Background = Brushes.Transparent,
                Foreground = Mau("#E2E8F0"),
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = $"{ghe.HangGhe}{ghe.SoGhe} - {ghe.TenLoaiGhe} - {ghe.Gia:N0} đ"
            };

            if (LaTrangThaiBaoTri(ghe.TrangThai))
            {
                btn.Background = Mau("#64748B");
                btn.BorderBrush = Mau("#64748B");
                btn.IsEnabled = false;
                btn.Content = $"{ghe.SoGhe}\nBT";
            }
            else if (LaTrangThaiDaBan(ghe.TrangThai))
            {
                btn.Background = Mau("#991B1B");
                btn.BorderBrush = Mau("#991B1B");
                btn.IsEnabled = false;
                btn.Content = $"{ghe.SoGhe}\nBan";
            }
            else if (dangChon)
            {
                btn.Background = Mau("#22C55E");
                btn.BorderBrush = Mau("#22C55E");
                btn.Foreground = Brushes.White;
            }
            else if (LaTrangThaiDangGiu(ghe.TrangThai))
            {
                btn.Background = Mau("#CA8A04");
                btn.BorderBrush = Mau("#CA8A04");
                btn.Foreground = Brushes.White;
                btn.Content = $"{ghe.SoGhe}\n{Math.Max(0, ghe.GiayConLai)}s";
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

            if (LaTrangThaiDangGiu(ghe.TrangThai))
            {
                MessageBox.Show("Ghế này đang được giữ. Vui lòng chọn ghế khác.", "Thông báo");
                return;
            }

            var thanhCong = banVeBUS.GiuGhe(suatChieuDangChon.MaSuatChieu, ghe.MaGhe);
            if (!thanhCong)
            {
                MessageBox.Show("Không thể giữ ghế này. Vui long tai lai va chon ghe khac.", "Thông báo");
                LoadGhe(suatChieuDangChon.MaSuatChieu);
                return;
            }

            ghe.TrangThai = "Đang giữ";
            ghe.GiayConLai = 60;
            gheDangChon[ghe.MaGhe] = ghe;
            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void LoadDichVu()
        {
            var list = dichVuBUS.getDichVu()
                .Where(x => LaTrangThaiDangBanSanPham(x.TrangThai))
                .OrderBy(x => x.Ten)
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

            if (!CoTheTangSoLuong(item, true))
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
                if (!CoTheTangSoLuong(exist, true))
                    return;

                exist.SoLuong++;
            }
            else
            {
                var tonKho = banVeBUS.GetSoLuongTonDichVu(dv.MaSanPham);
                if (tonKho <= 0)
                {
                    MessageBox.Show($"Dich vụ {dv.Ten} đã hết hàng.", "Thông báo");
                    return;
                }

                dichVus.Add(new DichVuTam
                {
                    MaSanPham = dv.MaSanPham,
                    Ten = dv.Ten,
                    Gia = dv.Gia,
                    SoLuong = 1
                });
            }

            lvDichVu.Items.Refresh();
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
            if (phuongThuc == "Chuyển khoản")
            {
                var tongThanhToan = gheDangChon.Values.Sum(x => x.Gia) + dichVus.Sum(x => x.SoLuong * x.Gia) - (khuyenMaiDangApDung?.SoTienGiam ?? 0);
                if (tongThanhToan < 0)
                    tongThanhToan = 0;

                var popupQr = new QrThanhToanWindow(
                    tongThanhToan,
                    TaoNoiDungChuyenKhoan())
                {
                    Owner = Window.GetWindow(this)
                };

                if (popupQr.ShowDialog() != true)
                    return;
            }

            var ketQua = banVeBUS.ThanhToan(
                suatChieuDangChon.MaSuatChieu,
                gheDangChon.Keys,
                dichVus,
                txtKhachHang.Text,
                txtSoDienThoai.Text,
                phuongThuc,
                khuyenMaiDangApDung?.MaCode,
                Session.MaNhanVien);

            if (!ketQua.ThanhCong)
            {
                MessageBox.Show(ketQua.ThongBao, "Thong bao");
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
            lvDichVu.Items.Refresh();
            txtKhachHang.Clear();
            txtSoDienThoai.Clear();
            txtMaKhuyenMai.Clear();
            khuyenMaiDangApDung = null;
            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void HoldTimer_Tick(object? sender, EventArgs e)
        {
            RefreshGheDangChon();
        }

        private void BanVe_Unloaded(object sender, RoutedEventArgs e)
        {
            holdTimer.Stop();
            holdTimer.Tick -= HoldTimer_Tick;
            Unloaded -= BanVe_Unloaded;
            GiaiPhongGheDangGiu();
        }

        private void RefreshGheDangChon()
        {
            if (suatChieuDangChon == null)
                return;

            LoadGhe(suatChieuDangChon.MaSuatChieu);
            TinhTien();
        }

        private void GiaiPhongGheDangGiu()
        {
            if (suatChieuDangChon == null || !gheDangChon.Any())
                return;

            foreach (var maGhe in gheDangChon.Keys.ToList())
            {
                banVeBUS.BoGiuGhe(suatChieuDangChon.MaSuatChieu, maGhe);
            }
        }

        private bool CoTheTangSoLuong(DichVuTam item, bool hienThongBao)
        {
            var tonKho = banVeBUS.GetSoLuongTonDichVu(item.MaSanPham);
            if (item.SoLuong >= tonKho)
            {
                if (hienThongBao)
                    MessageBox.Show($"Dich vu {item.Ten} chi con {tonKho} san pham trong kho.", "Thong bao");

                return false;
            }

            return true;
        }

        private void TinhTien()
        {
            var tongVe = gheDangChon.Values.Sum(x => x.Gia);
            var tongDichVu = dichVus.Sum(x => x.SoLuong * x.Gia);
            var tamTinh = tongVe + tongDichVu;

            if (khuyenMaiDangApDung != null)
            {
                var ketQuaKhuyenMai = banVeBUS.KiemTraKhuyenMai(khuyenMaiDangApDung.MaCode, tamTinh);
                if (ketQuaKhuyenMai.ThanhCong)
                {
                    khuyenMaiDangApDung = ketQuaKhuyenMai;
                }
                else
                {
                    khuyenMaiDangApDung = null;
                    txtThongTinKhuyenMai.Text = "Khuyến mãi đã được gỡ vì hóa đơn không còn đủ điều kiện.";
                    txtThongTinKhuyenMai.Foreground = Mau("#FBBF24");
                }
            }

            var tienGiam = khuyenMaiDangApDung?.SoTienGiam ?? 0;
            var tong = tamTinh - tienGiam;
            if (tong < 0)
                tong = 0;

            txtTongTienDichVu.Text = DinhDangTien(tongDichVu);
            txtTongDichVuTomTat.Text = DinhDangTien(tongDichVu);
            txtTongTienVe.Text = DinhDangTien(tongVe);
            txtTienGiam.Text = DinhDangTien(tienGiam);
            txtTongThanhToan.Text = DinhDangTien(tong);

            txtGheDangChon.Text = gheDangChon.Any()
                ? string.Join(", ", gheDangChon.Values.OrderBy(g => g.HangGhe).ThenBy(g => g.SoGhe).Select(g => $"{g.HangGhe}{g.SoGhe}"))
                : "Chua chon ghe";

            if (khuyenMaiDangApDung != null)
            {
                txtThongTinKhuyenMai.Text = $"{khuyenMaiDangApDung.MaCode} - giam {DinhDangTien(khuyenMaiDangApDung.SoTienGiam)}";
                txtThongTinKhuyenMai.Foreground = Mau("#22C55E");
            }
            else if (!txtThongTinKhuyenMai.Text.Contains("Khuyến mãi đã được gỡ"))
            {
                txtThongTinKhuyenMai.Text = "Chưa áp dụng khuyến mãi";
                txtThongTinKhuyenMai.Foreground = Mau("#94A3B8");
            }
        }

        private void btnApDungKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            var maCode = txtMaKhuyenMai.Text.Trim();
            var tamTinh = gheDangChon.Values.Sum(x => x.Gia) + dichVus.Sum(x => x.SoLuong * x.Gia);
            var ketQua = banVeBUS.KiemTraKhuyenMai(maCode, tamTinh);

            if (!ketQua.ThanhCong)
            {
                MessageBox.Show(ketQua.ThongBao, "Thông báo");
                return;
            }

            khuyenMaiDangApDung = ketQua;
            txtMaKhuyenMai.Text = ketQua.MaCode;
            TinhTien();
            MessageBox.Show("Áp dụng khuyến mãi thành công.", "Thông báo");
        }

        private void btnBoKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            khuyenMaiDangApDung = null;
            txtMaKhuyenMai.Clear();
            txtThongTinKhuyenMai.Text = "Chưa áp dụng khuyến mãi";
            txtThongTinKhuyenMai.Foreground = Mau("#94A3B8");
            TinhTien();
        }

        private static string DinhDangTien(decimal value)
        {
            return value.ToString("N0") + " đ";
        }

        private static Brush Mau(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        private static bool LaTrangThaiDangGiu(string? trangThai)
        {
            var value = (trangThai ?? string.Empty).Trim().ToLowerInvariant();
            return value.Contains("danggiu") || value.Contains("đang giữ") || value.Contains("giữ");
        }

        private static bool LaTrangThaiDaBan(string? trangThai)
        {
            var value = (trangThai ?? string.Empty).Trim().ToLowerInvariant();
            return value.Contains("daban") || value.Contains("đã bán") || value.Contains("bán");
        }

        private static bool LaTrangThaiBaoTri(string? trangThai)
        {
            var value = (trangThai ?? string.Empty).Trim().ToLowerInvariant();
            return value.Contains("baotri") || value.Contains("bảo trì") || value.Contains("trì");
        }

        private static bool LaTrangThaiDangBanSanPham(string? trangThai)
        {
            var value = (trangThai ?? string.Empty).Trim().ToLowerInvariant();
            return value.Contains("dangban") || value.Contains("đang bán") || value.Contains("bán");
        }

        private string TaoNoiDungChuyenKhoan()
        {
            return $"HD{DateTime.Now:ddMMyyHHmmss}{phim.MaPhim:D2}";
        }
    }
}


