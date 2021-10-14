using System;
using System.Runtime.InteropServices;

namespace LingUnion
{
    public partial class ProtocolRegistry
    {
        private static IProtocolRegistry protocolRegistry;
        static ProtocolRegistry()
        {
            protocolRegistry = Platform switch
            {
                Platforms.Windows => new Windows.ProtocolRegistry(),
                Platforms.Linux => new Linux.ProtocolRegistry(),
                Platforms.OSX => new OSX.ProtocolRegistry(),
            };
        }
        public static bool CheckIfExists(string protocol)
        {
            return protocolRegistry.CheckIfExists(protocol);
        }

        // Caution: protocol must contain lower alphabet and hypen
        public static void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool script = false)
        {
            protocolRegistry.Register(protocol, command, overwrite, terminal, script);
        }
    }
}
