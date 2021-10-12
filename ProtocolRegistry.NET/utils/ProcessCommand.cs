using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion
{
    public partial class ProtocolRegistry
    {
        public static string subtituteCommand(string command, string url) => string.Join(url, command.Split("$_URL_"));
        public static string PreProcessCommand(string protocol, string command, bool scriptRequired)
        {
            if (!scriptRequired)
            {
                return subtituteCommand(command, Constants.UrlArgment);
            } else
            {
                return HandleWrapperScript(protocol, command);
            }
        }
    }
}
