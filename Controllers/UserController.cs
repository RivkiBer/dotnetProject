using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UserNamespace.Models;
using UserNamespace.Services;
using UserService.interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserNamespace.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{  
   IUserService services;
   User activeUser;
    public UserController(IUserService Iceservices, IActiveUser activeUser)
    {
        this.services=Iceservices;
        this.activeUser = activeUser.ActiveUser
            ?? throw new System.InvalidOperationException("Active user is required");
    }

    [Authorize(Policy="Admin")]
    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        return services.Get();
    }

    [Authorize(Policy="AllUsers")]
    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        if(id == activeUser.Id || activeUser.Type == "Admin"){
            var m = services.Get(id);
            if(m==null)
                return NotFound();
            return m;
        }
        return Unauthorized();
    }

    [Authorize(Policy="Admin")]
    [HttpPost]
    public ActionResult Create(User newUser)
    {
        var postedUser = services.Create(newUser);
      
       return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [Authorize(Policy="AllUsers")]
    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        if(!(id == activeUser.Id || activeUser.Type == "Admin"))
            return Unauthorized();
        if(activeUser.Type != "Admin" && newUser.Type != activeUser.Type)
            return Unauthorized();
            
        var ans= services.Update( id, newUser);
      
        if(ans==1)
          return NotFound();

        if(ans==2)
           return BadRequest();

       
        return NoContent();

    }

    [Authorize(Policy="Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var ans= services.Delete(id);
      
        if(ans==false)
            return NotFound();
        return NoContent();

    }
}
 
//     [HttpDelete("{id}")]
//     public ActionResult Delete(int id)
//     {
//         var Ice = find(id);
//         if (Ice == null)
//             return NotFound();
//         else
//         {
//             services.Remove(Ice);
//         }  
//          return NoContent();  
        
//     }
// }
