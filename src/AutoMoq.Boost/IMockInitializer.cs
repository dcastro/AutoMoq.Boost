using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Initializes or sets up mocks created by AutoMoq.
    /// </summary>
    public interface IMockInitializer
    {
        /// <summary>
        /// Initializes or sets up a mock.
        /// </summary>
        /// <param name="mock">The mock to initialize/setup.</param>
        /// <param name="context">The context of the mock.</param>
        void Setup(Mock mock, ISpecimenContext context);
    }
}
