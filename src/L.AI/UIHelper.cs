using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;
using OutputWindowPane = Community.VisualStudio.Toolkit.OutputWindowPane;

namespace L_AI
{
    public static class UIHelper
    {
        private static AsyncLazy<OutputWindowPane> _outputWindowPane = new AsyncLazy<OutputWindowPane>(async () => await VS.Windows.CreateOutputWindowPaneAsync("L.AI"), ThreadHelper.JoinableTaskFactory);
        public static OutputWindowPane WindowPane => _outputWindowPane.GetValue();

        public static async Task<TextDocument> GetActiveDocumentAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            DTE dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            if (dte == null || dte.ActiveDocument == null)
            {
                return null;
            }

            TextDocument textDoc = dte.ActiveDocument.Object("TextDocument") as TextDocument;

            return textDoc;
        }

        public static async Task<IWpfTextView> GetTextViewAsync(AsyncPackage package)
        {
            IVsTextManager textManager = (IVsTextManager)await package.GetServiceAsync(typeof(SVsTextManager));
            textManager.GetActiveView(1, null, out IVsTextView textView);
            var wpfTextView = ToWpfTextView(textView);

            if (!wpfTextView.Roles.Contains(PredefinedTextViewRoles.Document))
            {
                return null;
            }

            return wpfTextView;
        }

        private static IWpfTextView ToWpfTextView(IVsTextView vTextView)
        {
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
            {
                return null;
            }

            Guid guidWpfTextViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidWpfTextViewHost, out object host);
            IWpfTextViewHost textViewHost = host as IWpfTextViewHost;
            return textViewHost?.TextView;
        }
    }
}
