using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework.Interfaces;

namespace SkbKontur.NUnit.Retries
{
    public sealed class CustomAttributeMethodWrapper : IMethodInfo
    {
        public CustomAttributeMethodWrapper(IMethodInfo baseInfo, params Attribute[] extraAttributes)
        {
            this.baseInfo = baseInfo;
            this.extraAttributes = extraAttributes;
        }

        public ITypeInfo TypeInfo => baseInfo.TypeInfo;
        public MethodInfo MethodInfo => baseInfo.MethodInfo;
        public string Name => baseInfo.Name;
        public bool IsAbstract => baseInfo.IsAbstract;
        public bool IsPublic => baseInfo.IsPublic;
        public bool IsStatic => baseInfo.IsStatic;
        public bool ContainsGenericParameters => baseInfo.ContainsGenericParameters;
        public bool IsGenericMethod => baseInfo.IsGenericMethod;
        public bool IsGenericMethodDefinition => baseInfo.IsGenericMethodDefinition;
        public ITypeInfo ReturnType => baseInfo.ReturnType;

        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            var bases = baseInfo.GetCustomAttributes<T>(inherit);
            var extras = extraAttributes.OfType<T>().ToArray();

            return !extras.Any()
                       ? bases
                       : !bases.Any()
                           ? extras
                           : MergeAttributes(bases, extras);
        }

        public bool IsDefined<T>(bool inherit) where T : class
            => baseInfo.IsDefined<T>(inherit) || extraAttributes.OfType<T>().Any();

        public Type[] GetGenericArguments() => baseInfo.GetGenericArguments();
        public IParameterInfo[] GetParameters() => baseInfo.GetParameters();
        public object? Invoke(object? fixture, params object?[]? args) => baseInfo.Invoke(fixture, args);
        public IMethodInfo MakeGenericMethod(params Type[] typeArguments) => baseInfo.MakeGenericMethod(typeArguments);

        private static T[] MergeAttributes<T>(T[] bases, T[] extras) where T : class
        {
            var baseTypes = new HashSet<Type>(bases.Select(x => x.GetType()));
            return bases
                   .Concat(extras.Where(e => !baseTypes.Contains(e.GetType())))
                   .ToArray();
        }

        private readonly IMethodInfo baseInfo;
        private readonly Attribute[] extraAttributes;
    }
}