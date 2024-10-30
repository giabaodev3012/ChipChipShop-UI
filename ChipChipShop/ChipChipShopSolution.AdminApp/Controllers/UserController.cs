using ChipChipShopSolution.AdminApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ChipChipShopSolution.AdminApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        public UserController(IUserApiClient userApiClient) { 
           _userApiClient = userApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
        
            if(!ModelState.IsValid)
                return View(ModelState);

            var token = await _userApiClient.Authenticate(request);

            return View(token);
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            // Bật chế độ hiển thị thông tin chi tiết (PII)
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true; // Kiểm tra thời gian sống của token
            validationParameters.ValidAudience = _configuration["Tokens:Issuer"]; // Đối tượng dự kiến của token
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"]; // Người phát hành token
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); // Khóa bí mật để ký token


            // Xác thực token

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().Vali dateToken(jwtToken, validationParameters, out validatedToken);
            // Trả về thông tin người dùng
            return principal;
        }

    }

    }
}
