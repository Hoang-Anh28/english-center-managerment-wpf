using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Do_An.BLL;

namespace Do_An
{
    public partial class NVQL_PCGV : UserControl
    {
        private readonly PhanCongBLL phanCongBLL = new PhanCongBLL();

        public NVQL_PCGV()
        {
            InitializeComponent();
            LoadComboBoxData();
            LoadDataGrid();
        }

        /// <summary>
        /// Load dữ liệu vào ComboBox Lớp học và Giảng viên
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // --- Lớp học ---
                DataTable dtLop = phanCongBLL.LayDanhSachLop();

                // Đảm bảo cột TenLopFull tồn tại
                if (!dtLop.Columns.Contains("TenLopFull"))
                    dtLop.Columns.Add("TenLopFull", typeof(string));

                foreach (DataRow row in dtLop.Rows)
                {
                    // Kiểm tra cột MaMH tồn tại và không bị null
                    if (!dtLop.Columns.Contains("MaMH") || row["MaMH"] == DBNull.Value)
                    {
                        row["TenLopFull"] = row["TenLop"].ToString();
                        continue;
                    }

                    int maMH = Convert.ToInt32(row["MaMH"]);

                    DataTable dtMH = phanCongBLL.LayThongTinMonHocVaKhoaHoc(maMH);
                    if (dtMH != null && dtMH.Rows.Count > 0)
                    {
                        string tenMH = dtMH.Rows[0]["TenMH"].ToString();
                        string tenKH = dtMH.Rows[0]["TenKH"].ToString();
                        string tenLop = row["TenLop"].ToString();

                        row["TenLopFull"] = $"{tenKH} - {tenMH} - {tenLop}";
                    }
                    else
                    {
                        row["TenLopFull"] = row["TenLop"].ToString();
                    }
                }

                cboLopHoc.ItemsSource = dtLop.DefaultView;
                cboLopHoc.DisplayMemberPath = "TenLopFull";
                cboLopHoc.SelectedValuePath = "MaLop";
                cboLopHoc.SelectedIndex = -1;

                // --- Giảng viên ---
                DataTable dtGV = phanCongBLL.LayDanhSachGiaoVien();
                cboGiaoVien.ItemsSource = dtGV.DefaultView;
                cboGiaoVien.DisplayMemberPath = "HoTen";
                cboGiaoVien.SelectedValuePath = "MaGV";
                cboGiaoVien.SelectedIndex = -1;

                dpNgayPhanCong.SelectedDate = DateTime.Now;
                txtGhiChu.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải ComboBox: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Load dữ liệu vào DataGrid
        /// </summary>
        private void LoadDataGrid()
        {
            try
            {
                // Lấy dữ liệu từ BLL/DAL, đã có cột TenLopFull
                DataTable dt = phanCongBLL.LayDanhSachPhanCong();

                // Chỉ cần bind trực tiếp
                dgPhanCong.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải DataGrid: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            cboLopHoc.SelectedIndex = -1;
            cboGiaoVien.SelectedIndex = -1;
            dpNgayPhanCong.SelectedDate = DateTime.Now;
            txtGhiChu.Clear();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboLopHoc.SelectedValue == null || cboGiaoVien.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Lớp học và Giảng viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maLop = Convert.ToInt32(cboLopHoc.SelectedValue);
                int maGV = Convert.ToInt32(cboGiaoVien.SelectedValue);
                DateTime ngay = dpNgayPhanCong.SelectedDate ?? DateTime.Now;
                string ghiChu = txtGhiChu.Text.Trim();

                if (phanCongBLL.ThemPhanCong(maLop, maGV, ngay, ghiChu))
                {
                    MessageBox.Show("Thêm phân công thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGrid();
                    BtnLamMoi_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhanCong.SelectedItem is DataRowView drv)
            {
                try
                {
                    int maPC = Convert.ToInt32(drv["MaPC"]);
                    int maLop = Convert.ToInt32(cboLopHoc.SelectedValue);
                    int maGV = Convert.ToInt32(cboGiaoVien.SelectedValue);
                    DateTime ngay = dpNgayPhanCong.SelectedDate ?? DateTime.Now;
                    string ghiChu = txtGhiChu.Text.Trim();
                    if (phanCongBLL.SuaPhanCong(maPC, maLop, maGV, ngay, ghiChu))
                    {
                        MessageBox.Show("Sửa phân công thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataGrid();
                        BtnLamMoi_Click(null, null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi sửa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhanCong.SelectedItem is DataRowView drv)
            {
                int maPC = Convert.ToInt32(drv["MaPC"]);
                if (MessageBox.Show("Bạn có chắc muốn xóa phân công này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (phanCongBLL.XoaPhanCong(maPC))
                    {
                        MessageBox.Show("Xóa phân công thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataGrid();
                        BtnLamMoi_Click(null, null);
                    }
                }
            }
        }

        private void dgPhanCong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPhanCong.SelectedItem is DataRowView drv)
            {
                int maLop = Convert.ToInt32(drv["MaLop"]);
                int maGV = Convert.ToInt32(drv["MaGV"]);
                DateTime ngay = Convert.ToDateTime(drv["NgayPhanCong"]);
                string ghiChu = drv["GhiChu"].ToString();

                cboLopHoc.SelectedValue = maLop;
                cboGiaoVien.SelectedValue = maGV;
                dpNgayPhanCong.SelectedDate = ngay;
                txtGhiChu.Text = ghiChu;
            }
        }
    }
}
