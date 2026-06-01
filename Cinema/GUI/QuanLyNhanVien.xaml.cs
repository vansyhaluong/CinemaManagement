using BUS;
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
using Cinema.Models;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLyNhanVien.xaml
    /// </summary>
    public partial class QuanLyNhanVien : UserControl
    {
        private readonly NhanVienBUS bus = new NhanVienBUS();
        private readonly RapBUS rapBUS = new RapBUS();
        private List<NhanVien> allNhanVien = new();

        public QuanLyNhanVien()
        {
            InitializeComponent();
            LoadNhanVienData();
            LoadRapComboBox();
        }

        private void LoadNhanVienData()
        {
            allNhanVien = bus.getNhanVien();
            ApplyRapFilter();
        }

        private void LoadRapComboBox()
        {
            var dsRap = rapBUS.getAllRap();
            dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "Tất cả rạp" });

            cbRap.ItemsSource = dsRap;
            cbRap.SelectedValue = 0;
        }

        private void ApplyRapFilter()
        {
            int maRap = cbRap.SelectedValue is int value ? value : 0;

            if (maRap <= 0)
            {
                dgNhanVien.ItemsSource = allNhanVien;
                return;
            }

            dgNhanVien.ItemsSource = allNhanVien
                .Where(x => x.MaRap == maRap)
                .ToList();
        }

        private void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyRapFilter();
        }
    }
}
