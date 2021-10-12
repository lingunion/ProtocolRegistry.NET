using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion
{
    public interface IProtocolRegistry
    {
        public void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool script = false);
        public bool CheckIfExists(string protocol);
    }
}
