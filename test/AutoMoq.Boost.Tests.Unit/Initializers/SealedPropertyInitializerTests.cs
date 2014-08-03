using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Dash.AutoMoq.Boost.Tests.Unit
{
    public class SealedPropertyInitializerTests
    {
        [Theory, AutoData]
        public void Initializes_SealedProperty_UsingContext(Fixture fixture, [Frozen] string frozenString,
                                                            SealedPropertyInitializer initializer,
                                                            Mock<ClassWithSealedProperty> mock)
        {
            initializer.Setup(mock, new SpecimenContext(fixture));

            Assert.Equal(frozenString, mock.Object.SealedProperty);
            Assert.Equal(frozenString, mock.Object.ImplicitlySealedProperty);
        }

        [Theory, AutoData]
        public void IgnoresGetOnlyProperties(Fixture fixture, SealedPropertyInitializer initializer,
                                             Mock<ClassWithReadOnlyProperty> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
        }

        [Theory, AutoData]
        public void IgnoresVirtualProperties(Fixture fixture, [Frozen] string frozenString,
                                             SealedPropertyInitializer initializer,
                                             Mock<ClassWithVirtualProperty> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
            Assert.NotEqual(frozenString, mock.Object.VirtualProperty);
        }

        [Theory, AutoData]
        public void IgnoresInterfaceProperties(Fixture fixture, [Frozen] string frozenString,
                                               SealedPropertyInitializer initializer,
                                               Mock<IInterfaceWithProperty> mock)
        {
            Assert.DoesNotThrow(() => initializer.Setup(mock, new SpecimenContext(fixture)));
            Assert.NotEqual(frozenString, mock.Object.Property);
        }

        public abstract class TempClass
        {
            public abstract string SealedProperty { get; set; }
        }

        public class ClassWithSealedProperty : TempClass
        {
            public override sealed string SealedProperty { get; set; }
            public string ImplicitlySealedProperty { get; set; }
        }

        public class ClassWithReadOnlyProperty
        {
            public string ReadOnlyProperty
            {
                get { return ""; }
            }
        }

        public class ClassWithVirtualProperty
        {
            public virtual string VirtualProperty { get; set; }
        }

        public interface IInterfaceWithProperty
        {
            string Property { get; set; }
        }
    }
}
