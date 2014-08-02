using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
    /// instead of being created directly by Moq.
    /// </summary>
    public class MockMethodInitializer : IMockInitializer
    {
        private readonly IFixture _fixture;

        /// <summary>
        /// Creates an instance of <see cref="MockMethodInitializer"/>.
        /// </summary>
        /// <param name="fixture">The fixture that will be used to retrieve the return values for an object's methods.</param>
        public MockMethodInitializer(IFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
        /// instead of being created directly by Moq.
        /// </summary>
        /// <param name="mock">The mock to setup.</param>
        public void Setup(Mock mock)
        {
            var mockedType = mock.GetMockedType();
            var methods = mockedType.GetMethods()
                                    .Where(method => !method.IsGenericMethod &&
                                                     method.ReturnType != typeof (void));

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method);

                //call `Setup`
                var setup = mock.Setup(returnType, methodInvocationLambda);

                //call `Returns`
                setup.ReturnsUsingFixture(_fixture, mockedType, returnType);
            }
        }

        /// <summary>
        /// Returns a lambda expression thats represents an invocation of a mocked type's method.
        /// E.g., <![CDATA[ x => x.Method(It.IsAny<string>(), It.IsAny<int>()) ]]> 
        /// </summary>
        private Expression MakeMethodInvocationLambda(Type mockedType, MethodInfo method)
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");

            var methodCallParams = method.GetParameters()
                                         .Select(param => typeof (It).GetMethod("IsAny")
                                                                     .MakeGenericMethod(param.ParameterType))
                                         .Select(isAnyMethod => Expression.Call(isAnyMethod))
                                         .ToList();
            var methodCall = Expression.Call(lambdaParam, method, methodCallParams);

            return Expression.Lambda(methodCall, lambdaParam);
        }
    }
}
