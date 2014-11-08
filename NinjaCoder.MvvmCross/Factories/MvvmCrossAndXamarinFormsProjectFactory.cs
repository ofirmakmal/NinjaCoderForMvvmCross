﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the MvvmCrossAndXamarinFormsProjectFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NinjaCoder.MvvmCross.Factories
{
    using NinjaCoder.MvvmCross.Entities;
    using NinjaCoder.MvvmCross.Factories.Interfaces;
    using NinjaCoder.MvvmCross.Services.Interfaces;
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.VisualStudio.Entities;
    using System.Collections.Generic;

    /// <summary>
    ///  Defines the MvvmCrossAndXamarinFormsProjectFactory type. 
    /// </summary>
    public class MvvmCrossAndXamarinFormsProjectFactory : IMvvmCrossAndXamarinFormsProjectFactory
    {
        /// <summary>
        /// The visual studio service.
        /// </summary>
        private readonly IVisualStudioService visualStudioService;

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// The nuget commands service.
        /// </summary>
        private readonly INugetCommandsService nugetCommandsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamarinFormsProjectFactory"/> class.
        /// </summary>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="nugetCommandsService">The nuget commands service.</param>
        public MvvmCrossAndXamarinFormsProjectFactory(
            IVisualStudioService visualStudioService,
            ISettingsService settingsService,
            INugetCommandsService nugetCommandsService)
        {
            this.visualStudioService = visualStudioService;
            this.settingsService = settingsService;
            this.nugetCommandsService = nugetCommandsService;
        }

        /// <summary>
        /// Gets the allowed projects.
        /// </summary>
        /// <returns>
        /// The allowed projects.
        /// </returns>
        public IEnumerable<ProjectTemplateInfo> GetAllowedProjects()
        {
            List<ProjectTemplateInfo> projectInfos = new List<ProjectTemplateInfo>();

            if (this.visualStudioService.iOSProjectService == null)
            {
                projectInfos.Add(this.GetiOSProject());
            }

            return projectInfos;;
        }

        /// <summary>
        /// Gets the ios project.
        /// </summary>
        /// <returns>an iOS project.</returns>
        internal ProjectTemplateInfo GetiOSProject()
        {
            return new ProjectTemplateInfo
            {
                FriendlyName = ProjectType.iOS.GetDescription() + " " + this.settingsService.iOSBuildVersion,
                ProjectSuffix = ProjectSuffix.iOS.GetDescription(),
                TemplateName = MvvmCrossAndXmarinFormsProjectTemplates.iOS.GetDescription(),
                PreSelected = true,
                ReferenceCoreProject = true,
                ReferenceXamarinFormsProject = true,
                NugetCommands = this.nugetCommandsService.GetXamarinFormsCommands()
            };
        }
    }
}

