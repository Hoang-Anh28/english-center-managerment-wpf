using System;
using System.Windows;
using Do_An.BLL;

namespace Do_An.GiangVien
{
    public partial class Frm_GiangVienWindow : Window
    {
        private readonly int maGV;                     // Lưu mã giảng viên hiện tại
        private readonly GiangVienBLL giangVienBLL;    // BLL xử lý dữ liệu
        private readonly TaiKhoanBLL.LoaiNguoiDung userrole;
        // ✅ Constructor nhận mã giảng viên (được gọi từ GiaoDienDangNhap)
        public Frm_GiangVienWindow(TaiKhoanBLL.LoaiNguoiDung _userrole, int maGV = 0)
        {
            InitializeComponent();
            userrole = _userrole;
            this.maGV = maGV;
            giangVienBLL = new GiangVienBLL();

            // Khi cửa sổ tải xong → mở trang chủ mặc định
            Loaded += Frm_GiangVienWindow_Loaded;
        }

        private void Frm_GiangVienWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gán trang chủ mặc định vào ContentControl
                MainContent.Content = new UC_TrangChu(maGV);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải giao diện giảng viên: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTrangChu_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UC_TrangChu(maGV);
        }

        private void BtnLichDay_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UC_LichDay(maGV);
        }

        private void BtnNhapDiem_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UC_NhapDiem(maGV);
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                 "Xác nhận",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question) == MessageBoxResult.Yes)
                if (this.Owner != null)
                {
                    // Đóng cửa sổ con — luồng ở login sẽ tiếp tục và hiện login lại
                    this.Close();
                }
                else
                {
                    // Dự phòng: tìm instance login đang tồn tại trong Application.Windows
                    var login = Application.Current.Windows.OfType<GiaoDienDangNhap>().FirstOrDefault();
                    if (login != null)
                    {
                        login.Show();
                    }
                    this.Close();
                }
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát chương trình không?",
                "Xác nhận thoát",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }


    }
}
