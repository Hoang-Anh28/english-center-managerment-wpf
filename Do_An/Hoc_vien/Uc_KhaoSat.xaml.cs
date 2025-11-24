using System.Windows;
using System.Windows.Controls;

namespace Do_An
{
    public partial class KhaoSat : UserControl
    {
        public KhaoSat()
        {
            InitializeComponent();
        }

        private void BtnGui_Click(object sender, RoutedEventArgs e)
        {
            string hoTen = txtHoTen.Text.Trim();
            string lop = txtLop.Text.Trim();
            string camNhan = txtCamNhan.Text.Trim();
            bool gioiTinhDaChon = rdoNam.IsChecked == true || rdoNu.IsChecked == true;

            // Kiểm tra nếu người dùng chưa điền đầy đủ thông tin
            if (string.IsNullOrEmpty(hoTen) ||
                !gioiTinhDaChon ||
                string.IsNullOrEmpty(lop) ||
                string.IsNullOrEmpty(camNhan))
            {
                MessageBox.Show(
                    "Vui lòng điền đầy đủ tất cả thông tin trước khi gửi khảo sát!",
                    "Thiếu thông tin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Nếu hợp lệ -> hiển thị thông báo thành công
            MessageBox.Show(
                "Đã ghi nhận khảo sát của bạn!\nCảm ơn bạn đã dành thời gian chia sẻ ý kiến.",
                "Khảo sát thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Xóa dữ liệu sau khi gửi
            txtHoTen.Clear();
            rdoNam.IsChecked = false;
            rdoNu.IsChecked = false;
            txtLop.Clear();
            txtCamNhan.Clear();
        }
    }
}