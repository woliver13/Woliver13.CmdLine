using System.Collections.Generic;

namespace Woliver13.CmdLine
{
    public interface IShortcutParser
    {
        IEnumerable<ValidOption> Parse(string shortcutString);
    }
}
