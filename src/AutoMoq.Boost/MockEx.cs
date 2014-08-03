using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Mock extensions.
    /// </summary>
    public static class MockEx
    {
        /// <summary>
        /// Sets up a member to retrieve the return value from a fixture.
        /// </summary>
        /// <typeparam name="TMock">The type of the object being mocked.</typeparam>
        /// <typeparam name="TResult">The return type of the object's member being mocked.</typeparam>
        /// <param name="setup">The member setup.</param>
        /// <param name="fixture">The fixture from which the return value will be retrieved.</param>
        /// <returns></returns>
        public static IReturnsResult<TMock> ReturnsUsingFixture<TMock, TResult>(this IReturns<TMock, TResult> setup,
                                                                                IFixture fixture)
            where TMock : class
        {
            return setup.ReturnsUsingContext(new SpecimenContext(fixture));
        }

        internal static Type GetMockedType(this Mock mock)
        {
            return mock.GetType().GetGenericArguments().Single();
        }

        /// <summary>
        /// Sets up a method with a given member access expression, and returns an instance of <see cref="ISetup{TMock}"/>
        /// </summary>
        /// <param name="mock">The mock being set up.</param>
        /// <param name="memberType">The return type of the member being set up.</param>
        /// <param name="memberAccessExpression">The expression needed to setup the member.</param>
        /// <returns>The result of setting up <paramref name="mock"/> with <paramref name="memberAccessExpression"/>.</returns>
        internal static object Setup(this Mock mock, Type memberType, Expression memberAccessExpression)
        {
            var setupMethod = mock.GetType()
                                  .GetMethods()
                                  .First(method => method.Name == "Setup" &&
                                                   method.IsGenericMethod &&
                                                   method.GetGenericArguments().Count() == 1)
                                  .MakeGenericMethod(memberType);

            return setupMethod.Invoke(mock, new object[] {memberAccessExpression});
        }

        /// <summary>
        /// Configures an instance of <see cref="ISetup{TMock,TResult}"/> to retrieve the return value from <paramref name="context"/>.
        /// </summary>
        /// <param name="setup">An instance of <see cref="ISetup{TMock,TResult}"/>.</param>
        /// <param name="context">The context (fixture) that will be used to retrieve the return value for the mock's member being setup.</param>
        /// <param name="mockedType">The type of the object being mocked.</param>
        /// <param name="memberType">The return type of the member of being setup.</param>
        /// <returns></returns>
        internal static object ReturnsUsingContext(this object setup, ISpecimenContext context, Type mockedType, Type memberType)
        {
            var returns = typeof (MockEx).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                                         .Where(method => method.Name == "ReturnsUsingContext")
                                         .Single(method => method.IsGenericMethod)
                                         .MakeGenericMethod(new[] {mockedType, memberType});

            return returns.Invoke(null, new[] {setup, context});
        }

        internal static IReturnsResult<TMock> ReturnsUsingContext<TMock, TResult>(this IReturns<TMock, TResult> setup,
                                                                                ISpecimenContext context)
            where TMock : class
        {
            return setup.Returns(() =>
                {
                    var result = (TResult) context.Resolve(typeof (TResult));
                    setup.Returns(result);
                    return result;
                });
        }
    }
}
