using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ap2ex3_server.Services;
using ap2ex3_server.ActionModels;

namespace ap2ex3_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IService _service;

        public UsersController(IConfiguration configuration, IService service)
        {
            _configuration = configuration;
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (loginModel.Username == null || loginModel.Password == null)
                return Forbid();

            if (await _service.Login(loginModel.Username, loginModel.Password))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", loginModel.Username)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: mac);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
                return Forbid();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupModel signupModel)
        {
            if (!ModelState.IsValid || signupModel.Id == null)
                return Forbid();

            if (await _service.DoesUserExist(signupModel.Id))
                return Forbid();

            await _service.AddUser(signupModel.ConvertToUser());
            return Ok();
        }
    }
}