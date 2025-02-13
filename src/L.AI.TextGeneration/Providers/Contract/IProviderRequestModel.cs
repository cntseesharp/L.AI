using L_AI.TextGeneration.WebApi;

namespace L_AI.TextGeneration.Providers.Contract
{
    public interface IProviderRequestModel<T>
    {
        T Convert(GenerationRequestModel generationRequestModel);
    }
}
