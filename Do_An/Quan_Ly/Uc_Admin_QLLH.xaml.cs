using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class Admin_QLPH : UserControl
    {
        private readonly LopHocBLL lopHocBLL = new LopHocBLL();
        private DataView dvLopHoc;

        public Admin_QLPH()
        {
            InitializeComponent();
            LoadDanhSachLopHoc();
        }

        private void LoadDanhSachLopHoc()
        {
            try
            {
                DataTable dt = lopHocBLL.LayDanhSachLopHoc_Admin();
                dvLopHoc = dt.DefaultView;
                dgLopHoc.ItemsSource = dvLopHoc;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp học: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dvLopHoc == null) return;

            string filter = txtSearch.Text.Trim().Replace("'", "''");
            dvLopHoc.RowFilter = string.IsNullOrEmpty(filter)
                ? ""
                : $"TenLop LIKE '%{filter}%' OR GiaoVien LIKE '%{filter}%'";
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng thêm lớp học đang được phát triển.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgLopHoc.SelectedItem is DataRowView row)
            {
                string tenLop = row["TenLop"].ToString();
                MessageBox.Show($"Chức năng sửa thông tin lớp '{tenLop}' đang được phát triển.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn lớp học cần sửa.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgLopHoc.SelectedItem is not DataRowView row)
            {
                MessageBox.Show("Vui lòng chọn lớp học cần xóa.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int maLop = Convert.ToInt32(row["MaLop"]);
            string tenLop = row["TenLop"].ToString();

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa lớp '{tenLop}' không?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int kq = lopHocBLL.XoaLopHoc_Admin(maLop);
                    if (kq > 0)
                    {
                        MessageBox.Show("Xóa lớp học thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDanhSachLopHoc();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa lớp học này.",
                            "Thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa lớp học: " + ex.Message,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
