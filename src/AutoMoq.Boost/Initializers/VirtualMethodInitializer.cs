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
    /// 
    /// This will setup any virtual methods.
    /// This includes:
    ///  - interface's methods/property getters;
    ///  - class's abstract/virtual/overridden/non-sealed methods/property getters.
    /// </summary>
    public class VirtualMethodInitializer : IMockInitializer
    {
        /// <summary>
        /// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
        /// instead of being created directly by Moq.
        /// </summary>
        /// <param name="mock">The mock to setup.</param>
        /// <param name="context">The context of the mock.</param>
        public void Setup(Mock mock, ISpecimenContext context)
        {
            var mockedType = mock.GetMockedType();
            var methods = mockedType.GetMethods()
                                    .Where(method => !method.IsGenericMethod &&
                                                     method.IsVirtual && !method.IsFinal &&
                                                     method.ReturnType != typeof (void));

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method);

                //call `Setup`
                var setup = mock.Setup(returnType, methodInvocationLambda);

                //call `Returns`
                setup.ReturnsUsingContext(context, mockedType, returnType);
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
