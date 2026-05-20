using BUS;
using Cinema.Models;
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
    /// Interaction logic for Student.xaml
    /// </summary>
    public partial class Student : UserControl
    {
        NhanVienBUS bus=new NhanVienBUS();
        public Student()
        {
            InitializeComponent();
            loadData();
            
        }
        public void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Button? btn=sender as Button;
            var item=btn?.Tag as NhanVien;
            if (item==null)
            {
                MessageBox.Show("Không tìm thấy nhân viên để xóa");
                return;
            }
            if (bus.xoaNhanVien(item.MaNhanVien))
            {
                MessageBox.Show("Xóa thành công");
                loadData();

            }
            else
            {
                MessageBox.Show("Xóa không thành công");
            }
        }
        public void loadData()
        {
            dgvNhanVien.ItemsSource = bus.getNhanVien();
        }
       public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemNhanVien add = new ThemNhanVien();
            if (add.ShowDialog() == true)
            {
                dgvNhanVien.ItemsSource = bus.getNhanVien();
            }
        }
        
        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
            => ((App)Application.Current).ThemeToggle();

        public void btnSua_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as FrameworkElement;
            var phong = border?.DataContext as NhanVien;
            if (phong == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên để sửa!");
                return;
            }
            ThemNhanVien add=new ThemNhanVien(phong);
            if (add.ShowDialog() == true)
            {
                dgvNhanVien.ItemsSource = bus.getNhanVien();
            }
        }
    }
}
