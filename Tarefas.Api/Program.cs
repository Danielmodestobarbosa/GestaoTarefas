
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Tarefas.Api.Exceptions;
using Tarefas.Infrastructure.Extensions;
using Tarefas.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

//Converte os enums para string no Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        var type = context.JsonTypeInfo.Type;

        if (type.IsEnum)
        {
            schema.Type = Microsoft.OpenApi.JsonSchemaType.String;

            schema.Enum = Enum.GetNames(type)
                .Select(nome => System.Text.Json.Nodes.JsonValue.Create(nome)!)
                .Cast<System.Text.Json.Nodes.JsonNode>()
                .ToList();
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddEndpointsApiExplorer();

//Extensions
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddIntegrations();

//ExceptionHandler
builder.Services.AddExceptionHandler<TarefasExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpResilience();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/openapi/v1.json", "Tarefas API v1"));

    // Aplica as migrations pendentes no banco.
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<TarefasDbContext>();

    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
