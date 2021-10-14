using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion.Windows
{
#pragma warning disable CA1416 // 플랫폼 호환성 유효성 검사
    public class ProtocolRegistry : IProtocolRegistry
    {
        public bool CheckIfExists(string protocol)
        {

            var registryKey = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{protocol}");

            if (registryKey == null) return false;
            return true;
        }

        public void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool scriptRequired = false)
        {
            string keyPath = $"Software\\Classes\\{protocol}";
            if (this.CheckIfExists(protocol))
            {
                if (overwrite == false) throw new Exception("Protocol already exists");
                Registry.CurrentUser.DeleteSubKeyTree(keyPath);
            }
            string processedCommand = LingUnion.ProtocolRegistry.PreProcessCommands(protocol, command, scriptRequired);

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(keyPath);

            string urlDecl = $"URL:{protocol}";
            string cmdPath = $"{keyPath}\\shell\\open\\command";

            registryKey.SetValue(
                "URL Protocol",
                "",
                RegistryValueKind.String // REG_SZ
                );

            registryKey.SetValue(
                null,
                urlDecl,
                RegistryValueKind.String // REG_SZ
                );

            RegistryKey commandKey = Registry.CurrentUser.CreateSubKey(cmdPath);
            commandKey.SetValue(
                null,
                // Caution: Fix when terminal is false, quota should be removed
                $"{(terminal ? "cmd /k " : "")}\"{processedCommand}\"",
                RegistryValueKind.String // REG_SZ
                );
        }
    }
#pragma warning restore CA1416 // 플랫폼 호환성 유효성 검사
}
