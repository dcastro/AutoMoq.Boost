using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.xUnit;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Functional
{
    public class SealedSettablePropertiesAreSetUsingFixture
    {
        [Theory, AutoMoqBoostData]
        public void ExplicitlySealedProperty([Frozen] string frozenString, Mock<ClassWithSealedProperties> mock)
        {
            Assert.Same(frozenString, mock.Object.SealedProperty);
        }

        [Theory, AutoMoqBoostData]
        public void ImplicitlySealedProperty([Frozen] string frozenString, Mock<ClassWithSealedProperties> mock)
        {
            Assert.Same(frozenString, mock.Object.ImplicitlySealedProperty);
        }

        public class Temp
        {
            public virtual string SealedProperty { get; set; }
        }

        public class ClassWithSealedProperties : Temp
        {
            public override sealed string SealedProperty { get; set; }

            public string ImplicitlySealedProperty { get; set; }
        }
    }
}
