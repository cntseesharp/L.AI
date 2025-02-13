using L_AI.UI.ToolWindows.ViewModels;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace L_AI.UI
{
    public partial class FirstSetup : DialogWindow
    {
        public FirstSetup()
        {
            DataContext = new FirstSetupViewModel();
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Open the hyperlink in the default web browser
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Skip_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            closeDialog.Visibility = System.Windows.Visibility.Visible;
        }

        private void Continue_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            closeDialog.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ConfirmClose_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {

            closeDialog.Visibility = System.Windows.Visibility.Hidden;
            this.Close();
        }
    }
}
