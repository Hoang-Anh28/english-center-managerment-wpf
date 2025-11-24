using System;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    /// <summary>
    /// Interaction logic for Admin_Menu.xaml
    /// </summary>
    public partial class Admin_Menu : UserControl
    {
        // Tạo các service BLL
        private readonly HocVienBLL hocVienBLL = new HocVienBLL();
        private readonly PhanCongBLL giaoVienBLL = new PhanCongBLL(); // hoặc GiaoVienBLL nếu bạn có riêng
        private readonly TaiKhoanBLL nhanVienBLL = new TaiKhoanBLL();

        public Admin_Menu()
        {
            InitializeComponent();
            Loaded += TrangChuControl_Loaded;
        }

        private void TrangChuControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                // Lấy số lượng và hiển thị
                int soNhanVien = nhanVienBLL.DemSoLuongNhanVien();
                int soGiaoVien = giaoVienBLL.DemSoLuongGiaoVien();
                int soHocVien = hocVienBLL.DemSoLuong();

                lblTaiKhoan.Text = soNhanVien.ToString();
                lblGiaoVien.Text = soGiaoVien.ToString();
                lblHocVien.Text = soHocVien.ToString();
            }
            catch (Exception ex)
            {
                // Nếu có lỗi kết nối hoặc lỗi SQL
                System.Windows.MessageBox.Show("Lỗi khi tải thống kê: " + ex.Message);
            }
        }
    }
}
