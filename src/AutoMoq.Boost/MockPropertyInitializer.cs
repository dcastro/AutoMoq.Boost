using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Ploeh.AutoFixture;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Sets up a mocked object's properties so that the return values will be retrieved from a fixture,
    /// instead of being created directly by Moq.
    /// </summary>
    public class MockPropertyInitializer : IMockInitializer
    {
        private readonly IFixture _fixture;

        /// <summary>
        /// Creates an instance of <see cref="MockPropertyInitializer"/>.
        /// </summary>
        /// <param name="fixture">The fixture that will be used to retrieve a mocked object's properties.</param>
        public MockPropertyInitializer(IFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Sets up a mocked object's properties so that the return values will be retrieved from a fixture,
        /// instead of being created directly by Moq.
        /// </summary>
        /// <param name="mock">The mock to setup.</param>
        public void Setup(Mock mock)
        {
            var mockedType = mock.GetMockedType();
            var properties = mockedType.GetProperties().Where(property => property.CanRead);

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var propertyAccessLambda = MakePropertyAccessLambda(mockedType, property);

                //call `Setup`
                var setup = mock.Setup(propertyType, propertyAccessLambda);

                //call `Returns`
                setup.ReturnsUsingFixture(_fixture, mockedType, propertyType);
            }
        }

        /// <summary>
        /// Returns a lambda expression thats represents an access to a mocked type's property.
        /// E.g., <![CDATA[ x => x.Prop ]]>
        /// </summary>
        private Expression MakePropertyAccessLambda(Type mockedType, PropertyInfo property)
        {
            var param = Expression.Parameter(mockedType, "x");
            var propertyAccess = Expression.MakeMemberAccess(param, property);
            var propertyAccessLambda = Expression.Lambda(propertyAccess, param);

            return propertyAccessLambda;
        }
    }
}
