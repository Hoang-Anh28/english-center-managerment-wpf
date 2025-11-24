using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Do_An.DAL
{
    public class LopHocDAL
    {
        private readonly Database db = new Database();

        // --------------------- LOGIC CŨ ---------------------

        public DataTable LayTatCaLopHoc()
        {
            string sql = @"
                SELECT l.MaLop, l.TenLop, l.TrinhDo, l.Phong, l.ThoiGian, l.SiSoToiDa, l.TrangThai,
                       l.MaGV, gv.HoTen AS TenGV,
                       l.MaMH, mh.TenMH,
                       l.MaKH, kh.TenKH
                FROM LopHoc l
                LEFT JOIN GiaoVien gv ON l.MaGV = gv.MaGV
                LEFT JOIN MonHoc mh ON l.MaMH = mh.MaMH
                LEFT JOIN KhoaHoc kh ON l.MaKH = kh.MaKH
                ORDER BY kh.TenKH, l.TenLop";
            return db.Execute(sql, new Dictionary<string, object>());
        }

        public int ThemLopHoc(string tenLop, string trinhDo, string phong, string thoiGian,
                              int siSoToiDa, string trangThai, int maMH, int maKH)
        {
            string sql = @"
                INSERT INTO LopHoc (TenLop, TrinhDo, Phong, ThoiGian, SiSoToiDa, TrangThai, MaMH, MaKH)
                VALUES (@TenLop, @TrinhDo, @Phong, @ThoiGian, @SiSoToiDa, @TrangThai, @MaMH, @MaKH)";
            var parameters = new Dictionary<string, object>
            {
                {"@TenLop", tenLop},
                {"@TrinhDo", trinhDo ?? (object)DBNull.Value},
                {"@Phong", phong ?? (object)DBNull.Value},
                {"@ThoiGian", thoiGian ?? (object)DBNull.Value},
                {"@SiSoToiDa", siSoToiDa},
                {"@TrangThai", trangThai ?? (object)DBNull.Value},
                {"@MaMH", maMH > 0 ? (object)maMH : DBNull.Value},
                {"@MaKH", maKH > 0 ? (object)maKH : DBNull.Value}
            };

            return db.ExecuteNonQuery(sql, parameters);
        }

        // Lấy ds lớp (bổ sung join tên KH, MH, GV để hiển thị rõ)



        public DataTable LayDanhSachLopHoc()
        {
            string sql = @"SELECT L.*, GV.TenGV, MH.TenMH, KH.TenKH
                           FROM LopHoc L
                           LEFT JOIN GiaoVien GV ON L.MaGV = GV.MaGV
                           LEFT JOIN MonHoc MH ON L.MaMH = MH.MaMH
                           LEFT JOIN KhoaHoc KH ON L.MaKH = KH.MaKH";
            return db.Execute(sql);
        }


        public DataTable LayDanhSachMonHoc()
        {
            string sql = "SELECT DISTINCT MaMH, TenMH FROM MonHoc";
            return db.Execute(sql);
        }

        public DataTable LayDanhSachGiaoVien()
        {
            string sql = "SELECT MaGV, HoTen FROM GiaoVien";
            return db.Execute(sql);
        }

        public DataTable LayLopTheoGiangVien(int maGV)
        {
            string sql = @"SELECT MaLop, TenLop, Phong, ThoiGian, SiSoToiDa, TrangThai, TrinhDo
                           FROM LopHoc WHERE MaGV = @MaGV";
            var parameters = new Dictionary<string, object>
            {
                {"@MaGV", maGV}
            };
            return db.Execute(sql, parameters);
        }

        public int CapNhatTrangThaiLop(int maLop, string trangThai)
        {
            string sql = @"UPDATE LopHoc SET TrangThai = @TrangThai WHERE MaLop = @MaLop";
            var parameters = new Dictionary<string, object>
            {
                {"@TrangThai", trangThai},
                {"@MaLop", maLop}
            };
            return db.ExecuteNonQuery(sql, parameters);
        }

        public DataTable LayThongTinLopHoc_DanhSach()
        {
            string sql = @"
        SELECT 
            lh.MaLop,
            kh.TenKH,
            mh.TenMH,
            lh.TenLop,
            gv.HoTen AS GiaoVien,
            COUNT(dk.MaHV) AS SiSo,
            lh.TrinhDo
        FROM LopHoc lh
        LEFT JOIN MonHoc mh ON lh.MaMH = mh.MaMH
        LEFT JOIN KhoaHoc kh ON lh.MaKH = kh.MaKH
        LEFT JOIN PhanCong pc ON lh.MaLop = pc.MaLop
        LEFT JOIN GiaoVien gv ON pc.MaGV = gv.MaGV
        LEFT JOIN DangKy dk ON lh.MaLop = dk.MaLop
        GROUP BY lh.MaLop, kh.TenKH, mh.TenMH, lh.TenLop, gv.HoTen, lh.TrinhDo
        ORDER BY kh.TenKH, mh.TenMH, lh.TenLop, lh.TrinhDo";
            return db.Execute(sql);
        }

        public DataTable TimKiemLopHoc(string tuKhoa)
        {
            string sql = @"
                SELECT l.MaLop, l.TenLop, l.TrinhDo, g.HoTen AS GiaoVien, 
                       COUNT(dk.MaHV) AS SiSo
                FROM LopHoc l
                LEFT JOIN GiaoVien g ON l.MaGV = g.MaGV
                LEFT JOIN DangKy dk ON l.MaLop = dk.MaLop
                WHERE l.TenLop LIKE @TuKhoa OR g.HoTen LIKE @TuKhoa
                GROUP BY l.MaLop, l.TenLop, l.TrinhDo, g.HoTen";
            var parameters = new Dictionary<string, object>
            {
                {"@TuKhoa", "%" + tuKhoa + "%"}
            };
            return db.Execute(sql, parameters);
        }

        public int XoaLopHoc(int maLop)
        {
            string sql = "DELETE FROM LopHoc WHERE MaLop = @MaLop";
            var parameters = new Dictionary<string, object>
            {
                {"@MaLop", maLop}
            };
            return db.ExecuteNonQuery(sql, parameters);
        }

        public int SuaLopHoc(int maLop, string tenLop, string trinhDo, int maGV)
        {
            string sql = "UPDATE LopHoc SET TenLop = @TenLop, TrinhDo = @TrinhDo, MaGV = @MaGV WHERE MaLop = @MaLop";
            var parameters = new Dictionary<string, object>
            {
                {"@TenLop", tenLop},
                {"@TrinhDo", trinhDo},
                {"@MaGV", maGV},
                {"@MaLop", maLop}
            };
            return db.ExecuteNonQuery(sql, parameters);
        }

        public DataTable LayThongTinLopHocChiTiet(string maLop)
        {
            string sql = @"
                SELECT l.*, g.HoTen AS GiaoVien, m.TenMH AS MonHoc
                FROM LopHoc l
                LEFT JOIN GiaoVien g ON l.MaGV = g.MaGV
                LEFT JOIN MonHoc m ON l.MaMH = m.MaMH
                WHERE l.MaLop = @MaLop";
            var parameters = new Dictionary<string, object>
            {
                {"@MaLop", maLop}
            };
            return db.Execute(sql, parameters);
        }

        public int DemSoHocVien(int maLop)
        {
            string sql = "SELECT COUNT(*) FROM DangKy WHERE MaLop = @MaLop";
            var parameters = new Dictionary<string, object>
            {
                {"@MaLop", maLop}
            };
            object result = db.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result);
        }

        // --------------------- PHƯƠNG THỨC MỚI ĐỒNG BỘ VỚI MLH ---------------------

        // Lấy danh sách khóa học (cho ComboBox Khóa học)
        public DataTable LayDanhSachKhoaHoc()
        {
            string sql = "SELECT KH.MaKH, KH.TenKH\r\nFROM KhoaHoc KH";
            return db.Execute(sql);
        }

        // Lấy danh sách môn học theo khóa học (cho ComboBox Môn học)
        public DataTable LayMonHocTheoKhoaHoc(int maKhoaHoc)
        {
            string sql = @"
        SELECT TenMH
        FROM MonHoc
        WHERE MaKH = @MaKhoaHoc
        GROUP BY TenMH
        ORDER BY TenMH";

            var parameters = new Dictionary<string, object>
    {
        { "@MaKhoaHoc", maKhoaHoc }
    };

            return db.Execute(sql, parameters);
        }
        public int LayMaMonHocTheoTen(int maKhoaHoc, string tenMonHoc)
        {
            string sql = "SELECT TOP 1 MaMH FROM MonHoc WHERE MaKH = @MaKH AND TenMH = @TenMH";
            var parameters = new Dictionary<string, object>
    {
        { "@MaKH", maKhoaHoc },
        { "@TenMH", tenMonHoc }
    };
            object result = db.ExecuteScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

    }
}
