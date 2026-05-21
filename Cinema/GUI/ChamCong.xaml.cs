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
using System.Windows.Threading;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for ChamCong.xaml
    /// </summary>
    public partial class ChamCong : UserControl
    {
        private readonly ChamCongBUS chamCongBUS = new();
        private readonly RapBUS rapBUS = new();
        private readonly CaLamBUS caLamBUS = new();

        private List<ChamCongDTO> dsChamCong = new();

        private DispatcherTimer timer = new();

        public ChamCong()
        {
            InitializeComponent();
            Loaded += QuanLyChamCong_Loaded;
        }
        private void QuanLyChamCong_Loaded(object sender, RoutedEventArgs e)
        {
            dpNgay.SelectedDate = DateTime.Now;

            LoadRap();
            LoadCa();
            LoadClock();
            LoadData();
        }

        private void LoadClock()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("HH:mm:ss");
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            };
            timer.Start();
        }

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

        private void LoadData()
        {
            if (dpNgay.SelectedDate == null)
                return;

            int maRap = cbRap.SelectedValue == null ? 0 : Convert.ToInt32(cbRap.SelectedValue);
            int maCa = cbCa.SelectedValue == null ? 0 : Convert.ToInt32(cbCa.SelectedValue);

            dsChamCong = chamCongBUS.GetDanhSachChamCong(
                dpNgay.SelectedDate.Value,
                maRap,
                maCa
            );

            string keyword = txtSearch.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(keyword))
            {
                dsChamCong = dsChamCong
                    .Where(x => x.HoTen.ToLower().Contains(keyword))
                    .ToList();
            }

            icNhanVien.ItemsSource = null;
            icNhanVien.ItemsSource = dsChamCong;

            LoadTimeline();
            UpdateThongKe();
        }

        private void LoadTimeline()
        {
            var logs = dsChamCong
                .Where(x => x.GioVao != null)
                .OrderByDescending(x => x.GioVao)
                .Select(x => new
                {
                    ThoiGian = x.GioVaoText,
                    NoiDung = $"{x.HoTen} check-in ({x.TrangThai})"
                })
                .ToList();

            icLog.ItemsSource = logs;
        }

        private void UpdateThongKe()
        {
            cardActive.Value = dsChamCong
                .Count(x => x.TrangThai == "Đúng giờ")
                .ToString();

            cardLate.Value = dsChamCong
                .Count(x => x.TrangThai == "Trễ")
                .ToString();

            cardAbsent.Value = dsChamCong
                .Count(x => x.TrangThai == "Chưa chấm công")
                .ToString();

            cardTotal.Value = dsChamCong.Count.ToString();

            lblPageInfo.Text =
                $"Showing {dsChamCong.Count} attendance records";
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not Button btn) return;
                if (btn.Tag is not ChamCongDTO dto) return;

                bool result = chamCongBUS.CheckIn(dto);

                if (result)
                {
                    MessageBox.Show("Check-in thành công.");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not Button btn) return;
                if (btn.Tag is not ChamCongDTO dto) return;

                bool result = chamCongBUS.CheckOut(dto);

                if (result)
                {
                    MessageBox.Show("Check-out thành công.");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            LoadData();
        }

        private void btnLoc_Click(object sender, RoutedEventArgs e)
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
    }
}
