﻿namespace Microsoft.VisualStudio.Composition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using MEFv1 = System.ComponentModel.Composition;

    public class CompositionCatalogTests
    {
        [Fact]
        public async Task CreateFromTypesOmitsNonPartsV1()
        {
            var catalog = ComposableCatalog.Create(
                await new AttributedPartDiscoveryV1().CreatePartsAsync(typeof(NonExportingType), typeof(ExportingType)));
            Assert.Equal(1, catalog.Parts.Count);
            Assert.Equal(typeof(ExportingType), catalog.Parts.Single().Type);
        }

        [Fact]
        public async Task CreateFromTypesOmitsNonPartsV2()
        {
            var catalog = ComposableCatalog.Create(
                await new AttributedPartDiscovery().CreatePartsAsync(typeof(NonExportingType), typeof(ExportingType)));
            Assert.Equal(1, catalog.Parts.Count);
            Assert.Equal(typeof(ExportingType), catalog.Parts.Single().Type);
        }

        [Fact]
        public void WithPartNullThrows()
        {
            var catalog = ComposableCatalog.Create();
            Assert.Throws<ArgumentNullException>(() => catalog.WithPart(null));
        }

        [Fact]
        public void GetAssemblyInputs_Empty()
        {
            var catalog = ComposableCatalog.Create();
            Assert.Equal(0, catalog.GetInputAssemblies().Count);
        }

        [Fact]
        public async Task GetAssemblyInputs()
        {
            var catalog = ComposableCatalog.Create(
                await new AttributedPartDiscoveryV1().CreatePartsAsync(typeof(NonExportingType), typeof(ExportingType)));

            var expected = new AssemblyName[] { typeof(NonExportingType).Assembly.GetName() };
            var actual = catalog.GetInputAssemblies();
            Assert.Equal(expected, actual, AssemblyNameComparer.Default);
        }

        public class NonExportingType { }

        [Export, MEFv1.Export]
        public class ExportingType { }

        private class AssemblyNameComparer : IEqualityComparer<AssemblyName>
        {
            internal static readonly AssemblyNameComparer Default = new AssemblyNameComparer();

            internal AssemblyNameComparer() { }

            public bool Equals(AssemblyName x, AssemblyName y)
            {
                if (x == null ^ y == null)
                {
                    return false;
                }

                if (x == null)
                {
                    return true;
                }

                // fast path
                if (x.CodeBase == y.CodeBase)
                {
                    return true;
                }

                // Testing on FullName is horrifically slow.
                // So test directly on its components instead.
                return x.Name == y.Name
                    && x.Version.Equals(y.Version)
                    && x.CultureName.Equals(y.CultureName);
            }

            public int GetHashCode(AssemblyName obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
