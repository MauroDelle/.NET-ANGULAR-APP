using Microsoft.AspNetCore.Mvc;
using webAPI.Models;
using webAPI.DataBase;
namespace webAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserDataBase _userDB;

        public LoginController()
        {
            _userDB = new UserDataBase();
        }

        [HttpPost]
        public IActionResult Login([FromBody] User loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Datos de login inválidos");
            }

            var user = _userDB.GetUserByUsername(loginRequest.Username);

            if (user != null && user.Password == loginRequest.Password)
            {
                return Ok("Login exitoso");
            }
            else
            {
                return Unauthorized("Credenciales incorrectas");
            }
        }
    }
}
