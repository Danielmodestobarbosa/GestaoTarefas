

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

using Tarefas.Api.Exceptions;
using Tarefas.Infrastructure.Extensions;
using Tarefas.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Converte os enums para string no Swagger e JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Configuração do OpenAPI.
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

// Registro das dependências.
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddIntegrations();
builder.Services.AddSecurity();

// Tratamento global de exceções.
builder.Services.AddExceptionHandler<TarefasExceptionHandler>();
builder.Services.AddProblemDetails();

// Resiliência HTTP.
builder.Services.AddHttpResilience();

// Configuração do Swagger.
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

// Configurações JWT.
var jwtSecretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new InvalidOperationException(
        "A chave secreta JWT não foi configurada.");

var jwtIssuer = builder.Configuration["JWT:ValidIssuer"]
    ?? throw new InvalidOperationException(
        "O Issuer JWT não foi configurado.");

var jwtAudience = builder.Configuration["JWT:ValidAudience"]
    ?? throw new InvalidOperationException(
        "O Audience JWT não foi configurado.");

// Valida o tamanho mínimo da chave secreta.
if (Encoding.UTF8.GetByteCount(jwtSecretKey) < 32)
{
    throw new InvalidOperationException(
        "A chave secreta JWT deve possuir pelo menos 32 bytes.");
}

// Autenticação JWT.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

// Autorização.
builder.Services.AddAuthorization();

var app = builder.Build();

// Tratamento global de exceções.
app.UseExceptionHandler();

// Swagger em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Tarefas API v1"));

    // Aplica as migrations pendentes.
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<TarefasDbContext>();

    db.Database.Migrate();
}

// Pipeline HTTP.
app.UseHttpsRedirection();

// Identifica o usuário através do JWT.
app.UseAuthentication();

// Verifica as permissões de acesso.
app.UseAuthorization();

app.MapControllers();

app.Run();

