using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Do_An.BLL;
using Do_An.DAL;
using Microsoft.Win32;

namespace Do_An
{
    public partial class NVQL_BC : UserControl
    {
        private readonly Database db = new Database();
        private DataTable currentTable;
        private readonly GiaoDien_NVQL NVQL;
        private readonly ReportBLL reportBLL = new ReportBLL();
        public NVQL_BC(GiaoDien_NVQL parent = null)
        {
            InitializeComponent();
            NVQL = parent;
            Loaded += NVQL_BC_Loaded;
        }

        private void NVQL_BC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCombos();
            cbReportType.SelectedIndex = 0;
        }

        private void LoadCombos()
        {
            try
            {
                cbMon.ItemsSource = reportBLL.LayDanhSachMonHocKhongTrung().DefaultView;
                cbMon.DisplayMemberPath = "TenMH";
                cbMon.SelectedValuePath = "MaMH";

                cbLop.ItemsSource = reportBLL.LayDanhSachLopKhongTrung().DefaultView;
                cbLop.DisplayMemberPath = "TenLop";
                cbLop.SelectedValuePath = "MaLop";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load dữ liệu combobox:\n" + ex.Message);
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            int? maLop = cbLop.SelectedValue != null ? Convert.ToInt32(cbLop.SelectedValue) : (int?)null;
            int? maMH = cbMon.SelectedValue != null ? Convert.ToInt32(cbMon.SelectedValue) : (int?)null;
            string keyword = (txtKeyword.Text == "Nhập từ khóa..." || string.IsNullOrWhiteSpace(txtKeyword.Text)) ? null : txtKeyword.Text.Trim();

            try
            {
                string selectedReport = (cbReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedReport)
                {
                    case "Danh sách học viên theo lớp":
                        currentTable = reportBLL.BaoCaoHocVien(maLop, null); // không cần keyword
                        break;

                    case "Báo cáo điểm theo lớp / môn":
                        currentTable = reportBLL.BaoCaoDiem(maLop, maMH, keyword); // có keyword
                        break;

                    case "Báo cáo học phí":
                        currentTable = reportBLL.BaoCaoHocPhi(maLop, null); // không cần keyword
                        break;

                    default:
                        MessageBox.Show("Vui lòng chọn loại báo cáo hợp lệ!");
                        return;
                }

                dgReport.ItemsSource = currentTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo báo cáo: " + ex.Message);
            }
        }



        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (dgReport.ItemsSource == null)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = "BaoCaoHocVien.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                var dt = ((DataView)dgReport.ItemsSource).ToTable();
                ReportBLL.ExportToCSV(dt, sfd.FileName);
                MessageBox.Show("Xuất file thành công!", "Thông báo");
            }
        }
        private void dgReport_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void txtKeyword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtKeyword.Text == "Nhập từ khóa...")
            {
                txtKeyword.Text = "";
                txtKeyword.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtKeyword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKeyword.Text))
            {
                txtKeyword.Text = "Nhập từ khóa...";
                txtKeyword.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
        private void cbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbMon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMon.SelectedValue != null)
            {
                MessageBox.Show($"Mã môn được chọn: {cbMon.SelectedValue}");
            }
        }

    }
}
