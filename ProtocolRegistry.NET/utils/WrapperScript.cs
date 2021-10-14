using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingUnion
{
    public partial class ProtocolRegistry
    {
        const string BatchScriptContent = @"@echo off
";

        private static string SubtituteWindowsCommand(string command)
        {
            string identifier;
            string result = command;
            identifier = "\"$_URL_\"";
            result = string.Join($"\"{Constants.UrlArgmentInScript}\"", result.Split(identifier));
            identifier = "'$_URL_'";
            result = string.Join($"\"{Constants.UrlArgmentInScript}\"", result.Split(identifier));
            return subtituteCommand(result, Constants.UrlArgment);
        }

        private static string GetWrapperScriptContent(string command)
        {
            if (Platform == Platforms.Windows)
            {
                return $"{BatchScriptContent}{SubtituteWindowsCommand(command)}";
            }
            else
            {
                return
$@"#!/usr/bin/env bash
_URL_=$1
{command}";
            }
        }

        private static string SaveWrapperScript(string protocol, string contents)
        {
            string wrapperScriptPath = Path.Join(
                Constants.HomeDir,
                $@"./{protocol}Wrapper.{(Platform == Platforms.Windows ? "bat" : "sh")}"
                );
            // Caution: batch cannot recognize UTF8
            File.WriteAllLines(wrapperScriptPath, contents.Split(new []{ "\r\n" , "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries), new UTF8Encoding(false));
            return wrapperScriptPath;
        }

        private static string HandleWrapperScript(string protocol, string command)
        {
            var contents = GetWrapperScriptContent(command);
            string scriptPath = SaveWrapperScript(protocol, contents);
            if (Platform != Platforms.Windows)
            {
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
                return $"'{scriptPath}' '{Constants.UrlArgment}'";
            } 
            else
            {
                return $"\"{scriptPath}\" \"{Constants.UrlArgment}\"";
            }
            
        }
    }
}
