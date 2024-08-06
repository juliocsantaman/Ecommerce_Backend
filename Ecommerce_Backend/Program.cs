using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Ecommerce_Backend;
using Ecommerce_Backend.Middlewares;
using Ecommerce_Backend.Models;
using Ecommerce_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlServer<UserContext>(builder.Configuration.GetConnectionString("CnEcommerce"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ExcelService>();


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

// Data.
ClientModel client1 = new ClientModel();
client1.Id = 1;
client1.Name = "Jhon Doe";
ClientModel client2 = new ClientModel();
client2.Id = 2;
client2.Name = "Jane Smith";
ClientModel client3 = new ClientModel();
client3.Id = 3;
client3.Name = "Julio Santaman";
ClientModel client4 = new ClientModel();
client4.Id = 4;
client4.Name = "Claire Redfield";
ClientModel client5 = new ClientModel();
client5.Id = 5;
client5.Name = "Leon Kennedy";

ClientModel[] clients = [client1, client2, client3, client4, client5];

ClientModel client6 = new ClientModel();
client6.Id = 1;
client6.Name = "Jessica RLz";


ClientModel[] clients2 = [client6];

string[] headers = new string[] { "ID", "NAME" };

ExcelService excelService = new ExcelService();
excelService.CreateWorkSheet("test1");
excelService.CreateHeaders(headers);
excelService.CreateRows(clients);
excelService.SaveBook("C:\\Users\\julio\\OneDrive\\Desktop\\file.xlsx");

excelService.CreateWorkSheet("test1");
excelService.CreateHeaders(headers);
excelService.CreateRows(clients2);
excelService.SaveBook("C:\\Users\\julio\\OneDrive\\Desktop\\file2.xlsx");



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
