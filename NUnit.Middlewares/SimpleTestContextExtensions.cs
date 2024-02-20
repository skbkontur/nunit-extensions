using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Middlewares
{
    public static class SimpleTestContextExtensions
    {
        public static T Get<T>(this IPropertyBag properties)
        {
            return properties.Get<T>(typeof(T).Name);
        }

        public static T Get<T>(this IPropertyBag properties, string key)
        {
            return (T?)properties.Get(key)
                   ?? throw new KeyNotFoundException($"Cannot find item by key {key} in test properties");
        }

        public static T GetFromThisOrParentContext<T>(this ITest test)
        {
            return test.GetFromThisOrParentContext<T>(typeof(T).Name);
        }

        public static T GetFromThisOrParentContext<T>(this ITest test, string key)
        {
            return (T)test.GetRecursiveOrThrow(key);
        }

        public static T Get<T>(this SimpleTestContext context)
        {
            return context.Get<T>(typeof(T).Name);
        }

        public static T Get<T>(this SimpleTestContext context, string key)
        {
            return (T?)context.Get(key)
                   ?? throw new KeyNotFoundException($"Cannot find item by key {key} in test context");
        }

        public static void Set<T>(this IPropertyBag properties, T value)
            where T : notnull
        {
            properties.Set(typeof(T).Name, value);
        }

        public static object GetRecursiveOrThrow(this ITest test, string key)
        {
            return test.GetRecursive(key)
                   ?? throw new KeyNotFoundException($"Cannot find item by key {key} in test {test.Name} or its parents");
        }

        public static object? GetRecursive(this ITest test, string key) =>
            GetRecursive(test, key, (p, k) => p.Get(k));

        public static bool ContainsKeyRecursive(this ITest test, string key) =>
            GetRecursive(test, key, (p, k) => p.ContainsKey(k) ? true : (bool?)null) ?? false;

        public static IList? ListRecursive(this ITest test, string key) =>
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