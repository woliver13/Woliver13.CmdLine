using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Woliver13.CmdLine.Exceptions;

namespace Woliver13.CmdLine.UnitTests
{
    [TestFixture]
    public class ShortcutParserTests
    {
        [Test]
        public void ShouldParseFlags()
        {
            // Arrange
            const string shortcutString = "ab:c::";
            IShortcutParser parser = new DefaultShortcutParser();

            // Act
            List<ValidOption> actual = parser.Parse(shortcutString).ToList();

            // Assert
            actual.Count.ShouldEqual(3);
            actual[0].Flag.ShouldEqual('a');
            actual[0].OptionType.ShouldEqual(OptionType.None);
            actual[0].LongOption.ShouldBeNull();
            actual[0].HelpText.ShouldBeNull();
            actual[1].Flag.ShouldEqual('b');
            actual[1].OptionType.ShouldEqual(OptionType.Required);
            actual[1].LongOption.ShouldBeNull();
            actual[1].HelpText.ShouldBeNull();
            actual[2].Flag.ShouldEqual('c');
            actual[2].OptionType.ShouldEqual(OptionType.Optional);
            actual[2].LongOption.ShouldBeNull();
            actual[2].HelpText.ShouldBeNull();
        }

        [Test]
        [ExpectedException(typeof(InvalidDefinitionException))]
        public void ShouldThrowInvalidDefinitionExceptionForDuplicateFlag()
        {
            // Arrange
            const string shortcutString = "ab:c::a::";
            IShortcutParser parser = new DefaultShortcutParser();

            // Act
            List<ValidOption> actual = parser.Parse(shortcutString).ToList();

            // Assert
            // Fail test unless exception is thrown
            true.ShouldBeFalse();
        }
    }
}
