using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace System
{
    public static class JsonExtentions
    {
        private readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        public static string ToJsonString(this object value)
        {
            if (value == null)
            {
                return null;
            }
            return JsonSerializer.Serialize(value, value.GetType(),
                new JsonSerializerOptions { Encoder = Encoder });
        }

        public static T ParseJson<T>(this string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Encoder = Encoder });
        }
    }
}
