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
using System.Windows.Shapes;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for SuaSanPhamWindow.xaml
    /// </summary>
    public partial class SuaSanPhamWindow : Window
    {
        public SanPhamKhoDTO SanPham { get; private set; }
        private readonly bool laCombo;
        public SuaSanPhamWindow(SanPhamKhoDTO sp)
        {
            InitializeComponent();
            laCombo = string.Equals(sp.TenLoai, "Combo", StringComparison.OrdinalIgnoreCase);
            SanPham = new SanPhamKhoDTO
            {
                MaSanPham = sp.MaSanPham,
                Ten = sp.Ten,
                TenLoai = sp.TenLoai,
                Gia = sp.Gia,
                SoLuongTon = sp.SoLuongTon,
                HinhAnh = sp.HinhAnh
            };

            txtTen.Text = SanPham.Ten;
            txtGia.Text = SanPham.Gia.ToString("0");
            txtSoLuongTon.Text = SanPham.SoLuongTon.ToString();
            txtSoLuongTon.IsEnabled = !laCombo;
            lblSoLuongTon.Visibility = laCombo ? Visibility.Collapsed : Visibility.Visible;
            txtSoLuongTon.Visibility = laCombo ? Visibility.Collapsed : Visibility.Visible;
        }
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Tên sản phẩm không được để trống!");
                return;
            }

            if (!decimal.TryParse(txtGia.Text, out decimal gia) || gia < 0)
            {
                MessageBox.Show("Giá bán không hợp lệ!");
                return;
            }
            if (!int.TryParse(txtSoLuongTon.Text, out int soLuongTon) || soLuongTon < 0)
            {
                MessageBox.Show("Số lượng tồn kho không hợp lệ!");
                return;
            }
            if (!laCombo)
            {
                SanPham.SoLuongTon = soLuongTon;
            }
            SanPham.Ten = txtTen.Text.Trim();
            SanPham.Gia = gia;

            DialogResult = true;
            Close();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
