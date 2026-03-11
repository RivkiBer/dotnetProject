using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using UserNamespace.Models;
using UserService.interfaces;

namespace UserNamespace.Services;

    public  class UserService : IUserService
    {
      
     private List<User> list;

    public UserService()
    {
        this.list = new List<User>{
             new User { Id = 1, Name = "vanilla",isGloutenFree=true},
             new User { Id = 2, Name = "strawberry",isGloutenFree=true},
             new User { Id = 3, Name = "chocolate",isGloutenFree=true},
             new User { Id = 4, Name = "Pistachio",isGloutenFree=false} 
        };
    }
   
   

    public List<User> Get()
    {
        return list;
    }

    private User find(int id)
    {
        
        return list.FirstOrDefault(p => p.Id == id);

    }

    public User Get(int id) => find(id);

    public User Create(User newUser)
    {
        var maxId = list.Max(p => p.Id);
        newUser.Id = maxId + 1;
        list.Add(newUser);
            return newUser;
    }

    public int Update(int id, User newUser)
    {
        var Ice = find(id);
        if(Ice == null)
          return 1;

        if(Ice.Id != newUser.Id)
           return 2;

        var index = list.IndexOf(Ice);
        list[index] = newUser;

        return 3;
    }

   
    public bool Delete(int id)
    {
         var u= find(id);
        if(u==null)
            return false;
        list.Remove(u);
        return true;
    }
}
    public static class BakeryExtension{
      public static void AddUserService(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            //services.AddScope<IOrderManager, OrderManager>();
            //services.AddTransient<IOrderSender, OrderSenderHttp>();            
        }




}

