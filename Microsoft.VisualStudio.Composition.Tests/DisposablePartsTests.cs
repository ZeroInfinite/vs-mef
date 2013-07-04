﻿namespace Microsoft.VisualStudio.Composition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class DisposablePartsTests
    {
        [Fact]
        public void DisposablePartDisposedWithContainer()
        {
            var configurationBuilder = new CompositionConfigurationBuilder();
            configurationBuilder.AddType(typeof(DisposablePart));
            var configuration = configurationBuilder.CreateConfiguration();
            var containerFactory = configuration.CreateContainerFactoryAsync().Result;
            var container = containerFactory.CreateContainer();

            var part = container.GetExport<DisposablePart>();
            Assert.False(part.IsDisposed);
            container.Dispose();
            Assert.True(part.IsDisposed);

            // Values not created should not be disposed.
            Assert.False(UninstantiatedPart.EverInstantiated);
            Assert.False(UninstantiatedPart.EverDisposed);
        }

        [Export]
        public class DisposablePart : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                this.IsDisposed = true;
            }
        }

        [Export]
        public class UninstantiatedPart : IDisposable
        {
            public UninstantiatedPart()
            {
                EverInstantiated = true;
            }
            public static bool EverDisposed { get; private set; }
            public static bool EverInstantiated { get; private set; }

            public void Dispose()
            {
                EverDisposed = true;
            }
        }
    }
}
