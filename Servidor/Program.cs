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
using ProcesarArbolPresupuestoRequest = Bibioteca.Clases.ProcesarArbolPresupuestoRequest;
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
builder.Services.AddSingleton(new rPresupuestos(connectionString));

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

#region Grupo de rutas: /documentos

var doc = app.MapGroup("/documentos");

doc.MapPost("/", async (rDocumentos repositorio, DocumentoDTO documento) =>
{
    var (id, errorMessage) = await repositorio.InsertarDocumentoAsync(documento);
    if (id != 0)
    {
        return Results.Created($"documentos/{id}", new { Id = id, Message = "Documento creado exitosamente." });
    }
    else
    {
        return Results.BadRequest(new { Message = errorMessage });
    }
}).RequireAuthorization();



doc.MapPost("/procesar", async (rDocumentos repositorio, InfoDocumento infodocumento) =>
{
    try
    {
        await repositorio.ProcesarInfoDocumentoAsync(infodocumento);
        return Results.Ok(new { Success = true, Message = "Documento procesado exitosamente." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
}).RequireAuthorization();

/// ajustar

doc.MapDelete("/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var (success, message) = await repositorio.EliminarDocumentoAsync(id);

    if (success)
    {
        return Results.Ok(new { Success = true, Message = "Documento eliminado exitosamente." });
    }
    else
    {
        return Results.BadRequest(new { Success = false, Message = message });
    }
}).RequireAuthorization();

doc.MapPut("/", async (rDocumentos repositorio, DocumentoDTO documento) =>
{
    var (success, message) = await repositorio.ActualizarDocumentoAsync(documento);

    if (success)
    {
        return Results.Ok(new { Success = true, Message = message });
    }
    else
    {
        return Results.BadRequest(new { Success = false, Message = message });
    }
}).RequireAuthorization();


// hasta acá actualizados



doc.MapDelete("rel/{supID:int}/{infID:int}", async (rDocumentos repositorio, int supID, int infID) =>
{
    var resultado = await repositorio.EliminarDocumentoRelAsync(supID, infID);

    if (resultado)
    {
        return Results.Ok(new { Success = true, Message = "Relación eliminada exitosamente." });
    }
    else
    {
        return Results.Ok(new { Success = false, Message = "No se encontró relación para eliminar." });
    }
}).RequireAuthorization();








//doc.MapPost("/rel/", async (rDocumentos repositorio, DocumentoRel rel) =>
//{
//    var nuevoDocumento = await repositorio.InsertarDocumentoRelAsync(rel);

//    if (nuevoDocumento)
//    {
//        return Results.Ok(new { Success = true, Message = "Documento agregado exitosamente." });
//    }
//    else
//    {
//        return Results.Ok(new { Success = false, Message = "No se pudo agregar el registro. Posiblemente ya existía" });
//    }
//}).RequireAuthorization();

doc.MapGet("/cuenta/{cuentaID:int}", async (rDocumentos repositorio, int cuentaID) =>
{
    var documentos = await repositorio.ObtenerDocumentosPorCuentaIDAsync(cuentaID);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos con el CuentaID proporcionado." });
}).RequireAuthorization();

//doc.MapGet("/rel/{superiorID:int}", async (rDocumentos repositorio, int superiorID) =>
//{
//    var documentos = await repositorio.ObtenerDocumentosRelPorSuperiorIDAsync(superiorID);
//    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos relacionados." });
//}).RequireAuthorization();

doc.MapGet("/id/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var documento = await repositorio.ObtenerDocumentosPorIDAsync(id);
    if (documento != null)
    {
        return Results.Ok(documento);
    }
    else
    {
        return Results.Ok(null);
    }
}).RequireAuthorization();




#endregion

#region Grupo de rutas: /documentosdet

var dod = app.MapGroup("/documentosdet");

dod.MapPost("/", async (rDocumentos repositorio, DocumentoDet documento) =>
{
    var nuevoDocumento = await repositorio.InsertarDocumentoDetAsync(documento);
    return Results.Created($"documentosdet/{nuevoDocumento}", nuevoDocumento);
}).RequireAuthorization();

dod.MapPost("/procesar", async (rDocumentos repositorio, List<DocumentoDet> listaDetalleDocumento) =>
{
    try
    {
        await repositorio.ProcesarListaDetalleDocumentoAsync(listaDetalleDocumento);
        return Results.Ok(new { Success = true, Message = "Lista de detalles de documentos procesada exitosamente." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
}).RequireAuthorization();

dod.MapGet("/{fieldName}/{id:int}/{cuentaID:short}", async (rDocumentos repositorio, int id, string fieldName, short cuentaID) =>
{
    var documentos = await repositorio.ObtenerDocumentosDetPorCampoAsync(id, fieldName, cuentaID);
    return documentos != null ? Results.Ok(documentos) : Results.NotFound(new { Mensaje = "No se encontraron documentos con el CuentaID proporcionado." });
});


dod.MapPut("/", async (rDocumentos repositorio, DocumentoDet documento) =>
{
    var resultado = await repositorio.ActualizarOEliminarDocumentoDetAsync(documento);
    if (resultado != null)
    {
        return Results.Ok(new { Success = true, Message = resultado });
    }
    else
    {
        return Results.Ok(new { Success = false, Message = "No se encontró el registro para modificar." });
    }
}).RequireAuthorization();

#endregion

#region Grupo de rutas: /movimientos

var mov = app.MapGroup("/movimientos");

mov.MapPost("/procesar", async (rDocumentos repositorio, List<MovimientoDTO> listaMovimientos) =>
{
    try
    {
        await repositorio.ProcesarMovimientosAsync(listaMovimientos);
        return Results.Ok(new { Success = true, Message = "Lista de movimientos procesada exitosamente." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
}).RequireAuthorization();

mov.MapGet("/{fieldName}/{id:int}/{cuentaID:short}", async (rDocumentos repositorio, int id, string fieldName, short cuentaID) =>
{
    var movimientos = await repositorio.ObtenerMovimientosPorCampoAsync(id, fieldName, cuentaID);
    return movimientos != null ? Results.Ok(movimientos) : Results.NotFound(new { Mensaje = "No se encontraron movimientos con el CuentaID proporcionado." });
}).RequireAuthorization();

#endregion



#region Grupo de rutas: /impuestos

var imp = app.MapGroup("/impuestos");

imp.MapPost("/procesar", async (rDocumentos repositorio, List<Impuesto> listaImpuestos) =>
{
    try
    {
        await repositorio.ProcesarImpuestosAsync(listaImpuestos);
        return Results.Ok(new { Success = true, Message = "Lista de impuestos procesada exitosamente." });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
}).RequireAuthorization();

imp.MapGet("/{fieldName}/{id:int}/{cuentaID:short}", async (rDocumentos repositorio, int id, string fieldName, short cuentaID) =>
{
    var impuestos = await repositorio.ObtenerImpuestosPorCampoAsync(id, fieldName, cuentaID);
    return impuestos != null ? Results.Ok(impuestos) : Results.NotFound(new { Mensaje = "No se encontraron impuestos con el CuentaID proporcionado." });
}).RequireAuthorization();

#endregion



#region Grupo de rutas: /agrupadores

var agr = app.MapGroup("/agrupadores");

agr.MapPost("/", async (rDocumentos repositorio, AgrupadorDTO agrupador) =>
{
    var (id, errorMessage) = await repositorio.InsertarAgrupadorAsync(agrupador);
    if (id != 0)
    {
        return Results.Created($"agrupador/{id}", new { Id = id, Message = "Agrupador creado exitosamente." });
    }
    else
    {
        return Results.BadRequest(new { Message = errorMessage });
    }
}).RequireAuthorization();



agr.MapPut("/", async (rDocumentos repositorio, AgrupadorDTO agrupador) =>
{
    var resultado = await repositorio.ActualizarAgrupadorAsync(agrupador);

    if (resultado)
    {
        return Results.Ok(new { Success = true, Message = "Agrupador modificado exitosamente." });
    }
    else
    {
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
    var (success, message) = await repositorio.EliminarAgrupadorAsync(id);

    if (success)
    {
        return Results.Ok(new { Success = true, Message = "Agrupador eliminado exitosamente." });
    }
    else
    {
        return Results.BadRequest(new { Success = false, Message = message });
    }
}).RequireAuthorization();



#endregion

#region Grupo de rutas: /pre

var pre = app.MapGroup("/presupuestos");

pre.MapGet("/{presupuestoID:int}", async (int presupuestoID, rPresupuestos repo) =>
{
    try
        {
        var (conceptos, relaciones) = await repo.ObtenerRegistrosPorPresupuestoIDAsync(presupuestoID);
        return Results.Ok(new { Conceptos = conceptos, Relaciones = relaciones });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Message = ex.Message });
        }
}).RequireAuthorization();



pre.MapPost("/procesar", async (ProcesaPresupuestoDTO request, rPresupuestos repo) =>
{
    try
        {
        await repo.ProcesarArbolPresupuestoAsync(request.PresupuestoID, request.ListaConceptos, request.ListaRelaciones);
        return Results.Ok(new { Success = true, Message = "Árbol de presupuesto procesado exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();

pre.MapDelete("/{presupuestoID:int}", async (int presupuestoID, bool verifica, rPresupuestos repo) =>
{
    try
        {
        await repo.EliminarPresupuestoAsync(presupuestoID, verifica);
        return Results.Ok(new { Success = true, Message = "Presupuesto eliminado exitosamente." });
        }
    catch (Exception ex)
        {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
        }
}).RequireAuthorization();



#endregion

#endregion

app.Run();


