using System;
using System.Reflection;

public class ReflectionUtil<T>
{
    private readonly T targetObject;

    public ReflectionUtil(T targetObject)
    {
        this.targetObject = targetObject;
    }

    public void InvokeMethod(string methodName, params object[] args)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        MethodInfo method = targetType.GetMethod(methodName, bindingFlags);

        if (method == null)
        {
            throw new ArgumentException($"Method '{methodName}' not found in type '{targetType}'.");
        }

        method.Invoke(targetObject, args);
    }

    public object GetFieldValue(string fieldName)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

        FieldInfo field = targetType.GetField(fieldName, bindingFlags);

        if (field == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{targetType}'.");
        }

        return field.GetValue(targetObject);
    }

    public void SetFieldValue(string fieldName, object value)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField;

        FieldInfo field = targetType.GetField(fieldName, bindingFlags);

        if (field == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{targetType}'.");
        }

        field.SetValue(targetObject, value);
    }

    public object GetPropertyValue(string propertyName)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty;

        PropertyInfo property = targetType.GetProperty(propertyName, bindingFlags);

        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in type '{targetType}'.");
        }

        return property.GetValue(targetObject);
    }

    public void SetPropertyValue(string propertyName, object value)
    {
        Type targetType = typeof(T);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty;

        PropertyInfo property = targetType.GetProperty(propertyName, bindingFlags);

        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in type '{targetType}'.");
        }

        property.SetValue(targetObject, value);
    }
}
