using Ecommerce_Backend;
using Ecommerce_Backend.Middlewares;
using Ecommerce_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlServer<UserContext>(builder.Configuration.GetConnectionString("CnEcommerce"));

builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // La URL de tu frontend Angular
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                    .AllowCredentials(); // Permite credenciales si es necesario

        });
});

// Configurar la autenticación JWT
var secretKey = builder.Configuration["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("La clave secreta no está configurada.");
}

//Console.WriteLine("secretKey: " + secretKey);

var key = Convert.FromBase64String(secretKey);

//Console.WriteLine("Key (Hex): " + BitConverter.ToString(key).Replace("-", ""));



//builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true; //true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, //true
        ValidateIssuerSigningKey = true, //true
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Ajusta esto si es necesario


    };
    options.UseSecurityTokenValidators = true;




    // Incluye el header WWW-Authenticate en la respuesta cuando la autenticación falle
    //options.Events = new JwtBearerEvents
    //{
    //    OnChallenge = context =>
    //    {
    //        context.HandleResponse();
    //        context.Response.StatusCode = 401;
    //        context.Response.Headers.Append("WWW-Authenticate", "Bearer");
    //        return Task.CompletedTask;
    //    }
    //};

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated: " + context.SecurityToken);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            Console.WriteLine("Token received: " + context.Token);
            if (context.Token == null)
            {
                Console.WriteLine("Token is null");
            }
            else
            {
                Console.WriteLine("Token: " + context.Token);
            }
            return Task.CompletedTask;
        }
    };
});


var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication(); // Asegúrate de agregar esta línea antes de Authorization
app.UseAuthorization();

app.MapControllers();

app.MapGet("/dbconnection", async ([FromServices] UserContext dbContext) =>
{
    dbContext.Database.EnsureCreated();
    return Results.Ok("Database in memory: " + dbContext.Database.IsInMemory());
});

app.Run();
