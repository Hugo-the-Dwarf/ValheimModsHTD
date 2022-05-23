using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ValheimHTDArmory
{
    public static class CustomConfigParser
    {
        public static string SerializeObject<T>(T obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Object:{obj.GetType().Name}");
            FieldInfo[] objFields = obj.GetType().GetFields();
            foreach(var field in objFields)
            {
                sb.AppendLine($"\t{field.Name}: {field.GetValue(obj)}");
            }

            return sb.ToString();
        }
    }
}
