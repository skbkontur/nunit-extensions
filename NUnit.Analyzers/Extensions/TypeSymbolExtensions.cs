using Microsoft.CodeAnalysis;

namespace SkbKontur.NUnit.Analyzers.Extensions
{
    public static class TypeSymbolExtensions
    {
        internal static bool IsType(this ITypeSymbol? @this, string fullMetadataName, Compilation compilation)
        {
            var typeSymbol = compilation.GetTypeByMetadataName(fullMetadataName);

            return SymbolEqualityComparer.Default.Equals(typeSymbol, @this);
        }
    }
}