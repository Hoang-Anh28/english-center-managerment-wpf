using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Do_An.BLL;

namespace Do_An
{
    public partial class QuenMatKhau2 : Window
    {
        private string _maSo;
        private string _otpCode;
        private string _tenDN;
        private TaiKhoanBLL.LoaiNguoiDung _vaiTro;
        private TaiKhoanBLL taiKhoanBLL = new TaiKhoanBLL();


        public QuenMatKhau2(string tenDN,TaiKhoanBLL.LoaiNguoiDung vaitro )
        {
            InitializeComponent();
            _tenDN = tenDN;
            _vaiTro = vaitro;
        }
        private void btnSendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // BỔ SUNG: GỌI BLL KIỂM TRA EMAIL TRƯỚC KHI TẠO OTP
            bool isEmailValid = taiKhoanBLL.KiemTraEmail(_tenDN, _vaiTro, email);

            if (!isEmailValid)
            {
                MessageBox.Show("Email không khớp với Tên đăng nhập và Vai trò đã chọn. Vui lòng kiểm tra lại.",
                                "Lỗi Xác Minh", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Sinh mã OTP ngẫu nhiên 6 số
            Random rnd = new Random();
            _otpCode = rnd.Next(100000, 999999).ToString();
            MessageBox.Show($"Mã OTP (test): {_otpCode}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);


            // Hiển thị mã OTP ra label (thay cho việc gửi mail)
            lblOtpInfo.Text = $"Mã xác minh của bạn: {_otpCode} (demo)";

            // Mở Step3
            QuenMatKhau3 step3 = new QuenMatKhau3(_otpCode, _maSo, _tenDN);
            step3.Owner = this;
            this.Hide();
            step3.ShowDialog();
            this.Show();
        }

        private void Back_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
            if (this.Owner != null)
                this.Owner.Show();
        }
    }
}
