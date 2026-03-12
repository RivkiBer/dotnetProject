using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BakeryNamespace.Models;
using BakeryServices.Interface;

namespace NamespaceBakery.Services;

public class BakeryService : IBakeryService
{

    private List<Pastry> list;


    public BakeryService()
    {
        this.list = new List<Pastry>{
             new Pastry { Id = 1, Name = "Croissant",isMilky=true},
             new Pastry { Id = 2, Name = "Muffin",isMilky=false},
             new Pastry { Id = 3, Name = "Donut",isMilky=false},
             new Pastry { Id = 4, Name = "Cake",isMilky=false}
        };
    }
    private Pastry find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);
    }

    public List<Pastry> Get()
    {
        return list;
    }


    public Pastry Get(int id) => find(id);

    public Pastry Create(Pastry newPastry)
    {
        var maxId = list.Max(m => m.Id);
        newPastry.Id = maxId + 1;
        list.Add(newPastry);
        return newPastry;
    }

    public int Update(int id, Pastry newPastry)
    {
        var pas = find(id);
        if (pas == null)
            return 0;
        if (pas.Id != newPastry.Id)
            return 1;

        var index = list.IndexOf(pas);
        list[index] = newPastry;

        return 2;
    }

    public bool Delete(int id)
    {
        var pas = find(id);
        if (pas == null)
            return false;
        list.Remove(pas);
        return true;
    }
}
public static class BakeryExtension
{
    public static void AddPastryService(this IServiceCollection services)
    {
        services.AddSingleton<IBakeryService, BakeryService>();
    }
}






