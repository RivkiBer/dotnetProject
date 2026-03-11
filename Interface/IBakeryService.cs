using System.Threading.Tasks;
using BakeryNamespace.Models;


namespace BakeryServices.Interface;


    public interface IBakeryService
    {
     List<Pastry> Get();
   
     Pastry Get(int id);
     Pastry Create(Pastry newPastry);

     int Update(int id, Pastry newPastry);
   
     bool Delete(int id);
   
}


