using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dash.AutoMoq.Boost.xUnit;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Functional
{
    public class MocksGetTheirDependenciesFromFixture
    {
        [Theory, AutoMoqBoostData]
        public void Test(Fixture fixture)
        {
            fixture.Freeze<Mock<IDbCommand>>()
                   .Setup(cmd => cmd.ExecuteScalar())
                   .Returns(2);

            var repo = fixture.Create<Repo>();

            Assert.Equal(2, repo.Count());
        }

        public class Repo
        {
            private readonly Func<IDbConnection> _connFactory;

            public Repo(Func<IDbConnection> connFactory)
            {
                _connFactory = connFactory;
            }

            public int Count()
            {
                using (var conn = _connFactory())
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public interface IDbConnection : IDisposable
        {
            void Open();
            IDbCommand CreateCommand();
        }

        public interface IDbCommand : IDisposable
        {
            object ExecuteScalar();
        }
    }
}
