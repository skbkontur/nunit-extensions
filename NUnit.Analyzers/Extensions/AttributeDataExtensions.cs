using System.Linq;

using Microsoft.CodeAnalysis;

using NUnit.Analyzers.Constants;

namespace NUnit.Analyzers.Extensions
{
    public static class AttributeDataExtensions
    {
        public static bool DerivesFromISimpleTestBuilder(this AttributeData @this, Compilation compilation)
        {
            return DerivesFromInterface(compilation, @this, NUnitFrameworkConstants.FullNameOfTypeISimpleTestBuilder);
        }

        public static bool DerivesFromITestBuilder(this AttributeData @this, Compilation compilation)
        {
            return DerivesFromInterface(compilation, @this, NUnitFrameworkConstants.FullNameOfTypeITestBuilder);
        }

        public static bool IsTestMethodAttribute(this AttributeData @this, Compilation compilation)
        {
            return @this.DerivesFromITestBuilder(compilation) ||
                   @this.DerivesFromISimpleTestBuilder(compilation);
        }

        public static bool IsSetUpOrTearDownMethodAttribute(this AttributeData @this, Compilation compilation)
        {
            var attributeType = @this.AttributeClass;

            if (attributeType is null)
                return false;

            return attributeType.IsType(NUnitFrameworkConstants.FullNameOfTypeOneTimeSetUpAttribute, compilation)
                   || attributeType.IsType(NUnitFrameworkConstants.FullNameOfTypeOneTimeTearDownAttribute, compilation)
                   || attributeType.IsType(NUnitFrameworkConstants.FullNameOfTypeSetUpAttribute, compilation)
                   || attributeType.IsType(NUnitFrameworkConstants.FullNameOfTypeTearDownAttribute, compilation);
        }

        private static bool DerivesFromInterface(Compilation compilation, AttributeData attributeData, string interfaceTypeFullName)
        {
            if (attributeData.AttributeClass is null)
                return false;

            var interfaceType = compilation.GetTypeByMetadataName(interfaceTypeFullName);

            if (interfaceType is null)
                return false;

            return attributeData.AttributeClass.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceType));
        }
    }
}