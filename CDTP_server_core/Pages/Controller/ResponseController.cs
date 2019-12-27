using System;
using System.Collections.Generic;
using System.Linq;
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
                    //UserInfo user = new UserInfo();
                    //
                    //
                    //user.username = data.Rows[0][0] as string;
                    //user.id = (int)data.Rows[0][1];
                    //user.name = data.Rows[0][2] as string;
                    //user.surname = data.Rows[0][3] as string;


                    HttpContext.Session.SetString("user", "010");
                    HttpContext.Session.SetString("username", data.Rows[0][0] as string);
                    HttpContext.Session.SetString("id", ((int)data.Rows[0][1]).ToString());
                    HttpContext.Session.SetString("deviceid", ((int)data.Rows[0][4]).ToString());
                    HttpContext.Session.SetString("name", data.Rows[0][2] as string);
                    HttpContext.Session.SetString("surname", data.Rows[0][3] as string);

 
           
                    //Response.Write(data);
                    // Response.Redirect("dashboard.aspx");
                }
            }


        }
    }
}
