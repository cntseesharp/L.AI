using L_AI.Options;
using L_AI.TextGeneration.Providers.LlamaCpp.Models;
using L_AI.TextGeneration.WebApi;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers
{
    internal class LLamaCppProvider : GenerationProviderBase, IGenerationProvider
    {
        private const string GenerationEndpoint = "/api/v1/generate";
        private const string ModelEndpoint = "/api/v1/model";
        private const string AbortEndpoint = "/api/extra/abort";
        private const string TokenCountEndpoint = "/api/extra/tokencount";
        private const string ContextLengthEndpoint = "/api/extra/true_max_context_length";

        public async Task<ApiResult<int>> CalculateTokens(string prompt, CancellationToken cancellationToken = default)
        {
            var url = BaseUrl + TokenCountEndpoint;

            var data = new LlamaCppTokenCountRequest { Prompt = prompt };
            try
            {
                var response = await _httpClient.PostAsync(url, ToContent(data), cancellationToken);
                var model = JsonConvert.DeserializeObject<LlamaCppTokenCountResponse>(await response.Content.ReadAsStringAsync());
                return ApiResult<int>.Result(model.Value);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Error($"[GenerationSource] Exception while obtaining token count = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        public async Task<ApiResult<bool>> CancelGeneration(CancellationToken cancellationToken = default)
        {
            var abortEndpoint = BaseUrl + AbortEndpoint;
            try
            {
                var result = await _httpClient.PostAsync(abortEndpoint, new StringContent(string.Empty), cancellationToken);
                var resultString = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<LlamaCppBoolResponse>(resultString);

                return new ApiResult<bool>
                {
                    IsSuccessful = data.Success,
                    Data = data.Success,
                    ErrorMessage = null,
                };
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Error($"[GenerationSource] Exception while cancelling generation = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        public async Task<ApiResult<string>> GenerateRequest(GenerationRequestModel parameters, CancellationToken cancellationToken, GeneralOptions optionsOverride = null)
        {
            var settings = optionsOverride ?? GeneralOptions.Instance;
            var model = new LlamaCppGenerationRequest(parameters, settings);

            var url = BaseUrlFromSettings(optionsOverride) + GenerationEndpoint;

            try
            {
                var result = await _httpClient.PostAsync(url, ToContent(model), cancellationToken);

                var responseString = await result.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                    return ApiResult<string>.Error($"[GenerationSource] Empty api response");

                var response = JsonConvert.DeserializeObject<LlamaCppGenerationResponse>(responseString);
                if (response?.Results == null || response.Results.Count == 0)
                    return ApiResult<string>.Error($"[GenerationSource] Unexpected api response = {responseString}");

                return ApiResult<string>.Result(response.Results[0].Text);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Error($"[GenerationSource] Exception while generating autocomplete = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        public async Task<ApiResult<string>> GetModel()
        {
            var url = BaseUrl + ModelEndpoint;

            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(3));
                    var response = await _httpClient.GetAsync(url, cts.Token);
                    var model = JsonConvert.DeserializeObject<LlamaCppStringResponse>(await response.Content.ReadAsStringAsync());
                    return ApiResult<string>.Result(model.Result);
                }

            }
            catch (Exception ex)
            {
                return ApiResult<string>.Error($"[GenerationSource] Exception while obtaining model info = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        public async Task<ApiResult<int>> GetMaxLength()
        {
            var url = BaseUrl + ContextLengthEndpoint;

            try
            {

                using (var cts = new CancellationTokenSource())
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(3));
                    var response = await _httpClient.GetAsync(url, cts.Token);
                    var model = JsonConvert.DeserializeObject<LlamaCppValueResponse>(await response.Content.ReadAsStringAsync());
                    return ApiResult<int>.Result(model.Value);
                }
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Error($"[GenerationSource] Exception while context length = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }
    }
}
