using Backend_dotnet.Core.DbContext;
using Backend_dotnet.Core.DTOs.Log;
using Backend_dotnet.Core.Entities;
using Backend_dotnet.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Security.Claims;

namespace Backend_dotnet.Core.Services
{
    public class LogService (AppDbContext _context) : ILogService
    {

        public async Task SaveNewLog(string UserName, string Description)
        {
            var newLog = new Log()
            {


                UserName = UserName,
                Description = Description
            };

            await _context.Logs.AddAsync(newLog);
            await _context.SaveChangesAsync();  
        }
      
        
        public async Task<IEnumerable<GetLogDto>> GetLogsAsync()
        {
            var logs = await _context.Logs.Select(log => new GetLogDto
            {
                CreatedAt = log.CreatedAt,
                Description = log.Description,
                UserName = log.UserName



            }).OrderByDescending(log=> log.CreatedAt).ToListAsync();


            return logs;
        }



        public async Task<IEnumerable<GetLogDto>> GetMyLogsAsync(ClaimsPrincipal User)
        {

            var currentusername = User.Identity.Name;
            var logs = await _context.Logs
          .Where(q => q.UserName == User.Identity.Name)
         .Select(q => new GetLogDto
         {
             CreatedAt = q.CreatedAt,
             Description = q.Description,
             UserName = q.UserName,
         })
         .OrderByDescending(q => q.CreatedAt)
         .ToListAsync();
            return logs;
        }

       
    }
}
