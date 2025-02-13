using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.Options.Base
{
    [ComVisible(true)]
    public class BaseOptionPage<T> : DialogPage where T : OptionsBaseModel<T>, new()
    {
        private OptionsBaseModel<T> _model;

        public BaseOptionPage()
        {
            _model = OptionsBaseModel<T>.Instance;
        }

        public override object AutomationObject => _model;

        public override void LoadSettingsFromStorage()
        {
            //_model.Load();
        }

        public override void SaveSettingsToStorage()
        {
            _model.Save();
        }
    }
}
