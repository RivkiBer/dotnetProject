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
        // רק Admin יכול לראות את רשימת כל המשתמשים
        return services.Get();
    }

    [Authorize(Policy="AllUsers")]
    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        // בדיקה 1: משתמש רגיל יכול רק לראות את עצמו
        if(activeUser.Type == "Regular" && id != activeUser.Id)
            return Unauthorized("משתמש רגיל יכול רק לראות את הפרופיל שלו");

        // בדיקה 2: Admin יכול לראות כל משתמש
        var user = services.Get(id);
        if(user == null)
            return NotFound("המשתמש לא קיים");

        return user;
    }

    [Authorize(Policy="Admin")]
    [HttpPost]
    public ActionResult Create(User newUser)
    {
        // רק Admin יכול ליצור משתמשים חדשים
        var postedUser = services.Create(newUser);
      
        return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [Authorize(Policy="AllUsers")]
    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        // בדיקה 1: משתמש רגיל יכול לעדכן רק את עצמו
        if(activeUser.Type == "Regular" && id != activeUser.Id)
            return Unauthorized("משתמש רגיל יכול לעדכן רק את הפרופיל שלו");

        // בדיקה 2: משתמש רגיל לא יכול לשנות את סוג המשתמש
        if(activeUser.Type == "Regular" && newUser.Type != activeUser.Type)
            return Unauthorized("משתמש רגיל לא יכול לשנות את סוג המשתמש שלו");

        // בדיקה 3: Admin יכול לעדכן כל משתמש וכל שדה
        // (אם הגיע לכאן, Admin יכול לעשות כל דבר)

        // בדיקה 4: בדיקת תקינות - ה-ID בתוך הנתונים חייב להתאים
        if(services.Get(id) == null)
            return NotFound("המשתמש לא נמצא");

        if(id != newUser.Id)
            return BadRequest("ה-ID לא תואם");

        var ans = services.Update(id, newUser);
      
        if(ans == 1)
            return NotFound("המשתמש לא נמצא");

        if(ans == 2)
            return BadRequest("חריגה בעדכון");

        return NoContent();
    }

    [Authorize(Policy="Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        // רק Admin יכול למחוק משתמשים
        var ans = services.Delete(id);
      
        if(ans == false)
            return NotFound("המשתמש לא קיים");

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
