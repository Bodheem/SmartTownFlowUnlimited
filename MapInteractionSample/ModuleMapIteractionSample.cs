using System;
using Genetec.Sdk.Workspace;
using Module = Genetec.Sdk.Workspace.Modules.Module;
using Genetec.Sdk.Workspace.Services;
using Genetec.Sdk.Workspace.Tasks;
using System.Collections.Generic;
using System.Windows.Threading;

// ==========================================================================
// Copyright (C) 2017 by Genetec, Inc.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// ==========================================================================
namespace MapInteractionSample
{
    /// <summary>
    /// This class is used to register your module in the system.
    /// </summary>
    public sealed class ModuleMapIteractionSample : Module
    {
        #region Constants

        private readonly List<Task> m_tasks = new List<Task>();

        #endregion

        #region Fields

        private FireMapObjectProvider m_fireMapObjectProvider;

        private DispatcherTimer m_timer;

        public static DateTime selecteDate;


        #endregion

        #region Event Handlers

        private void OnWorkspaceInitialized(object sender, InitializedEventArgs e)
        {
            var mapService = Workspace.Services.Get<IMapService>();
            if (mapService != null)
            {
                // Add a layer to the maps to be able to add the Fires on a specific layer.
                mapService.RegisterLayer(new LayerDescriptor(FireMapObject.FiresLayerId, FireMapObject.FireLayerName));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the module in the workspace and register it's workspace extensions and shared components
        /// </summary>
        public override void Load()
        {
            SubscribeToWorkspaceEvents();

            RegisterTaskExtensions();

            // Register the provider
            m_fireMapObjectProvider = new FireMapObjectProvider();
            m_fireMapObjectProvider.Initialize(Workspace);
            Workspace.Components.Register(m_fireMapObjectProvider);
        }

        /// <summary>
        /// Unloads the module in the workspace by unregistering it's workspace extensions and shared components
        /// </summary>
        public override void Unload()
        {
            if (Workspace != null)
            {
                m_timer.Stop();

                UnregisterTaskExtensions();

                UnsubscribeFromWorkspaceEvents();

                if (m_fireMapObjectProvider != null)
                {
                    Workspace.Components.Unregister(m_fireMapObjectProvider);
                    m_fireMapObjectProvider = null;
                }

            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Here you can register your custom tasks and task group.
        /// </summary>
        private void RegisterTaskExtensions()
        {
            // Register task extensions
            Task task = new CreatePageTask<MapFireAnalysisPage>();
            task.Initialize(Workspace);
            m_tasks.Add(task);

            // Register them to the workspace
            foreach (Task pageExtension in m_tasks)
            {
                Workspace.Tasks.Register(pageExtension);
            }
        }

        private void SubscribeToWorkspaceEvents()
        {
            if (Workspace != null)
            {
                Workspace.Initialized += OnWorkspaceInitialized;
            }
        }

        private void UnregisterTaskExtensions()
        {
            // Unregister them from the workspace
            foreach (Task task in m_tasks)
            {
                Workspace.Tasks.Unregister(task);
            }

            m_tasks.Clear();
        }

        private void UnsubscribeFromWorkspaceEvents()
        {
            if (Workspace != null)
            {
                Workspace.Initialized -= OnWorkspaceInitialized;
            }
        }

        #endregion
    }

}