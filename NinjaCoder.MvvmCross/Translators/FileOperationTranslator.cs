﻿// --------------------------------- -----------------------------------------------------------------------------------
// <summary>
//    Defines the FileOperationTranslator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NinjaCoder.MvvmCross.Translators
{
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.Infrastructure.Translators;
    using System.Xml.Linq;

    using Scorchio.VisualStudio.Entities;

    /// <summary>
    /// Defines the FileOperationTranslator type.
    /// </summary>
    public class FileOperationTranslator : ITranslator<XElement, FileOperation>
    {
        /// <summary>
        /// Translates the specified from.
        /// </summary>
        /// <param name="from">The Source..</param>
        /// <returns>A File Operation.</returns>
        public FileOperation Translate(XElement @from)
        {
            return new FileOperation
            {
                PlatForm = @from.GetSafeAttributeStringValue("Platform"),
                CommandType = @from.GetSafeAttributeStringValue("Type"),
                Name = @from.GetSafeAttributeStringValue("Name"),
                File = @from.GetSafeAttributeStringValue("File"),
                Directory = @from.GetSafeAttributeStringValue("Directory"),
                From = @from.GetSafeAttributeStringValue("From"),
                To = @from.GetSafeAttributeStringValue("To")
            };
        }
    }
}
