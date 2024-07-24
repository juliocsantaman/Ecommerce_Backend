using Ecommerce_Backend.Models;
using Ecommerce_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Ecommerce_Backend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {

        IUserService userService;
        IConfiguration _configuration;


        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            _configuration = configuration;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var savedUser = await userService.Save(user);
                return Ok(savedUser); // Devuelve el usuario guardado como respuesta
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error si tienes un sistema de logging
                // LogError(ex);

                // Devuelve una respuesta de error
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel login)
        {
            User user = await userService.GetUserByEmailAndPassword(login.Email, login.Password);

            if(user == null)
            {

                return Unauthorized("Correo electrónico o contraseña incorrectos.");

            }

            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Convert.FromBase64String(_configuration["SecretKey"]); // Clave secreta desde Base64
            var secretKey = _configuration["SecretKey"];
            //Console.WriteLine("secretKey: " + secretKey);
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta no está configurada.");
            }

            var key = Convert.FromBase64String(secretKey);

            //Console.WriteLine("Key (Hex): " + BitConverter.ToString(key).Replace("-", ""));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Email, login.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            return Ok(new { user = user, token = tokenString  });
        }

        [Authorize]
        [HttpGet("getUserData")] 
        public async Task<ActionResult<User>> GetUserData(string email)
        {
            try
            {
                User user = await userService.GetUserByEmail(email);

                if (user == null)
                {
                    return Unauthorized("NOOOOOOOOOOOOOOOOO");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetProtectedData()
        {
            return Ok("This is protected data");
        }



    }
}
