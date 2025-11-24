using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Do_An.BLL;

namespace Do_An
{
    public partial class QuenMatKhau3 : Window
    {
        private string _tenDN;
        private string _otpCode;
        private string _maSo;
        private DispatcherTimer _timer;
        private int _timeLeft = 60;
        private bool _canResend = false;
        private TaiKhoanBLL.LoaiNguoiDung vaitro;
        public QuenMatKhau3(string otpCode, string maSo, string tenDN)
        {
            InitializeComponent();
            _otpCode = otpCode;
            _maSo = maSo;
            _tenDN = tenDN;

            StartCountdown();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = otp1.Text + otp2.Text + otp3.Text + otp4.Text + otp5.Text + otp6.Text;

            if (enteredCode == _otpCode)
            {
                MessageBox.Show("Xác minh thành công! Vui lòng đặt lại mật khẩu.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                QuenMatKhau4 reset = new QuenMatKhau4(_tenDN);
                reset.Owner = this;
                this.Hide();
                reset.ShowDialog();
            }
            else
            {
                MessageBox.Show("Mã xác minh không đúng!",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtResend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_canResend)
            {
                MessageBox.Show("Vui lòng chờ hết thời gian đếm ngược để gửi lại mã.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Tạo mã OTP mới
            Random rand = new Random();
            _otpCode = rand.Next(100000, 999999).ToString();

            // Sau này bạn có thể thêm gửi mail ở đây
            MessageBox.Show($"Đã gửi lại mã mới: {_otpCode}",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Bắt đầu lại đếm ngược
            StartCountdown();
        }

        private void StartCountdown()
        {
            _canResend = false;
            _timeLeft = 60;

            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += Timer_Tick;
            }

            _timer.Start();
            txtResend.Text = $"Gửi lại mã ({_timeLeft})";
            txtResend.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            txtResend.Text = $"Gửi lại mã ({_timeLeft})";

            if (_timeLeft <= 0)
            {
                _timer.Stop();
                txtResend.Text = "Gửi lại mã";
                txtResend.Foreground = System.Windows.Media.Brushes.Blue;
                _canResend = true;
            }
        }

        private void Back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            if (this.Owner != null)
                this.Owner.Show();
        }
    }

}
