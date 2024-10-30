using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bibioteca.Clases
{
    public class Fiebdc
    {
        public List<Concepto> listaConceptos;
        public List<Relacion> listaRelaciones;
        string[] lineas;
        string origenFiebdc = "";

        public Fiebdc(string _fiebdc)
        {
            listaConceptos = new List<Concepto>();
            listaRelaciones = new List<Relacion>();
            llenaConceptos(_fiebdc);
            llenaRelaciones(_fiebdc);
        }

        private void llenaRelaciones(string _fiebdc)
        {
            lineas = _fiebdc.Split("~".ToCharArray());
            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i];
                if (linea.StartsWith("D"))
                {
                    string CodSupe;
                    string[] vInferiores;
                    decimal CantInfe;
                    string CodInfe;
                    string Cant;
                    string[] vLinea = linea.Split("|".ToCharArray());
                    CodSupe = vLinea[1];
                    {
                        vInferiores = vLinea[2].Split(@"\".ToCharArray());
                        CantInfe = (vInferiores.Length - 1) / 3;
                        for (int z = 1; z <= CantInfe; z++)
                        {
                            Relacion rel = new Relacion();
                            CodInfe = vInferiores[((z * 3) - 3)];
                            rel.Inferior = vInferiores[((z * 3) - 3)];
                            Cant = vInferiores[((z * 3) - 1)];
                            Cant = Cant.Replace(".", ",");
                            if (CodSupe.EndsWith("##")) //dependen del raiz...o sea...es rubro
                            {
                                rel.Superior = null;
                            }
                            else
                            {
                                if (origenFiebdc == "Presto")
                                {
                                    if (CodSupe.EndsWith("#"))
                                    {
                                        rel.Superior = CodSupe.Substring(0, CodSupe.Length - 1);
                                    }
                                    else
                                    {
                                        rel.Superior = CodSupe;

                                    }

                                }
                                else
                                {
                                    rel.Superior = CodSupe;
                                }
                            }

                            try
                            {
                                rel.Cantidad = Convert.ToDecimal(Cant);
                            }
                            catch (Exception)
                            {

                                //Pone cero si da error...
                                rel.Cantidad = 0;
                            }


                            listaRelaciones.Add(rel);
                        }
                    }
                }
            }
        }
        private void llenaConceptos(string _fiebdc)
        {
            lineas = _fiebdc.Split("~".ToCharArray());
            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i];
                if (linea.StartsWith("V|SOFT"))
                {
                    origenFiebdc = "Presto";
                }
                if (linea.StartsWith("C"))
                {
                    string[] vLinea = linea.Split("|".ToCharArray());

                    Concepto registro = new Concepto();
                    registro.Codigo = Convert.ToString(vLinea[1]);
                    if (registro.Codigo.EndsWith("#"))
                    {
                        if (origenFiebdc == "Presto")
                        {
                            registro.Codigo = registro.Codigo.Substring(0, registro.Codigo.Length - 1);
                        }
                    }

                    switch (Convert.ToString(vLinea[6]))
                    {
                        case "0": //Rubro o tarea...no viene por niveles en Fiebdc
                            registro.Tipo = "0";
                            break;
                        case "1": //Mano de obra
                            registro.Tipo = "D";
                            break;
                        case "2":  // Equipos
                            registro.Tipo = "E";
                            break;
                        case "3": //Materiales
                            registro.Tipo = "M";
                            break;
                        default: // Otros
                            registro.Tipo = "O";
                            //registro.Tipo = Convert.ToString(vLinea[6]);
                            break;
                    }

                    string unidad = Convert.ToString(vLinea[2]).Trim();
                    if (unidad.Length > 2)
                    {
                        registro.Unidad = unidad.Substring(0, 2);
                    }
                    else
                    {
                        registro.Unidad = unidad;
                    }

                    string descrip = Convert.ToString(vLinea[3]);
                    if (descrip.Length > 250)
                    {
                        registro.Descrip = descrip.Substring(0, 250);
                    }
                    else
                    {
                        registro.Descrip = descrip;
                    }
                    vLinea[4] = vLinea[4].Replace(".", ",");
                    try
                    {
                        registro.Precio = Convert.ToDecimal(vLinea[4]);

                    }
                    catch (Exception)
                    {
                        registro.Precio = 0;
                    }
                    listaConceptos.Add(registro);
                }
            }
        }
    }
}
