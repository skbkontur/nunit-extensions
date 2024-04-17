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
            return properties.Get<T>(TypeKey<T>());
        }

        public static T? TryGet<T>(this IPropertyBag properties)
        {
            return properties.TryGet<T>(TypeKey<T>());
        }

        public static T Get<T>(this IPropertyBag properties, string key)
        {
            return properties.TryGet<T>(key)
                   ?? throw new KeyNotFoundException($"Cannot find item by key {key} in test properties");
        }

        public static T? TryGet<T>(this IPropertyBag properties, string key)
        {
            return (T?)properties.Get(key);
        }

        public static T GetFromThisOrParentContext<T>(this ITest test)
        {
            return test.GetFromThisOrParentContext<T>(TypeKey<T>());
        }

        public static T? TryGetFromThisOrParentContext<T>(this ITest test)
        {
            return test.TryGetFromThisOrParentContext<T>(TypeKey<T>());
        }

        public static T GetFromThisOrParentContext<T>(this ITest test, string key)
        {
            return (T)test.GetRecursive(key);
        }

        public static T? TryGetFromThisOrParentContext<T>(this ITest test, string key)
        {
            return (T?)test.TryGetRecursive(key);
        }

        public static T Get<T>(this SimpleTestContext context)
        {
            return context.Get<T>(TypeKey<T>());
        }

        public static T? TryGet<T>(this SimpleTestContext context)
        {
            return context.TryGet<T>(TypeKey<T>());
        }

        public static T Get<T>(this SimpleTestContext context, string key)
        {
            return context.TryGet<T>(key)
                   ?? throw new KeyNotFoundException($"Cannot find item by key {key} in test context");
        }

        public static T? TryGet<T>(this SimpleTestContext context, string key)
        {
            return (T?)context.TryGet(key);
        }

        public static void Set<T>(this IPropertyBag properties, T value)
            where T : notnull
        {
            properties.Set(TypeKey<T>(), value);
        }

        public static object GetRecursive(this ITest test, string key)
        {
            return test.TryGetRecursive(key)
                   ?? new KeyNotFoundException($"Cannot find item by key {key} in test {test.Name} or its parents");
        }

        public static object? TryGetRecursive(this ITest test, string key) =>
            GetRecursive(test, key, static (p, k) => p.Get(k));

        public static bool ContainsKeyRecursive(this ITest test, string key) =>
            GetRecursive(test, key, static (p, k) => p.ContainsKey(k) ? true : (bool?)null) ?? false;

        public static IList? ListRecursive(this ITest test, string key) =>
            GetRecursive(test, key, static (p, k) => p[k]);

        private static string TypeKey<T>()
        {
            return $"nunit-middlewares.{typeof(T).Name}";
        }

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