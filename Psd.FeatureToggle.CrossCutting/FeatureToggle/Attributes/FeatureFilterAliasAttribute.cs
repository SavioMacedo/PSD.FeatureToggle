using System.Diagnostics.CodeAnalysis;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFilterAliasAttribute : Attribute
    {
        public FeatureFilterAliasAttribute(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }
            Alias = alias;
        }
        public string Alias { get; }
    }
}
