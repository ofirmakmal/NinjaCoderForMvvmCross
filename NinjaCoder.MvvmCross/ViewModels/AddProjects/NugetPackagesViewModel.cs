﻿// ------------------------------------- -------------------------------------------------------------------------------
// <summary>
//    Defines the NugetPackagesViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.ViewModels.AddProjects
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NinjaCoder.MvvmCross.Entities;
    using NinjaCoder.MvvmCross.Factories.Interfaces;
    using NinjaCoder.MvvmCross.Services.Interfaces;

    using Scorchio.Infrastructure.Wpf.ViewModels;
    using Scorchio.Infrastructure.Wpf.ViewModels.Wizard;
    using Scorchio.VisualStudio.Services;

    /// <summary>
    ///  Defines the NugetPackagesViewModel type.
    /// </summary>
    public class NugetPackagesViewModel : BaseWizardStepViewModel
    {
        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// The plugin factory.
        /// </summary>
        private readonly IPluginFactory pluginFactory;

        /// <summary>
        /// The nuget packages.
        /// </summary>
        private ObservableCollection<SelectableItemViewModel<Plugin>> nugetPackages;

        /// <summary>
        /// Initializes a new instance of the <see cref="NugetPackagesViewModel" /> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="pluginFactory">The plugin factory.</param>
        public NugetPackagesViewModel(
            ISettingsService settingsService,
            IPluginFactory pluginFactory)
        {
            TraceService.WriteLine("NugetServicesViewModel::Constructor Start");

            this.settingsService = settingsService;
            this.pluginFactory = pluginFactory;

            TraceService.WriteLine("NugetServicesViewModel::Constructor End");
        }

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        public override void OnInitialize()
        {
            Plugins allPackages = this.pluginFactory.GetPlugins(this.settingsService.NugetPackagesUri);
            this.NugetPackages = this.GetPackages(allPackages);
        }

        /// <summary>
        /// Gets the nuget packagess.
        /// </summary>
        public ObservableCollection<SelectableItemViewModel<Plugin>> NugetPackages
        {
            get { return this.nugetPackages; }
            set { this.SetProperty(ref this.nugetPackages, value); }
        }

        /// <summary>
        /// Gets the required packages.
        /// </summary>
        /// <returns>The required packages.</returns>
        public IEnumerable<Plugin> GetRequiredPackages()
        {
            TraceService.WriteLine("NugetPackagesViewModel::GetRequiredPackages");

            IEnumerable<SelectableItemViewModel<Plugin>> viewModels = this.NugetPackages;
                                                                
            return viewModels.ToList()
                .Where(x => x.IsSelected)
                .Select(x => x.Item).ToList();
        }

        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <param name="plugins">The plugins.</param>
        /// <returns></returns>
        internal ObservableCollection<SelectableItemViewModel<Plugin>> GetPackages(Plugins plugins)
        {
            ObservableCollection<SelectableItemViewModel<Plugin>> viewModels = new ObservableCollection<SelectableItemViewModel<Plugin>>();

            foreach (SelectableItemViewModel<Plugin> viewModel in from plugin in plugins.Items
                                                                  where plugin.Frameworks.Contains(this.settingsService.FrameworkType)
                                                                  select new SelectableItemViewModel<Plugin>(plugin))
            {
                viewModels.Add(viewModel);
            }

            return new ObservableCollection<SelectableItemViewModel<Plugin>>(viewModels.OrderBy(x => x.Item.FriendlyName));
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public override string DisplayName
        {
            get { return "Nuget Packages"; }
        }
    }
}
