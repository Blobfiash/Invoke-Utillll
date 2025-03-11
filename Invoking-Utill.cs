using System;
using System.Linq;
using System.Reflection;

namespace Utils
{
    public class INVOKEUTILL<R>
    {
        private const BindingFlags PrivateFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags StaticFlags = BindingFlags.NonPublic | BindingFlags.Static;
        private readonly R Obj; 
        private Type Type => typeof(R);

        internal INVOKEUTILL(R obj) => Obj = obj;

        private T TryInvoke<T>(Func<T> func) { try { return func(); } catch { return default; } }

        public T GetValue<T>(string name, bool isStatic = false, bool isProperty = false) => TryInvoke<T>(() => (T)(Type.GetField(name, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.GetProperty : BindingFlags.GetField))?.GetValue(Obj) ?? Type.GetProperty(name, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.GetProperty : BindingFlags.GetField))?.GetValue(Obj)));

        public INVOKEUTILL<R> SetValue(string name, object value, bool isStatic = false, bool isProperty = false) =>
            TryInvoke<INVOKEUTILL<R>>(() =>
            {
                var field = Type.GetField(name, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.SetProperty : BindingFlags.SetField));
                var property = Type.GetProperty(name, (isStatic ? StaticFlags : PrivateFlags) | (isProperty ? BindingFlags.SetProperty : BindingFlags.SetField));

                field?.SetValue(Obj, value);
                property?.SetValue(Obj, value);

                return this;
            });

        public void Invoke(string name, params object[] args) => TryInvoke(() => Type.GetMethods(PrivateFlags | StaticFlags | BindingFlags.InvokeMethod).FirstOrDefault(m => m.Name == name && m.GetParameters().Length == args.Length && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(args.Select(a => a?.GetType() ?? typeof(object))))?.Invoke(Obj, args));

        public bool HasField(string name) => Type?.GetField(name, PrivateFlags | StaticFlags) != null;
        public bool HasProperty(string name) => Type?.GetProperty(name, PrivateFlags | StaticFlags) != null;
        public bool HasMethod(string name) => Type.GetMethods(PrivateFlags | StaticFlags).Any(m => m.Name == name);
        public Type GetFieldType(string name) => Type?.GetField(name, PrivateFlags | StaticFlags)?.FieldType;
        public Type GetPropertyType(string name) => Type?.GetProperty(name, PrivateFlags | StaticFlags)?.PropertyType;
        public object[] GetMethodParameters(string name) => Type?.GetMethods(PrivateFlags | StaticFlags).FirstOrDefault(m => m.Name == name)?.GetParameters().Select(p => p.ParameterType).ToArray();
    }

    public static class ReflectorExtensions
    {
        public static INVOKEUTILL<R> Ref<R>(this R obj) => new INVOKEUTILL<R>(obj);
    }
}
