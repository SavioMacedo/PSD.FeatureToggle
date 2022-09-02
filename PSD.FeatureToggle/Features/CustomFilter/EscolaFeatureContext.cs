using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;

namespace PSD.FeatureToggle.Features.CustomFilter
{
    public class EscolaFeatureContext: IFeatureToggleContext
    {
        public int EscolaId { get; set; }
    }
}
