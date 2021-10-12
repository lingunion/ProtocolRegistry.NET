using System;
using System.Runtime.InteropServices;

namespace LingUnion
{
    public partial class ProtocolRegistry
    {
        private static Platforms? _platform = null;
        public enum Platforms
        {
            Windows,
            Linux,
            OSX,
        }

        public static Platforms Platform
        {
            get
            {
                if (_platform is null)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        _platform = Platforms.Windows;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        _platform = Platforms.Linux;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        _platform = Platforms.OSX;
                    }
                    else
                    {
                        throw new PlatformNotSupportedException();
                    }
                }
                return (Platforms)_platform;
            }
        }
    }
}
