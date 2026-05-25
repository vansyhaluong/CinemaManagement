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
    /// Interaction logic for QuanLyKho.xaml
    /// </summary>
    public partial class QuanLyKho : UserControl
    {
        SanPhamBUS spBUS = new SanPhamBUS();

        List<SanPhamKhoDTO> dsSanPham = new();
        public QuanLyKho()
        {
            InitializeComponent();
            LoadData();
            LoadLoaiSanPham();
        }
        private void LoadData()
        {
            dsSanPham = spBUS.GetDanhSachKho();

            icSanPham.ItemsSource = dsSanPham;

            CapNhatThongKe(dsSanPham);
        }
        private void BtnLocLoai_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LoaiSanPham loai)
            {
                var list = dsSanPham
                    .Where(x => x.TenLoai == loai.TenLoai)
                    .ToList();

                icSanPham.ItemsSource = list;
                CapNhatThongKe(list);
            }
        }
        private void CapNhatThongKe(List<SanPhamKhoDTO> data)
        {
            cardTongSP.Value = data.Count.ToString();

            cardTongTon.Value = data
                .Sum(x => x.SoLuongTon)
                .ToString();

            cardSapHet.Value = data
                .Count(x => x.SoLuongTon > 0 && x.SoLuongTon <= 10)
                .ToString();

            cardHetHang.Value = data
                .Count(x => x.SoLuongTon <= 0)
                .ToString();
        }
        private void LocSanPham()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (keyword == "tìm sản phẩm...")
                keyword = "";

            string status = "";

            if (cmbStatus.SelectedItem is ComboBoxItem item)
                status = item.Content.ToString();

            var result = dsSanPham.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(x =>
                    x.Ten.ToLower().Contains(keyword)
                    || x.TenLoai.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(status)
                && status != "Tất cả trạng thái")
            {
                result = result.Where(x =>
                    x.TrangThaiKho == status);
            }

            var list = result.ToList();

            icSanPham.ItemsSource = list;

            CapNhatThongKe(list);
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (icSanPham != null)
                LocSanPham();
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (icSanPham != null)
                LocSanPham();
        }

        private void BtnLoc_Click(object sender, RoutedEventArgs e)
        {
            LocSanPham();
        }

        private void BtnTatCa_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbStatus.SelectedIndex = 0;

            icSanPham.ItemsSource = dsSanPham;

            CapNhatThongKe(dsSanPham);
        }

        
        private void BtnNhapKho_Click(object sender, RoutedEventArgs e)
        {
            var frm = new NhapKhoWindow();

            if (frm.ShowDialog() == true)
            {
                LoadData();
            }
        }
        private void LoadLoaiSanPham()
        {
            icLoaiSanPham.ItemsSource = spBUS.GetLoaiSanPham();
        }

        private void BtnThemSanPham_Click(object sender, RoutedEventArgs e)
        {
            ThemSanPham frm = new ThemSanPham();

            if (frm.ShowDialog() == true)
            {
                bool kq = spBUS.ThemSanPham(frm.SanPhamMoi);

                if (kq)
                {
                    MessageBox.Show("Thêm sản phẩm thành công!");

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Thêm sản phẩm thất bại!");
                }
            }
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SanPhamKhoDTO sp)
            {
                var frm = new SuaSanPhamWindow(sp);

                if (frm.ShowDialog() == true)
                {
                    bool kq = spBUS.SuaSanPham(frm.SanPham);

                    if (kq)
                    {
                        MessageBox.Show("Cập nhật sản phẩm thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật sản phẩm thất bại!");
                    }
                }
            }
        }

        public void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SanPhamKhoDTO sp)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa sản phẩm \"{sp.Ten}\" không?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    bool kq = spBUS.XoaSanPham(sp.MaSanPham);

                    if (kq)
                    {
                        MessageBox.Show("Xóa sản phẩm thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa sản phẩm thất bại!");
                    }
                }
            }

        }

    }
}
