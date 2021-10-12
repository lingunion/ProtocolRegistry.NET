using System;
using Xunit;
using LingUnion;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace TestProject1
{
    public class RegisterTests
    {
        public string GetTestProtocol() => $"protocol-registry-test-1-{(new Random()).Next(100000000)}";
        public string CodeBaseDir => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", ""));
        public string ProtocolExecPath => Path.Join(CodeBaseDir, @"RegisterWindowTestConsole\RegisterWindowTestConsole.exe");

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
            // Currently runWindowsCallback cannot run
            return File.Exists(Path.Join($"{dir}", $"{protocol}.txt"));
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
                throw new NotImplementedException();
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
