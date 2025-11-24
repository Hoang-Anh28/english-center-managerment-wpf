using System;
using System.Collections.Generic;
using System.Data;
using Do_An.DAL;
using static Do_An.DAL.Database;

namespace Do_An.BLL
{
    internal class HocVienBLL
    {
        private readonly Database db = new Database();
        private readonly HocVienDAL hocVienDAL = new HocVienDAL(); // Tên DAL đã sửa lại

        // =======================================================
        // CÁC HÀM HỖ TRỢ COMBOBOX (FIX LỖI)
        // =======================================================

        // Fix lỗi 4: 'HocVienBLL' does not contain a definition for 'LayDanhSachKhoaHoc'
        public DataTable LayDanhSachKhoaHoc()
        {
            return hocVienDAL.LayDanhSachKhoaHoc();
        }

        // Fix lỗi 2: 'HocVienBLL' does not contain a definition for 'LayMonHocTheoKhoaHoc'
        public DataTable LayMonHocTheoKhoaHoc(int maKH)
        {
            if (maKH <= 0) return new DataTable();
            return hocVienDAL.LayMonHocTheoKhoaHoc(maKH);
        }

        // Fix lỗi 3: 'HocVienBLL' does not contain a definition for 'LayLopHocTheoMonHoc'
        public DataTable LayLopHocTheoMonHoc(int maKH,int maMH)
        {
            if (maMH <= 0) return new DataTable();
            return hocVienDAL.LayLopHocTheoMonHoc(maKH,maMH);
        }

        // =======================================================
        // LỌC LỚP THEO TRÌNH ĐỘ (Đã tối ưu ở các bước trước)
        // =======================================================
        public DataTable LayLopHocPhuHop(string trinhDo)
        {
            if (string.IsNullOrEmpty(trinhDo)) return new DataTable();
            return hocVienDAL.LayLopPhuHopVaConCho(trinhDo);
        }

        public DataTable LayTatCaLopTheoTrinhDo(string trinhDo)
        {
            if (string.IsNullOrEmpty(trinhDo)) return new DataTable();
            return hocVienDAL.LayTatCaLopTheoTrinhDo(trinhDo);
        }

        // =======================================================
        // HÀM CHÍNH: Đăng ký học viên VÀ xếp lớp
        // =======================================================

        /// <summary>
        /// Fix lỗi 1: No overload for method 'DangKyHocVienVaXepLop' takes 9 arguments
        /// </summary>
        /// <param name="cccd">Tham số CCCD mới được thêm.</param>
        public int DangKyHocVienVaXepLop(string hoTen, DateTime? ngaySinh, string gioiTinh,
                                          string diaChi, string sdt, string email,
                                          string trinhDo, int maLop, string cccd) // ĐÃ BỔ SUNG CCCD
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (string.IsNullOrEmpty(hoTen) || ngaySinh == null || string.IsNullOrEmpty(gioiTinh) || maLop <= 0)
                throw new Exception("Vui lòng nhập đầy đủ thông tin và chọn Lớp học.");

            // KIỂM TRA SĨ SỐ (Logic nghiệp vụ cốt lõi)
            int siSoHienTai = hocVienDAL.LaySiSoHienTaiCuaLop(maLop);
            int siSoToiDa = hocVienDAL.LaySiSoToiDaCuaLop(maLop);

            if (siSoToiDa == 0)
                throw new Exception("Lỗi cấu hình: Không tìm thấy thông tin sĩ số tối đa của lớp. Vui lòng kiểm tra lại MaLop.");

            if (siSoHienTai >= siSoToiDa)
            {
                throw new Exception("Lỗi nghiệp vụ: Lớp học đã đầy (" + siSoHienTai + "/" + siSoToiDa + "). Vui lòng chọn lớp khác.");
            }

            // 1️⃣ Thêm học viên vào bảng HocVien và lấy MaHV (ĐÃ THÊM CCCD VÀO INSERT)
            string sqlHV = @"
                INSERT INTO HocVien (HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email, TrinhDo, CCCD)
                OUTPUT INSERTED.MaHV
                VALUES (@HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @Email, @TrinhDo, @CCCD)";

            var parametersHV = new Dictionary<string, object>
            {
                { "@HoTen", hoTen },
                { "@NgaySinh", ngaySinh },
                { "@GioiTinh", gioiTinh },
                { "@DiaChi", string.IsNullOrEmpty(diaChi) ? DBNull.Value : (object)diaChi },
                { "@SDT", string.IsNullOrEmpty(sdt) ? DBNull.Value : (object)sdt },
                { "@Email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email },
                { "@TrinhDo", string.IsNullOrEmpty(trinhDo) ? DBNull.Value : (object)trinhDo },
                { "@CCCD", string.IsNullOrEmpty(cccd) ? DBNull.Value : (object)cccd } // THAM SỐ CCCD MỚI
            };

            DataTable result = db.Execute(sqlHV, parametersHV);
            if (result.Rows.Count == 0)
                throw new Exception("Không thể thêm hồ sơ học viên mới!");

            int maHV = Convert.ToInt32(result.Rows[0][0]);

            // 2️⃣ Tạo tài khoản học viên (Sử dụng code đã fix lỗi tên cột)
            string tenDN = "HV" + maHV;
            string matKhau = "123456";

            try
            {
                string sqlTK = @"
                    INSERT INTO TaiKhoan (TenDN, MatKhau, LoaiNguoiDung, MaHV)
                    VALUES (@TenDN, @MatKhau, N'Học viên', @MaHV)";

                var parametersTK = new Dictionary<string, object>
                {
                    { "@TenDN", tenDN },
                    { "@MatKhau", matKhau },
                    { "@MaHV", maHV }
                };

                db.ExecuteNonQuery(sqlTK, parametersTK);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi SQL khi tạo tài khoản. Chi tiết: " + ex.Message);
            }

            // 3️⃣ Đăng ký học viên vào lớp học
            bool dkSuccess = hocVienDAL.DangKyHocVienVaoLop(maHV, maLop);

            if (!dkSuccess)
            {
                throw new Exception("Đã tạo hồ sơ học viên nhưng không thể đăng ký vào lớp học.");
            }

            return maHV;
        }

        // =======================================================
        // HÀM CŨ: Chỉ tạo hồ sơ HV và Tài khoản (Giữ lại tính tương thích)
        // =======================================================

        /// <summary>
        /// PHIÊN BẢN CŨ ĐƯỢC KHÔI PHỤC: Chỉ tạo hồ sơ HV và Tài khoản (Không xếp lớp)
        /// </summary>
        public int DangKyHocVien(string hoTen, DateTime? ngaySinh, string gioiTinh,
                                          string diaChi, string sdt, string email, string trinhDo)
        {
            // Dùng hàm này nếu không cần CCCD và không cần xếp lớp
            if (string.IsNullOrEmpty(hoTen) || ngaySinh == null || string.IsNullOrEmpty(gioiTinh))
                throw new Exception("Vui lòng nhập đầy đủ Họ tên, Ngày sinh và Giới tính!");

            string sqlHV = @"
                INSERT INTO HocVien (HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email, TrinhDo)
                OUTPUT INSERTED.MaHV
                VALUES (@HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @Email, @TrinhDo)";

            var parametersHV = new Dictionary<string, object>
            {
                { "@HoTen", hoTen }, { "@NgaySinh", ngaySinh }, { "@GioiTinh", gioiTinh },
                { "@DiaChi", string.IsNullOrEmpty(diaChi) ? DBNull.Value : (object)diaChi },
                { "@SDT", string.IsNullOrEmpty(sdt) ? DBNull.Value : (object)sdt },
                { "@Email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email },
                { "@TrinhDo", string.IsNullOrEmpty(trinhDo) ? DBNull.Value : (object)trinhDo }
            };

            DataTable result = db.Execute(sqlHV, parametersHV);
            if (result.Rows.Count == 0)
                throw new Exception("Không thể thêm hồ sơ học viên mới!");
            int maHV = Convert.ToInt32(result.Rows[0][0]);

            // Tạo tài khoản
            string tenDN = "HV" + maHV;
            string matKhau = "123456";
            try
            {
                string sqlTK = @"
                    INSERT INTO TaiKhoan (TenDN, MatKhau, LoaiNguoiDung, MaHV)
                    VALUES (@TenDN, @MatKhau, N'Học viên', @MaHV)";
                var parametersTK = new Dictionary<string, object>
                {
                    { "@TenDN", tenDN }, { "@MatKhau", matKhau }, { "@MaHV", maHV }
                };
                db.ExecuteNonQuery(sqlTK, parametersTK);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi SQL khi tạo tài khoản. Chi tiết: " + ex.Message);
            }

            return maHV;
        }

        public int DemSoLuong()
        {
            string sql = "SELECT COUNT(*) FROM GiaoVien";
            object result = db.ExecuteScalar(sql);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public DataTable LayThongTinHocVien(int maHV)
        {
            return hocVienDAL.LayThongTinHocVien(maHV);
        }

        public DataTable LayThongTinHocVienVaLop(int maHV)
        {
            if (maHV <= 0) return new DataTable();
            return hocVienDAL.LayThongTinHocVien_VaLop(maHV);
        }

        public DataTable LayHocPhiTheoHocVien(int maHV)
        {
            string sql = "SELECT * FROM HocPhi WHERE MaHV = @MaHV";
            var parameters = new Dictionary<string, object> { { "@MaHV", maHV } };
            return db.Execute(sql, parameters);
        }
        public int LayMaHVTheoTenDN(string tenDN)
        {
            if (string.IsNullOrWhiteSpace(tenDN)) return 0;
            return hocVienDAL.LayMaHVTheoTenDN(tenDN);
        }
    }
}