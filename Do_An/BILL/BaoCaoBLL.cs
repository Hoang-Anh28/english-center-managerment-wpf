using System;
using System.Data;
using System.IO;
using System.Text;
using Do_An.DAL;
using static Do_An.DAL.Database;

namespace Do_An.BLL
{
    public class ReportBLL
    {
        private readonly ReportDAL dal = new ReportDAL();
        private static readonly Database db = new Database();

        public DataTable GetClasses() => dal.LayDanhSachLop();

        public DataTable GetSubjects() => dal.LayDanhSachMonHoc();

        public DataTable GetTeachers() => dal.LayDanhSachGiaoVien();

        // Report: students by class/subject
        public DataTable ReportStudents(int? maLop, int? maMH)
        {
            return dal.LayHocVienTheoLopOrMon(maLop, maMH);
        }

        // Report: scores
        public DataTable ReportScores(int? maLop, int? maMH, string keyword)
        {
            return dal.LayDiemTheoLoc(maLop, maMH, keyword);
        }

        // Report: fees
        public DataTable ReportFees(DateTime? fromDate, DateTime? toDate, int? maLop)
        {
            return dal.LayBaoCaoHocPhi(fromDate, toDate, maLop);
        }

        // Report: schedule
        public DataTable ReportSchedule(int? maGV, int? maLop, DateTime? fromDate, DateTime? toDate)
        {
            return dal.LayLichGiangDay(maGV, maLop, fromDate, toDate);
        }
        public DataTable BaoCaoHocVienHocPhi()
        {
            return dal.LayBaoCaoHocVienHocPhi();
        }
        public static DataTable LayBaoCaoHocVien(int? maLop, string keyword)
        {
            string sql = "SELECT HV.MaHV, HV.HoTen, HV.NgaySinh, L.TenLop " +
                         "FROM HocVien HV JOIN LopHoc L ON HV.MaLop = L.MaLop WHERE 1=1";

            var parameters = new Dictionary<string, object>();

            if (maLop.HasValue)
            {
                sql += " AND HV.MaLop = @MaLop";
                parameters["@MaLop"] = maLop.Value;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND HV.HoTen LIKE @Keyword";
                parameters["@Keyword"] = "%" + keyword + "%";
            }

            return db.Execute(sql, parameters);
        }

        public static void ExportToCSV(DataTable dt, string filePath)
        {
            var sb = new StringBuilder();

            // header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1) sb.Append(',');
            }
            sb.AppendLine();

            // data
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(row[i]?.ToString()?.Replace(",", " "));
                    if (i < dt.Columns.Count - 1) sb.Append(',');
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
        public DataTable BaoCaoHocVien(int? maLop, string keyword)
        {
            string sql = "SELECT distinct MaHV, HoTen, NgaySinh FROM HocVien";

            if (!string.IsNullOrEmpty(keyword))
                sql += " WHERE HoTen LIKE N'%' + @keyword + '%'";

            var parameters = new Dictionary<string, object>
            {
                {"@keyword", keyword ?? string.Empty}
            };

            return db.Execute(sql, parameters);
        }

        // 2️⃣ Báo cáo điểm theo lớp / môn
        public DataTable BaoCaoDiem(int? maLop, int? maMH, string keyword)
        {
            // Giả lập dữ liệu (vì chưa có bảng Diem)
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHV", typeof(int));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("MonHoc", typeof(string));
            dt.Columns.Add("Diem", typeof(double));

            dt.Rows.Add(1, "Nguyễn Văn A", "Ngữ pháp cơ bản", 8.5);
            dt.Rows.Add(2, "Trần Thị B", "Ngữ pháp cơ bản", 7.0);
            dt.Rows.Add(3, "Lê Văn C", "Ngữ pháp cơ bản", 9.0);

            // Lọc theo từ khóa nếu có
            if (!string.IsNullOrEmpty(keyword))
            {
                DataRow[] filteredRows = dt.Select($"HoTen LIKE '%{keyword}%'");
                return filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : dt.Clone();
            }

            return dt;
        }

        // 3️⃣ Báo cáo học phí
        public DataTable BaoCaoHocPhi(int? maLop, string keyword)
        {
            // Giả lập dữ liệu
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHV", typeof(int));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("HocPhi", typeof(decimal));
            dt.Columns.Add("TrangThai", typeof(string));

            dt.Rows.Add(1, "Nguyễn Văn A", "Lớp Anh cơ bản", 1500000, "Đã đóng");
            dt.Rows.Add(2, "Trần Thị B", "Lớp Anh cơ bản", 1500000, "Chưa đóng");
            dt.Rows.Add(3, "Lê Văn C", "Lớp Anh cơ bản", 1500000, "Đã đóng");

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                DataRow[] filteredRows = dt.Select($"HoTen LIKE '%{keyword}%'");
                return filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : dt.Clone();
            }

            return dt;
        }
        public DataTable LayDanhSachLopKhongTrung()
        {
            string sql = @"
        SELECT MIN(MaLop) AS MaLop, TenLop
        FROM LopHoc
        GROUP BY TenLop
        ORDER BY TenLop";
            return db.Execute(sql);
        }
        public DataTable LayDanhSachMonHocKhongTrung()
        {
            string sql = @"
        SELECT MIN(MaMH) AS MaMH, TenMH
        FROM MonHoc
        GROUP BY TenMH
        ORDER BY TenMH";
            return db.Execute(sql);
        }

    }
}
