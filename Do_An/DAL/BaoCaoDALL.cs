using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Do_An.DAL
{
    // --------------------- LỚP Báo Cáo ---------------------
    public class ReportDAL
    {
        private readonly Database db = new Database();

        // Dùng để load danh sách lớp (combobox)
        public DataTable LayDanhSachLop()
        {
            string sql = "SELECT MaLop, TenLop FROM LopHoc ORDER BY TenLop";
            return db.Execute(sql);
        }

        // Dùng để load danh sách môn học (combobox)
        public DataTable LayDanhSachMonHoc()
        {
            string sql = "SELECT MaMH, TenMH FROM MonHoc ORDER BY TenMH";
            return db.Execute(sql);
        }

        // Dùng để load danh sách giáo viên (combobox)
        public DataTable LayDanhSachGiaoVien()
        {
            string sql = "SELECT MaGV, HoTen AS TenGV FROM GiaoVien ORDER BY HoTen";
            return db.Execute(sql);
        }

        // 1) Báo cáo: danh sách học viên theo lớp / môn
        public DataTable LayHocVienTheoLopOrMon(int? maLop, int? maMH)
        {
            var sql = @"
                SELECT hv.MaHV, hv.HoTen, hv.NgaySinh, hv.GioiTinh, hv.SDT, hv.Email, hv.TrinhDo,
                       lh.MaLop, lh.TenLop, mh.MaMH, mh.TenMH
                FROM HocVien hv
                LEFT JOIN DangKy dk ON hv.MaHV = dk.MaHV
                LEFT JOIN LopHoc lh ON dk.MaLop = lh.MaLop
                LEFT JOIN MonHoc mh ON lh.MaMH = mh.MaMH
                WHERE 1=1
            ";

            var parameters = new Dictionary<string, object>();

            if (maLop.HasValue)
            {
                sql += " AND lh.MaLop = @MaLop";
                parameters.Add("@MaLop", maLop.Value);
            }

            if (maMH.HasValue)
            {
                sql += " AND mh.MaMH = @MaMH";
                parameters.Add("@MaMH", maMH.Value);
            }

            sql += " ORDER BY lh.TenLop, hv.HoTen";

            return db.Execute(sql, parameters);
        }

        // 2) Báo cáo: điểm (lọc theo lớp / môn / học viên)
        public DataTable LayDiemTheoLoc(int? maLop, int? maMH, string keyword)
        {
            var sql = @"
                SELECT D.MaHV, HV.HoTen, LH.MaLop, LH.TenLop, MH.MaMH, MH.TenMH,
                       D.DiemGK, D.DiemCK, D.DiemTB
                FROM Diem D
                INNER JOIN HocVien HV ON D.MaHV = HV.MaHV
                INNER JOIN LopHoc LH ON D.MaLop = LH.MaLop
                INNER JOIN MonHoc MH ON LH.MaMH = MH.MaMH
                WHERE 1=1
            ";

            var parameters = new Dictionary<string, object>();

            if (maLop.HasValue)
            {
                sql += " AND LH.MaLop = @MaLop";
                parameters.Add("@MaLop", maLop.Value);
            }

            if (maMH.HasValue)
            {
                sql += " AND MH.MaMH = @MaMH";
                parameters.Add("@MaMH", maMH.Value);
            }

            sql += " AND (@Keyword IS NULL OR HV.HoTen LIKE '%' + @Keyword + '%')";
            parameters.Add("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim());

            return db.Execute(sql, parameters);
        }

        // 3) Báo cáo: doanh thu / học phí (theo khoảng thời gian và/hoặc lớp)
        public DataTable LayBaoCaoHocPhi(DateTime? fromDate, DateTime? toDate, int? maLop)
        {
            var sql = @"
                SELECT hp.MaHP, hp.MaHV, hv.HoTen, hp.MaLop, lh.TenLop,
                       hp.SoTien, hp.DaDong, hp.ConNo, hp.PhuongThuc, hp.NgayDong
                FROM HocPhi hp
                LEFT JOIN HocVien hv ON hp.MaHV = hv.MaHV
                LEFT JOIN LopHoc lh ON hp.MaLop = lh.MaLop
                WHERE 1=1
            ";

            var parameters = new Dictionary<string, object>();

            if (maLop.HasValue)
            {
                sql += " AND hp.MaLop = @MaLop";
                parameters.Add("@MaLop", maLop.Value);
            }

            if (fromDate.HasValue)
            {
                sql += " AND hp.NgayDong >= @FromDate";
                parameters.Add("@FromDate", fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                sql += " AND hp.NgayDong <= @ToDate";
                parameters.Add("@ToDate", toDate.Value.Date);
            }

            sql += " ORDER BY hp.NgayDong DESC";

            return db.Execute(sql, parameters);
        }

        // 4) Báo cáo: lịch giảng dạy (dựa trên PhanCong)
        public DataTable LayLichGiangDay(int? maGV, int? maLop, DateTime? fromDate, DateTime? toDate)
        {
            var sql = @"
                SELECT pc.MaPC, pc.MaLop, lh.TenLop, pc.MaGV, gv.HoTen AS TenGV,
                       pc.NgayPhanCong, pc.GhiChu
                FROM PhanCong pc
                INNER JOIN LopHoc lh ON pc.MaLop = lh.MaLop
                INNER JOIN GiaoVien gv ON pc.MaGV = gv.MaGV
                WHERE 1=1
            ";

            var parameters = new Dictionary<string, object>();

            if (maGV.HasValue)
            {
                sql += " AND pc.MaGV = @MaGV";
                parameters.Add("@MaGV", maGV.Value);
            }

            if (maLop.HasValue)
            {
                sql += " AND pc.MaLop = @MaLop";
                parameters.Add("@MaLop", maLop.Value);
            }

            if (fromDate.HasValue)
            {
                sql += " AND pc.NgayPhanCong >= @FromDate";
                parameters.Add("@FromDate", fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                sql += " AND pc.NgayPhanCong <= @ToDate";
                parameters.Add("@ToDate", toDate.Value.Date);
            }

            sql += " ORDER BY pc.NgayPhanCong DESC";

            return db.Execute(sql, parameters);
        }
        // Báo cáo học viên và học phí
        public DataTable LayBaoCaoHocVienHocPhi()
        {
            string sql = @"
        SELECT 
            HV.MaHV,
            HV.HoTen AS [Tên học viên],
            LH.TenLop AS [Lớp học],
            MH.TenMH AS [Môn học],
            LH.NgayBatDau,
            LH.NgayKetThuc,
            CASE 
                WHEN HP.TrangThai = 1 THEN N'Đã đóng'
                ELSE N'Chưa đóng'
            END AS [Tình trạng học phí],
            ISNULL(Diem.DiemTB, 0) AS [Điểm trung bình]
        FROM HocVien HV
        JOIN DangKy DK ON HV.MaHV = DK.MaHV
        JOIN LopHoc LH ON DK.MaLop = LH.MaLop
        JOIN MonHoc MH ON LH.MaMH = MH.MaMH
        LEFT JOIN HocPhi HP ON HV.MaHV = HP.MaHV AND LH.MaLop = HP.MaLop
        LEFT JOIN (
            SELECT MaHV, MaLop, AVG(DiemSo) AS DiemTB
            FROM KetQua
            GROUP BY MaHV, MaLop
        ) AS Diem ON HV.MaHV = Diem.MaHV AND LH.MaLop = Diem.MaLop
        ORDER BY HV.HoTen
    ";

            return db.Execute(sql);
        }

    }
}
