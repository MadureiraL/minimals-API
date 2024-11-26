using Microsoft.EntityFrameworkCore;
using Minimals_API.Infraestrutura.Db;
using Minimals_API.Dominio.DTOs;
using Minimals_API.Dominio.Interfaces;
using Minimals_API.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Minimals_API.Dominio.ModelViews;
using Minimals_API.Dominio.Entidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Minimals_API.Dominio.Enuns;

#region Builder
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAdministrador, AdministradorServico>();
builder.Services.AddScoped<IVeiculo, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sqlServer")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
#endregion

#region App
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
//validação de login e senha
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministrador administrador) =>
{

    if (administrador.Login(loginDTO) != null)
        return Results.Ok("Login feito com sucesso");
    else
        return Results.Unauthorized();

}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdminstradorDTO administradorDTO, IAdministrador administrador) =>
{
    var validacao = new ErrosDeValidação
    {
        Mensagens = new List<string>()  
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("O Email não pode ser vazio");
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("A senha não pode ser vazia");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("O Perfil não pode ser vazio");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);



    var veiculo = new Administrador
    {

        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.editor.ToString()
    };

    administrador.Incluir(veiculo);

    return Results.Created($"/administrador/{veiculo.Id}", veiculo);


}).WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidação validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidação
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca não pode ficar em branco");

    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo");

    return validacao;



}


app.MapPost("/veiculos/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculo veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);

}).WithTags("Veiculos");

app.MapGet("/veiculos/", ([FromQuery] int? pagina, IVeiculo veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);


    return Results.Ok(veiculos);

}).WithTags("Veiculos");

app.MapGet("/veiculos{id}/", ([FromRoute] int id, IVeiculo veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos{id}/", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculo veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();


    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);


    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos{id}/", ([FromRoute] int id, IVeiculo veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();



    veiculoServico.Apagar(veiculo);


    return Results.NoContent();

}).WithTags("Veiculos");



#endregion

app.Run();


