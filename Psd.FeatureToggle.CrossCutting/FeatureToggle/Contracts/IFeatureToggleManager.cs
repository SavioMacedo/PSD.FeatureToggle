namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts
{
    public interface IFeatureToggleManager
    {
        Task<bool> IsEnabledAsync(string feature);
        Task<bool> IsEnabledAsync(string feature, string filterName);
        Task<bool> IsEnabledAsync<T>(string feature, string filterName, T toggleContext) where T: IFeatureToggleContext;
        Task<bool> IsEnabledByUserNameAsync(string feature, string userName);
        Task<bool> IsEnabledByRoleAsync(string feature, string role);
        IAsyncEnumerable<string> GetFeatureNamesAsync();
    }
}
