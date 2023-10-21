using ARID.Servicces.AuthAPI.Models;
using ARID.Services.AuthAPI.Models.Dto;
using System.Collections.Generic;

namespace ARID.Servicces.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<string> Edit(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<bool> Delete(string id);
        Task<ApplicationUser> GetById(string id);
        Task<ApplicationUser> GetByEmail(string email);


    }
}
