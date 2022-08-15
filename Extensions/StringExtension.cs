using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IgnoreCaseEquals(this string str, string second)
        {
            return string.Equals(str, second, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeJson<T>(this string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static bool TryDeserializeJson<T>(this string content, out T output)
        {
            output = default(T);
            content = content.Trim();
            if ((content.StartsWith("{") && content.EndsWith("}")) || (content.StartsWith("[") && content.EndsWith("]")))
            {
                try
                {
                    output = JsonConvert.DeserializeObject<T>(content);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public static T DeserializeXml<T>(this string content)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(new StringReader(content));
        }

        public static bool TryDeserializeXml<T>(this string content, out T output)
        {
            output = default(T);
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                output = (T)xmlSerializer.Deserialize(new StringReader(content));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
