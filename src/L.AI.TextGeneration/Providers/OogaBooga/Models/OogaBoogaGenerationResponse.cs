using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers.OogaBooga
{
    internal class OogaBoogaGenerationResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }

        [JsonProperty("usage")]
        public UsageData Usage { get; set; }

        public class Choice
        {
            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("finish_reason")]
            public string FinishReason { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("logprobs")]
            public Logprobs Logprobs { get; set; }
        }

        public class Logprobs
        {
            [JsonProperty("top_logprobs")]
            public List<TopLogprob> TopLogprobs { get; set; }
        }

        public class TopLogprob
        {
        }

        public class UsageData
        {
            [JsonProperty("prompt_tokens")]
            public int PromptTokens { get; set; }

            [JsonProperty("completion_tokens")]
            public int CompletionTokens { get; set; }

            [JsonProperty("total_tokens")]
            public int TotalTokens { get; set; }
        }

    }
}
