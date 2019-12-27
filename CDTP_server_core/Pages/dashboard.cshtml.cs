using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace CDTP_server_core.Pages
{
    public class dashboardModel : PageModel
    {
        public bool state;
        public string msg;
        public string tablestr;

        public UserInfo userInfo;
        public void OnGet()
        {

            if (HttpContext.Session.IsAvailable && HttpContext.Session.TryGetValue("user" ,out  var test ) )
            {
                var keys = HttpContext.Session.Keys;

                userInfo = new UserInfo();
                userInfo.deviceID = HttpContext.Session.GetString( "deviceid");
                userInfo.name = HttpContext.Session.GetString("name");
                userInfo.surname = HttpContext.Session.GetString("surname");
                userInfo.username = HttpContext.Session.GetString("username");


 
                var table = Sql.Query($"select sum(cost),sum(freeusage)/sum(energyusage),extract(year from timestamp) as selected_year, extract(month from timestamp) as selected_month from usage where deviceid = '{userInfo.deviceID}' group by selected_year,selected_month order by selected_year,selected_month desc");

                string html = "<table class=\"table\">";
                //add header row
                html += "<tr>";
                html += "<th>#</th>";
                html += "<th>Fatura Tarihi</th>";
                html += "<th>Ücret</th>";
                html += "<th>Yenilenebilir Kullanım Oranı</th>";
                html += "</tr>";
                //add rows
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    int ratio = (int)((double)row[1] * 100);

                    var cost = (double)row[0];

                    html += "<tr>" +

                "<td>" +
                 i.ToString() +
                "</td>" +
                "<td>" +
                 row[2].ToString() + "/" + row[3].ToString() +
                "</td>" +
                "<td>" +
                 string.Format("{0:0.00}" ,cost )  + " birim" +
                "</td>" +
                "<td>" +
                "  <div class=\"progress\">" +
                $"    <div class=\"progress-bar bg-gradient-success\" role=\"progressbar\" style=\"width: {ratio}%\" aria-valuenow=\"{ratio}\" " +
                "    aria-valuemin=\"0\" aria-valuemax=\"100\"></div>" +
                "  </div>" +
                "</td>";

                }
                html += "</table>";

                tablestr = html;// Util.ConvertDataTableToHTML(table);


            }


        }
    }
}