namespace Http.Helper
{
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Unicode;
    using Newtonsoft.Json;

    public class HttpMasterSerializer
    {
        private JsonSerializerSettings newtonsoftSerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        private JsonSerializerOptions systemTextSerializerSettings = new()
        {
            IgnoreReadOnlyFields = false,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        };

        private SerializersList selectedSerializer = SerializersList.NewtonsoftJson;

        public enum SerializersList
        {
            /// <summary>
            /// Newtonsoft serializer.
            /// </summary>
            NewtonsoftJson = 1,

            /// <summary>
            /// SystemText serializer.
            /// </summary>
            SystemTextJson = 2,
        }

        public HttpMasterSerializer SetSerializer(SerializersList serializer)
        {
            selectedSerializer = serializer;
            return this;
        }

        public HttpMasterSerializer SetSerializerSettings(JsonSerializerSettings settings)
        {
            if (selectedSerializer == SerializersList.NewtonsoftJson && settings != null)
            {
                newtonsoftSerializerSettings = settings;
            }

            return this;
        }

        public HttpMasterSerializer SetSerializerSettings(JsonSerializerOptions settings)
        {
            if (selectedSerializer == SerializersList.SystemTextJson && settings != null)
            {
                systemTextSerializerSettings = settings;
            }

            return this;
        }

        public string Serialize(object data)
        {
            if (data != null && selectedSerializer == SerializersList.NewtonsoftJson)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(data, newtonsoftSerializerSettings);
            }

            if (data != null && selectedSerializer == SerializersList.SystemTextJson)
            {
                return System.Text.Json.JsonSerializer.Serialize(data, systemTextSerializerSettings);
            }

            return string.Empty;
        }

        public T? Deserialize<T>(string json)
        {
            if (!string.IsNullOrEmpty(json) && selectedSerializer == SerializersList.NewtonsoftJson)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, newtonsoftSerializerSettings);
            }

            if (!string.IsNullOrEmpty(json) && selectedSerializer == SerializersList.SystemTextJson)
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json, systemTextSerializerSettings);
            }

            return default;
        }
    }
}
