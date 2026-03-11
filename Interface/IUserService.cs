using System.Threading.Tasks;
using UserNamespace.Models;


namespace UserService.interfaces;


    public interface IUserService
    {
     List<User> Get();
   
     User Get(int id);
     User Create(User newUser);

     int Update(int id, User newUser);
   
     bool Delete(int id);
    // int Update(int id, User newUser);
}


