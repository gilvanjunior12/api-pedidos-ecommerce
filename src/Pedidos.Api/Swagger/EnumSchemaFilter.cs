using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pedidos.Api.Swagger;

// Pro filtro do Swagger usar o nome do status, não o número do enum
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
