using System.ComponentModel;

namespace L_AI.Options
{
    public class GeneralOptions : OptionsBaseModel<GeneralOptions>
    {
        //
        //      API
        //

        [Category("API")]
        [DisplayName("API Provider")]
        [Description("Generation provider API")]
        [DefaultValue(GenerationProviderType.Kobold)]
        public GenerationProviderType ApiProvider { get; set; } = GenerationProviderType.Kobold;

        [Category("API")]
        [DisplayName("API Endpoint")]
        [Description("URL of your text generation API")]
        public string ApiBaseEndpoint { get; set; } = "http://127.0.0.1:5001";

        //
        //      FIM
        //

        [Category("Fill-in-the-Middle")]
        [DisplayName("Text begin token")]
        [Description("Will be put at the beginning of the prompt (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁begin｜>")]
        public string StartToken { get; set; } = "<｜fim▁begin｜>";

        [Category("Fill-in-the-Middle")]
        [DisplayName("Text autocomplete token")]
        [Description("Mark point of autocompletion (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁hole｜>")]
        public string HoleToken { get; set; } = "<｜fim▁hole｜>";

        [Category("Fill-in-the-Middle")]
        [DisplayName("Text end token")]
        [Description("Will be put at the end of the prompt (Default for deepseek-coder)")]
        [DefaultValue("<｜fim▁end｜>")]
        public string EndToken { get; set; } = "<｜fim▁end｜>";

        //
        //      Generation
        //

        [Category("Generation Parameters")]
        [DisplayName("Temperature")]
        [Description("Temperature Sampling. \"Creativity\" or \"randomness\" of generation. Temperature affects the probability distribution over the possible tokens at each step of the generation process.\nKISS: Higher value = more randomness.")]
        [DefaultValue(1f)]
        public float Temperature { get; set; } = 1f;

        [Category("Generation Parameters")]
        [DisplayName("Top P")]
        [Description("Nucleus Sampling. Instead of considering all possible tokens, considers only a subset of tokens whose cumulative probability mass adds up to the stated threshold. \nKISS: LLM will only pick from the top % of its 'best guesses', not 'best next token'.")]
        [DefaultValue(0.3f)]
        public float TopP { get; set; } = 0.3f;

        [Category("Generation Parameters")]
        [DisplayName("Max response length")]
        [Description("If Singleline is disable - here you can adjust response length in tokens")]
        [DefaultValue(150)]
        public int MaxLength { get; set; } = 150;

        [Category("Generation Parameters")]
        [DisplayName("Stop sequence")]
        [Description("Additional stop sequences to abort generation (for example ).")]
        [DefaultValue(new[] { "using System", "//Q:" })]
        public string[] StopSequence { get; set; } = new[] { "using System", "//Q:" };

        [Category("Generation Parameters")]
        [DisplayName("Context length")]
        [Description("Only used if impossible to get context length from the API.")]
        public int ContextLength { get; set; } = 4096;

        //
        //      Suggestions Settings
        //

        [Category("Suggestions Settings")]
        [DisplayName("Automatic suggestions enabled")]
        [Description("Choose how to treat IntelliCode-style suggestion. When disabled - will only suggest an autocomplete if prompted via hotkey or in from Tools commands.")]
        [DefaultValue(true)]
        public bool AutocompleteEnabled { get; set; } = true;

        [Category("Suggestions Settings")]
        [DisplayName("Delay before making API request")]
        [Description("When set too low it can overload API and make a long queue of generation request. Handle with care. Min value of 500ms will be forced.")]
        [DefaultValue(800)]
        public int TimeBeforeMakingApiRequest { get; set; } = 800;

        [Category("Suggestions Settings")]
        [DisplayName("Use analyzer")]
        [Description("ONLY APPLIES TO COMMANDS (Alt+A, Alt+Z). Uses VS analyzer to include code from used classes/enums/interfaces/records. Will not exceed context limit. \nHandle with care, can/will make inference take too long.")]
        [DefaultValue(true)]
        public bool UseAnalyzer { get; set; } = true;

        public GeneralOptions CreateCopy()
        {
            var source = this;
            return new GeneralOptions
            {
                ApiProvider = source.ApiProvider,
                ApiBaseEndpoint = source.ApiBaseEndpoint,
                StartToken = source.StartToken,
                EndToken = source.EndToken,
                HoleToken = source.HoleToken,
                Temperature = source.Temperature,
                TopP = source.TopP,
                MaxLength = source.MaxLength,
                StopSequence = source.StopSequence,
                AutocompleteEnabled = source.AutocompleteEnabled,
                TimeBeforeMakingApiRequest = source.TimeBeforeMakingApiRequest,
                UseAnalyzer = source.UseAnalyzer,
            };
        }

        public void LoadFrom(GeneralOptions source)
        {
            ApiProvider = source.ApiProvider;
            ApiBaseEndpoint = source.ApiBaseEndpoint;
            StartToken = source.StartToken;
            EndToken = source.EndToken;
            HoleToken = source.HoleToken;
            Temperature = source.Temperature;
            TopP = source.TopP;
            MaxLength = source.MaxLength;
            StopSequence = source.StopSequence;
            AutocompleteEnabled = source.AutocompleteEnabled;
            TimeBeforeMakingApiRequest = source.TimeBeforeMakingApiRequest;
            UseAnalyzer = source.UseAnalyzer;
        }
    }
}
