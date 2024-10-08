using DataObra.Agrupadores;
using DataObra.Documentos;
using DataObra.Sur;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sistema
{
    public class Servidor
    {
        #region LOGUEADO
        public string Cuenta { get; set; }
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public int SolapaActiva { get; set; }
        public int UsuariosActivos { get; set; } // Maximo de usuarios Activos de la Cuenta
        #endregion

        #region COLECCIONES

        public ObservableCollection<SurUsuario> Usuarios { get; private set; }

        //public ObservableCollection<SysClasificador> Clasificadores { get; set; }
        public ObservableCollection<Agrupador> Agrupadores { get; private set; }
        public ObservableCollection<Documento> Documentos { get; private set; }

        #endregion

        public Servidor()
        {
            Cuenta = "Sur";
            UsuarioID = 1;
            Usuario = "SF";
            SolapaActiva = 2;
            UsuariosActivos = 3;
            Usuarios = new ObservableCollection<SurUsuario>();
            Agrupadores = new ObservableCollection<Agrupador>();
            Documentos = new ObservableCollection<Documento>();

            InitializeUsuarios();
            InitializeAgrupadores();
            InitializeClasificadores();
            InitializeDocumentos();
        }

        #region Carga Colecciones
        private void InitializeDocumentos()
        {
            Documentos = new ObservableCollection<Documento>
            {
                // Carga documentos
                new Documento { ObraID = 1, AdminID = 10, EntidadID = 200, ID = 1, TipoID = 1, Descrip = "Factura 1", Active = true, Numero1 = 1, Fecha1 = DateTime.Now, UsuarioID = 1, RevisadoID = 1, CreadoFecha = DateTime.Now.AddDays(-30), EditadoFecha = DateTime.Now.AddDays(-20), RevisadoFecha = DateTime.Now.AddDays(-10) },
                new Documento { ObraID = 1, AdminID = 10, EntidadID = 201, ID = 2, TipoID = 1, Descrip = "Factura de Compra 2", Active = true, Numero1 = 2, Fecha1 = DateTime.Now, Pesos = 5000.00m, UsuarioID = 1, CreadoFecha = DateTime.Now.AddDays(-29), EditadoFecha = DateTime.Now.AddDays(-19), RevisadoFecha = DateTime.Now.AddDays(-9) },
                new Documento { ObraID = 2, AdminID = 10, EntidadID = 101, ID = 3, TipoID = 2, Descrip = "Reporte 3", Active = false, Numero1 = 3, Fecha1 = DateTime.Now.AddDays(-10), Fecha2 = DateTime.Now.AddDays(20), Materiales = 300.50m, ManodeObra = 150.25m, Pesos = 0, UsuarioID = 2, Dolares = 233.49m, CreadoFecha = DateTime.Now.AddDays(-28), EditadoFecha = DateTime.Now.AddDays(-18), RevisadoFecha = DateTime.Now.AddDays(-8) },
                new Documento { ObraID = 2, AdminID = 11, EntidadID = 301, ID = 4, TipoID = 2, Descrip = "Nota de Crédito 4", Active = true, Numero1 = 4, Fecha1 = DateTime.Now.AddDays(-25), Notas = "Nota relacionada con el pedido 2", Impuestos = 100.00m, Pesos = 1000.00m, UsuarioID = 2, CreadoFecha = DateTime.Now.AddDays(-27), EditadoFecha = DateTime.Now.AddDays(-17), RevisadoFecha = DateTime.Now.AddDays(-7) },
                new Documento { AdminID = 11, EntidadID = 401, ID = 5, TipoID = 3, Descrip = "Presupuesto 5", Active = true, Numero1 = 5, Concepto1 = "Concepto Presupuesto", Fecha1 = DateTime.Now.AddMonths(-1), Subcontratos = 1200.00m, Equipos = 750.00m, Pesos = 1950.00m, RelRub = true, UsuarioID = 3, CreadoFecha = DateTime.Now.AddDays(-26), EditadoFecha = DateTime.Now.AddDays(-16), RevisadoFecha = DateTime.Now.AddDays(-6) },
                new Documento { ObraID = 3, AdminID = 12, EntidadID = 202, ID = 6, TipoID = 1, Descrip = "Factura 6", Active = true, Numero1 = 6, Fecha1 = DateTime.Now, Pesos = 6000.00m, UsuarioID = 1, CreadoFecha = DateTime.Now.AddDays(-25), EditadoFecha = DateTime.Now.AddDays(-15), RevisadoFecha = DateTime.Now.AddDays(-5) },
                new Documento { ObraID = 3, AdminID = 12, EntidadID = 102, ID = 7, TipoID = 2, Descrip = "Reporte 7", Active = true, Numero1 = 7, Fecha1 = DateTime.Now.AddDays(-5), Fecha2 = DateTime.Now.AddDays(15), Materiales = 400.50m, ManodeObra = 250.75m, Pesos = 650.25m, UsuarioID = 2, CreadoFecha = DateTime.Now.AddDays(-24), EditadoFecha = DateTime.Now.AddDays(-14), RevisadoFecha = DateTime.Now.AddDays(-4) },
                new Documento { ObraID = 4, AdminID = 13, EntidadID = 202, ID = 8, TipoID = 3, Descrip = "Presupuesto 8", Active = true, Numero1 = 8, Concepto1 = "Concepto Presupuesto", Fecha1 = DateTime.Now.AddMonths(-2), Subcontratos = 2200.00m, Equipos = 850.00m, Pesos = 3050.00m, RelRub = true, UsuarioID = 3, CreadoFecha = DateTime.Now.AddDays(-23), EditadoFecha = DateTime.Now.AddDays(-13), RevisadoFecha = DateTime.Now.AddDays(-3) }
            };

            // Reemplaza ID x Texto
            var tipoDiccionario = new Dictionary<int, string>
            {
                { 10, "Cliente" },
                { 20, "Proveedor" },
                { 30, "Contratista" },
                { 40, "Obrero" }
            };

            // Cache Agrupadores, Usuarios y Clasificadores
            var agrupadoresDict = Agrupadores.ToDictionary(a => a.ID);
            var usuariosDict = Usuarios.ToDictionary(u => u.ID);
            //var clasificadoresDict = Clasificadores.ToDictionary(c => c.ID);

            foreach (var item in Documentos)
            {
                item.Obra = item.ObraID.HasValue && agrupadoresDict.TryGetValue(item.ObraID.Value, out var obra) ? obra.Descrip : item.Obra;
                item.Admin = item.AdminID.HasValue && agrupadoresDict.TryGetValue(item.AdminID.Value, out var admin) ? admin.Descrip : item.Admin;
                if (item.EntidadID.HasValue && agrupadoresDict.TryGetValue(item.EntidadID.Value, out var entidad))
                {
                    item.Entidad = entidad.Descrip;
                    item.EntidadTipo = tipoDiccionario.TryGetValue(entidad.TipoID, out var tipo) ? tipo : "Otro";
                }
                item.Usuario = usuariosDict.TryGetValue(item.UsuarioID, out var usuario) ? $"{usuario.Nombre} {usuario.Apellido}" : item.Usuario;
                item.Revisado = usuariosDict.TryGetValue(item.RevisadoID, out var verifica) ? $"{verifica.Nombre} {verifica.Apellido}" : item.Revisado;
                //item.TipoDoc = clasificadoresDict.TryGetValue(item.TipoID, out var tipoDoc) ? tipoDoc.Descrip : item.TipoDoc;
            }
        }

        private void InitializeUsuarios()
        {
            Usuarios = new ObservableCollection<SurUsuario>
            {
                new SurUsuario { ID = 1, Nombre = "German", Apellido = "Stahli", Active = true, SesionID = 1 },
                new SurUsuario { ID = 2, Nombre = "Jose", Apellido = "Laks", Active = true, SesionID = 1 },
                new SurUsuario { ID = 3, Nombre = "Alejandra", Apellido = "Barbero", Active = true, SesionID = 1 },
            };
        }

        private void InitializeClasificadores()
        {
            //Clasificadores = new ObservableCollection<SysClasificador>
            //{
            //    new SysClasificador { ID = 1, Descrip = "Factura", Active = true, Tipo = 'D', Numero = 10 },
            //    new SysClasificador { ID = 2, Descrip = "Remito", Active = false, Tipo = 'D', Numero = null },
            //    new SysClasificador { ID = 3, Descrip = "Pedido", Active = true, Tipo = 'D', Numero = 20 },
            //};
        }

        #endregion

        #region AGRUPADORES
        private void InitializeAgrupadores()
        {
            Agrupadores = new ObservableCollection<Agrupador>
            {
                new() { ID = 1, TipoID = 'O', Descrip = "Obra Vettel", Active = true, Numero = 1 },
                new() { ID = 2, TipoID = 'O', Descrip = "Obra Russell", Active = true, Numero = 2 },
                new() { ID = 3, TipoID = 'O', Descrip = "Obra Hamilton", Active = true, Numero = 3 },
                new() { ID = 4, TipoID = 'O', Descrip = "Obra Laks", Active = true, Numero = 3 },
                new() { ID = 10, TipoID = 'A', Descrip = "Admin Perez", Active = true, Numero = 4 },
                new() { ID = 11, TipoID = 'A', Descrip = "Admin Rodriguez", Active = true, Numero = 5 },
                new() { ID = 12, TipoID = 'A', Descrip = "Admin Hamilton", Active = true, Numero = 6 },
                new() { ID = 13, TipoID = 'A', Descrip = "Admin Laks", Active = true, Numero = 6 },
                new() { ID = 101, TipoID = 'C', Descrip = "Rodrigo Diaz", Active = true, Numero = 9 },
                new() { ID = 102, TipoID = 'C', Descrip = "Carlos Alonso", Active = true, Numero = 10 },
                new() { ID = 200, TipoID = 'P', Descrip = "Abelson", Active = true, Numero = 7 },
                new() { ID = 201, TipoID = 'P', Descrip = "Easy", Active = true, Numero = 8 },
                new() { ID = 202, TipoID = 'P', Descrip = "Matyser", Active = true, Numero = 8 },
                new() { ID = 301, TipoID = 'C', Descrip = "Electricista Ramon", Active = true, Numero = 9 },
                new() { ID = 401, TipoID = 'E', Descrip = "Luis Nuñez", Active = true, Numero = 10 },
            };
        }

        public void AddAgrupador(Agrupador agrupador)
        {
            Agrupadores.Add(agrupador);
        }

        public bool RemoveAgrupador(int id)
        {
            var agrupador = Agrupadores.FirstOrDefault(a => a.ID == id);
            if (agrupador != null)
            {
                Agrupadores.Remove(agrupador);
                return true;
            }
            return false;
        }

        public bool UpdateAgrupador(int id, Agrupador newAgrupador)
        {
            var agrupador = Agrupadores.FirstOrDefault(a => a.ID == id);
            if (agrupador != null)
            {
                agrupador.CuentaID = newAgrupador.CuentaID;
                agrupador.UsuarioID = newAgrupador.UsuarioID;
                agrupador.Editado = newAgrupador.Editado;
                agrupador.TipoID = newAgrupador.TipoID;
                agrupador.Descrip = newAgrupador.Descrip;
                agrupador.Active = newAgrupador.Active;
                agrupador.Numero = newAgrupador.Numero;
                return true;
            }
            return false;
        }

        public Agrupador GetFirstAgrupadorByTipoIDOrdered(int tipoID)
        {
            return Agrupadores
                .Where(a => a.TipoID == tipoID)
                .OrderBy(a => a.Descrip)
                .FirstOrDefault();
        }
    }

    #endregion Agrupadores

}

