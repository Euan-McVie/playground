using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Playground.Infrastructure.Extensions.Swagger
{
    /// <summary>
    /// Extension methods to aid interop with <see cref="SwaggerGenOptions"/>.
    /// </summary>
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
        /// for all loaded assemblies that have a <c>*.xml</c> documentation file in the application's base directory.
        /// </summary>
        /// <param name="swaggerGenOptions">The <see cref="SwaggerGenOptions"/> to add the comments to.</param>
        /// <param name="includeControllerXmlComments">Flag to indicate if controller XML comments (i.e. summary) should be used to
        /// assign Tag descriptions. Don't set this flag if you're customizing the default tag for operations via TagActionsBy.</param>
        public static void IncludeAllXmlComments(this SwaggerGenOptions swaggerGenOptions, bool includeControllerXmlComments = false)
        {
            // Get comments files
            IEnumerable<string> commentsFiles = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.GlobalAssemblyCache && !ass.IsDynamic)
                .Select(ass => Path.Combine(AppContext.BaseDirectory, Path.ChangeExtension(ass.Location, ".xml")));
            foreach (string commentFile in commentsFiles)
            {
                if (File.Exists(commentFile))
                    swaggerGenOptions.IncludeXmlComments(commentFile, includeControllerXmlComments);
            }
        }
    }
}
