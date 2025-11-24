using System;
using System.Data;
using System.Collections.Generic;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class HocPhiBLL
    {
        private readonly Database dbAccess;

        public HocPhiBLL()
        {
            dbAccess = new Database();
        }

        // ==========================
        // LẤY TẤT CẢ HỌC PHÍ
        // ==========================
        public DataTable LayTatCaHocPhi()
        {
            string sql = "SELECT * FROM HocPhi";
            return dbAccess.Execute(sql);
        }
        public DataTable LayHocPhiTheoHocVien(int maHV)
        {
            string sql = "SELECT * FROM HocPhi WHERE MaHV = @MaHV";
            var parameters = new Dictionary<string, object> { { "@MaHV", maHV } };
            return dbAccess.Execute(sql, parameters);
        }
        // ==========================
        // THÊM HỌC PHÍ
        // ==========================
        public void ThemHocPhi(string maHV, string maLop, decimal soTien, decimal daDong, string phuongThuc, DateTime? ngayDong)
        {
            if (string.IsNullOrEmpty(maHV) || string.IsNullOrEmpty(maLop))
                throw new Exception("Mã học viên và Mã lớp không được để trống.");

            decimal conNo = soTien - daDong;

            string sql = @"INSERT INTO HocPhi (MaHV, MaLop, SoTien, DaDong, ConNo, PhuongThuc, NgayDong)
                           VALUES (@MaHV, @MaLop, @SoTien, @DaDong, @ConNo, @PhuongThuc, @NgayDong)";

            var parameters = new Dictionary<string, object>
            {
                {"@MaHV", maHV },
                {"@MaLop", maLop },
                {"@SoTien", soTien },
                {"@DaDong", daDong },
                {"@ConNo", conNo },
                {"@PhuongThuc", phuongThuc ?? "" },
                {"@NgayDong", (object)ngayDong ?? DBNull.Value }
            };

            dbAccess.ExecuteNonQuery(sql, parameters);
        }

        // ==========================
        // CẬP NHẬT HỌC PHÍ
        // ==========================
        public void CapNhatHocPhi(string maHP, string maHV, string maLop, decimal soTien, decimal daDong, string phuongThuc, DateTime? ngayDong)
        {
            if (string.IsNullOrEmpty(maHP))
                throw new Exception("Mã học phí không hợp lệ.");

            decimal conNo = soTien - daDong;

            string sql = @"UPDATE HocPhi
                           SET MaHV=@MaHV, MaLop=@MaLop, SoTien=@SoTien, DaDong=@DaDong, ConNo=@ConNo,
                               PhuongThuc=@PhuongThuc, NgayDong=@NgayDong
                           WHERE MaHP=@MaHP";

            var parameters = new Dictionary<string, object>
            {
                {"@MaHP", maHP },
                {"@MaHV", maHV },
                {"@MaLop", maLop },
                {"@SoTien", soTien },
                {"@DaDong", daDong },
                {"@ConNo", conNo },
                {"@PhuongThuc", phuongThuc ?? "" },
                {"@NgayDong", (object)ngayDong ?? DBNull.Value }
            };

            dbAccess.ExecuteNonQuery(sql, parameters);
        }

        // ==========================
        // XÓA HỌC PHÍ
        // ==========================
        public void XoaHocPhi(string maHP)
        {
            if (string.IsNullOrEmpty(maHP))
                throw new Exception("Mã học phí không hợp lệ.");

            string sql = "DELETE FROM HocPhi WHERE MaHP=@MaHP";
            var parameters = new Dictionary<string, object> { { "@MaHP", maHP } };

            dbAccess.ExecuteNonQuery(sql, parameters);
        }
    }
}
