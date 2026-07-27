using System;

namespace PagoDirecto.Application.Extensions;

public static class ExtendReflection
{
    public static string GetPropertyTypeName(this object obj, string propertyName)
    {
        if (obj == null) return null;

        string tipoDato = null;
        var type = obj.GetType();

        if (type.GetProperty(propertyName) == null)
        {
            foreach (var propiedad in type.GetProperties())
            {
                var clase = type.GetProperty(propiedad.Name)?.GetValue(obj, null);
                if (clase != null && clase.GetType().GetProperty(propertyName) != null)
                {
                    tipoDato = clase.GetType().GetProperty(propertyName)?.GetValue(clase, null)?.GetType().Name;
                }
            }
        }
        else
        {
            var val = type.GetProperty(propertyName)?.GetValue(obj, null);
            if (val != null)
            {
                tipoDato = val.GetType().Name;
            }
        }

        return tipoDato;
    }

    public static object? GetPropertyValue(this object obj, string propertyName)
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var prop = type.GetProperty(propertyName);

        if (prop != null)
        {
            return prop.GetValue(obj, null);
        }

        foreach (var propiedad in type.GetProperties())
        {
            var clase = propiedad.GetValue(obj, null);
            if (clase != null)
            {
                var childProp = clase.GetType().GetProperty(propertyName);
                if (childProp != null)
                {
                    return childProp.GetValue(clase, null);
                }
            }
        }

        return null;
    }
}
