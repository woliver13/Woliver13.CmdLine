using System.Collections.Generic;
using System.Text;

namespace Woliver13.CmdLine
{
    public class DefaultUsageFormatter : IUsageFormatter
    {
        public string Usage(IEnumerable<ValidOption> validOptions)
        {
            var result = new StringBuilder();
            foreach (ValidOption option in validOptions)
            {
                result.AppendFormat("-{0}{1}", option.Flag, AddArgument(option));
                if (!string.IsNullOrWhiteSpace(option.LongOption))
                    result.AppendFormat(" | --{0}{1}", option.LongOption, AddArgument(option));
                if (!string.IsNullOrWhiteSpace(option.HelpText))
                    result.AppendFormat(" {0}", option.HelpText);
                result.AppendLine();
            }
            return result.ToString();
        }

        private static string AddArgument(ValidOption option)
        {
            switch (option.OptionType)
            {
                case OptionType.Optional:
                    return string.Format(" [{0}]", option.ArgumentName);
                case OptionType.Required:
                    return string.Format(" {0}", option.ArgumentName);
                default:
                    return string.Empty;
            }
        }
    }
}