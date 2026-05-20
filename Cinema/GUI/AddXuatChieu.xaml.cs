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
using System.Windows.Shapes;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for AddXuatChieu.xaml
    /// </summary>
    public partial class AddXuatChieu : Window
    {
        MovieBUS movieBUS = new MovieBUS();
        PhongChieuBUS phongChieuBUS = new PhongChieuBUS();
        RapBUS rapBUS = new RapBUS();
        SuatChieuBUS suatChieuBUS = new SuatChieuBUS();
        private bool isEdit;
        private SuatChieu? editingSuatChieu;
        public AddXuatChieu(SuatChieu? s=null)
        {
            InitializeComponent();
            loadComboPhim();
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
            cbPhim.SelectedValue = s.MaPhim;
            cbRap.SelectedValue = s.MaPhongNavigation.MaRap;
            cbPhong.SelectedValue = s.MaPhong;
            dpNgay.SelectedDate = s.ThoiGianBatDau.Value.Date;
            cbGio.SelectedItem = s.ThoiGianBatDau.Value.ToString("HH:mm");
            txtGioKetThuc.Text = s.ThoiGianKetThuc.Value.ToString("HH:mm");
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
            if (isEdit)
            {
                if (editingSuatChieu != null)
                {
                    if (cbPhim.SelectedValue == null ||
                        cbPhong.SelectedValue == null ||
                        dpNgay.SelectedDate == null ||
                        cbGio.SelectedItem == null)
                    {
                        new CustomMessageBox("Thông báo", "Vui lòng nhập đủ dữ liệu!").ShowDialog();
                        return;
                    }

                    editingSuatChieu.MaPhim = (int)cbPhim.SelectedValue;
                    editingSuatChieu.MaPhong = (int)cbPhong.SelectedValue;

                    var ngay = dpNgay.SelectedDate.Value;
                    var gio = cbGio.SelectedItem.ToString();

                    editingSuatChieu.ThoiGianBatDau =
                        DateTime.Parse($"{ngay:yyyy-MM-dd} {gio}");

                    editingSuatChieu.ThoiGianKetThuc =
                        DateTime.Parse($"{ngay:yyyy-MM-dd} {txtGioKetThuc.Text}");

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
                if (cbPhim.SelectedValue == null ||
                    cbPhong.SelectedValue == null ||
                    dpNgay.SelectedDate == null ||
                    cbGio.SelectedItem == null)
                {
                    new CustomMessageBox("Thông báo", "Vui lòng nhập đủ dữ liệu!").ShowDialog();
                    return;
                }

                var ngay = dpNgay.SelectedDate.Value;
                var gio = cbGio.SelectedItem.ToString();

                var xuatChieu = new SuatChieu
                {
                    MaPhim = (int)cbPhim.SelectedValue,
                    MaPhong = (int)cbPhong.SelectedValue,
                    ThoiGianBatDau = DateTime.Parse($"{ngay:yyyy-MM-dd} {gio}"),
                    ThoiGianKetThuc = DateTime.Parse($"{ngay:yyyy-MM-dd} {txtGioKetThuc.Text}")
                };

                bool result = suatChieuBUS.addSuatChieu(xuatChieu);

                new CustomMessageBox(
                    "Thông báo",
                    result ? "Thêm thành công!" : "Thêm thất bại!"
                ).ShowDialog();
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
            var dsRap = rapBUS.getRapByPhim(maPhim);
            dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "---Chọn Rạp---" });
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.ItemsSource = dsRap;
            cbRap.SelectedIndex = 0;
        }
        public void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maRap = Convert.ToInt32(cbRap.SelectedValue);
            if (maRap != 0)
            {
                loadComBoPhong(maRap);
            }
        }
        public void cbPhim_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maPhim = Convert.ToInt32(cbPhim.SelectedValue);
           if(maPhim != 0)
            {
                loadComboRap(maPhim);
            }
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
    }
}
