using Newtonsoft.Json;
using StereoKit;

namespace SKRecording
{
    public class JsonCoder
    {

        public JsonCoder()
        {
        }
        public T Deserialize<T>(string jsonStr)
        {
            // Deserialize into desired class object
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public string Serialize<T>(T param)
        {
            return JsonConvert.SerializeObject(param);
        }

    }
}