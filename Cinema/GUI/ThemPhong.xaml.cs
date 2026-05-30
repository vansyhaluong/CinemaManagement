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
    /// Interaction logic for ThemPhong.xaml
    /// </summary>
    public partial class ThemPhong : Window
    {
        BUS.PhongChieuBUS bus = new BUS.PhongChieuBUS();
        RapBUS bus2 = new RapBUS();
        private readonly int? maRapDangNhap = Session.IsAdmin ? null : Session.MaRap;
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
            if (!ValidatePhongInput(out string tenPhong, out int maRap))
            {
                return;
            }

            if (isEdit)
            {
                editingPhong.TenPhong = tenPhong;
                editingPhong.MaRap = maRap;
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
                    TenPhong = tenPhong,
                    MaRap = maRap
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
           var listRap = maRapDangNhap.HasValue && maRapDangNhap.Value > 0
                ? bus2.getAllRap().Where(x => x.MaRap == maRapDangNhap.Value).ToList()
                : bus2.getAllRap();
            cbRap.ItemsSource = listRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.IsEnabled = !maRapDangNhap.HasValue;
            if (maRapDangNhap.HasValue && maRapDangNhap.Value > 0)
            {
                cbRap.SelectedValue = maRapDangNhap.Value;
            }
            else if (cbRap.Items.Count > 0)
            {
                cbRap.SelectedIndex = 0;
            }
        }
        public void loadData(PhongChieu ph)
        {
            txtTenPhong.Text = ph.TenPhong;
            cbRap.SelectedValue = ph.MaRap;
            btnSave.Content = "Cập nhật";
        }

        private bool ValidatePhongInput(out string tenPhong, out int maRap)
        {
            tenPhong = txtTenPhong.Text?.Trim() ?? string.Empty;
            maRap = 0;

            if (string.IsNullOrWhiteSpace(tenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên phòng chiếu!");
                txtTenPhong.Focus();
                return false;
            }

            if (cbRap.SelectedValue == null || !int.TryParse(cbRap.SelectedValue.ToString(), out maRap))
            {
                MessageBox.Show("Vui lòng chọn rạp!");
                cbRap.Focus();
                return false;
            }

            return true;
        }
    }
}
