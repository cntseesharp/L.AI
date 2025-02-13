using L_AI.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers
{
    internal class GenerationProviderBase
    {
        protected string BaseUrl => GeneralOptions.Instance.ApiBaseEndpoint.TrimEnd('/');
        protected static HttpClient _httpClient = new HttpClient();

        protected string BaseUrlFromSettings(GeneralOptions settingsOverride = null) => settingsOverride == null ? BaseUrl : settingsOverride.ApiBaseEndpoint.TrimEnd('/');

        protected HttpContent ToContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }
    }
}
