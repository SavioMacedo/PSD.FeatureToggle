using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSD.FeatureToggle.Entities;

namespace PSD.FeatureToggle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public LoginController(IUsuarioService usuarioService) => _usuarioService = usuarioService;

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromForm] IFormCollection form)
        {
            string login = form.ContainsKey("login") ? form["login"] : string.Empty;
            string password = form.ContainsKey("password") ? form["password"] : string.Empty;
            Usuario user = await _usuarioService.GetUserAsync(login, password);

            if (user == null)
                return Unauthorized();

            string token = user.GenerateToken();
            return Ok(new
            {
                user.Login,
                user.Name,
                user.Role,
                Accesstoken = token,
            });
        }

    }
}
