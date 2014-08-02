using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class MockMethodInitializerTests
    {
        [Theory, AutoData]
        public void SetsUpMethodsToRetrieveReturnValueFromFixture(MockMethodInitializer initializer,
                                                                  [Frozen] Fixture fixture,
                                                                  Mock<IInterfaceWithMethod> mock)
        {
            var stringFromFixture = fixture.Freeze<string>();

            //act
            initializer.Setup(mock);
            var result = mock.Object.SomeMethod();

            //assert
            Assert.Equal(stringFromFixture, result);
        }

        [Theory, AutoData]
        public void IgnoresVoidMethods(MockMethodInitializer initializer, Mock<IInterfaceWithVoidMethod> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock));
        }

        [Theory, AutoData]
        public void IgnoresGenericMethods(MockMethodInitializer initializer, [Frozen] Fixture fixture,
                                          Mock<IInterfaceWithGenericMethod> mock)
        {
            fixture.Freeze<string>();
            mock.DefaultValue = DefaultValue.Empty;

            //assert that a string was not retrieved from the fixture
            Assert.DoesNotThrow(() => initializer.Setup(mock));
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
    }
}
