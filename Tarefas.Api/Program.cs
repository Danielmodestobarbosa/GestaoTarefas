
using Microsoft.EntityFrameworkCore;
using Tarefas.Api.Exceptions;
using Tarefas.Infrastructure.Extensions;
using Tarefas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;


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
builder.Services.AddSecurity();

//ExceptionHandler
builder.Services.AddExceptionHandler<TarefasExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpResilience();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Tarefas API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe o token JWT."
        });

        c.AddSecurityRequirement(document =>
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
    });
}

var app = builder.Build();

app.UseExceptionHandler();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tarefas API v1"));

    // Aplica as migrations pendentes.
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<TarefasDbContext>();

    db.Database.Migrate();
}

app.UseHttpsRedirection();

// Identifica o usuário através do JWT.
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();
