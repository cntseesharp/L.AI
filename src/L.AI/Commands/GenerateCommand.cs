using L_AI.VisualStudioSuggestions;
using L_AI.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;
using L_AI.CodeGeneration;
using LocalLlmAutocomplete.CodeGeneration;
using Community.VisualStudio.Toolkit;

namespace L_AI.Commands
{
    [Command("620889b3-cd41-43ff-8786-020fd2c48b28", 4131)]
    internal sealed class GenerateCommand : BaseCommand<GenerateCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await GenerationHelper.StartNewAndShowAsync();
        }
    }
}
