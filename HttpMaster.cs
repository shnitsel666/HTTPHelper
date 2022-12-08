namespace Http.Helper
{
    using System.Text;

    public class HttpMaster
    {
        private List<HttpMasterHeader> _headers;

        private HttpMasterSerializer _serializer = new HttpMasterSerializer().SetSerializer(HttpMasterSerializer.SerializersList.NewtonsoftJson);

        public HttpMasterHeader[] Headers => _headers.ToArray();

        public bool LoggingOn { get; set; }

        public TimeSpan TimeOut { get; private set; }

        public HttpMaster()
        {
            _headers = new List<HttpMasterHeader>();
            LoggingOn = false;
            TimeOut = TimeSpan.FromSeconds(120);
        }

        public HttpMaster DisableLogging()
        {
            LoggingOn = false;
            return this;
        }

        public HttpMaster SetTimeOut(int timeOutInSeconds)
        {
            TimeOut = TimeSpan.FromSeconds(timeOutInSeconds);
            return this;
        }

        public HttpMaster AddHeaders(params HttpMasterHeader[] headers)
        {
            _headers.AddRange(headers);
            return this;
        }

        public HttpMaster AddSerializer(HttpMasterSerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public HttpMaster RemoveHeaders(params string[] headers)
        {
            _headers = _headers.Where(_h => !headers.Any(h => h == _h.Type)).ToList();
            return this;
        }

        public HttpMaster ClearHeaders()
        {
            _headers = new List<HttpMasterHeader>();
            return this;
        }

        public HttpMasterResponse GET(string url) =>
            Request(url, "GET", null);

        public HttpMasterResponse POST(string url, object? body) =>
            Request(url, "POST", body);

        public HttpMasterResponse POST(string url) =>
            POST(url, null);

        public HttpMasterResponse PUT(string url, object? body) =>
            Request(url, "PUT", body);

        public HttpMasterResponse PUT(string url) =>
            PUT(url, null);

        public HttpMasterResponse DELETE(string url) =>
            Request(url, "DELETE", null);

        public async Task<HttpMasterResponse> GETAsync(string url) =>
            await Task.Run(() => Request(url, "GET", null));

        public async Task<HttpMasterResponse> POSTAsync(string url, object? body) =>
            await Task.Run(() => Request(url, "POST", body));

        public async Task<HttpMasterResponse> POSTAsync(string url) =>
            await Task.Run(() => POST(url, null));

        public async Task<HttpMasterResponse> PUTAsync(string url, object? body) =>
            await Task.Run(() => Request(url, "PUT", body));

        public async Task<HttpMasterResponse> PUTAsync(string url) =>
            await Task.Run(() => PUT(url, null));

        public async Task<HttpMasterResponse> DELETEAsync(string url) =>
            await Task.Run(() => Request(url, "DELETE", null));

        public static HttpMaster operator +(HttpMaster master, HttpMasterHeader header) =>
            master.AddHeaders(header);

        public static HttpMaster operator -(HttpMaster master, string header) =>
            master.RemoveHeaders(header);

        private HttpMasterResponse Request(string url, string method, object? body)
        {
            DateTime startRequest = DateTime.Now;
            Guid requestId = Guid.NewGuid();
            try
            {
                HttpMasterResponse response = new();
                response._serializer = _serializer;
                string json = string.Empty;
                if (body != null)
                {
                    json = _serializer.Serialize(body);
                }

                if (LoggingOn)
                {
                    string log = $"START REQUEST {requestId} | START DATE = {startRequest} | TO {url} | METHOD = {method} | REQUEST BODY = " + (string.IsNullOrEmpty(json) ? "EMPTY" : json);
                    Console.WriteLine(log);
                }

                StringContent content = new(json, Encoding.UTF8, "application/json");
                using (HttpClient client = new())
                {
                    client.Timeout = TimeOut;
                    _headers.ForEach(h => client.DefaultRequestHeaders.Add(h.Type, h.Value.ToString()));
                    switch (method.ToUpper())
                    {
                        case "GET": response._httpResponse = client.GetAsync(url).Result; break;
                        case "POST": response._httpResponse = client.PostAsync(url, content).Result; break;
                        case "PUT": response._httpResponse = client.PutAsync(url, content).Result; break;
                        case "DELETE": response._httpResponse = client.DeleteAsync(url).Result; break;
                    }

                    if (LoggingOn)
                    {
                        DateTime finishRequest = DateTime.Now;
                        string log = $"FINISH REQUEST {requestId} | FINISH DATE = {finishRequest} | TO {url} | METHOD = {method} | " +
                            $"TIME = {(finishRequest - startRequest).TotalMilliseconds}ms | HTTP CODE = {response.StatusCode} RESPONSE BODY = " +
                            (string.IsNullOrEmpty(response.ResponseString) ? "EMPTY" : response.ResponseString);
                        Console.WriteLine(log);
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                if (LoggingOn)
                {
                    DateTime finishRequest = DateTime.Now;
                    string log = $"FINISH REQUEST {requestId} | FINISH DATE = {finishRequest} | TO {url} | METHOD = {method} | EXCEPTION = {_serializer.Serialize(e)}";
                }

                throw;
            }
        }
    }
}