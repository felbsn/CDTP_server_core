using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDTP_server_core.Pages
{
    [Route("sse")]
    [ApiController]
    public class SseController : ControllerBase
    {
        [HttpGet]
        public async Task Get(CancellationToken cancellationToken)
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");


            Task.Run(() =>
            {
                Task.Delay(1 * 2314).Wait();

                response
                     .WriteAsync($"data: From Task Controller x at {DateTime.Now}\r\r").Wait();

                response.Body.Flush();


            });


            for (var i = 0; true; ++i)
            {
                await response
                    .WriteAsync($"data: Controller {i} at {DateTime.Now}\r\r");

                response.Body.Flush();
                await Task.Delay(5 * 1000);
            }
        }
    }
}
