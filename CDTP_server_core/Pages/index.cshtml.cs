using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CDTP_server_core.Pages
{
    public class indexModel : PageModel
    {
        public bool state;
        public string msg;

        public void OnGet()
        {
            if(HttpContext.Request.Cookies.TryGetValue("peki" , out var str))
            {
                if(DateTime.TryParse(str , out var othertime) && (DateTime.Now - othertime).TotalSeconds > 30)
                { 

                    msg = "login expired";
                    HttpContext.Response.Cookies.Delete("peki");
                }
                else
                {
                    msg = "login grandted";


                    HttpContext.Response.Cookies.Append("peki", DateTime.Now.ToString());
                state = true;
                }


            }
            else
            {
                msg = "not login";
                HttpContext.Response.Cookies.Append("peki", "yeni", new Microsoft.AspNetCore.Http.CookieOptions()
                { IsEssential = true }) ;
                state = false;
            }
        }
    }
}