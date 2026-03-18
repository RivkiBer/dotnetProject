using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BakeryNamespace.Models;
using NamespaceBakery.Services;
using BakeryServices.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserService.interfaces;
using UserNamespace.Models;
using Microsoft.AspNetCore.SignalR;
using BakeryApp.Hubs;
using BakeryNamespace.Services;

namespace BakeryNamespace.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AllUsers")] // Only authenticated users (Admin or Regular)
public class BakeryController : ControllerBase
{  
   IBakeryService services;
   User activeUser;
   INotificationService notificationService;

    public BakeryController(IBakeryService Pastservices, IActiveUser activeUser, INotificationService notificationService)
    {
        this.services = Pastservices;
        this.activeUser = activeUser.ActiveUser
            ?? throw new System.InvalidOperationException("Active user is required");
        this.notificationService = notificationService 
            ?? throw new System.InvalidOperationException("Notification service is required");
    }

    [HttpGet()]
    public ActionResult<IEnumerable<Pastry>> Get()
    {
        var pastries = services.Get().Where(p => p.UserId == activeUser.Id);
        return Ok(pastries);
    }

    [HttpGet("{id}")]
    public ActionResult<Pastry> Get(int id)
    {
        var m = services.Get(id);
        if(m==null || m.UserId != activeUser.Id)
            return NotFound();
        return m;

    }

    [HttpPost]
    public async Task<ActionResult> Create(Pastry newPastry)
    {
        newPastry.UserId = activeUser.Id;
        var postedPastry = services.Create(newPastry);
        
        // Send notification via centralized service
        await notificationService.NotifyItemUpdate(
            activeUser.Id,
            activeUser.Name,
            activeUser.Type,
            "add",
            postedPastry.Name
        );
      
        return CreatedAtAction(nameof(Create), new { id = postedPastry.Id });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Pastry newPastry)
    {
        var existing = services.Get(id);
        if (existing == null || existing.UserId != activeUser.Id)
            return NotFound();

        newPastry.UserId = activeUser.Id; // ensure it stays the same
        var ans= services.Update( id, newPastry);
      
        if(ans==0)
            return NotFound();

        if(ans==1)
            return BadRequest();

        // Send notification via centralized service
        await notificationService.NotifyItemUpdate(
            activeUser.Id,
            activeUser.Name,
            activeUser.Type,
            "update",
            newPastry.Name
        );

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existing = services.Get(id);
        if (existing == null || existing.UserId != activeUser.Id)
            return NotFound();

        var ans= services.Delete(id);
      
        if(ans==false)
            return NotFound();

        // Send notification via centralized service
        await notificationService.NotifyItemUpdate(
            activeUser.Id,
            activeUser.Name,
            activeUser.Type,
            "delete",
            existing.Name
        );

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
