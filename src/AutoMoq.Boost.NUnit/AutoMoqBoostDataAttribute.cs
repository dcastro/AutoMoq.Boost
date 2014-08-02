using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.NUnit2;

namespace Dash.AutoMoq.Boost.NUnit
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
