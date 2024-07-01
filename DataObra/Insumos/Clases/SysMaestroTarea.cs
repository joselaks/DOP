using System;

namespace DataObra.Insumos.Clases
{
    public class SysMaestroTarea
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int UsuarioID { get; set; } 
        public DateTime Editado { get; set; }
        #endregion
        #region DATOS
        public int TipoID { get; set; } // Hay tipos?
        public string Descrip { get; set; }
        public string Unidad { get; set; }
        public string Codigo { get; set; }
        public string Memoria { get; set; } // Memoria descriptiva de la Tarea
        public int RubroGenericoID { get; set; } 
        public int RubroID { get; set; }
        public int ZonaID { get; set; } // Zona de los precios
        #endregion
        #region VALORES
        public decimal Pesos { get; set; }
        public decimal Dolares { get; set; }
        #endregion
        #region RELACIONES
        public bool RelIns { get; set; } // Si tiene insumos (o es global...)
        #endregion
    }

    class SysMaestroRelacion  // Relacion de Tarea con Insumos del Maestro DO
    {
        public int TareaID { get; set; }
        public int InsumoID { get; set; }
        public decimal Cantidad { get; set; }
    }
}
