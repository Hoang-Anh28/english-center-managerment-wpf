using System;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class GiaoDien_HocVien : Window
    {
        private TaiKhoanBLL.LoaiNguoiDung userrole;
        private int maHVDangNhap;

        public GiaoDien_HocVien(TaiKhoanBLL.LoaiNguoiDung _userrole, int maHV = 0)
        {
            InitializeComponent();
            userrole = _userrole;
            maHVDangNhap = maHV;

            // Gắn sự kiện cho menu bên trái
            MenuList.SelectionChanged += MenuList_SelectionChanged;

            // Gắn sự kiện cho các nút dashboard
            GanSuKienChoButton();
        }

        // ================== GẮN SỰ KIỆN CHO BUTTON ==================
        private void GanSuKienChoButton()
        {
            foreach (var element in FindVisualChildren<Button>(this))
            {
                string content = element.Content?.ToString() ?? "";
                if (string.IsNullOrEmpty(content)) continue;
                if (content.Contains("Đăng xuất")) continue; // bỏ qua nút đăng xuất

                element.Click += (s, e) =>
                {
                    XuLyChucNang(content);
                };
            }
        }

        // ================== XỬ LÝ MENU BÊN TRÁI ==================
        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                string muc = item.Content.ToString();
                XuLyChucNang(muc);
                MenuList.SelectedIndex = -1;
            }
        }

        // ================== XỬ LÝ CHỨC NĂNG ==================
        private void XuLyChucNang(string muc)
        {
            DashboardPanel.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;

            if (muc.Contains("Trang chủ"))
            {
                DashboardPanel.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
            }
            else if (muc.Contains("Hồ sơ học viên"))
            {
                MainContent.Content = new HoSoHocVien(maHVDangNhap);
            }
            else if (muc.Contains("Học vụ"))
            {
                MainContent.Content = new HocVu(maHVDangNhap);
            }
            else if (muc.Contains("Học phí"))
            {
                MainContent.Content = new HocPhi(maHVDangNhap);
            }
            else if (muc.Contains("Khảo sát"))
            {
                MainContent.Content = new KhaoSat();
            }
            else
            {
                DashboardPanel.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
            }
        }

        // ================== NÚT ĐĂNG XUẤT AN TOÀN ==================
        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                "Xác nhận",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // 1. Ẩn window hiện tại
                this.Hide();

                // 2. Tạo window login
                var login = new GiaoDienDangNhap();

                // 3. Hiển thị login ở dạng modal (chặn tương tác window cũ)
                bool? result = login.ShowDialog();

                // 4. Sau khi login đóng, nếu người dùng login thành công, mở lại GiaoDien_HocVien mới
                if (result == true)
                {
                    var newHocVien = new GiaoDien_HocVien(userrole, maHVDangNhap);
                    Application.Current.MainWindow = newHocVien;
                    newHocVien.Show();
                }

                // 5. Đóng window cũ (giờ đã an toàn vì login đã modal)
                this.Close();
            }
        }


        // ================== HỖ TRỢ TÌM BUTTON ==================
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}
