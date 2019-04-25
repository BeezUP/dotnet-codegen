using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.CodeGen.Tests
{
    public class PackAndUseTests : IDisposable
    {
        const string PACKAGE_NAME = "Dotnet.CodeGen";
        const string COMMAND_NAME = "dotnet-codegen";
        private readonly ITestOutputHelper _output;
        private readonly string _tmpOutput;
        private readonly string _solutionFolder;

        public PackAndUseTests(ITestOutputHelper output)
        {
            _output = output;
            _solutionFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../.."));
            _tmpOutput = Path.Combine(_solutionFolder, "_testsnugetpakage." + Path.GetRandomFileName());
        }

        public void Dispose()
        {
            if (Directory.Exists(_tmpOutput))
                Directory.Delete(_tmpOutput, true);

            UninstallTool();
        }

        [Fact]
        public void Should_pack_install_unistall()
        {
            ListTools();
            UninstallTool();
            PackTool();
            InstallTool();
            ListTools();
        }

        [IgnoreOnLinuxFact]
        public void Should_pack_install_tool_execute()
        {
            UninstallTool();
            PackTool();
            InstallTool();

            var outputPath = Path.GetRandomFileName();

            try
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = COMMAND_NAME,
                    Arguments = $"\"./_samples/test1/schema.json\" \"./_samples/test1/template\" \"{outputPath}\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                }))
                {
                    process.WaitForExit();
                    _output.WriteLine(process.StandardError.ReadToEnd());

                    if (process.ExitCode != 0)
                        throw new Exception($"Failed : '{process.StandardError.ReadToEnd()}'");
                }
            }
            finally
            {
                if (Directory.Exists(outputPath))
                    Directory.Delete(outputPath, true);
            }
        }


        private void InstallTool()
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool install -g {PACKAGE_NAME} --add-source {_tmpOutput}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _solutionFolder
            }))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception($"Failed : '{process.StandardError.ReadToEnd()}'");
            }
        }

        private void ListTools()
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool list -g",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            }))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception($"Failed : '{process.StandardError.ReadToEnd()}'");

                _output.WriteLine($"Tools list : '\n{process.StandardOutput.ReadToEnd()}\n'");
            }
        }

        private void UninstallTool()
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool uninstall -g {PACKAGE_NAME}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _solutionFolder
            }))
            {
                process.WaitForExit();
                // Errors are ignored
            }
        }

        private void PackTool()
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"pack ./{PACKAGE_NAME}/{PACKAGE_NAME}.csproj --configuration Release --output {_tmpOutput}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _solutionFolder
            }))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    _output.WriteLine($"{process.StandardOutput.ReadToEnd()}");
                    throw new Exception($"Failed : '{process.StandardError.ReadToEnd()}'");
                }
            }
        }
    }
}
