using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static LingUnion.ProtocolRegistry;

namespace LingUnion.OSX
{
    public partial class ProtocolRegistry : IProtocolRegistry
    {
        static readonly string defaultAppExistPath = Path.Join(Constants.HomeDir, "defaultAppExist.sh");

        public string GetEmbededResource(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        public string[] SplitNewLines(string old)
        {
            return old.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void PrepareDefaultAppExist()
        {
            string content = GetEmbededResource("LingUnion.OSX.defaultAppExist.sh");
            Directory.CreateDirectory(Constants.HomeDir);
            File.WriteAllLines(defaultAppExistPath, SplitNewLines(content));

            Process chmod = new()
            {
                StartInfo =
                        {
                            FileName = "chmod",
                            ArgumentList = {"+x", defaultAppExistPath},
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                        }
            };
            chmod.Start();
            chmod.WaitForExit();
            if (chmod.ExitCode != 0)
            {
                throw new Exception(chmod.StandardError.ReadToEnd());
            }
        }

        public ProtocolRegistry()
        {
            PrepareDefaultAppExist();
        }

        public bool CheckIfExists(string protocol)
        {
            Process defaultAppExist = new()
            {
                StartInfo =
                {
                    FileName = defaultAppExistPath,
                    ArgumentList = {$"{protocol}://test"},
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };
            defaultAppExist.Start();
            defaultAppExist.WaitForExit();
            string defaultAppExistError = defaultAppExist.StandardError.ReadToEnd();
            string defaultAppExistOutput = defaultAppExist.StandardOutput.ReadToEnd();
            if (defaultAppExist.ExitCode != 0 || !string.IsNullOrWhiteSpace(defaultAppExistError))
            {
                throw new Exception(defaultAppExistError);
            }
            if (defaultAppExistOutput.Trim() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Register(string protocol, string command, bool overwrite = false, bool terminal = false, bool script = false)
        {
            bool exist = CheckIfExists(protocol);
            if (exist)
            {
                if (!overwrite)
                {
                    throw new Exception("Protocol already exists");
                }
            }

            string processedCommand = PreProcessCommands(protocol, command, script);

            string appPath = Path.Join(Constants.HomeDir, $"APP-{protocol}.app");
            string urlAppPath = Path.Join(Constants.HomeDir, $"URL-{protocol}.app");
            string scriptFilePath = Path.Join(Constants.HomeDir, "script.sh");

            string appSource = Path.Join(Constants.HomeDir, $"app-{protocol}.txt");
            string appSourceContent = $@"set this_URL to ( the clipboard as text )
{(terminal ?
$@"tell application ""Terminal""
    do script ""{processedCommand}""
    activate
end tell" :
$@"do shell script ""{processedCommand}"""
)}
set the clipboard to """"";
            File.WriteAllLines(appSource, SplitNewLines(appSourceContent));

            string urlAppSource = Path.Join(Constants.HomeDir, $"URL-{protocol}.txt");
            string urlAppSourceContent = $@"on open location this_URL
    set the clipboard to this_URL
    tell application ""{appPath}"" to activate
end open location";
            File.WriteAllLines(urlAppSource, SplitNewLines(urlAppSourceContent));

            string scriptContent = $@"#!/bin/sh
osacompile -o '{appPath}'  '{appSource}' 
osacompile -o '{urlAppPath}'  '{urlAppSource}'";
            File.WriteAllLines(scriptFilePath, SplitNewLines(scriptContent));

            Process chmod = new()
            {
                StartInfo =
                {
                    FileName = "chmod",
                    ArgumentList = {"+x", scriptFilePath },
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                }
            };
            chmod.Start();
            chmod.WaitForExit();
            string chmodError = chmod.StandardError.ReadToEnd();
            if (chmod.ExitCode != 0 || !string.IsNullOrWhiteSpace(chmodError))
            {
                throw new Exception(chmodError);
            }

            Process scriptProcess = new()
            {
                StartInfo =
                {
                    FileName = scriptFilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
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

            PlistMutator(urlAppPath, protocol);

            Process openProcess = new()
            {
                StartInfo =
                {
                    FileName = "open",
                    ArgumentList = {urlAppPath },
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                }
            };
            openProcess.Start();
            openProcess.WaitForExit();
            string openError = openProcess.StandardError.ReadToEnd();
            if (openProcess.ExitCode != 0 || !string.IsNullOrWhiteSpace(openError))
            {
                throw new Exception(openError);
            }
            File.Delete(scriptFilePath);
            File.Delete(urlAppSource);
            File.Delete(appSource);
        }
    }
}
