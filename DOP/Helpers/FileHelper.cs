using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.Pdf;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Csv;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Txt;

namespace DOP.Helpers
{
    public static class FileHelper
    {
        public const string XlsxFormat = "Xlsx";
        public const string CsvFormat = "Csv";
        public const string TxtFormat = "Txt";
        public const string PdfFormat = "Pdf";

        public static readonly string[] ExportFormats;
        private static readonly string SampleDataFolder = "SampleData/";

        static FileHelper()
        {
            ExportFormats = new string[] { XlsxFormat, CsvFormat, TxtFormat, PdfFormat };

            WorkbookFormatProvidersManager.RegisterFormatProvider(new PdfFormatProvider());
            WorkbookFormatProvidersManager.RegisterFormatProvider(new XlsxFormatProvider());
        }

        public static IWorkbookFormatProvider GetFormatProvider(string extension)
        {
            IWorkbookFormatProvider formatProvider;
            switch (extension)
            {
                case XlsxFormat:
                    formatProvider = new XlsxFormatProvider();
                    break;
                case CsvFormat:
                    formatProvider = new CsvFormatProvider();
                    ((CsvFormatProvider)formatProvider).Settings.HasHeaderRow = true;
                    break;
                case TxtFormat:
                    formatProvider = new TxtFormatProvider();
                    break;
                case PdfFormat:
                    formatProvider = new PdfFormatProvider();
                    break;
                default:
                    formatProvider = null;
                    break;
            }

            return formatProvider;
        }

        public static Stream GetSampleResourceStream(string resource)
        {
            var streamInfo = Application.GetResourceStream(GetResourceUri(SampleDataFolder + resource));
            if (streamInfo != null)
            {
                return streamInfo.Stream;
            }

            return null;
        }

        private static Uri GetResourceUri(string resource)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(FileHelper).Assembly.FullName);
            string resourcePath = "/" + assemblyName.Name + ";component/" + resource;
            Uri resourceUri = new Uri(resourcePath, UriKind.Relative);

            return resourceUri;
        }
    }
}
