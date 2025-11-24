using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Do_An.BLL;

namespace Do_An
{
    public partial class QuenMatKhau4 : Window
    {
        private TaiKhoan tk = new TaiKhoan();
        private string _tenDN;
        private bool _isNewVisible = false;
        private bool _isConfirmVisible = false;
        private TaiKhoanBLL.LoaiNguoiDung vaitro;
        public QuenMatKhau4(string tenDN)
        {
            InitializeComponent();
            _tenDN = tenDN;
        }

        // Toggle hiển/ẩn mật khẩu mới
        private void btnToggleNew_Click(object sender, RoutedEventArgs e)
        {
            _isNewVisible = !_isNewVisible;

            if (_isNewVisible)
            {
                txtNewVisible.Text = pwdNew.Password;
                txtNewVisible.Visibility = Visibility.Visible;
                pwdNew.Visibility = Visibility.Collapsed;
            }
            else
            {
                pwdNew.Password = txtNewVisible.Text;
                pwdNew.Visibility = Visibility.Visible;
                txtNewVisible.Visibility = Visibility.Collapsed;
            }
        }

        // Toggle hiển/ẩn mật khẩu xác nhận
        private void btnToggleConfirm_Click(object sender, RoutedEventArgs e)
        {
            _isConfirmVisible = !_isConfirmVisible;

            if (_isConfirmVisible)
            {
                txtConfirmVisible.Text = pwdConfirm.Password;
                txtConfirmVisible.Visibility = Visibility.Visible;
                pwdConfirm.Visibility = Visibility.Collapsed;
            }
            else
            {
                pwdConfirm.Password = txtConfirmVisible.Text;
                pwdConfirm.Visibility = Visibility.Visible;
                txtConfirmVisible.Visibility = Visibility.Collapsed;
            }
        }

        // Nút xác nhận đổi mật khẩu
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = _isNewVisible ? txtNewVisible.Text : pwdNew.Password;
            string confirmPassword = _isConfirmVisible ? txtConfirmVisible.Text : pwdConfirm.Password;

            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra độ dài + ký tự đặc biệt
            if (!IsValidPassword(newPassword))
            {
                MessageBox.Show("Mật khẩu phải có 8–20 ký tự, gồm chữ, số và ký tự đặc biệt.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra trùng khớp
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool success = tk.UpdateMatKhau(_tenDN,newPassword);
            if (success)
            {
                // TODO: Lưu mật khẩu mới vào DB ở đây
                MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Đổi mật khẩu thất bại! Vui lòng thử lại.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm kiểm tra mật khẩu hợp lệ
        private bool IsValidPassword(string password)
        {
            // 8–20 ký tự, ít nhất 1 chữ, 1 số, 1 ký tự đặc biệt
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,20}$";
            return Regex.IsMatch(password, pattern);
        }

        // Quay về login
        private void BackToLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();

            if (this.Owner is QuenMatKhau2 step2 && step2.IsLoaded)
            {
                // Nếu Step2 vẫn còn tồn tại
                step2.Show();
            }
            else
            {
                // Nếu Step2 đã bị đóng (disposed) thì tạo lại
                var newStep2 = new QuenMatKhau2(_tenDN, vaitro);
                newStep2.Show();
            }
        }

    }
}
