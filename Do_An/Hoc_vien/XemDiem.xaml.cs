using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Do_An.DAL;

namespace Do_An
{
    public partial class XemDiem : UserControl
    {
        public int MaHV { get; set; }  // <-- Nhận ID khi login
        Database db = new Database();

        // ✅ Constructor mới có tham số
        public XemDiem(int maHVdangNhap)
        {
            InitializeComponent();
            MaHV = maHVdangNhap;
            Loaded += XemDiem_Load;
        }

        // ⚠️ Giữ constructor cũ để nếu ai đó lỡ dùng không truyền ID
        public XemDiem()
        {
            InitializeComponent();
            Loaded += XemDiem_Load;
        }

        private void XemDiem_Load(object sender, RoutedEventArgs e)
        {
            if (MaHV == 0)
            {
                MessageBox.Show("Không tìm thấy mã học viên. Hãy đăng nhập lại!",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadThongTinHV();
            LoadKhoaHoc();
        }

        void LoadThongTinHV()
        {
            string sql = @"SELECT HoTen FROM HocVien WHERE MaHV=@id";
            var tb = db.Execute(sql, new Dictionary<string, object> { { "@id", MaHV } });

            if (tb.Rows.Count > 0)
            {
                txtHoTen.Text = tb.Rows[0]["HoTen"].ToString();
                txtMaHV.Text = MaHV.ToString();
            }
        }

        void LoadKhoaHoc()
        {
            string sql = @"
                SELECT DISTINCT kh.MaKH, kh.TenKH
                FROM DangKy dk
                JOIN LopHoc lh ON dk.MaLop = lh.MaLop
                JOIN KhoaHoc kh ON lh.MaKH = kh.MaKH
                WHERE dk.MaHV = @id";

            var tb = db.Execute(sql, new Dictionary<string, object> { { "@id", MaHV } });

            cbKhoaHoc.ItemsSource = tb.DefaultView;
            cbKhoaHoc.DisplayMemberPath = "TenKH";
            cbKhoaHoc.SelectedValuePath = "MaKH";
        }

        void LoadMonHoc()
        {
            if (cbKhoaHoc.SelectedValue == null) return;

            string sql = @"
                SELECT mh.MaMH, mh.TenMH
                FROM MonHoc mh
                WHERE mh.MaKH=@kh";

            var tb = db.Execute(sql, new Dictionary<string, object> {
                { "@kh", cbKhoaHoc.SelectedValue }
            });

            cbMonHoc.ItemsSource = tb.DefaultView;
            cbMonHoc.DisplayMemberPath = "TenMH";
            cbMonHoc.SelectedValuePath = "MaMH";
        }

        private void cbKhoaHoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadMonHoc();
        }

        private void btnXemDiem_Click(object sender, RoutedEventArgs e)
        {
            LoadBangDiem();
        }

        private void LoadBangDiem()
        {
            if (cbMonHoc.SelectedValue == null) return;

            string sql = @"
                SELECT mh.TenMH, d.DiemGK, d.DiemCK, d.DiemTB, lh.TenLop
                FROM Diem d
                JOIN LopHoc lh ON d.MaLop = lh.MaLop
                JOIN MonHoc mh ON lh.MaMH = mh.MaMH
                WHERE d.MaHV=@hv AND mh.MaMH=@mh";

            DataTable tb = db.Execute(sql, new Dictionary<string, object> {
                { "@hv", MaHV },
                { "@mh", cbMonHoc.SelectedValue }
            });

            List<DiemHocVien> ds = new List<DiemHocVien>();
            int i = 1;

            foreach (DataRow row in tb.Rows)
            {
                ds.Add(new DiemHocVien(
                    i++,
                    row["TenMH"].ToString(),
                    Convert.ToDouble(row["DiemGK"]),
                    Convert.ToDouble(row["DiemCK"]),
                    Convert.ToDouble(row["DiemTB"])
                ));

                txtLop.Text = row["TenLop"].ToString();
            }

            dgvDiem.ItemsSource = ds;

            if (ds.Count > 0)
            {
                double avg = ds.Average(x => x.DiemTB);
                txtDiemTB.Text = avg.ToString("F2");
                txtXepLoai.Text = avg >= 8 ? "Giỏi" :
                                  avg >= 6.5 ? "Khá" :
                                  avg >= 5 ? "Trung bình" : "Yếu";
            }
        }
    }

    public class DiemHocVien
    {
        public int STT { get; set; }
        public string MonHoc { get; set; }
        public double DiemGK { get; set; }
        public double DiemCK { get; set; }
        public double DiemTB { get; set; }
        public string XepLoai =>
            DiemTB >= 8 ? "Giỏi" :
            DiemTB >= 6.5 ? "Khá" :
            DiemTB >= 5 ? "Trung bình" : "Yếu";

        public DiemHocVien(int stt, string mh, double gk, double ck, double tb)
        {
            STT = stt;
            MonHoc = mh;
            DiemGK = gk;
            DiemCK = ck;
            DiemTB = tb;
        }
    }
}
