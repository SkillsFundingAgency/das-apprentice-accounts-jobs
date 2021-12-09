using NUnit.Framework;
using System;

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus.Tests
{
    public class TestAzureQueueNameShortener
    {
        [Test]
        public void Name_under_limit_when_common_parts_removed_is_used_whole()
        {
            var shortName = AzureQueueNameShortener.Shorten(typeof(SFA.DAS.TestLib.Messages.Events.Tiny));
            Assert.That(shortName, Is.EqualTo("TestLib.Tiny"));
        }

        [TestCase(typeof(VeryLongNamespaceThatItselfIsMoreThan50CharactersLong.ShortName), "ShortName.140174A7")]
        [TestCase(typeof(VeryLongNamespaceThatItselfIsMoreThan50CharactersLong.AFairlyLongNameButUnder50Chars), "AFairlyLongNameButUnder50Chars.D965D415")]
        [TestCase(typeof(VeryLongNamespaceThatItselfIsMoreThan50CharactersLong.ThisIsTheReallyReallyReallyLongNameThatIsOver50CharsItself), "ThisIsTheReallyReallyReallyLongNameThatIs.E4D35887")]
        [TestCase(typeof(LongNamespaceButUnder50Chars.ShortName), "LongNamespaceButUnder50Chars.ShortName")]
        [TestCase(typeof(LongNamespaceButUnder50Chars.AFairlyLongNameButUnder50Chars), "AFairlyLongNameButUnder50Chars.533EC818")]
        [TestCase(typeof(LongNamespaceButUnder50Chars.ThisIsTheReallyReallyReallyLongNameThatIsOver50CharsItself), "ThisIsTheReallyReallyReallyLongNameThatIs.6B7A0139")]
        public void Name_over_limit_when_common_parts_removed_is_used_shortened(Type type, string name)
        {
            var shortName = AzureQueueNameShortener.Shorten(type);
            Assert.That(shortName.Length, Is.LessThanOrEqualTo(50));
            Assert.That(shortName, Is.EqualTo(name));
        }
    }
}

namespace SFA.DAS.TestLib.Messages.Events
{
    public class Tiny { }
}

namespace VeryLongNamespaceThatItselfIsMoreThan50CharactersLong
{
    public class ShortName { }
    public class AFairlyLongNameButUnder50Chars { }
    public class ThisIsTheReallyReallyReallyLongNameThatIsOver50CharsItself { }
}

namespace LongNamespaceButUnder50Chars
{
    public class ShortName { }
    public class AFairlyLongNameButUnder50Chars { }
    public class ThisIsTheReallyReallyReallyLongNameThatIsOver50CharsItself { }
}