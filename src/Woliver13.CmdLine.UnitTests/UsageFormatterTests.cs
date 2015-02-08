using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace Woliver13.CmdLine.UnitTests
{
    [TestFixture]
    public class UsageFormatterTests
    {
        [Test]
        public void ShouldShowUsage()
        {
            // Arrange
            const string expected = @"-h This help message
-i inputFileName | --inputfile inputFileName
-o [argument] Full path to the output file or standard output instead of the log file if no argument is defined.
";
            var validOptions = new List<ValidOption>
            {
                new ValidOption {Flag = 'h', HelpText = "This help message"},
                new ValidOption
                {
                    Flag = 'i',
                    OptionType = OptionType.Required,
                    ArgumentName = "inputFileName",
                    LongOption = "inputfile"
                },
                new ValidOption
                {
                    Flag = 'o',
                    OptionType = OptionType.Optional,
                    HelpText =
                        "Full path to the output file or standard output instead of the log file if no argument is defined."
                }
            };
            var formatter = new DefaultUsageFormatter();

            // Act
            string actual = formatter.Usage(validOptions);

            // Assert
            actual.ShouldEqual(expected);
        }
    }
}