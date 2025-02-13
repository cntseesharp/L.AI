using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using L_AI.Options;
using L_AI.Options.Base;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;

namespace L_AI.Commands.Toolbar
{
    [Command("11f5a130-ab1e-4b1c-a10c-3c6d1a0e19a1", 0x2000)]
    internal sealed class OpenSettings : BaseCommand<OpenSettings>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            LAIPackage.Instance.ShowOptionPage(typeof(DialogPageProvider.General));
        }
    }
}