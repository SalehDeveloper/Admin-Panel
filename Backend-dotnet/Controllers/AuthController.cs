using Backend_dotnet.Core.Constants;
using Backend_dotnet.Core.DTOs.Auth;
using Backend_dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (IAuthService _service): ControllerBase
    {

        [HttpPost]
        [Route("SeedRoles")]
        public async Task<IActionResult> SeedRoles ()
        {
            var result = await _service.SeedRoleAsync();

            if (result.isSucceed)
                return Ok(result.Message);

            return StatusCode(result.StatusCode, result.Message); 
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register ([FromBody]RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result  = await _service.RegisterAsync(dto);    

            if (result.isSucceed)
                return Ok(result.Message);
            return StatusCode(result.StatusCode, result.Message);

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginServiceResponseDto>> Login ([FromBody]LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result  =await  _service.LoginAsync(dto);
            if (result is null)
                return Unauthorized("Invalid Credantials"); 
            return Ok(result);


        }



        //An Owner can Change everything expect for other owners 
        // Admin can change just user to manager or reverse 
        //user - manager cannot use this endpoint 
        [HttpPut]
        [Route("UpdateRole")]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<IActionResult> UpdateRole ([FromBody]UpdateRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result  = await _service.UpdateRoleAsync(User , dto);

            if(result.isSucceed)
                return Ok(result.Message);

            return StatusCode(result.StatusCode, result.Message);


        }



        //Getting Data of user from its JWT 
        [HttpGet]
        [Route("me")]
        public async Task<ActionResult<LoginServiceResponseDto>> Me ([FromQuery]MeDto dto)
        {



            try
            {
                var me = await _service.MeAsync(dto);

                if (me is not null)
                    return Ok(me);
                else
                {
                    return Unauthorized("Invalid Token");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized($"Invalid Token: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid Request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }


        }


        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUsers()
        {
            var users = await _service.GetUsersAsync();

            if (users is null)
                return Ok("No Users found");

            return Ok(users);   
        }

        [HttpGet]
        [Route("UserByName")]
        public async Task<ActionResult<UserInfoResult>> GetUserByName (string userName)
        {
            var user  =await _service.GetUserDetailsByUserName(userName);

            if (user is null)
                return Ok("user No Found");

            return Ok(user);
        }

       [HttpGet]
       [Route("UserNames")]
       public async Task<ActionResult<IEnumerable<UserInfoResult>>>GetUserNames ()
        {
            var names = await _service.GetUserNamesAsync();

            if (names is null)
                return Ok("No names availale");

            return Ok(names);
        }


    }
}
