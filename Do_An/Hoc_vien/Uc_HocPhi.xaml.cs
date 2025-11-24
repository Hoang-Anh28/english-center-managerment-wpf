using Do_An.BLL;
using Do_An.DAL;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Do_An
{
    public partial class HocPhi : UserControl
    {
        private readonly HocPhiBLL hocPhiBLL = new HocPhiBLL();
        private readonly int maHocVien;

        public HocPhi(int maHocVienDangNhap)
        {
            InitializeComponent();
            maHocVien = maHocVienDangNhap;
            LoadData();
        }

        private void LoadData()
        {
            DataTable dt = hocPhiBLL.LayHocPhiTheoHocVien(maHocVien);
            dgHocPhi.ItemsSource = dt.DefaultView;
        }
    }
}
