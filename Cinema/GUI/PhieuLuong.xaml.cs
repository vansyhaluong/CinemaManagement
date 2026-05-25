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
    /// Interaction logic for PhieuLuong.xaml
    /// </summary>
    public partial class PhieuLuong : Window
    {
        private BangLuongDTO bangLuong;
        public PhieuLuong(BangLuongDTO dto)
        {
            InitializeComponent();
            if (dto == null)
            {
                MessageBox.Show("Không có dữ liệu bảng lương để in.");
                Close();
                return;
            }

            bangLuong = dto;
            LoadData();
        }
        private void LoadData()
        {
            decimal tienLuongGio = (decimal)bangLuong.TongGioLam * bangLuong.LuongCoBan;

            txtThoiGian.Text = $"Từ ngày {bangLuong.TuNgay:dd/MM/yyyy} đến {bangLuong.DenNgay:dd/MM/yyyy}";
            txtHoTen.Text = bangLuong.HoTen;
            txtNgayCong.Text = bangLuong.TongNgayCong.ToString();
            txtGioLam.Text = bangLuong.TongGioLam.ToString("0.##") + " giờ";
            txtLuongCoBan.Text = bangLuong.LuongCoBan.ToString("N0") + " đ";
            txtTienLuongGio.Text = tienLuongGio.ToString("N0") + " đ";
            string lyDoThuong = "";

            if (bangLuong.Thuong > 0)
            {
                if (bangLuong.TongNgayCong >= 6)
                {
                    lyDoThuong = "Đủ công";
                }
                else
                {
                    lyDoThuong = "Thưởng chuyên cần";
                }
            }

            txtThuong.Text = bangLuong.Thuong.ToString("N0") + " đ";

            if (!string.IsNullOrEmpty(lyDoThuong))
            {
                txtThuong.Text += $" ({lyDoThuong})";
            }
            string lyDoPhat = "";

            if (bangLuong.SoLanTre > 0)
            {
                lyDoPhat += $"Đi trễ {bangLuong.SoLanTre} lần";
            }

            if (bangLuong.SoNgayNghi > 0)
            {
                if (!string.IsNullOrEmpty(lyDoPhat))
                    lyDoPhat += ", ";

                lyDoPhat += $"Nghỉ {bangLuong.SoNgayNghi} ngày";
            }

            txtPhat.Text = bangLuong.Phat.ToString("N0") + " đ";

            if (!string.IsNullOrEmpty(lyDoPhat))
            {
                txtPhat.Text += $" ({lyDoPhat})";
            }
            txtTongLuong.Text = bangLuong.TongLuong.ToString("N0") + " đ";
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(printArea, "Phiếu lương nhân viên");
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
