using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion.OSX
{
    public class ProtocolRegistry : IProtocolRegistry
    {
        public bool CheckIfExists(string protocol)
        {
            throw new NotImplementedException();
        }

        public void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool script = false)
        {
            throw new NotImplementedException();
        }
    }
}
