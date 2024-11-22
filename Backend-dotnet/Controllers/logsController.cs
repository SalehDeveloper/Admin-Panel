using Backend_dotnet.Core.Constants;
using Backend_dotnet.Core.DbContext;
using Backend_dotnet.Core.DTOs.Log;
using Backend_dotnet.Core.Entities;
using Backend_dotnet.Core.Interfaces;
using Backend_dotnet.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class logsController(ILogService _service , AppDbContext context) : ControllerBase
    {
        [HttpGet]
        [Route("Logs")]
        [Authorize(Roles =StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<GetLogDto>> GetLogs ()
        {
                var logs = await _service.GetLogsAsync();
                
                return Ok(logs); 

        }



        [HttpGet]
        [Route("MyLogs")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLogs()
        {
            var logs = await _service.GetMyLogsAsync(User);

            return Ok(logs);    
        }



    }
}
