﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the ViewModelViewsService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Services
{
    using Constants;
    using Entities;
    using Factories.Interfaces;
    using Interfaces;
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.VisualStudio.Entities;
    using Scorchio.VisualStudio.Services;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///  Defines the ViewModelViewsService type.
    /// </summary>
    internal class ViewModelViewsService : BaseService, IViewModelViewsService
    {
        /// <summary>
        /// The visual studio service.
        /// </summary>
        private readonly IVisualStudioService visualStudioService;

        /// <summary>
        /// The view model and views factory.
        /// </summary>
        private readonly IViewModelAndViewsFactory viewModelAndViewsFactory;

        /// <summary>
        /// The file operation service.
        /// </summary>
        private readonly IFileOperationService fileOperationService;

        /// <summary>
        /// The nuget commands service.
        /// </summary>
        private readonly INugetCommandsService nugetCommandsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelViewsService" /> class.
        /// </summary>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="viewModelAndViewsFactory">The view model and views factory.</param>
        /// <param name="fileOperationService">The file operation service.</param>
        /// <param name="nugetCommandsService">The nuget commands service.</param>
        public ViewModelViewsService(
            IVisualStudioService visualStudioService,
            ISettingsService settingsService,
            IViewModelAndViewsFactory viewModelAndViewsFactory,
            IFileOperationService fileOperationService,
            INugetCommandsService nugetCommandsService)
            :base(settingsService)
        {
            TraceService.WriteLine("ViewModelViewsService::Constructor");

            this.visualStudioService = visualStudioService;
            this.viewModelAndViewsFactory = viewModelAndViewsFactory;
            this.fileOperationService = fileOperationService;
            this.nugetCommandsService = nugetCommandsService;
        }

        /// <summary>
        /// Adds the view model and views.
        /// </summary>
        /// <param name="textTemplateInfos">The text template infos.</param>
        /// <returns>
        /// The messages.
        /// </returns>
        public IEnumerable<string> AddViewModelAndViews(
            IEnumerable<TextTemplateInfo> textTemplateInfos)
        {
            TraceService.WriteLine("ViewModelViewsService::AddViewModelAndViews");

            List<string> messages = new List<string>();

            List<TextTemplateInfo> textTemplates = textTemplateInfos.ToList();

            messages.AddRange(this.visualStudioService.SolutionService.AddItemTemplateToProjects(textTemplates, this.SettingsService.OutputTextTemplateContentToTraceFile));

            //// now add any post action commands

            foreach (TextTemplateInfo textTemplateInfo in textTemplates)
            {
                if (textTemplateInfo.FileOperations != null)
                {
                    foreach (FileOperation fileOperation in textTemplateInfo.FileOperations)
                    {
                        this.fileOperationService.ProcessCommand(fileOperation);

                        foreach (TextTemplateInfo childTemplateInfo in textTemplateInfo.ChildItems)
                        {
                            foreach (FileOperation childFileOperation in childTemplateInfo.FileOperations)
                            {
                                this.fileOperationService.ProcessCommand(childFileOperation); 
                            }
                        }
                    }
                }
            }
            
            TraceService.WriteLine("ViewModelViewsService::AddViewModelAndViews END");

            return messages;
        }

        /// <summary>
        /// Adds the view models and views.
        /// </summary>
        /// <param name="views">The views.</param>
        /// <returns>A list of views.</returns>
        public IEnumerable<string> AddViewModelsAndViews(IEnumerable<View> views)
        {
            TraceService.WriteLine("ViewModelViewsService::AddViewModelsAndViews");

            List<string> messages = new List<string>();

            this.visualStudioService.WriteStatusBarMessage(NinjaMessages.AddingViewModelAndViews);
            
            if (this.SettingsService.FrameworkType == FrameworkType.MvvmCrossAndXamarinForms)
            {
                this.SettingsService.BindXamlForXamarinForms = true;
                this.SettingsService.BindContextInXamlForXamarinForms = false;
            }
            else
            {
                this.SettingsService.BindXamlForXamarinForms = true;
                this.SettingsService.BindContextInXamlForXamarinForms = true;
            }

            foreach (View view in views)
            {
                if (view.Existing == false)
                {
                    string viewModelName = view.Name + "ViewModel";

                    this.visualStudioService.WriteStatusBarMessage(NinjaMessages.AddingViewModelAndViews + " (" + viewModelName + ")");

                    IEnumerable<TextTemplateInfo> textTemplateInfos = this.viewModelAndViewsFactory.GetRequiredTextTemplates(
                            view,
                            viewModelName,
                            this.viewModelAndViewsFactory.AllowedUIViews,
                            this.visualStudioService.CoreTestsProjectService != null);

                    IEnumerable<string> viewModelMessages = this.AddViewModelAndViews(
                        textTemplateInfos);

                    messages.AddRange(viewModelMessages);
                }
            }

            TraceService.WriteLine("ViewModelViewsService::AddViewModelsAndViews END");

            return messages;
        }

        /// <summary>
        /// Gets the nuget commands.
        /// </summary>
        /// <returns>A list of nuget commands.</returns>
        public IEnumerable<string> GetNugetCommands()
        {
            List<string> commands = new List<string>();

            //// get the ios storyboard nuget command

            if (this.SettingsService.AddiOSProject &&
                this.SettingsService.SelectedMvvmCrossiOSViewType == MvvmCrossViewType.StoryBoard.GetDescription())
            {
                switch (this.SettingsService.FrameworkType)
                {
                    case FrameworkType.MvvmCross:
                    case FrameworkType.MvvmCrossAndXamarinForms:

                        string mvxCommand = this.nugetCommandsService.GetMvvmCrossIosStoryBoardCommand();
                        mvxCommand += " " + this.visualStudioService.iOSProjectService.Name;
                        commands.Add(mvxCommand);
                        break;
                }
            }

            return commands;
        }
    }
}