using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Documentos
{
    class DocumentoRel  // Relaciones entre Documentos
    {
        public int SuperiorID { get; set; }
        public int InferiorID { get; set; }
        public short CuentaID { get; set; }
        public bool PorInsumos { get; set; } = true; // Si la relación esta dada por compartir detalle de insumos
                                                     // o es manual
    }
    public class Documento
    {
        #region SISTEMA
        public int ID { get; set; }  // Int
        public short CuentaID { get; set; } // SmallInt
        public byte TipoID { get; set; } // TinyInt o 1 Char
        public string? TipoDoc { get; set; } // Tipo en texto para mostrar
        #endregion
        #region USUARIOS
        public int UsuarioID { get; set; } // Int Creador inicial
        public string? Usuario { get; set; } // Mostrar
        public DateTime CreadoFecha { get; set; } // Date Fecha del primer guardado en la tabla
        public int EditadoID { get; set; } // Int Ultimo usuario en editar
        public string? Editado { get; set; } // Mostrar usuario
        public DateTime EditadoFecha { get; set; } // Date Ultima modificación
        public int RevisadoID { get; set; } // Int Usuario que Autoriza o Verifica
        public string? Revisado { get; set; } // Mostrar usuario
        public DateTime RevisadoFecha { get; set; } // Date Cuando fue revisado
        #endregion
        #region AGRUPADORES
        public int? AdminID { get; set; } // Int Administracion o Empresa
        public string? Admin { get; set; } // Mostrar
        public int? ObraID { get; set; }  // Int
        public string? Obra { get; set; }  // Mostrar
        public int? PresupuestoID { get; set; }  // Int
        public string? Presupuesto { get; set; } // Mostrar
        public int? RubroID { get; set; } // Int
        public string? Rubro { get; set; } // Mostrar
        public int? EntidadID { get; set; } // ID
        public string? Entidad { get; set; } // Mostrar
        public string? EntidadTipo { get; set; }  // Mostrar 
        public int? DepositoID { get; set; } // Int
        public string? Deposito { get; set; } // Mostrar
        #endregion
        #region DATOS
        public string? Descrip { get; set; } // varchar150  Titulo o descripción del usuario
        public string? Concepto1 { get; set; } // varchar150
        public DateTime Fecha1 { get; set; } // Date Fecha del documento ingresada por el usuario
        public DateTime? Fecha2 { get; set; } // Date Fecha de vencimiento o entrega o ...
        public DateTime? Fecha3 { get; set; } // Date Fecha contable o ...
        public int Numero1 { get; set; } // Int en las facturas en numero de sucursal
        public int Numero2 { get; set; } // Int 
        public int Numero3 { get; set; } // Int numero de archivo u otro
        public string? Notas { get; set; } // Varchar250 Ingresadas por el usuario
        public bool Active { get; set; } // Bool
        #endregion
        #region TOTALES
        public decimal Pesos { get; set; } // Decimal 19.2
        public decimal Dolares { get; set; } // Decimal 19.2
        public decimal Impuestos { get; set; } // Decimal 19.2
        public decimal ImpuestosD { get; set; } // Decimal 19.2
        #endregion
        #region Por Tipo
        public decimal Materiales { get; set; } // Decimal 19.2
        public decimal ManodeObra { get; set; } // Decimal 19.2
        public decimal Subcontratos { get; set; } // Decimal 19.2
        public decimal Equipos { get; set; } // Decimal 19.2
        public decimal Otros { get; set; } // Decimal 19.2
        public decimal MaterialesD { get; set; } // Decimal 19.2
        public decimal ManodeObraD { get; set; } // Decimal 19.2
        public decimal SubcontratosD { get; set; } // Decimal 19.2
        public decimal EquiposD { get; set; } // Decimal 19.2
        public decimal OtrosD { get; set; } // Decimal 19.2
        #endregion
        #region RELACIONES
        public bool RelDoc { get; set; } // Entre documentos
        public bool RelArt { get; set; } // Articulos Detalle
        public bool RelMov { get; set; } // Movimientos de tesorerias, confirmados o previstos
        public bool RelImp { get; set; } // Impuestos (confirmados o previstos?)
        public bool RelRub { get; set; } // Rubros
        public bool RelTar { get; set; } // Tareas
        public bool RelIns { get; set; } // Insumos
        #endregion

        // Método de conversión
        public static Documento Convertir(Biblioteca.Documento docBiblioteca)
        {
            return new Documento
            {
                ID = docBiblioteca.ID,
                CuentaID = docBiblioteca.CuentaID,
                TipoID = docBiblioteca.TipoID,
                UsuarioID = docBiblioteca.UsuarioID,
                CreadoFecha = docBiblioteca.CreadoFecha,
                EditadoID = docBiblioteca.EditadoID,
                EditadoFecha = docBiblioteca.EditadoFecha,
                RevisadoID = docBiblioteca.RevisadoID,
                RevisadoFecha = docBiblioteca.RevisadoFecha,
                AdminID = docBiblioteca.AdminID,
                ObraID = docBiblioteca.ObraID,
                PresupuestoID = docBiblioteca.PresupuestoID,
                RubroID = docBiblioteca.RubroID,
                EntidadID = docBiblioteca.EntidadID,
                DepositoID = docBiblioteca.DepositoID,
                Descrip = docBiblioteca.Descrip,
                Concepto1 = docBiblioteca.Concepto1,
                Fecha1 = docBiblioteca.Fecha1,
                Fecha2 = docBiblioteca.Fecha2,
                Fecha3 = docBiblioteca.Fecha3,
                Numero1 = docBiblioteca.Numero1,
                Numero2 = docBiblioteca.Numero2,
                Numero3 = docBiblioteca.Numero3,
                Notas = docBiblioteca.Notas,
                Active = docBiblioteca.Active,
                Pesos = docBiblioteca.Pesos,
                Dolares = docBiblioteca.Dolares,
                Impuestos = docBiblioteca.Impuestos,
                ImpuestosD = docBiblioteca.ImpuestosD,
                Materiales = docBiblioteca.Materiales,
                ManodeObra = docBiblioteca.ManodeObra,
                Subcontratos = docBiblioteca.Subcontratos,
                Equipos = docBiblioteca.Equipos,
                Otros = docBiblioteca.Otros,
                MaterialesD = docBiblioteca.MaterialesD,
                ManodeObraD = docBiblioteca.ManodeObraD,
                SubcontratosD = docBiblioteca.SubcontratosD,
                EquiposD = docBiblioteca.EquiposD,
                OtrosD = docBiblioteca.OtrosD,
                RelDoc = docBiblioteca.RelDoc,
                RelArt = docBiblioteca.RelArt,
                RelMov = docBiblioteca.RelMov,
                RelImp = docBiblioteca.RelImp,
                RelRub = docBiblioteca.RelRub,
                RelTar = docBiblioteca.RelTar,
                RelIns = docBiblioteca.RelIns
            };
        }

        public static Biblioteca.Documento ConvertirInverso(Documento doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            return new Biblioteca.Documento
            {
                ID = doc.ID,
                CuentaID = doc.CuentaID,
                TipoID = doc.TipoID,
                UsuarioID = doc.UsuarioID,
                CreadoFecha = doc.CreadoFecha,
                EditadoID = doc.EditadoID,
                EditadoFecha = doc.EditadoFecha,
                RevisadoID = doc.RevisadoID,
                RevisadoFecha = doc.RevisadoFecha,
                AdminID = doc.AdminID,
                ObraID = doc.ObraID,
                PresupuestoID = doc.PresupuestoID,
                RubroID = doc.RubroID,
                EntidadID = doc.EntidadID,
                DepositoID = doc.DepositoID,
                Descrip = doc.Descrip,
                Concepto1 = doc.Concepto1,
                Fecha1 = doc.Fecha1,
                Fecha2 = doc.Fecha2,
                Fecha3 = doc.Fecha3,
                Numero1 = doc.Numero1,
                Numero2 = doc.Numero2,
                Numero3 = doc.Numero3,
                Notas = doc.Notas,
                Active = doc.Active,
                Pesos = doc.Pesos,
                Dolares = doc.Dolares,
                Impuestos = doc.Impuestos,
                ImpuestosD = doc.ImpuestosD,
                Materiales = doc.Materiales,
                ManodeObra = doc.ManodeObra,
                Subcontratos = doc.Subcontratos,
                Equipos = doc.Equipos,
                Otros = doc.Otros,
                MaterialesD = doc.MaterialesD,
                ManodeObraD = doc.ManodeObraD,
                SubcontratosD = doc.SubcontratosD,
                EquiposD = doc.EquiposD,
                OtrosD = doc.OtrosD,
                RelDoc = doc.RelDoc,
                RelArt = doc.RelArt,
                RelMov = doc.RelMov,
                RelImp = doc.RelImp,
                RelRub = doc.RelRub,
                RelTar = doc.RelTar,
                RelIns = doc.RelIns
            };
        }

    }
}
