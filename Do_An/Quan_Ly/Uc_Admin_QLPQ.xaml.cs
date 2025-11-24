using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class Admin_QLPQ : UserControl
    {
        private readonly PhanQuyenBLL pqBLL = new PhanQuyenBLL();
        private DataTable dtPhanQuyen;

        public Admin_QLPQ()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dtPhanQuyen = pqBLL.GetAll();

                if (dtPhanQuyen == null || dtPhanQuyen.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu tài khoản nào!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgPhanQuyen.ItemsSource = null;
                    return;
                }

                // Kiểm tra xem các cột có tồn tại không
                if (!dtPhanQuyen.Columns.Contains("TenDN") || !dtPhanQuyen.Columns.Contains("LoaiNguoiDung"))
                {
                    string cols = string.Join(", ", dtPhanQuyen.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    MessageBox.Show("Cột dữ liệu không khớp! Các cột hiện có: " + cols, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dgPhanQuyen.ItemsSource = dtPhanQuyen.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtPhanQuyen == null) return;

            string keyword = txtSearch.Text.Trim().ToLower();
            DataView dv = dtPhanQuyen.DefaultView;

            if (string.IsNullOrEmpty(keyword))
                dv.RowFilter = string.Empty;
            else
                dv.RowFilter = $"TenDN LIKE '%{keyword.Replace("'", "''")}%'";

            dgPhanQuyen.ItemsSource = dv;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtPhanQuyen == null || dtPhanQuyen.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                pqBLL.UpdatePhanQuyen(dtPhanQuyen);
                MessageBox.Show("Cập nhật phân quyền thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
