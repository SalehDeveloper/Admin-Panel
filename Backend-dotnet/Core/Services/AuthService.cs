using Backend_dotnet.Core.Constants;
using Backend_dotnet.Core.DbContext;
using Backend_dotnet.Core.DTOs.Auth;
using Backend_dotnet.Core.DTOs.General;
using Backend_dotnet.Core.Entities;
using Backend_dotnet.Core.Interfaces;
using Backend_dotnet.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_dotnet.Core.Services
{
    public class AuthService(AppDbContext _context, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, ILogService _logService, IOptions<JwtHelper> _jwt ,IConfiguration _configuration) : IAuthService
    {
        public async Task<GeneralServiceResponseDto> SeedRoleAsync()
        {
            bool isOwnerRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isManagerRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.MANAGER);
            bool isUserRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isAdminRoleExist && isOwnerRoleExist && isManagerRoleExist && isUserRoleExist)
            {
                return new GeneralServiceResponseDto
                {
                    isSucceed = true,
                    Message = "Roles Seeding is already done",
                    StatusCode = 200
                };
            }

            // Create only the missing roles
            if (!isOwnerRoleExist)
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            if (!isUserRoleExist)
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

            if (!isManagerRoleExist)
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.MANAGER));

            if (!isAdminRoleExist)
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));

            return new GeneralServiceResponseDto
            {
                isSucceed = true,
                Message = "Roles Seeding Successfully Completed",
                StatusCode = 200
            };


        }

        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = await _userManager.FindByNameAsync(registerDto.UserName);

            if (user is not null)
                return new GeneralServiceResponseDto() { isSucceed = false, Message = "User Already Exist", StatusCode = 400 };

            var userToAdd = new ApplicationUser();
            //Fill user info 
            userToAdd.FirstName = registerDto.FirstName;
            userToAdd.LastName = registerDto.LastName;
            userToAdd.Email = registerDto.Email;
            userToAdd.UserName = registerDto.UserName;
            userToAdd.Adrress = registerDto.Address;
            userToAdd.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.CreateAsync(userToAdd, registerDto.Password);

            if (!result.Succeeded)
            {
                var errorString = "User Creation Failed because";
                foreach (var error in result.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new GeneralServiceResponseDto() { isSucceed = false, Message = errorString, StatusCode = 500 };
            }


            //Add a default USER Role to all users 
            await _userManager.AddToRoleAsync(userToAdd, StaticUserRoles.USER);
            await _logService.SaveNewLog(userToAdd.UserName, "Register To website");



            return new GeneralServiceResponseDto() { isSucceed = true, Message = "User Created Successfuly", StatusCode = 201 };






        }

        public async Task<LoginServiceResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var IsUserExist = await _userManager.FindByNameAsync(loginDto.UserName);

            if (IsUserExist is null)
                return null;

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(IsUserExist, loginDto.Password);


            if (!isPasswordCorrect)
                return null;


            var token = await CreateJWTToken(IsUserExist);
            var jwtSecurityToken = new JwtSecurityTokenHandler().WriteToken(token);

            var roles = await _userManager.GetRolesAsync(IsUserExist);
            var userInfo = GenerateUserInfo(IsUserExist, roles);

            await _logService.SaveNewLog(IsUserExist.UserName, "New Login");


            return new LoginServiceResponseDto() { UserInfo = userInfo, NewToken = jwtSecurityToken };







        }


        public async Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto)
        {
            // Only ADMIN and OWNER can Change the Roles of Others

            // Restricts of ADMIN:
            // -1 ADMIN can only assign the USER OR MANAGER To The Target User
            // -2 ADMIN Cannot change the Role of other ADMINs or OWNERs 

            //Restricts of OWNER : 
            // -1 OWNER cannot change Roles Of Other OWNERs

            var user = await _userManager.FindByNameAsync(updateRoleDto.UserName);
            if (user is null)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 404,
                    Message = "Invalid UserName"
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            // Just The OWNER and ADMIN can update roles
            if (User.IsInRole(StaticUserRoles.ADMIN))
            {
                // User is admin
                if (updateRoleDto.NewRole == RoleType.USER || updateRoleDto.NewRole == RoleType.MANAGER)
                {
                    // admin can change the role of everyone except for owners and admins
                    if (userRoles.Any(q => q.Equals(StaticUserRoles.OWNER) || q.Equals(StaticUserRoles.ADMIN)))
                    {
                        return new GeneralServiceResponseDto()
                        {
                            isSucceed = false,
                            StatusCode = 403,
                            Message = "You are not allowed to change role of this user"
                        };
                    }
                    else
                    {
                        await UpdateUserRole(user, updateRoleDto.NewRole); 
                        return new GeneralServiceResponseDto()
                        {
                            isSucceed = true,
                            StatusCode = 200,
                            Message = "Role updated successfully"
                        };
                    }
                }
                else return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 403,
                    Message = "You are not allowed to change role of this user"
                };
            }
            else if (User.IsInRole(StaticUserRoles.OWNER))
            {
                // user is owner
                if (userRoles.Any(q => q.Equals(StaticUserRoles.OWNER)))
                {
                    return new GeneralServiceResponseDto()
                    {
                        isSucceed = false,
                        StatusCode = 403,
                        Message = "You are not allowed to change role of this user"
                    };
                }
                else
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                    await _logService.SaveNewLog(user.UserName, "User Roles Updated");

                    return new GeneralServiceResponseDto()
                    {
                        isSucceed = true,
                        StatusCode = 200,
                        Message = "Role updated successfully"
                    };
                }
            }

            else
            {
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 403,
                    Message = "You do not have permission to change roles."
                };
            }

        }

        public async Task<LoginServiceResponseDto?> MeAsync(MeDto meDto)
        {
            if (string.IsNullOrWhiteSpace(meDto.Token))
            {
                throw new ArgumentException("Token cannot be null or empty.");
            }

            try
            {
                ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meDto.Token, new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]))
                }, out SecurityToken securityToken);

                string decodedUserName = handler.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Name)?.Value;

                if (decodedUserName is null)
                    return null;

                var user = await _userManager.FindByNameAsync(decodedUserName);
                if (user is null)
                    return null;

                var newToken = await CreateJWTToken(user);
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfo(user, roles);

                await _logService.SaveNewLog(user.UserName, "New Token Generated");

                return new LoginServiceResponseDto()
                {
                    NewToken = new JwtSecurityTokenHandler().WriteToken(newToken),
                    UserInfo = userInfo
                };
            }
            catch (SecurityTokenException ex)
            {
                // This will catch any token validation errors.
                throw new UnauthorizedAccessException("Token validation failed", ex);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }



        public async Task<UserInfoResult?> GetUserDetailsByUserName(string userName)
        {

            var user  = await _userManager.FindByNameAsync(userName); 

            if (user is not null)
            {
                var roles = await _userManager.GetRolesAsync(user);

               return GenerateUserInfo(user, roles);    
            }

            return null;
            
           
        }

        public async Task<IEnumerable<string>?> GetUserNamesAsync()
        {
            var users = await _userManager.Users.Select(user => user.UserName).ToListAsync();
            if (users is null)
                return null;
           
            return users;
        }

        public async Task<IEnumerable<UserInfoResult>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync(); 

            var userInfoResults  = new List<UserInfoResult>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfo(user , roles);
                userInfoResults.Add(userInfo);
            }

            return userInfoResults; 


        }

      

      
       
    


        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub , user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email , user.Email),
                 new Claim(ClaimTypes.Name, user.UserName),
                new Claim("uid" , user.Id)


            }.Union(userClaims)
             .Union(roleClaims);

            var symmatricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.key));
            var signingCredentials = new SigningCredentials(symmatricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken
                (
                  issuer: _jwt.Value.Issuer,
                  audience: _jwt.Value.Audience,
                  claims: claims,
                  expires: DateTime.Now.AddDays(_jwt.Value.DuartionInDays),
                  signingCredentials: signingCredentials



                );


            return jwtSecurityToken;


        }
        private  UserInfoResult GenerateUserInfo(ApplicationUser user, IEnumerable<string> roles)
        {
         
            return new UserInfoResult()
            {

                Id = user.Id,
                CreatedAt = DateTime.UtcNow,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Roles = roles

            };
        }

        private async Task UpdateUserRole(ApplicationUser user  , RoleType newRole)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, newRole.ToString());
            await _logService.SaveNewLog(user.UserName, "User Roles Updated");
        }

    }
}
