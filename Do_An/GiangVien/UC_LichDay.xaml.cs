using System;
using System.Windows;
using System.Windows.Controls;
using Do_An.BLL;

namespace Do_An.GiangVien
{
    public partial class UC_LichDay : UserControl
    {
        private readonly GiangVienBLL bll;
        private readonly int maGV;

        // ✅ Constructor có tham số (được gọi từ Frm_GiangVienWindow)
        public UC_LichDay(int maGV)
        {
            InitializeComponent();
            this.maGV = maGV;
            bll = new GiangVienBLL();
            LoadLichDay();
        }

        // ✅ Constructor mặc định (dành cho Designer, tránh lỗi XAML)
        public UC_LichDay() : this(0) { }

        private void LoadLichDay()
        {
            try
            {
                if (maGV <= 0) return; // nếu đang ở chế độ Designer thì bỏ qua

                var dt = bll.LayLichDay(maGV.ToString());
                if (dt != null)
                    dgLichDay.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch dạy: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
