using Do_An.BLL;
using Do_An.GiangVien;
using System;
using System.Windows;
using System.Windows.Input;

namespace Do_An
{
    public partial class GiaoDienDangNhap : Window
    {
        private TaiKhoanBLL taiKhoanBLL = new TaiKhoanBLL();
        private HocVienBLL hocVienBLL = new HocVienBLL();
        public GiaoDienDangNhap()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Xác định vai trò được chọn
            string expectedRole = "";
            if (rbStudent.IsChecked == true)
                expectedRole = "Học viên";
            else if (rbTeacher.IsChecked == true)
                expectedRole = "Giáo viên";
            else if (rbStaff.IsChecked == true)
                expectedRole = "Nhân viên";
            else if (rbAdmin.IsChecked == true)
                expectedRole = "Quản lý";

            if (string.IsNullOrEmpty(expectedRole))
            {
                MessageBox.Show("Vui lòng chọn vai trò đăng nhập!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 🔹 Chuyển vai trò chuỗi sang enum
                TaiKhoanBLL.LoaiNguoiDung roleEnum = expectedRole switch
                {
                    "Học viên" => TaiKhoanBLL.LoaiNguoiDung.HocVien,
                    "Giáo viên" => TaiKhoanBLL.LoaiNguoiDung.GiaoVien,
                    "Nhân viên" => TaiKhoanBLL.LoaiNguoiDung.NhanVien,
                    "Quản lý" => TaiKhoanBLL.LoaiNguoiDung.Admin,
                    _ => TaiKhoanBLL.LoaiNguoiDung.KhongHopLe
                };

                // 🔹 Gọi hàm kiểm tra đăng nhập
                TaiKhoanBLL.LoaiNguoiDung role = taiKhoanBLL.KiemTraDangNhap(username, password, roleEnum);

                switch (role)
                {
                    case TaiKhoanBLL.LoaiNguoiDung.HocVien:
                        int maHV = hocVienBLL.LayMaHVTheoTenDN(username);
                        var hvWindow = new GiaoDien_HocVien(role, maHV);
                        this.Hide();
                        hvWindow.ShowDialog();
                        this.Show();
                        break;
                    case TaiKhoanBLL.LoaiNguoiDung.GiaoVien:
                        // Lấy mã giảng viên theo tài khoản đăng nhập
                        int maGV = new GiangVienBLL().LayMaGVTheoTenDN(username);
                        // Mở giao diện chính của giảng viên
                        var gvWindow = new Frm_GiangVienWindow(role, maGV);
                        this.Hide();
                        gvWindow.ShowDialog();
                        this.Show();
                        break;
                    case TaiKhoanBLL.LoaiNguoiDung.NhanVien:
                        int maNV = new TaiKhoanBLL().LayMaNVTheoTenDN(username);
                        var nvWindow = new GiaoDien_NVQL(role, maNV);
                        this.Hide();
                        nvWindow.ShowDialog();
                        this.Show();
                        break;

                    case TaiKhoanBLL.LoaiNguoiDung.Admin:
                        int maQL = new TaiKhoanBLL().LayMaQLTheoTenDN(username);
                        var adWindow = new GiaoDien_Admin(role, maQL);
                        this.Hide();
                        adWindow.ShowDialog();
                        this.Show();
                        break;

                    case TaiKhoanBLL.LoaiNguoiDung.SaiVaiTro:
                        MessageBox.Show($"Bạn đã chọn sai vai trò. Tài khoản này không phải {expectedRole}!",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case TaiKhoanBLL.LoaiNguoiDung.KhongHopLe:
                    default:
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi hệ thống: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var forgot = new QuenMatKhau1();
            forgot.Owner = this;
            this.Hide();
            forgot.ShowDialog();
            this.Show();
        }
    }
}
