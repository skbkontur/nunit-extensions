using System.Linq;

using Microsoft.CodeAnalysis;

namespace NUnit.Analyzers.Extensions
{
    internal static class MethodSymbolExtensions
    {
        internal static bool IsTestRelatedMethod(this IMethodSymbol methodSymbol, Compilation compilation)
        {
            return methodSymbol.HasTestRelatedAttributes(compilation) ||
                   (methodSymbol.OverriddenMethod is not null && methodSymbol.OverriddenMethod.IsTestRelatedMethod(compilation));
        }

        internal static bool HasTestRelatedAttributes(this IMethodSymbol methodSymbol, Compilation compilation)
        {
            return methodSymbol.GetAttributes().Any(
                a => a.IsTestMethodAttribute(compilation) || a.IsSetUpOrTearDownMethodAttribute(compilation));
        }
    }
}