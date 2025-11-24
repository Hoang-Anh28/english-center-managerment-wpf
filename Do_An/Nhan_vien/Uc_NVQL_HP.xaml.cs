using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Do_An.BLL;

namespace Do_An
{
    public partial class NVQL_HP : UserControl
    {
        private readonly HocPhiBLL hocPhiBLL = new HocPhiBLL();
        private readonly GiaoDien_NVQL NVQL;

        public NVQL_HP()
        {
            InitializeComponent();
            LoadHocPhi();
        }

        private void LoadHocPhi()
        {
            try
            {
                DataTable dt = hocPhiBLL.LayTatCaHocPhi();
                dgHocPhi.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi tải dữ liệu học phí:\n" + ex.Message);
            }
        }

        private void BtnThemMoi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hocPhiBLL.ThemHocPhi(
                    txtMaHV.Text.Trim(),
                    txtMaLop.Text.Trim(),
                    Convert.ToDecimal(txtSoTien.Text.Trim()),
                    Convert.ToDecimal(txtDaDong.Text.Trim()),
                    (cbPhuongThuc.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    dpNgayDong.SelectedDate
                );
                MessageBox.Show("✅ Thêm học phí thành công!");
                LoadHocPhi();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi thêm học phí:\n" + ex.Message);
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hocPhiBLL.CapNhatHocPhi(
                    txtMaHP.Text.Trim(),
                    txtMaHV.Text.Trim(),
                    txtMaLop.Text.Trim(),
                    Convert.ToDecimal(txtSoTien.Text.Trim()),
                    Convert.ToDecimal(txtDaDong.Text.Trim()),
                    (cbPhuongThuc.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    dpNgayDong.SelectedDate
                );
                MessageBox.Show("✅ Cập nhật học phí thành công!");
                LoadHocPhi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi cập nhật học phí:\n" + ex.Message);
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hocPhiBLL.XoaHocPhi(txtMaHP.Text.Trim());
                MessageBox.Show("✅ Xóa học phí thành công!");
                LoadHocPhi();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi xóa học phí:\n" + ex.Message);
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            LamMoiForm();
        }

        private void LamMoiForm()
        {
            txtMaHP.Clear();
            txtMaHV.Clear();
            txtMaLop.Clear();
            txtSoTien.Clear();
            txtDaDong.Clear();
            txtConNo.Clear();
            cbPhuongThuc.SelectedIndex = -1;
            dpNgayDong.SelectedDate = null;
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            // Gọi phương thức từ form cha để trở lại menu

        }
    }
}
