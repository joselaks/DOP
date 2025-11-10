using System;

namespace Biblioteca.DTO
{
    public class GastoDTO
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }               // Cambiado de short a int para homogeneidad con repositorio/SP
        public int UsuarioID { get; set; }
        public DateTime FechaDoc { get; set; }
        public DateTime FechaCreado { get; set; }
        public DateTime FechaEditado { get; set; }
        public string? Entidad { get; set; }
        public string? Documento { get; set; }
        public string? Descrip { get; set; }
        public string? Notas { get; set; }
        public decimal Importe { get; set; }
        public char Moneda { get; set; }
        public decimal TipoCambioD { get; set; } = 1.0000000000m; // Nuevo: coincide con columna NOT NULL decimal(19,10)
    }
}

