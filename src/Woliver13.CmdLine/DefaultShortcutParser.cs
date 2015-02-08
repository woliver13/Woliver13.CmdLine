using System.Collections.Generic;
using System.Linq;
using Woliver13.CmdLine.Exceptions;

namespace Woliver13.CmdLine
{
    public class DefaultShortcutParser : IShortcutParser
    {
        public IEnumerable<ValidOption> Parse(string shortcut)
        {
            var validOptions = new List<ValidOption>();
            if (string.IsNullOrWhiteSpace(shortcut)) return validOptions;
            ValidOption validOption = null;
            foreach (char flag in shortcut)
            {
                if (flag != ':')
                {
                    if (validOption != null)
                        validOptions.Add(validOption);
                    if (validOptions.Count(v => v.Flag == flag) != 0)
                        throw new InvalidDefinitionException(string.Format("{0} is used more than once in the shortcut list '{1}'",
                            flag, shortcut));
                    validOption = new ValidOption {Flag = flag};
                    continue;
                }
                if (validOption != null)
                    validOption.OptionType++;
            }
            if (validOption == null) return validOptions;
            if (validOptions.Contains(validOption)) return validOptions;
            validOptions.Add(validOption);
            return validOptions;
        }
    }
}