using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class AutoMoqBoostCustomizationTests
    {
        [Theory, AutoMoqData]
        public void Customize_AddsMockSetup(AutoMoqBoostCustomization boost, IFixture fixture)
        {
            boost.Customize(fixture);

            Assert.True(fixture.Customizations.Any(builder => builder is MockSetup));
        }

        [Theory, AutoMoqData]
        public void Customize_AddsInitializers(AutoMoqBoostCustomization boost, IFixture fixture)
        {
            boost.Customize(fixture);

            var mockSetup = (MockSetup) fixture.Customizations.Single(builder => builder is MockSetup);
            Assert.True(mockSetup.Initializers.Any(init => init is MockMethodInitializer));
            Assert.True(mockSetup.Initializers.Any(init => init is MockPropertyInitializer));
        }

        [Theory, AutoMoqData]
        public void Customize_AddsResidueCollector(AutoMoqBoostCustomization boost, IFixture fixture)
        {
            boost.Customize(fixture);

            Assert.True(fixture.ResidueCollectors.Any(collector => collector is MockRelay));
        }
    }
}
