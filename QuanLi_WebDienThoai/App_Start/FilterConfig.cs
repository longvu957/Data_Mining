using System.Web;
using System.Web.Mvc;

namespace QuanLi_WebDienThoai
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
