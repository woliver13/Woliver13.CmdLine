using System.Collections.Generic;

namespace Woliver13.CmdLine
{
    public interface IUsageFormatter
    {
        string Usage(IEnumerable<ValidOption> validOptions);
    }
}