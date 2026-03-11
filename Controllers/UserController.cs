using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UserNamespace.Models;
using UserNamespace.Services;
using UserService.interfaces;

namespace UserNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{  
   IUserService services;
    public UserController(IUserService Iceservices)
    {
        this.services=Iceservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var m = services.Get(id);
        if(m==null)
            return NotFound();
        return m;

    }

    [HttpPost]
    public ActionResult Create(User newUser)
    {
        var postedUser = services.Create(newUser);
      
       return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        var ans= services.Update( id, newUser);
      
        if(ans==1)
          return NotFound();

        if(ans==2)
           return BadRequest();

       
        return NoContent();

    }

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
