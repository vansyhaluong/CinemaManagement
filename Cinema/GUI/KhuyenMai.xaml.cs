using BUS;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ModelKhuyenMai = Cinema.Models.KhuyenMai;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for KhuyenMai.xaml
    /// </summary>
    public partial class KhuyenMai : UserControl
    {
        private readonly KhuyenMaiBUS bus = new KhuyenMaiBUS();
        private List<KhuyenMaiCardViewModel> allPromotions = new();

        public KhuyenMai()
        {
            InitializeComponent();
            LoadKhuyenMaiData();
        }

        private void LoadKhuyenMaiData()
        {
            var promotions = bus.GetKhuyenMais();
            allPromotions = promotions.Select(promotion => MapPromotion(promotion)).ToList();

            UpdateStats(allPromotions);
            ApplyFilters();
        }

        private void UpdateStats(List<KhuyenMaiCardViewModel> promotions)
        {
            txtTongKhuyenMai.Text = promotions.Count.ToString();
            txtDangHoatDong.Text = promotions.Count(x => x.TrangThaiHienThi == "Đang hoạt động").ToString();
            txtSapHetHan.Text = promotions.Count(x => x.TrangThaiHienThi == "Sắp hết hạn").ToString();
            txtDaHetHan.Text = promotions.Count(x => x.TrangThaiHienThi == "Đã hết hạn").ToString();
        }

        private void ApplyFilters()
        {
            var keyword = txtTimKiem.Text.Trim();
            var selectedStatus = GetSelectedComboBoxText(cbTrangThai);
            var selectedType = GetSelectedComboBoxText(cbLoai);
            var isAllStatus = cbTrangThai.SelectedIndex <= 0;
            var isAllType = cbLoai.SelectedIndex <= 0;

            var filtered = allPromotions.Where(x =>
            {
                var matchesKeyword = string.IsNullOrWhiteSpace(keyword)
                    || x.HienThiMa.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                    || x.TenKhuyenMai.Contains(keyword, StringComparison.CurrentCultureIgnoreCase);

                var matchesStatus = isAllStatus
                    || x.TrangThaiHienThi.Equals(selectedStatus, StringComparison.CurrentCultureIgnoreCase);

                var matchesType = isAllType
                    || x.LoaiGiamHienThi.Equals(selectedType, StringComparison.CurrentCultureIgnoreCase);

                return matchesKeyword && matchesStatus && matchesType;
            }).ToList();

            icKhuyenMai.ItemsSource = filtered;
            txtEmptyState.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static string GetSelectedComboBoxText(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        }

        private static KhuyenMaiCardViewModel MapPromotion(ModelKhuyenMai promotion)
        {
            var displayStatus = ResolveStatus(promotion);
            var (statusBackground, statusForeground) = GetStatusColors(displayStatus);

            var codeOrName = !string.IsNullOrWhiteSpace(promotion.MaCode)
                ? promotion.MaCode.Trim()
                : promotion.TenKhuyenMai?.Trim() ?? $"KM{promotion.MaKhuyenMai}";

            var startDate = promotion.NgayBatDau?.ToString("dd/MM/yyyy");
            var endDate = promotion.NgayKetThuc?.ToString("dd/MM/yyyy");

            return new KhuyenMaiCardViewModel
            {
                MaKhuyenMai = promotion.MaKhuyenMai,
                TenKhuyenMai = promotion.TenKhuyenMai?.Trim() ?? string.Empty,
                HienThiMa = codeOrName,
                MoTaHienThi = !string.IsNullOrWhiteSpace(promotion.MoTa)
                    ? promotion.MoTa.Trim()
                    : "Chưa có mô tả cho chương trình khuyến mãi này.",
                LoaiGiamHienThi = NormalizeLoaiGiam(promotion.LoaiGiam),
                HieuLucText = startDate != null && endDate != null
                    ? $"{startDate} - {endDate}"
                    : endDate != null
                        ? $"Đến {endDate}"
                        : "Không giới hạn",
                DaSuDungText = $"{promotion.DaDung ?? 0} / {promotion.SoLuong ?? 0}",
                DonToiThieuText = FormatCurrency(promotion.DonToiThieu),
                TrangThaiHienThi = displayStatus,
                TrangThaiNen = statusBackground,
                TrangThaiChu = statusForeground,
                HanhDongText = GetActionLabel(displayStatus),
                HanhDongChu = GetActionColors(displayStatus).Foreground,
                HanhDongVien = GetActionColors(displayStatus).Border
            };
        }

        private static string ResolveStatus(ModelKhuyenMai promotion)
        {
            var today = DateTime.Today;
            var rawStatus = promotion.TrangThai?.Trim().ToLowerInvariant() ?? string.Empty;

            if (rawStatus is "tamdung" or "tạm dừng")
            {
                return "Tạm dừng";
            }

            if (promotion.NgayKetThuc.HasValue && promotion.NgayKetThuc.Value.Date < today)
            {
                return "Đã hết hạn";
            }

            if (promotion.NgayBatDau.HasValue && promotion.NgayBatDau.Value.Date > today)
            {
                return "Sắp áp dụng";
            }

            if (promotion.NgayKetThuc.HasValue && promotion.NgayKetThuc.Value.Date <= today.AddDays(7))
            {
                return "Sắp hết hạn";
            }

            return "Đang hoạt động";
        }

        private static (string Background, string Foreground) GetStatusColors(string status)
        {
            return status switch
            {
                "Đang hoạt động" => ("#052E1B", "#4ADE80"),
                "Sắp hết hạn" => ("#3F2A05", "#FBBF24"),
                "Đã hết hạn" => ("#3F1D1D", "#FCA5A5"),
                "Tạm dừng" => ("#312E81", "#A5B4FC"),
                "Sắp áp dụng" => ("#0C4A6E", "#7DD3FC"),
                _ => ("#1E293B", "#CBD5E1")
            };
        }

        private static string NormalizeLoaiGiam(string? loaiGiam)
        {
            return loaiGiam?.Trim().ToLowerInvariant() switch
            {
                "tienmat" => "Giảm tiền",
                "phantram" => "Giảm phần trăm",
                _ => loaiGiam ?? "Khác"
            };
        }

        private static string GetActionLabel(string status)
        {
            return status switch
            {
                "Đang hoạt động" => "Tạm dừng",
                "Tạm dừng" => "Kích hoạt",
                "Đã hết hạn" => "Gia hạn",
                "Sắp áp dụng" => "Chỉnh sửa",
                "Sắp hết hạn" => "Gia hạn",
                _ => "Xem"
            };
        }

        private static (string Foreground, string Border) GetActionColors(string status)
        {
            return status switch
            {
                "Đang hoạt động" => ("#FBBF24", "#D97706"),
                "Tạm dừng" => ("#93C5FD", "#3B82F6"),
                "Đã hết hạn" => ("#FCA5A5", "#EF4444"),
                "Sắp áp dụng" => ("#7DD3FC", "#0EA5E9"),
                "Sắp hết hạn" => ("#FCD34D", "#F59E0B"),
                _ => ("#CBD5E1", "#64748B")
            };
        }

        private static string FormatCurrency(decimal? value)
        {
            if (!value.HasValue)
            {
                return "Không yêu cầu";
            }

            return string.Format(new CultureInfo("vi-VN"), "{0:#,##0}đ", value.Value);
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiem.Text = string.Empty;
            cbTrangThai.SelectedIndex = 0;
            cbLoai.SelectedIndex = 0;
            ApplyFilters();
        }

        private void btnThemKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            var frm = new ThemKhuyenMai();
            if (frm.ShowDialog() == true)
            {
                LoadKhuyenMaiData();
            }
        }

        private void btnSuaKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int maKhuyenMai)
            {
                MessageBox.Show("Không tìm thấy khuyến mãi để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var promotion = bus.GetKhuyenMaiById(maKhuyenMai);
            if (promotion == null)
            {
                MessageBox.Show("Khuyến mãi không còn tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var frm = new ThemKhuyenMai(promotion);
            if (frm.ShowDialog() == true)
            {
                LoadKhuyenMaiData();
            }
        }

        private void btnTrangThaiKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int maKhuyenMai)
            {
                MessageBox.Show("Không tìm thấy khuyến mãi để cập nhật trạng thái.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var promotion = bus.GetKhuyenMaiById(maKhuyenMai);
            if (promotion == null)
            {
                MessageBox.Show("Khuyến mãi không còn tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var displayStatus = ResolveStatus(promotion);
            var actionLabel = GetActionLabel(displayStatus);

            try
            {
                switch (displayStatus)
                {
                    case "Đang hoạt động":
                    {
                        if (MessageBox.Show(
                                $"Bạn có muốn {actionLabel.ToLower()} khuyến mãi này không?",
                                "Xác nhận",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            return;
                        }

                        bus.CapNhatTrangThai(maKhuyenMai, "Tạm dừng");
                        MessageBox.Show("Đã tạm dừng khuyến mãi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadKhuyenMaiData();
                        break;
                    }
                    case "Tạm dừng":
                    {
                        if (MessageBox.Show(
                                "Bạn có muốn kích hoạt lại khuyến mãi này không?",
                                "Xác nhận",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            return;
                        }

                        bus.CapNhatTrangThai(maKhuyenMai, "Đang hoạt động");
                        MessageBox.Show("Đã kích hoạt khuyến mãi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadKhuyenMaiData();
                        break;
                    }
                    case "Sắp áp dụng":
                    case "Sắp hết hạn":
                    case "Đã hết hạn":
                    {
                        var frm = new ThemKhuyenMai(promotion);
                        if (frm.ShowDialog() == true)
                        {
                            LoadKhuyenMaiData();
                        }

                        break;
                    }
                    default:
                        MessageBox.Show("Trạng thái khuyến mãi hiện tại chưa hỗ trợ thao tác này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private sealed class KhuyenMaiCardViewModel
        {
            public int MaKhuyenMai { get; set; }
            public string TenKhuyenMai { get; set; } = string.Empty;
            public string HienThiMa { get; set; } = string.Empty;
            public string MoTaHienThi { get; set; } = string.Empty;
            public string LoaiGiamHienThi { get; set; } = string.Empty;
            public string HieuLucText { get; set; } = string.Empty;
            public string DaSuDungText { get; set; } = string.Empty;
            public string DonToiThieuText { get; set; } = string.Empty;
            public string TrangThaiHienThi { get; set; } = string.Empty;
            public string TrangThaiNen { get; set; } = string.Empty;
            public string TrangThaiChu { get; set; } = string.Empty;
            public string HanhDongText { get; set; } = string.Empty;
            public string HanhDongChu { get; set; } = string.Empty;
            public string HanhDongVien { get; set; } = string.Empty;
        }
    }
}
