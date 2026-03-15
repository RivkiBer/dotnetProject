using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.interfaces;
using System.Security.Claims;
using UserNamespace.Services;
using UserNamespace.Models;

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
            var user = service
                .Get()
                .FirstOrDefault(u => u.Name == req.Name && u.Pass == req.Pass);

            if (user == null)
                return Unauthorized("Wrong userId / password");

            var claims = new List<Claim>
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Name),
                new Claim("password", user.Pass),
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