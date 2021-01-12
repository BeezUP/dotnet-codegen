using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace CodegenUP.Tests
{
    public class PackAndUseTests : IDisposable
    {
        const string PACKAGE_NAME = "CodegenUP";
        const string COMMAND_NAME = "codegenup";
        private readonly ITestOutputHelper _output;
        private readonly string _tmpOutput;
        private readonly string _solutionFolder;

        public PackAndUseTests(ITestOutputHelper output)
        {
            _output = output;
            _solutionFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../.."));
            _tmpOutput = Path.Combine(_solutionFolder, "_testsnugetpakage." + Path.GetRandomFileName());
        }

        public void Dispose()
        {
            if (Directory.Exists(_tmpOutput))
                Directory.Delete(_tmpOutput, true);

            UninstallTool(_output, _solutionFolder, _tmpOutput);
        }

        [IgnoreOnLinuxFact]
        public void Should_pack_install()
        {
            ListTools(_output, _solutionFolder, _tmpOutput);
            UninstallTool(_output, _solutionFolder, _tmpOutput);
            PackTool(_output, _solutionFolder, _tmpOutput);
            InstallTool(_output, _solutionFolder, _tmpOutput);
            ListTools(_output, _solutionFolder, _tmpOutput);
        }

        [IgnoreOnLinuxFact]
        public void Should_pack_install_tool_execute()
        {
            UninstallTool(_output, _solutionFolder, _tmpOutput);
            PackTool(_output, _solutionFolder, _tmpOutput);
            InstallTool(_output, _solutionFolder, _tmpOutput);

            var outputPath = Path.GetRandomFileName();

            try
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = COMMAND_NAME,
                    Arguments = $"-s \"./_samples/test1/schema.json\" -o \"{outputPath}\" -t \"./_samples/test1/template\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                }))
                {
                    process!.WaitForExit();
                    _output.WriteLine(process.StandardOutput.ReadToEnd());
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


        internal static void InstallTool(ITestOutputHelper output, string solutionFolder, string folder)
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool install -g {PACKAGE_NAME} --add-source {folder}",
                RedirectStandardError = true,
                //RedirectStandardOutput = true,
                //UseShellExecute = false,
                WorkingDirectory = solutionFolder
            }))
            {
                process!.WaitForExit();
                //Output(output, process);
                OutputIfError(output, process);
            }
        }

        internal static void ListTools(ITestOutputHelper output, string solutionFolder, string folder)
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
                process!.WaitForExit();
                OutputIfError(output, process);
                output.WriteLine($"Tools list : '\n{process.StandardOutput.ReadToEnd()}\n'");
            }
        }

        internal static void UninstallTool(ITestOutputHelper output, string solutionFolder, string folder)
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool uninstall -g {PACKAGE_NAME}",
                RedirectStandardError = true,
                //RedirectStandardOutput = true,
                //UseShellExecute = false,
                WorkingDirectory = solutionFolder
            }))
            {
                process!.WaitForExit();
            }
        }

        internal static void PackTool(ITestOutputHelper output, string solutionFolder, string folder)
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"pack ./src/{PACKAGE_NAME}/{PACKAGE_NAME}.csproj --configuration Release --output {folder}",
                RedirectStandardError = true,
                //RedirectStandardOutput = true,
                //UseShellExecute = false,
                WorkingDirectory = solutionFolder
            }))
            {
                process!.WaitForExit();
                OutputIfError(output, process);
            }
        }

        private static void OutputIfError(ITestOutputHelper output, Process process)
        {
            if (process.ExitCode != 0)
            {
                //Output(output, process);
                throw new Exception($"Failed : '{process.StandardError.ReadToEnd()}'");
            }
        }

        //private static void Output(ITestOutputHelper output, Process process)
        //{
        //    output.WriteLine($"{process.StandardOutput.ReadToEnd()}");
        //}
    }
}
