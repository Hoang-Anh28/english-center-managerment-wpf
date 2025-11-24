using System;
using System.Collections.Generic;
using System.Data;
using Do_An.DAL;

namespace Do_An.DAL
{
    public class LichThiDALL
    {
        // Dùng class Database hiện tại
        private Database db = new Database();

        /// <summary>
        /// Lấy lịch thi của một học viên theo mã học viên
        /// </summary>
        /// <param name="maHV">Mã học viên</param>
        /// <returns>DataTable chứa lịch thi</returns>
        public DataTable GetLichThiByMaHV(int maHV)
        {
            string sql = @"
                SELECT LT.*, MH.TenMon
                FROM LichThi LT
                INNER JOIN MonHoc MH ON LT.MaMon = MH.MaMon
                WHERE LT.MaHV = @maHV";

            // Tạo dictionary tham số
            var parameters = new Dictionary<string, object>
            {
                { "@maHV", maHV }
            };

            // Trả về DataTable từ Database.Execute
            return db.Execute(sql, parameters);
        }

        /// <summary>
        /// Thêm lịch thi mới cho học viên
        /// </summary>
        /// <param name="maHV">Mã học viên</param>
        /// <param name="maMon">Mã môn học</param>
        /// <param name="ngayThi">Ngày thi</param>
        /// <param name="gioThi">Giờ thi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public int ThemLichThi(int maHV, int maMon, DateTime ngayThi, string gioThi)
        {
            string sql = @"
                INSERT INTO LichThi(MaHV, MaMon, NgayThi, GioThi)
                VALUES(@maHV, @maMon, @ngayThi, @gioThi)";

            var parameters = new Dictionary<string, object>
            {
                { "@maHV", maHV },
                { "@maMon", maMon },
                { "@ngayThi", ngayThi },
                { "@gioThi", gioThi }
            };

            return db.ExecuteNonQuery(sql, parameters);
        }
    }
}
