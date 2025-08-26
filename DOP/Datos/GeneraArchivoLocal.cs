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
            // Carpeta segura para datos de usuario
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NombreDeTuAplicacion" // Cambia esto por el nombre de tu app
            );

            // Crea la carpeta si no existe
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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
