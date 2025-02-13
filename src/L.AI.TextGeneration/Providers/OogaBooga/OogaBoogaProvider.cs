using L_AI.Options;
using L_AI.TextGeneration.Tokenizers;
using L_AI.TextGeneration.WebApi;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers.OogaBooga
{
    internal class OogaBoogaProvider : GenerationProviderBase, IGenerationProvider
    {
        private const string GenerationEndpoint = "/v1/completions";

        public async Task<ApiResult<int>> CalculateTokens(string prompt, CancellationToken cancellationToken = default)
        {
            var tokens = LlamaTokenizer.CalculateTokenCount(prompt);
            return ApiResult<int>.Result(tokens);
        }

        public async Task<ApiResult<bool>> CancelGeneration(CancellationToken cancellationToken = default)
        {
            return ApiResult<bool>.Result(true);
        }

        public async Task<ApiResult<string>> GenerateRequest(GenerationRequestModel parameters, CancellationToken cancellationToken, GeneralOptions optionsOverride = null)
        {
            var settings = GeneralOptions.Instance;
            var model = new OogaBoogaGenerationRequest(parameters, settings);

            var url = BaseUrlFromSettings(optionsOverride) + GenerationEndpoint;

            try
            {
                var result = await _httpClient.PostAsync(url, ToContent(model), cancellationToken);

                var responseString = await result.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                    return ApiResult<string>.Error($"[GenerationSource] Empty api response");

                var response = JsonConvert.DeserializeObject<OogaBoogaGenerationResponse>(responseString);
                var output = response?.Choices.FirstOrDefault()?.Text;

                if (output == null)
                    return ApiResult<string>.Error($"[GenerationSource] Unexpected api response = {responseString}");

                return ApiResult<string>.Result(output);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Error($"[GenerationSource] Exception while generating autocomplete = {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        public async Task<ApiResult<int>> GetMaxLength()
        {
            return ApiResult<int>.Error("Not supported");
        }

        public async Task<ApiResult<string>> GetModel()
        {
            var url = BaseUrl + GenerationEndpoint;
            var model = new OogaBoogaGenerationRequest(new GenerationRequestModel { ContextLength = 256, Prompt = " ", Stop = new[] { "" } }, GeneralOptions.Instance);
            var result = await _httpClient.PostAsync(url, ToContent(model), default);
            var responseString = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseString))
                return ApiResult<string>.Error($"[GenerationSource] Empty api response");

            var response = JsonConvert.DeserializeObject<OogaBoogaGenerationResponse>(responseString);
            var output = response?.Choices.FirstOrDefault()?.Text;

            if (output == null)
                return ApiResult<string>.Error($"[GenerationSource] Unexpected api response = {responseString}");

            return ApiResult<string>.Result(response.Model);
        }
    }
}
