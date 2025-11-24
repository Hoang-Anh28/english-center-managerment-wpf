using System;
using System.Collections.Generic;
using System.Data;

namespace Do_An.DAL
{
    public class PhanCongDAL
    {
        private readonly Database db = new Database();

        /// <summary>
        /// Lấy danh sách phân công với thông tin đầy đủ: Khóa học - Môn học - Lớp học
        /// </summary>
        public DataTable LayDanhSachPhanCong()
        {
            string sql = @"
                SELECT 
                    pc.MaPC,
                    lh.MaLop,
                    kh.TenKH,
                    mh.TenMH,
                    lh.TenLop,
                    gv.MaGV, 
                    gv.HoTen AS TenGV,
                    pc.NgayPhanCong,
                    pc.GhiChu,
                    -- Tạo cột TenLopFull để hiển thị trong DataGrid
                    (kh.TenKH + ' , ' + mh.TenMH + ' , ' + lh.TenLop) AS TenLopFull
                FROM PhanCong pc
                INNER JOIN LopHoc lh ON pc.MaLop = lh.MaLop
                INNER JOIN MonHoc mh ON lh.MaMH = mh.MaMH
                INNER JOIN KhoaHoc kh ON mh.MaKH = kh.MaKH
                INNER JOIN GiaoVien gv ON pc.MaGV = gv.MaGV
                ORDER BY pc.MaPC DESC";
            return db.Execute(sql);
        }

        public DataTable LayThongTinMonHocVaKhoaHoc(int maMH)
        {
            string sql = @"
        SELECT distinct mh.TenMH, kh.TenKH
        FROM MonHoc mh
        INNER JOIN KhoaHoc kh ON mh.MaKH = kh.MaKH
        WHERE mh.MaMH = @MaMH";
            var parameters = new Dictionary<string, object> { { "@MaMH", maMH } };
            return db.Execute(sql, parameters);
        }


        /// <summary>
        /// Lấy danh sách lớp học
        /// </summary>
        public DataTable LayDanhSachLop()
        {
            // Chỉ lấy tất cả lớp thật sự, không gộp
            string sql = @"
       WITH LopDanhSach AS
(
    SELECT 
        lh.MaLop,
        kh.TenKH,
        mh.TenMH,
        lh.TenLop,
        (kh.TenKH + ' - ' + mh.TenMH + ' - ' + lh.TenLop) AS TenLopFull,
        ROW_NUMBER() OVER(PARTITION BY kh.TenKH, mh.TenMH, lh.TenLop ORDER BY lh.MaLop) AS rn
    FROM LopHoc lh
    INNER JOIN MonHoc mh ON lh.MaMH = mh.MaMH
    INNER JOIN KhoaHoc kh ON mh.MaKH = kh.MaKH
)
SELECT MaLop, TenKH, TenMH, TenLop, TenLopFull
FROM LopDanhSach
WHERE rn = 1
ORDER BY TenKH, TenMH, TenLop";


            return db.Execute(sql);
        }


        /// <summary>
        /// Lấy danh sách giáo viên
        /// </summary>
        public DataTable LayDanhSachGiaoVien()
        {
            return db.Execute("SELECT MIN(MaGV) AS MaGV, HoTen FROM GiaoVien GROUP BY HoTen ORDER BY HoTen");
        }

        /// <summary>
        /// Thêm phân công
        /// </summary>
        public bool ThemPhanCong(int maLop, int maGV, DateTime ngayPhanCong, string ghiChu)
        {
            string sql = @"
                INSERT INTO PhanCong (MaLop, MaGV, NgayPhanCong, GhiChu)
                VALUES (@MaLop, @MaGV, @NgayPhanCong, @GhiChu)";
            var parameters = new Dictionary<string, object>
            {
                { "@MaLop", maLop },
                { "@MaGV", maGV },
                { "@NgayPhanCong", ngayPhanCong },
                { "@GhiChu", string.IsNullOrWhiteSpace(ghiChu) ? (object)DBNull.Value : ghiChu }
            };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        /// <summary>
        /// Sửa phân công
        /// </summary>
        public bool SuaPhanCong(int maPC, int maLop, int maGV, DateTime ngayPhanCong, string ghiChu)
        {
            string sql = @"
                UPDATE PhanCong
                SET MaLop = @MaLop, MaGV = @MaGV, NgayPhanCong = @NgayPhanCong, GhiChu = @GhiChu
                WHERE MaPC = @MaPC";
            var parameters = new Dictionary<string, object>
            {
                { "@MaPC", maPC },
                { "@MaLop", maLop },
                { "@MaGV", maGV },
                { "@NgayPhanCong", ngayPhanCong },
                { "@GhiChu", string.IsNullOrWhiteSpace(ghiChu) ? (object)DBNull.Value : ghiChu }
            };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        /// <summary>
        /// Xóa phân công
        /// </summary>
        public bool XoaPhanCong(int maPC)
        {
            string sql = "DELETE FROM PhanCong WHERE MaPC = @MaPC";
            var parameters = new Dictionary<string, object> { { "@MaPC", maPC } };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }
        public int DemSoLuongGiaoVien()
        {
            string sql = "SELECT COUNT(*) FROM GiaoVien";
            object result = db.ExecuteScalar(sql);
            return result != null ? Convert.ToInt32(result) : 0;
        }

    }
}
