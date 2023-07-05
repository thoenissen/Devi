// <copyright file="LocatedEventQueueSubscriber.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using Devi.EventQueue.Configurations;
using Devi.ServiceHosts.Core.Localization;

namespace Devi.EventQueue.Core
{
    /// <summary>
    /// Event queue subscriber
    /// </summary>
    /// <typeparam name="TConfiguration">Configuration</typeparam>
    /// <typeparam name="TData">Data</typeparam>
    public abstract class LocatedEventQueueSubscriber<TConfiguration, TData> : EventQueueSubscriber<TConfiguration, TData>
        where TConfiguration : EventQueueConfiguration<TData>, new()
        where TData : class
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizationService">Localization service</param>
        protected LocatedEventQueueSubscriber(LocalizationService localizationService)
        {
            LocalizationGroup = localizationService.GetGroup(GetType().Name);
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Localization group
        /// </summary>
        public LocalizationGroup LocalizationGroup { get; }

        #endregion // Properties
    }
}