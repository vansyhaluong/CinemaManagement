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
using static Cinema.GUI.CustomMessageBox;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLySuatChieu.xaml
    /// </summary>
    public partial class QuanLySuatChieu : UserControl
    {
        private readonly SuatChieuBUS bus = new SuatChieuBUS();
        private readonly MovieBUS phimBus = new MovieBUS();
        private List<SuatChieu> allSuatChieus = new();
        private readonly int? maRapDangNhap = Session.IsAdmin ? null : Session.MaRap;

        public QuanLySuatChieu()
        {
            InitializeComponent();
            loadComboPhim();
            LoadSuatChieuData();
        }
        public void loadComboPhim()
        {
            var dsPhim = phimBus.getMovies();
            dsPhim.Insert(0, new Phim { MaPhim = 0, TieuDe = "Tất cả" });

            cbPhim.DisplayMemberPath = "TieuDe";
            cbPhim.SelectedValuePath = "MaPhim";
            cbPhim.ItemsSource = dsPhim;
            cbPhim.SelectedIndex = 0;
        }

        private void LoadSuatChieuData()
        {
            allSuatChieus = bus.GetAllSuatChieu(maRapDangNhap);
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<SuatChieu> query = allSuatChieus;

            int selectedMovieId = 0;
            if (cbPhim.SelectedValue != null)
            {
                int.TryParse(cbPhim.SelectedValue.ToString(), out selectedMovieId);
            }

            if (selectedMovieId > 0)
            {
                query = query.Where(item => item.MaPhim == selectedMovieId);
            }

            if (dtNgayChieu.SelectedDate.HasValue)
            {
                var selectedDate = dtNgayChieu.SelectedDate.Value.Date;
                query = query.Where(item => item.ThoiGianBatDau.HasValue && item.ThoiGianBatDau.Value.Date == selectedDate);
            }

            var filtered = query
                .OrderBy(item => item.ThoiGianBatDau)
                .ThenBy(item => item.MaPhongNavigation?.TenPhong)
                .ToList();

            dgSuatChieu.ItemsSource = filtered;
            txtTongSuat.Text = filtered.Count.ToString();
            txtBoLocDangDung.Text = BuildFilterSummary(selectedMovieId);
        }

        private string BuildFilterSummary(int selectedMovieId)
        {
            var filters = new List<string>();

            if (dtNgayChieu.SelectedDate.HasValue)
            {
                filters.Add($"Ngày {dtNgayChieu.SelectedDate.Value:dd/MM/yyyy}");
            }

            if (selectedMovieId > 0 && cbPhim.SelectedItem is Phim phim)
            {
                filters.Add(phim.TieuDe ?? "Phim đã chọn");
            }

            return filters.Count == 0 ? "Tất cả suất chiếu" : string.Join("  •  ", filters);
        }

        public void cbPhim_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }
        public void dpNgayChieu_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }
        public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            AddXuatChieu themSuatChieu = new AddXuatChieu();
            if (themSuatChieu.ShowDialog() == true)
            {
                LoadSuatChieuData();
            }
        }
        public void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var suatChieu = btn?.Tag as SuatChieu;
            if (suatChieu == null)
            {
                MessageBox.Show("Không tìm thấy suất chiếu để xóa!");
                return;
            }
            var msg = new CustomMessageBox(
                "Xác nhận xóa",
                "Bạn có chắc chắn muốn xóa suất chiếu này?",
                MessageBoxType.YesNo);
            msg.ShowDialog();
            if (msg.Result)
            {
                bool result = bus.removeSuatChieu(suatChieu.MaSuatChieu);
                if (result)
                {
                    CustomMessageBox cus = new CustomMessageBox("Thông báo", "Xóa suất chiếu thành công!");
                    cus.ShowDialog();
                    LoadSuatChieuData();
                }
                else
                {
                    CustomMessageBox cus = new CustomMessageBox("Thông báo", "Xóa suất chiếu thất bại!");
                    cus.ShowDialog();
                }
            }
        }
        public void btnSua_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var suatChieu = btn?.Tag as SuatChieu;
            if (suatChieu == null)
            {
                MessageBox.Show("Không tìm thấy suất chiếu để sửa!");
                return;
            }
            var item = bus.getSuatChieuById(suatChieu.MaSuatChieu, maRapDangNhap);
            AddXuatChieu edit = new AddXuatChieu(item);
            if (edit.ShowDialog() == true)
            {
                LoadSuatChieuData();
            }
        }

        public void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            dtNgayChieu.SelectedDate = null;
            cbPhim.SelectedIndex = 0;
            LoadSuatChieuData();
        }
    }
}
