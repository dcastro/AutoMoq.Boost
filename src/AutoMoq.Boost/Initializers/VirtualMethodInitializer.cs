﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dash.AutoMoq.Boost.Extensions;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
    /// instead of being created directly by Moq.
    /// 
    /// This will setup any virtual methods that are either non-void or have "out" parameters.
    /// This includes:
    ///  - interface's methods/property getters;
    ///  - class's abstract/virtual/overridden/non-sealed methods/property getters.
    /// 
    /// Notes:
    /// - Due to a limitation in Moq, methods with "ref" parameters are skipped.
    /// - Automatically mocking of generic methods isn't feasible either - we'd have to antecipate any type parameters that this method could be called with. 
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
                                    .Where(method => method.IsOverridable() &&
                                                     !method.IsGenericMethod &&
                                                     !method.HasRefParameters() &&
                                                     (!method.IsVoid() || method.HasOutParameters()));

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method, context);

                if (method.IsVoid())
                {
                    //call `Setup`
                    mock.Setup(methodInvocationLambda);
                }
                else
                {
                    //call `Setup`
                    var setup = mock.Setup(returnType, methodInvocationLambda);

                    //call `Returns`
                    setup.ReturnsUsingContext(context, mockedType, returnType);
                }
            }
        }

        /// <summary>
        /// Returns a lambda expression thats represents an invocation of a mocked type's method.
        /// E.g., <![CDATA[ x => x.Method(It.IsAny<string>(), It.IsAny<int>()) ]]> 
        /// </summary>
        private Expression MakeMethodInvocationLambda(Type mockedType, MethodInfo method, ISpecimenContext context)
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");

            var methodCallParams = method.GetParameters()
                                         .Select(param => MakeParameterExpression(param, context))
                                         .ToList();

            var methodCall = Expression.Call(lambdaParam, method, methodCallParams);

            return Expression.Lambda(methodCall, lambdaParam);
        }

        private Expression MakeParameterExpression(ParameterInfo parameter, ISpecimenContext context)
        {
            //check if parameter is an "out" parameter
            if (parameter.IsOut)
            {
                //gets the type corresponding to this "byref" type
                //e.g., the underlying type of "System.String&" is "System.String"
                var underlyingType = parameter.ParameterType.GetElementType();

                //resolve the "out" param from the context
                object variable = context.Resolve(underlyingType);

                return Expression.Constant(variable, underlyingType);
            }
            else
            {
                //for any non-out parameter, invoke "It.IsAny<T>()"
                var isAnyMethod = typeof (It).GetMethod("IsAny")
                                             .MakeGenericMethod(parameter.ParameterType);

                return Expression.Call(isAnyMethod);
            }
        }
    }
}
