using System;
using System.Collections.Generic;
using System.Data;

namespace Do_An.DAL
{
    public class TaiKhoanDAL
    {
        private readonly Database db = new Database();

        // Lấy tất cả tài khoản
        public DataTable LayTatCaTaiKhoan()
        {
            string sql = @"SELECT 
    tk.TenDN,
    tk.MatKhau,
    tk.LoaiNguoiDung,
    tk.TrangThai,
    COALESCE(hv.HoTen, gv.HoTen, nv.HoTen, ql.HoTen) AS HoTen
FROM TaiKhoan tk
LEFT JOIN HocVien hv ON tk.MaHV = hv.MaHV
LEFT JOIN GiaoVien gv ON tk.MaGV = gv.MaGV
LEFT JOIN NhanVien nv ON tk.MaNV = nv.MaNV
LEFT JOIN QuanLy ql ON tk.MaQL = ql.MaQL;
";
            return db.Execute(sql);
        }
        public DataRow LayTaiKhoanTheoTenVaVaiTro(string tenDN, string vaiTro)
        {
            string sql = "SELECT * FROM TaiKhoan WHERE TenDN = @TenDN AND LoaiNguoiDung = @VaiTro";
            var parameters = new Dictionary<string, object>
    {
        { "@TenDN", tenDN },
        { "@VaiTro", vaiTro }
    };

            DataTable dt = db.Execute(sql, parameters);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        // Lấy thông tin chi tiết tài khoản
        public DataTable LayThongTinTaiKhoan(string tenDN)
        {
            string sql = @"
SELECT 
    TK.TenDN, 
    TK.MatKhau, 
    TK.LoaiNguoiDung, 
    TK.TrangThai,
    COALESCE(HV.HoTen, GV.HoTen, NV.HoTen, QL.HoTen, '') AS HoTen
FROM TaiKhoan TK
LEFT JOIN HocVien HV ON TK.MaHV = HV.MaHV
LEFT JOIN GiaoVien GV ON TK.MaGV = GV.MaGV
LEFT JOIN NhanVien NV ON TK.MaNV = NV.MaNV
LEFT JOIN QuanLy QL ON TK.MaQL = QL.MaQL
WHERE TK.TenDN = @TenDN";

            var parameters = new Dictionary<string, object>
            {
                {"@TenDN", tenDN}
            };

            return db.Execute(sql, parameters);
        }

        // Thêm tài khoản
        public bool ThemTaiKhoan(string tenDN, string hoTen, string matKhau, string loaiNguoiDung)
        {
            if (string.IsNullOrWhiteSpace(tenDN) || string.IsNullOrWhiteSpace(matKhau))
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống.");

            string sqlInsertNguoi;
            string idColumn;

            switch (loaiNguoiDung)
            {
                case "Học viên":
                    sqlInsertNguoi = "INSERT INTO HocVien (HoTen) OUTPUT INSERTED.MaHV VALUES (@HoTen)";
                    idColumn = "MaHV";
                    break;
                case "Giáo viên":
                    sqlInsertNguoi = "INSERT INTO GiaoVien (HoTen) OUTPUT INSERTED.MaGV VALUES (@HoTen)";
                    idColumn = "MaGV";
                    break;
                case "Nhân viên":
                    sqlInsertNguoi = "INSERT INTO NhanVien (HoTen) OUTPUT INSERTED.MaNV VALUES (@HoTen)";
                    idColumn = "MaNV";
                    break;
                case "Quản lý":
                    sqlInsertNguoi = "INSERT INTO QuanLy (HoTen) OUTPUT INSERTED.MaQL VALUES (@HoTen)";
                    idColumn = "MaQL";
                    break;
                default:
                    throw new Exception("Loại người dùng không hợp lệ");
            }

            // 1) Thêm người vào bảng tương ứng và lấy ID (kiểm tra kết quả)
            var paramNguoi = new Dictionary<string, object> { { "@HoTen", hoTen } };
            object maMoiObj = db.ExecuteScalar(sqlInsertNguoi, paramNguoi);
            if (maMoiObj == null || maMoiObj == DBNull.Value) return false;

            int maMoi;
            try { maMoi = Convert.ToInt32(maMoiObj); }
            catch { return false; }

            // 2) Thêm vào TaiKhoan, chỉ truyền duy nhất cột idColumn
            string sqlInsertTK = $@"
                   INSERT INTO TaiKhoan (TenDN, MatKhau, LoaiNguoiDung, {idColumn}, TrangThai)
                   VALUES (@TenDN, @MatKhau, @LoaiNguoiDung, @MaNguoi, 1)";

            var paramTK = new Dictionary<string, object>
    {
        { "@TenDN", tenDN },
        { "@MatKhau", matKhau },
        { "@LoaiNguoiDung", loaiNguoiDung },
        { "@MaNguoi", maMoi }
    };

            return db.ExecuteNonQuery(sqlInsertTK, paramTK) > 0;
        }



        // Cập nhật tài khoản
        public bool CapNhatTaiKhoan(string tenDN, string hoTen, string matKhauMoi, string loaiNguoiDung)
        {
            // 1️⃣ Lấy dữ liệu tài khoản hiện tại
            string sqlGet = "SELECT * FROM TaiKhoan WHERE TenDN = @TenDN";
            var dt = db.Execute(sqlGet, new Dictionary<string, object> { { "@TenDN", tenDN } });

            if (dt.Rows.Count == 0)
                return false;

            int? maHV = dt.Rows[0]["MaHV"] != DBNull.Value ? (int?)Convert.ToInt32(dt.Rows[0]["MaHV"]) : null;
            int? maGV = dt.Rows[0]["MaGV"] != DBNull.Value ? (int?)Convert.ToInt32(dt.Rows[0]["MaGV"]) : null;
            int? maNV = dt.Rows[0]["MaNV"] != DBNull.Value ? (int?)Convert.ToInt32(dt.Rows[0]["MaNV"]) : null;
            int? maQL = dt.Rows[0]["MaQL"] != DBNull.Value ? (int?)Convert.ToInt32(dt.Rows[0]["MaQL"]) : null;

            // 2️⃣ Nếu vai trò thay đổi → tạo bản ghi mới tương ứng
            int newId = 0;
            string sqlInsert = "";

            switch (loaiNguoiDung)
            {
                case "Học viên":
                    sqlInsert = "INSERT INTO HocVien(HoTen) OUTPUT INSERTED.MaHV VALUES(@HoTen)";
                    newId = db.ExecuteScalar<int>(sqlInsert, new Dictionary<string, object> { { "@HoTen", hoTen } });
                    break;

                case "Giáo viên":
                    sqlInsert = "INSERT INTO GiaoVien(HoTen) OUTPUT INSERTED.MaGV VALUES(@HoTen)";
                    newId = db.ExecuteScalar<int>(sqlInsert, new Dictionary<string, object> { { "@HoTen", hoTen } });
                    break;

                case "Nhân viên":
                    sqlInsert = "INSERT INTO NhanVien(HoTen) OUTPUT INSERTED.MaNV VALUES(@HoTen)";
                    newId = db.ExecuteScalar<int>(sqlInsert, new Dictionary<string, object> { { "@HoTen", hoTen } });
                    break;

                case "Quản lý":
                    sqlInsert = "INSERT INTO QuanLy(HoTen) OUTPUT INSERTED.MaQL VALUES(@HoTen)";
                    newId = db.ExecuteScalar<int>(sqlInsert, new Dictionary<string, object> { { "@HoTen", hoTen } });
                    break;
            }

            // 3️⃣ Reset toàn bộ mã cũ trong TaiKhoan
            string sqlReset = @"UPDATE TaiKhoan 
                        SET MaHV = NULL, MaGV = NULL, MaNV = NULL, MaQL = NULL 
                        WHERE TenDN = @TenDN";
            db.ExecuteNonQuery(sqlReset, new Dictionary<string, object> { { "@TenDN", tenDN } });

            // 4️⃣ Gán mã mới + vai trò mới
            string sqlUpdate = $@"UPDATE TaiKhoan 
                          SET LoaiNguoiDung = @LoaiNguoiDung, 
                              {(loaiNguoiDung == "Học viên" ? "MaHV" :
                                        loaiNguoiDung == "Giáo viên" ? "MaGV" :
                                        loaiNguoiDung == "Nhân viên" ? "MaNV" : "MaQL")} = @NewID
                          WHERE TenDN = @TenDN";

            db.ExecuteNonQuery(sqlUpdate, new Dictionary<string, object>
    {
        {"@LoaiNguoiDung", loaiNguoiDung},
        {"@TenDN", tenDN},
        {"@NewID", newId}
    });

            // 5️⃣ Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(matKhauMoi))
            {
                string sqlPass = "UPDATE TaiKhoan SET MatKhau = @MatKhau WHERE TenDN = @TenDN";
                db.ExecuteNonQuery(sqlPass, new Dictionary<string, object>
        {
            {"@MatKhau", matKhauMoi},
            {"@TenDN", tenDN}
        });
            }

            return true;
        }

        public int LayMaNVTheoTenDN(string tenDN)
        {
            string sql = "SELECT MaNV FROM TaiKhoan WHERE TenDN=@TenDN";
            var dt = db.Execute(sql, new Dictionary<string, object> { { "@TenDN", tenDN } });
            if (dt.Rows.Count == 0) return 0;
            return dt.Rows[0]["MaNV"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["MaNV"]) : 0;
        }
        public int LayMaQLTheoTenDN(string tenDN)
        {
            string sql = "SELECT MaQL FROM TaiKhoan WHERE TenDN=@TenDN";
            var dt = db.Execute(sql, new Dictionary<string, object> { { "@TenDN", tenDN } });
            if (dt.Rows.Count == 0) return 0;
            return dt.Rows[0]["MaQL"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["MaQL"]) : 0;
        }

        public bool XoaTaiKhoan(string tenDN)
        {
            string sql = "DELETE FROM TaiKhoan WHERE TenDN = @TenDN";
            var parameters = new Dictionary<string, object> { { "@TenDN", tenDN } };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool KhoaTaiKhoan(string tenDN) => CapNhatTrangThai(tenDN, "0");
        public bool MoKhoaTaiKhoan(string tenDN) => CapNhatTrangThai(tenDN, "1");

        public bool CapNhatTrangThai(string tenDN, string trangThaiMoi)
        {
            string sql = "UPDATE TaiKhoan SET TrangThai = @TrangThai WHERE TenDN = @TenDN";
            var parameters = new Dictionary<string, object>
            {
                { "@TrangThai", trangThaiMoi },
                { "@TenDN", tenDN }
            };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        public DataTable TimKiemTaiKhoan(string keyword)
        {
            string sql = @"
        SELECT 
            tk.TenDN,
            tk.MatKhau,
            tk.LoaiNguoiDung,
            tk.TrangThai,
            COALESCE(hv.HoTen, gv.HoTen, nv.HoTen, ql.HoTen) AS HoTen
        FROM TaiKhoan tk
        LEFT JOIN HocVien hv ON tk.MaHV = hv.MaHV
        LEFT JOIN GiaoVien gv ON tk.MaGV = gv.MaGV
        LEFT JOIN NhanVien nv ON tk.MaNV = nv.MaNV
        LEFT JOIN QuanLy ql ON tk.MaQL = ql.MaQL
        WHERE 
            tk.TenDN LIKE @Keyword 
            OR tk.LoaiNguoiDung LIKE @Keyword
            OR COALESCE(hv.HoTen, gv.HoTen, nv.HoTen, ql.HoTen, '') LIKE @Keyword;
    ";

            var parameters = new Dictionary<string, object>
    {
        { "@Keyword", "%" + keyword + "%" }
    };

            return db.Execute(sql, parameters);
        }


        public bool KiemTraTonTai(string tenDN)
        {
            string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDN = @TenDN";
            var parameters = new Dictionary<string, object> { { "@TenDN", tenDN } };
            var result = db.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }
    }
}
