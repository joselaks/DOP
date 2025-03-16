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

        public async Task<CredencialesUsuarioDTO> VerificaUsuario(string email, string pass)
        {
            var respuesta = new CredencialesUsuarioDTO();
            string key = "ESTALLAVEFUNCOINARIASI12345PARARECORDARLAMEJOR=";
            using (var db = new SqlConnection(_connectionString))
            {
                try
                {
                    var verificado = await db.QuerySingleOrDefaultAsync<UsuarioDTO>("UsuariosGet", new { email, pass },
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
                }
                catch (SqlException ex)
                {
                    // Capturar el mensaje de error del procedimiento almacenado
                    respuesta.DatosUsuario = null;
                    respuesta.Token = null;
                    respuesta.ErrorMessage = $"Error al verificar usuario: {ex.Message}";
                }
                return respuesta;
            }
        }
    }
}
