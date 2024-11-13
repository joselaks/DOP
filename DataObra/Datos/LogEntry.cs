using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace DataObra.Datos
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }

        public string LogEntryDisplay => $"{Timestamp:G} - {Status} - {Method} {Url} - Success: {Success} - {ErrorMessage}";
    }
}
