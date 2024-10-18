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


var builder = WebApplication.CreateBuilder(args);
string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";


#region Configuraci�n de la cadena de conexi�n
// ------- Azure ------------- 
// string connectionString = "Server=tcp:ghu95zexx2.database.windows.net,1433;Initial Catalog=DataObraBeta001;Persist Security Info=False;User ID=joselaks;Password=Santorini2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"
// ------- Unpaz -------------
// string connectionString = "Data Source=UEJINF-P2-19\\TEST01;Initial Catalog=DataObraBeta001;User ID=sa;Password=santorini2010;Encrypt=False"
// ----Notebook Lenovo Jos� --
string connectionString = "Data Source=LENOVO-JOSE;Initial Catalog=MiBaseDeDatos;Integrated Security=True;Encrypt=False";
#endregion

#region Area de servicios

// Configura la conexi�n a la base
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

// Configurar servicios de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Servidor DataObra", Version = "v1" });
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

app.MapGet("/", () => "Hello World!");

#endregion

app.Run();
