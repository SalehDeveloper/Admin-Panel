using Backend_dotnet.Core.Constants;
using Backend_dotnet.Core.DTOs.General;
using Backend_dotnet.Core.DTOs.Message;
using Backend_dotnet.Core.Interfaces;
using Backend_dotnet.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(IMessageService _service) : ControllerBase
    {

        [HttpGet]
        [Route("MyMessages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMyMessages()
        {

            var messages = await _service.GetMyMessagesAsync(User);

            if (!messages.Any())
                return Ok("No Messages Found");

            return Ok(messages);

        }


        [HttpGet]
        [Route("Messages")]
        [Authorize (Roles =StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMeesages()
        {

            var messages =await  _service.GetMessagesAsync();
            if (!messages.Any())
                return Ok("No Messages Found");

            return Ok(messages);    
        }

        [HttpPost]
        [Route("SendMessage")]
        [Authorize]
        public async Task<IActionResult> SendMessage ([FromBody]CreateMessageDto message)
        {
            var result  = await _service.CreateNewMessageAsync(User , message); 

            if (result.isSucceed)
                return Ok(result.Message);
            return StatusCode(result.StatusCode, result.Message);
           
        }



    }
}
