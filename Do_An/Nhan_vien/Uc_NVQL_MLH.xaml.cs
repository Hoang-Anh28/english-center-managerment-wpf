using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class NVQL_MLH : UserControl
    {
        private readonly LopHocBLL lopHocBLL = new LopHocBLL();

        public NVQL_MLH()
        {
            InitializeComponent();
            if (cbKhoaHoc.Items.Count == 0 && cbMonHoc.Items.Count == 0)
                LoadComboBoxData();
        }

        /// <summary>
        /// Load dữ liệu vào các ComboBox khi khởi tạo
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // --- Khóa học ---
                cbKhoaHoc.ItemsSource = lopHocBLL.LayDanhSachKhoaHoc().DefaultView;
                cbKhoaHoc.DisplayMemberPath = "TenKH";
                cbKhoaHoc.SelectedValuePath = "MaKH";
                cbKhoaHoc.SelectedIndex = -1;

                // --- Trạng thái lớp ---
                cbTrangThai.SelectedIndex = 0; // Mặc định "Chờ khai giảng"

                // --- Trình độ lớp ---
                cbTrinhDoLop.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khởi tạo: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Khi chọn Khóa học, tự động lọc môn học
        /// </summary>
        private void cbKhoaHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbKhoaHoc.SelectedValue == null)
            {
                cbMonHoc.ItemsSource = null;
                return;
            }

            try
            {
                int maKhoaHoc = Convert.ToInt32(cbKhoaHoc.SelectedValue);
                DataTable dtMonHoc = lopHocBLL.LayMonHocTheoKhoaHoc(maKhoaHoc);

                // Chỉ hiện tên môn học
                cbMonHoc.ItemsSource = dtMonHoc.DefaultView;
                cbMonHoc.DisplayMemberPath = "TenMH";
                cbMonHoc.SelectedValuePath = "TenMH"; // để SelectedValue có giá trị là tên môn
                cbMonHoc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc môn học theo khóa: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Khi nhấn Tạo lớp
        /// </summary>
        private void btnTaoLop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tenLop = txtTenLop.Text.Trim();
                string trinhDo = cbTrinhDoLop.SelectedItem is ComboBoxItem ctd ? ctd.Content.ToString() : cbTrinhDoLop.Text;
                string phong = txtPhong.Text.Trim();
                string thoiGian = txtThoiGian.Text.Trim();
                int siSoToiDa;

                if (!int.TryParse(txtSiSoToiDa.Text.Trim(), out siSoToiDa) || siSoToiDa <= 0)
                    siSoToiDa = 20;

                string trangThai = cbTrangThai.SelectedItem is ComboBoxItem cti ? cti.Content.ToString() : cbTrangThai.Text;

                if (cbKhoaHoc.SelectedValue == null || string.IsNullOrEmpty(cbMonHoc.Text))
                {
                    MessageBox.Show("Vui lòng chọn Khóa học và Môn học.",
                                    "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maKH = Convert.ToInt32(cbKhoaHoc.SelectedValue);
                string tenMH = cbMonHoc.Text;

                // Lấy mã môn học thật từ tên môn học
                int maMH = lopHocBLL.LayMaMonHocTheoTen(maKH, tenMH);
                if (maMH == 0)
                {
                    MessageBox.Show("Không tìm thấy mã môn học tương ứng!",
                                    "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool kq = lopHocBLL.ThemLopHoc(tenLop, trinhDo, phong, thoiGian, siSoToiDa, trangThai, maMH, maKH);

                if (kq)
                {
                    MessageBox.Show("Tạo lớp học thành công!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDanhSachLop();
                    btnLamMoi_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Tạo lớp học thất bại!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo lớp: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load danh sách lớp học vào DataGrid (tự tìm control phù hợp)
        /// </summary>
        private void LoadDanhSachLop()
        {
            DataTable dt = lopHocBLL.LayDanhSachLopHoc();

            var dg1 = this.FindName("dgLopHoc") as DataGrid;
            var dg2 = this.FindName("dgDanhSachLop") as DataGrid;
            var dg3 = this.FindName("dgDanhSach") as DataGrid;

            if (dg1 != null) dg1.ItemsSource = dt.DefaultView;
            else if (dg2 != null) dg2.ItemsSource = dt.DefaultView;
            else if (dg3 != null) dg3.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// Làm mới form
        /// </summary>
        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            txtTenLop.Clear();
            txtPhong.Clear();
            txtThoiGian.Text = "";
            txtSiSoToiDa.Text = "20";
            txtMaLop.Clear();

            cbKhoaHoc.SelectedIndex = -1;
            cbMonHoc.ItemsSource = null;
            cbTrinhDoLop.SelectedIndex = -1;
            cbTrangThai.SelectedIndex = 0;
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void cbMonHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
