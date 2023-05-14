using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Dialog.Base;

using Discord;

namespace Devi.ServiceHosts.Discord.Dialog.Configuration;

/// <summary>
/// Server configuration
/// </summary>
public class ServerConfigurationDialogElement : DialogEmbedSelectMenuElementBase<bool>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public ServerConfigurationDialogElement(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region DialogSelectMenuElementBase

    /// <summary>
    /// Return the message of element
    /// </summary>
    /// <returns>Message</returns>
    public override Task<EmbedBuilder> GetMessage()
    {
        var builder = new EmbedBuilder().WithTitle(LocalizationGroup.GetText("Title", "Server configuration"))
                                        .WithDescription(LocalizationGroup.GetText("Description", "With the following assistant you are able to configure your server."))
                                        .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64")
                                        .WithColor(Color.Green)
                                        .WithTimestamp(DateTime.Now);

        return Task.FromResult(builder);
    }

    /// <summary>
    /// Returning the placeholder
    /// </summary>
    /// <returns>Placeholder</returns>
    public override string GetPlaceholder() => LocalizationGroup.GetText("ChooseAction", "Choose one of the following options...");

    /// <summary>
    /// Returns the select menu entries which should be added to the message
    /// </summary>
    /// <returns>Reactions</returns>
    public override IReadOnlyList<SelectMenuEntryData<bool>> GetEntries()
    {
        return new List<SelectMenuEntryData<bool>>
               {
                   new()
                   {
                       CommandText = LocalizationGroup.GetText("InstallCommands", "Command installation"),
                       Response = async () =>
                              {
                                  IEnumerable<ApplicationCommandProperties> commands = null;

                                  var buildContext = new SlashCommandBuildContext
                                                     {
                                                         Guild = CommandContext.Guild,
                                                         ServiceProvider = CommandContext.ServiceProvider,
                                                         CultureInfo = LocalizationGroup.CultureInfo
                                                     };

                                  foreach (var type in Assembly.Load("Devi.ServiceHosts.Discord")
                                                               .GetTypes()
                                                               .Where(obj => typeof(SlashCommandModuleBase).IsAssignableFrom(obj)
                                                                          && obj.IsAbstract == false))
                                  {
                                      var commandModule = (SlashCommandModuleBase)Activator.CreateInstance(type);
                                      if (commandModule != null)
                                      {
                                          commands = commands == null
                                                         ? commandModule.GetCommands(buildContext)
                                                         : commands.Concat(commandModule.GetCommands(buildContext));
                                      }
                                  }

                                  foreach (var type in Assembly.Load("Devi.ServiceHosts.Discord")
                                                               .GetTypes()
                                                               .Where(obj => typeof(MessageCommandModuleBase).IsAssignableFrom(obj)
                                                                          && obj.IsAbstract == false))
                                  {
                                      var commandModule = (MessageCommandModuleBase)Activator.CreateInstance(type);
                                      if (commandModule != null)
                                      {
                                          commands = commands == null
                                                         ? commandModule.GetCommands(buildContext)
                                                         : commands.Concat(commandModule.GetCommands(buildContext));
                                      }
                                  }

                                  if (commands != null)
                                  {
                                      await CommandContext.Guild
                                                          .BulkOverwriteApplicationCommandsAsync(commands.ToArray())
                                                          .ConfigureAwait(false);
                                  }

                                  return true;
                              }
                   },
                   new()
                   {
                       CommandText = LocalizationGroup.GetText("UninstallCommands", "Command uninstallation"),
                       Response = async () =>
                              {
                                  await CommandContext.Guild
                                                      .BulkOverwriteApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>())
                                                      .ConfigureAwait(false);

                                  return true;
                              }
                   }
               };
    }

    /// <summary>
    /// Default case if none of the given buttons is used
    /// </summary>
    /// <returns>Result</returns>
    protected override bool DefaultFunc() => false;

    #endregion // DialogSelectMenuElementBase<bool>
}