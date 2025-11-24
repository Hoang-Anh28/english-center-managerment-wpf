using System.Windows;
using System.Windows.Controls;
using System.Data;
using Do_An.BLL;

namespace Do_An.GiangVien
{
    public partial class UC_GiangVien : UserControl
    {
        private readonly GiangVienBLL gvBLL = new GiangVienBLL();
        private readonly string maGV = "GV001"; // test tạm

        public UC_GiangVien()
        {
            InitializeComponent();
            LoadLichDay();
        }

        private void LoadLichDay()
        {
            dgLichDay.ItemsSource = gvBLL.LayLichDay(maGV).DefaultView;
        }

        private void dgLichDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgLichDay.SelectedItem is DataRowView row)
            {
                string maLop = row["MaLop"].ToString();
                dgHocVien.ItemsSource = gvBLL.LayHocVienTheoLop(maLop).DefaultView;
            }
        }

        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHV.Text)) return;

            if (double.TryParse(txtDiemGK.Text, out double gk) && double.TryParse(txtDiemCK.Text, out double ck))
            {
                bool ok = gvBLL.CapNhatDiem(txtMaHV.Text, gk, ck);
                MessageBox.Show(ok ? "Cập nhật thành công" : "Điểm không hợp lệ!");
            }
            else
            {
                MessageBox.Show("Nhập điểm sai định dạng!");
            }
        }
    }
}
