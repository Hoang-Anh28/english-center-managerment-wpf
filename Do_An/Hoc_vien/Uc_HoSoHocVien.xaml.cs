using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Do_An.BLL;

namespace Do_An
{
    public partial class HoSoHocVien : UserControl
    {
        private readonly HocVienBLL hocVienBLL = new HocVienBLL();
        private readonly int maHVdangDangNhap;

        public HoSoHocVien(int maHV)
        {
            InitializeComponent();
            maHVdangDangNhap = maHV;
            Loaded += HoSoHocVien_Load;
        }

        private void HoSoHocVien_Load(object sender, RoutedEventArgs e)
        {
            LoadThongTinHocVien();
            LoadAnhHocVien();
        }

        private void LoadThongTinHocVien()
        {
            try
            {
                DataTable dt = hocVienBLL.LayThongTinHocVienVaLop(maHVdangDangNhap);
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy hồ sơ học viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    return;
                }

                // Nếu học viên có nhiều dòng (nhiều lớp) ta lấy dòng đầu để hiển thị thông tin cá nhân + 1 lớp hiện tại
                DataRow row = dt.Rows[0];

                // Helper để kiểm tra cột tồn tại trước khi đọc
                bool HasCol(DataColumnCollection cols, string name) => cols.Contains(name);

                var cols = dt.Columns;

                if (HasCol(cols, "MaHV") && row["MaHV"] != DBNull.Value) txtMaHV.Text = row["MaHV"].ToString();
                if (HasCol(cols, "HoTen") && row["HoTen"] != DBNull.Value) txtHoTen.Text = row["HoTen"].ToString();
                if (HasCol(cols, "NgaySinh") && row["NgaySinh"] != DBNull.Value)
                {
                    if (DateTime.TryParse(row["NgaySinh"].ToString(), out DateTime ns))
                        dtNgaySinh.SelectedDate = ns;
                }
                if (HasCol(cols, "GioiTinh") && row["GioiTinh"] != DBNull.Value) cbGioiTinh.Text = row["GioiTinh"].ToString();
                if (HasCol(cols, "DiaChi") && row["DiaChi"] != DBNull.Value) txtDiaChi.Text = row["DiaChi"].ToString();
                if (HasCol(cols, "SDT") && row["SDT"] != DBNull.Value) txtSDT.Text = row["SDT"].ToString();
                if (HasCol(cols, "Email") && row["Email"] != DBNull.Value) txtEmail.Text = row["Email"].ToString();
                if (HasCol(cols, "TrinhDo") && row["TrinhDo"] != DBNull.Value) /* bạn có textbox tương ứng? */ { /* txtTrinhDo.Text = row["TrinhDo"].ToString(); */ }
                // Lớp: có thể null nếu chưa đăng ký lớp
                if (HasCol(cols, "TenLop") && row["TenLop"] != DBNull.Value) txtLop.Text = row["TenLop"].ToString();
                else txtLop.Text = "Chưa có lớp";// Trường phụ huynh (schema của bạn không có) — để trống nếu không có
                txtTenPH.Text = row["TenPhuHuynh"].ToString();
                txtSDTPH.Text = row["SDTPhuHuynh"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load hồ sơ học viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtMaHV.Text = "";
            txtHoTen.Text = "";
            dtNgaySinh.SelectedDate = null;
            cbGioiTinh.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            txtLop.Text = "";
            txtTenPH.Text = "";
            txtSDTPH.Text = "";
            picHocVien.Source = null;
        }

        private void LoadAnhHocVien()
        {
            try
            {
                // Tên file mặc định: HV_<MaHV>.png (bạn có thể thay quy ước)
                string fileName = $"HV_{maHVdangDangNhap}.png";
                string appImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imagee");
                string imagePath = Path.Combine(appImagesFolder, fileName);

                if (File.Exists(imagePath))
                {
                    picHocVien.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
                else
                {
                    // fallback avatar mặc định
                    string defaultAvatar = Path.Combine(appImagesFolder, "avatar.png");
                    if (File.Exists(defaultAvatar))
                        picHocVien.Source = new BitmapImage(new Uri(defaultAvatar, UriKind.Absolute));
                    else
                        picHocVien.Source = null;
                }
            }
            catch
            {
                picHocVien.Source = null;
            }
        }
    }
}