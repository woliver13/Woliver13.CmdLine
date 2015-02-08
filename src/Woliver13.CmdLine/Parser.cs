using System.Collections.Generic;
using System.Linq;
using Woliver13.CmdLine.Exceptions;

namespace Woliver13.CmdLine
{
    public class Parser
    {
        private IUsageFormatter _formatter;
        private IShortcutParser _shortcutParser;

        public Parser(string validOptionSetupString = null)
        {
            ValidOptions = new List<ValidOption>();
            if (string.IsNullOrWhiteSpace(validOptionSetupString)) return;
            ValidOptions.AddRange(ShortcutParser.Parse(validOptionSetupString));
        }

        public IUsageFormatter Formatter
        {
            set { _formatter = value; }
            get { return _formatter ?? (_formatter = new DefaultUsageFormatter()); }
        }

        public IShortcutParser ShortcutParser
        {
            set { _shortcutParser = value; }
            get { return _shortcutParser ?? (_shortcutParser = new DefaultShortcutParser()); }
        }

        public List<Option> Options { get; private set; }
        public List<string> Arguments { get; private set; }
        public List<ValidOption> ValidOptions { get; set; }

        public ValidOption this[char c]
        {
            get { return ValidOptions.FirstOrDefault(m => m.Flag == c); }
        }

        public string Usage
        {
            get { return Formatter.Usage(ValidOptions); }
        }

        public void Parse(IEnumerable<string> argv)
        {
            Options = new List<Option>();
            Arguments = new List<string>();
            bool needOptArg = false;
            bool stopScanning = false;
            foreach (string argument in argv)
            {
                if (stopScanning)
                {
                    Arguments.Add(argument);
                    continue;
                }
                if (!argument.StartsWith("-"))
                {
                    if (!needOptArg)
                    {
                        Arguments.Add(argument);
                        continue;
                    }
                    Options[Options.Count - 1].Argument = argument;
                    needOptArg = false;
                    continue;
                }
                if (argument == "--")
                {
                    stopScanning = true;
                    continue;
                }
                if (argument.StartsWith("--"))
                {
                    string[] parts = argument.Split('=', ':');
                    string longOpt = parts[0].Substring(2);
                    ValidOption validOption = GetLongOption(longOpt);
                    Options.Add(new Option {Flag = validOption.Flag});
                    if (validOption.OptionType != OptionType.None)
                    {
                        if (parts.Length > 1)
                            Options[Options.Count - 1].Argument = parts[1];
                        else
                            needOptArg = true;
                    }
                    continue;
                }
                foreach (char flag in argument.Where(c => c != '-'))
                {
                    if (ValidOptions.Count(v => v.Flag == flag) == 0)
                        throw new InvalidOptionException(string.Format("{0} is not a valid option.", flag));
                    Options.Add(new Option {Flag = flag});
                }
                needOptArg = ValidOptions.Last(m => m.Flag == Options[Options.Count - 1].Flag).OptionType !=
                             OptionType.None;
            }
        }

        private ValidOption GetLongOption(string longOpt)
        {
            switch (ValidOptions.Count(v => v.LongOption.StartsWith(longOpt)))
            {
                case 0:
                    throw new InvalidOptionException(string.Format("{0} is not a valid option", longOpt));
                case 1:
                    return ValidOptions.FirstOrDefault(v => v.LongOption.StartsWith(longOpt));
                default:
                    string message = string.Format("{0} could be interpreted as {1}", longOpt,
                        string.Join(" or ", (from v in ValidOptions
                            where v.LongOption.StartsWith(longOpt)
                            select v.LongOption).ToList()));
                    throw new AmbiguousOptionException(message);
            }
        }
    }
}