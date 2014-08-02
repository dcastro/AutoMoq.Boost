using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Uses a set of mock initializers to setup a <see cref="Mock{T}"/>.
    /// </summary>
    public class MockSetup : ISpecimenBuilder
    {
        private readonly ISpecimenBuilder _builder;
        private readonly IEnumerable<IMockInitializer> _initializers;

        /// <summary>
        /// Creates a new instance of <see cref="MockSetup"/>.
        /// </summary>
        /// <param name="builder">The builder used to create <see cref="Mock{T}"/> instances.</param>
        /// <param name="initializers">The initializers used to setup the mock object.</param>
        public MockSetup(ISpecimenBuilder builder, params IMockInitializer[] initializers)
            : this(builder, initializers.AsEnumerable())
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="MockSetup"/>.
        /// </summary>
        /// <param name="builder">The builder used to create <see cref="Mock{T}"/> instances.</param>
        /// <param name="initializers">The initializers used to setup the mock object.</param>
        public MockSetup(ISpecimenBuilder builder, IEnumerable<IMockInitializer> initializers)
        {
            _builder = builder;
            _initializers = initializers;
        }

        /// <summary>
        /// Runs a set of initializers to setup a <see cref="Mock{T}"/> instance.
        /// </summary>
        /// <param name="request">The request that describes what to create.</param>
        /// <param name="context">A context that can be used to create other specimens.</param>
        /// <returns>
        /// The specimen created by created by the underlying builder.
        /// If the specimen is a <see cref="Mock{T}"/>, this instance will setup the mock before returning it.
        /// </returns>
        public object Create(object request, ISpecimenContext context)
        {
            var specimen = _builder.Create(request, context);
            if (specimen is NoSpecimen)
                return specimen;

            var mock = specimen as Mock;

            foreach (var mockInitializer in _initializers)
                mockInitializer.Setup(mock);

            return mock;
        }
    }
}
