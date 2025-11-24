using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An.GiangVien
{
    public partial class UC_NhapDiem : UserControl
    {
        private readonly GiangVienBLL bll;
        private readonly int maGV; // mã giảng viên hiện tại

        // ✅ Constructor có tham số maGV để nhận từ Frm_GiangVienWindow
        public UC_NhapDiem(int maGV)
        {
            InitializeComponent();
            this.maGV = maGV;
            bll = new GiangVienBLL();
        }

        // 📌 Nút "Tải danh sách" – lấy danh sách học viên theo mã lớp
        private void BtnTaiDS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maLop = txtMaLop.Text.Trim();
                if (string.IsNullOrEmpty(maLop))
                {
                    MessageBox.Show("Vui lòng nhập mã lớp để tải danh sách!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DataTable dt = bll.LayHocVienTheoLop(maLop);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy học viên trong lớp này!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                dgHocVien.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 📌 Nút "Cập nhật điểm" – cập nhật điểm cho học viên nhập tay
        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maHV = txtMaHV.Text.Trim();
                if (string.IsNullOrEmpty(maHV))
                {
                    MessageBox.Show("Vui lòng nhập mã học viên!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(txtGK.Text.Trim(), out double diemGK) ||
                    !double.TryParse(txtCK.Text.Trim(), out double diemCK))
                {
                    MessageBox.Show("Điểm phải là số hợp lệ!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


               bool ok = bll.CapNhatDiem(maHV, diemGK, diemCK);

                if (ok)
                    MessageBox.Show("✅ Cập nhật điểm thành công!", "Thông báo");
                else
                    MessageBox.Show("⚠️ Cập nhật thất bại!", "Lỗi");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật điểm: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
