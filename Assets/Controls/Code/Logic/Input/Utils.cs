using Newtonsoft.Json;
using System.Linq;
using System.Net;

public static class Utils
{
    public static bool IsIp(string potentialIp)
    {
        return IPAddress.TryParse(potentialIp, out IPAddress ipAddress);
    }

    public static bool IsPort(string potentialPort)
    {
        return ushort.TryParse(potentialPort, out ushort port);
    }

    public static bool IsJsonObject(string potentialJson)
    {
        return potentialJson.Length >= 2
            && potentialJson.First() == '{'
            && potentialJson.Last() == '}';
    }

    public static T FromJson<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string ToJson(this object dto, bool beautify = false)
    {
        return JsonConvert.SerializeObject(dto, beautify ? Formatting.Indented : Formatting.None);
    }
}