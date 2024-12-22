namespace GenericGuidPostgresSqlRepository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<,>));

        return services;
    }
}