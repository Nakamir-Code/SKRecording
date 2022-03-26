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
            // Get a single json string in case multiple are concatenated e.g. {}{}{}
            var endOfStr = jsonStr.IndexOf("}");
            jsonStr = jsonStr.Substring(0, endOfStr + 1);

            // Deserialize into desired class object
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public string Serialize<T>(T param)
        {
            return JsonConvert.SerializeObject(param);
        }

    }
}