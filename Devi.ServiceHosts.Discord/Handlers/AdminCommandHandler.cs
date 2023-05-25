using System;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.Docker;

using Discord;

using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Administration commands
/// </summary>
public class AdminCommandHandler : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Connector</param>
    public AdminCommandHandler(LocalizationService localizationService,
                               WebApiConnector connector)
        : base(localizationService)
    {
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Show docker command assistant
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task ShowDockerAssistant(InteractionContextContainer context)
    {
        var messageDataTask = GetDockerContainerOverview(context.Guild.Id);

        await context.DeferAsync()
                            .ConfigureAwait(false);

        var (embed, components) = await messageDataTask.ConfigureAwait(false);

        await context.ReplyAsync(embed: embed.Build(),
                                 components: components.Build())
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Show modal to create a new container
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CreateNewContainer(InteractionContextContainer context)
    {
        await context.RespondWithModalAsync<CreateDockerContainerModalData>("modal;admin;docker;create")
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Create a new container with the given modal data
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CreateNewContainer(InteractionContextContainer context, CreateDockerContainerModalData data)
    {
        await context.DeferAsync()
                     .ConfigureAwait(false);

        await _connector.Docker
                        .AddOrRefreshContainer(context.Guild.Id,
                                               new DockerContainerDTO
                                               {
                                                   Name = data.Name,
                                                   Description = data.Description,
                                               })
                        .ConfigureAwait(false);

        var (embed, components) = await GetDockerContainerOverview(context.Guild.Id).ConfigureAwait(false);

        await context.ModifyOriginalResponseAsync(obj =>
                                                  {
                                                      obj.Embed = embed.Build();
                                                      obj.Components = components.Build();
                                                  })
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Show information of container
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="containerName">Container name</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public Task ShowSelectContainer(InteractionContextContainer context, string containerName) => Task.CompletedTask;

    /// <summary>
    /// Get overview of existing docker containers
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    private async Task<(EmbedBuilder Embed, ComponentBuilder Components)> GetDockerContainerOverview(ulong serverId)
    {
        var containers = await _connector.Docker
                                         .GetDockerContainers(serverId)
                                         .ConfigureAwait(false);

        var embed = new EmbedBuilder().WithTitle(LocalizationGroup.GetText("DockerTitle", "Docker assistant"))
                                      .WithTimestamp(DateTimeOffset.Now)
                                      .WithThumbnailUrl("https://cdn.discordapp.com/attachments/1111028091784019990/1111028127624331285/Moby-logo.png")
                                      .WithDescription(LocalizationGroup.GetText("DockerDescription", "With this assistant you are to administrate your docker containers."))
                                      .AddField(LocalizationGroup.GetText("DockerContainers", "Containers"), $"{string.Join(Environment.NewLine, containers.Select(obj => $"{(obj.IsOnline ? "🟢" : "🔴")} {obj.Description} ({obj.Name})"))}\u200b")
                                      .WithColor(Color.Blue)
                                      .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64");

        var components = new ComponentBuilder().WithButton(LocalizationGroup.GetText("DockerCreate", "Create"), "admin;docker;create", ButtonStyle.Success)
                                               .WithSelectMenu(new SelectMenuBuilder("admin;docker;selectContainer", containers.Select(obj => new SelectMenuOptionBuilder().WithValue(obj.Name).WithLabel(obj.Description)).ToList()));

        return (embed, components);
    }

    #endregion // Methods
}