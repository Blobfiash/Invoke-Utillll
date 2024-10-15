using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Util
{
    //Idea from MainCool
    public class INVOKEUTILL<R>
    {
        private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private const BindingFlags PrivateField = PrivateInstance | BindingFlags.GetField;
        private const BindingFlags PrivateProperty = PrivateInstance | BindingFlags.GetProperty;
        private const BindingFlags StaticField = PrivateStatic | BindingFlags.GetField;
        private const BindingFlags StaticProperty = PrivateStatic | BindingFlags.GetProperty;
        private const BindingFlags PrivateMethod = PrivateInstance | BindingFlags.InvokeMethod;
        private const BindingFlags StaticMethod = PrivateStatic | BindingFlags.InvokeMethod;
        private const BindingFlags InternalMethod = BindingFlags.NonPublic | BindingFlags.InvokeMethod;

        private R Object { get; }
        private Type Type { get; }

        internal INVOKEUTILL(R obj)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
            Type = typeof(R);
        }

        private T GetValue<T>(string name, BindingFlags flags)
        {
            try
            {
                var field = Type.GetField(name, flags);
                return field != null ? (T)field.GetValue(Object) : default;
            }
            catch { return default; }
        }

        private T GetProperty<T>(string name, BindingFlags flags)
        {
            try
            {
                var property = Type.GetProperty(name, flags);
                return property != null ? (T)property.GetValue(Object) : default;
            }
            catch { return default; }
        }

        private INVOKEUTILL<R> SetValue(string name, object value, BindingFlags flags)
        {
            try
            {
                var field = Type.GetField(name, flags);
                if (field != null) field.SetValue(Object, value);
            }
            catch { }
            return this;
        }

        private INVOKEUTILL<R> SetProperty(string name, object value, BindingFlags flags)
        {
            try
            {
                var property = Type.GetProperty(name, flags);
                if (property != null) property.SetValue(Object, value);
            }
            catch { }
            return this;
        }

        private T InvokeMethod<T>(string name, BindingFlags flags, params object[] args)
        {
            try
            {
                var method = Type.GetMethods(flags).FirstOrDefault(m =>
                    m.Name == name && m.GetParameters().Length == args.Length &&
                    m.GetParameters().Select(p => p.ParameterType).SequenceEqual(args.Select(a => a?.GetType() ?? typeof(object))));
                return method != null ? (T)method.Invoke(Object, args) : default;
            }
            catch { return default; }
        }

        public T GetValue<T>(string name, bool isStatic = false, bool isProperty = false)
        {
            var flags = isProperty ? (isStatic ? StaticProperty : PrivateProperty) : (isStatic ? StaticField : PrivateField);
            return isProperty ? GetProperty<T>(name, flags) : GetValue<T>(name, flags);
        }

        public INVOKEUTILL<R> SetValue(string name, object value, bool isStatic = false, bool isProperty = false)
        {
            var flags = isProperty ? (isStatic ? StaticProperty : PrivateProperty) : (isStatic ? StaticField : PrivateField);
            return isProperty ? SetProperty(name, value, flags) : SetValue(name, value, flags);
        }

        public T InvokeInternalMethod<T>(string name, params object[] args) => InvokeMethod<T>(name, InternalMethod, args);
        public T Invoke<T>(string name, params object[] args) => InvokeMethod<T>(name, PrivateMethod, args);

        public object GetValue(string name, bool isStatic = false, bool isProperty = false) => GetValue<R>(name, isStatic, isProperty);

        public INVOKEUTILL<R> Invoke(string name, params object[] args) => Invoke<R>(name, args)?.Ref();

        public bool HasField(string name) => Type.GetField(name, PrivateField) != null || Type.GetField(name, StaticField) != null;
        public bool HasProperty(string name) => Type.GetProperty(name, PrivateProperty) != null || Type.GetProperty(name, StaticProperty) != null;
        public bool HasMethod(string name) => Type.GetMethods(PrivateMethod).Any(m => m.Name == name) || Type.GetMethods(StaticMethod).Any(m => m.Name == name);

        public Type GetFieldType(string name) => Type.GetField(name, PrivateField)?.FieldType ?? Type.GetField(name, StaticField)?.FieldType;

        public Type GetPropertyType(string name) => Type.GetProperty(name, PrivateProperty)?.PropertyType ?? Type.GetProperty(name, StaticProperty)?.PropertyType;

        public object[] GetMethodParameters(string name) => Type.GetMethods(PrivateMethod)
            .Where(m => m.Name == name)
            .Select(m => m.GetParameters().Select(p => p.ParameterType).ToArray())
            .FirstOrDefault();
    }

    public static class ReflectorExtensions
    {
        public static INVOKEUTILL<R> Ref<R>(this R obj) => new INVOKEUTILL<R>(obj);
    }
}
