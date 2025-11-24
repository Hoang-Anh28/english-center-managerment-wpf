using Do_An.BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Do_An
{
    public partial class ThoiKhoaBieu : UserControl
    {
        ThoiKhoaBieuBLL bll = new ThoiKhoaBieuBLL();
        private int currentWeek = 1;
        private const int totalWeeks = 10;

        // 👇 Ngày bắt đầu học kỳ
        private DateTime semesterStart = new DateTime(2025, 1, 1);

        public ThoiKhoaBieu()
        {
            InitializeComponent();

            currentWeek = 1;
            LoadWeek();
        }

        private int GetCurrentWeek()
        {
            TimeSpan diff = DateTime.Now.Date - semesterStart.Date;
            int week = (diff.Days / 7) + 1;

            if (week < 1) week = 1;
            if (week > totalWeeks) week = totalWeeks;

            return week;
        }

        private void LoadWeek()
        {
            DateTime startDate = semesterStart.AddDays((currentWeek - 1) * 7);
            DateTime endDate = startDate.AddDays(6);

            txtCurrentWeek.Text = $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";

            dgvTKB.ItemsSource = bll.LayTKBTheoTuan(currentWeek).DefaultView;
        }

        private void btnPrevWeek_Click(object sender, RoutedEventArgs e)
        {
            if (currentWeek > 1)
            {
                currentWeek--;
                LoadWeek();
            }
            else
            {
                MessageBox.Show("Đây là tuần đầu tiên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnNextWeek_Click(object sender, RoutedEventArgs e)
        {
            if (currentWeek < totalWeeks)
            {
                currentWeek++;
                LoadWeek();
            }
            else
            {
                MessageBox.Show("Đây là tuần cuối cùng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnTuanHienTai_Click(object sender, RoutedEventArgs e)
        {
            currentWeek = 1;
            LoadWeek();
        }

        private void dgvTKB_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
