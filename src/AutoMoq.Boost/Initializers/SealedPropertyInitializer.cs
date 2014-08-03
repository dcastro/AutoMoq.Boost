using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dash.AutoMoq.Boost.Extensions;
using Moq;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// If the type of the object being mocked contains any non-virtual/sealed settable properties,
    /// this initializer will resolve them from a given context.
    /// </summary>
    public class SealedPropertyInitializer : IMockInitializer
    {
        /// <summary>
        /// If the type of the object being mocked contains any non-virtual/sealed settable properties,
        /// this initializer will resolve them from a given context.
        /// </summary>
        /// <param name="mock">The mock object.</param>
        /// <param name="context">The context.</param>
        public void Setup(Mock mock, ISpecimenContext context)
        {
            var mockedType = mock.GetMockedType();
            var mockedObject = mock.Object;

            var properties = mockedType.GetProperties()
                                       .Where(p => p.CanWrite &&
                                                   p.GetSetMethod().IsSealed());

            foreach (var property in properties)
            {
                var value = context.Resolve(property.PropertyType);
                property.SetValue(mockedObject, value, null);
            }
        }
    }
}
