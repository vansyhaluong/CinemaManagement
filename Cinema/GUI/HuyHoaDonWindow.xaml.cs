using DAL.Models;
using System.Windows;
using System.Windows.Controls;

namespace Cinema.GUI
{
    public partial class HuyHoaDonWindow : Window
    {
        public string LyDoHuy { get; private set; } = string.Empty;

        public HuyHoaDonWindow(HoaDonDetailInfo detail)
        {
            InitializeComponent();
            LoadData(detail);
            cbLyDo.SelectedIndex = 0;
        }

        private void LoadData(HoaDonDetailInfo detail)
        {
            txtMaHoaDon.Text = detail.MaHoaDon;
            txtNgayLap.Text = detail.NgayLap;
            txtKhachHang.Text = detail.KhachHang;
            txtTongTien.Text = detail.TongTien;
            txtTenPhim.Text = detail.TenPhim;
            txtSuatChieu.Text = detail.SuatChieu;
            txtPhong.Text = detail.PhongChieu;
            txtGhe.Text = string.IsNullOrWhiteSpace(detail.Ghe) ? "--" : detail.Ghe;
        }

        private void cbLyDo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (cbLyDo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var isOther = selected == "Khác";
            txtLyDoKhac.Visibility = isOther ? Visibility.Visible : Visibility.Collapsed;

            if (!isOther)
            {
                txtLyDoKhac.Clear();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var selected = (cbLyDo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selected))
            {
                MessageBox.Show("Vui lòng chọn lý do hủy.", "Thông báo");
                return;
            }

            if (selected == "Khác")
            {
                if (string.IsNullOrWhiteSpace(txtLyDoKhac.Text))
                {
                    MessageBox.Show("Vui lòng nhập lý do hủy cụ thể.", "Thông báo");
                    txtLyDoKhac.Focus();
                    return;
                }

                LyDoHuy = txtLyDoKhac.Text.Trim();
            }
            else
            {
                LyDoHuy = selected;
            }

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
