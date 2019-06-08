using System.Collections.Generic;
using System.Reflection;

namespace SomeOneSpyingOnYou.Models
{
    public class GenericClassConverter
    {
        public string TableName { get; set; }
        public Dictionary<string, string> PropNamesAndTypes { get; set; }


        public void ConvertClass<T>()
        {
            PropNamesAndTypes = new Dictionary<string, string>();

            TableName = typeof(T).Name;

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                if(!PropNamesAndTypes.ContainsKey(pi.Name))
                    PropNamesAndTypes.Add(pi.Name, pi.PropertyType.Name);
            }
        }
    }
}
