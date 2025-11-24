using System;
using System.Data;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class TaiKhoanBLL
    {
        private readonly TaiKhoanDAL taiKhoanDAL = new TaiKhoanDAL();
        private readonly Database db = new Database();
                public enum LoaiNguoiDung
                {
                    KhongHopLe = 0,
                    SaiVaiTro = 1,
                    HocVien = 2,
                    GiaoVien = 3,
                    NhanVien = 4,
                    Admin = 5
                }

        public DataTable LayTatCaTaiKhoan() => taiKhoanDAL.LayTatCaTaiKhoan();

        public DataTable LayThongTinTaiKhoan(string tenDN)
        {
            if (string.IsNullOrWhiteSpace(tenDN))
                throw new ArgumentException("Tên đăng nhập không hợp lệ.");
            return taiKhoanDAL.LayThongTinTaiKhoan(tenDN);
        }

        public bool ThemTaiKhoan(string tenDN, string hoTen, string matKhau, string loaiNguoiDung)
        {
            if (string.IsNullOrWhiteSpace(tenDN) || string.IsNullOrWhiteSpace(matKhau))
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống.");

            if (taiKhoanDAL.KiemTraTonTai(tenDN))
                throw new Exception("Tên đăng nhập này đã tồn tại.");

            return taiKhoanDAL.ThemTaiKhoan(tenDN, hoTen, matKhau, loaiNguoiDung);
        }

        public bool CapNhatTaiKhoan(string tenDN, string hoTen, string matKhauMoi, string loaiNguoiDung)
        {
            if (string.IsNullOrWhiteSpace(tenDN))
                throw new ArgumentException("Tên đăng nhập không hợp lệ.");
            return taiKhoanDAL.CapNhatTaiKhoan(tenDN, hoTen, matKhauMoi, loaiNguoiDung);
        }

        public bool XoaTaiKhoan(string tenDN)
        {
            if (string.IsNullOrWhiteSpace(tenDN))
                throw new ArgumentException("Tên đăng nhập không hợp lệ.");
            return taiKhoanDAL.XoaTaiKhoan(tenDN);
        }

        public bool KhoaTaiKhoan(string tenDN) => taiKhoanDAL.KhoaTaiKhoan(tenDN);
        public bool MoKhoaTaiKhoan(string tenDN) => taiKhoanDAL.MoKhoaTaiKhoan(tenDN);
        public bool CapNhatTrangThai(string tenDN, string trangThaiMoi) => taiKhoanDAL.CapNhatTrangThai(tenDN, trangThaiMoi);

        public DataTable TimKiemTaiKhoan(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return LayTatCaTaiKhoan();
            return taiKhoanDAL.TimKiemTaiKhoan(keyword);
        }

        public LoaiNguoiDung KiemTraDangNhap(string tenDN, string matKhau, LoaiNguoiDung vaiTro)
        {
            string sql = "SELECT * FROM TaiKhoan WHERE TenDN = @TenDN AND LoaiNguoiDung = @VaiTro";
            var parameters = new Dictionary<string, object>
    {
        { "@TenDN", tenDN },
        { "@VaiTro", ChuyenEnumThanhChuoi(vaiTro) }
    };

            DataTable dt = db.Execute(sql, parameters);

            if (dt.Rows.Count == 0)
                return LoaiNguoiDung.KhongHopLe;
            // 🔹 Kiểm tra trạng thái (1 = hoạt động, 0 = bị khóa)
            DataRow row = dt.Rows[0];

            // 🔹 Kiểm tra trạng thái (1 = hoạt động, 0 = bị khóa)
            bool isActive = row["TrangThai"] != DBNull.Value && Convert.ToBoolean(row["TrangThai"]);
            if (!isActive)
            {
                // Có thể thêm thông báo cụ thể để xử lý ở GUI
                throw new Exception("Tài khoản này đã bị khóa. Vui lòng liên hệ quản lý để mở khóa.");
            }

            // 🔹 Kiểm tra mật khẩu
            string matKhauDB = row["MatKhau"].ToString();
            if (matKhauDB != matKhau)
                return LoaiNguoiDung.KhongHopLe;


            return vaiTro;
        }
      /*  public LoaiNguoiDung KiemTraDangNhap_QMK(string tenDN, string matKhau, LoaiNguoiDung vaiTro) 
        { string sql = "SELECT * FROM TaiKhoan WHERE TenDN = @TenDN AND LoaiNguoiDung = @VaiTro"; 
            var parameters = new Dictionary<string, object> 
            { 
                { "@TenDN", tenDN }, 
                { "@VaiTro", ChuyenEnumThanhChuoi(vaiTro) } };
            DataTable dt = db.Execute(sql, parameters); 

            if (dt.Rows.Count == 0)
                return LoaiNguoiDung.KhongHopLe; 

            if (!string.IsNullOrEmpty(matKhau) && dt.Rows[0]["MatKhau"].ToString() != matKhau) 
                return LoaiNguoiDung.KhongHopLe; return vaiTro; 
        }
      */
        private string ChuyenEnumThanhChuoi(LoaiNguoiDung role)
        {
            return role switch
            {
                LoaiNguoiDung.Admin => "Quản lý",
                LoaiNguoiDung.GiaoVien => "Giáo viên",
                LoaiNguoiDung.HocVien => "Học viên",
                LoaiNguoiDung.NhanVien => "Nhân viên",
                _ => ""
            };
        }
        /// <summary>
        /// Kiểm tra email có khớp với tên đăng nhập và vai trò hay không.
        /// Trả về true nếu tìm thấy bản ghi khớp (TenDN + LoaiNguoiDung + Email).
        /// </summary>
        public bool KiemTraEmail(string tenDN, LoaiNguoiDung vaiTroEnum, string email)
        {
            if (string.IsNullOrWhiteSpace(tenDN) || string.IsNullOrWhiteSpace(email))
                return false;

            // Chuyển enum sang chuỗi giống giá trị lưu trong CSDL
            string roleString = vaiTroEnum switch
            {
                LoaiNguoiDung.HocVien => "Học viên",
                LoaiNguoiDung.GiaoVien => "Giáo viên",
                LoaiNguoiDung.NhanVien => "Nhân viên",
                LoaiNguoiDung.Admin => "Quản lý",
                _ => ""
            };

            if (string.IsNullOrEmpty(roleString))
                return false;

            string sql = "";
            // Tham số dùng chung
            var parameters = new System.Collections.Generic.Dictionary<string, object>
    {
        { "@TenDN", tenDN },
        { "@Role", roleString },
        { "@Email", email }
    };

            // Tạo query theo vai trò (join vào bảng chi tiết chứa Email)
            if (vaiTroEnum == LoaiNguoiDung.HocVien)
            {
                sql = @"
            SELECT 1
            FROM TaiKhoan T
            INNER JOIN HocVien C ON T.MaHV = C.MaHV
            WHERE T.TenDN = @TenDN AND T.LoaiNguoiDung = @Role AND C.Email = @Email";
            }
            else if (vaiTroEnum == LoaiNguoiDung.GiaoVien)
            {
                sql = @"
            SELECT 1
            FROM TaiKhoan T
            INNER JOIN GiaoVien C ON T.MaGV = C.MaGV
            WHERE T.TenDN = @TenDN AND T.LoaiNguoiDung = @Role AND C.Email = @Email";
            }
            else if (vaiTroEnum == LoaiNguoiDung.NhanVien)
            {
                sql = @"
            SELECT 1
            FROM TaiKhoan T
            INNER JOIN NhanVien C ON T.MaNV = C.MaNV
            WHERE T.TenDN = @TenDN AND T.LoaiNguoiDung = @Role AND C.Email = @Email";
            }
            else if (vaiTroEnum == LoaiNguoiDung.Admin)
            {
                // Nếu bạn có bảng QuanLy (có Email)
                sql = @"
            SELECT 1
            FROM TaiKhoan T
            INNER JOIN QuanLy C ON T.MaQL = C.MaQL
            WHERE T.TenDN = @TenDN AND T.LoaiNguoiDung = @Role AND C.Email = @Email";
            }
            else
            {
                return false;
            }

            object result = db.ExecuteScalar(sql, parameters);
            if (result == null || result == DBNull.Value) return false;

            try
            {
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        public int DemSoLuongNhanVien()
        {
            string sql = "SELECT COUNT(*) FROM TaiKhoan WHERE LoaiNguoiDung = @Loai";
            var parameters = new Dictionary<string, object>
            {
                { "@Loai", "Nhân viên" }
            };

            object result = db.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result);
        }
        public int LayMaNVTheoTenDN(string tenDN)
        {
            if (string.IsNullOrWhiteSpace(tenDN)) return 0;
            return taiKhoanDAL.LayMaNVTheoTenDN(tenDN);
        }
        public int LayMaQLTheoTenDN(string tenDN)
        {
            if (string.IsNullOrWhiteSpace(tenDN)) return 0;
            return taiKhoanDAL.LayMaQLTheoTenDN(tenDN);
        }
    }
}
