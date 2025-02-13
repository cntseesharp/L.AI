using L_AI.CodeGeneration;
using L_AI.Commands;
using L_AI.Commands.Toolbar;
using L_AI.Commands.ToolbarHelper;
using L_AI.Options;
using L_AI.Options.Base;
using L_AI.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace L_AI
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "L.AI", "General", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class LAIPackage : AsyncPackage
    {
        public const string PackageGuidString = "51c54524-845d-44de-bdf2-59e262607835";

        private static string GetLogPath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if DEBUG
            var path = Path.Combine(localAppData, "Microsoft", "VisualStudio", "DEBUG_LAI_Log.log");
#else
            var path = Path.Combine(localAppData, "Microsoft", "VisualStudio", "LAI_Log.log");
#endif
            return path;
        }

        private static ILogger
            _serilog = new LoggerConfiguration()
                .WriteTo.File(GetLogPath(), retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}")
                .Enrich.FromLogContext()
                .CreateLogger();
        public static ILogger Serilog => _serilog;


        public static AsyncPackage Instance { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Instance = this;

            Serilog.Information("[Main] InitializeAsync started");
            await UIHelper.WindowPane.WriteLineAsync("L.AI: Starting;");

            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                Serilog.Error("!!!!! UNHANDLED EXCEPTION IN DISPATCHER: " + e.Exception);
            };

            // Shitty way to init, but I need that subscription to the GenerationManager to update status
            GenerationSource.Instance.Init();

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await IconOleHandler.RegisterIconButton();
                await GenerateCommand.InitializeAsync(this);
                await GenerateSinglelineCommand.InitializeAsync(this);
                await InsertGeneratedCommand.InitializeAsync(this);
                await OpenSettings.InitializeAsync(this);

                //var dialogWindow = new FirstSetup();
                //dialogWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Serilog.Error($"[Main] InitializeAsync failed: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.Write($"[L.AI]: MAIN, elapsed {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
