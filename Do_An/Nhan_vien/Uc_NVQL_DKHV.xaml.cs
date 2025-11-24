using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class NVQL_DKHV : UserControl
    {
        private readonly HocVienBLL hocVienBLL = new HocVienBLL();

        public NVQL_DKHV()
        {
            InitializeComponent();
            Loaded += NVQL_DKHV_Loaded;
        }

        private void NVQL_DKHV_Loaded(object sender, RoutedEventArgs e)
        {
            LamMoiForm();
            NapDanhSachKhoaHoc();
        }

        private void NapDanhSachKhoaHoc()
        {
            try
            {
                var dt = hocVienBLL.LayDanhSachKhoaHoc();
                if (dt != null && dt.Rows.Count > 0)
                {
                    cbKhoaHoc.ItemsSource = dt.DefaultView;
                    cbKhoaHoc.DisplayMemberPath = "TenKH";
                    cbKhoaHoc.SelectedValuePath = "MaKH";
                    cbKhoaHoc.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải khóa học:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbKhoaHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // reset UI
                cbMonHoc.ItemsSource = null;
                cbLopHoc.ItemsSource = null;
                btnTaoLopNhanh.Visibility = Visibility.Collapsed;

                if (cbKhoaHoc.SelectedValue == null) return;

                int maKH = Convert.ToInt32(cbKhoaHoc.SelectedValue);

                DataTable dtMon = hocVienBLL.LayMonHocTheoKhoaHoc(maKH);

                if (dtMon == null || dtMon.Rows.Count == 0)
                {
                    MessageBox.Show("Khóa học này chưa có môn học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // đảm bảo chỉ 2 cột MaMH, TenMH (theo DAL trả về)
                // set ItemsSource an toàn
                cbMonHoc.DisplayMemberPath = "TenMH";
                cbMonHoc.SelectedValuePath = "MaMH";
                cbMonHoc.ItemsSource = dtMon.DefaultView;
                cbMonHoc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải môn học:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void cbMonHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cbLopHoc.ItemsSource = null;
                btnTaoLopNhanh.Visibility = Visibility.Collapsed;

                if (cbMonHoc.SelectedValue == null) return;
                int maKH = Convert.ToInt32(cbKhoaHoc.SelectedValue);
                int maMH = Convert.ToInt32(cbMonHoc.SelectedValue);
                var dtLop = hocVienBLL.LayLopHocTheoMonHoc(maKH, maMH);

                if (dtLop != null && dtLop.Rows.Count > 0)
                {
                    if (!dtLop.Columns.Contains("HienThi"))
                        dtLop.Columns.Add("HienThi", typeof(string));

                    foreach (DataRow row in dtLop.Rows)
                    {
                        string ten = row["TenLop"]?.ToString() ?? "";
                        string cho = row.Table.Columns.Contains("ChoTrong") ? row["ChoTrong"].ToString() : "0";
                        row["HienThi"] = $"{ten} (Còn: {cho} chỗ)";
                    }

                    cbLopHoc.ItemsSource = dtLop.DefaultView;
                    cbLopHoc.DisplayMemberPath = "HienThi";
                    cbLopHoc.SelectedValuePath = "MaLop";
                    cbLopHoc.SelectedIndex = -1;
                }
                else
                {
                    btnTaoLopNhanh.Visibility = Visibility.Visible;
                    MessageBox.Show("Không có lớp phù hợp cho môn này. Bạn có thể tạo lớp mới.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lớp học:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDangKy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hoTen = txtHoTen.Text.Trim();
                DateTime? ngaySinh = dpNgaySinh.SelectedDate;
                string gioiTinh = rbNam.IsChecked == true ? "Nam" : rbNu.IsChecked == true ? "Nữ" : null;
                string diaChi = txtDiaChi.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string email = txtEmail.Text.Trim();
                string cccd = txtCCCD?.Text.Trim() ?? "";

                if (string.IsNullOrEmpty(hoTen) || ngaySinh == null || string.IsNullOrEmpty(gioiTinh))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Họ tên, Ngày sinh và Giới tính!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbLopHoc.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Lớp học!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(cccd) && cccd.Length != 12)
                {
                    MessageBox.Show("Số CCCD phải có đúng 12 ký tự (nếu đã nhập).", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maLop = Convert.ToInt32(cbLopHoc.SelectedValue);
                string trinhDo = (cbTrinhDo.SelectedItem as ComboBoxItem)?.Content?.ToString();

                int maHV = hocVienBLL.DangKyHocVienVaXepLop(hoTen, ngaySinh, gioiTinh, diaChi, sdt, email, trinhDo, maLop, cccd);

                string lopHienThi = ((DataRowView)cbLopHoc.SelectedItem)?["HienThi"]?.ToString();

                MessageBox.Show($"Đăng ký thành công!\nMã HV: HV{maHV}\nLớp: {lopHienThi}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng ký: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LamMoiForm()
        {
            txtHoTen.Clear();
            dpNgaySinh.SelectedDate = null;
            txtCCCD.Clear();
            rbNam.IsChecked = rbNu.IsChecked = false;
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            cbKhoaHoc.SelectedIndex = -1;
            cbMonHoc.ItemsSource = null;
            cbLopHoc.ItemsSource = null;
            btnTaoLopNhanh.Visibility = Visibility.Collapsed;
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            LamMoiForm();
        }

        private void btnTaoLopNhanh_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mở form tạo lớp nhanh (chọn môn, giáo viên, thời gian,...).", "Gợi ý", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
