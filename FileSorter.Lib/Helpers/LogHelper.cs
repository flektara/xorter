using System.Runtime.CompilerServices;

namespace FileSorter.Lib.Helpers
{
    public class LogHelper
    {
        public static log4net.ILog GetLogger([CallerFilePath] string name = "")
        {
            return log4net.LogManager.GetLogger(name);
        }
    }
}
