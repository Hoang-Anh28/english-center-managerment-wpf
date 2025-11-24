using System.Data;
using Microsoft.Data.SqlClient;

namespace Do_An.DAL
{
    public class ThoiKhoaBieuDAL
    {
        // ✅ CHUỖI KẾT NỐI (thay Data Source nếu khác)
        private string conStr = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=QL_TrungTamAnhVan;Integrated Security=True;Encrypt=False";

        // ✅ LẤY THỜI KHÓA BIỂU THEO TUẦN
        public DataTable GetTKBByWeek(int week)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
        SELECT 
            tkb.Thu,
            lh.MaLop,
            mh.TenMH AS TenMonHoc,
            tkb.Gio AS GioHoc,
            tkb.Phong,
            gv.HoTen AS GiaoVien
        FROM ThoiKhoaBieu tkb
        JOIN LopHoc lh ON tkb.MonHoc = lh.TenLop
        JOIN MonHoc mh ON lh.MaMH = mh.MaMH
        JOIN GiaoVien gv ON lh.MaGV = gv.MaGV
        WHERE tkb.Tuan = @Tuan";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@Tuan", week);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
    }
}
