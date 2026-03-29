using Application.DTO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger;


public sealed class StudentRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(CreateStudent) && context.Type != typeof(UpdateStudentDto))
            return;

        if (schema.Properties is null)
            return;

        foreach (var prop in schema.Properties.Values)
        {
            if (string.Equals(prop.Type, "integer", StringComparison.OrdinalIgnoreCase))
            {
                prop.Maximum = null;
                prop.Minimum = null;
                prop.ExclusiveMaximum = null;
                prop.ExclusiveMinimum = null;
            }
        }

        if (context.Type == typeof(CreateStudent))
        {
            schema.Example = new OpenApiObject
            {
                ["classId"] = new OpenApiInteger(1),
                ["firstName"] = new OpenApiString("Jane"),
                ["lastName"] = new OpenApiString("Doe"),
                ["rollNo"] = new OpenApiInteger(101),
                ["active"] = new OpenApiBoolean(true)
            };
            return;
        }

        schema.Example = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(1),
            ["classId"] = new OpenApiInteger(1),
            ["firstName"] = new OpenApiString("Jane"),
            ["lastName"] = new OpenApiString("Doe"),
            ["rollNo"] = new OpenApiInteger(101),
            ["active"] = new OpenApiBoolean(true)
        };
    }
}
