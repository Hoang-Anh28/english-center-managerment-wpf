using System;
using System.Collections.Generic;
using System.Data;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class PhanQuyenBLL
    {
        private readonly PhanQuyenDAL dal = new PhanQuyenDAL();

        // Lấy tất cả tài khoản
        public DataTable GetAll()
        {
            return dal.LayTatCa();
        }

        // Cập nhật danh sách quyền mới
        public void UpdatePhanQuyen(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string tenDN = row["TenDN"].ToString();
                string loaiNguoiDung = row["LoaiNguoiDung"].ToString();
                dal.CapNhatQuyen(tenDN, loaiNguoiDung);
            }
        }
    }
}
