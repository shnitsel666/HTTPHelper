namespace Http.Helper
{
    using System.Net;

    public class HttpMasterResponse
    {
        public HttpStatusCode StatusCode => _httpResponse.StatusCode;

        internal HttpMasterSerializer _serializer;

        internal HttpResponseMessage _httpResponse;

        private string _responseString;

        private object _responseObject;

        public string ResponseString
        {
            get
            {
                if (_responseString == null)
                {
                    _responseString = _httpResponse.Content.ReadAsStringAsync().Result;
                }

                return _responseString;
            }
        }

        public bool CanDeserialize<T>()
        {
            try
            {
                T result = _serializer.Deserialize<T>(ResponseString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public T Deserialize<T>()
        {
            if (_responseObject == null)
            {
                _responseObject = _serializer.Deserialize<T>(ResponseString);
            }

            return (T)_responseObject;
        }
    }
}
