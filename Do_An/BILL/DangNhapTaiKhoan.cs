using System.Data;
using Do_An.DAL;
using System.Collections.Generic;

namespace Do_An.BLL
{
    public enum LoaiNguoiDung
    {
        KhongHopLe, // Tên DN/MK sai
        HocSinh,
        GiaoVien,
        NhanVien,
        QuanLy,
        SaiVaiTro
    }

    public class TaiKhoan
    {
        private Database dbAccess;

        public TaiKhoan()
        {
            dbAccess = new Database();
        }

        public LoaiNguoiDung KiemTraDangNhap(string username, string password, string expectedRole)
        {
            // Đã sửa: Thay COLLATE Latin1_General_CS_AS bằng Latin1_General_CI_AS
            string sql = @"
            SELECT LoaiNguoiDung 
            FROM TaiKhoan 
            WHERE RTRIM(LTRIM(TenDN)) COLLATE Latin1_General_CI_AS = @Username 
            AND RTRIM(LTRIM(MatKhau)) COLLATE Latin1_General_CI_AS = @Password";

            var parameters = new Dictionary<string, object>
    {
        { "@Username", username },
        { "@Password", password }
    };

            DataTable dt = dbAccess.Execute(sql, parameters);

            if (dt.Rows.Count == 0)
                return LoaiNguoiDung.KhongHopLe;

            // ---- Lấy vai trò thực tế trong SQL ----
            string actualRole = dt.Rows[0]["LoaiNguoiDung"].ToString().Trim();

            // ---- Chuẩn hóa vai trò để tránh lỗi dấu cách, khác dấu, hoa/thường ----
            string normalizedActual = NormalizeRole(actualRole);
            string normalizedExpected = NormalizeRole(expectedRole);

            if (normalizedActual == normalizedExpected)
            {
                return actualRole switch
                {
                    "Học viên" => LoaiNguoiDung.HocSinh,
                    "Giáo viên" => LoaiNguoiDung.GiaoVien,
                    "Nhân viên" => LoaiNguoiDung.NhanVien,
                    "Quản lý" => LoaiNguoiDung.QuanLy,
                    _ => LoaiNguoiDung.KhongHopLe
                };
            }
            return LoaiNguoiDung.SaiVaiTro;
        }

        // === HÀM PHỤ: CHUẨN HÓA CHUỖI ===
        // Giúp so sánh không phân biệt hoa thường, dấu cách, dấu tiếng Việt
        private string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "";
            role = role.Trim().ToLower();

            // Bỏ dấu tiếng Việt
            string normalized = role.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in normalized)
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
        public LoaiNguoiDung KiemTraTaiKhoanTheoTenDN(string username, string expectedRole)
        {
            string sql = "SELECT LoaiNguoiDung FROM TaiKhoan WHERE TenDN=@Username";
            var parameters = new Dictionary<string, object> { { "@Username", username } };
            DataTable dt = dbAccess.Execute(sql, parameters);

            if (dt.Rows.Count == 0)
                return LoaiNguoiDung.KhongHopLe;

            string actualRole = dt.Rows[0]["LoaiNguoiDung"].ToString().Trim();
            if (actualRole.Equals(expectedRole, StringComparison.OrdinalIgnoreCase))
            {
                return actualRole switch
                {
                    "Học viên" => LoaiNguoiDung.HocSinh,
                    "Giáo viên" => LoaiNguoiDung.GiaoVien,
                    "Nhân viên" => LoaiNguoiDung.NhanVien,
                    "Quản lý" => LoaiNguoiDung.QuanLy,
                    _ => LoaiNguoiDung.KhongHopLe
                };
            }
            return LoaiNguoiDung.SaiVaiTro;
        }
        // BỔ SUNG: HÀM MỚI CHO QUÊN MẬT KHẨU BƯỚC 2: KIỂM TRA EMAIL
        /// <summary>
        /// Kiểm tra xem Email có khớp với Tên đăng nhập và Vai trò đã xác nhận hay không.
        /// </summary>
        public bool KiemTraEmail(string tenDN, string userRole, string email)
        {
            string sql = "";
            string joinTable = "";
            string joinColumn = "";

            // 1. Xác định bảng chi tiết (HocVien/GiaoVien)
            if (userRole.Equals("Học viên", StringComparison.OrdinalIgnoreCase))
            {
                joinTable = "HocVien";
                joinColumn = "MaHV";
            }
            else if (userRole.Equals("Giáo viên", StringComparison.OrdinalIgnoreCase))
            {
                joinTable = "GiaoVien";
                joinColumn = "MaGV";
            }
            else if (userRole.Equals("Nhân viên", StringComparison.OrdinalIgnoreCase))
            {
                joinTable = "NhanVien";
                joinColumn = "MaNV";
            }
            // CHÚ THÍCH: Nếu Quản lý không có bảng chi tiết chứa email, bạn cần xử lý riêng
            else if (userRole.Equals("Quản lý", StringComparison.OrdinalIgnoreCase))
            {
                // GIẢ ĐỊNH: Quản lý không cần kiểm tra email qua JOIN, hoặc Email nằm trong bảng TaiKhoan (Nếu TaiKhoan có cột Email)
                // Vì CSDL mẫu bạn gửi không có email cho Quản lý, ta tạm trả về false cho Quản lý
                return false;
            }
            else
            {
                return false; // Vai trò không được hỗ trợ
            }

            // 2. Tạo câu lệnh SQL an toàn (Parameterized Query)
            sql = $@"
                SELECT T.TenDN
                FROM TaiKhoan T 
                INNER JOIN {joinTable} C ON T.{joinColumn} = C.{joinColumn}
                WHERE T.TenDN = @TenDN AND T.LoaiNguoiDung = @Role AND C.Email = @Email";

            var parameters = new Dictionary<string, object>
            {
                { "@TenDN", tenDN },
                { "@Role", userRole },
                { "@Email", email }
            };

            // 3. Gọi DAL để lấy dữ liệu
            DataTable dt = dbAccess.Execute(sql, parameters);

            // 4. Kiểm tra kết quả
            return dt.Rows.Count > 0; // Trả về true nếu tìm thấy bản ghi khớp
        }
        /// <summary>
        /// Cập nhật mật khẩu mới cho tài khoản.
        /// </summary>
        /// <param name="tenDN">Tên đăng nhập cần đổi mật khẩu.</param>
        /// <param name="newPassword">Mật khẩu mới (chưa mã hóa).</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại.</returns>
        public bool UpdateMatKhau(string tenDN, string newPassword)
        {
            // CHÚ THÍCH: Trong thực tế, bạn nên mã hóa mật khẩu ở đây (ví dụ: SHA-256 Hash)
            // Hiện tại, ta sử dụng mật khẩu thô như trong CSDL mẫu của bạn.

            string sql = "UPDATE TaiKhoan SET MatKhau = @NewPassword WHERE RTRIM(LTRIM(TenDN)) COLLATE Latin1_General_CS_AS = @TenDN";

            var parameters = new Dictionary<string, object>
        {
            { "@NewPassword", newPassword },
            { "@TenDN", tenDN }
        };

            // CHÚ THÍCH: Hàm ExecuteNonQuery trong DAL cần trả về số dòng bị ảnh hưởng
            int rowsAffected = dbAccess.ExecuteNonQuery(sql, parameters);

            return rowsAffected > 0;
        }
    }
}
