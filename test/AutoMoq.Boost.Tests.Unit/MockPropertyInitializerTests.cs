using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class MockPropertyInitializerTests
    {
        [Theory, AutoData]
        public void SetsUpPropertiesToRetrieveReturnValueFromFixture(MockPropertyInitializer initializer,
                                                                     [Frozen] Fixture fixture,
                                                                     Mock<IInterfaceWithProperty> mock)
        {
            fixture.Freeze("a string");

            //act
            initializer.Setup(mock);
            var result = mock.Object.SomeProperty;

            //assert
            Assert.Contains("a string", result);
        }

        [Theory, AutoData]
        public void IgnoresPropertiesWithoutGetter(MockPropertyInitializer initializer,
                                                   Mock<IInterfaceWithSetProperty> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock));
        }

        public interface IInterfaceWithProperty
        {
            string SomeProperty { get; }
        }

        public interface IInterfaceWithSetProperty
        {
            string SetProperty { set; }
        }
    }
}
