using System.ComponentModel;

namespace API.Helpers;
public class DefaultValueHelper
{
    public static void ApplyDefaults<T>(T obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var type = typeof(T);
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var currentValue = property.GetValue(obj);

            if (currentValue == null)
            {

                var defaultAttr = property.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                    .FirstOrDefault() as DefaultValueAttribute;

                if (defaultAttr != null)
                {
                    property.SetValue(obj, defaultAttr.Value);
                }
            }
        }
    }
}