using System;
using System.Data;
using System.Collections.Generic;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class GiangVienBLL
    {
        private readonly GiangVienDAL dal = new GiangVienDAL();
        private readonly Database db = new Database();

        public DataTable LayThongTinGiangVien(string maGV)
        {
            if (string.IsNullOrWhiteSpace(maGV))
                throw new ArgumentException("Mã giảng viên không hợp lệ.");

            return dal.LayThongTinGiangVien(maGV);
        }

        public DataTable LayLichDay(string maGV)
        {
            if (string.IsNullOrWhiteSpace(maGV))
                throw new ArgumentException("Mã giảng viên không hợp lệ.");

            return dal.LayLichDay(maGV);
        }

        public DataTable LayHocVienTheoLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp không hợp lệ.");

            return dal.LayHocVienTheoLop(maLop);
        }

        public bool CapNhatDiem(string maHV, double diemGK, double diemCK)
        {
            if (string.IsNullOrWhiteSpace(maHV))
                throw new ArgumentException("Mã học viên không hợp lệ.");

            if (diemGK < 0 || diemGK > 10 || diemCK < 0 || diemCK > 10)
                throw new ArgumentException("Điểm phải nằm trong khoảng 0 - 10.");

            return dal.CapNhatDiem(maHV, diemGK, diemCK);
        }


        public int LayMaGVTheoTenDN(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                throw new ArgumentException("Tên đăng nhập không hợp lệ.");

            string sql = @"
        SELECT GV.MaGV
        FROM GiaoVien GV
        JOIN TaiKhoan TK ON GV.MaGV = TK.MaGV
        WHERE TK.TenDN = @TenDN";

            var parameters = new Dictionary<string, object>
    {
        { "@TenDN", tenDangNhap }
    };

            DataTable dt = db.Execute(sql, parameters);

            if (dt.Rows.Count > 0 && dt.Rows[0]["MaGV"] != DBNull.Value)
                return Convert.ToInt32(dt.Rows[0]["MaGV"]);

            return -1;
        }

    }
}
