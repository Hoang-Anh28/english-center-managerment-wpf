using System.Collections.Generic;
using System.Data;

namespace Do_An.DAL
{
    public class HocPhiDALL : Database
    {
        public DataTable LayHocPhiTheoHocVien(string maHV)
        {
            string query = "SELECT MaHV, KhoaHoc, SoTien, NgayDong FROM HocPhi WHERE MaHV = @MaHV";
            var parameters = new Dictionary<string, object>
            {
                { "@MaHV", maHV }
            };

            return Execute(query, parameters);
        }
    }
}
