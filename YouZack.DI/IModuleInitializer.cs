using Microsoft.Extensions.DependencyInjection;

namespace Infrastructures.DI
{
    /// <summary>
    /// IModuleInitializer还是有必要的，不仅仅是注册DbContext用，还可能会注册Respository用的一些其他类，比如HttpClientFactory等
    /// </summary>
    public interface IModuleInitializer
    {
        public void Initialize(IServiceCollection services);
    }
}
