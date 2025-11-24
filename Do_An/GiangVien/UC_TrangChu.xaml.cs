using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Do_An.BLL;

namespace Do_An.GiangVien
{
    public partial class UC_TrangChu : UserControl
    {
        private readonly GiangVienBLL giangVienBLL = new GiangVienBLL();
        private readonly int maGVdangDangNhap;

        public UC_TrangChu(int maGV)
        {
            InitializeComponent();
            maGVdangDangNhap = maGV;
            Loaded += UC_TrangChu_Loaded;
        }

        // Constructor mặc định (dành cho thiết kế XAML)
        public UC_TrangChu() : this(0) { }

        private void UC_TrangChu_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThongTinGiangVien();
            LoadAnhGiaoVien();
        }

        private void LoadThongTinGiangVien()
        {
            try
            {
                DataTable dt = giangVienBLL.LayThongTinGiangVien(maGVdangDangNhap.ToString());
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy thông tin giảng viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    return;
                }

                DataRow row = dt.Rows[0];

                bool HasCol(DataColumnCollection cols, string name) => cols.Contains(name);
                var cols = dt.Columns;

                if (HasCol(cols, "MaGV") && row["MaGV"] != DBNull.Value) txtMaGV.Text = row["MaGV"].ToString();
                if (HasCol(cols, "HoTen") && row["HoTen"] != DBNull.Value) txtHoTenGV.Text = row["HoTen"].ToString();
                if (HasCol(cols, "ChuyenMon") && row["ChuyenMon"] != DBNull.Value) txtChuyenMon.Text = row["ChuyenMon"].ToString();
                if (HasCol(cols, "Email") && row["Email"] != DBNull.Value) txtEmailGV.Text = row["Email"].ToString();
                if (HasCol(cols, "CCCD") && row["CCCD"] != DBNull.Value) txtCCCD.Text = row["CCCD"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load thông tin giảng viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtMaGV.Text = "";
            txtHoTenGV.Text = "";
            txtChuyenMon.Text = "";
            txtEmailGV.Text = "";
            txtCCCD.Text = "";
            picGiaoVien.Source = null;
        }

        private void LoadAnhGiaoVien()
        {
            try
            {
                // Tên file ảnh: GV_<MaGV>.png
                string fileName = $"GV_{maGVdangDangNhap}.png";
                string appImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagee");
                string imagePath = Path.Combine(appImagesFolder, fileName);

                if (File.Exists(imagePath))
                {
                    picGiaoVien.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
                else
                {
                    // fallback avatar mặc định
                    string defaultAvatar = Path.Combine(appImagesFolder, "avatar.png");
                    if (File.Exists(defaultAvatar))
                        picGiaoVien.Source = new BitmapImage(new Uri(defaultAvatar, UriKind.Absolute));
                    else
                        picGiaoVien.Source = null;
                }
            }
            catch
            {
                picGiaoVien.Source = null;
            }
        }
    }
}
