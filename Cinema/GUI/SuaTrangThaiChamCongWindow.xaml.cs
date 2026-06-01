using DTO;
using System.Collections.Generic;
using System.Windows;

namespace Cinema.GUI
{
    public partial class SuaTrangThaiChamCongWindow : Window
    {
        public string TrangThaiDaChon => cbTrangThai.SelectedItem?.ToString() ?? "";

        public SuaTrangThaiChamCongWindow(ChamCongDTO dto)
        {
            InitializeComponent();

            cbTrangThai.ItemsSource = new List<string>
            {
                "Đúng giờ",
                "Trễ",
                "Vắng",
                "Off",
                "Nghỉ"
            };

            txtNhanVien.Text = dto.HoTen ?? "";
            txtCaLam.Text = dto.ThoiGianCa;
            txtNgay.Text = dto.Ngay.ToString("dd/MM/yyyy");
            txtGioVao.Text = dto.GioVaoText;
            txtGioRa.Text = dto.GioRaText;
            cbTrangThai.SelectedItem = string.IsNullOrWhiteSpace(dto.TrangThai) || dto.TrangThai == "Chưa chấm công"
                ? "Vắng"
                : dto.TrangThai;
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (cbTrangThai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái.");
                return;
            }

            DialogResult = true;
        }
    }
}
