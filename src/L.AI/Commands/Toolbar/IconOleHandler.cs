using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.Commands.Toolbar
{
    public class IconOleHandler
    {
        public static async Task RegisterIconButton()
        {
            var commandService = (await LAIPackage.Instance.GetServiceAsync(typeof(IMenuCommandService))) as OleMenuCommandService;
            if (commandService != null)
            {
                OleMenuCommand greenIcon = null;

                greenIcon = new OleMenuCommand((e, id) =>
                {

                },
                new CommandID(new Guid("11f5a130-ab1e-4b1c-a10c-3c6d1a0e19a1"), 0x4000));
                commandService.AddCommand(greenIcon);
            }
        }
    }
}
