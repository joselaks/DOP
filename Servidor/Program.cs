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


#region Configuraci�n de la cadena de conexi�n
// ------- Azure ------------- 
string connectionString = "Server=tcp:ghu95zexx2.database.windows.net,1433;Initial Catalog=DataObraBeta001;Persist Security Info=False;User ID=joselaks;Password=Santorini2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
// ------- Unpaz -------------
// string connectionString = "Data Source=UEJINF-P2-19\\TEST01;Initial Catalog=DataObraBeta001;User ID=sa;Password=santorini2010;Encrypt=False";
// ----Notebook Lenovo Jos� --
// string connectionString = "Data Source=LENOVO-JOSE;Initial Catalog=DataObraBetaTest;Integrated Security=True;Encrypt=False";
#endregion

#region Area de servicios

// Configura la conexi�n a la base
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
        Description="Servidor API con los servicios necesarios"

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

app.MapGet("/", () => "Hello World!").RequireAuthorization().WithTags("Testeos");
app.MapGet("usuarios/validacion/", async (string email, string pass, rUsuarios repo) =>
{
    var respuesta = await repo.VerificaUsuario(email, pass);
    return respuesta;

}).WithOpenApi(opciones=>
{
    opciones.Summary = "Validacion de usuarios";
    opciones.Description = "Devuelve un objeto con el Token y el registro del usuario completo";
    opciones.Parameters[0].Description = "Email del usuario";
    opciones.Parameters[1].Description = "Contrase�a";
    return opciones;
}).WithTags("Usuarios");

app.MapPost("documentos/", async (rDocumentos repositorio, Documento documento) =>
{
    var nuevoDocumento = await repositorio.InsertarDocumentoAsync(documento);
    return Results.Created($"documentos/{nuevoDocumento}", nuevoDocumento);
}).WithTags("Documentos").WithName("InsertarDocumento");

app.MapDelete("/{id:int}", async (rDocumentos repositorio, int id) =>
{
    var resultado = await repositorio.EliminarDocumentoAsync(id);
    return resultado ? Results.NoContent() : Results.NotFound();
})
.WithTags("Documentos")
.WithName("EliminarDocumento");

app.MapGet("/cuenta/{cuentaID:int}", async (rDocumentos repositorio, int cuentaID) =>
{
    var documentos = await repositorio.ObtenerDocumentosPorCuentaIDAsync(cuentaID);
    return Results.Ok(documentos);
})
.WithTags("Documentos")
.WithName("ObtenerDocumentosPorCuentaID");

app.MapGet("/ID/{ID:int}", async (rDocumentos repositorio, int ID) =>
{
    var documentos = await repositorio.ObtenerDocumentosPorIDAsync(ID);
    return Results.Ok(documentos);
})
.WithTags("Documentos")
.WithName("ObtenerDocumentosPorID");

app.MapPut("/{id:int}", async (rDocumentos repositorio, int id, Documento documento) =>
{
    if (id != documento.ID)
    {
        return Results.BadRequest("El ID del documento no coincide con el ID del par�metro.");
    }

    var resultado = await repositorio.ActualizarDocumentoAsync(documento);
    return resultado ? Results.NoContent() : Results.NotFound();
})
.WithTags("Documentos")
.WithName("ActualizarDocumento");



app.Run();




#endregion


