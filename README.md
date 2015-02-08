The is yet another Command Line processor.

Sample usage:
```c#
public static class Program
{
	public static int Main(string[] argv)
	{
		bool flag1 = false;
		string optionArg2 = string.Empty;
		bool flag3 = false;
		string optionArg3 = string.Empty;

		var parser = new Parser("ab:c::h");

		parser.Parse(argv);
		foreach(Option option in parser.Options)
		{
			switch(option.Flag) 
			{
				case 'a':
					flag1 = true;
					break;
				case 'b':
					optionArg2 = option.Argument;
				case 'c':
					flag3 = true;
					optionArg3 = option.Argument;
				case 'h':
					Console.Write(parser.Usage);
					return;
			}
		}

		// TODO: do something with the options.
	}
}
```
