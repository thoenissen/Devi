﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Services.Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Dialog.Base;

/// <summary>
/// Dialog element
/// </summary>
public abstract class DialogElementBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    protected DialogElementBase(LocalizationService localizationService)
    {
        LocalizationGroup = localizationService.GetGroup(GetType().Name);
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Localization group
    /// </summary>
    public LocalizationGroup LocalizationGroup { get; }

    /// <summary>
    /// Command context
    /// </summary>
    public InteractionContextContainer CommandContext { get; private set; }

    /// <summary>
    /// Service provider
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Current dialog context
    /// </summary>
    public DialogContext DialogContext { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Initializing
    /// </summary>
    /// <param name="commandContext">Command context</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="dialogContext">Dialog context</param>
    internal void Initialize(InteractionContextContainer commandContext, IServiceProvider serviceProvider, DialogContext dialogContext)
    {
        CommandContext = commandContext;
        ServiceProvider = serviceProvider;
        DialogContext = dialogContext;
    }

    /// <summary>
    /// Execution of the element
    /// </summary>
    /// <returns>Result</returns>
    internal abstract Task<object> InternalRun();

    #endregion // Methods
}

/// <summary>
/// Dialog element
/// </summary>
/// <typeparam name="TData">Type of the result</typeparam>
public abstract class DialogElementBase<TData> : DialogElementBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    protected DialogElementBase(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Execution of the element
    /// </summary>
    /// <returns>Result</returns>
    internal sealed override async Task<object> InternalRun() => await Run().ConfigureAwait(false);

    /// <summary>
    /// Execution of the element
    /// </summary>
    /// <returns>Result</returns>
    public abstract Task<TData> Run();

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <typeparam name="T">Type of the element</typeparam>
    /// <typeparam name="TSubData">Type of the element result</typeparam>
    /// <returns>Result</returns>
    public async Task<TSubData> RunSubElement<T, TSubData>() where T : DialogElementBase<TSubData>
    {
        var service = ServiceProvider.GetService<T>();

        service.Initialize(CommandContext, ServiceProvider, DialogContext);

        return await service.Run()
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <param name="element">Element</param>
    /// <typeparam name="T">Type of the element</typeparam>
    /// <typeparam name="TSubData">Type of the element result</typeparam>
    /// <returns>Result</returns>
    public async Task<TSubData> RunSubElement<T, TSubData>(T element) where T : DialogElementBase<TSubData>
    {
        element.Initialize(CommandContext, ServiceProvider, DialogContext);

        return await element.Run()
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <typeparam name="TSubData">Type of the element result</typeparam>
    /// <returns>Result</returns>
    public async Task<TSubData> RunSubForm<TSubData>() where TSubData : new()
    {
        var data = new TSubData();

        foreach (var property in data.GetType()
                                     .GetProperties())
        {
            var attribute = property.GetCustomAttributes(typeof(DialogElementAssignmentAttribute), false)
                                    .OfType<DialogElementAssignmentAttribute>()
                                    .FirstOrDefault();

            if (attribute != null)
            {
                var service = (DialogElementBase)ServiceProvider.GetService(attribute.DialogElementType);

                service.Initialize(CommandContext, ServiceProvider, DialogContext);

                property.SetValue(data,
                                  await service.InternalRun()
                                               .ConfigureAwait(false));
            }
        }

        return data;
    }

    #endregion // Methods
}