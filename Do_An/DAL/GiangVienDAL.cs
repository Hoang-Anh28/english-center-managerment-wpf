using System;
using System.Data;
using System.Collections.Generic;

namespace Do_An.DAL
{
    public class GiangVienDAL
    {
        private readonly Database db = new Database();

        // 1. Lấy thông tin giảng viên (trang chủ)
        public DataTable LayThongTinGiangVien(string maGV)
        {
            string sql = "SELECT * FROM GiaoVien WHERE MaGV = @MaGV";
            var param = new Dictionary<string, object> { { "@MaGV", maGV } };
            return db.Execute(sql, param);
        }

        // 2. Lấy lịch dạy (các lớp do GV phụ trách)
        public DataTable LayLichDay(string maGV)
        {
            string sql = "SELECT MaLop, TenLop, Phong, ThoiGian, TrangThai FROM LopHoc WHERE MaGV = @MaGV";
            var param = new Dictionary<string, object> { { "@MaGV", maGV } };
            return db.Execute(sql, param);
        }

        // 3. Lấy danh sách học viên trong lớp
        public DataTable LayHocVienTheoLop(string maLop)
        {
            string sql = @"
                SELECT HV.MaHV, HV.HoTen, D.DiemGK, D.DiemCK, D.DiemTB
                FROM HocVien HV
                JOIN DangKy DK ON HV.MaHV = DK.MaHV
                LEFT JOIN Diem D ON DK.MaHV = D.MaHV AND DK.MaLop = D.MaLop
                WHERE DK.MaLop = @MaLop";
            var param = new Dictionary<string, object> { { "@MaLop", maLop } };
            return db.Execute(sql, param);
        }

        // 4. Cập nhật điểm học viên
        public bool CapNhatDiem(string maHV, double diemGK, double diemCK)
        {
            string sql = @"
        IF EXISTS (SELECT 1 FROM Diem WHERE MaHV = @MaHV)
            UPDATE Diem SET DiemGK = @GK, DiemCK = @CK WHERE MaHV = @MaHV
        ELSE
            INSERT INTO Diem (MaHV, DiemGK, DiemCK) VALUES (@MaHV, @GK, @CK)";
            var param = new Dictionary<string, object>
    {
        { "@MaHV", maHV },
        { "@GK", diemGK },
        { "@CK", diemCK }
    };
            return db.ExecuteNonQuery(sql, param) > 0;
        }

    }
}
