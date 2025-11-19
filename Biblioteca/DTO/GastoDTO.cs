using System;

namespace Biblioteca.DTO
{
    public class GastoDTO
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }               // Se mantiene int por compatibilidad con repositorio/API
        public int UsuarioID { get; set; }
        public byte TipoID { get; set; }                // Corresponde a Documentos.TipoID (tinyint NOT NULL)
        public DateTime FechaDoc { get; set; }
        public DateTime FechaCreado { get; set; }
        public DateTime FechaEditado { get; set; }
        public string? Entidad { get; set; }
        public string? Documento { get; set; }
        public string? Descrip { get; set; }
        public string? Notas { get; set; }
        public decimal Importe { get; set; }
        public char Moneda { get; set; }                // CHAR(1)
        public decimal TipoCambioD { get; set; } = 1.0000000000m; // Coincide con Documentos.TipoCambioD (decimal(19,10))
    }
}

