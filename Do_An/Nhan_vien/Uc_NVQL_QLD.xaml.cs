using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An
{
    public partial class NVQL_QLD : UserControl
    {
        private readonly DiemBLL diemBLL = new DiemBLL();
        private bool _isLoaded = false;

        public NVQL_QLD()
        {
            InitializeComponent();
            Loaded += NVQL_QLD_Loaded;
        }

        private void NVQL_QLD_Loaded(object sender, RoutedEventArgs e)
        {
            // Ngăn việc load lại dữ liệu nhiều lần khi control được re-render
            if (_isLoaded) return;
            _isLoaded = true;

            try
            {
                LoadKhoaHoc();
                LoadAllDiem();

                // Đăng ký event chỉ một lần
                btnTimKiem.Click += BtnTimKiem_Click;
                btnLamMoi.Click += BtnLamMoi_Click;
                cbKhoaHoc.SelectionChanged += cbKhoaHoc_SelectionChanged;
                cbLopHoc.SelectionChanged += cbLopHoc_SelectionChanged;
                txtMaHV.TextChanged += txtMaHV_TextChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khởi tạo giao diện: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ------------------ LOAD DỮ LIỆU ------------------

        private void LoadKhoaHoc()
        {
            try
            {
                DataTable dtKhoaHoc = diemBLL.LayDanhSachKhoaHoc();
                cbKhoaHoc.ItemsSource = dtKhoaHoc?.DefaultView;
                cbKhoaHoc.DisplayMemberPath = "TenKH";
                cbKhoaHoc.SelectedValuePath = "MaKH";
                cbKhoaHoc.SelectedIndex = -1;
                cbLopHoc.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải khóa học: " + ex.Message);
            }
        }

        private void LoadAllDiem()
        {
            try
            {
                txtMaHV.Text = string.Empty;
                txtPlaceholder.Visibility = Visibility.Visible;

                DataTable dt = diemBLL.LayTatCaDiem();
                dgDiem.ItemsSource = dt?.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải điểm: " + ex.Message);
            }
        }

        // ------------------ SỰ KIỆN ------------------

        private void cbKhoaHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbKhoaHoc.SelectedValue == null)
            {
                cbLopHoc.ItemsSource = null;
                return;
            }

            try
            {
                int maKH = Convert.ToInt32(cbKhoaHoc.SelectedValue);
                DataTable dtLop = diemBLL.LayMonHocTheoKhoaHoc(maKH);

                if (dtLop != null && dtLop.Rows.Count > 0)
                {
                    cbLopHoc.ItemsSource = dtLop.DefaultView;
                    cbLopHoc.DisplayMemberPath = "TenLop";
                    cbLopHoc.SelectedValuePath = "MaLop";
                    cbLopHoc.SelectedIndex = -1;
                }
                else
                {
                    cbLopHoc.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lớp học: " + ex.Message);
            }
        }

        private void cbLopHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLopHoc.SelectedValue == null) return;

            try
            {
                int maLop = Convert.ToInt32(cbLopHoc.SelectedValue);
                DataTable dt = diemBLL.LayDiemTheoLop(maLop);
                dgDiem.ItemsSource = dt?.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải điểm theo lớp: " + ex.Message);
            }
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? maKH = cbKhoaHoc.SelectedValue != null ? (int?)Convert.ToInt32(cbKhoaHoc.SelectedValue) : null;
                int? maLop = cbLopHoc.SelectedValue != null ? (int?)Convert.ToInt32(cbLopHoc.SelectedValue) : null;
                string keyword = string.IsNullOrWhiteSpace(txtMaHV.Text) ? null : txtMaHV.Text.Trim();

                DataTable dt = diemBLL.TimDiem(maLop, maKH, keyword);
                dgDiem.ItemsSource = dt?.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            LoadAllDiem();
            LoadKhoaHoc();
        }

        private void txtMaHV_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPlaceholder.Visibility = string.IsNullOrWhiteSpace(txtMaHV.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}
