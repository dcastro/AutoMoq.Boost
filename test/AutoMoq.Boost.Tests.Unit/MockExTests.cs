using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MockExTests
    {
        [Theory, AutoData]
        public void ReturnsUsingFixture(Fixture fixture, Mock<IInterface> mock)
        {
            var stringFromFixture = fixture.Freeze<string>();

            mock.Setup(x => x.Method()).ReturnsUsingFixture(fixture);
            var result = mock.Object.Method();

            Assert.Equal(stringFromFixture, result);
        }

        [Theory, AutoData]
        public void ReturnsLazily(Mock<IFixture> fixture, Mock<IInterface> mock)
        {
            //setup
            fixture.Setup(x => x.Create(It.IsAny<object>(), It.IsAny<ISpecimenContext>()))
                   .Returns("");

            //act
            mock.Setup(x => x.Method()).ReturnsUsingFixture(fixture.Object);

            //assert
            fixture.Verify(x => x.Create(It.IsAny<object>(), It.IsAny<ISpecimenContext>()), Times.Never());

            mock.Object.Method();

            fixture.Verify(x => x.Create(It.IsAny<object>(), It.IsAny<ISpecimenContext>()), Times.Once());
        }

        public interface IInterface
        {
            string Method();
        }
    }
}
