using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;
using System.Windows.Data; // Cần thiết cho Converter
using System.Globalization; // Cần thiết cho Converter

namespace Do_An
{
    // =======================================================
    // BỔ SUNG: CONVERTER (Bộ chuyển đổi)
    // (Thêm class này vào để XAML có thể tìm thấy)
    // =======================================================
    public class TrangThaiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Xử lý giá trị (CSDL có thể lưu là 1/0 hoặc True/False)
            if (value is bool trangThai)
            {
                return trangThai ? "✅ Hoạt động" : "🔒 Đã khóa";
            }
            if (value is int intValue)
            {
                return intValue == 1 ? "✅ Hoạt động" : "🔒 Đã khóa";
            }
            if (value is string strValue)
            {
                return strValue == "1" ? "✅ Hoạt động" : "🔒 Đã khóa";
            }
            return "🔒 Đã khóa"; // Mặc định
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // =======================================================
    // CODE-BEHIND CHÍNH CỦA BẠN (Giữ nguyên)
    // =======================================================
    public partial class Admin_QLTK : UserControl
    {
        private readonly TaiKhoanBLL tkBLL = new TaiKhoanBLL();

        public Admin_QLTK()
        {
            InitializeComponent();
            LoadDSTaiKhoan();
        }

        // 🟢 Load danh sách tài khoản
        private void LoadDSTaiKhoan()
        {
            try
            {
                DataTable dt = tkBLL.LayTatCaTaiKhoan();
                dgTaiKhoan.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔍 Tìm kiếm theo Tên đăng nhập hoặc Họ tên
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            // Giả định BLL có hàm TimKiemTaiKhoan
            DataTable dt = tkBLL.TimKiemTaiKhoan(keyword);
            dgTaiKhoan.ItemsSource = dt.DefaultView;
        }

        // ➕ Thêm tài khoản (mở form thêm)
        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            AdminQLTK_AddTK form = new AdminQLTK_AddTK();
            // form.Owner = Window.GetWindow(this); 
            form.ShowDialog();
            LoadDSTaiKhoan();
        }

        // ✏ Sửa tài khoản (mở form sửa)
        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgTaiKhoan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgTaiKhoan.SelectedItem as DataRowView;
            string tenDN = row["TenDN"].ToString();
            AdminQLTK_AddTK form = new AdminQLTK_AddTK(tenDN);
            // form.Owner = Window.GetWindow(this);
            form.ShowDialog();
            LoadDSTaiKhoan();
        }

        // 🗑 Xóa tài khoản
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgTaiKhoan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgTaiKhoan.SelectedItem as DataRowView;
            string tenDN = row["TenDN"].ToString();

            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản '{tenDN}' không?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = tkBLL.XoaTaiKhoan(tenDN);
                if (success)
                {
                    MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDSTaiKhoan();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 🔒 Khóa / Mở khóa tài khoản (FIX LỖI LOGIC)
        private void btnKhoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgTaiKhoan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần khóa/mở!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgTaiKhoan.SelectedItem as DataRowView;
            string tenDN = row["TenDN"].ToString();

            // FIX: Lấy giá trị bool (hoặc int) gốc từ CSDL
            bool trangThaiHienTai = false;
            if (row["TrangThai"] is bool)
            {
                trangThaiHienTai = (bool)row["TrangThai"];
            }
            else if (row["TrangThai"] is int)
            {
                trangThaiHienTai = (int)row["TrangThai"] == 1;
            }
            else if (row["TrangThai"] != null)
            {
                trangThaiHienTai = row["TrangThai"].ToString() == "1";
            }

            string trangThaiMoi; // Giá trị gửi xuống CSDL (1 hoặc 0)
            string thongBao;

            if (trangThaiHienTai) // Nếu đang hoạt động (True/1)
            {
                trangThaiMoi = "0"; // Khóa (0)
                thongBao = "Khóa tài khoản thành công.";
            }
            else // Nếu đang bị khóa (False/0)
            {
                trangThaiMoi = "1"; // Mở khóa (1)
                thongBao = "Mở khóa tài khoản thành công.";
            }

            // (Giả định BLL/DAL có hàm CapNhatTrangThai)
            bool success = tkBLL.CapNhatTrangThai(tenDN, trangThaiMoi);

            if (success)
            {
                MessageBox.Show(thongBao, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDSTaiKhoan();
            }
            else
            {
                MessageBox.Show("Cập nhật trạng thái thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}