﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Devi.ServiceHosts.WebApi.Data.Entity.Queryable.Base;

using Microsoft.EntityFrameworkCore;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Repositories.Base;

/// <summary>
/// Base class for creating a repository
/// </summary>
public abstract class RepositoryBase
{
}

/// <summary>
/// Base class for creating a repository
/// </summary>
/// <typeparam name="TQueryable">Queryable type</typeparam>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class RepositoryBase<TQueryable, TEntity> : RepositoryBase
    where TQueryable : QueryableBase<TEntity>
    where TEntity : class
{
    #region Fields

    /// <summary>
    /// Internal <see cref="Microsoft.EntityFrameworkCore.DbContext"/>-object
    /// </summary>
    private DbContext _dbContext;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"><see cref="Microsoft.EntityFrameworkCore.DbContext"/>-object</param>
    protected RepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Last error
    /// </summary>
    public Exception LastError
    {
        get => _dbContext.LastError;
        protected set => _dbContext.LastError = value;
    }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns a queryable object of the related entity
    /// </summary>
    /// <returns>Queryable object of the related entity</returns>
    public TQueryable GetQuery()
    {
        return (TQueryable)Activator.CreateInstance(typeof(TQueryable), _dbContext.Set<TEntity>().AsNoTracking());
    }

    /// <summary>
    /// Add a new entity object
    /// </summary>
    /// <param name="entity">Entity object</param>
    /// <returns>Is the operation performed successfully?</returns>
    public async Task<bool> Add(TEntity entity)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            _dbContext.Set<TEntity>()
                      .Add(entity);

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Add new entity objects
    /// </summary>
    /// <param name="entities">Entity objects</param>
    /// <returns>Is the operation performed successfully?</returns>
    public bool AddRange(IEnumerable<TEntity> entities)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            _dbContext.Set<TEntity>()
                      .AddRange(entities);

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Refresh a specific entity object or add it, if it doesn't exists
    /// </summary>
    /// <param name="expression">Defines the entity object to be refreshed</param>
    /// <param name="refreshAction">Action to be performed with the entity object</param>
    /// <param name="after">Action to be performed after the refresh/add-operation</param>
    /// <returns>Is the operation performed successfully?</returns>
    public bool AddOrRefresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction, Action<TEntity> after = null)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var newEntity = false;

            var entity = _dbContext.Set<TEntity>().FirstOrDefault(expression);
            if (entity == null)
            {
                entity = Activator.CreateInstance<TEntity>();

                newEntity = true;
            }

            refreshAction(entity);

            if (newEntity)
            {
                _dbContext.Set<TEntity>().Add(entity);
            }

            _dbContext.SaveChanges();

            after?.Invoke(entity);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Refresh a specific entity object
    /// </summary>
    /// <param name="expression">Defines the entity object to be refreshed</param>
    /// <param name="refreshAction">Action to be performed with the entity object</param>
    /// <returns>Is the operation performed successfully?</returns>
    public async Task<bool> Refresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var set = _dbContext.Set<TEntity>();

            var entity = await set.FirstAsync(expression)
                                  .ConfigureAwait(false);

            if (set.Local.Contains(entity) == false)
            {
                set.Attach(entity);
            }

            refreshAction(entity);

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Refresh a range of entity objects
    /// </summary>
    /// <param name="expression">Defines the entity objects to be refreshed</param>
    /// <param name="refreshAction">Action to be performed with the entity objects</param>
    /// <returns>Is the operation performed successfully?</returns>
    public bool RefreshRange(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            foreach (var entry in dbSet.Where(expression))
            {
                refreshAction(entry);
            }

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Refresh a range of entity objects
    /// </summary>
    /// <param name="expression">Defines the entity objects to be refreshed</param>
    /// <param name="refreshAction">Action to be performed with the entity objects</param>
    /// <returns>Is the operation performed successfully?</returns>
    public async Task<bool> RefreshRangeAsync(Expression<Func<TEntity, bool>> expression, Func<TEntity, Task> refreshAction)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            await foreach (var entry in dbSet.Where(expression).AsAsyncEnumerable())
            {
                await refreshAction(entry).ConfigureAwait(false);
            }

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Remove a entity object
    /// </summary>
    /// <param name="expression">Defines the entity object to be refreshed</param>
    /// <param name="beforeRemove">Action to be performed before the remove-operation</param>
    /// <returns>Is the operation performed successfully?</returns>
    public async Task<bool> Remove(Expression<Func<TEntity, bool>> expression, Action<TEntity> beforeRemove = null)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = await dbSet.FirstAsync(expression)
                                    .ConfigureAwait(false);

            beforeRemove?.Invoke(entity);

            dbSet.Remove(entity);

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Remove entity objects
    /// </summary>
    /// <param name="expression">Defines the entity objects to be refreshed</param>
    /// <returns>Is the operation performed successfully?</returns>
    public async Task<bool> RemoveRange(Expression<Func<TEntity, bool>> expression)
    {
        var success = false;

        _dbContext.LastError = null;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            foreach (var entry in await dbSet.Where(expression)
                                             .ToListAsync()
                                             .ConfigureAwait(false))
            {
                dbSet.Remove(entry);
            }

            await _dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            _dbContext.LastError = ex;
        }

        return success;
    }

    /// <summary>
    /// Returning the internal DbContext
    /// </summary>
    /// <returns>DbContext</returns>
    protected DbContext GetDbContext() => _dbContext;

    #endregion // Methods
}