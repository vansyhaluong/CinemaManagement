using QRCoder;
using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Cinema.GUI
{
    public partial class QrThanhToanWindow : Window
    {
        public QrThanhToanWindow(decimal tongTien, string noiDungChuyenKhoan)
        {
            InitializeComponent();
            LoadData(tongTien, noiDungChuyenKhoan);
        }

        private void LoadData(decimal tongTien, string noiDungChuyenKhoan)
        {
            txtTongTien.Text = tongTien.ToString("N0") + " đ";
            txtNganHang.Text = "Vietcombank";
            txtSoTaiKhoan.Text = "1050181821";
            txtChuTaiKhoan.Text = "VAN SY HA LUONG";
            txtNoiDung.Text = noiDungChuyenKhoan;
            imgQr.Source = TaoQrThanhToan(tongTien, noiDungChuyenKhoan);
        }

        private BitmapImage TaoQrThanhToan(decimal tongTien, string noiDungChuyenKhoan)
        {
            var duLieuQr = TaoVietQrPayload(
                bankBin: "970436",
                soTaiKhoan: txtSoTaiKhoan.Text.Trim(),
                soTien: tongTien,
                noiDung: noiDungChuyenKhoan,
                tenTaiKhoan: txtChuTaiKhoan.Text.Trim(),
                thanhPho: "HCM");

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(duLieuQr, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(20);

            var image = new BitmapImage();
            using var stream = new MemoryStream(qrBytes);
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();

            return image;
        }

        private static string TaoVietQrPayload(
            string bankBin,
            string soTaiKhoan,
            decimal soTien,
            string noiDung,
            string tenTaiKhoan,
            string thanhPho)
        {
            var merchantAccountInfo = BuildField("00", "A000000727")
                + BuildField("01", $"0006{bankBin}01{soTaiKhoan.Length:D2}{soTaiKhoan}")
                + BuildField("02", "QRIBFTTA");

            var payload = new StringBuilder();
            payload.Append(BuildField("00", "01"));
            payload.Append(BuildField("01", "12"));
            payload.Append(BuildField("38", merchantAccountInfo));
            payload.Append(BuildField("53", "704"));
            payload.Append(BuildField("54", soTien.ToString("0", CultureInfo.InvariantCulture)));
            payload.Append(BuildField("58", "VN"));
            payload.Append(BuildField("59", tenTaiKhoan));
            payload.Append(BuildField("60", thanhPho));
            payload.Append(BuildField("62", BuildField("08", noiDung)));

            var crcInput = payload + "6304";
            var crc = ComputeCrc16(crcInput);
            return crcInput + crc;
        }

        private static string BuildField(string id, string value)
        {
            return id + value.Length.ToString("D2") + value;
        }

        private static string ComputeCrc16(string input)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc & 0x8000) != 0
                        ? (ushort)((crc << 1) ^ 0x1021)
                        : (ushort)(crc << 1);
                }
            }

            return crc.ToString("X4");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
