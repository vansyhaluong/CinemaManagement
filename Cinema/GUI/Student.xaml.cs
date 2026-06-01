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
using DAL;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for Student.xaml
    /// </summary>
    public partial class Student : UserControl
    {
        private const string SearchPlaceholder = "Search employee...";
        private readonly NhanVienBUS bus = new NhanVienBUS();
        private readonly RapBUS rapBus = new RapBUS();
        private readonly PhanCaDAL phanCaDal = new PhanCaDAL();
        private List<NhanVien> allNhanVien = new();

        public Student()
        {
            InitializeComponent();
            LoadData();
            LoadRapComboBox();
            LoadStatusComboBox();
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
                LoadData();

            }
            else
            {
                MessageBox.Show("Xóa không thành công");
            }
        }
        public void LoadData()
        {
            allNhanVien = bus.getNhanVien();
            UpdateStatCards();
            ApplyFilters();
        }

        private void UpdateStatCards()
        {
            int totalEmployees = allNhanVien.Count;

            int workingToday = phanCaDal.GetAll(DateTime.Today)
                .Select(x => x.MaNhanVien)
                .Distinct()
                .Count();

            int managers = allNhanVien.Count(nv =>
                !string.IsNullOrWhiteSpace(nv.MaTaiKhoanNavigation?.VaiTro) &&
                !string.Equals(nv.MaTaiKhoanNavigation?.VaiTro, "NhanVien", StringComparison.CurrentCultureIgnoreCase));

            int inactive = allNhanVien.Count(nv => !nv.MaTaiKhoan.HasValue);

            cardTotalEmployees.Value = totalEmployees.ToString();
            cardWorkingToday.Value = workingToday.ToString();
            cardManagers.Value = managers.ToString();
            cardInactive.Value = inactive.ToString();
        }

        private void LoadRapComboBox()
        {
            var dsRap = rapBus.getAllRap();
            dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "All Branches" });

            cmbRap.ItemsSource = dsRap;
            cmbRap.SelectedValue = 0;
        }

        private void LoadStatusComboBox()
        {
            cmbStatus.ItemsSource = new List<string>
            {
                "All Status",
                "Active",
                "Inactive"
            };
            cmbStatus.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var keyword = txtSearch.Text == SearchPlaceholder ? string.Empty : txtSearch.Text.Trim();
            int maRap = cmbRap.SelectedValue is int rapValue ? rapValue : 0;
            string selectedStatus = cmbStatus.SelectedItem as string ?? "All Status";

            var filtered = allNhanVien.Where(nv =>
            {
                bool matchesKeyword =
                    string.IsNullOrWhiteSpace(keyword)
                    || (nv.HoTen?.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || (nv.SoDienThoai?.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || (nv.MaTaiKhoanNavigation?.TenDangNhap?.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || nv.MaNhanVien.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase);

                bool matchesRap = maRap <= 0 || nv.MaRap == maRap;

                string status = GetEmployeeStatus(nv);
                bool matchesStatus = selectedStatus == "All Status" || status == selectedStatus;

                return matchesKeyword && matchesRap && matchesStatus;
            }).ToList();

            dgvNhanVien.ItemsSource = filtered;
            lblPageInfo.Text = $"Showing {filtered.Count} of {allNhanVien.Count} employees";
        }

        private static string GetEmployeeStatus(NhanVien nv)
        {
            return nv.MaTaiKhoan.HasValue ? "Active" : "Inactive";
        }

       public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemNhanVien add = new ThemNhanVien();
            if (add.ShowDialog() == true)
            {
                LoadData();
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
                LoadData();
            }
        }

        private void cmbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != SearchPlaceholder)
            {
                return;
            }

            txtSearch.Text = string.Empty;
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                return;
            }

            txtSearch.Text = SearchPlaceholder;
        }
    }
}
