using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
    {
    public class Gasto : ObjetoNotificable
        {
        #region Estructura

        public GastoDTO encabezado;
        public List<GastoDetalleDTO> detalle;

        #endregion

        }
    }
