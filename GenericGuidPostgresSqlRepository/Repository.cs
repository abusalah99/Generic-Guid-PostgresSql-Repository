namespace GenericGuidPostgresSqlRepository;

internal sealed class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : BaseEntity where TContext : DbContext
{
    private readonly TContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default, bool spiltQuery = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (additionalQuery != null)
            query = additionalQuery(query);

        if (spiltQuery)
            query = query.AsSplitQuery();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id,
        Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
        CancellationToken cancellationToken = default, bool spiltQuery = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (additionalQuery != null)
            query = additionalQuery(query);

        if (spiltQuery)
            query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default, bool spiltQuery = false)
    {
        IQueryable<TEntity> query = _dbSet.Where(filter);

        if (additionalQuery != null)
            query = additionalQuery(query);

        if (spiltQuery)
            query = query.AsSplitQuery();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<TResult>> SelectListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (additionalQuery != null)
            query = additionalQuery(query);

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<TResult?> GetSinglePropertyValueAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(filter, cancellationToken);
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;

        await _dbSet.AddAsync(entity, cancellationToken);
    }
    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Task.Run(() => _dbSet.Update(entity), cancellationToken);
    }
    public async Task<List<TResult>> GetListFromRawSqlAsync<TResult>(
        string tableName = nameof(TResult),
        string columnName = null!,
        string? condition = null,
        Dictionary<string, string>? additionalQueries = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        var queryBuilder = new StringBuilder(@$"SELECT t.""{columnName}"" FROM public.""{tableName}"" AS t");
            
        if (!string.IsNullOrWhiteSpace(condition))
            queryBuilder.Append($" WHERE {condition}");
            
        if(additionalQueries != null && additionalQueries.Any())
            foreach (var additionalQuery in additionalQueries)
                queryBuilder.Append($" {additionalQuery.Key.ToUpperInvariant()} {additionalQuery.Value}");
            
        if (skip.HasValue || take.HasValue)
        {
            skip ??= 0;
            queryBuilder.Append($" OFFSET {skip.Value}");
            if (take.HasValue)
                queryBuilder.Append($" LIMIT {take.Value}");
        }
            
        FormattableString query = FormattableStringFactory.Create(queryBuilder.ToString());

        return await _context.Database
            .SqlQuery<TResult>(query)
            .ToListAsync(cancellationToken);
    }
        
    public async Task<int> GetTotalCountFromRawSqlAsync(
        string tableName = null!,
        string? condition = null,
        Dictionary<string, string>? additionalQueries = null,
        CancellationToken cancellationToken = default)
    {
        var queryBuilder = new StringBuilder(@$"SELECT t.""Id"" FROM public.""{tableName}"" AS t");
            
        if (!string.IsNullOrWhiteSpace(condition))
            queryBuilder.Append($" WHERE {condition}");
            
        if(additionalQueries != null && additionalQueries.Any())
            foreach (var additionalQuery in additionalQueries)
                queryBuilder.Append($" {additionalQuery.Key.ToUpperInvariant()} {additionalQuery.Value}");
            
        FormattableString query = FormattableStringFactory.Create(queryBuilder.ToString());

        return (await _context.Database
            .SqlQuery<Guid>(query)
            .ToListAsync(cancellationToken)).Count;
    }
}