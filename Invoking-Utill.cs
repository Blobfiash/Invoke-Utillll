using System;
using System.Linq;
using System.Reflection;

namespace Utils
{
    // Idea from MainCool
    public class INVOKEUTILL<R>
    {
        private const BindingFlags PrivateFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags StaticFlags = BindingFlags.NonPublic | BindingFlags.Static;
        private R Obj { get; }
        private Type Type => typeof(R);

        internal INVOKEUTILL(R obj) => Obj = obj;

        private T GetValue<T>(string name, BindingFlags flags) =>  Try<T>(() => (T)(Type.GetField(name, flags)?.GetValue(Obj) ?? Type.GetProperty(name, flags)?.GetValue(Obj)));

        private INVOKEUTILL<R> SetValue(string name, object value, BindingFlags flags)
        {
            Try<object>(() => { Type.GetField(name, flags)?.SetValue(Obj, value); return null; });
            Try<object>(() => { Type.GetProperty(name, flags)?.SetValue(Obj, value); return null; });
            return this;
        }

        private T InvokeMethod<T>(string name, BindingFlags flags, params object[] args) =>
            Try<T>(() =>
            {
                var method = Type.GetMethods(flags).FirstOrDefault(m =>
                    m.Name == name && m.GetParameters().Length == args.Length &&
                    m.GetParameters().Select(p => p.ParameterType).SequenceEqual(args.Select(a => a?.GetType() ?? typeof(object))));
                return method != null ? (T)method.Invoke(Obj, args) : default;
            });

        private static T Try<T>(Func<T> func)
        {
            try { return func(); }
            catch { return default; }
        }

        public T GetValue<T>(string name, bool isStatic = false, bool isProperty = false) => GetValue<T>(name, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.GetProperty : BindingFlags.GetField));

        public INVOKEUTILL<R> SetValue(string name, object value, bool isStatic = false, bool isProperty = false) => SetValue(name, value, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.SetProperty : BindingFlags.SetField));

        public T Invoke<T>(string name, params object[] args) => InvokeMethod<T>(name, PrivateFlags | BindingFlags.InvokeMethod, args);

        public T InvokeInternalMethod<T>(string name, params object[] args) => InvokeMethod<T>(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod, args);

        public bool HasField(string name) => Type.GetField(name, PrivateFlags | StaticFlags) != null;

        public bool HasProperty(string name) =>  Type.GetProperty(name, PrivateFlags | StaticFlags) != null;

        public bool HasMethod(string name) => Type.GetMethods(PrivateFlags | StaticFlags).Any(m => m.Name == name);

        public Type GetFieldType(string name) => Type.GetField(name, PrivateFlags | StaticFlags)?.FieldType;

        public Type GetPropertyType(string name) => Type.GetProperty(name, PrivateFlags | StaticFlags)?.PropertyType;

        public object[] GetMethodParameters(string name) => Type.GetMethods(PrivateFlags | StaticFlags).FirstOrDefault(m => m.Name == name)?.GetParameters().Select(p => p.ParameterType).ToArray();
    }

    public static class ReflectorExtensions
    {
        public static INVOKEUTILL<R> Ref<R>(this R obj) => new INVOKEUTILL<R>(obj);
    }
}
