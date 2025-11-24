using System;
using System.Data;
using Do_An.DAL;
using static Do_An.DAL.Database; // giữ nguyên

namespace Do_An.BLL
{
    public class LopHocBLL
    {
        private readonly LopHocDAL lopHocDAL = new LopHocDAL();

        // ------------------- LOGIC CŨ -------------------
        public DataTable LayDanhSachLopHoc() => lopHocDAL.LayTatCaLopHoc();
        public DataTable LayDanhSachMonHoc() => lopHocDAL.LayDanhSachMonHoc();
        public DataTable LayDanhSachGiaoVien() => lopHocDAL.LayDanhSachGiaoVien();

        public DataTable LayDanhSachLopTheoGiangVien(int maGV)
        {
            if (maGV <= 0)
                throw new ArgumentException("Mã giảng viên không hợp lệ.");

            return lopHocDAL.LayLopTheoGiangVien(maGV);
        }

        public string CapNhatTrangThai(int maLop, string trangThai)
        {
            if (maLop <= 0)
                return "Mã lớp không hợp lệ.";
            if (string.IsNullOrWhiteSpace(trangThai))
                return "Trạng thái không được để trống.";

            try
            {
                int result = lopHocDAL.CapNhatTrangThaiLop(maLop, trangThai);
                return result > 0 ? "Cập nhật trạng thái thành công!" : "Không thể cập nhật trạng thái lớp.";
            }
            catch (Exception ex)
            {
                return "Lỗi khi cập nhật trạng thái: " + ex.Message;
            }
        }

        public DataTable LayDanhSachLopHoc_Admin()
        {
            try
            {
                return lopHocDAL.LayThongTinLopHoc_DanhSach();
            }
            catch (MissingMethodException)
            {
                return lopHocDAL.LayTatCaLopHoc();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tải danh sách lớp học (Admin): " + ex.Message, ex);
            }
        }

        public int XoaLopHoc_Admin(int maLop)
        {
            if (maLop <= 0)
                throw new ArgumentException("Mã lớp không hợp lệ.");

            try
            {
                return lopHocDAL.XoaLopHoc(maLop);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa lớp học: " + ex.Message, ex);
            }
        }

        // ------------------- PHƯƠNG THỨC MỚI CHO MLH -------------------

        /// <summary>
        /// Tạo lớp học mới, xử lý nghiệp vụ, và gọi DAL
        /// Đã thêm tham số 'trinhDo' mới và đồng bộ với MLH
        /// </summary>
        public bool ThemLopHoc(string tenLop, string trinhDo, string phong, string thoiGian,
                                   int siSoToiDa, string trangThai, int maMH, int maKH)
        {
            int result = lopHocDAL.ThemLopHoc(tenLop, trinhDo, phong, thoiGian, siSoToiDa,
                                              trangThai, maMH, maKH);
            return result > 0;
        }

        // ✅ Giữ nguyên hoặc chỉnh lại cho đồng bộ
        public string TaoLopHoc(string tenLop, string trinhDo, string phong, string thoiGian,
                                int siSoToiDa, string trangThai, int maMH, int maKH)
        {
            try
            {
                bool ok = ThemLopHoc(tenLop, trinhDo, phong, thoiGian, siSoToiDa, trangThai, maMH, maKH);
                return ok ? "Tạo lớp học thành công!" : "Không thể thêm lớp học.";
            }
            catch (Exception ex)
            {
                return "Lỗi khi thêm lớp học: " + ex.Message;
            }
        }


        // Lấy danh sách khóa học (ComboBox Khóa học)
        public DataTable LayDanhSachKhoaHoc()
        {
            try
            {
                return lopHocDAL.LayDanhSachKhoaHoc();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tải danh sách khóa học: " + ex.Message, ex);
            }
        }

        // Lấy danh sách môn học theo khóa học (ComboBox Môn học)
        public DataTable LayMonHocTheoKhoaHoc(int maKhoaHoc)
        {
            if (maKhoaHoc <= 0) return new DataTable();
            return lopHocDAL.LayMonHocTheoKhoaHoc(maKhoaHoc);
        }
        public int LayMaMonHocTheoTen(int maKhoaHoc, string tenMonHoc)
        {
            return lopHocDAL.LayMaMonHocTheoTen(maKhoaHoc, tenMonHoc);
        }

    }
}
