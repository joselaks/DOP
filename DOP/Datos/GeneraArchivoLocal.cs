using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace DOP.Datos
{
    public class GeneraArchivoLocal
        {
        private string _cadenaConexion = string.Empty;
        private string _archivoCompleto = string.Empty;

        public GeneraArchivoLocal(string archivo)
            {
            string path = Directory.GetCurrentDirectory();
            _archivoCompleto = Path.Combine(path, archivo);

            _cadenaConexion = $"Data Source={_archivoCompleto}";


            CrearArchivoYTablaSiNoExiste();
            }

        private void CrearArchivoYTablaSiNoExiste()
            {
            // Si el archivo no existe, se crea automáticamente al abrir la conexión
            bool crearTabla = !File.Exists(_archivoCompleto);

            using (var connection = new SqliteConnection(_cadenaConexion))
                {
                connection.Open();

                if (crearTabla)
                    {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Usuario (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT NOT NULL,
                            Password TEXT NOT NULL
                        );
                    ";
                    command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
