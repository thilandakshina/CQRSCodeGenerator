using CQRSGenerator;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("CQRS Generator");
        Console.WriteLine("--------------");

        // Get solution name
        Console.Write("Enter solution name (e.g., SCT): ");
        string solutionName = Console.ReadLine() ?? "SCT";

        Console.WriteLine("\nEnter your entity class definition (type 'END' on a new line when finished):");
        var entityDefinition = ReadEntityDefinition();

        Console.Write("\nEnter output path: ");
        string basePath = Console.ReadLine() ?? Directory.GetCurrentDirectory();

        try
        {
            // Parse entity
            var entity = EntityParser.ParseEntity(entityDefinition);

            // Generate CQRS components
            var generator = new CQRSGenerator.CQRSGenerator(solutionName, basePath, entity);
            generator.GenerateAll();

            Console.WriteLine("\nCQRS components generated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }

    static string ReadEntityDefinition()
    {
        var sb = new StringBuilder();
        string line;
        while ((line = Console.ReadLine()) != "END")
        {
            sb.AppendLine(line);
        }
        return sb.ToString();
    }
}