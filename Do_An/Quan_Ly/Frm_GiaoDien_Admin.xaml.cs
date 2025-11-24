using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Do_An.BLL;

namespace Do_An
{
    public partial class GiaoDien_Admin : Window
    {
        private TaiKhoanBLL.LoaiNguoiDung userRole;
        private int maNVQLDangNhap;
        public GiaoDien_Admin(TaiKhoanBLL.LoaiNguoiDung _userRole, int _maNVQL =  0)
        {
            InitializeComponent();
            maNVQLDangNhap = _maNVQL;
            userRole = _userRole;

            // Gán sự kiện 1 lần duy nhất
             this.Loaded += GiaoDien_Admin_Loaded;
        }
        private void GiaoDien_Admin_Loaded(object sender, RoutedEventArgs e)
        {
            // Lúc này UI đã hoàn tất, mọi control (txtTitle, MainContent) đều != null
            MenuList.SelectedIndex = 0;
            LoadView("Trang chủ", new Admin_Menu());
        }
        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(MenuList.SelectedItem is ListBoxItem item)) return;

            // Lấy nguyên nội dung (có emoji cũng được)
            string selectedContent = item.Content.ToString()?.Trim() ?? "";

            // Bỏ emoji đầu chuỗi nếu có (bằng Regex nhẹ hơn)
            string selected = Regex.Replace(selectedContent, @"^[^\p{L}]+", "").Trim();

            string title = "";
            UserControl newControl = null;

            // Match nội dung tiếng Việt đầy đủ
            if (selected.Contains("Trang chủ"))
            {
                title = "Trang Chủ";
                newControl = new Admin_Menu();
            }
            else if (selected.Contains("Quản lý tài khoản"))
            {
                title = "Quản lý Tài khoản";
                newControl = new Admin_QLTK();
            }
            else if (selected.Contains("Quản lý lớp học"))
            {
                title = "Quản lý Lớp học";
                newControl = new Admin_QLPH();
            }
            else if (selected.Contains("Quản lý phân quyền"))
            {
               //title = "Quản lý Phân quyền";
               //newControl = new Admin_QLPQ();
            }
            else if (selected.Contains("Bảo mật hệ thống"))
            {
                title = "Bảo mật hệ thống";
                
            }
            else if (selected.Contains("Xem báo cáo"))
            {
                title = "Xem Báo cáo";
                newControl = new NVQL_BC();
            }

            LoadView(title, newControl);
        }

        private void LoadView(string title, object content)
        {
            try
            {
                // Nếu txtTitle hoặc MainContent chưa sẵn sàng -> trì hoãn 1 tick UI để gọi lại
                if (txtTitle == null || MainContent == null)
                {
                    Dispatcher.BeginInvoke(new Action(() => LoadView(title, content)),
                                           DispatcherPriority.Loaded);
                    return;
                }

                // Gán title an toàn
                txtTitle.Text = string.IsNullOrWhiteSpace(title) ? "StarEnglish" : title;

                // Nếu content null -> hiển thị placeholder nhẹ
                if (content == null)
                {
                    MainContent.Content = new TextBlock
                    {
                        Text = "Chức năng đang được phát triển...",
                        FontSize = 16,
                        FontStyle = FontStyles.Italic,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    return;
                }

                // Gán UserControl hoặc TextBlock, còn kiểu khác thì hiển thị mô tả
                if (content is UserControl uc)
                    MainContent.Content = uc;
                else if (content is TextBlock tb)
                    MainContent.Content = tb;
                else
                    MainContent.Content = new TextBlock
                    {
                        Text = content.ToString(),
                        FontSize = 14,
                        Margin = new Thickness(12)
                    };
            }
            catch (Exception ex)
            {
                // Nếu có exception lạ, hiển thị thông tin nhỏ gọn (dùng Debug cho dev)
                System.Diagnostics.Debug.WriteLine($"LoadView exception: {ex}");
                // Không ném lỗi ra UI production
            }
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
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
    }
}
