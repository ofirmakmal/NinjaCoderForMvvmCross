﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Defines the TextTransformationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Scorchio.VisualStudio.Services
{
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using Scorchio.VisualStudio.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    ///  Defines the TextTransformationService type.
    /// </summary>
    public class TextTransformationService : ITextTransformationService
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The text templating engine host.
        /// </summary>
        private readonly ITextTemplatingEngineHost textTemplatingEngineHost;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly T4Cache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTransformationService" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="textTemplatingEngineHost">The text templating engine host.</param>
        public TextTransformationService(
            IServiceProvider serviceProvider,
            ITextTemplatingEngineHost textTemplatingEngineHost)
        {
            TraceService.WriteLine("TextTransformationService::Constructor");

            this.serviceProvider = serviceProvider;
            this.textTemplatingEngineHost = textTemplatingEngineHost;

            this.cache = new T4Cache();

            TraceService.WriteLine("TextTransformationService::Constructor END");
        }

        /// <summary>
        /// Gets or sets the t4 call back.
        /// </summary>
        public T4CallBack T4CallBack { get; private set; }

        /// <summary>
        /// Transforms the specified source file.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public string Transform(
            string sourceFile,
            IDictionary<string, string> parameters)
        {
            TraceService.WriteLine("TextTransformationService::Transform sourceFile=" + sourceFile);

            string sourceText = this.GetText(sourceFile);

            ServiceProvider serviceProviderImpl = new ServiceProvider(this.serviceProvider, true);

            ITextTemplating t4 = serviceProviderImpl.GetService(typeof(STextTemplating)) as ITextTemplating;

            ITextTemplatingSessionHost sessionHost = t4 as ITextTemplatingSessionHost;

            if (sessionHost != null)
            {
                sessionHost.Session = sessionHost.CreateSession();

                foreach (KeyValuePair<string, string> keyValuePair in parameters)
                {
                    TraceService.WriteLine(keyValuePair.Key + "=" + keyValuePair.Value);

                    sessionHost.Session[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if (t4 != null)
            {
                this.T4CallBack = new T4CallBack();

                try
                {
                    TraceService.WriteLine("Before processing template");

                    string output;

                    if (this.textTemplatingEngineHost != null)
                    {
                        Engine engine = new Engine();

                        output = engine.ProcessTemplate(sourceFile, this.textTemplatingEngineHost);
                    }

                    else
                    {
                        output = t4.ProcessTemplate(sourceFile, sourceText, this.T4CallBack);
                    }

                    TraceService.WriteLine("After processing template");

                    return output;
                }
                catch (Exception exception)
                {
                    TraceService.WriteError(exception.Message);
                }
            }

            foreach (string errorMessage in this.T4CallBack.ErrorMessages)
            {
                TraceService.WriteLine("T4 Error " + errorMessage);
            }

            TraceService.WriteLine("TextTransformationService::Transform END");

            return string.Empty;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <returns></returns>
        internal string GetText(string sourceFile)
        {
            if (this.cache.Files.ContainsKey(sourceFile))
            {
                string cachedFile = this.cache.Files[sourceFile];

                TraceService.WriteLine("Using cached version of " + sourceFile);
                return cachedFile;
            }

            if (sourceFile.Contains("http") == false)
            {
                return File.ReadAllText(sourceFile);
            }

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(sourceFile);

            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }

            return string.Empty;
        }
    }
}
