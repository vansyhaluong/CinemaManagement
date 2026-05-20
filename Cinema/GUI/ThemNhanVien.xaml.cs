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
            // Kiểm tra dữ liệu chung
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                cbRap.SelectedValue == null)
            {
                CustomMessageBox customMessageBox = new CustomMessageBox(
                    "Vui lòng điền đầy đủ thông tin!",
                    "Thông báo");

                customMessageBox.ShowDialog();
                return;
            }

            

            if (isEdit)
            {
                if (editingNhanVien != null)
                {
                    editingNhanVien.HoTen = txtHoTen.Text.Trim();
                    editingNhanVien.SoDienThoai = txtSoDienThoai.Text.Trim();
                    editingNhanVien.MaRap = (int)cbRap.SelectedValue;
                    editingNhanVien.MaTaiKhoan =int.Parse(txtMaTaiKhoan.Text);
   

                    bool result = bus.suaNhanVien(editingNhanVien);

                    if (result)
                    {
                        new CustomMessageBox(
                            "Cập nhật nhân viên thành công!",
                            "Thông báo").ShowDialog();

                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        new CustomMessageBox(
                            "Cập nhật nhân viên thất bại!",
                            "Thông báo").ShowDialog();
                    }
                }
            }
            else
            {
                cbRap.SelectedIndex = 0;
                NhanVien nv = new NhanVien()
                {
                    HoTen = txtHoTen.Text.Trim(),
                    SoDienThoai = txtSoDienThoai.Text.Trim(),
                    MaRap =(int)cbRap.SelectedValue,
                    MaTaiKhoan = int.Parse(txtMaTaiKhoan.Text)
                }; 

                bool result = bus.themNhanVien(nv);

                if (result)
                {
                    new CustomMessageBox(
                        "Thêm nhân viên thành công!",
                        "Thông báo").ShowDialog();

                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    new CustomMessageBox(
                        "Thêm nhân viên thất bại!",
                        "Thông báo").ShowDialog();
                }
            }
        }
        public void loadData(NhanVien nv)
        {
            txtHoTen.Text = nv.HoTen;
            btnSave.Content = "Cập nhật";
            txtSoDienThoai.Text = nv.SoDienThoai;
            txtMaTaiKhoan.Text = nv.MaTaiKhoan.ToString();
            cbRap.SelectedValue = nv.MaRap;
        }
    }
}
