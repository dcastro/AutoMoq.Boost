using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.xUnit;
using Moq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Functional
{
    public class SealedMethodsAreIgnored
    {
        [Theory, AutoMoqBoostData]
        public void ExplicitlySealedMethod(Fixture fixture)
        {
            Assert.DoesNotThrow(() => fixture.Create<Mock<ClassWithSealedMethods>>());
        }

        public class Temp
        {
            public virtual string ExplicitlySealedMethod()
            {
                return Guid.NewGuid().ToString();
            }
        }

        public class ClassWithSealedMethods : Temp
        {
            public override sealed string ExplicitlySealedMethod()
            {
                return Guid.NewGuid().ToString();
            }

            public string ImplicitlySealedMethod()
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
