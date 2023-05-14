﻿using System;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.WebApi.Jobs.Base;

using FluentScheduler;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.WebApi.Services;

/// <summary>
/// Scheduling jobs
/// </summary>
public sealed class JobScheduler : LocatedSingletonServiceBase,
                                   IAsyncDisposable,
                                   IJobFactory
{
    #region Methods

    /// <summary>
    /// Starting the job server
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async Task StartAsync()
    {
        await Task.Run(JobManager.Start).ConfigureAwait(false);
    }

    /// <summary>
    /// Add a job
    /// </summary>
    /// <param name="job">Job</param>
    /// <param name="timeStamp">Time stamp to run the job</param>
    /// <returns>Name of the added job</returns>
    public string AddJob(IJob job, DateTime timeStamp)
    {
        var jobName = Guid.NewGuid().ToString();

        JobManager.AddJob(job, obj => obj.WithName(jobName).ToRunOnceAt(timeStamp));

        return jobName;
    }

    #endregion // Methods

    #region SingletonLocatedServiceBase

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <remarks>When this method is called all services are registered and can be resolved.  But not all singleton services may be initialized. </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task Initialize(IServiceProvider serviceProvider)
    {
        await base.Initialize(serviceProvider)
                  .ConfigureAwait(false);

        JobManager.JobFactory = this;
        JobManager.Initialize();
    }

    #endregion // SingletonLocatedServiceBase

    #region IJobFactory

    /// <summary>
    /// Instantiate a job of the given type.
    /// </summary>
    /// <typeparam name="T">Type of the job to instantiate</typeparam>
    /// <returns>The instantiated job</returns>
    public IJob GetJobInstance<T>()
        where T : IJob
    {
        var scope = ServiceProviderFactory.Create();

        var job = scope.GetRequiredService<T>();
        if (job is IServiceScopeSupport scopeSupport)
        {
            scopeSupport.SetScope(scope);
        }

        return job;
    }

    #endregion //IJobFactory

    #region IAsyncDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns> A task that represents the asynchronous dispose operation.</returns>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await Task.Run(JobManager.StopAndBlock).ConfigureAwait(false);
    }

    #endregion // IAsyncDisposable
}