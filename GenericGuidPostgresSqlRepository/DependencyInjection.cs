namespace GenericGuidPostgresSqlRepository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped(typeof(IRepository<>), provider =>
        {
            var context = provider.GetRequiredService<TContext>();
            var repositoryType = typeof(Repository<,>).MakeGenericType(typeof(BaseEntity), typeof(TContext));
            return Activator.CreateInstance(repositoryType, context)!;
        });

        return services;
    }
}