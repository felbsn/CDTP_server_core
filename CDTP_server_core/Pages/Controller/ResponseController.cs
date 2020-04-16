using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDTP_server_core.Pages.Controller
{
    [Route("r")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        // GET: api/Response
        [HttpPost]
        public async Task Post()
        {

            if (Request.Form["username"].Count != 0)
            {

                string username = Request.Form["username"];
                string password = Request.Form["password"];


                var data = Sql.Query($"select username , id  , name , surname,deviceid   from customer where username = '{username}' and password='{password}'");

                if (data.Rows.Count == 0)
                {
                    Response.Clear();
                    Response.StatusCode = 404;
                    Response.Body.Flush();
                }
                else
                {

                    HttpContext.Session.SetString("user", "010");
                    HttpContext.Session.SetString("username", data.Rows[0][0] as string);
                    HttpContext.Session.SetString("id", ((int)data.Rows[0][1]).ToString());
                    HttpContext.Session.SetString("deviceid", ((int)data.Rows[0][4]).ToString());
                    HttpContext.Session.SetString("name", data.Rows[0][2] as string);
                    HttpContext.Session.SetString("surname", data.Rows[0][3] as string);

                }
            }


        }

        [HttpGet]
        public async Task Get()
        {
            if(HttpContext.Session.IsAvailable)
            {
                var deviceid = HttpContext.Session.GetString("deviceid");
                if (deviceid != null)
                {
 
                    string plotChartString = "";
                    string plotDataString = "";
                    string plotColorString = "";

 
                    var monthlyCostsUsagesTable = Sql.Query(string.Format(@"
                                                        select 
                                                        	sum(cost),
                                                        	sum(freeusage)/sum(energyusage),
                                                        	extract(year from timestamp) as selected_year,
                                                        	extract(month from timestamp) as selected_month ,
                                                        	count(*) as days
                                                        from usage 
                                                        where deviceid = '{0}'   
                                                        group by selected_year,selected_month 
                                                        order by selected_year desc,selected_month desc", deviceid));

                    string tableHtml = "<table class=\\\"table\\\">";
                    //add header row
                    tableHtml += "<tr>";
                    tableHtml += "<th>#</th>";
                    tableHtml += "<th>Fatura Tarihi</th>";
                    tableHtml += "<th>Gün Sayısı</th>";
                    tableHtml += "<th>Ücret</th>";
                    tableHtml += "<th>Yenilenebilir Kullanım Oranı</th>";
                    tableHtml += "</tr>";
                    //add rows

                    int startIndex = 0;
                    int year = (int)((double)monthlyCostsUsagesTable.Rows[0][2]);
                    int month = (int)((double)monthlyCostsUsagesTable.Rows[0][3]);
                    //if ( DateTime.Now.Year == year
                    //    &&
                    //    DateTime.Now.Month == month)
                    //{
                    //    startIndex++;
                    //}

                    for (int i = startIndex; i < monthlyCostsUsagesTable.Rows.Count; i++)
                    {
                        var row = monthlyCostsUsagesTable.Rows[i];
                        int ratio = (int)((double)row[1] * 100);

                        var cost = (double)row[0];

                        tableHtml += "<tr>" +

                    "<td>" +
                     i.ToString() +
                    "</td>" +
                    "<td>" +
                     row[2].ToString() + "/" + row[3].ToString() +
                    "</td>" +
                     "<td>" +
                      row[4].ToString() +
                    "</td>" +


                    "<td>" +
                     string.Format("{0:0.00}", cost) + " birim" +
                    "</td>" +
                    "<td>" +
                    // double escpa because json...
                    "  <div class=\\\"progress\\\">" +
                    $"    <div class=\\\"progress-bar bg-gradient-success\\\" role=\\\"progressbar\\\" style=\\\"width: {ratio}%\\\" aria-valuenow=\\\"{ratio}\\\" " +
                    "    aria-valuemin=\\\"0\\\" aria-valuemax=\\\"100\\\"></div>" +
                    "  </div>" +
                    "</td>";

                    }
                    tableHtml += "</table>";
               

                     
                    var table = Sql.Query("select timestamp,energyusage,freeusage from usage where deviceid='" + deviceid + "' order by timestamp DESC limit 30 ");
                    var sumtable = Sql.Query("select sum(energyusage),sum(freeusage) from usage where deviceid='" + deviceid + "' ");

                    double totalUsage = (double)sumtable.Rows[0][0];
                    double totalFree = (double)sumtable.Rows[0][1];
                    double weeklyUsage = 0;

                    if (table.Rows.Count > 0)
                    {
                        DateTime lastDate = (DateTime)table.Rows[0][0];
                        var days = DateTime.DaysInMonth(lastDate.Year, lastDate.Month);
                        var dailyUsages = new double[lastDate.Day + 1, 2];


                        int index = 0;
                        while (index <= table.Rows.Count && ((DateTime)table.Rows[index][0]).Month == lastDate.Month)
                        {
                            if (index < 7)
                            {
                                weeklyUsage += (double)table.Rows[index][1];
                            }

                            var day = ((DateTime)table.Rows[index][0]).Day;
                            dailyUsages[day, 0] = (double)table.Rows[index][1];
                            dailyUsages[day, 1] = (double)table.Rows[index][2];
                            index++;
                        }

                        var dayCount = 1;

                        var sb0 = new StringBuilder();
                        var sb1 = new StringBuilder();
                        var sb2 = new StringBuilder();

                        sb0.Append(dailyUsages[1, 0]);
                        sb1.Append(dailyUsages[1, 1]);
                        sb2.Append(dayCount.ToString());


                        for (int i = 2; i < dailyUsages.Length / 2; i++)
                        {
                            
                            sb0.Append(",");
                            sb0.Append(dailyUsages[i, 0]);
                            sb1.Append(",");
                            sb1.Append(dailyUsages[i, 1]);


                            dayCount++;

                            sb2.Append(",");
                            sb2.Append(dayCount.ToString());
                            
                            
                          
                            
                        }

                        var json = "{\"datasets\":[{\"label\":\"EnergyUsage\"," +
                        "\"backgroundColor\":\"rgba(210 , 60,60,0.2)\"," +
                        "\"borderColor\":\"rgba(210 ,110,110,0.8)\"," +
                        "\"data\":[" +
                         sb0.ToString() +
                        "]}," +
                        "{\"label\":\"My First dataset\"," +
                        "\"backgroundColor\":\"rgba(60 , 210,60,0.2)\"," +
                        "\"borderColor\":\"rgba(60 , 210,60,0.9)\"," +
                        "\"data\":[" +
                         sb1.ToString() +
                        "]}]," +
                        "\"labels\":[" +
                         sb2.ToString() +
                        "]," +
                        "\"extra\":{" +
                            "\"weeklyUsages\":" + weeklyUsage.ToString() + "," +
                            "\"totalUsages\":" + totalUsage.ToString() + "," +
                            "\"freeUsages\":" + totalFree.ToString() + "" +
                        "}," +
                        "\"date\":\"" +
                        lastDate.ToLongDateString() + "\"," +
                        "\"tablehtml\":\"" +
                        tableHtml +
                        "\"}";


                        Response.ContentType = "application/json";
                        //Response.Write("{\"test\":12}");
                        await Response.WriteAsync(json);
                   
                        Response.Body.Flush();
                        Response.Body.Close();

                    }
                     
                   // var tablestr = Util.ConvertDataTableToHTML(table);

                }else
                {
                    Response.ContentType = "application/json";
                    //Response.Write("{\"test\":12}");
                    await Response.WriteAsync("{test:12}");
                    Response.Body.Flush();
                    Response.Body.Close();
                }


            }
        }


        [HttpGet("logout")]
        public void Logout()
        {
            if(HttpContext.Session.IsAvailable)
            {
                HttpContext.Session.Clear();
            } 
            Response.Redirect("/login");
            Response.Body.Flush();
        }
    }
}
