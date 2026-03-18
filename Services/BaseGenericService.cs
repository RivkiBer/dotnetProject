using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace BakeryNamespace.Services;

/// <summary>
/// Base class for generic CRUD operations on JSON-based entities
/// Eliminates code duplication between BakeryService and UserService
/// </summary>
public abstract class BaseGenericService<T> where T : class
{
    protected List<T> list;
    protected string filePath;

    protected BaseGenericService(IWebHostEnvironment webHost, string dataFileName)
    {
        list = new List<T>();
        filePath = Path.Combine(webHost.ContentRootPath, "Data", dataFileName);
        LoadFromFile();
    }

    /// <summary>
    /// Load entities from JSON file
    /// </summary>
    protected virtual void LoadFromFile()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Data file not found: {filePath}");

        using (var jsonFile = File.OpenText(filePath))
        {
            var content = jsonFile.ReadToEnd();
            list = JsonSerializer.Deserialize<List<T>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<T>();
        }
    }

    /// <summary>
    /// Save entities to JSON file
    /// </summary>
    protected virtual void SaveToFile()
    {
        var text = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, text);
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual List<T> Get()
    {
        return list;
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    public virtual T Create(T newEntity)
    {
        if (newEntity == null)
            throw new ArgumentNullException(nameof(newEntity));

        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
            throw new InvalidOperationException("Entity must have an Id property");

        int maxId = 0;
        if (list.Any())
        {
            var maxEntity = list.OrderByDescending(e => (int?)idProperty.GetValue(e) ?? 0).First();
            maxId = (int?)idProperty.GetValue(maxEntity) ?? 0;
        }
        
        idProperty.SetValue(newEntity, maxId + 1);
        list.Add(newEntity);
        SaveToFile();
        
        return newEntity;
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    public virtual bool Delete(int id)
    {
        var entity = FindById(id);
        if (entity == null)
            return false;

        list.Remove(entity);
        SaveToFile();
        return true;
    }

    /// <summary>
    /// Find entity by ID property
    /// </summary>
    protected virtual T? FindById(int id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
            return null;

        return list.FirstOrDefault(e => (int?)idProperty.GetValue(e) == id);
    }
}
