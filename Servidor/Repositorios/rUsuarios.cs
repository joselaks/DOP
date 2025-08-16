using Biblioteca;
using Biblioteca.DTO;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Servidor.Repositorios
{
    public class rUsuarios
    {
        private readonly string _connectionString;

        public rUsuarios(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CredencialesUsuarioDTO> VerificaUsuario(string email, string pass, string macaddress)
            {
            var respuesta = new CredencialesUsuarioDTO();
            string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";
            using (var db = new SqlConnection(_connectionString))
                {
                try
                    {
                    var verificado = await db.QuerySingleOrDefaultAsync<UsuarioDTO>(
                        "UsuariosGet",
                        new { email, pass, macaddress },
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
                        }
                    else
                        {
                        respuesta.DatosUsuario = null;
                        respuesta.Token = null;
                        respuesta.ErrorMessage = "Usuario o contraseña incorrectos, o usuario no existe.";
                        }
                    }
                catch (SqlException ex)
                    {
                    respuesta.DatosUsuario = null;
                    respuesta.Token = null;
                    respuesta.ErrorMessage = $"Error al verificar usuario: {ex.Message}";
                    }
                return respuesta;
                }
            }

        public async Task<bool> RegistrarSalidaUsuario(int usuarioId, string macaddress)
            {
            using (var db = new SqlConnection(_connectionString))
                {
                // Obtener la hora actual de Buenos Aires
                var zonaBuenosAires = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
                var fechaHoraBuenosAires = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaBuenosAires);

                var sql = @"
            UPDATE UsuariosLog
            SET Salida = @salida, Resultado = 5
            WHERE UsuarioID = @usuarioId AND Macaddress = @macaddress AND Salida IS NULL";
                var rows = await db.ExecuteAsync(sql, new { usuarioId, macaddress, salida = fechaHoraBuenosAires });
                return rows > 0;
                }
            }

        }
}
