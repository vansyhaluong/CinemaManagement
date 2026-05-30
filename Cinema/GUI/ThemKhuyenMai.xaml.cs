using BUS;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModelKhuyenMai = Cinema.Models.KhuyenMai;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for ThemKhuyenMai.xaml
    /// </summary>
    public partial class ThemKhuyenMai : Window
    {
        private readonly KhuyenMaiBUS bus = new KhuyenMaiBUS();
        private readonly ModelKhuyenMai? editingPromotion;

        public ThemKhuyenMai(ModelKhuyenMai? promotion = null)
        {
            InitializeComponent();
            editingPromotion = promotion;

            btnHuy.Click += (_, _) => Close();
            InitializeDefaults();

            if (editingPromotion != null)
            {
                LoadPromotionData(editingPromotion);
            }
        }

        private void InitializeDefaults()
        {
            cbLoaiGiam.SelectedIndex = 0;
            cbTrangThai.SelectedIndex = 0;
            dpNgayBatDau.SelectedDate = DateTime.Today;
            dpNgayKetThuc.SelectedDate = DateTime.Today.AddMonths(1);

            if (editingPromotion == null)
            {
                txtSoLuong.Text = "0";
                txtDonToiThieu.Text = "0";
                txtGiaTriGiam.Text = "0";
            }
        }

        private void LoadPromotionData(ModelKhuyenMai promotion)
        {
            Title = "Sửa khuyến mãi";
            btnLuu.Content = "Cập nhật khuyến mãi";

            txtTenKhuyenMai.Text = promotion.TenKhuyenMai ?? string.Empty;
            txtMaCode.Text = promotion.MaCode ?? string.Empty;
            txtMoTa.Text = promotion.MoTa ?? string.Empty;
            txtGiaTriGiam.Text = FormatDecimal(promotion.GiaTriGiam);
            txtDonToiThieu.Text = FormatDecimal(promotion.DonToiThieu);
            txtSoLuong.Text = (promotion.SoLuong ?? 0).ToString();
            dpNgayBatDau.SelectedDate = promotion.NgayBatDau?.Date;
            dpNgayKetThuc.SelectedDate = promotion.NgayKetThuc?.Date;

            SelectComboBoxItem(cbLoaiGiam, NormalizeLoaiGiam(promotion.LoaiGiam));
            SelectComboBoxItem(cbTrangThai, NormalizeTrangThai(promotion.TrangThai));
        }

        public void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var khuyenMai = BuildPromotionFromForm();

                if (editingPromotion == null)
                {
                    bus.ThemKhuyenMai(khuyenMai);
                    MessageBox.Show("Thêm khuyến mãi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    khuyenMai.MaKhuyenMai = editingPromotion.MaKhuyenMai;
                    khuyenMai.DaDung = editingPromotion.DaDung;
                    bus.CapNhatKhuyenMai(khuyenMai);
                    MessageBox.Show("Cập nhật khuyến mãi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private ModelKhuyenMai BuildPromotionFromForm()
        {
            var tenKhuyenMai = txtTenKhuyenMai.Text.Trim();
            var maCode = txtMaCode.Text.Trim();
            var moTa = txtMoTa.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenKhuyenMai))
            {
                throw new Exception("Vui lòng nhập tên khuyến mãi.");
            }

            if (string.IsNullOrWhiteSpace(maCode))
            {
                throw new Exception("Vui lòng nhập mã khuyến mãi.");
            }

            if (!decimal.TryParse(txtGiaTriGiam.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var giaTriGiam)
                && !decimal.TryParse(txtGiaTriGiam.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out giaTriGiam))
            {
                throw new Exception("Giá trị giảm không hợp lệ.");
            }

            if (!decimal.TryParse(txtDonToiThieu.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var donToiThieu)
                && !decimal.TryParse(txtDonToiThieu.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out donToiThieu))
            {
                throw new Exception("Đơn tối thiểu không hợp lệ.");
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out var soLuong))
            {
                throw new Exception("Số lượng áp dụng không hợp lệ.");
            }

            if (giaTriGiam <= 0)
            {
                throw new Exception("Giá trị giảm phải lớn hơn 0.");
            }

            if (donToiThieu < 0)
            {
                throw new Exception("Đơn tối thiểu không được âm.");
            }

            if (soLuong < 0)
            {
                throw new Exception("Số lượng áp dụng không được âm.");
            }

            if (!dpNgayBatDau.SelectedDate.HasValue)
            {
                throw new Exception("Vui lòng chọn ngày bắt đầu.");
            }

            if (!dpNgayKetThuc.SelectedDate.HasValue)
            {
                throw new Exception("Vui lòng chọn ngày kết thúc.");
            }

            if (dpNgayKetThuc.SelectedDate.Value.Date < dpNgayBatDau.SelectedDate.Value.Date)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            return new ModelKhuyenMai
            {
                TenKhuyenMai = tenKhuyenMai,
                MaCode = maCode,
                MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa,
                LoaiGiam = MapLoaiGiamToDb(GetSelectedComboBoxText(cbLoaiGiam)),
                GiaTriGiam = giaTriGiam,
                DonToiThieu = donToiThieu,
                NgayBatDau = dpNgayBatDau.SelectedDate.Value,
                NgayKetThuc = dpNgayKetThuc.SelectedDate.Value,
                SoLuong = soLuong,
                TrangThai = GetSelectedComboBoxText(cbTrangThai),
                DaDung = editingPromotion?.DaDung ?? 0
            };
        }

        private static string GetSelectedComboBoxText(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        }

        private static void SelectComboBoxItem(ComboBox comboBox, string targetContent)
        {
            foreach (var item in comboBox.Items.OfType<ComboBoxItem>())
            {
                if (string.Equals(item.Content?.ToString(), targetContent, StringComparison.CurrentCultureIgnoreCase))
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
        }

        private static string NormalizeLoaiGiam(string? loaiGiam)
        {
            return loaiGiam?.Trim().ToLowerInvariant() switch
            {
                "tienmat" => "Giảm tiền",
                "phantram" => "Giảm phần trăm",
                _ => string.IsNullOrWhiteSpace(loaiGiam) ? "Giảm tiền" : loaiGiam
            };
        }

        private static string NormalizeTrangThai(string? trangThai)
        {
            return trangThai?.Trim().ToLowerInvariant() switch
            {
                "hoatdong" => "Đang hoạt động",
                "tamdung" => "Tạm dừng",
                _ => string.IsNullOrWhiteSpace(trangThai) ? "Đang hoạt động" : trangThai
            };
        }

        private static string MapLoaiGiamToDb(string loaiGiam)
        {
            return loaiGiam.Trim().ToLowerInvariant() switch
            {
                "giảm tiền" => "TienMat",
                "giảm phần trăm" => "PhanTram",
                _ => loaiGiam
            };
        }

        private static string FormatDecimal(decimal? value)
        {
            return value?.ToString("0.##", CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
