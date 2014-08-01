using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Moq.Language.Flow;
using Ploeh.AutoFixture;

namespace Dash.AutoMoq.Boost.Extensions
{
    internal static class MockExtensions
    {
        public static Type GetMockedType(this Mock mock)
        {
            return mock.GetType().GetGenericArguments().Single();
        }

        /// <summary>
        /// Sets up a method with a given member access expression, and returns an instance of <see cref="ISetup{TMock}"/>
        /// </summary>
        /// <param name="mock">The mock being set up.</param>
        /// <param name="memberType">The type of the member being set up.</param>
        /// <param name="memberAccessExpression">The expression needed to setup the member.</param>
        /// <returns>The result of setting up <paramref name="mock"/> with <paramref name="memberAccessExpression"/>.</returns>
        public static object Setup(this Mock mock, Type memberType, Expression memberAccessExpression)
        {
            var setupMethod = mock.GetType()
                                  .GetMethods()
                                  .First(method => method.Name == "Setup" &&
                                                   method.ContainsGenericParameters &&
                                                   method.GetGenericArguments().Count() == 1)
                                  .MakeGenericMethod(memberType);

            return setupMethod.Invoke(mock, new object[] {memberAccessExpression});
        }

        /// <summary>
        /// Configures an instance of <see cref="ISetup{TMock,TResult}"/> to retrieve the return value from <paramref name="fixture"/>.
        /// </summary>
        /// <param name="setup">An instance of <see cref="ISetup{TMock,TResult}"/>.</param>
        /// <param name="fixture">The fixture that will be used to retrieve the return value for the mock's member being setup.</param>
        /// <param name="mockedType">The type of the object being mocked.</param>
        /// <param name="memberType">The type of the member of being setup.</param>
        /// <returns></returns>
        public static object ReturnsFromFixture(this object setup, IFixture fixture, Type mockedType, Type memberType)
        {
            var returns = typeof (MockExtensions).GetMethod("ReturnsFromFixtureAux",
                                                            BindingFlags.Static | BindingFlags.NonPublic)
                                                 .MakeGenericMethod(new[] {mockedType, memberType});

            return returns.Invoke(null, new[] {setup, fixture});
        }

        private static IReturnsResult<TMock> ReturnsFromFixtureAux<TMock, TResult>(this ISetup<TMock, TResult> setup,
                                                                                   IFixture fixture)
            where TMock : class
        {
            return setup.Returns(fixture.Create<TResult>());
        }
    }
}
