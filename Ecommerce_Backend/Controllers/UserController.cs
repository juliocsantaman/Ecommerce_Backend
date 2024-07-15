using Ecommerce_Backend.Models;
using Ecommerce_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Backend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {

        IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
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


    }
}
