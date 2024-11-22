using Backend_dotnet.Core.DTOs.General;
using Backend_dotnet.Core.DTOs.Message;
using System.Security.Claims;

namespace Backend_dotnet.Core.Interfaces
{
    public interface IMessageService
    { 
        Task<GeneralServiceResponseDto> CreateNewMessageAsync ( ClaimsPrincipal User , CreateMessageDto createMessageDto );

        Task<IEnumerable<GetMessageDto>> GetMessagesAsync();

        Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal User);
    }
}
