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
    /// Interaction logic for ThemPhong.xaml
    /// </summary>
    public partial class ThemPhong : Window
    {
        BUS.PhongChieuBUS bus = new BUS.PhongChieuBUS();
        RapBUS bus2 = new RapBUS();
        private PhongChieu? editingPhong;
        private bool isEdit;
        public ThemPhong(PhongChieu? ph=null)
        {
            InitializeComponent();
            getAllRap();
            if (ph != null)
            {
                isEdit = true;
                editingPhong = ph;
                loadData(ph);
            }
        }
        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                editingPhong.TenPhong = txtTenPhong.Text;
                editingPhong.MaRap = (int)cbRap.SelectedValue;
                bool result = bus.updatePhongChieu(editingPhong);
                if (result)
                {
                    CustomMessageBox customMessageBox = new CustomMessageBox("Cập nhật phòng chiếu thành công!", "Thông báo");
                    customMessageBox.ShowDialog();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    CustomMessageBox customMessageBox = new CustomMessageBox("Cập nhật phòng chiếu Thất bại!", "Thông báo");
                    customMessageBox.ShowDialog();
                }
            }
            else
            {
                PhongChieu newPhong = new PhongChieu
                {
                    TenPhong = txtTenPhong.Text,
                    MaRap = (int)cbRap.SelectedValue
                };
                bool result = bus.addPhongChieu(newPhong);
                if (result)
                {
                    MessageBox.Show("Thêm phòng chiếu thành công!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Thêm phòng chiếu thất bại!");
                }
            }
        }
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void getAllRap()
        {
           var listRap = bus2.getAllRap();
            cbRap.ItemsSource = listRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
        }
        public void loadData(PhongChieu ph)
        {
            txtTenPhong.Text = ph.TenPhong;
            cbRap.SelectedValue = ph.MaRap;
            btnSave.Content = "Cập nhật";
        }
    }
}
