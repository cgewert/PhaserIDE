using Microsoft.Extensions.DependencyInjection;

public static class ViewModelService
{
    private static IServiceProvider _serviceProvider;

    public static void Init(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T InstanceOf<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ViewModelService has not been initialized.");

        return _serviceProvider.GetRequiredService<T>();
    }

    public static bool TryInstanceOf<T>() where T : notnull
    {
        if (_serviceProvider == null)
            return false;

        try
        {
            _serviceProvider.GetRequiredService<T>();
            return true;
        }
        catch
        {
            return false;
        }
    }

}
