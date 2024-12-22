namespace GenericGuidPostgresSqlRepository
{
    /// <summary>
    /// Represents a generic repository interface for CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the repository will handle.</typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Gets a list of entities from the database asynchronously, with optional filtering and additional query options.
        /// </summary>
        /// <param name="filter">An optional filter expression to apply to the query.</param>
        /// <param name="additionalQuery">An optional function to further modify the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <param name="spiltQuery">Whether to use split query execution for large data sets.</param>
        /// <returns>A task representing the asynchronous operation, with a list of entities as the result.</returns>
        Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
            CancellationToken cancellationToken = default, 
            bool spiltQuery = false);

        /// <summary>
        /// Gets a single entity matching the given filter expression.
        /// </summary>
        /// <param name="filter">A filter expression to identify the entity.</param>
        /// <param name="additionalQuery">An optional function to further modify the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <param name="spiltQuery">Whether to use split query execution for large data sets.</param>
        /// <returns>A task representing the asynchronous operation, with the entity or null as the result.</returns>
        Task<TEntity?> GetSingleAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
            CancellationToken cancellationToken = default, 
            bool spiltQuery = false);

        /// <summary>
        /// Gets an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="filter">An optional filter expression to apply to the query.</param>
        /// <param name="additionalQuery">An optional function to further modify the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <param name="spiltQuery">Whether to use split query execution for large data sets.</param>
        /// <returns>A task representing the asynchronous operation, with the entity or null as the result.</returns>
        Task<TEntity?> GetByIdAsync(
            Guid id, 
            Expression<Func<TEntity, bool>>? filter = null, 
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null,
            CancellationToken cancellationToken = default, 
            bool spiltQuery = false);

        /// <summary>
        /// Gets the total count of entities in the database.
        /// </summary>
        /// <param name="filter">An optional filter expression to apply to the count operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with the total count as the result.</returns>
        Task<int> GetCountAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Selects a list of entities with a specified projection.
        /// </summary>
        /// <typeparam name="TResult">The type to project the entities into.</typeparam>
        /// <param name="selector">The projection expression to map the entity to another type.</param>
        /// <param name="filter">An optional filter expression to apply to the query.</param>
        /// <param name="additionalQuery">An optional function to further modify the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with a list of the selected entities as the result.</returns>
        Task<List<TResult>> SelectListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? additionalQuery = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single property value from an entity matching the given filter.
        /// </summary>
        /// <typeparam name="TResult">The type of the property value to be retrieved.</typeparam>
        /// <param name="selector">The expression to select the property value.</param>
        /// <param name="filter">An optional filter expression to apply to the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with the selected property value or null as the result.</returns>
        Task<TResult?> GetSinglePropertyValueAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? filter = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to check for existence.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating existence.</returns>
        Task<bool> IsExistAsync(
            Expression<Func<TEntity, bool>> filter, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a raw SQL query to retrieve a list of entities.
        /// </summary>
        /// <typeparam name="TResult">The result type of the query.</typeparam>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="columnName">The name of the column to select.</param>
        /// <param name="condition">An optional condition for the query.</param>
        /// <param name="additionalQueries">Additional SQL queries to modify the query.</param>
        /// <param name="skip">The number of records to skip (for pagination).</param>
        /// <param name="take">The number of records to take (for pagination).</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with a list of the query results as the result.</returns>
        Task<List<TResult>> GetListFromRawSqlAsync<TResult>(
            string tableName = nameof(TResult),
            string columnName = null!,
            string? condition = null,
            Dictionary<string, string>? additionalQueries = null,
            int? skip = null,
            int? take = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of records from a raw SQL query.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="condition">An optional condition for the query.</param>
        /// <param name="additionalQueries">Additional SQL queries to modify the query.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, with the total count as the result.</returns>
        Task<int> GetTotalCountFromRawSqlAsync(
            string tableName = null!,
            string? condition = null,
            Dictionary<string, string>? additionalQueries = null,
            CancellationToken cancellationToken = default);
    }
}
