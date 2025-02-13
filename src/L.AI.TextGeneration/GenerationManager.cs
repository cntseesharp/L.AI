using L_AI.Options;
using L_AI.TextGeneration.Providers;
using L_AI.TextGeneration.Providers.OogaBooga;
using L_AI.TextGeneration.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.TextGeneration
{
    public static class GenerationManager
    {
        private static readonly Dictionary<GenerationProviderType, IGenerationProvider> _providers = new Dictionary<GenerationProviderType, IGenerationProvider>();
        private static string _lastKnownApi = null;
        private static GenerationProviderType _lastKnownProvider = GenerationProviderType.Kobold;

        private static bool _isBusy;

        private static string _model;
        public static string Model => _model;

        private static int _contextLength;
        public static int ContextLength => _contextLength == 0 ? GeneralOptions.Instance.ContextLength : _contextLength;

        private static bool _isConnected;
        public static bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if(value == _isConnected)
                    return;

                _isConnected = value;
                ConnectivityChanged?.Invoke(null, new ConnectivityEventArgs { IsConnected = value });
            }
        }

        public static event EventHandler<ConnectivityEventArgs> ConnectivityChanged;


        // Contract
        // Tokenizer (either existing one or from API)

        // Internal
        // Auto-detect provider

        // TODO: No trying to generate if connection is bad.
        // TODO: Make miniature requests to get model and status, with 3s TTL

        static GenerationManager()
        {
            _providers.Add(GenerationProviderType.Kobold, new LLamaCppProvider());
            _providers.Add(GenerationProviderType.OogaBooga, new OogaBoogaProvider());

            Task.Run(UpdateConfigAndTestConnection);
        }

        // TODO: How about a better impl of status checks?
        public static async Task<ApiResult<string>> RequestAutocomplete(GenerationRequestModel request, CancellationToken cancellationToken)
        {
            if (!await CheckConnection())
                return ApiResult<string>.Error("[GenerationManager] No connection to API");

            request.ContextLength = ContextLength;

            var provider = GetProvider();
            var res = await provider.GenerateRequest(request, cancellationToken);

            if (!res.IsSuccessful)
                IsConnected = false;

            return res;
        }

        public static async Task<ApiResult<bool>> RequestCancellation()
        {
            if (!await CheckConnection())
                return ApiResult<bool>.Error("[GenerationManager] No connection to API");

            var provider = GetProvider();
            var res = await provider.CancelGeneration();

            if (!res.IsSuccessful)
                IsConnected = false;

            return res;
        }

        public static async Task<ApiResult<int>> RequestTokenCount(string text)
        {
            if (!await CheckConnection())
                return ApiResult<int>.Error("[GenerationManager] No connection to API");

            var provider = GetProvider();
            var res = await provider.CalculateTokens(text);

            if (!res.IsSuccessful)
                IsConnected = false;

            return res;
        }

        public static async Task<bool> TestConnection(GeneralOptions optionsOverride = null)
        {
            var provider = GetProvider();
            var genRequest = await provider.GenerateRequest(new GenerationRequestModel { Prompt = " ", Stop = new[] { " " }, ContextLength = 256 }, default, optionsOverride);
            return genRequest.IsSuccessful;
        }

        private static async Task<bool> CheckConnection()
        {
            var options = GeneralOptions.Instance;

            var settingsChanged = _lastKnownApi != options.ApiBaseEndpoint || _lastKnownProvider != options.ApiProvider;
            if (settingsChanged || !IsConnected)
                await UpdateConfigAndTestConnection();

            if (!IsConnected)
                return false;

            return true;
        }

        public static async Task UpdateConfigAndTestConnection()
        {
            if (_isBusy) return;
#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            _isBusy = true;

            var provider = GetProvider();

            //var model = await provider.GetModel();
            //if (model.IsSuccessful) _model = model.Data;

            var contextLength = await provider.GetMaxLength();
            if (contextLength.IsSuccessful)
            {
                _contextLength = contextLength.Data;
                //GeneralOptions.Instance.ContextLength = contextLength.Data;
                await GeneralOptions.Instance.SaveAsync();
            }

            var hasConnection = await TestConnection();
            _isBusy = false;

#if DEBUG
            stopwatch.Stop();
            System.Diagnostics.Debug.Write($"[L.AI]: UpdateApiConfig, elapsed {stopwatch.ElapsedMilliseconds}ms");
#endif

            if (!hasConnection)
            {
                IsConnected = false;
                return;
            }

            _lastKnownApi = GeneralOptions.Instance.ApiBaseEndpoint;
            _lastKnownProvider = GeneralOptions.Instance.ApiProvider;

            IsConnected = true;
        }

        private static IGenerationProvider GetProvider()
        {
            var options = GeneralOptions.Instance;
            if (!_providers.ContainsKey(options.ApiProvider))
                throw new NotImplementedException();

            return _providers[options.ApiProvider];
        }
    }
}
