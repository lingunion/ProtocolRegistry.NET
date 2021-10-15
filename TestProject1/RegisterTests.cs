using System;
using Xunit;
using LingUnion;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

namespace TestProject1
{
    public class RegisterTests
    {
        public string GetTestProtocol()
        {
            var randChars = (new int[(new Random()).Next(50)]).Select(num => (char)('a' + (new Random().Next(26))));
            return $"protocolregistrytest{(string.Join("", randChars))}";
        }
        public string CodeBaseDir => Directory.GetCurrentDirectory();
        public string ProtocolExecPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Join(CodeBaseDir, @"RegisterWindowTestConsole", @"RegisterWindowTestConsole.exe");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path.Join(CodeBaseDir, @"RegisterWindowTestConsole" ,@"RegisterWindowTestConsoleLinux");
                }
                else
                {
                    return Path.Join(CodeBaseDir, @"RegisterWindowTestConsole", @"RegisterWindowTestConsoleOSX");
                }
            }
        }

        public bool CheckResultTextWindows(string protocol, string dir) 
        {
            Process runWindowsCallback = new Process()
            {
                StartInfo = {
                        FileName = "explorer.exe",
                        ArgumentList = {$"{protocol}://{dir}"},
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    },
            };
            runWindowsCallback.Start();
            runWindowsCallback.WaitForExit();
            System.Threading.Thread.Sleep(1000);
            return File.Exists(Path.Join($"{dir}", $"{protocol}.txt"));
        }

        public bool CheckResultTextLinux(string protocol, string dir)
        {
            string scriptPath = Path.Join(CodeBaseDir, "check.sh");
            File.WriteAllText(scriptPath, $@"xdg-open {protocol}://{dir}");
            Process chmod = new Process()
            {
                StartInfo = {
                    FileName = "chmod",
                    ArgumentList =
                    {
                        "+x",
                        scriptPath,
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
                    ArgumentList = { "-c", $"'{scriptPath}'" },
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
            File.Delete(scriptPath);
            
            System.Threading.Thread.Sleep(1000);
            return File.Exists(Path.Join($"{dir}", $"{protocol}.txt"));
        }

        public bool CheckResultTextOSX(string protocol, string dir)
        {
            Process openProcess = new()
            {
                StartInfo =
                {
                    FileName = "open",
                    ArgumentList = {$"{protocol}://{dir}"},
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
            System.Threading.Thread.Sleep(5000);
            return File.Exists(Path.Join(dir, $"{protocol}.txt"));
        }

        [Fact]
        public void DefaultRegisterTest()
        {
            string testProtocol = GetTestProtocol();
            Assert.False(ProtocolRegistry.CheckIfExists(testProtocol));
            string folder = CodeBaseDir;
            string protocolExecPath = ProtocolExecPath;
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_");
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Currently cannot run without terminal due to bug
                // Assert.True(CheckResultTextWindows(testProtocol, folder));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CheckResultTextLinux(testProtocol, folder);
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(CheckResultTextOSX(testProtocol, folder));
            } 
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Fact]
        public void ScriptRegisterTest()
        {
            string testProtocol = GetTestProtocol();
            Assert.False(ProtocolRegistry.CheckIfExists(testProtocol));
            string folder = CodeBaseDir;
            string protocolExecPath = ProtocolExecPath;
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_", script: true);
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Currently cannot run without terminal due to bug
                // Assert.True(CheckResultTextWindows(testProtocol, folder));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CheckResultTextLinux(testProtocol, folder);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(CheckResultTextOSX(testProtocol, folder));
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Fact]
        public void TerminalRegisterTest()
        {
            string testProtocol = GetTestProtocol();
            Assert.False(ProtocolRegistry.CheckIfExists(testProtocol));
            string folder = CodeBaseDir;
            string protocolExecPath = ProtocolExecPath;
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_", terminal: true);
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(CheckResultTextWindows(testProtocol, folder));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CheckResultTextLinux(testProtocol, folder);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(CheckResultTextOSX(testProtocol, folder));
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Fact]
        public void ScriptTerminalRegisterTest()
        {
            string testProtocol = GetTestProtocol();
            Assert.False(ProtocolRegistry.CheckIfExists(testProtocol));
            string folder = CodeBaseDir;
            string protocolExecPath = ProtocolExecPath;
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_", script:true, terminal: true);
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(CheckResultTextWindows(testProtocol, folder));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CheckResultTextLinux(testProtocol, folder);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(CheckResultTextOSX(testProtocol, folder));
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Fact]
        public void DefaultRegisterTestOverwrite()
        {
            string testProtocol = GetTestProtocol();
            Assert.False(ProtocolRegistry.CheckIfExists(testProtocol));
            string folder = CodeBaseDir;
            string protocolExecPath = ProtocolExecPath;
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_");
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            ProtocolRegistry.Register(testProtocol, $"{protocolExecPath} $_URL_", overwrite: true);
            Assert.True(ProtocolRegistry.CheckIfExists(testProtocol));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Currently cannot run without terminal due to bug
                // Assert.True(CheckResultTextWindows(testProtocol, folder));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CheckResultTextLinux(testProtocol, folder);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.True(CheckResultTextOSX(testProtocol, folder));
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
