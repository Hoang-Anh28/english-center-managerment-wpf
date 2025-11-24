using Do_An.BLL;
using Do_An.DAL;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Do_An
{
    public partial class LichThi : UserControl
    {
        private int maHV;
        private readonly LichThiBLL bll = new LichThiBLL();
        private readonly Database db = new Database();

        public LichThi(int maHVdangNhap)
        {
            InitializeComponent();
            maHV = maHVdangNhap;
            Loaded += LichThi_Loaded;
        }

        private void LichThi_Loaded(object sender, RoutedEventArgs e)
        {
            HienThiThongTinHocVien();
            HienThiLichThi();
        }

        // ==============================
        // HIỂN THỊ THÔNG TIN HỌC VIÊN
        // ==============================
        private void HienThiThongTinHocVien()
        {
            try
            {
                string sql = $@"
                    SELECT HoTen, 
                           (SELECT TOP 1 TenLop 
                            FROM LopHoc lh 
                            JOIN DangKy dk ON lh.MaLop = dk.MaLop 
                            WHERE dk.MaHV = {maHV}) AS TenLop 
                    FROM HocVien 
                    WHERE MaHV = {maHV}";

                DataTable dt = db.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    txtMaHV.Text = maHV.ToString();
                    txtHoTen.Text = dt.Rows[0]["HoTen"].ToString();
                    txtLop.Text = dt.Rows[0]["TenLop"].ToString();
                }
                else
                {
                    txtMaHV.Text = maHV.ToString();
                    txtHoTen.Text = "Không tìm thấy";
                    txtLop.Text = "-";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin học viên: " + ex.Message);
            }
        }

        // ==============================
        // HIỂN THỊ LỊCH THI
        // ==============================
        private void HienThiLichThi()
        {
            try
            {
                dgLichThi.ItemsSource = bll.LayLichThi(maHV).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch thi: " + ex.Message);
            }
        }

        // ==============================
        // NÚT XEM LỊCH THI
        // ==============================
        private void btnXemLichThi_Click(object sender, RoutedEventArgs e)
        {
            HienThiLichThi();
        }
    }
}
