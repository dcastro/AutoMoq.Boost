using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class MockSetupTests
    {
        [Theory, AutoMoqData]
        public void TriggersInitializers_When_UnderlyingBuilderHandlesRequest(Mock<ISpecimenBuilder> builder,
                                                                              IList<IMockInitializer> inits,
                                                                              ISpecimenContext context)
        {
            var request = new object();
            var mock = new Mock<object>();
            builder.Setup(b => b.Create(request, context)).Returns(mock);

            //act
            var setup = new MockSetup(builder.Object, inits);
            setup.Create(request, context);

            //assert
            foreach (var initializer in inits)
                Mock.Get(initializer).Verify(i => i.Setup(mock, context), Times.Once());
        }

        [Theory, AutoMoqData]
        public void DoesnotTriggerInitializers_When_UnderlyingBuilderCantHandleRequest(Mock<ISpecimenBuilder> builder,
                                                                                       IList<IMockInitializer> inits,
                                                                                       ISpecimenContext context)
        {
            var request = new object();
            var specimen = new NoSpecimen();
            builder.Setup(b => b.Create(request, context)).Returns(specimen);

            //act
            var setup = new MockSetup(builder.Object, inits);
            setup.Create(request, context);

            //assert
            foreach (var initializer in inits)
                Mock.Get(initializer).Verify(i => i.Setup(It.IsAny<Mock>(), null), Times.Never());
        }
    }
}
