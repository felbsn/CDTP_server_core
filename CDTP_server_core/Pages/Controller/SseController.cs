using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CDTP_server_core.Pages
{
    [Route("sse")]
    [ApiController]
    public class SseController : ControllerBase
    {
        [HttpGet]
        public async Task Get(CancellationToken cancellationToken)
        {
 
            Response.Headers.Add("Content-Type", "text/event-stream");
            var deviceid = HttpContext.Session.GetString("deviceid");

            if(deviceid != null)
            {
                NotificationEventHandler notificationEventHandler = (s, d) =>
                {
                    if (deviceid == d.Payload.Split(',')[1])
                    {
                        Response.WriteAsync($"data: Controller at {DateTime.Now}\r\r");

                        Response.Body.Flush();
                    }
                };

                Sql.notify_conn.Notification += notificationEventHandler;



                cancellationToken.WaitHandle.WaitOne();

                Sql.notify_conn.Notification -= notificationEventHandler;
            }


   

 
        }
    }
}
