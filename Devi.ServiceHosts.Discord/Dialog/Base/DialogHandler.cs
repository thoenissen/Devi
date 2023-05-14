using System;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Dialog.Base;

/// <summary>
/// Handling dialog elements
/// </summary>
public sealed class DialogHandler : IDisposable
{
    #region Fields

    /// <summary>
    /// Command context
    /// </summary>
    private InteractionContextContainer _commandContext;

    /// <summary>
    /// Service provider
    /// </summary>
    private IServiceProviderContainer _serviceProvider;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commandContext">Command context</param>
    public DialogHandler(InteractionContextContainer commandContext)
    {
        _commandContext = commandContext;
        _serviceProvider = ServiceProviderFactory.Create();

        DialogContext = new DialogContext();
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Dialog context
    /// </summary>
    public DialogContext DialogContext { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <typeparam name="T">Type of the element</typeparam>
    /// <typeparam name="TData">Type of the element result</typeparam>
    /// <returns>Result</returns>
    public async Task<TData> Run<T, TData>() where T : DialogElementBase<TData>
    {
        var element = _serviceProvider.GetService<T>();

        element.Initialize(_commandContext, _serviceProvider, DialogContext);

        return await element.Run()
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <typeparam name="T">Type of the element</typeparam>
    /// <typeparam name="TData">Type of the element result</typeparam>
    /// <param name="element">Dialog element</param>
    /// <returns>Result</returns>
    public async Task<TData> Run<T, TData>(T element) where T : DialogElementBase<TData>
    {
        element.Initialize(_commandContext, _serviceProvider, DialogContext);

        return await element.Run()
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Execution one dialog element
    /// </summary>
    /// <typeparam name="TData">Type of the element result</typeparam>
    /// <returns>Result</returns>
    public async Task<TData> RunForm<TData>() where TData : new()
    {
        var data = new TData();

        foreach (var property in data.GetType()
                                     .GetProperties())
        {
            var attribute = property.GetCustomAttributes(typeof(DialogElementAssignmentAttribute), false)
                                    .OfType<DialogElementAssignmentAttribute>()
                                    .FirstOrDefault();

            if (attribute != null)
            {
                var service = (DialogElementBase)_serviceProvider.GetService(attribute.DialogElementType);

                service.Initialize(_commandContext, _serviceProvider, DialogContext);

                property.SetValue(data,
                                  await service.InternalRun()
                                               .ConfigureAwait(false));
            }
        }

        return data;
    }

    /// <summary>
    /// Deletes all messages
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task DeleteMessages()
    {
        if (_commandContext.Channel is ITextChannel textChannel)
        {
            await textChannel.DeleteMessagesAsync(DialogContext.Messages)
                             .ConfigureAwait(false);
        }
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }

    #endregion // IDisposable
}