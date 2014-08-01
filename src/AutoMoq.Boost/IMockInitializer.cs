using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

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
        void Setup(Mock mock);
    }
}
