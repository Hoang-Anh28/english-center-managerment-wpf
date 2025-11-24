using System;
using System.Collections.Generic;
using System.Data;

namespace Do_An.DAL
{
    public class PhanQuyenDAL
    {
        private readonly Database db = new Database();

        // Lấy toàn bộ danh sách tài khoản và loại người dùng (quyền)
        public DataTable LayTatCa()
        {
            string sql = @"SELECT 
    tk.TenDN,
    tk.MatKhau,
    tk.LoaiNguoiDung,
    tk.TrangThai,
    COALESCE(hv.HoTen, gv.HoTen, nv.HoTen, ql.HoTen) AS TenNguoiDung
FROM TaiKhoan tk
LEFT JOIN HocVien hv ON tk.MaHV = hv.MaHV
LEFT JOIN GiaoVien gv ON tk.MaGV = gv.MaGV
LEFT JOIN NhanVien nv ON tk.MaNV = nv.MaNV
LEFT JOIN QuanLy ql ON tk.MaQL = ql.MaQL;
";
            return db.Execute(sql);
        }

        // Cập nhật loại người dùng (quyền) cho tài khoản cụ thể
        public int CapNhatQuyen(string tenDN, string loaiNguoiDung)
        {
            string sql = @"UPDATE TaiKhoan 
                           SET LoaiNguoiDung = @LoaiNguoiDung 
                           WHERE TenDN = @TenDN";

            var parameters = new Dictionary<string, object>
            {
                {"@LoaiNguoiDung", loaiNguoiDung},
                {"@TenDN", tenDN}
            };

            return db.ExecuteNonQuery(sql, parameters);
        }

        // Lấy quyền (loại người dùng) của 1 tài khoản
        public string LayQuyenTheoTenDN(string tenDN)
        {
            string sql = @"SELECT LoaiNguoiDung FROM TaiKhoan WHERE TenDN = @TenDN";
            var parameters = new Dictionary<string, object> { { "@TenDN", tenDN } };

            DataTable dt = db.Execute(sql, parameters);
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["LoaiNguoiDung"].ToString();
            return null;
        }
    }
}
