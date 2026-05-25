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
using System.Windows.Shapes;
using Cinema.Models;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for ThemNhanVien.xaml
    /// </summary>
    public partial class ThemNhanVien : Window
    {
        NhanVienBUS bus=new NhanVienBUS();
        private NhanVien? editingNhanVien;
        private bool isEdit;
        RapBUS rapBUS = new RapBUS();
        public ThemNhanVien(NhanVien? nv=null)
        {
            InitializeComponent();
            if(nv!=null)
            {
                isEdit = true;
                editingNhanVien = nv;
                loadData(nv);
            }
            loadCombo();
        }
        public void loadCombo()
        {
            var dsRap = rapBUS.getAllRap();
            
            dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "🏢 Tất cả rạp" });
            
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.ItemsSource = dsRap;
            
        }
        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                cbRap.SelectedValue == null)
            {
                new CustomMessageBox(
                    "Vui lòng điền đầy đủ thông tin!",
                    "Thông báo").ShowDialog();
                return;
            }

            if (isEdit)
            {
                if (editingNhanVien != null)
                {
                    editingNhanVien.HoTen = txtHoTen.Text.Trim();
                    editingNhanVien.SoDienThoai = txtSoDienThoai.Text.Trim();
                    editingNhanVien.MaRap = (int)cbRap.SelectedValue;

                    bool result = bus.suaNhanVien(editingNhanVien);

                    if (result)
                    {
                        new CustomMessageBox(
                            "Cập nhật nhân viên thành công!",
                            "Thông báo").ShowDialog();

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        new CustomMessageBox(
                            "Cập nhật nhân viên thất bại!",
                            "Thông báo").ShowDialog();
                    }
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Password))
            {
                new CustomMessageBox(
                    "Vui lòng nhập tên đăng nhập và mật khẩu!",
                    "Thông báo").ShowDialog();
                return;
            }

            bool themResult = bus.ThemNhanVienKemTaiKhoan(
                txtHoTen.Text.Trim(),
                txtSoDienThoai.Text.Trim(),
                (int)cbRap.SelectedValue,
                txtTenDangNhap.Text.Trim(),
                txtMatKhau.Password.Trim()
            );

            if (themResult)
            {
                new CustomMessageBox(
                    "Thêm nhân viên thành công!",
                    "Thông báo").ShowDialog();

                DialogResult = true;
                Close();
            }
            else
            {
                new CustomMessageBox(
                    "Thêm nhân viên thất bại! Có thể tên đăng nhập đã tồn tại.",
                    "Thông báo").ShowDialog();
            }
        }
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        public void loadData(NhanVien nv)
        {
            isEdit = true;
            editingNhanVien = nv;

            txtHoTen.Text = nv.HoTen;
            txtSoDienThoai.Text = nv.SoDienThoai;
            cbRap.SelectedValue = nv.MaRap;

            btnSave.Content = "Cập nhật";

            txtTenDangNhap.IsEnabled = false;
            txtMatKhau.IsEnabled = false;

            txtTenDangNhap.Text = "Không thay đổi khi sửa";
            txtMatKhau.Password = "";
        }
    }
}
