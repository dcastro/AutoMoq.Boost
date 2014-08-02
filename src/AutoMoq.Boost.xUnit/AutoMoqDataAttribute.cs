using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace Dash.AutoMoq.Boost.xUnit
{
    public class AutoMoqBoostDataAttribute : AutoDataAttribute
    {
        public AutoMoqBoostDataAttribute()
            : base(new Fixture()
                       .Customize(new AutoMoqBoostCustomization()))
        {
        }
    }
}
