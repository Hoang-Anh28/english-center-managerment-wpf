using System.Data;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class LichThiBLL
    {
        private readonly Database db = new Database();

        public DataTable LayLichThi(int maHV)
        {
            string sql = $@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY NgayThi) AS STT,
                    MonThi,
                    CONVERT(varchar, NgayThi, 103) AS NgayThi,
                    GioThi,
                    PhongThi,
                    GhiChu
                FROM LichThi
                WHERE MaHV = {maHV}";
            return db.ExecuteQuery(sql);
        }
    }
}
