using FastMember;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace SqlDataReaderExtensions
{
    public static class Extensions
    {
        public static List<T> ReadListReflectionAsync<T>(this IDataReader dr) where T : class, new()
        {
            var list = new List<T>();

            if (dr.Read())
            {
                string[] columnNames = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i)).ToArray();


                do
                {


                    var resultItem = new T();
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        var value = dr[i];
                        if (value is DBNull) 
                            value = null;

                        SetProperty(resultItem, columnNames[i], value);
                    }
                 
                    list.Add(resultItem);
                }
                while (dr.Read());
            }
         
            dr.Close();
        
            return list;
        }

        private static void SetProperty<T>(this T target, string name, object value)
        {
            PropertyInfo? propInfo = typeof(T).GetProperty(name, bindingAttr: BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propInfo == null)
                return;

           propInfo.SetValue(target, value, null);
        }

        public static T MapEntity<T>(IDictionary<string, object> dbOutput) where T : new()
        {
            T result = new T();

            foreach (var pair in dbOutput)
            {
                var propertyName = pair.Key;

                PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName, bindingAttr: BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(result, pair.Value, null);

                }
            }
            return result;
        }
    }
}
