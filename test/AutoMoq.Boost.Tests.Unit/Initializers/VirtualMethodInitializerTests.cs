﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.Extensions;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class VirtualMethodInitializerTests
    {
        [Theory, AutoData]
        public void SetsUp_InterfaceMethods_ToRetrieveReturnValueFromContext(Fixture fixture,
                                                                             [Frozen] string frozenString,
                                                                             VirtualMethodInitializer initializer,
                                                                             Mock<IInterfaceWithMethod> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.SomeMethod();

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void SetsUp_VirtualMethods_ToRetrieveReturnValueFromContext(Fixture fixture,
                                                                           [Frozen] string frozenString,
                                                                           VirtualMethodInitializer initializer,
                                                                           Mock<ClassWithVirtualMethod> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.VirtualMethod();

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void SetsUp_PropertyGetters_ToRetrieveReturnValueFromContext(Fixture fixture,
                                                                            [Frozen] string frozenString,
                                                                            VirtualMethodInitializer initializer,
                                                                            Mock<IInterfaceWithProperty> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.SomeProperty;

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void SetsUp_MethodsWithParameters(Fixture fixture,
                                                 [Frozen] string frozenString, VirtualMethodInitializer initializer,
                                                 Mock<IInterfaceWithParameter> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.Method(4);

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void SetsUp_MethodsWithOutParameters(Fixture fixture,
                                                    [Frozen] int frozenInt, VirtualMethodInitializer initializer,
                                                    Mock<IInterfaceWithOutParameter> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            int outResult;
            mock.Object.Method(out outResult);

            //assert
            Assert.Equal(frozenInt, outResult);
        }

        [Theory, AutoData]
        public void SetsUp_Indexers(Fixture fixture,
                                    [Frozen] int frozenInt, VirtualMethodInitializer initializer,
                                    Mock<IInterfaceWithIndexer> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            int result = mock.Object[3];

            //assert
            Assert.Equal(frozenInt, result);
        }

        [Theory, AutoData]
        public void IgnoresMethodsWithRefParameters(Fixture fixture,
                                                    [Frozen] string frozenString, VirtualMethodInitializer initializer,
                                                    Mock<IInterfaceWithRefParameter> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
        }


        [Theory, AutoData]
        public void IgnoresSealedMethods(Fixture fixture, VirtualMethodInitializer initializer,
                                         Mock<ClassWithSealedMethod> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));

            Assert.Equal("Awesome string", mock.Object.SealedMethod());
            Assert.Equal("Awesome string", mock.Object.ImplicitlySealedMethod());
        }

        [Theory, AutoData]
        public void IgnoresVoidMethods(Fixture fixture, VirtualMethodInitializer initializer,
                                       Mock<IInterfaceWithVoidMethod> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
        }

        [Theory, AutoData]
        public void IgnoresGenericMethods(Fixture fixture, [Frozen] string frozenString,
                                          VirtualMethodInitializer initializer,
                                          Mock<IInterfaceWithGenericMethod> mock)
        {
            //assert that a string was not retrieved from the fixture
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
            Assert.NotEqual(frozenString, mock.Object.GenericMethod<string>());
        }

        public interface IInterfaceWithMethod
        {
            string SomeMethod();
        }

        public interface IInterfaceWithProperty
        {
            string SomeProperty { get; set; }
        }

        public interface IInterfaceWithVoidMethod
        {
            void VoidMethod();
            string SetOnlyProperty { set; }
        }

        public interface IInterfaceWithGenericMethod
        {
            string GenericMethod<T>();
        }

        public class ClassWithVirtualMethod
        {
            public virtual string VirtualMethod()
            {
                throw new NotImplementedException();
            }
        }

        public abstract class TempClass
        {
            public abstract string SealedMethod();
        }

        public class ClassWithSealedMethod : TempClass
        {
            public override sealed string SealedMethod()
            {
                return "Awesome string";
            }

            public string ImplicitlySealedMethod()
            {
                return "Awesome string";
            }
        }

        public interface IInterfaceWithParameter
        {
            string Method(int i);
        }

        public interface IInterfaceWithRefParameter
        {
            string Method(ref string s);
        }

        public interface IInterfaceWithOutParameter
        {
            void Method(out int i);
        }

        public interface IInterfaceWithIndexer
        {
            int this[int index] { get; }
        }
    }
}
