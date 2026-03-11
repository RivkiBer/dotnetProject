using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BakeryNamespace.Models;
using NamespaceBakery.Services;
using BakeryServices.Interface;

namespace BakeryNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class BakeryController : ControllerBase
{  
   IBakeryService services;
    public BakeryController(IBakeryService Pastservices)
    {
        this.services=Pastservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<Pastry>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Pastry> Get(int id)
    {
        var m = services.Get(id);
        if(m==null)
            return NotFound();
        return m;

    }

    [HttpPost]
    public ActionResult Create(Pastry newPastry)
    {
        var postedPastry = services.Create(newPastry);
      
       return CreatedAtAction(nameof(Create), new { id = postedPastry.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Pastry newPastry)
    {
        var ans= services.Update( id, newPastry);
      
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
