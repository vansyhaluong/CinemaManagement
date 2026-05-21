using BUS;
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
using Cinema.Models;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for KhenThuong.xaml
    /// </summary>
    public partial class KhenThuong : UserControl
    {
        private KhenThuongBUS khenThuongBUS = new KhenThuongBUS();
        private KyLuatBUS kyLuatBUS = new KyLuatBUS();
        private ThuongKyLuatBUS thuongKyLuatBUS = new ThuongKyLuatBUS();

        public KhenThuong()
        {
            InitializeComponent();
            
            loadData();
        }
        //private void loadNhanVien()
        //{
        //    cbNhanVien.ItemsSource = nvBUS.getNhanVien();
        //}

        //private void loadData()
        //{
        //    dsKhenThuong = ktBUS.GetAll();

        //    dgKhenThuong.ItemsSource = dsKhenThuong;
        //}

        //private void btnThem_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Models.KhenThuong kt = new Models.KhenThuong();

        //        kt.MaNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);

        //        kt.Ngay = DateOnly.FromDateTime(
        //            dpNgay.SelectedDate ?? DateTime.Now);

        //        kt.LyDo = txtLyDo.Text;

        //        kt.SoTienThuong = Convert.ToDecimal(txtTienThuong.Text);

        //        ktBUS.Add(kt);

        //        loadData();

        //        MessageBox.Show("Thêm thành công");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Lỗi thêm khen thưởng");
        //    }
        //}

        //private void btnSua_Click(object sender, RoutedEventArgs e)
        //{
        //    if (maKhenThuongSelected == -1)
        //        return;

        //    try
        //    {
        //        Models.KhenThuong kt = new Models.KhenThuong();

        //        kt.MaKhenThuong = maKhenThuongSelected;

        //        kt.MaNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);

        //        kt.Ngay = DateOnly.FromDateTime(
        //            dpNgay.SelectedDate ?? DateTime.Now);

        //        kt.LyDo = txtLyDo.Text;

        //        kt.SoTienThuong = Convert.ToDecimal(txtTienThuong.Text);

        //        ktBUS.Update(kt);

        //        loadData();

        //        MessageBox.Show("Sửa thành công");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Lỗi sửa");
        //    }
        //}

        //private void btnXoa_Click(object sender, RoutedEventArgs e)
        //{
        //    if (maKhenThuongSelected == -1)
        //        return;

        //    ktBUS.Delete(maKhenThuongSelected);

        //    loadData();

        //    MessageBox.Show("Xóa thành công");
        //}

        //private void dgKhenThuong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (dgKhenThuong.SelectedItem is KhenThuongDTO kt)
        //    {
        //        maKhenThuongSelected = kt.MaKhenThuong;

        //        cbNhanVien.SelectedValue = kt.MaNhanVien;

        //        dpNgay.SelectedDate = kt.Ngay;

        //        txtLyDo.Text = kt.LyDo;

        //        txtTienThuong.Text = kt.SoTienThuong.ToString();
        //    }
        //}
        ////private void btnTinhTuDong_Click(object sender, RoutedEventArgs e)
        ////{
        ////    if (dpTuNgay.SelectedDate == null ||
        ////        dpDenNgay.SelectedDate == null)
        ////    {
        ////        MessageBox.Show("Vui lòng chọn ngày");
        ////        return;
        ////    }

        ////    DateOnly tuNgay =
        ////        DateOnly.FromDateTime(dpTuNgay.SelectedDate.Value);

        ////    DateOnly denNgay =
        ////        DateOnly.FromDateTime(dpDenNgay.SelectedDate.Value);

        ////    tkBUS.TinhThuongKyLuat(tuNgay, denNgay);

        ////    loadData();

        ////    MessageBox.Show("Tính thưởng/phạt thành công");
        ////}
        private void loadData()
        {
            var dsKhenThuong = khenThuongBUS.GetAll();
            var dsKyLuat = kyLuatBUS.GetAll();

            dgKhenThuong.ItemsSource = dsKhenThuong;
            dgKyLuat.ItemsSource = dsKyLuat;

            txtTongThuong.Text =
                dsKhenThuong.Sum(x => x.SoTienThuong).ToString("N0") + "đ";

            txtTongPhat.Text =
                dsKyLuat.Sum(x => x.SoTienPhat).ToString("N0") + "đ";

            txtSoNhanVienThuong.Text =
                dsKhenThuong
                    .Select(x => x.MaNhanVien)
                    .Distinct()
                    .Count()
                    .ToString();
        }

        private void btnTinhTuDong_Click(object sender, RoutedEventArgs e)
        {
            if (dpTuNgay.SelectedDate == null || dpDenNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian cần tính!");
                return;
            }

            DateTime tu = dpTuNgay.SelectedDate.Value;
            DateTime den = dpDenNgay.SelectedDate.Value;

            if ((den - tu).TotalDays > 6)
            {
                MessageBox.Show("Chỉ nên chọn tối đa 7 ngày để tính theo tuần!");
                return;
            }

            DateOnly tuNgay = DateOnly.FromDateTime(tu);
            DateOnly denNgay = DateOnly.FromDateTime(den);

            try
            {
                thuongKyLuatBUS.TinhThuongKyLuat(tuNgay, denNgay);

                loadData();

                MessageBox.Show("Tính thưởng/phạt tuần thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính thưởng/phạt: " + ex.Message);
            }
        }
    }
}
