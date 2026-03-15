using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using UserNamespace.Models;
using UserService.interfaces;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace UserNamespace.Services;

    public  class UserService : IUserService
    {
      
     private List<User> list;

    private string filePath;

    public UserService(IWebHostEnvironment webHost)
    {
        this.list = new List<User>();

        this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Users.json");
        using (var jsonFile = File.OpenText(filePath))
        {
            var content = jsonFile.ReadToEnd();
            list = JsonSerializer.Deserialize<List<User>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(list);
        File.WriteAllText(filePath, text);
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
        saveToFile();
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
        saveToFile();

        return 3;
    }

   
    public bool Delete(int id)
    {
         var u= find(id);
        if(u==null)
            return false;
        list.Remove(u);
        saveToFile();
        return true;
    }
}
public static class UserServiceExtension
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
    }
}

