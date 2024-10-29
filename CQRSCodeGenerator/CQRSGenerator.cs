using System.Text;

namespace CQRSGenerator;

public class CQRSGenerator
{
    private readonly string _solutionName;
    private readonly string _basePath;
    private readonly EntityDefinition _entity;

    public CQRSGenerator(string solutionName, string basePath, EntityDefinition entity)
    {
        _solutionName = solutionName;
        _basePath = basePath;
        _entity = entity;
    }

    public void GenerateAll()
    {
        string entityPath = Path.Combine(_basePath, $"{_solutionName}.Shared", "Services", _entity.Name);
        CreateDirectoryStructure(entityPath);

        // 1.Generate Commands
        GenerateCreateCommand(entityPath);
        GenerateUpdateCommand(entityPath);
        GenerateDeleteCommand(entityPath);

        // 2.Generate Queries
        GenerateGetByIdQuery(entityPath);
        GenerateGetAllQuery(entityPath);

        // 3.Generate DTOs
        GenerateDTO(entityPath);

        // 4.Generate Validators
        GenerateValidators(entityPath);
    }

    private void CreateDirectoryStructure(string basePath)
    {
        foreach (var folder in new[] { "Commands", "Queries", "DTOs", "Validators" })
        {
            Directory.CreateDirectory(Path.Combine(basePath, folder));
        }
    }

    private void GenerateCreateCommand(string basePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"using MediatR;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Commands;

public sealed class Create{_entity.Name}Command : IRequest<Create{_entity.Name}Response>
{{");

        foreach (var prop in _entity.Properties.Where(p => !p.IsPrimaryKey &&
                                                         p.Name != "CreatedDate" &&
                                                         p.Name != "UpdatedDate"))
        {
            sb.AppendLine($"    public {prop.Type} {prop.Name} {{ get; set; }} = {GetDefaultValue(prop.Type)};");
        }

        sb.AppendLine($@"}}

public class Create{_entity.Name}Response
{{
    public {EntityParser.GetPrimaryKeyType(_entity)} {EntityParser.GetPrimaryKeyProperty(_entity)} {{ get; set; }}
    public bool Success {{ get; set; }}
    public string Message {{ get; set; }} = string.Empty;
}}");

        File.WriteAllText(Path.Combine(basePath, "Commands", $"Create{_entity.Name}Command.cs"), sb.ToString());
    }

    private void GenerateUpdateCommand(string basePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"using MediatR;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Commands;

public sealed class Update{_entity.Name}Command : IRequest<Update{_entity.Name}Response>
{{
    public {EntityParser.GetPrimaryKeyType(_entity)} {EntityParser.GetPrimaryKeyProperty(_entity)} {{ get; set; }}");

        foreach (var prop in _entity.Properties.Where(p => !p.IsPrimaryKey &&
                                                         p.Name != "CreatedDate" &&
                                                         p.Name != "UpdatedDate"))
        {
            sb.AppendLine($"    public {prop.Type} {prop.Name} {{ get; set; }} = {GetDefaultValue(prop.Type)};");
        }

        sb.AppendLine($@"}}

public class Update{_entity.Name}Response
{{
    public bool Success {{ get; set; }}
    public string Message {{ get; set; }} = string.Empty;
}}");

        File.WriteAllText(Path.Combine(basePath, "Commands", $"Update{_entity.Name}Command.cs"), sb.ToString());
    }

    private void GenerateDeleteCommand(string basePath)
    {
        var command = $@"using MediatR;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Commands;

public sealed class Delete{_entity.Name}Command : IRequest<Delete{_entity.Name}Response>
{{
    public {EntityParser.GetPrimaryKeyType(_entity)} {EntityParser.GetPrimaryKeyProperty(_entity)} {{ get; set; }}
}}

public class Delete{_entity.Name}Response
{{
    public bool Success {{ get; set; }}
    public string Message {{ get; set; }} = string.Empty;
}}";

        File.WriteAllText(Path.Combine(basePath, "Commands", $"Delete{_entity.Name}Command.cs"), command);
    }

    private void GenerateGetByIdQuery(string basePath)
    {
        var query = $@"using MediatR;
using {_solutionName}.Shared.Services.{_entity.Name}.DTOs;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Queries;

public sealed class Get{_entity.Name}ByIdQuery : IRequest<{_entity.Name}DTO>
{{
    public {EntityParser.GetPrimaryKeyType(_entity)} {EntityParser.GetPrimaryKeyProperty(_entity)} {{ get; set; }}
}}";

        File.WriteAllText(Path.Combine(basePath, "Queries", $"Get{_entity.Name}ByIdQuery.cs"), query);
    }

    private void GenerateGetAllQuery(string basePath)
    {
        var query = $@"using MediatR;
using {_solutionName}.Shared.Services.{_entity.Name}.DTOs;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Queries;

public sealed class GetAll{_entity.Name}Query : IRequest<List<{_entity.Name}DTO>>
{{
    public string? Status {{ get; set; }}
    public DateTime? StartDate {{ get; set; }}
    public DateTime? EndDate {{ get; set; }}
}}";

        File.WriteAllText(Path.Combine(basePath, "Queries", $"GetAll{_entity.Name}Query.cs"), query);
    }

    private void GenerateDTO(string basePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"namespace {_solutionName}.Shared.Services.{_entity.Name}.DTOs;

public class {_entity.Name}DTO
{{");

        foreach (var prop in _entity.Properties)
        {
            sb.AppendLine($"    public {prop.Type} {prop.Name} {{ get; set; }} = {GetDefaultValue(prop.Type)};");
        }

        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(basePath, "DTOs", $"{_entity.Name}DTO.cs"), sb.ToString());
    }

    private void GenerateValidators(string basePath)
    {
        GenerateCreateCommandValidator(basePath);
        GenerateUpdateCommandValidator(basePath);
        GenerateGetByIdQueryValidator(basePath);
    }

    private void GenerateCreateCommandValidator(string basePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"using FluentValidation;
using {_solutionName}.Shared.Services.{_entity.Name}.Commands;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Validators;

public class Create{_entity.Name}CommandValidator : AbstractValidator<Create{_entity.Name}Command>
{{
    public Create{_entity.Name}CommandValidator()
    {{");

        foreach (var prop in _entity.Properties.Where(p => !p.IsPrimaryKey))
        {
            GenerateValidationRules(sb, prop);
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(basePath, "Validators", $"Create{_entity.Name}CommandValidator.cs"), sb.ToString());
    }

    private void GenerateUpdateCommandValidator(string basePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($@"using FluentValidation;
using {_solutionName}.Shared.Services.{_entity.Name}.Commands;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Validators;

public class Update{_entity.Name}CommandValidator : AbstractValidator<Update{_entity.Name}Command>
{{
    public Update{_entity.Name}CommandValidator()
    {{
        RuleFor(x => x.{EntityParser.GetPrimaryKeyProperty(_entity)})
            .NotEmpty();");

        foreach (var prop in _entity.Properties.Where(p => !p.IsPrimaryKey))
        {
            GenerateValidationRules(sb, prop);
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(basePath, "Validators", $"Update{_entity.Name}CommandValidator.cs"), sb.ToString());
    }

    private void GenerateGetByIdQueryValidator(string basePath)
    {
        var validator = $@"using FluentValidation;
using {_solutionName}.Shared.Services.{_entity.Name}.Queries;

namespace {_solutionName}.Shared.Services.{_entity.Name}.Validators;

public class Get{_entity.Name}ByIdQueryValidator : AbstractValidator<Get{_entity.Name}ByIdQuery>
{{
    public Get{_entity.Name}ByIdQueryValidator()
    {{
        RuleFor(x => x.{EntityParser.GetPrimaryKeyProperty(_entity)})
            .NotEmpty();
    }}
}}";

        File.WriteAllText(Path.Combine(basePath, "Validators", $"Get{_entity.Name}ByIdQueryValidator.cs"), validator);
    }

    private void GenerateValidationRules(StringBuilder sb, EntityProperty prop)
    {
        sb.AppendLine($"\n        RuleFor(x => x.{prop.Name})");

        switch (prop.Type.ToLower())
        {
            case "string":
                sb.AppendLine("            .NotEmpty()");
                if (prop.Name.Contains("Email"))
                    sb.AppendLine("            .EmailAddress()");
                if (prop.Name.Contains("Name"))
                    sb.AppendLine("            .MaximumLength(100)");
                if (prop.Name.Contains("Status"))
                {
                    sb.AppendLine(@"            .Must(status => status is ""Active"" or ""Inactive"")");
                    sb.AppendLine(@"            .WithMessage(""Status must be either Active or Inactive"");");
                }
                break;

            case "datetime":
                sb.AppendLine("            .NotEmpty()");
                if (prop.Name.Contains("Date") && !prop.Name.Contains("Created") && !prop.Name.Contains("Updated"))
                    sb.AppendLine("            .Must(date => date >= DateTime.Today)");
                break;

            default:
                sb.AppendLine("            .NotEmpty();");
                break;
        }
    }

    private string GetDefaultValue(string type) => type.ToLower() switch
    {
        "string" => "string.Empty",
        "int" => "0",
        "long" => "0L",
        "decimal" => "0m",
        "double" => "0d",
        "float" => "0f",
        "datetime" => "DateTime.MinValue",
        "guid" => "Guid.Empty",
        "bool" => "false",
        _ => "default"
    };
}