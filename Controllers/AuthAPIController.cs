using ARID.Servicces.AuthAPI.Service.IService;
using ARID.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ARID.Servicces.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService=authService;
            _response=new();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;

                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Edit(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;

                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var errorMessage = await _authService.Delete(id);
            if (!errorMessage)
            {
                _response.IsSuccess = false;
                _response.Message = "Error Encountered";

                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "User or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result=loginResponse;
            return Ok(_response);
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error Encountered";
                return BadRequest(_response);
            }
            
            return Ok(_response);
        }


       

        [HttpGet]
        [Route("{id}")]
        public ResponseDto Get(string id)
        {
            try
            {
                _response.Result = _authService.GetById(id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByEmail/{email}")]
        public ResponseDto GetByEmail(string email)
        {
            try
            {
               
                _response.Result = _authService.GetByEmail(email);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
