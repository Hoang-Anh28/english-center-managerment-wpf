using System;
using System.Data;
using Do_An.DAL;
using static Do_An.DAL.Database;
using System.Collections.Generic; 

namespace Do_An.BLL
{
    public class DiemBLL
    {
        // Khai báo DAL chính xác
        private readonly DiemDAL dal = new DiemDAL();
        private readonly Database db = new Database();
        // ------------------- LOGIC MỚI: LỌC PHỤ THUỘC -------------------

        /// <summary>
        /// Lấy danh sách Khóa học (cho ComboBox Khóa học)
        /// </summary>
        public DataTable LayDanhSachKhoaHoc()
        {
            return dal.LayDanhSachKhoaHoc();
        }

        /// <summary>
        /// Lấy danh sách Môn học theo Khóa học (cho ComboBox Môn học)
        /// </summary>
        public DataTable LayMonHocTheoKhoaHoc(int maKhoaHoc)
        {
            return dal.LayMonHocTheoKhoaHoc(maKhoaHoc);
        }
        public DataTable LayDiemTheoLop(int maLop)
        {
            return dal.LayDiemTheoLop(maLop);
        }

        // ------------------- LOGIC CHÍNH: LẤY VÀ LỌC ĐIỂM -------------------

        /// <summary>
        /// Lấy toàn bộ điểm (hiển thị ban đầu)
        /// </summary>
        public DataTable LayTatCaDiem() 
        {
            return dal.LayTatCaDiem();
        }

        // FIX: Hàm LayDanhSachLopHoc (gọi DAL)
        /// <summary>
        /// Lấy danh sách lớp (dùng cho combobox)
        /// </summary>
        public DataTable LayDanhSachLopHoc()
        {
            var d = dal.LayDanhSachLopHoc();
            if (d == null) return null;

            // Xóa trùng dựa trên MaKH
            var distinctRows = d.AsEnumerable()
                                 .GroupBy(r => r.Field<int>("MaKH"))
                                 .Select(g => g.First())
                                 .CopyToDataTable();

            return distinctRows;
        }

        // FIX: Hàm LayDanhSachMonHoc (gọi DAL)
        /// <summary>
        /// Lấy danh sách môn học (dùng cho combobox)
        /// </summary>
        public DataTable LayDanhSachMonHoc()
        {
            return dal.LayDanhSachMonHoc();
        }

        /// <summary>
        /// Lọc điểm theo điều kiện (Sử dụng 4 tham số mới)
        /// </summary>
        public DataTable TimDiem(int? maLop, int? maKH, string keyword)
        {
            string sql = @"
        SELECT HV.MaHV, HV.HoTen, L.TenLop, KH.TenKH, 
               D.DiemGK, D.DiemCK, D.DiemTB
        FROM Diem D
        JOIN HocVien HV ON D.MaHV = HV.MaHV
        JOIN LopHoc L ON D.MaLop = L.MaLop
        JOIN KhoaHoc KH ON L.MaKH = KH.MaKH
        WHERE ( @MaLop IS NULL OR L.MaLop = @MaLop )
          AND ( @MaKH IS NULL OR KH.MaKH = @MaKH )
          AND ( @Keyword IS NULL OR HV.HoTen LIKE '%' + @Keyword + '%' OR HV.MaHV LIKE '%' + @Keyword + '%' )";

            var parameters = new Dictionary<string, object>
    {
        { "@MaLop", (object?)maLop ?? DBNull.Value },
        { "@MaKH", (object?)maKH ?? DBNull.Value },
        { "@Keyword", (object?)keyword ?? DBNull.Value }
    };

            return db.Execute(sql, parameters);
        }

    }
}