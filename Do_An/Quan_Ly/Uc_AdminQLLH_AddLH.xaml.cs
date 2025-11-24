using System;
using System.Windows;
using Do_An.BLL;
using System.Data;
using System.Linq;
using System.Collections.Generic; // Cần thiết
using System.Windows.Controls; // Cần thiết cho ComboBoxItem

namespace Do_An
{
    public partial class AdminQLLH_AddLH : Window
    {
        private readonly LopHocBLL lopHocBLL = new LopHocBLL();

        public AdminQLLH_AddLH()
        {
            InitializeComponent();
            LoadComboData();
        }

        private void LoadComboData()
        {
            try
            {
                // Tải danh sách Giáo viên
                DataTable dsGV = lopHocBLL.LayDanhSachGiaoVien();
                cbGiaoVien.ItemsSource = dsGV.DefaultView;
                cbGiaoVien.SelectedIndex = -1;

                // Tải danh sách Môn học (Giả định Môn học không phụ thuộc Khóa học trong form này)
                DataTable dsMH = lopHocBLL.LayDanhSachMonHoc();
                cbMonHoc.ItemsSource = dsMH.DefaultView;
                cbMonHoc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách (GV/MH): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // --- 1. Thu thập dữ liệu từ XAML mới ---
                string tenLop = txtTenLop.Text.Trim();
                string trinhDo = (cbTrinhDo.SelectedItem as ComboBoxItem)?.Content.ToString();

                // Các trường ẩn (dùng giá trị mặc định)
                string phong = "P.TBA"; // "TBA" = To Be Announced (Chưa sắp xếp)
                string thoiGian = "Chưa xếp lịch";
                string trangThai = "Chờ khai giảng";

                // Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrWhiteSpace(tenLop))
                {
                    MessageBox.Show("Vui lòng nhập tên lớp học.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cbGiaoVien.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Giáo viên.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cbMonHoc.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Môn học.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(trinhDo))
                {
                    MessageBox.Show("Vui lòng chọn Trình độ áp dụng.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtSiSoToiDa.Text.Trim(), out int siSoToiDa) || siSoToiDa <= 0)
                {
                    MessageBox.Show("Sĩ số tối đa phải là số nguyên dương.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maGV = Convert.ToInt32(cbGiaoVien.SelectedValue);
                int maMH = Convert.ToInt32(cbMonHoc.SelectedValue);

                // --- 2. Gọi BLL với 8 tham số (Thứ tự chính xác) ---
                string kq = lopHocBLL.TaoLopHoc(
                    tenLop,     // 1. string tenLop
                    trinhDo,    // 2. string trinhDo
                    phong,      // 3. string phong
                    thoiGian,   // 4. string thoiGian
                    siSoToiDa,  // 5. int siSoToiDa
                    trangThai,  // 6. string trangThai
                    maGV,       // 7. int maGV
                    maMH        // 8. int maMH
                );

                if (kq.Contains("thành công"))
                {
                    MessageBox.Show(kq, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(kq, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}