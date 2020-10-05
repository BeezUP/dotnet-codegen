using System;
using System.Threading.Tasks;

namespace CodegenUP.CodeGen.Instructions
{
    public abstract class BaseInstruction : IInstruction
    {
        protected BaseInstruction(string command)
        {
            Command = command;
        }

        public string Command { get; }

        public abstract Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters);

        public abstract Task HandleLineAsync(string line);

        public abstract void Dispose();
    }
}
