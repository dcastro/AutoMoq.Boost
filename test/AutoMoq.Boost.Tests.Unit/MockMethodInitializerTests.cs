using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class MockMethodInitializerTests
    {
        [Theory, AutoData]
        public void SetsUp_InterfaceMethods_ToRetrieveReturnValueFromContext(Fixture fixture,
                                                                             [Frozen] string frozenString,
                                                                             MockMethodInitializer initializer,
                                                                             Mock<IInterfaceWithMethod> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.SomeMethod();

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void SetsUp_AbstractMethods_ToRetrieveReturnValueFromContext(Fixture fixture,
                                                                            [Frozen] string frozenString,
                                                                            MockMethodInitializer initializer,
                                                                            Mock<ClassWithAbstractMethod> mock)
        {
            //act
            initializer.Setup(mock, new SpecimenContext(fixture));
            var result = mock.Object.AbstractMethod();

            //assert
            Assert.Equal(frozenString, result);
        }

        [Theory, AutoData]
        public void IgnoresSealedMethods(Fixture fixture, MockMethodInitializer initializer,
                                         Mock<ClassWithSealedMethod> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));

            var result = mock.Object.AbstractMethod();
            Assert.Equal("Awesome string", result);
        }

        [Theory, AutoData]
        public void IgnoresVoidMethods(Fixture fixture, MockMethodInitializer initializer,
                                       Mock<IInterfaceWithVoidMethod> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
        }

        [Theory, AutoData]
        public void IgnoresGenericMethods(Fixture fixture, MockMethodInitializer initializer,
                                          Mock<IInterfaceWithGenericMethod> mock)
        {
            fixture.Freeze<string>();
            mock.DefaultValue = DefaultValue.Empty;

            //assert that a string was not retrieved from the fixture
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
            Assert.Null(mock.Object.GenericMethod<string>());
        }

        public interface IInterfaceWithMethod
        {
            string SomeMethod();
        }

        public interface IInterfaceWithVoidMethod
        {
            void VoidMethod();
        }

        public interface IInterfaceWithGenericMethod
        {
            string GenericMethod<T>();
        }

        public class ClassWithAbstractMethod
        {
            public virtual string AbstractMethod()
            {
                throw new NotImplementedException();
            }
        }

        public class ClassWithSealedMethod : ClassWithAbstractMethod
        {
            public override sealed string AbstractMethod()
            {
                return "Awesome string";
            }
        }
    }
}
