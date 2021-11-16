namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines
{
    using Arguments;
    using Microsoft.Extensions.Logging;
    using Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public class ReProcessProblemOrdersPipeline : CommercePipeline<ReProcessProblemOrdersArgument, ReProcessProblemOrdersResult>, IReProcessProblemOrdersPipeline
    {
        public ReProcessProblemOrdersPipeline(IPipelineConfiguration<IReProcessProblemOrdersPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
