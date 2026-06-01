using BUS;
using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Cinema.GUI
{
    public partial class SuaThuongPhatWindow : Window
    {
        private readonly List<NhanVien> dsNhanVien;

        public string Loai { get; }
        public int MaBanGhi { get; }

        public int MaNhanVien => Convert.ToInt32(cbNhanVien.SelectedValue);
        public DateTime Ngay => dpNgay.SelectedDate ?? DateTime.Today;
        public string LyDo => txtLyDo.Text.Trim();
        public decimal SoTien { get; private set; }

        public SuaThuongPhatWindow(
            string loai,
            int maBanGhi,
            int maNhanVien,
            DateTime? ngay,
            string lyDo,
            decimal soTien)
        {
            InitializeComponent();

            Loai = loai;
            MaBanGhi = maBanGhi;
            dsNhanVien = new NhanVienBUS().getNhanVien();

            cbNhanVien.ItemsSource = dsNhanVien;

            txtLoai.Text = loai;
            cbNhanVien.SelectedValue = maNhanVien;
            dpNgay.SelectedDate = ngay ?? DateTime.Today;
            txtLyDo.Text = lyDo ?? string.Empty;
            txtSoTien.Text = soTien.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (cbNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.");
                return;
            }

            if (dpNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLyDo.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do.");
                return;
            }

            if (!decimal.TryParse(txtSoTien.Text.Trim(), out var soTien) || soTien < 0)
            {
                MessageBox.Show("Số tiền không hợp lệ.");
                return;
            }

            SoTien = soTien;
            DialogResult = true;
        }
    }
}
