using FurnitureStore.Api.Configuration;
using FurnitureStore.Models.Auth;
using FurnitureStore.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FurnitureStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly UserManager<IdentityUser> _userManager;
        public readonly JwtConfig _jwtConfig;


        public AuthenticationController(UserManager<IdentityUser> userManager,
                                        IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO request)
        {
            if(!ModelState.IsValid) return BadRequest();
            //verify email exist
            var emailExist = await _userManager.FindByNameAsync(request.Email);

            if(emailExist != null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already exist"
                    }
                });
            }

            var user = new IdentityUser()
            {
                Email = request.Email,
                UserName = request.Email
            };
            var isCreated = await _userManager.CreateAsync(user,request.Password);

            if(isCreated.Succeeded)
            {
                var token = GenerateToken(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }
            else
            {
                var errors = new List<string>();
                foreach (var err in isCreated.Errors)
                {
                    errors.Add(err.Description);
                }
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = errors
                });
            }
         
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
        {
            if(!ModelState.IsValid)  return BadRequest();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser == null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid Payload"
                    }
                });
            }
            var checkUser = await _userManager.CheckPasswordAsync(existingUser, request.Password);

            if(!checkUser)
            {
                return BadRequest(new AuthResult
                {
                    Result= false,
                    Errors = new List<string>()
                    {
                        "Invalid Credentials"
                    }
                });
            }
            var token = GenerateToken(existingUser);
            return Ok(new AuthResult
            {
                Token = token,
                Result = true
            });
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new  ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                })),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }

    }
}
