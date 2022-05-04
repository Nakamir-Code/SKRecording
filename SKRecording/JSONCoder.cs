using Newtonsoft.Json;
using StereoKit;

namespace SKRecording
{
    // Wrapper class for Newtonsoft.Json for converting to and from JSON
    public class JsonCoder
    {

        public JsonCoder()
        {
        }

        // Deserialize into desired type
        public T Deserialize<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        // Serialize from desired type
        public string Serialize<T>(T param)
        {
            return JsonConvert.SerializeObject(param);
        }

    }
}