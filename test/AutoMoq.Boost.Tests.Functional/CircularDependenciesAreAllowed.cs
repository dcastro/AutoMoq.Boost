using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.xUnit;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Functional
{
    public class CircularDependenciesAreAllowed
    {
        [Theory, AutoMoqBoostData]
        public void Test(Fixture fixture)
        {
            Assert.DoesNotThrow(() => fixture.Freeze<IDbConnection>());
        }

        public interface IDbConnection
        {
            IDbConnection Clone();
        }
    }
}
