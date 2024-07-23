using Ecommerce_Backend;
using Ecommerce_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                          .AllowAnyMethod();
               });
});

// Configurar la autenticación JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

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
