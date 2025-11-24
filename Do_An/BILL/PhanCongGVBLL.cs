using System;
using System.Data;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class PhanCongBLL
    {
        private readonly PhanCongDAL dal = new PhanCongDAL();

        /// <summary>
        /// Lấy danh sách phân công với thông tin đầy đủ
        /// </summary>
        public DataTable LayDanhSachPhanCong()
        {
            return dal.LayDanhSachPhanCong();
        }

        /// <summary>
        /// Lấy danh sách lớp học
        /// Nếu muốn hiển thị ComboBox full tên: Khóa - Môn - Lớp, bind cột TenLopFull
        /// </summary>
        public DataTable LayDanhSachLop()
        {
            return dal.LayDanhSachLop();
        }

        /// <summary>
        /// Lấy danh sách giáo viên
        /// </summary>
        public DataTable LayDanhSachGiaoVien()
        {
            return dal.LayDanhSachGiaoVien();
        }

        /// <summary>
        /// Thêm phân công
        /// </summary>
        public bool ThemPhanCong(int maLop, int maGV, DateTime ngayPhanCong, string ghiChu)
        {
            return dal.ThemPhanCong(maLop, maGV, ngayPhanCong, ghiChu);
        }

        /// <summary>
        /// Sửa phân công
        /// </summary>
        public bool SuaPhanCong(int maPC, int maLop, int maGV, DateTime ngayPhanCong, string ghiChu)
        {
            return dal.SuaPhanCong(maPC, maLop, maGV, ngayPhanCong, ghiChu);
        }

        /// <summary>
        /// Xóa phân công
        /// </summary>
        public bool XoaPhanCong(int maPC)
        {
            return dal.XoaPhanCong(maPC);
        }

        /// <summary>
        /// Đếm số lượng giáo viên
        /// </summary>
        public int DemSoLuongGiaoVien()
        {
            return dal.DemSoLuongGiaoVien();
        }
        public DataTable LayThongTinMonHocVaKhoaHoc(int maMH)
        {
            return dal.LayThongTinMonHocVaKhoaHoc(maMH);
        }

    }
}
