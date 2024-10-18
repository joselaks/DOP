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


var builder = WebApplication.CreateBuilder(args);
string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";


#region Configuración de la cadena de conexión
// ------- Azure ------------- 
// string connectionString = "Server=tcp:ghu95zexx2.database.windows.net,1433;Initial Catalog=DataObraBeta001;Persist Security Info=False;User ID=joselaks;Password=Santorini2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
// ------- Unpaz -------------
string connectionString = "Data Source=UEJINF-P2-19\\TEST01;Initial Catalog=DataObraBeta001;User ID=sa;Password=santorini2010;Encrypt=False";
// ----Notebook Lenovo José --
//string connectionString = "Data Source=LENOVO-JOSE;Initial Catalog=MiBaseDeDatos;Integrated Security=True;Encrypt=False";
#endregion

#region Area de servicios

// Configura la conexión a la base
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
app.MapGet("/Validacion/{email}/{pass}", async (string email, string pass, IDbConnection db) =>
{
    var respuesta = new CredencialesUsuario();
    var verificado = await db.QueryFirstOrDefaultAsync<Usuario>("VerificaUsuario", new { email, pass },
    commandType: CommandType.StoredProcedure);

    if (verificado != null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var byteKey = Encoding.UTF8.GetBytes(key);

        var tokenDes = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMonths(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey),
            SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDes);

        respuesta.Token = tokenHandler.WriteToken(token);
        respuesta.DatosUsuario = verificado;
        return Results.Ok(respuesta);
    }
    return Results.NotFound();

});




#endregion

app.Run();
