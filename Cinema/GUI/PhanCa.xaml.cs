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
    /// Interaction logic for PhanCa.xaml
    /// </summary>
    public partial class PhanCa : UserControl
    {
        private readonly PhanCaBUS phanCaBUS = new();
        private readonly RapBUS rapBUS = new();
        private readonly NhanVienBUS nhanVienBUS = new();
        private readonly CaLamBUS caLamBUS = new();

        private List<PhanCaDTO> dsPhanCa = new();

        private int selectedMaPhanCa = 0;
        public PhanCa()
        {
            InitializeComponent();
            Loaded += QuanLyPhanCa_Loaded;
        }
        private void QuanLyPhanCa_Loaded(object sender, RoutedEventArgs e)
        {
            dpNgay.SelectedDate = DateTime.Now;

            LoadRap();
            LoadCa();
            LoadNhanVien();
            LoadData();
        }

        // ========================= LOAD =========================

        private void LoadRap()
        {
            var dsRap = rapBUS.getAllRap();

            dsRap.Insert(0, new Rap
            {
                MaRap = 0,
                TenRap = "Tất cả rạp"
            });

            cbRap.ItemsSource = dsRap;
            cbRap.SelectedIndex = 0;
        }

        private void LoadCa()
        {
            var dsCa = caLamBUS.GetAll();

            dsCa.Insert(0, new CaLam
            {
                MaCa = 0,
                TenCa = "Tất cả ca"
            });

            cbCa.ItemsSource = dsCa;
            cbCa.SelectedIndex = 0;
        }

        private void LoadNhanVien()
        {
            var dsNV = nhanVienBUS.getNhanVien();

            cbNhanVien.ItemsSource = dsNV;
            cbNhanVien.SelectedIndex = 0;
        }

        private void LoadData()
        {
            if (dpNgay.SelectedDate == null)
                return;

            int maRap = 0;
            int maCa = 0;

            if (cbRap.SelectedValue != null)
                maRap = Convert.ToInt32(cbRap.SelectedValue);

            if (cbCa.SelectedValue != null)
                maCa = Convert.ToInt32(cbCa.SelectedValue);

            dsPhanCa = phanCaBUS.GetAll(
                dpNgay.SelectedDate.Value,
                maRap,
                maCa
            );

            string keyword = txtSearch.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(keyword))
            {
                dsPhanCa = dsPhanCa
                    .Where(x => x.HoTen.ToLower().Contains(keyword))
                    .ToList();
            }

            dgPhanCa.ItemsSource = null;
            dgPhanCa.ItemsSource = dsPhanCa;

            UpdateThongKe();
        }

        private void UpdateThongKe()
        {
            cardTongPhanCa.Value = dsPhanCa.Count.ToString();

            cardCaSang.Value = dsPhanCa
                .Count(x => x.TenCa == "Ca sáng")
                .ToString();

            cardCaChieu.Value = dsPhanCa
                .Count(x => x.TenCa == "Ca chiều")
                .ToString();

            cardCaToi.Value = dsPhanCa
                .Count(x => x.TenCa == "Ca tối")
                .ToString();

            lblPageInfo.Text = $"Showing {dsPhanCa.Count} shift assignments";
        }

        // ========================= ADD =========================

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbNhanVien.SelectedValue == null ||
                    cbCa.SelectedValue == null ||
                    dpNgay.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                    return;
                }

                int maNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);
                int maCa = Convert.ToInt32(cbCa.SelectedValue);

                bool result = phanCaBUS.Add(
                    maNhanVien,
                    maCa,
                    dpNgay.SelectedDate.Value
                );

                if (result)
                {
                    MessageBox.Show("Phân ca thành công.");

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ========================= UPDATE =========================

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedMaPhanCa == 0)
                {
                    MessageBox.Show("Vui lòng chọn phân ca.");
                    return;
                }

                int maNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);
                int maCa = Convert.ToInt32(cbCa.SelectedValue);

                bool result = phanCaBUS.Update(
                    selectedMaPhanCa,
                    maNhanVien,
                    maCa,
                    dpNgay.SelectedDate.Value
                );

                if (result)
                {
                    MessageBox.Show("Cập nhật thành công.");

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ========================= DELETE =========================

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedMaPhanCa == 0)
                {
                    MessageBox.Show("Vui lòng chọn phân ca.");
                    return;
                }

                var confirm = MessageBox.Show(
                    "Xóa phân ca này?",
                    "Xác nhận",
                    MessageBoxButton.YesNo
                );

                if (confirm != MessageBoxResult.Yes)
                    return;

                bool result = phanCaBUS.Delete(selectedMaPhanCa);

                if (result)
                {
                    MessageBox.Show("Xóa thành công.");

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ========================= SELECT =========================

        private void dgPhanCa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPhanCa.SelectedItem is not PhanCaDTO pc)
                return;

            selectedMaPhanCa = pc.MaPhanCa;

            cbNhanVien.SelectedValue = pc.MaNhanVien;
            cbCa.SelectedValue = pc.MaCa;
            dpNgay.SelectedDate = pc.Ngay;
        }

        // ========================= FILTER =========================

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            LoadData();
        }

        private void cbCa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            LoadData();
        }

        private void dpNgay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            LoadData();
        }

        // ========================= REFRESH =========================

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            selectedMaPhanCa = 0;

            txtSearch.Clear();

            dpNgay.SelectedDate = DateTime.Now;

            cbRap.SelectedIndex = 0;
            cbCa.SelectedIndex = 0;

            LoadData();
        }
    }
}
