using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
{
    public static class ConvierteDoc
    {
        //public static Documento Convertir(DocumentoDTO dto)
        //{
        //    return new Documento
        //    {
        //        ID = dto.ID,
        //        CuentaID = dto.CuentaID ?? 0,
        //        TipoID = dto.TipoID,
        //        //Tipo = TxtTipoDoc(dto.TipoID),
        //        UsuarioID = dto.UsuarioID,
        //        //Usuario = App.ListaAgrupadores.FirstOrDefault(a => a.ID).Descrip,
        //        CreadoFecha = dto.CreadoFecha,
        //        EditadoID = dto.EditadoID,
        //        Editado = ObtenerTextoPorID(dto.EditadoID),
        //        EditadoFecha = dto.EditadoFecha,
        //        AutorizadoID = dto.AutorizadoID,
        //        Autorizado = ObtenerTextoPorID(dto.AutorizadoID),
        //        AutorizadoFecha = dto.AutorizadoFecha,
        //        AdminID = dto.AdminID,
        //        Admin = ObtenerTextoPorID(dto.AdminID),
        //        ObraID = dto.ObraID,
        //        Obra = ObtenerTextoPorID(dto.ObraID),
        //        RubroID = dto.RubroID,
        //        Rubro = ObtenerTextoPorID(dto.RubroID),
        //        EntidadID = dto.EntidadID,
        //        Entidad = ObtenerTextoPorID(dto.EntidadID),
        //        EntidadTipo = ObtenerTextoPorID(dto.EntidadID), // Ver
        //        DepositoID = dto.DepositoID,
        //        Deposito = ObtenerTextoPorID(dto.DepositoID),
        //        Presupuesto = ObtenerTextoPorID(dto.PresupuestoID),
        //        Descrip = dto.Descrip ?? string.Empty,
        //        Concepto1 = dto.Concepto1,
        //        Fecha1 = dto.Fecha1,
        //        Fecha2 = dto.Fecha2,
        //        Fecha3 = dto.Fecha3,
        //        Numero1 = dto.Numero1,
        //        Numero2 = dto.Numero2,
        //        Numero3 = dto.Numero3,
        //        Notas = dto.Notas ?? string.Empty,
        //        Active = dto.Active,
        //        Pesos = dto.Pesos,
        //        Dolares = dto.Dolares,
        //        Impuestos = dto.Impuestos,
        //        ImpuestosD = dto.ImpuestosD,
        //        Materiales = dto.Materiales,
        //        ManodeObra = dto.ManodeObra,
        //        Subcontratos = dto.Subcontratos,
        //        Equipos = dto.Equipos,
        //        Otros = dto.Otros,
        //        MaterialesD = dto.MaterialesD,
        //        ManodeObraD = dto.ManodeObraD,
        //        SubcontratosD = dto.SubcontratosD,
        //        EquiposD = dto.EquiposD,
        //        OtrosD = dto.OtrosD,
        //        RelDoc = dto.RelDoc,
        //        RelArt = dto.RelArt,
        //        RelMov = dto.RelMov,
        //        RelImp = dto.RelImp,
        //        RelRub = dto.RelRub,
        //        RelTar = dto.RelTar,
        //        RelIns = dto.RelIns,
        //        Accion = 'A', // Por defecto se asume que es una nueva alta // Tiene sentido aqui?
        //        DetalleDocumento = new List<DocumentoDet>(), // Se inicializa vacío
        //        DetalleMovimientos = new List<Movimiento>(),
        //        DetalleImpuestos = new List<Impuesto>()
        //    };
        //}

        private static string ObtenerTextoPorID(int? id)
        {
            return id.HasValue ? $"Texto para ID {id.Value}" : string.Empty;
        }

        //public static string TxtTipoDoc(byte pID)
        //{
        //    switch (pID)
        //    {
        //        case 1: return "Factura";  // F
        //        case 2: return "Pago";  // P
        //        case 3: return "Pedido";  // O
        //        case 4: return "Plan";  // N
        //        case 5: return "Parte"; // E
        //        case 6: return "Certificado";  // D
        //        case 7: return "Compra";  // C
        //        case 8: return "Remito";  // R
        //        case 9: return "Acopio";  // A
        //        case 10: return "Presupuesto";  // P
        //        case 11: return "Cobro";  // C
        //        default: return "Desconocido";
        //    }
        //}

        //private static readonly Dictionary<char, string> dictionary = new()
        //{
        //    { 'O', "Obra" },
        //    { 'A', "Administración" },
        //    { 'C', "Cliente" },
        //    { 'P', "Proveedor" },
        //    { 'E', "Empleado" },
        //    { 'S', "SubContratista" },
        //    { 'U', "Cuenta" },
        //    { 'D', "Deposito" },
        //    { 'I', "Impuesto" },
        //    { 'T', "Tema" }
        //};
        //private readonly Dictionary<char, string> tiposAgrupadores = dictionary;


    }
}
