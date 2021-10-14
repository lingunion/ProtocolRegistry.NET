using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion.Linux
{
    public class ProtocolRegistry : IProtocolRegistry
    {
        public bool CheckIfExists(string protocol)
        {
            Process xdgMine = new Process()
            {
                StartInfo = {
                        FileName = "xdg-mime",
                        ArgumentList =
                        {
                            "query",
                            "default",
                            $"x-scheme-handler/{protocol}",
                        },
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    },
            };
            xdgMine.Start();
            xdgMine.WaitForExit();
            string stdOutput = xdgMine.StandardOutput.ReadToEnd();
            string stdError = xdgMine.StandardError.ReadToEnd();
            if (xdgMine.ExitCode != 0 || !string.IsNullOrWhiteSpace(stdError))
            {
                throw new Exception(stdError);
            }
            if (string.IsNullOrWhiteSpace(stdOutput))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool script = false)
        {
            bool exist = CheckIfExists(protocol);
            if (exist)
            {
                if (!overwrite) throw new Exception("Protocol already exists");
            }

            string desktopFileName = $"{protocol}.desktop";
            string desktopFilePath = Path.Join(Directory.GetCurrentDirectory(), "temp", desktopFileName);
            string scriptFilePath = Path.Join(Directory.GetCurrentDirectory(), "temp", "script.sh");
            Directory.CreateDirectory("temp");

            string processedCommand = LingUnion.ProtocolRegistry.PreProcessCommands(protocol, command, script);

            // Caution: Exec whitespace should be \s not '~~~~'
            string desktopFileContent = $@"[Desktop Entry]
Type=Application
Name=URL {protocol} 
Exec={processedCommand}
StartupNotify=false
Terminal={(terminal ? "true" : "false")}
MimeType=x-scheme-handler/{protocol};";
            File.WriteAllLines(desktopFilePath,
                               desktopFileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries),
                               new UTF8Encoding(false));

            string scriptContent = $@"set -e;
mv '{desktopFilePath}'  ~/.local/share/applications
cd ~/.local/share/applications
xdg-mime default {desktopFileName} x-scheme-handler/{protocol}
";
            // Caution when Apps directory not exist
            // string AppDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "/.local/share/applications");
            // Directory.CreateDirectory(AppDir);
            File.WriteAllLines(scriptFilePath,
                               scriptContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries),
                               new UTF8Encoding(false));

            Process chmod = new Process()
            {
                StartInfo = {
                    FileName = "chmod",
                    ArgumentList =
                    {
                        "+x",
                        scriptFilePath,
                    },
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    },
            };
            chmod.Start();
            chmod.WaitForExit();
            string chmodError = chmod.StandardError.ReadToEnd();
            if (chmod.ExitCode != 0 || !string.IsNullOrWhiteSpace(chmodError))
            {
                throw new Exception(chmodError);
            }

            Process scriptProcess = new Process()
            {
                StartInfo =
                {
                    FileName = "/bin/bash",
                    ArgumentList = { "-c", $"'{scriptFilePath}'" },
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };
            scriptProcess.Start();
            scriptProcess.WaitForExit();
            string scriptError = scriptProcess.StandardError.ReadToEnd();
            if (scriptProcess.ExitCode != 0 || !string.IsNullOrWhiteSpace(scriptError))
            {
                
               throw new Exception(scriptError);
            }
            File.Delete(scriptFilePath);
        }
    }
}
