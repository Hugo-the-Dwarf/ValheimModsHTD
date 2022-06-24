using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ValheimHTDArmory
{
    public static class ReflectionUtil
    {
        // super high performance reflection via IL emission
        // https://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo
        // there are ever so slightly faster ways of doing this that are way more complex
        public static Func<S, T> CreateGetterForField<S, T>(string fieldName)
        {
            var instExp = Expression.Parameter(typeof(S));
            var fieldExp = Expression.Field(instExp, fieldName);
            return Expression.Lambda<Func<S, T>>(fieldExp, instExp).Compile();
        }

        public static Action<S, T> CreateSetterForField<S, T>(string fieldName)
        {
            var targetExp = Expression.Parameter(typeof(S), "target");
            var valueExp = Expression.Parameter(typeof(T), "value");

            var fieldExp = Expression.Field(targetExp, fieldName);
            var assignExp = Expression.Assign(fieldExp, valueExp); //Expression.Convert(valueExp, fieldExp.Type));

            var setter = Expression.Lambda<Action<S, T>>(assignExp, targetExp, valueExp).Compile();
            return setter;
        }

        public static Func<S, T> MethodPointer<S, T>(string functionName)
        {
            var method = typeof(S).GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) throw new Exception($"Unable to locate method '{functionName}' on type: '{typeof(S)}'");

            return (Func<S, T>)method.CreateDelegate(typeof(Func<S, T>));
        }
    }
}