using ARID.Servicces.AuthAPI.Models;
using ARID.Servicces.AuthAPI.Service.IService;
using ARID.Services.AuthAPI.Data;
using ARID.Services.AuthAPI.Models.Dto;
using ARID.Services.AuthAPI.Service.IService;
using Azure;
using Microsoft.AspNetCore.Identity;

namespace ARID.Servicces.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUser.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<bool> Delete(string id)
        {
            var user = _db.ApplicationUser.FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }
            return false;
        }

        public async Task<string> Edit(RegistrationRequestDto registrationRequestDto)
        {
            var user = _db.ApplicationUser.FirstOrDefault(a => a.Email == registrationRequestDto.Email);
            if (user != null)
            {

                user.UserName = registrationRequestDto.Email;
                user.Email = registrationRequestDto.Email;
                user.NormalizedEmail = registrationRequestDto.Email.ToUpper();
                user.Name = registrationRequestDto.Name;
                user.PhoneNumber = registrationRequestDto.PhoneNumber;


                try
                {
                    _db.ApplicationUser.Update(user);
                    _db.SaveChanges();
                    return "";
                }
                catch (Exception ex)
                {

                }
            }
            return "Error Encountered";
        }


        public async Task<ApplicationUser> GetByEmail(string email)
        {
            ApplicationUser user =new ApplicationUser();
             user = _db.ApplicationUser.FirstOrDefault(a => a.Email == email);
            if (user != null)
            {
                try
                {

                    return user;
                }
                catch (Exception ex)
                {

                }
            }
            return user;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            ApplicationUser user =new ApplicationUser();
             user = _db.ApplicationUser.FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                try
                {

                    return user;
                }
                catch (Exception ex)
                {

                }
            }
            return user;
        }



        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUser.FirstOrDefault(a => a.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isValid || user == null)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }
            //if user was found generate jwt token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token,
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUser.First(u => u.UserName == registrationRequestDto.Email);
                    UserDto userDto = new UserDto()
                    {

                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        ID = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber,
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }


    }
}
