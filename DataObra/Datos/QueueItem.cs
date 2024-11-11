using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Datos
{
    public class QueueItem
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public object Data { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public TaskCompletionSource<HttpResponseMessage> ResponseTaskCompletionSource { get; } = new TaskCompletionSource<HttpResponseMessage>();

    }
}
