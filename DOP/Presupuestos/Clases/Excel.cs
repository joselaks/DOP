using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Syncfusion.XlsIO;

namespace DOP.Presupuestos.Clases
    {
    public class Excel : IDisposable
        {
        Presupuesto oActivo;
        public IWorkbook book;
        private ExcelEngine excelEngine;
        int row;
        int hoja = 0;
        bool PE = false;
        bool PC = false;
        int filainicio;

        public Excel(Presupuesto _oActivo)
            {
            oActivo = _oActivo;
            excelEngine = new ExcelEngine();
            book = excelEngine.Excel.Workbooks.Create(1);
            hoja = 0;
            }

        private IWorksheet ActiveWorksheet => book.Worksheets[hoja];

        public void PresupuestoEjecutivo()
            {
            PE = true;
            hojanueva("Presupuesto Ejecutivo");
            var ws = ActiveWorksheet;

            ws.Range["A1"].Text = "PRESUPUESTO EJECUTIVO";
            ws.Range["A1"].CellStyle.Font.Bold = true;
            ws.Range["A1"].CellStyle.Font.Size = 19;
            ws.Columns[0].ColumnWidth = 40;
            row = 4;

            // Encabezado
            SetHeader(ws, row + 1, new[] { "ITEM", "UNIDAD", "CANTIDAD", "PU", "IMPORTE" }, new[] { 60, 15, 15, 15, 15 });

            row = row + 1;
            filainicio = row;

            Aplanar(oActivo.Arbol, ws);
            Totales(row + 1, ws);

            PE = false;
            }

        public void PresupuestoTipos()
            {
            hojanueva("Presupuesto por Tipo de insumo");
            var ws = ActiveWorksheet;

            ws.Range["A1"].Text = "PRESUPUESTO DE EJECUCION DESGLOSADO POR TIPO DE INSUMO";
            ws.Range["A1"].CellStyle.Font.Bold = true;
            ws.Range["A1"].CellStyle.Font.Size = 19;
            ws.Columns[0].ColumnWidth = 40;
            row = 2;

            SetHeader(ws, row + 1, new[] { "ITEM", "MATERIALES", "MANO DE OBRA", "EQUIPOS", "SUB CONTRATOS", "OTROS", "INDIRECTOS", "TOTALES" }, new[] { 60, 15, 15, 15, 15, 15, 15, 15 });

            row = row + 1;
            filainicio = row;

            AplanarPorTipo(oActivo.Arbol, ws);
            TotalesPorTipo(row + 1, ws);
            }

        public void PresupuestoComercial()
            {
            PC = true;
            hojanueva("Presupuesto Comercial");
            var ws = ActiveWorksheet;

            ws.Range["A1"].Text = "PRESUPUESTO";
            ws.Range["A1"].CellStyle.Font.Bold = true;
            ws.Range["A1"].CellStyle.Font.Size = 19;
            ws.Columns[0].ColumnWidth = 40;
            row = 2;

            SetHeader(ws, row + 1, new[] { "ITEM", "UNIDAD", "CANTIDAD", "PU", "IMPORTE" }, new[] { 60, 18, 18, 18, 18 });

            row = row + 1;
            filainicio = row;

            Aplanar(oActivo.Arbol, ws);
            Totales(row + 1, ws);

            PC = false;
            }

        private void SetHeader(IWorksheet ws, int row, string[] headers, int[] widths)
            {
            for (int i = 0; i < headers.Length; i++)
                {
                ws[row, i + 1].Text = headers[i];
                ws[row, i + 1].CellStyle.Font.Bold = true;
                ws[row, i + 1].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                ws[row, i + 1].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                ws[row, i + 1].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                ws[row, i + 1].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                ws.Columns[i].ColumnWidth = widths[i];
                ws[row, i + 1].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                }
            }

        private void hojanueva(string nombre)
            {
            if (hoja > 0)
                book.Worksheets.Create(nombre);
            book.Worksheets[hoja].Name = nombre;
            }

        private void Aplanar(IEnumerable<Nodo> items, IWorksheet ws)
            {
            foreach (var item in items)
                {
                if (item.Tipo == "R")
                    {
                    ws[row + 1, 1].Text = item.Descripcion;
                    string formula = "";
                    bool primera = true;
                    foreach (var inferiores in item.Inferiores)
                        {
                        int filaSuma = inferiores.OrdenInt + filainicio;
                        if (primera)
                            formula = $"=E{filaSuma}";
                        else
                            formula += $"+E{filaSuma}";
                        primera = false;
                        }
                    ws[row + 1, 5].Formula = formula;
                    ws[row + 1, 1].CellStyle.Font.Bold = true;
                    ws[row + 1, 5].CellStyle.Font.Bold = true;
                    ws[row + 1, 5].CellStyle.NumberFormat = "#,##0.00";
                    row++;
                    }
                if (item.Tipo == "T")
                    {
                    ws[row + 1, 1].Text = item.Descripcion;
                    ws[row + 1, 2].Text = item.Unidad;
                    ws[row + 1, 3].Number = (double)item.Cantidad;
                    if (PE || PC)
                        ws[row + 1, 4].Number = (double)item.PU1;
                    ws[row + 1, 3].CellStyle.NumberFormat = "#,##0.00";
                    ws[row + 1, 4].CellStyle.NumberFormat = "#,##0.00";
                    ws[row + 1, 5].CellStyle.NumberFormat = "#,##0.00";
                    ws[row + 1, 5].Formula = $"=C{row + 1}*D{row + 1}";
                    row++;
                    }
                if (item.HasItems)
                    Aplanar(item.Inferiores, ws);
                }
            }

        private void AplanarPorTipo(ObservableCollection<Nodo> items, IWorksheet ws)
            {
            foreach (var item in items)
                {
                if (item.Tipo == "R")
                    {
                    ws[row + 1, 1].Text = item.Descripcion;
                    string[] formulas = new string[7];
                    bool primera = true;
                    foreach (var inferiores in item.Inferiores)
                        {
                        int filaSuma = inferiores.OrdenInt + filainicio;
                        for (int i = 0; i < 7; i++)
                            {
                            string col = ((char)('B' + i)).ToString();
                            if (primera)
                                formulas[i] = $"={col}{filaSuma}";
                            else
                                formulas[i] += $"+{col}{filaSuma}";
                            }
                        primera = false;
                        }
                    for (int i = 0; i < 7; i++)
                        {
                        ws[row + 1, i + 2].Formula = formulas[i];
                        ws[row + 1, i + 2].CellStyle.Font.Bold = true;
                        ws[row + 1, i + 2].CellStyle.NumberFormat = "#,##0.00";
                        }
                    ws[row + 1, 1].CellStyle.Font.Bold = true;
                    row++;
                    }
                if (item.Tipo == "T")
                    {
                    ws[row + 1, 1].Text = item.Descripcion;
                    ws[row + 1, 2].Number = (double)item.Materiales1;
                    ws[row + 1, 3].Number = (double)item.ManodeObra1;
                    ws[row + 1, 4].Number = (double)item.Equipos1;
                    ws[row + 1, 5].Number = (double)item.Subcontratos1;
                    ws[row + 1, 6].Number = (double)item.Otros1;
                    // ws[row + 1, 7].Number = (double)item.Indi1; // Si tienes el campo
                    for (int i = 2; i <= 7; i++)
                        ws[row + 1, i].CellStyle.NumberFormat = "#,##0.00";
                    ws[row + 1, 8].Formula = $"=SUM(B{row + 1}:G{row + 1})";
                    ws[row + 1, 8].CellStyle.NumberFormat = "#,##0.00";
                    row++;
                    }
                if (item.HasItems)
                    AplanarPorTipo(item.Inferiores, ws);
                }
            }

        private void Totales(int ubicaTotal, IWorksheet ws)
            {
            string formula = "";
            bool primera = true;
            foreach (var inferiores in oActivo.Arbol)
                {
                int filaSuma = inferiores.OrdenInt + filainicio;
                if (primera)
                    formula = $"=E{filaSuma}";
                else
                    formula += $"+E{filaSuma}";
                primera = false;
                }
            ws[ubicaTotal + 1, 3].Text = "Total:";
            ws[ubicaTotal + 1, 3].CellStyle.Font.Bold = true;
            ws[ubicaTotal + 1, 5].Formula = formula;
            ws[ubicaTotal + 1, 5].CellStyle.Font.Bold = true;
            ws[ubicaTotal + 1, 5].CellStyle.NumberFormat = "#,##0.00";
            }

        private void TotalesPorTipo(int ubicaTotal, IWorksheet ws)
            {
            string[] formulas = new string[7];
            bool primera = true;
            foreach (var inferiores in oActivo.Arbol)
                {
                int filaSuma = inferiores.OrdenInt + filainicio;
                for (int i = 0; i < 7; i++)
                    {
                    string col = ((char)('B' + i)).ToString();
                    if (primera)
                        formulas[i] = $"={col}{filaSuma}";
                    else
                        formulas[i] += $"+{col}{filaSuma}";
                    }
                primera = false;
                }
            ws[ubicaTotal + 1, 1].Text = "Totales:";
            ws[ubicaTotal + 1, 1].CellStyle.Font.Bold = true;
            for (int i = 0; i < 7; i++)
                {
                ws[ubicaTotal + 1, i + 2].Formula = formulas[i];
                ws[ubicaTotal + 1, i + 2].CellStyle.Font.Bold = true;
                ws[ubicaTotal + 1, i + 2].CellStyle.NumberFormat = "#,##0.00";
                }
            }

        public void Dispose()
            {
            // book?.Dispose(); // <-- Elimina esta línea
            excelEngine?.Dispose(); // Esto es suficiente
            }
        }
    }