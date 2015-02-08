namespace Woliver13.CmdLine
{
    public class ValidOption
    {
        private string _argumentName;

        public char Flag { get; set; }
        public string LongOption { get; set; }
        public OptionType OptionType { get; set; }
        public string HelpText { get; set; }

        public string ArgumentName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_argumentName)) return _argumentName;
                _argumentName = !string.IsNullOrWhiteSpace(LongOption)
                    ? string.Format("{0}", LongOption)
                    : "argument";
                return _argumentName;
            }
            set { _argumentName = value; }
        }
    }
}