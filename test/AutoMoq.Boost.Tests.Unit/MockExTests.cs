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

        public interface IInterface
        {
            string Method();
        }
    }
}
