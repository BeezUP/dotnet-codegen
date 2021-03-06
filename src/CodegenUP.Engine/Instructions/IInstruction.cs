﻿using System;
using System.Threading.Tasks;

namespace CodegenUP.Instructions
{
    public interface IInstruction : IDisposable
    {
        string Command { get; }

        Task InitializeInstructionAsync(IProcessorContext context, params string[] parameters);

        Task HandleLineAsync(string line);
    }
}
