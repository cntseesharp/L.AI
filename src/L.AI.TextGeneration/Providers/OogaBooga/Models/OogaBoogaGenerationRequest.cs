using L_AI.Options;
using L_AI.TextGeneration.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.TextGeneration.Providers.OogaBooga
{
    internal class OogaBoogaGenerationRequest
    {
        [JsonProperty("max_context_length")]
        public int MaxContextLength { get; set; } = 4096;

        [JsonProperty("max_tokens")]
        public int MaxLength { get; set; } = 70;

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("quiet")]
        public bool Quiet { get; set; } = false;

        [JsonProperty("rep_pen")]
        public int RepPen { get; set; } = 1;

        [JsonProperty("rep_pen_range")]
        public int RepPenRange { get; set; } = 256;

        [JsonProperty("rep_pen_slope")]
        public int RepPenSlope { get; set; } = 1;

        [JsonProperty("temperature")]
        public double Temperature { get; set; } = 1;

        [JsonProperty("tfs")]
        public int Tfs { get; set; } = 1;

        [JsonProperty("top_a")]
        public int TopA { get; set; } = 0;

        [JsonProperty("top_k")]
        public int TopK { get; set; } = 100;

        [JsonProperty("top_p")]
        public double TopP { get; set; } = 0.3;

        [JsonProperty("typical")]
        public int Typical { get; set; } = 1;

        [JsonProperty("stop_sequence")]
        public string[] StopSequence => Stop;

        [JsonProperty("stop")]
        public string[] Stop { get; set; }

        [JsonProperty("stopping_strings")]
        public string[] StoppingStrings => Stop;

        public OogaBoogaGenerationRequest(GenerationRequestModel generationRequestModel, GeneralOptions options)
        {
            MaxContextLength = generationRequestModel.ContextLength;
            Prompt = generationRequestModel.Prompt;
            Stop = generationRequestModel.Stop;
            MaxLength = options.MaxLength;
            TopP = options.TopP;
            Temperature = options.Temperature;
        }
    }
}
