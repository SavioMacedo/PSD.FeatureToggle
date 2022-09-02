using Newtonsoft.Json;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Attributes;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Models;

namespace PSD.FeatureToggle.Features.CustomFilter
{
    [FeatureFilterAlias(Alias)]
    public class EscolaFeatureFilter : IFeatureToggleFilter
    {
        private const string Alias = "escolas";
        private readonly ILogger _logger;

        public EscolaFeatureFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EscolaFeatureFilter>();
        }

        public Task<bool> EvaluateAsync(FeatureToggleDefinition featureToggleDefinition, IFeatureToggleContext context, CancellationToken cancellationToken = default)
        {
            int escolaId = (context as EscolaFeatureContext).EscolaId;

            int[] escolasPermitidas = JsonConvert.DeserializeObject<int[]>(featureToggleDefinition.Parameters[Alias]);

            return Task.FromResult(escolasPermitidas.Contains(escolaId));
        }
    }
}
