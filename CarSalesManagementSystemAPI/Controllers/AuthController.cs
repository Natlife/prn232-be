using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public class RegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            bool result = await _authService.RegisterAsync(request.FullName, request.Email, request.Password);
            if (!result)
                return BadRequest(new { Message = "Email đã tồn tại." });
            return Ok(new { Message = "Đăng ký thành công! Vui lòng kiểm tra Email để lấy mã xác nhận OTP." });
        }

        public class VerifyRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyRequest request)
        {
            bool result = await _authService.VerifyEmailAsync(request.Email, request.Otp);
            if (!result)
                return BadRequest(new { Message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            return Ok(new { Message = "Xác thực Email thành công. Bạn đã có thể đăng nhập." });
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.Login(request.Email, request.Password);
            if (token == null)
                return Unauthorized(new { Message = "Email hoặc Mật khẩu không đúng. Vui lòng kiểm tra lại (Lưu ý: Bạn phải xác nhận Email trước khi đăng nhập)." });
            
            return Ok(new { Token = token, Message = "Đăng nhập thành công!" });
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            bool result = await _authService.ForgotPasswordAsync(request.Email);
            if (!result)
                return BadRequest(new { Message = "Không tìm thấy tài khoản với Email này." });
            return Ok(new { Message = "Vui lòng kiểm tra Email để lấy mã OTP đặt lại mật khẩu." });
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
            public string NewPassword { get; set; }
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            bool result = _authService.ResetPassword(request.Email, request.Otp, request.NewPassword);
            if (!result)
                return BadRequest(new { Message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            return Ok(new { Message = "Đặt lại mật khẩu thành công!" });
        }
    }
}
