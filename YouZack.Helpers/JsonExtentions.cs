using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace System
{
    public static class JsonExtentions
    {
        private readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        public static string ToJsonString(this object value,bool camelCase=false)
        {
            if (value == null)
            {
                return null;
            }
            JsonSerializerOptions opt = new JsonSerializerOptions { Encoder = Encoder };
            if(camelCase)
            {
                opt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }            
            return JsonSerializer.Serialize(value, value.GetType(), opt);
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
