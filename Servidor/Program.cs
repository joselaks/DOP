using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Biblioteca;
using Servidor.Utilidades;
using Servidor.Repositorios;
using Bibioteca.Clases;
using Microsoft.Win32;
using Servidor;
using Biblioteca.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";

// Registrar la restricción de ruta personalizada
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("short", typeof(ShortRouteConstraint));
});

#region Configuración de la cadena de conexión
// ------- Azure ------------- 
string connectionString = "Server=tcp:ghu95zexx2.database.windows.net,1433;Initial Catalog=DOP01;Persist Security Info=False;User ID=joselaks;Password=Santorini2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
// ------- Unpaz -------------
// string connectionString = "Data Source=UEJINF-P2-19\\TEST01;Initial Catalog=DataObraBeta001;User ID=sa;Password=santorini2010;Encrypt=False";
// ----Notebook Lenovo José --
// string connectionString = "Data Source=LENOVO-JOSE;Initial Catalog=DataObraBetaTest;Integrated Security=True;Encrypt=False";
#endregion

#region Area de servicios

// Configura la conexión a la base
//builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

//Agrega el servicio del repositorio al contenedor de dependencias
builder.Services.AddSingleton(new rUsuarios(connectionString));
builder.Services.AddSingleton(new rPresupuestos(connectionString));
builder.Services.AddSingleton(new rInsumos(connectionString));

// Configurar servicios de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Servidor DataObra",
        Version = "v1",
        Description = "Servidor API con los servicios necesarios"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.OperationFilter<FiltroAutorizacion>();
});
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    var signigKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var signingCredentials = new SigningCredentials(signigKey, SecurityAlgorithms.HmacSha256Signature);
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signigKey
    };
});

#endregion

var app = builder.Build();

#region Area de los middlewere

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "Conexión establecida" }));


#region Grupo de rutas: /usuarios

var usu = app.MapGroup("/usuarios");

usu.MapGet("validacion/", async (string email, string pass, rUsuarios repo) =>
{
    var respuesta = await repo.VerificaUsuario(email, pass);
    if (respuesta.DatosUsuario != null)
    {
        return Results.Ok(respuesta);
    }
    else if (!string.IsNullOrEmpty(respuesta.ErrorMessage))
    {
        return Results.BadRequest(new { Mensaje = respuesta.ErrorMessage });
    }
    else
    {
        return Results.NotFound(new { Mensaje = "Usuario no encontrado o credenciales incorrectas." });
    }
});



#endregion

#region Grupo de rutas: /pre

var pre = app.MapGroup("/presupuestos");

pre.MapGet("/usuario/{usuarioID:int}", async (int usuarioID, rPresupuestos repo) =>
{
    try
        {
        var presupuestos = await repo.ListarPresupuestosPorUsuarioAsync(usuarioID);
        return Results.Ok(presupuestos);
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Message = ex.Message });
        }
}).RequireAuthorization();

pre.MapPost("/procesar", async (ProcesaPresupuestoRequest request, rPresupuestos repo) =>
{
    try
        {
        var id = await repo.ProcesarPresupuestoAsync(request.Presupuesto, request.Conceptos, request.Relaciones);
        return Results.Ok(new { Success = true, PresupuestoID = id, Message = "Presupuesto procesado exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();




pre.MapGet("/{presupuestoID:int}", async (int presupuestoID, rPresupuestos repo) =>
{
    try
        {
        var (conceptos, relaciones) = await repo.ObtenerConceptosYRelacionesAsync(presupuestoID);
        return Results.Ok(new { Conceptos = conceptos, Relaciones = relaciones });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Message = ex.Message });
        }
}).RequireAuthorization();

pre.MapDelete("/{presupuestoID:int}", async (int presupuestoID, rPresupuestos repo) =>
{
    try
        {
        await repo.BorrarPresupuestoAsync(presupuestoID);
        return Results.Ok(new { Success = true, Message = "Presupuesto eliminado exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();




#endregion

#region Grupo de rutas: /ins

var ins = app.MapGroup("/insumos");

ins.MapGet("/usuario/{usuarioID:int}", async (int usuarioID, rInsumos repo) =>
{
    try
        {
        var insumos = await repo.ListarInsumosPorUsuarioAsync(usuarioID);
        return Results.Ok(insumos);
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Message = ex.Message });
        }
}).RequireAuthorization();

ins.MapPost("/procesar", async (ProcesaInsumoRequest request, rInsumos repo) =>
{
    try
        {
        var id = await repo.ProcesarInsumoAsync(request.Insumo, request.Articulos);
        return Results.Ok(new { Success = true, InsumoID = id, Message = "Insumo procesado exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

ins.MapDelete("/{insumoID:int}", async (int insumoID, rInsumos repo) =>
{
    try
        {
        await repo.EliminarInsumoYArticulosRelAsync(insumoID);
        return Results.Ok(new { Success = true, Message = "Insumo y artículos relacionados eliminados exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

ins.MapPost("/articulos/procesar", async (ProcesaArticulosListaRequest request, rInsumos repo) =>
{
    try
        {
        var id = await repo.ProcesarArticulosListaAsync(request.Lista, request.Articulos);
        return Results.Ok(new { Success = true, ListaID = id, Message = "Lista de artículos procesada exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

ins.MapDelete("/articulos/{listaID:int}", async (int listaID, rInsumos repo) =>
{
    try
        {
        await repo.EliminarArticulosListaYArticulosAsync(listaID);
        return Results.Ok(new { Success = true, Message = "Lista y artículos relacionados eliminados exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

ins.MapGet("/articulos/{insumoID:int}", async (int insumoID, rInsumos repo) =>
{
    try
        {
        var articulos = await repo.ObtenerArticulosPorInsumoAsync(insumoID);
        return Results.Ok(articulos);
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();


pre.MapGet("/maestro/{usuarioID:int}", async (int usuarioID, rPresupuestos repo) =>
{
    try
        {
        var (conceptos, relaciones) = await repo.ObtenerConceptosYRelacionesMaestroAsync(usuarioID);
        return Results.Ok(new { Conceptos = conceptos, Relaciones = relaciones });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Message = ex.Message });
        }
}).RequireAuthorization();

pre.MapPost("/maestro/procesar", async (ProcesaTareaMaestroRequest request, rPresupuestos repo) =>
{
    try
        {
        var mensaje = await repo.ProcesarTareaMaestroAsync(request.UsuarioID, request.Conceptos, request.Relaciones);
        return Results.Ok(new { Success = mensaje == "OK", Message = mensaje });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

#endregion

#endregion

app.Run();


