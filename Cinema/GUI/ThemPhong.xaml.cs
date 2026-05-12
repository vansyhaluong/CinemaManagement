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
        public ThemPhong()
        {
            InitializeComponent();
            getAllRap();
        }
        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbRap.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn rạp!");
                return;
            }

            var item = new PhongChieu
            {
                TenPhong = txtTenPhong.Text,
                MaRap = (int)cbRap.SelectedValue
            };
            if (bus.addPhongChieu(item)){
                MessageBox.Show("Thêm phòng chiếu thành công!");
                this.DialogResult = true;
                this.Close();
            }
        }
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void getAllRap()
        {
           var listRap = bus2.getRap();
            cbRap.ItemsSource = listRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
        }
    }
}
