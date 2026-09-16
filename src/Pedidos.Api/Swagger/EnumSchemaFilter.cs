using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pedidos.Api.Swagger;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var tipo = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!tipo.IsEnum)
            return;

        schema.Type = "string";
        schema.Format = null;
        schema.Enum.Clear();

        foreach (var nome in Enum.GetNames(tipo))
            schema.Enum.Add(new OpenApiString(nome));
    }
}
