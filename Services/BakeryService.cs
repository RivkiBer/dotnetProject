using System.Linq;
using BakeryNamespace.Models;
using BakeryServices.Interface;
using Microsoft.AspNetCore.Hosting;
using BakeryNamespace.Services;

namespace NamespaceBakery.Services;

/// <summary>
/// Service for Pastry CRUD operations
/// Inherits generic CRUD logic from BaseGenericService
/// </summary>
public class BakeryService : BaseGenericService<Pastry>, IBakeryService
{
    public BakeryService(IWebHostEnvironment webHost)
        : base(webHost, "Pastries.json")
    {
    }

    /// <summary>
    /// Get pastry by ID
    /// </summary>
    public Pastry Get(int id) => FindById(id);

    /// <summary>
    /// Update pastry with validation
    /// Returns: 0 = not found, 1 = ID mismatch, 2 = success
    /// </summary>
    public int Update(int id, Pastry newPastry)
    {
        var pas = FindById(id);
        if (pas == null)
            return 0;
        if (pas.Id != newPastry.Id)
            return 1;

        var index = list.IndexOf(pas);
        list[index] = newPastry;
        SaveToFile();

        return 2;
    }
}
public static class BakeryExtension
{
    public static void AddPastryService(this IServiceCollection services)
    {
        services.AddSingleton<IBakeryService, BakeryService>();
    }
}






