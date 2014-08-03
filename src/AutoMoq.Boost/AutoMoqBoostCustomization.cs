using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace Dash.AutoMoq.Boost
{
    /// <summary>
    /// Enables auto-mocking and auto-setup with Moq.
    /// Members of a mock will be automatically setup to retrieve the return values from a fixture.
    /// </summary>
    public class AutoMoqBoostCustomization : ICustomization
    {
        private readonly ISpecimenBuilder _relay;

        /// <summary>
        /// Creates a new instance of <see cref="AutoMoqBoostCustomization"/>.
        /// </summary>
        public AutoMoqBoostCustomization()
            : this(new MockRelay())
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="AutoMoqBoostCustomization"/>.
        /// </summary>
        /// <param name="relay">A mock relay to be added to <see cref="IFixture.ResidueCollectors"/></param>
        public AutoMoqBoostCustomization(ISpecimenBuilder relay)
        {
            if(relay == null)
                throw new ArgumentNullException("relay");

            _relay = relay;
        }
        
        /// <summary>
        /// Gets the relay that will be added to <see cref="IFixture.ResidueCollectors"/> when <see cref="Customize"/> is invoked.
        /// </summary>
        public ISpecimenBuilder Relay
        {
            get { return _relay; }
        }

        /// <summary>
        /// Customizes a <see cref="IFixture"/> to enable auto-mocking and auto-setup with Moq.
        /// Members of a mock will be automatically setup to retrieve the return values from <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The fixture to customize.</param>
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new MockSetup(
                    new MockPostprocessor(
                        new MethodInvoker(
                            new MockConstructorQuery())),
                    new VirtualMethodInitializer()));

            fixture.ResidueCollectors.Add(Relay);
        }
    }
}
