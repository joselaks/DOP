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


var builder = WebApplication.CreateBuilder(args);
string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";


#region Configuración de la cadena de conexión
// ------- Azure ------------- 
string connectionString = "Server=tcp:ghu95zexx2.database.windows.net,1433;Initial Catalog=DataObraBeta001;Persist Security Info=False;User ID=joselaks;Password=Santorini2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
// ------- Unpaz -------------
// string connectionString = "Data Source=UEJINF-P2-19\\TEST01;Initial Catalog=DataObraBeta001;User ID=sa;Password=santorini2010;Encrypt=False";
// ----Notebook Lenovo José --
// string connectionString = "Data Source=LENOVO-JOSE;Initial Catalog=DataObraBetaTest;Integrated Security=True;Encrypt=False";
#endregion

#region Area de servicios

// Configura la conexión a la base
//builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

//Agrega el servicio del repositorio al contenedor de dependencias
builder.Services.AddSingleton(new rDocumentos(connectionString));
builder.Services.AddSingleton(new rUsuarios(connectionString));

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

}); ;

#endregion

var app = builder.Build();

#region Area de los middlewere

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

var usu = app.MapGroup("/usuarios");
var doc = app.MapGroup("/documentos");
var dod = app.MapGroup("/documentosdet");
var agr = app.MapGroup("/agrupadores");

usu.MapGet("validacion/", async (string email, string pass, rUsuarios repo) =>
{
    var respuesta = await repo.VerificaUsuario(email, pass);
    return respuesta != null ? Results.Ok(respuesta) : Results.NotFound(new { Mensaje = "Usuario no encontrado o credenciales incorrectas." });
});
doc.MapPost("/", async (rDocumentos repositorio, Documento documento) =>
{
var nuevoDocumento = await repositorio.InsertarDocumentoAsync(documento);
return Results.Created($"documentos/{nuevoDocumento}", nuevoDocumento);
}).RequireAuthorization();
doc.MapDelete("/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var resultado = await repositorio.EliminarDocumentoAsync(id);

    if (resultado)
    {
        // Si se eliminó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Documento eliminado exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para eliminar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró el documento para eliminar." });
    }
}).RequireAuthorization();
doc.MapDelete("rel/{supID:int}/{infID:int}", async (rDocumentos repositorio, int supID, int infID) =>
{
    var resultado = await repositorio.EliminarDocumentoRelAsync(supID, infID);

    if (resultado)
    {
        // Si se eliminó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Relación eliminada exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para eliminar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró relación para eliminar." });
    }
}).RequireAuthorization();
doc.MapPost("/rel/", async (rDocumentos repositorio, DocumentoRel rel) =>
{
    var nuevoDocumento = await repositorio.InsertarDocumentoRelAsync(rel);

    if (nuevoDocumento)
    {
        // Si se eliminó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Documento agregado exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para eliminar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se pudo agregar el registro. Posiblemente ya existía" });
    }

}).RequireAuthorization();
doc.MapGet("/cuenta/{cuentaID:int}", async (rDocumentos repositorio, int cuentaID) =>
{
    var documentos = await repositorio.ObtenerDocumentosPorCuentaIDAsync(cuentaID);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos con el CuentaID proporcionado." });
}).RequireAuthorization();
doc.MapGet("/rel/{superiorID:int}", async (rDocumentos repositorio, int superiorID) =>
{
    var documentos = await repositorio.ObtenerDocumentosRelPorSuperiorIDAsync(superiorID);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos relacionados." });
}).RequireAuthorization();
doc.MapGet("/id/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var documento = await repositorio.ObtenerDocumentosPorIDAsync(id);
    if (documento != null)
    {
        // Devuelve el documento buscado
        return Results.Ok(documento);
    }
    else
    {
        // no lo encontró y devuelve null
        return Results.Ok(null);
    }
}).RequireAuthorization();
doc.MapPut("/", async (rDocumentos repositorio, Documento documento) =>
{
    var resultado = await repositorio.ActualizarDocumentoAsync(documento);

    if (resultado)
    {
        // Si se eliminó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Documento modificado exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para eliminar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró el documento para modificar." });
    }
}).RequireAuthorization();

agr.MapPost("/", async (rDocumentos repositorio, AgrupadorAPI agrupador) =>
{
    var nuevoAgrupador = await repositorio.InsertarAgrupadorAsync(agrupador);
    return Results.Created($"agrupador/{nuevoAgrupador}", nuevoAgrupador);
}).RequireAuthorization();
agr.MapPut("/", async (rDocumentos repositorio, AgrupadorAPI agrupador) =>
{
    var resultado = await repositorio.ActualizarAgrupadorAsync(agrupador);

    if (resultado)
    {
        // Si se modificó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Agrupador modificado exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para modificar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró el agrupador para modificar." });
    }
}).RequireAuthorization();
agr.MapGet("/cuenta/{cuentaID:int}", async (rDocumentos repositorio, int cuentaID) =>
{
    var documentos = await repositorio.ObtenerAgrupadorPorCuentaIDAsync(cuentaID);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron agrupadores con el CuentaID proporcionado." });
}).RequireAuthorization();
agr.MapDelete("/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var resultado = await repositorio.EliminarAgrupadorAsync(id);

    if (resultado)
    {
        // Si se eliminó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = "Agrupador eliminado exitosamente." });
    }
    else
    {
        // Si no se encontró el documento para eliminar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró agrupador para eliminar." });
    }
}).RequireAuthorization();

dod.MapPost("/", async (rDocumentos repositorio, DocumentoDet documento) =>
{
    var nuevoDocumento = await repositorio.InsertarDocumentoDetAsync(documento);
    return Results.Created($"documentosdet/{nuevoDocumento}", nuevoDocumento);
}).RequireAuthorization();


dod.MapGet("/{fieldName}/{id:int}", async (rDocumentos repositorio, int id, string fieldName) =>
{
    var documentos = await repositorio.ObtenerDocumentosDetPorCampoAsync(id, fieldName);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos con el CuentaID proporcionado." });
});

dod.MapPut("/", async (rDocumentos repositorio, DocumentoDet documento) =>
{
    var resultado = await repositorio.ActualizarOEliminarDocumentoDetAsync(documento);
    if (resultado != null)
    {
        // Si se modificó, devolvemos un estado 200 (Ok) con éxito y mensaje
        return Results.Ok(new { Success = true, Message = resultado });
    }
    else
    {
        // Si no se encontró el documento para modificar, devolvemos un estado estado 200 (Ok) con mensaje avisando que no lo encontró
        return Results.Ok(new { Success = false, Message = "No se encontró el registro para modificar." });
    }


}).RequireAuthorization();






#endregion


app.Run();

