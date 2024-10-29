using System.Text.RegularExpressions;

namespace CQRSGenerator;

public class EntityProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsPrimaryKey { get; set; }

    public EntityProperty(string name, string type, bool isPrimaryKey)
    {
        Name = name;
        Type = type;
        IsPrimaryKey = isPrimaryKey;
    }
}

public class EntityDefinition
{
    public string Name { get; set; }
    public List<EntityProperty> Properties { get; set; }

    public EntityDefinition(string name, List<EntityProperty> properties)
    {
        Name = name;
        Properties = properties;
    }
}

public static class EntityParser
{
    public static EntityDefinition ParseEntity(string classDefinition)
    {
        // Extract class name
        var classNameRegex = new Regex(@"public\s+class\s+(\w+)");
        var classNameMatch = classNameRegex.Match(classDefinition);
        if (!classNameMatch.Success)
            throw new Exception("Could not find class name in definition");

        string className = classNameMatch.Groups[1].Value;

        // Extract properties
        var propertyRegex = new Regex(@"public\s+([^\s]+)\s+([^\s]+)\s*{\s*get;\s*set;\s*}");
        var properties = new List<EntityProperty>();

        foreach (Match match in propertyRegex.Matches(classDefinition))
        {
            string propertyType = match.Groups[1].Value;
            string propertyName = match.Groups[2].Value;

            // Determine if it's a primary key
            bool isPrimaryKey = propertyName.EndsWith("Id");

            properties.Add(new EntityProperty(propertyName, propertyType, isPrimaryKey));
        }

        return new EntityDefinition(className, properties);
    }

    public static string GetPrimaryKeyProperty(EntityDefinition entity)
    {
        return entity.Properties.FirstOrDefault(p => p.IsPrimaryKey)?.Name
            ?? $"{entity.Name}Id";
    }

    public static string GetPrimaryKeyType(EntityDefinition entity)
    {
        return entity.Properties.FirstOrDefault(p => p.IsPrimaryKey)?.Type ?? "Guid";
    }
}