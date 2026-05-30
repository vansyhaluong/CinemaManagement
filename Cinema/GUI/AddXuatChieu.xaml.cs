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
using System.Windows.Shapes;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for AddXuatChieu.xaml
    /// </summary>
    public partial class AddXuatChieu : Window
    {
        private readonly MovieBUS movieBUS = new MovieBUS();
        private readonly PhongChieuBUS phongChieuBUS = new PhongChieuBUS();
        private readonly RapBUS rapBUS = new RapBUS();
        private readonly SuatChieuBUS suatChieuBUS = new SuatChieuBUS();
        private readonly int? maRapDangNhap = Session.IsAdmin ? null : Session.MaRap;
        private bool isEdit;
        private SuatChieu? editingSuatChieu;
        public AddXuatChieu(SuatChieu? s=null)
        {
            InitializeComponent();
            loadComboPhim();
            loadAllRap();
            loadGio();
            if (s != null)
            {
                isEdit = true;
                editingSuatChieu = s;
                loadData(s);
            }
            
        }

        private void loadData(SuatChieu s)
        {
            if (!s.ThoiGianBatDau.HasValue || !s.ThoiGianKetThuc.HasValue)
            {
                return;
            }

            cbPhim.SelectedValue = s.MaPhim;
            cbRap.SelectedValue = s.MaPhongNavigation?.MaRap;
            cbPhong.SelectedValue = s.MaPhong;
            dpNgay.SelectedDate = s.ThoiGianBatDau.Value.Date;
            cbGio.SelectedItem = s.ThoiGianBatDau.Value.ToString("HH:mm");
            txtGioKetThuc.Text = s.ThoiGianKetThuc.Value.ToString("HH:mm");
            txtGiaVe.Text = s.GiaVeCoBan.ToString("0");
            btnSave.Content = "Cập nhật";
        }

        public void loadComboPhim()
        {
            var dsPhim = movieBUS.getMovies();
            dsPhim.Insert(0, new Phim { MaPhim = 0, TieuDe = "---Chọn Phim--- " });
            cbPhim.DisplayMemberPath = "TieuDe";
            cbPhim.SelectedValuePath = "MaPhim";
            cbPhim.ItemsSource = dsPhim;
            cbPhim.SelectedIndex = 0;
        }
        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBuildSuatChieuInput(out int maPhim, out int maPhong, out DateTime thoiGianBatDau, out DateTime thoiGianKetThuc, out decimal giaVe))
            {
                return;
            }

            if (isEdit)
            {
                if (editingSuatChieu != null)
                {
                    editingSuatChieu.MaPhim = maPhim;
                    editingSuatChieu.MaPhong = maPhong;
                    editingSuatChieu.ThoiGianBatDau = thoiGianBatDau;
                    editingSuatChieu.ThoiGianKetThuc = thoiGianKetThuc;
                    editingSuatChieu.GiaVeCoBan = giaVe;

                    bool result = suatChieuBUS.updateSuatChieu(editingSuatChieu);

                    if (result)
                    {
                        new CustomMessageBox("Thông báo", "Cập nhật thành công!").ShowDialog();
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        new CustomMessageBox("Thông báo", "Cập nhật thất bại!").ShowDialog();
                    }
                }
            }
            else
            {
                var xuatChieu = new SuatChieu
                {
                    MaPhim = maPhim,
                    MaPhong = maPhong,
                    ThoiGianBatDau = thoiGianBatDau,
                    ThoiGianKetThuc = thoiGianKetThuc,
                    GiaVeCoBan = giaVe
                };

                bool result = suatChieuBUS.addSuatChieu(xuatChieu);

                new CustomMessageBox(
                    "Thông báo",
                    result ? "Thêm thành công!" : "Thêm thất bại!"
                ).ShowDialog();

                if (result)
                {
                    DialogResult = true;
                    Close();
                }
            }

        }
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void loadComBoPhong(int maRap)
        {
            var dsPhong = phongChieuBUS.getPhongChieuByRap(maRap);
            dsPhong.Insert(0, new PhongChieu { MaPhong = 0, TenPhong = "---Chọn Phòng---" });
            cbPhong.DisplayMemberPath = "TenPhong";
            cbPhong.SelectedValuePath = "MaPhong";
            cbPhong.ItemsSource = dsPhong;
            cbPhong.SelectedIndex = 0;
        }
        public void loadComboRap(int maPhim)
        {
            loadAllRap();
        }

        private void loadAllRap()
        {
            var dsRap = maRapDangNhap.HasValue && maRapDangNhap.Value > 0
                ? rapBUS.getAllRap().Where(x => x.MaRap == maRapDangNhap.Value).ToList()
                : rapBUS.getAllRap();

            if (!maRapDangNhap.HasValue)
            {
                dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "---Chọn Rạp---" });
            }

            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.ItemsSource = dsRap;
            if (maRapDangNhap.HasValue && maRapDangNhap.Value > 0)
            {
                cbRap.SelectedValue = maRapDangNhap.Value;
                cbRap.IsEnabled = false;
            }
            else if (!isEdit)
            {
                cbRap.SelectedIndex = 0;
            }
        }
        public void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRap.SelectedValue == null || !int.TryParse(cbRap.SelectedValue.ToString(), out int maRap))
            {
                return;
            }

            if (maRap != 0)
            {
                loadComBoPhong(maRap);
            }
            else
            {
                cbPhong.ItemsSource = null;
            }
        }
        public void cbPhim_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhim.SelectedValue == null || !int.TryParse(cbPhim.SelectedValue.ToString(), out int maPhim))
            {
                return;
            }

            if(maPhim != 0)
            {
                if (!maRapDangNhap.HasValue)
                {
                    loadComboRap(maPhim);
                }
            }
            else
            {
                cbRap.ItemsSource = null;
                cbPhong.ItemsSource = null;
            }

            OnChange(sender, e);
        }
        public void loadGio()
        {
            for(int i=0; i < 24; i++)
            {
                cbGio.Items.Add($"{i:00}:00");
            }
            
            cbGio.SelectedIndex = 0;
        }
        private void OnChange(object sender, EventArgs e)
        {
            if (dpNgay.SelectedDate == null || cbGio.SelectedItem == null)
                return;
            var selectedPhim=cbPhim.SelectedItem as Phim;
            int thoiLuong = selectedPhim?.ThoiLuong ?? 0;

            string? gio = cbGio.SelectedItem.ToString();

            DateTime start = DateTime.Parse(dpNgay.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + gio);

            DateTime end = start.AddMinutes(thoiLuong);

            txtGioKetThuc.Text = end.ToString("HH:mm");
        }

        private bool TryBuildSuatChieuInput(out int maPhim, out int maPhong, out DateTime thoiGianBatDau, out DateTime thoiGianKetThuc, out decimal giaVe)
        {
            maPhim = 0;
            maPhong = 0;
            thoiGianBatDau = default;
            thoiGianKetThuc = default;
            giaVe = 0;

            if (!int.TryParse(cbPhim.SelectedValue?.ToString(), out maPhim) || maPhim <= 0)
            {
                new CustomMessageBox("Thông báo", "Vui lòng chọn phim!").ShowDialog();
                cbPhim.Focus();
                return false;
            }

            if (!int.TryParse(cbRap.SelectedValue?.ToString(), out int maRap) || maRap <= 0)
            {
                new CustomMessageBox("Thông báo", "Vui lòng chọn rạp chiếu!").ShowDialog();
                cbRap.Focus();
                return false;
            }

            if (!int.TryParse(cbPhong.SelectedValue?.ToString(), out maPhong) || maPhong <= 0)
            {
                new CustomMessageBox("Thông báo", "Vui lòng chọn phòng chiếu!").ShowDialog();
                cbPhong.Focus();
                return false;
            }

            if (dpNgay.SelectedDate == null)
            {
                new CustomMessageBox("Thông báo", "Vui lòng chọn ngày chiếu!").ShowDialog();
                dpNgay.Focus();
                return false;
            }

            if (cbGio.SelectedItem == null)
            {
                new CustomMessageBox("Thông báo", "Vui lòng chọn giờ bắt đầu!").ShowDialog();
                cbGio.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGiaVe.Text?.Trim(), out giaVe) || giaVe <= 0)
            {
                new CustomMessageBox("Thông báo", "Giá vé cơ bản phải là số dương!").ShowDialog();
                txtGiaVe.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtGioKetThuc.Text))
            {
                new CustomMessageBox("Thông báo", "Không tính được giờ kết thúc!").ShowDialog();
                return false;
            }

            var ngay = dpNgay.SelectedDate.Value;
            var gio = cbGio.SelectedItem.ToString();

            thoiGianBatDau = DateTime.Parse($"{ngay:yyyy-MM-dd} {gio}");
            thoiGianKetThuc = DateTime.Parse($"{ngay:yyyy-MM-dd} {txtGioKetThuc.Text}");

            if (thoiGianKetThuc <= thoiGianBatDau)
            {
                thoiGianKetThuc = thoiGianKetThuc.AddDays(1);
            }

            return true;
        }
    }
}
