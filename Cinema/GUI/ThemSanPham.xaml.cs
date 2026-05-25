using BUS;
using Cinema.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ThemSanPham.xaml
    /// </summary>
    public partial class ThemSanPham : Window
    {
        
        private SanPhamBUS spBUS = new SanPhamBUS();

        public SanPham SanPhamMoi { get; private set; }

        private string duongDanAnhLuuDb = "";

        public ThemSanPham()
        {
            InitializeComponent();
            LoadLoaiSanPham();
        }

        private void LoadLoaiSanPham()
        {
            cbLoaiSanPham.ItemsSource = spBUS.GetLoaiSanPham();

            if (cbLoaiSanPham.Items.Count > 0)
                cbLoaiSanPham.SelectedIndex = 0;
        }

        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Ảnh sản phẩm (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (open.ShowDialog() == true)
            {
                string sourcePath = open.FileName;

                string imagesFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Images"
                );

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(sourcePath);

                string destPath = System.IO.Path.Combine(imagesFolder, fileName);

                File.Copy(sourcePath, destPath, true);

                duongDanAnhLuuDb = System.IO.Path.Combine("Images", fileName);

                //txtHinhAnh.Text = duongDanAnhLuuDb;

                imgPreview.Source = new BitmapImage(
                    new Uri(destPath, UriKind.Absolute)
                );
            }
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

            if (cbLoaiSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm!");
                return;
            }

            SanPhamMoi = new SanPham
            {
                Ten = txtTen.Text.Trim(),
                Gia = gia,
                MaLoaiSp = (int)cbLoaiSanPham.SelectedValue,
                TrangThai = "Đang bán",
                HinhAnh = string.IsNullOrWhiteSpace(duongDanAnhLuuDb)
                    ? ""
                    : duongDanAnhLuuDb
            };

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

