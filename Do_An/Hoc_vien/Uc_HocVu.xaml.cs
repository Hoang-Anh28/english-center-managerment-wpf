using System;
using System.Windows;
using System.Windows.Controls;

namespace Do_An
{
    public partial class HocVu : UserControl
    {
        private int maHV;
        public HocVu(int maHVdangNhap)
        {
            InitializeComponent();
            maHV = maHVdangNhap;
            Loaded += HocVu_Load;
        }
        public int CurrentMaHV { get; set; }
        private void HocVu_Load(object sender, RoutedEventArgs e)
        {
            LoadSubControl(new ThoiKhoaBieu());
        }

        private void LoadSubControl(UserControl uc)
        {
            panHocVu.Content = uc;
        }

        private void btnThoiKhoaBieu_Click(object sender, RoutedEventArgs e)
        {
            LoadSubControl(new ThoiKhoaBieu());
        }

        private void btnXemDiem_Click(object sender, RoutedEventArgs e)
        {

            LoadSubControl(new XemDiem(maHV));

        }

        private void btnLichThi_Click(object sender, RoutedEventArgs e)
        {
            LoadSubControl(new LichThi(maHV));
        }

    }
}
