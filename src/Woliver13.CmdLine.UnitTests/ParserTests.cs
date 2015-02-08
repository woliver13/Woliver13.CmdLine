using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Woliver13.CmdLine.Exceptions;

namespace Woliver13.CmdLine.UnitTests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ShouldBuildArgAndOption()
        {
            // Arrange
            string[] argv = {"-a", "foo", "bar", "-d"};
            var parser = new Parser("a::d");

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(2);
            parser.ValidOptions[0].Flag.ShouldEqual('a');
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.Optional);
            parser.ValidOptions[1].Flag.ShouldEqual('d');
            parser.ValidOptions[1].OptionType.ShouldEqual(OptionType.None);

            parser.Options.Count.ShouldEqual(2);
            parser.Options[0].Flag.ShouldEqual('a');
            parser.Options[0].Argument.ShouldEqual("foo");
            parser.Options[1].Flag.ShouldEqual('d');
            parser.Options[1].Argument.ShouldBeNull();

            parser.Arguments.Count.ShouldEqual(1);
            parser.Arguments[0].ShouldEqual("bar");
        }

        [Test]
        public void ShouldGetMultipleFlags()
        {
            // Arrange
            string[] argv = {"-abc"};
            var parser = new Parser("abc");

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(3);
            parser.ValidOptions[0].Flag.ShouldEqual('a');
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.None);
            parser.ValidOptions[1].Flag.ShouldEqual('b');
            parser.ValidOptions[1].OptionType.ShouldEqual(OptionType.None);
            parser.ValidOptions[2].Flag.ShouldEqual('c');
            parser.ValidOptions[2].OptionType.ShouldEqual(OptionType.None);

            parser.Options.Count.ShouldEqual(3);
            parser.Options[0].Flag.ShouldEqual('a');
            parser.Options[0].Argument.ShouldBeNull();
            parser.Options[1].Flag.ShouldEqual('b');
            parser.Options[1].Argument.ShouldBeNull();
            parser.Options[2].Flag.ShouldEqual('c');
            parser.Options[2].Argument.ShouldBeNull();
        }

        [Test]
        public void ShouldSeeLongOptions()
        {
            // Arrange
            string[] argv = {"-h", "--help"};
            var parser = new Parser
            {
                ValidOptions = new List<ValidOption> {new ValidOption {Flag = 'h', LongOption = "help"}}
            };

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(1);
            parser.ValidOptions[0].Flag.ShouldEqual('h');
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.None);

            parser.Options.Count.ShouldEqual(2);
            parser.Options[0].Flag.ShouldEqual('h');
            parser.Options[0].Argument.ShouldBeNull();
            parser.Options[1].Flag.ShouldEqual('h');
            parser.Options[1].Argument.ShouldBeNull();
        }

        [Test]
        public void ShouldSeeLongOptionsWithArgs()
        {
            // Arrange
            string[] argv = {"--outputdir=foo", "--inputdir", "bar"};
            var parser = new Parser
            {
                ValidOptions = new List<ValidOption>
                {
                    new ValidOption {Flag = 'o', LongOption = "outputdir", OptionType = OptionType.Required},
                    new ValidOption {Flag = 'i', LongOption = "inputdir", OptionType = OptionType.Required}
                }
            };

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(2);
            parser.ValidOptions[0].Flag.ShouldEqual('o');
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.Required);
            parser.ValidOptions[1].Flag.ShouldEqual('i');
            parser.ValidOptions[1].OptionType.ShouldEqual(OptionType.Required);

            parser.Options.Count.ShouldEqual(2);
            parser.Options[0].Flag.ShouldEqual('o');
            parser.Options[0].Argument.ShouldEqual("foo");
            parser.Options[1].Flag.ShouldEqual('i');
            parser.Options[1].Argument.ShouldEqual("bar");
        }

        [Test]
        public void ShouldSeeShortenedLongOption()
        {
            // Arrange
            string[] argv = {"--in=abc"};
            var parser = new Parser();
            parser.ValidOptions.Add(new ValidOption
            {
                Flag = 'i',
                LongOption = "inputfile",
                OptionType = OptionType.Required
            });

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(1);
            parser.ValidOptions[0].Flag.ShouldEqual('i');
            parser.ValidOptions[0].LongOption.ShouldEqual("inputfile");
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.Required);

            parser.Options.Count.ShouldEqual(1);
            parser.Options[0].Flag.ShouldEqual('i');
            parser.Options[0].Argument.ShouldEqual("abc");

            parser.Arguments.Count.ShouldEqual(0);
        }

        [Test]
        public void ShouldStopScanningOnDoubleDash()
        {
            // Arrange
            string[] argv = {"foo", "-a", "--", "bar", "-d"};
            var parser = new Parser("a");

            // Act
            parser.Parse(argv);

            // Assert
            parser.ValidOptions.Count.ShouldEqual(1);
            parser.ValidOptions[0].Flag.ShouldEqual('a');
            parser.ValidOptions[0].OptionType.ShouldEqual(OptionType.None);

            parser.Options.Count.ShouldEqual(1);
            parser.Options[0].Flag.ShouldEqual('a');
            parser.Options[0].Argument.ShouldBeNull();

            parser.Arguments.Count.ShouldEqual(3);
            parser.Arguments[0].ShouldEqual("foo");
            parser.Arguments[1].ShouldEqual("bar");
            parser.Arguments[2].ShouldEqual("-d");
        }

        [Test]
        [ExpectedException(typeof (AmbiguousOptionException))]
        public void ShouldThrowAmbiguousOptionExceptionForDuplicateShortenedLongOption()
        {
            // Arrange
            string[] argv = {"--in=abc"};
            var parser = new Parser();
            parser.ValidOptions.Add(new ValidOption {Flag = 'i', LongOption = "inputfile"});
            parser.ValidOptions.Add(new ValidOption {Flag = 'f', LongOption = "information"});

            // Act
            parser.Parse(argv);

            // Assert
            // Fail test unless exception is thrown
            true.ShouldBeFalse();
        }

        [Test]
        [ExpectedException(typeof (InvalidOptionException))]
        public void ShouldThrowInvalidOptionExceptionForInvalidFlag()
        {
            // Arrange
            string[] argv = {"-abcde"};
            var parser = new Parser("abcd");

            // Act
            parser.Parse(argv);

            // Assert
            // Fail test unless exception is thrown
            true.ShouldBeFalse();
        }

        [Test]
        [ExpectedException(typeof (InvalidOptionException))]
        public void ShouldThrowInvalidOptionExceptionForUnknownLongOption()
        {
            // Arrange
            string[] argv = {"--out=abc"};
            var parser = new Parser
            {
                ValidOptions = new List<ValidOption>
                {
                    new ValidOption {Flag = 'i', LongOption = "inputfile"},
                    new ValidOption {Flag = 'h', LongOption = "information"}
                }
            };

            // Act
            parser.Parse(argv);

            // Assert
            // Fail test unless exception is thrown
            true.ShouldBeFalse();
        }
    }
}