using Backend_dotnet.Core.DTOs.Auth;
using Backend_dotnet.Core.DTOs.General;
using System.Security.Claims;

namespace Backend_dotnet.Core.Interfaces
{
    public interface IAuthService
    {


        Task<GeneralServiceResponseDto> SeedRoleAsync();

        Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<LoginServiceResponseDto?> LoginAsync( LoginDto loginDto);

        Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal user, UpdateRoleDto updateRoleDto);

        Task<LoginServiceResponseDto?> MeAsync(MeDto meDto);

        Task<IEnumerable<UserInfoResult>> GetUsersAsync();

        Task<UserInfoResult?> GetUserDetailsByUserName(string userName);

        Task<IEnumerable<string>?> GetUserNamesAsync(); 



    }
}
