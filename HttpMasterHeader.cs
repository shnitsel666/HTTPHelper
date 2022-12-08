namespace Http.Helper
{
    public class HttpMasterHeader
    {
        public readonly string Type;

        public readonly object Value;

        public HttpMasterHeader(string type, object value)
        {
            Type = type;
            Value = value;
        }

        public static HttpMasterHeader Authorization(string authorizationHeader) =>
            new("Authorization", authorizationHeader);

        public static HttpMasterHeader BearerAuthorization(string authorizationHeader) =>
            new("Authorization", $"Bearer {authorizationHeader}");

        public static HttpMasterHeader ContentType(string contentType) =>
            new("Content-Type", contentType);
    }
}
