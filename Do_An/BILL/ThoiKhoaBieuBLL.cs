using System.Data;
using Do_An.DAL;

namespace Do_An.BLL
{
    public class ThoiKhoaBieuBLL
    {
        ThoiKhoaBieuDAL dal = new ThoiKhoaBieuDAL();

        public DataTable LayTKBTheoTuan(int week)
        {
            return dal.GetTKBByWeek(week);
        }
    }
}
