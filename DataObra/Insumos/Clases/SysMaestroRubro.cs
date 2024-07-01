namespace DataObra.Insumos.Clases
{
    public class SysMaestroRubro
    {
        #region SISTEMA
        public int? ID { get; set; }
        public int UsuarioID { get; set; } // Creador
        #endregion
        #region DATOS
        public string Descrip { get; set; }
        public bool Tipo { get; set; } // Rubro o SubRubro?
        #endregion
    }
}
