using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dotnet.CodeGen.CodeGen.Instructions;
using Dotnet.CodeGen.Misc;

namespace Dotnet.CodeGen.CodeGen
{
    public class FilesProcessor : IDisposable
    {
        public FilesProcessor(IProcessorContext context, params IInstruction[] instructions)
        {
            _context = context;
            _instructions = instructions.ToCaseInsensitiveDictionary(i => i.Command, i => i);
        }

        private readonly IProcessorContext _context;

        private readonly CaseInsensitiveDictionary<IInstruction> _instructions;
        private readonly CaseInsensitiveDictionary<IInstruction> _activeInstructions = new CaseInsensitiveDictionary<IInstruction>();

        public async Task RunAsync()
        {
            var lineWithCommand = new Regex($"^{_context.CommandPrefix} (\\S+)(?: (\\S+))*$");

            using (var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(_context.InputFile))))
            {
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    if (line == null) continue;

                    var commandCheck = lineWithCommand.Match(line);

                    if (commandCheck.Success)
                    {
                        var groupEnumerator = commandCheck.Groups.GetEnumerator();
                        groupEnumerator.MoveNext();
                        groupEnumerator.MoveNext();

                        var group = groupEnumerator.Current as Group;
                        if (group == null)
                            continue;

                        var command = group.Value;
                        if (command == null)
                            continue;

                        if (!_instructions.TryGetValue(command, out var instruction))
                            continue;

                        var parameters = new List<string>();
                        while (groupEnumerator.MoveNext())
                        {
                            var value = (groupEnumerator.Current as Group)?.Value;
                            if (!string.IsNullOrWhiteSpace(value))
                                parameters.Add(value ?? throw new InvalidOperationException());
                        }

                        if (!_activeInstructions.ContainsKey(instruction.Command))
                            _activeInstructions.Add(instruction.Command, instruction);

                        await instruction.InitializeInstructionAsync(_context, parameters.ToArray());
                    }
                    else
                    {
                        foreach (var inst in _activeInstructions.Values)
                        {
                            await inst.HandleLineAsync(line);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var ins in _instructions.Values)
                ins.Dispose();
        }
    }
}
