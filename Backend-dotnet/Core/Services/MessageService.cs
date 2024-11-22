using Backend_dotnet.Core.DbContext;
using Backend_dotnet.Core.DTOs.General;
using Backend_dotnet.Core.DTOs.Message;
using Backend_dotnet.Core.Entities;
using Backend_dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_dotnet.Core.Services
{
    public class MessageService (AppDbContext _context , ILogService _logService , UserManager<ApplicationUser> _userManager): IMessageService
    {
        public async Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal User, CreateMessageDto createMessageDto)
        { 
            
            if (User.Identity.Name == createMessageDto.ReceiverUserName)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 400,
                    Message = "Sender and Receiver cannot be same"
                };

            var isReceiverUserNameValid = _userManager.Users.Any(user=> user.UserName == createMessageDto.ReceiverUserName);

            if (!isReceiverUserNameValid)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 400,
                    Message = "Receiver UserName is Not Valid"
                };

            var Message = new Message
            {

                SenderUserName = User.Identity.Name,
                ReceiverUserName = createMessageDto.ReceiverUserName,
                Text = createMessageDto.Text

            };

            await _context.Messages.AddAsync(Message);
            await _context.SaveChangesAsync();
            await _logService.SaveNewLog(User.Identity.Name, $"Send Message To {createMessageDto.ReceiverUserName}");


            return new GeneralServiceResponseDto()
            {
                isSucceed = true,
                 Message= "Message Sent Successfully", 
                 StatusCode= 200
            };
        }

        public async Task<IEnumerable<GetMessageDto>> GetMessagesAsync()
        {
            
            var messages = await _context.Messages.Select(m=> new GetMessageDto 
            {
                Id = m.Id  , 
                SenderName = m.SenderUserName , 
                ReceiverName = m.ReceiverUserName ,
                Text = m.Text   , 
                CreatedAt = m.CreatedAt
           
            }
            ).OrderByDescending(m=> m.CreatedAt).ToListAsync();

            return messages; 
        }

        public async Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal User)
        {
            var messages= await  _context.Messages.Where(user=> user.SenderUserName==User.Identity.Name || user.ReceiverUserName == User.Identity.Name)
                                            .Select(message=> new GetMessageDto 
                                            { 
                                            
                                                  Id = message.Id , 
                                                  CreatedAt= message.CreatedAt, 
                                                  SenderName= message.SenderUserName ,
                                                  ReceiverName= message.ReceiverUserName ,  
                                                  Text= message.Text 
                                                  
                                            
                                            }).OrderByDescending(m=> m.CreatedAt).ToListAsync();

            return messages;
        }
    }
}
