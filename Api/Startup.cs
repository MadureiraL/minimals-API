using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimals_API;
using Minimals_API.Dominio.DTOs;
using Minimals_API.Dominio.Entidades;
using Minimals_API.Dominio.Enuns;
using Minimals_API.Dominio.Interfaces;
using Minimals_API.Dominio.ModelViews;
using Minimals_API.Dominio.Servicos;
using Minimals_API.Infraestrutura.Db;


public class Startup
{
    

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key;
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
 {
     option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
     option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
 }).AddJwtBearer(option =>
 {
     option.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateLifetime = true,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
         ValidateIssuer = false,
         ValidateAudience = false
     };
 });

        services.AddAuthentication();

        // Escopo das classes Administrador e Veiculo
        services.AddScoped<IAdministrador, AdministradorServico>();
        services.AddScoped<IVeiculo, VeiculoServico>();

        //Conexão com o banco de dados
        services.AddDbContext<DbContexto>(options => options.UseSqlServer(Configuration.GetConnectionString("sqlServer")));

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
         {
             //Configuração do Swagger para receber e enviar o token
             options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
             {
                 Name = "Authorization",
                 Type = SecuritySchemeType.Http,
                 Scheme = "Bearer",
                 BearerFormat = "JWT",
                 In = ParameterLocation.Header,
                 Description = "Insira o token JWT aqui"
             });
             options.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
             });
         });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRouting();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administradores
            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };


                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }


            //validação de login e senha
            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministrador administradorServico) =>
            {
                var adm = administradorServico.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administradores");

            //Retorna todos os administradores
            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministrador administradorServico) =>
            {
                var adms = new List<AdministradorModelView>();
                var administradores = administradorServico.Todos(pagina);
                foreach (var adm in administradores)
                {
                    adms.Add(new AdministradorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(adms);

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            //Retorna apenas um adminstrador
            endpoints.MapGet("/administradores{id}/", ([FromRoute] int id, IAdministrador administradorServico) =>
            {
                var administrador = administradorServico.BuscaPorId(id);
                if (administrador == null) return Results.NotFound();
                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            // Cria um Administrador e retorna erros de validação caso algum campo fique vazio
            endpoints.MapPost("/administradores", ([FromBody] AdminstradorDTO administradorDTO, IAdministrador administradorServico) =>
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



                var administrador = new Administrador
                {

                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                administradorServico.Incluir(administrador);

                return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });


            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");
            #endregion

            #region Veiculos
            // Função que valida o nome, marca e ano de um veiculo
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

            // Cria um veiculo
            endpoints.MapPost("/veiculos/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculo veiculoServico) =>
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

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");

            // Retorna todos os veiculos regitrados no banco
            endpoints.MapGet("/veiculos/", ([FromQuery] int? pagina, IVeiculo veiculoServico) =>
            {
                var veiculos = veiculoServico.Todos(pagina);


                return Results.Ok(veiculos);

            }).RequireAuthorization().WithTags("Veiculos");

            // Retorna apenas um veiculo
            endpoints.MapGet("/veiculos{id}/", ([FromRoute] int id, IVeiculo veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();
                return Results.Ok(veiculo);

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");

            // Atualiza um veiculo
            endpoints.MapPut("/veiculos{id}/", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculo veiculoServico) =>
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

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");

            // Deleta um veiculo
            endpoints.MapDelete("/veiculos{id}/", ([FromRoute] int id, IVeiculo veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null) return Results.NotFound();



                veiculoServico.Apagar(veiculo);


                return Results.NoContent();

            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");
            #endregion
        });
    }
}
