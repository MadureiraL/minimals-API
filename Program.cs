using Microsoft.EntityFrameworkCore;
using Minimals_API.Infraestrutura.Db;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//validação de login e senha
app.MapPost("/login", (Minimals_API.Dominio.DTOs.LoginDTO loginDTO) =>
{

    if (loginDTO.Email == "adm@test.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login feito com sucesso");
    else
        return Results.Unauthorized();

});

app.Run();


