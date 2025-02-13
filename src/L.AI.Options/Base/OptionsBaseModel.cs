using Microsoft;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace L_AI.Options
{
    public abstract class OptionsBaseModel<T> where T : OptionsBaseModel<T>, new()
    {
        private static AsyncLazy<ShellSettingsManager> _settingsManager = new AsyncLazy<ShellSettingsManager>(GetSettingsManagerAsync, ThreadHelper.JoinableTaskFactory);
        private static AsyncLazy<T> _instanceLoader = new AsyncLazy<T>(InitializeFromStorageAsync<T>, ThreadHelper.JoinableTaskFactory);

        private static string Store = "L.AI.Store";

        protected OptionsBaseModel()
        { }

        private static T _instance;
        public static T Instance => _instance ?? (_instance = _instanceLoader.GetValue());

        protected static string CollectionName => typeof(T).FullName;

        public static async Task<T> InitializeFromStorageAsync<T>() where T : OptionsBaseModel<T>, new()
        {
            ShellSettingsManager manager = await _settingsManager.GetValueAsync();
            SettingsStore settingsStore = manager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            if (!settingsStore.CollectionExists(CollectionName))
            {
                return new T();
            }

            try
            {
                if (!settingsStore.PropertyExists(CollectionName, Store))
                    return new T();

                string serializedProp = settingsStore.GetString(CollectionName, Store);
                var instance = JsonConvert.DeserializeObject<T>(serializedProp);
                return instance;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return new T();
            }
        }

        public virtual void Save()
        {
            ThreadHelper.JoinableTaskFactory.Run(SaveAsync);
        }

        /// <summary>
        /// Saves the properties to the registry asyncronously.
        /// </summary>
        public virtual async Task SaveAsync()
        {
            ShellSettingsManager manager = await _settingsManager.GetValueAsync();
            WritableSettingsStore settingsStore = manager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!settingsStore.CollectionExists(CollectionName))
            {
                settingsStore.CreateCollection(CollectionName);
            }

            var asString = JsonConvert.SerializeObject(Instance);
            settingsStore.SetString(CollectionName, Store, asString);

            //T liveModel = await LoadAsync();

            //if (this != liveModel)
            //{
            //    await liveModel.LoadAsync();
            //}
        }

        private static async Task<ShellSettingsManager> GetSettingsManagerAsync()
        {
#pragma warning disable VSTHRD010 
            // False-positive in Threading Analyzers. Bug tracked here https://github.com/Microsoft/vs-threading/issues/230
            var svc = await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(SVsSettingsManager)) as IVsSettingsManager;
#pragma warning restore VSTHRD010 

            Assumes.Present(svc);

            return new ShellSettingsManager(svc);
        }
    }
}
