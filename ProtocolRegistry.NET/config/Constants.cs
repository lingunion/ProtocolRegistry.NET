using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion
{
    public partial class ProtocolRegistry
    {
        public class Constants
        {
            public static string HomeDir => Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".protocol-registry"
             );

            public static string UrlArgment => Platform switch
            {
                Platforms.Windows => "%1",
                Platforms.Linux => "%u",
                Platforms.OSX => "\" & this_URL & \"",
            };

            public static string UrlArgmentInScript => Platform switch
            {
                Platforms.Windows => "%~1%",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
