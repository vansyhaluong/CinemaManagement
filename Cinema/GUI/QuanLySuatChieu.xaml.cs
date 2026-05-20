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
using static Cinema.GUI.CustomMessageBox;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLySuatChieu.xaml
    /// </summary>
    public partial class QuanLySuatChieu : UserControl
    {
        SuatChieuBUS bus = new SuatChieuBUS();
        MovieBUS phimBus = new MovieBUS();
        public QuanLySuatChieu()
        {
            InitializeComponent();
            dgSuatChieu.ItemsSource = bus.GetAllSuatChieu();
            loadComboPhim();
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
        public void cbPhim_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maPhim = Convert.ToInt32(cbPhim.SelectedValue);
            if (maPhim == 0)
            {
                dgSuatChieu.ItemsSource = bus.GetAllSuatChieu();
            }
            else
            {
                dgSuatChieu.ItemsSource = bus.getSuatChieuByPhim(maPhim);
            }
        }
        public void dpNgayChieu_SelectedDateChanged(object sender, EventArgs e)
        {
            if (dtNgayChieu.SelectedDate != null)
            {
                DateTime day = dtNgayChieu.SelectedDate.Value;
                dgSuatChieu.ItemsSource = bus.getSuatChieuByNgay(day);
            }
        }
        public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            AddXuatChieu themSuatChieu = new AddXuatChieu();
            themSuatChieu.ShowDialog();
            dgSuatChieu.ItemsSource = bus.GetAllSuatChieu();
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
                    dgSuatChieu.ItemsSource = bus.GetAllSuatChieu();
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
            var item = bus.getSuatChieuById(suatChieu.MaSuatChieu);
            AddXuatChieu edit = new AddXuatChieu(item);
            if (edit.ShowDialog() == true)
            {
                dgSuatChieu.ItemsSource = bus.GetAllSuatChieu();
            }
        }
    }
}
