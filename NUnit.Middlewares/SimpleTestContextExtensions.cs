using System;
using System.Collections;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    public static class SimpleTestContextExtensions
    {
        public static T? Get<T>(this IPropertyBag properties)
        {
            return (T)properties.Get(typeof(T).Name);
        }

        public static T Get<T>(this ITest test)
        {
            return (T?)test.Get(typeof(T).Name)!;
        }

        public static T Get<T>(this SimpleTestContext context)
        {
            return (T)context.Get(typeof(T).Name)!;
        }

        public static void Set<T>(this IPropertyBag properties, T value)
            where T : notnull
        {
            properties.Set(typeof(T).Name, value);
        }

        public static object? Get(this ITest test, string key) =>
            GetRecursive(test, key, (p, k) => p.Get(k));

        public static bool ContainsKey(this ITest test, string key) =>
            GetRecursive(test, key, (p, k) => p.ContainsKey(k) ? true : (bool?)null) ?? false;

        public static IList? List(this ITest test, string key) =>
            GetRecursive(test, key, (p, k) => p[k]);

        private static T? GetRecursive<T>(ITest leaf, string key, Func<IPropertyBag, string, T?> getValue)
        {
            var current = leaf;
            while (current != null)
            {
                var item = getValue(current.Properties, key);
                if (item != null)
                {
                    return item;
                }

                current = current.Parent;
            }

            return default;
        }
    }
}