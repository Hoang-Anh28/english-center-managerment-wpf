using System;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;
using System.Data; // Cần dùng DataTable/DataRow cho chế độ Sửa

namespace Do_An
{
    public partial class AdminQLTK_AddTK : Window
    {
        private readonly TaiKhoanBLL tkBLL = new TaiKhoanBLL();
        private string CurrentTenDN; // Lưu Tên đăng nhập hiện tại (chỉ dùng khi Sửa)
        private bool IsEditing => !string.IsNullOrEmpty(CurrentTenDN);

        // Constructor 1: CHẾ ĐỘ SỬA (nhận TenDN để sửa)
        public AdminQLTK_AddTK(string tenDNCanSua)
        {
            InitializeComponent();
            CurrentTenDN = tenDNCanSua;
            this.Title = "Sửa thông tin tài khoản: " + tenDNCanSua;
            LoadQuyen();
            LoadThongTinTaiKhoan(CurrentTenDN);

            // Không cho sửa Tên đăng nhập khi đang ở chế độ Sửa
            txtTenDN.IsReadOnly = true;
        }

        // Constructor 2: CHẾ ĐỘ THÊM (không tham số)
        public AdminQLTK_AddTK()
        {
            InitializeComponent();
            this.Title = "Thêm tài khoản mới";
            LoadQuyen();
        }

        // Tải danh sách quyền (LoaiNguoiDung) vào ComboBox
        private void LoadQuyen()
        {
            // Tạm thời thêm cứng các vai trò. 
            // Bạn nên lấy từ DB hoặc Enum LoaiNguoiDung
            cboQuyen.Items.Add("Học viên");
            cboQuyen.Items.Add("Giáo viên");
            cboQuyen.Items.Add("Nhân viên");
            cboQuyen.Items.Add("Quản lý");

            cboQuyen.SelectedIndex = 0; // Chọn mặc định là Học viên
        }

        // Tải dữ liệu cho chế độ SỬA
        private void LoadThongTinTaiKhoan(string tenDN)
        {
            // ⚠️ GIẢ ĐỊNH: tkBLL có hàm trả về thông tin chi tiết
            DataTable dt = tkBLL.LayThongTinTaiKhoan(tenDN);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtTenDN.Text = row["TenDN"].ToString();
                txtHoTen.Text = row["HoTen"].ToString();
                // Mật khẩu thường không tải lên vì lý do bảo mật, 
                // hoặc bạn có thể đặt một placeholder và yêu cầu nhập lại khi Sửa
                txtMatKhau.Password = "";

                // Chọn đúng quyền trong ComboBox
                string loaiND = row["LoaiNguoiDung"].ToString();
                cboQuyen.SelectedItem = loaiND;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin tài khoản cần sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        // Xử lý nút LƯU
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            // [1] Lấy dữ liệu từ các controls
            string tenDN = txtTenDN.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string matKhau = txtMatKhau.Password; // Dùng .Password cho PasswordBox
            string loaiQuyen = (cboQuyen.SelectedItem as string) ?? string.Empty;

            // [2] Kiểm tra dữ liệu (Tối thiểu)
            if (string.IsNullOrEmpty(tenDN) || string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(loaiQuyen))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập, Họ tên và chọn Quyền.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mật khẩu chỉ bắt buộc khi THÊM
            if (!IsEditing && string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu cho tài khoản mới.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = false;

            if (IsEditing)
            {
                // [3a] CHẾ ĐỘ SỬA
                // Giả định: Hàm cập nhật của bạn có thể chấp nhận mật khẩu rỗng nếu không muốn thay đổi
                success = tkBLL.CapNhatTaiKhoan(CurrentTenDN, hoTen, matKhau, loaiQuyen);
            }
            else
            {
                // [3b] CHẾ ĐỘ THÊM
                success = tkBLL.ThemTaiKhoan(tenDN, hoTen, matKhau, loaiQuyen);
            }

            // [4] Thông báo và đóng form
            if (success)
            {
                MessageBox.Show($"Đã {(IsEditing ? "cập nhật" : "thêm")} tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // ⚠️ Đóng form để kích hoạt LoadDSTaiKhoan() ở form cha
            }
            else
            {
                MessageBox.Show("Thao tác thất bại. Vui lòng kiểm tra dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}