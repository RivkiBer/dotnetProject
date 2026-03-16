using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.interfaces;
using System.Security.Claims;
using UserNamespace.Services;
using UserNamespace.Models;
using System.Linq;
using BCrypt.Net;

namespace UserNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService service;

        public LoginController(IUserService userService)
        {
            service = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<string> Login([FromBody] LoginRequest req)
        {
            // get raw user including hashed password
            var user = service.GetRawByName(req.Name);

            if (user == null)
                return Unauthorized("Wrong userId / password");

            // verify hashed password
            if (!BCrypt.Net.BCrypt.Verify(req.Pass, user.Pass))
                return Unauthorized("Wrong userId / password");

            var claims = new List<Claim>
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Name),
                new Claim("type", user.Type)
            };

            var token = TokenService.GetToken(claims);
            return Ok(TokenService.WriteToken(token));
        }
    }

    public class LoginRequest
    {
        public string Name { get; set; }
        public string Pass { get; set; }
    }
}