using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.xUnit;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Functional
{
    public class InterfaceMembersReturnValueFromFixture
    {
        [Theory, AutoMoqBoostData]
        public void Methods([Frozen] string frozenString, IInterfaceWithDependency mockedObject)
        {
            Assert.Same(frozenString, mockedObject.Method());
        }

        [Theory, AutoMoqBoostData]
        public void Properties([Frozen] string frozenString, IInterfaceWithDependency mockedObject)
        {
            Assert.Same(frozenString, mockedObject.Property);
        }

        public interface IInterfaceWithDependency
        {
            string Property { get; }
            string Method();
        }
    }
}
