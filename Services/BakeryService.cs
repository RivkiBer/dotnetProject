using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BakeryNamespace.Models;
using BakeryServices.Interface;
using System.IO;
using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace NamespaceBakery.Services;

public class BakeryService : IBakeryService
{

    private List<Pastry> list;

    private string filePath;

    public BakeryService(IWebHostEnvironment webHost)
    {
        this.list = new List<Pastry>();

        this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "Pastries.json");
        using (var jsonFile = File.OpenText(filePath))
        {
            var content = jsonFile.ReadToEnd();
            list = JsonSerializer.Deserialize<List<Pastry>>(content,
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
        saveToFile();
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
        saveToFile();

        return 2;
    }

    public bool Delete(int id)
    {
        var pas = find(id);
        if (pas == null)
            return false;
        list.Remove(pas);
        saveToFile();
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






