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
 
            }else
            {
                HttpContext.Response.Redirect("login");
            }
 
        }
    }
}